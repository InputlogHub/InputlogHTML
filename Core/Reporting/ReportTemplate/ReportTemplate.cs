using System;
using System.Collections.Generic;
using System.Diagnostics;
using InputLog.Core.Reporting.ReportTemplate.Import;

namespace InputLog.Core.Reporting.ReportTemplate
{
    /// <summary>
    ///     A ReportTemplate is an in memory-representation of how 
    ///     a report should be structured. It contains the information about
    ///     both structure, texts, labels that deviate from the default labels etc.
    /// </summary>
    public class ReportTemplate
    {
        /// <summary>
        ///     Title of the report
        /// </summary>
        public string Title
        {
            get;
            protected set;
        }

        /// <summary>
        ///     Localization string to determine what language of resources
        ///     have to be loaded. This influences default labels, introductions and 
        ///     descriptions. 
        ///     Stored as a string, e.g.: 'en-us', 'fr-fr', 'be-nl', ...
        /// </summary>
        public string Localization
        {
            get;
            private set;
        }

        public string ID
        {
            get;
            private set;
        }

        /// <summary>
        ///     Enumeration of all BlockTemplates in this template.
        /// </summary>
        public IEnumerable<BlockTemplate> Blocks => _blocks;
   
        public List<BlockTemplate> _blocks;

        /// <summary>
        ///     Create a new report template.
        /// </summary>
        private ReportTemplate()
        {
            this._blocks = new List<BlockTemplate>();
        }

        /// <summary>
        ///     Add a block to the report.
        /// </summary>
        /// <param name="block">Block template to add.</param>
        protected void AddBlock(BlockTemplate block)
        {
            this._blocks.Add(block);
        }

        /// <summary>
        ///     Construct this ReportTemplate from given TemplateImporter.
        /// </summary>
        /// <param name="importer">Importer that should be used to
        /// construct the ReportTemplate</param>
        public static ReportTemplate ImportFrom(TemplateImporter importer)
        {
            try
            {
                ReportTemplate report = new();
                importer.StartTemplate();
                importer.StartReport();
                importer.StartBlocks();
                while (importer.HasMoreBlocks)
                {
                    BlockTemplate block = BlockTemplate.ImportFrom(importer);
                    report.AddBlock(block);
                }
                importer.EndBlocks();

                report.Title = importer.CurrentReportTitle;
                report.Localization = importer.CurrentReportLocalization;
                report.ID = importer.CurrentReportID;

                importer.EndReport();
                importer.EndTemplate();
                return report;
            }
            catch (Exception e)
            {
                // The importer should have more detailed information about
                // the error already. We just return a non-existing template.
                return null;
            }
        }

        /// <summary>
        ///     Get all the reporting resources of all the requested 
        ///     reporting targets template
        /// </summary>
        /// <returns>An enumeration of all reporting resources.</returns>
        public IEnumerable<Report.Report.ReportResource> GetReportTargets()
        {
            List<Report.Report.ReportResource> targets = new();
            foreach (BlockTemplate block in _blocks)
            {
                targets.AddRange(block.GetReportTargets());
            }
            return targets;
        }

        /// <summary>
        ///     Returns which blocks a certain target belongs too
        /// </summary>
        /// <param name="target">The reportingtarget id.</param>
        /// <returns>All the blocks where the target appears in.</returns>
        public IEnumerable<BlockTemplate> BlocksForTarget(string target)
        {
            List<BlockTemplate> blocksForTarget = new();
            foreach (BlockTemplate block in _blocks)
            {
                if (block.ContainsTarget(target))
                {
                    blocksForTarget.Add(block);
                }
            }
            return blocksForTarget;
        }

        /// <summary>
        ///     A class representing a piece of structured text that can be added
        ///     to a report. This can be a paragraph XOR a list. 
        /// </summary>
        public class StructuredText
        {
            /// <summary>
            ///     True if this StructuredText represents a paragraph
            /// </summary>
            public bool IsParagraph
            {
                get;
                private set;
            }
            
            /// <summary>
            ///     True if this StructuredText represents a list
            /// </summary>
            public bool IsList
            {
                get;
                private set;
            }

            /// <summary>
            ///     The content of the paragraph, if this is a paragraph, 
            ///     null if it is a list
            /// </summary>
            public string Content
            {
                get;
                private set;
            }

            /// <summary>
            ///     Enumeration of the list items for this ListType StructuredText.
            ///     This value is null if it is not a ListType 
            /// </summary>
            public ICollection<string> Items
            {
                get;
                private set;
            }

            /// <summary>
            ///     Create a new StructuredText instance and set the type.
            /// </summary>
            /// <param name="type">Type of the structured text, a paragraph or list.</param>
            public StructuredText(TextType type)
            {
                if (type == TextType.LIST)
                {
                    this.IsList = true;
                    this.IsParagraph = false;
                    this.Items = new List<string>();
                }
                else
                {
                    this.IsParagraph = true;
                    this.IsList = false;
                }
            }

            /// <summary>
            ///     Create a StructuredText of paragraph type and initialize
            ///     it with the paragraph's content.
            /// </summary>
            /// <param name="content">Content of the paragraph</param>
            public StructuredText(string content)
            {
                this.IsList = false;
                this.IsParagraph = true;
                this.Content = content;
            }

            /// <summary>
            ///     Create a ListType StructuredText and initialize
            ///     it with the list items.
            /// </summary>
            /// <param name="items">Items in the list.</param>
            public StructuredText(IEnumerable<string> items)
            {
                this.IsParagraph = false;
                this.IsList = true;
                this.Items = new List<string>(items);
            }

            /// <summary>
            ///     Add an item to the list of listitems. 
            ///     This method can only be used if this is a StructuredText
            ///     of the ListType.
            /// </summary>
            /// <param name="item">Item to add.</param>
            public void AddItem(string item)
            {
                Debug.Assert(this.IsList);
                Debug.Assert(!this.IsParagraph);
                Debug.Assert(this.Items != null);

                this.Items.Add(item);
            }
        }

        /// <summary>
        ///     Type of StructuredText available
        /// </summary>
        public enum TextType {
            PARAGRAPH,
            LIST
        }


        

    }
}
