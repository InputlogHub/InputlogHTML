using System;
using System.Collections.Generic;
using System.Xml;
using InputLog.Core.Analyses.LinguisticAnalysis;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories;
using InputLog.Core.Analyses.Revision.Notations.SymbolFactories.Symbols;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Revision.Notations
{
    /// <summary>
    /// A string with the text and its revisions represented as inserts, deletions, and breaks 
    /// according to the W-Notation conventions.
    /// </summary>
    public class WNotationSummary : AbstractAnalysisSummary
    {
        #region Fields

        /// <summary>
        /// List of the markup symbols and text in this summary.
        /// </summary>
        public LinkedList<SymbolNode> SymbolList { get; private set; }

        /// <summary>
        ///// Change W-Notation markup symbols into S-Notation to enhance readability in the W-Notation report.
        /// </summary>
        public LinkedList<SymbolNode> SNotationList
        {
            get { return MarkupRewriter.RewriteSymbolList(SymbolList); }
        }

        /// <summary>
        /// The syntax used when building the notation.
        /// </summary>
        public ISymbolFactory Syntax { get; private set; }

        /// <summary>
        /// String with W-Notation markup symbols added.
        /// </summary>
        public string RevisionTxt { get; private set; }

        /// <summary>
        /// Full text extracted from Word document
        /// </summary>
        public string FullText { get; private set; }

        #endregion

        /// <summary>
        /// W-Notation Summary Constructor starts the post processing.
        /// </summary>
        /// <param name="symbols">LinkedList with text and W-Notation markup symbols</param>
        /// <param name="syntax">The S-Notation syntax</param>
        /// <param name="revisionTxt">String with W-Notation markup symbols added.</param>
        public WNotationSummary(LinkedList<SymbolNode> symbols, ISymbolFactory syntax, string revisionTxt)
        {
            SymbolList = symbols;
            Syntax = syntax;
            FullText = "";
            RevisionTxt = revisionTxt;
        }

        /// <summary>
        /// W-Notation Summary copy constructor
        /// </summary>
        protected WNotationSummary(WNotationSummary rhs)
        {
            if (rhs == this) return;
            SymbolList = rhs.SymbolList;
            Syntax = rhs.Syntax;
            FullText = rhs.FullText;
            RevisionTxt = rhs.RevisionTxt;
        }

        /// <summary>
        /// Default constructor for use with serialization
        /// Constructor should be public, not protected or private.
        /// </summary>
        public WNotationSummary()
        {
            SymbolList = new LinkedList<SymbolNode>();
        }

        #region Xml Serialization Infrastructure

        public override void WriteXml(XmlWriter writer)
        {
            if (SymbolList != null && SymbolList.Count > 0)
            {
                writer.WriteStartElement("SymbolList");
                foreach (SymbolNode sn in SymbolList)
                {
                    writer.WriteStartElement("SymbolNode");
                    writer.WriteElementString("FullType", sn.GetType().FullName);
                    sn.WriteXml(writer);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteStartElement("Syntax");
            writer.WriteElementString("FullType", Syntax.GetType().FullName);
            writer.WriteEndElement();
            writer.WriteElementString("RevisionTxt", RevisionTxt);
            writer.WriteElementString("FullText", WordDocumentTools.OpenDoc(LinguisticAnalysisDescription.DocPath));
        }

        public override void ReadXml(XmlReader reader)
        {
            if (reader.IsStartElement("SymbolList"))
            {
                reader.ReadStartElement("SymbolList");
                while (reader.IsStartElement("SymbolNode"))
                {
                    reader.ReadStartElement("SymbolNode");
                    String typeStr = reader.ReadElementString("FullType");
                    var sn = (SymbolNode) Activator.CreateInstance(Type.GetType(typeStr));
                    sn.ReadXml(reader);
                    SymbolList.AddLast(sn);
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
            reader.ReadStartElement("Syntax");
            String sTypeStr = reader.ReadElementString("FullType");
            Syntax = (ISymbolFactory) Activator.CreateInstance(Type.GetType(sTypeStr));
            reader.ReadEndElement();
            RevisionTxt = reader.ReadElementString("RevisionTxt");
            FullText = reader.ReadElementString("FullText");
        }

        #endregion
    }
}