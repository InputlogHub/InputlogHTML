using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyTaskCreator
{
    class Zipper
    {
        /// <summary>
        /// Creates a zip archive with all files which are located at the specified file paths.
        /// </summary>
        /// <param name="folderPath">The destination path of the zip archive</param>
        /// <param name="filePaths">Array of file paths</param>
        /// <returns type="bool"></returns>
        public static bool zipFiles(string archivePath, string[] filePaths)
        {
            var directory = Path.GetDirectoryName(archivePath);
            var folderName = Path.GetFileNameWithoutExtension(archivePath);
            var folderPath = Path.Combine(directory, folderName);

            //create temporary directory
            var tempDir = new DirectoryInfo(folderPath);
            if (!tempDir.Exists)
            {
                tempDir.Create();
            }

            //copy files at specified file paths to temporary directory
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);
                string newPath = Path.Combine(folderPath, fileName);
                File.Copy(filePath, newPath,true);
            }

            //zip directory
            string startPath = folderPath;
            string zipPath = folderPath + ".zip";
            
            for (int i = 1; File.Exists(zipPath); i++)
            {
                zipPath = folderPath + "_" + i + ".zip";
            }

            ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);

            //delete temporary directory
            Directory.Delete(startPath);

            return true;
        }
    }
}
