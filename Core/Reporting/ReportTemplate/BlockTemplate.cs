using System.Collections.Generic;
using System.Linq;

namespace InputLog.Core.Reporting.ReportTemplate
{
    /// <summary>
    ///     Template for how a block within a report will be structured.
    ///     Also allows for specifying introductory (prefix) and closing texts (affix).
    ///     Keeps track of the elements that are in the block.
    /// </summary>
    public class BlockTemplate
    {
        #region Fields
        /// <summary>
        ///     Title of the block.
        /// </summary>
        public string Title
        {
            get;
            protected set;
        }

        /// <summary>
        ///     A prepended, introductory structured text in the block before the 
        ///     elements are shown. (optional)
        /// </summary>
        public IEnumerable<ReportTemplate.StructuredText> Prepend
        {
            get;
            protected set;
        }

        /// <summary>
        ///     An appended, closing structured text in the block after all the
        ///     elements have been shown (optional)
        /// </summary>
        public IEnumerable<ReportTemplate.StructuredText> Append
        {
            get;
            protected set;
        }

        /// <summary>
        ///     Elements in this block.
        /// </summary>
        public List<ElementTemplate> Elements { get; }

        /// <summary>
        ///     Returns true if this block contains elements. False if
        ///     it is a solely a text block.
        /// </summary>
        public bool ContainsElements
        {
            get
            {
                return Elements.Count > 0;
            }
        }

        #endregion

        /// <summary>
        ///     Create a new block template. The title for the block is required in the
        ///     constructor, but may be null or empty.
        /// </summary>
        private BlockTemplate()
        {
            Elements = new List<ElementTemplate>();
        }

        /// <summary>
        ///     Add an element to the block.
        /// </summary>
        /// <param name="element">Element template to add.</param>
        protected void AddElement(ElementTemplate element)
        {
            Elements.Add(element);
        }

        /// <summary>
        ///     Create a block an imported template.
        /// </summary>
        /// <param name="importer">The importer to use for importing blocks.</param>
        /// <returns>The imported block.</returns>
        public static BlockTemplate ImportFrom(Import.TemplateImporter importer)
        {
            BlockTemplate block = new BlockTemplate();
            importer.StartBlock();
            importer.StartElements();
            while (importer.HasMoreElements)
            {
                ElementTemplate element = ElementTemplate.ImportFrom(importer);
                block.AddElement(element);
            }
            importer.EndElements();

            // Read block values.
            block.Title = importer.CurrentBlockTitle;
            block.Prepend = importer.CurrentBlockPrepend;
            block.Append = importer.CurrentBlockAppend;

            importer.EndBlock();
            return block;
        }


        /// <summary>
        ///     Returns all the report resources requested
        ///     in this block template.
        /// </summary>
        /// <returns>An enumeration of value-id's.</returns>
        internal IEnumerable<Report.Report.ReportResource> GetReportTargets()
        {
            List<Report.Report.ReportResource> targets = new List<Report.Report.ReportResource>();
            foreach (ElementTemplate element in Elements)
            {
                targets.AddRange(element.GetReportTargets());
            }
            return targets;
        }

        /// <summary>
        ///     Returns true if this block contains the specified 
        ///     target variable
        /// </summary>
        /// <param name="target">The id of the target</param>
        /// <returns>True if this block contains the target, false if not.</returns>
        internal bool ContainsTarget(string target)
        {
            return Elements.Any(element => element.ContainsTarget(target));
        }
    }
}
