using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;

namespace InputLog.Core.IO.HTML
{
    /// <summary>
    /// Converts XML files to HTML by applying XSL transformations.
    /// </summary>
    public static class XmlToHtmlTranslator
    {
        /// <summary>
        /// Transforms all XML files in the specified input folder by applying the corresponding XSL stylesheets.
        /// Input XMLs are moved into an \"xml\" subdirectory, and HTML outputs are written back to the input folder.
        /// Stylesheet files are expected under an \"Style\" subdirectory, and common.xsl inclusions are resolved automatically.
        /// </summary>
        /// <param name="inputFolder">Full path to the root folder containing XML files and Style directory.</param>
        public static void Transform(string inputFolder)
        {
            if (string.IsNullOrWhiteSpace(inputFolder))
                throw new ArgumentException("Input folder path must be provided.", nameof(inputFolder));

            inputFolder = inputFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (!Directory.Exists(inputFolder))
                throw new DirectoryNotFoundException($"Input folder not found: {inputFolder}");

            string xmlDir = Path.Combine(inputFolder, "xml");
            Directory.CreateDirectory(xmlDir);

            var scriptsDir = Path.Combine(inputFolder, "Scripts");
            var imagesDir = Path.Combine(inputFolder, "Images");
            var styleDir = Path.Combine(inputFolder, "Style");
            var dirInfoList = new List<string> { scriptsDir, imagesDir, styleDir };

            var cssFile = Path.Combine(styleDir, "common.css");
            if (!File.Exists(cssFile))
                throw new FileNotFoundException("Shared CSS not found.", cssFile);

            MoveXmlFiles(inputFolder, xmlDir);
            MoveStylingFolders(xmlDir, dirInfoList);

            var analysisMap = CreateAnalysisMap();

            ProcessXmlFiles(xmlDir, styleDir, analysisMap, cssFile);
        }

        /// <summary>
        /// Moves all .xml files from the root input folder into the xml subdirectory.
        /// </summary>
        private static void MoveXmlFiles(string sourceDir, string targetDir)
        {
            foreach (var file in Directory.GetFiles(sourceDir, "*.xml"))
            {
                var destPath = Path.Combine(targetDir, Path.GetFileName(file));
                if (!File.Exists(destPath))
                {
                    File.Move(file, destPath);
                }
                //if file exists, ignore
            }
        }

        /// <summary>
        /// Moves styling folders from the root input folder into the xml subdirectory.
        /// </summary>
        private static void MoveStylingFolders(string targetDir, List<string> dirInfoList)
        {
            foreach (var sourceDir in dirInfoList)
            {
                var destDir = Path.Combine(targetDir, Path.GetFileName(sourceDir));
                CopyMissingFilesRecursively(sourceDir, destDir);
            }
        }

        private static void CopyMissingFilesRecursively(string sourceDir, string destDir)
        {
            // Ensure destination directory exists
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            //MessageBox.Show(sourceDir);

            // Copy files
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                if (!File.Exists(destFile))
                {
                    File.Copy(file, destFile);
                }
            }

            // Copy subdirectories
            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
                CopyMissingFilesRecursively(subDir, destSubDir);
            }
        }

        /// <summary>
        /// Processes each XML in xmlDir: determines the analysis key, locates the XSL stylesheet, and applies the transform.
        /// </summary>
        private static void ProcessXmlFiles(string xmlDir, string styleDir, Dictionary<string, string> analysisMap, String cssFile)
        {
            foreach (var xmlPath in Directory.GetFiles(xmlDir, "*.xml"))
            {
                var fileName = Path.GetFileNameWithoutExtension(xmlPath);
                string analysisKey = GetAnalysisKey(fileName, analysisMap.Keys);

                var xslName = analysisMap[analysisKey];
                var xslPath = Path.Combine(styleDir, xslName);

                if (!File.Exists(xslPath))
                    throw new FileNotFoundException($"Stylesheet not found for key '{analysisKey}'.", xslPath);

                var outputHtml = Path.Combine(Path.GetDirectoryName(xmlDir)!, fileName + ".html");
                ApplyTransform(xmlPath, xslPath, outputHtml);

                // 2) post‑process the HTML to inline the CSS
                EmbedCssInline(outputHtml, cssFile);
            }
        }

        private static void EmbedCssInline(string htmlPath, string cssFile)
        {
            // read both files
            string html = File.ReadAllText(htmlPath);
            string css = File.ReadAllText(cssFile);

            // build a <style> block
            string styleBlock = $"<style type=\"text/css\">{css}</style>";

            // replace any <link href="...common.css"...> with our inline style
            html = Regex.Replace(
                html,
                @"<link[^>]*href\s*=\s*[""'][^""']*common\.css[""'][^>]*>\s*",
                styleBlock,
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            // write it back
            File.WriteAllText(htmlPath, html, Encoding.UTF8);
        }

        /// <summary>
        /// Determines the analysis key based on filename patterns (_KEY or _KEY_).
        /// </summary>
        private static string GetAnalysisKey(string fileName, IEnumerable<string> keys)
        {
            try
            {
                // Normalize the filename to avoid extension confusion or trailing artifacts
                string cleanName = Path.GetFileNameWithoutExtension(fileName);

                return keys.First(k =>
                    Regex.IsMatch(cleanName, $@"_(?i:{Regex.Escape(k)})(?=(_|\s|\(|\.|$))")
                );
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException(
                    $"Filename '{fileName}' does not match any known analysis codes: {string.Join(", ", keys)}.");
            }
        }

        /// <summary>
        /// Applies the XSLT transformation to produce an HTML file.
        /// </summary>
        private static void ApplyTransform(string xmlPath, string xslPath, string htmlOutput)
        {
            var settings = new XsltSettings(enableDocumentFunction: false, enableScript: false);
            var resolver = new XmlUrlResolver();
            var xslt = new XslCompiledTransform();
            xslt.Load(xslPath, settings, resolver);

            var writerSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = false
            };

            using var writer = XmlWriter.Create(htmlOutput, writerSettings);
            xslt.Transform(xmlPath, null, writer);
        }

        /// <summary>
        /// Builds the mapping of analysis codes to XSL filenames.
        /// </summary>
        private static Dictionary<string, string> CreateAnalysisMap()
            => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["BA"] = "bigram_analysis.xsl",
                ["CT"] = "copytask_analysis.xsl",
                ["FLUA"] = "fluency_analysis.xsl",
                ["GA"] = "general_analysis.xsl",
                ["GEA"] = "general_eyetrack_analysis.xsl",
                ["LA"] = "linear_analysis.xsl",
                ["LG"] = "linguistic_analysis.xsl",
                ["PG"] = "",
                ["PA"] = "pause_analysis.xsl",
                ["RM"] = "revisionmatrix_analysis.xsl",
                ["SN"] = "snotation_analysis.xsl",
                ["SO"] = "source_analysis.xsl",
                ["SU"] = "summary_analysis.xsl",
                ["TA"] = "token_analysis.xsl",
                ["WP"] = "word_pause_analysis.xsl"
            };
    }

}