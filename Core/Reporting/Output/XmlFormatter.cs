using System.IO;
using System.Text;
using System.Xml;
using InputLog.Core.Reporting.ReportTemplate;

namespace InputLog.Core.Reporting.Output
{
    /// <summary>
    ///     Outputs a report as an XML - same format as the Module/Block 
    ///     analysis output files.
    /// </summary>
    public class XmlFormatter: Formatter
    {
        /// <summary>
        ///     Affix gets appended behind document names of xmls that
        ///     have been written by the Xmlformatter
        /// </summary>
        public const string AFFIX = "REP";

        /// <summary>
        ///     In memory XmlDocument.
        /// </summary>
        private XmlDocument _doc;

        /// <summary>
        ///     The module node. We only need one module node to which
        ///     we add all the blocks.
        /// </summary>
        private XmlElement _root;

        /// <summary>
        ///     Current block node.
        /// </summary>
        private XmlElement _currentBlock;

        /// <summary>
        ///     The target id of the current value.
        /// </summary>
        private string _currentValueId;

        /// <summary>
        ///     Create a new XmlFormatter.
        /// </summary>
        public XmlFormatter()
        {
            this._doc = new XmlDocument();
            XmlElement session = this._doc.CreateElement("session");
            this._doc.AppendChild(session);

            this._root = this._doc.CreateElement("module");
            this._root.SetAttribute("name", "report");
            session.AppendChild(this._root);

            // creates / -> session -> module -> (here come the blocks)
        }

        #region Structure methods 
        /// <summary>
        ///     Start a new module for the block with name 
        ///     equal to block title. Only writes a block if the block contains elements.
        /// </summary>
        /// <param name="template">Template for the block.</param>
        public override void StartBlock(BlockTemplate template)
        {
            base.StartBlock(template);
            if (template.ContainsElements)
            {
                this._currentBlock = this._doc.CreateElement("block");
                this._currentBlock.SetAttribute("name", template.Title);
                this._root.AppendChild(this._currentBlock);
            }
        }

        public override void StartValue(ValueTemplate template)
        {
            base.StartValue(template);
            this._currentValueId = template.TargetID;
        }

        public override void EndValue()
        {
            base.EndValue();
        }
        #endregion

        #region Output values
        public override void AddLabeledImage(string label, Stream imageStream, string intro = null, bool fullPage = false)
        {
            // Don't do anything. 
            // We do not add images to the xml.
        }

        public override void AddLabeledValue(string label, string value, string intro = null)
        {
            XmlElement element = this._doc.CreateElement("element");
            element.SetAttribute("name", this._currentValueId);
            element.SetAttribute("value", value);
            this._currentBlock.AppendChild(element);
        }

        #endregion

        #region Output methods
        public override string GetExtension()
        {
            return "xml";
        }

        public override string GetAffix()
        {
            return "_REPORT";
        }

        public override void WriteToFile(string filepath)
        {
            XmlTextWriter writer = new XmlTextWriter(filepath, Encoding.UTF8);
            this._doc.WriteTo(writer);
            writer.Flush();
            writer.Close();
        }

        public override void WriteToStream(Stream stream)
        {
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            this._doc.WriteTo(writer);
            writer.Flush();
            writer.Close();
        }
        #endregion
    }
}
