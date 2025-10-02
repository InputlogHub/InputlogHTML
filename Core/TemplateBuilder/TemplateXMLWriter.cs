using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace InputLog.Core.TemplateBuilder
{
    /// <summary>
    /// TemplateXMLWriter implements the functionality needed to construct templates from GUI blocks.
    /// </summary>
    public class TemplateXMLWriter
    {
        #region Fields
        /// <summary>
        /// XmlWriter used to writer xml to the template document.
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
        public TemplateXMLWriter(string destinationFilePath)
        {
            XMLWriter = new XmlTextWriter(destinationFilePath, Encoding.UTF8) {Formatting = Formatting.Indented};
            DestinationFilePath = destinationFilePath;
        }

        public void WriteStart(string title)
        {
            XMLWriter.WriteStartElement("report");
            XMLWriter.WriteElementString("id", "CustomReportTemplate");
            XMLWriter.WriteElementString("language", "EN-US");
            XMLWriter.WriteElementString("title", title);
        }

        public void WriteEnd()
        {
            XMLWriter.WriteEndElement();
        }

        public void WriteBlocks(List<TemplateBlock> blocks)
        {
            foreach (var block in blocks)
            {
                WriteBlock(block);
            }
        }

        public void WriteBlock(TemplateBlock block)
        {
            XMLWriter.WriteStartElement("block");
            XMLWriter.WriteElementString("title", block.Title);
            block.WriteIntroductionXML(XMLWriter);
            block.WriteElementsXML(XMLWriter);
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
        ~TemplateXMLWriter()
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