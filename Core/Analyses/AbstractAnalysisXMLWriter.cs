using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// AbstractAnalysisXMLWriter implements functionality that is shared between various *AnalysisXMLWriter's.
    /// </summary>
    public abstract class AbstractAnalysisXMLWriter : IAnalysisWriter
    {
        #region Fields
        // some common tags
        protected const string SESSION_TAG = "session";
        private const string SESSION_INFO_TAG = "sessionIdentification";
        protected const string COMMON_XSL = "common.xsl";
        protected const string COMMON_CSS = "common.css";

        private const string META_TAG = "meta";
        private const string MAINDOC = "MainDocument";
        private const string LOG_FILE = "Logfile";
        private const string LOG_CREATION_DATE = "LogCreationDate";
        private const string LOG_GUID = "LogGUID";
        private const string ANALYSIS_CREATION_DATE = "AnalysisCreationDate";
        private const string ANALYSIS_GUID = "AnalysisGUID";
        private const string ANALYSIS_PROGRAM_VERSION = "AnalysisProgramVersion";
        private const string LOG_PROGRAM_VERSION = "LogProgramVersion";
        private const string PROGRAM_CONVERTED_FROM = "ConvertedFrom";
        
        private const string EXTRA_INFO_TAG = "extraInfo";
        private const string EXTRA_INFO_TITLE_ATTR = "title";

        private const string ENTRY_TAG = "entry";
        protected const string NAME_ATTRIBUTE = "name";
        protected const string VALUE_ATTRIBUTE = "value";

        // Location of the resources used to style the analysis XML.
        private const string STYLES_LOCATION = "Analyses/Style/Style";
        private const string IMAGES_LOCATION = "Analyses/Style/Images";
        private const string SCRIPTS_LOCATION = "Analyses/Style/Scripts";
        private const string HTML_README_LOCATION = "Analyses/Style/";
        protected const string INPUTLOG_LOGO = "InputLog-Logo.gif";
        protected const string JQUERY_SCRIPT = "jquery-1.4.2.min.js";
        protected const string HTML_README = "HTML_README.txt";

        /// <summary>
        /// XmlWriter used to writer xml to the analysis document.
        /// </summary>
        protected readonly XmlTextWriter XMLWriter;

        /// <summary>
        /// Path to the destination file (= the result of the analysis).
        /// </summary>
        private string DestinationFilePath { get; set; }

        /// <summary>
        /// Indicates whether this instance of this class has been disposed.
        /// </summary>
        private bool Disposed;
        #endregion

        /// <summary>
        /// Constructs an AbstractAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the the analysis document should be created.</param>
        protected AbstractAnalysisXMLWriter(string destinationFilePath)
        {
            XMLWriter = new XmlTextWriter(destinationFilePath, Encoding.UTF8) {Formatting = Formatting.Indented};
            DestinationFilePath = destinationFilePath;
        }

        /// <summary>
        /// Writes out the analysis document using a given AnalysisSummary.
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification of the logging 
        /// session on which the Analysis was performed.</param>
        /// <param name="extraInfo">A dictionary containing extra information about the analysis. 
        /// This information in this dictionary should be written to the header of the document. 
        /// Using a dictionary within a dictionary, 
        /// it is possible to categorize key-value pairs into categories.</param>
        /// <param name="summary">Summary of the analysis.</param>
        public abstract void WriteDocument(SessionIdentification sessionIdentification,
           IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary);

        /// <summary>
        /// Writes the analyzed events to a file in memory.
        /// </summary>
        /// <param name="summary">the analyzed events</param>
        /// <returns>XmlDocument to be used inside the application, e.g. by the ProcessGraph Analysis</returns>
        public abstract XmlDocument WriteMemory(IAnalysisSummary summary);

        /// <summary>
        /// Writes out the XML-header of the analysis document.
        /// </summary>
        protected void WriteHeader(string stylesheetLocation)
        {
            XMLWriter.WriteStartDocument();
            XMLWriter.WriteProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"Style/"
                + stylesheetLocation + "\"");
        }

        /// <summary>
        /// 20110326 changed date format to include millisecs in ANALYSIS_CREATION_DATE EVH
        /// Writes out the session meta-data to the analysis-document.
        /// (Including log creation date, analysis creation date, ...).
        /// </summary>
        /// <param name="sessionId">he session identification data from which to extract 
        /// the meta data</param>
        protected void WriteSessionMetaData(SessionIdentification sessionId)
        {
            if (sessionId == null) return;
            XMLWriter.WriteStartElement(META_TAG);

            string filename = sessionId.GetFileName();
            if (filename != null)
            {
                WriteEntry(LOG_FILE, filename);
            }

            string mainDoc = sessionId.GetMainDocument();
            WriteEntry(MAINDOC, mainDoc);

            if (sessionId.IsConverted())
            {
                WriteEntry(PROGRAM_CONVERTED_FROM, sessionId.GetConversion());
            }

            string creationDate = sessionId.GetCreationDateString();
            if (creationDate != null)
            {
                WriteEntry(LOG_CREATION_DATE, creationDate);
            }

            string guid = sessionId.GetGuid();
            if (guid != null)
            {
                WriteEntry(LOG_GUID, guid);
            }

            string logVersion = sessionId.GetLogVersion();
            WriteEntry(LOG_PROGRAM_VERSION, logVersion ?? "Unknown Version");

            WriteEntry(ANALYSIS_CREATION_DATE, DateTime.Now.ToString("dd/MM/yy HH:mm:ss"));
            WriteEntry(ANALYSIS_GUID, Guid.NewGuid().ToString());

            string analysisVersion = sessionId.GetAnalysisVersion();
            if (analysisVersion != null)
            {
                WriteEntry(ANALYSIS_PROGRAM_VERSION, analysisVersion);
            }

            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes out a given dictionary of extra info in XML.
        /// </summary>
        /// <param name="extraInfo">The extra info to write out.</param>
        protected void WriteExtraInfo(IDictionary<string, IDictionary<string, object>> extraInfo)
        {
            if (extraInfo == null || extraInfo.Count == 0) return;

            foreach (var category in extraInfo)
            {
                XMLWriter.WriteStartElement(EXTRA_INFO_TAG);
                XMLWriter.WriteAttributeString(EXTRA_INFO_TITLE_ATTR, category.Key);
                if (category.Value != null)
                {
                    foreach (var entry in category.Value)
                    {
                        WriteEntry(entry.Key, entry.Value);
                    }
                }
                XMLWriter.WriteEndElement();
            }

        }

        /// <summary>
        /// Write a simple element to the document. You specify the
        /// tag and the value of the element.
        /// </summary>
        /// <param name="tag">Tag of the element</param>
        /// <param name="value">Value of the element.</param>
        public void WriteElement(string tag, object value)
        {
            XMLWriter.WriteElementString(tag, value.ToString());
        }

        /// <summary>
        /// Writes out an entry to the XML document.
        /// </summary>
        /// <param name="name">name of the entry.</param>
        /// <param name="value">value of the entry.</param>
        private void WriteEntry(string name, object value)
        {
            if (value == null) return;
            XMLWriter.WriteStartElement(ENTRY_TAG);
            XMLWriter.WriteAttributeString(NAME_ATTRIBUTE, name);
            XMLWriter.WriteAttributeString(VALUE_ATTRIBUTE, value.ToString());
            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes out the session identification information.
        /// (skips any key starting with a double underscore "__")
        /// </summary>
        /// <param name="sessionId">The session-identification dictionary to write out.</param>
        protected void WriteSessionIdentification(SessionIdentification sessionId)
        {
            var sessionInfo = sessionId.GetSessionInfo();
            if (sessionInfo == null || sessionInfo.Count == 0) return;
            XMLWriter.WriteStartElement(SESSION_INFO_TAG);
            foreach (var entry in sessionInfo)
            {
                WriteEntry(entry.Key, entry.Value);
            }
            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Utility-method that writes a tag with a value to the analysis-document if the value is not null.
        /// Otherwise, nothing is written.
        /// </summary>
        /// <param name="tagName">name of the tag to wrap the value in.</param>
        /// <param name="tagValue">value of the tag.</param>
        /// <param name="alternative">if tagValue is null, and alternative is not null, 
        /// the alternative value will be written in the tag. If both are null, the tag is not written.</param>
        protected void WriteIfValueNotNull(string tagName, object tagValue, object alternative = null)
        {
            if (tagValue != null)
            {
                XMLWriter.WriteElementString(tagName, tagValue.ToString());
            }
            else if (alternative != null)
            {
                XMLWriter.WriteElementString(tagName, alternative.ToString());
            }
        }

        /// <summary>
        /// 20110325 Try-catch added  EVH
        /// 20110330 changed path construction for the three folders (IMAGES, SCRIPTS, STYLES) EVH
        /// Copies the necessary xsl-stylesheets, images and scripts to the directory where the analysis-document
        ///  is located (this is derived from the AbstractAnalysisXMLWriter.DestinationFilePath).
        /// Note: the locations given by the parameters should all start from the style, 
        /// images and script basedirectories as given by STYLES_LOCATION, IMAGES_LOCATION and SCRIPTS_LOCATION.
        /// </summary>
        /// <param name="stylesheets">Array containing the src locations of the stylesheet to copy.</param>
        /// <param name="images">(Optional) Array containing the src locations of the images to copy.</param>
        /// <param name="scripts">(Optional) Array containing the src locations of the scripts to copy. </param>
        protected void CopyStyle(string[] stylesheets, string[] images = null, string[] scripts = null,
            string[] htmltext = null)
        {
            // copy .stylesheets
            if (stylesheets != null)
            {
                try
                {
                    // creates source & destination styles path 
                    var dstStylesPath = GetStylesDestinationPath();
                    var srcStylesPath = PathSanitizer.Sanitize(Path.Combine(Application.StartupPath, STYLES_LOCATION));

                    if (!Directory.Exists(dstStylesPath))
                    {
                        Directory.CreateDirectory(dstStylesPath);
                    }
                    foreach (var stylesheet in stylesheets)
                    {
                        var srcStyleSheet = Path.Combine(srcStylesPath, stylesheet);
                        var dstStyleSheet = Path.Combine(dstStylesPath, stylesheet);

                        if (File.Exists(srcStyleSheet))
                        {
                            File.Copy(srcStyleSheet, dstStyleSheet, true /* overwrite if file exists */);
                        }
                    }
                }
                catch (AnalysisException e)
                {
                    MessageLogger.CatchException(this, e, Severity.ERROR, "Style sheets not copied");
                }
            }

            // copy images
            if (images != null)
            {
                try
                {
                    // creates source & destination image path 
                    var dstImagesPath = GetImagesDestinationPath();
                    var srcImagesPath = PathSanitizer.Sanitize(Path.Combine(Application.StartupPath, IMAGES_LOCATION));

                    if (!Directory.Exists(dstImagesPath))
                    {
                        Directory.CreateDirectory(dstImagesPath);
                    }
                    foreach (var image in images)
                    {
                        var srcImage = Path.Combine(srcImagesPath, image);
                        var dstImage = Path.Combine(dstImagesPath, image);

                        if (File.Exists(srcImage))
                        {
                            File.Copy(srcImage, dstImage, true /* overwrite if file exists */);
                        }
                    }
                }
                catch (AnalysisException e)
                {
                    MessageLogger.CatchException(this, e, Severity.ERROR, "Image files not copied");
                }
            }

            // copy scripts
            if (scripts == null) return;
            try
            {
                // creates source & destination scripts path 
                var dstScriptPath = GetScriptsDestinationPath();
                var srcScriptPath = PathSanitizer.Sanitize(Path.Combine(Application.StartupPath, SCRIPTS_LOCATION));

                if (!Directory.Exists(dstScriptPath))
                {
                    Directory.CreateDirectory(dstScriptPath);
                }
                foreach (var script in scripts)
                {
                    var srcScript = Path.Combine(srcScriptPath, script);
                    var dstScript = Path.Combine(dstScriptPath, script);

                    if (File.Exists(srcScript))
                    {
                        File.Copy(srcScript, dstScript, true /* overwrite if file exists */);
                    }
                }
            }
            catch (AnalysisException e)
            {
                MessageLogger.CatchException(this, e, Severity.ERROR, "Scripts not copied");
            }
        }

        /// <summary>
        /// Copies the specified HTML text file to the destination directory.
        /// </summary>
        /// <remarks>This method ensures that the destination directory exists before copying the file. 
        /// If the file already exists at the destination, it will be overwritten.</remarks>
        /// <param name="htmltext">The name of the HTML text file to copy. If <see langword="null"/>, 
        /// the method does nothing.</param>
        protected void CopyText(string htmltext = null)
        {
            // copy html instruction text
            if (htmltext == null) return;
            {
                try
                {
                    // creates source & destination path for the html text
                    var dstTextPath = GetHTMLTextDestinationPath();
                    var srcTextPath = PathSanitizer.Sanitize(Path.Combine(Application.StartupPath, HTML_README_LOCATION));

                    if (!Directory.Exists(dstTextPath))
                    {
                        Directory.CreateDirectory(dstTextPath);
                    }

                    var srcPath = Path.Combine(srcTextPath, htmltext);
                    var dstPath = Path.Combine(dstTextPath, htmltext);

                    if (File.Exists(srcPath))
                    {
                        File.Copy(srcPath, dstPath, true /* overwrite if file exists */);
                    }

                }
                catch (AnalysisException e)
                {
                    MessageLogger.CatchException(this, e, Severity.ERROR, "HTMLText not copied");
                }
            }
        }

        /// <summary>
        /// Returns the absolute path of the styles directory under to same parent directory in which the analysis 
        /// file created by this analysis writer will be created.
        /// </summary>
        /// <returns>The absolute path of the styles directory under to same parent directory in which the analysis 
        /// file created by this analysis writer will be created.</returns>
        private string GetStylesDestinationPath()
        {
            return GetDestinationDirectoryPath(STYLES_LOCATION);
        }

        /// <summary>
        /// Returns the absolute path of the scripts directory under to same parent directory in which the analysis 
        /// file created by this analysis writer will be created.
        /// </summary>
        /// <returns>The absolute path of the scripts directory under to same parent directory in which the analysis 
        /// file created by this analysis writer will be created.</returns>
        private string GetScriptsDestinationPath()
        {
            return GetDestinationDirectoryPath(SCRIPTS_LOCATION);
        }

        /// <summary>
        /// Returns the absolute path of the images directory under to same parent directory in which the analysis 
        /// file created by this analysis writer will be created.
        /// </summary>
        /// <returns>The absolute path of the images directory under to same parent directory in which the analysis 
        /// file created by this analysis writer will be created.</returns>
        private string GetImagesDestinationPath()
        {
            return GetDestinationDirectoryPath(IMAGES_LOCATION);
        }

        /// <summary>
        /// Returns the absolute path of the directory with the HTML instruction text
        /// </summary>
        /// <returns>The absolute path of the HTML instruction text directory</returns>
        private string GetHTMLTextDestinationPath()
        {
            return GetDestinationDirectoryPath(HTML_README_LOCATION);
        }

        /// <summary>
        /// Returns the absolute path of where a certain subdirectory will be placed if it is placed in the same 
        /// directory as the analysis file for which this class provides a writer.
        /// </summary>
        /// <param name="directory">Directory for which to get the absolute path.</param>
        /// <returns>the absolute path of where a certain subdirectory will be placed if it is placed in the same 
        /// directory as the analysis file for which this class provides a writer.</returns>
        private string GetDestinationDirectoryPath(string directory)
        {
            return Path.Combine(Path.GetDirectoryName(DestinationFilePath), Path.GetFileName(directory));
        }

        protected void WriteModule(string name, Action writeFunc)
        {
            XMLWriter.WriteStartElement("module");
            XMLWriter.WriteAttributeString("name", name);
            writeFunc();
            XMLWriter.WriteEndElement();
        }

        protected void WriteModule(string name, Dictionary<string, string> extraParams, Action writeFunc)
        {
            XMLWriter.WriteStartElement("module");
            XMLWriter.WriteAttributeString("name", name);
            foreach (string param in extraParams.Keys)
            {
                XMLWriter.WriteAttributeString(param, extraParams[param]);
            }
            writeFunc();
            XMLWriter.WriteEndElement();
        }

        protected void WriteModuleBlock(string name, Action writeFunc, bool ignoreForMerge = false)
        {
            XMLWriter.WriteStartElement("block");
            XMLWriter.WriteAttributeString("name", name);
            if (ignoreForMerge) XMLWriter.WriteAttributeString("ignoreForMerge", "1");
            writeFunc();
            XMLWriter.WriteEndElement();
        }

        protected void WriteModuleElement(string name, string value)
        {
            XMLWriter.WriteStartElement("element");
            XMLWriter.WriteAttributeString("name", name);
            XMLWriter.WriteAttributeString("value", value);
            XMLWriter.WriteEndElement();
        }

        public void Abort()
        {
            Dispose();
            File.Delete(DestinationFilePath);     
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        private void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                XMLWriter.Close();
            }
            Disposed = true;
        }


        /// <summary>
        /// Destructor, will only be called whenever Dispose() is not called.
        /// Do not provide any destructors in types derived from this LinearAnalysisType.
        /// </summary>
        ~AbstractAnalysisXMLWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the object, call this method whenever the object is not needed any more.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass
            // of this LinearAnalysisType implements a finalizer.
            GC.SuppressFinalize(this);
        }
    }
}