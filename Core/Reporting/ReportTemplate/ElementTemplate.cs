using System.Collections.Generic;
using System.Linq;

namespace InputLog.Core.Reporting.ReportTemplate
{
    /// <summary>
    ///     A single element to be ouputted in the report. This template
    ///     holds all the information regarding that single element:
    ///     - Label (if altered, otherwise default label from resources is used)
    ///     - Introduction (if altered, otherwise default text from resources is used)
    ///     - TargetID: The id that identifies the exact piece of information
    ///     being requested by this element. 
    /// </summary>
    public class ElementTemplate
    {
        /// <summary>
        ///     The introduction to be outputted with this element. This is a short
        ///     text (paragraph/sentence) to explain what the values mean. (optional)
        ///     If it is not set, the default introductory text for the element is used.
        /// </summary>
        public string Introduction
        {
            get;
            protected set;
        }

        /// <summary>
        ///     The templates of the values within this element.
        /// </summary>
        public List<ValueTemplate> Values;

        /// <summary>
        ///     Create a new element template. The only mandatory
        ///     information required to create this class is the targetID
        ///     which is the key to the ReportResource for this element.
        /// </summary>
        protected ElementTemplate() 
        {
            this.Values = new List<ValueTemplate>();
        }

        /// <summary>
        ///     Import an element from a template, using the given importer.
        /// </summary>
        /// <param name="importer">Importer to use</param>
        /// <returns>Element template constructed by importer.</returns>
        internal static ElementTemplate ImportFrom(Import.TemplateImporter importer)
        {
            importer.StartElement();
            ElementTemplate element = new ElementTemplate();

            importer.StartValues();
            while (importer.HasMoreValues)
            {
                ValueTemplate value = ValueTemplate.ImportFrom(importer);
                element.Values.Add(value);
            }
            importer.EndValues();
            element.Introduction = importer.CurrentElementIntroduction;
            importer.EndElement();
            return element;
        }

        /// <summary>
        ///     Returns all the report resources requested
        ///     in this element template.
        /// </summary>
        /// <returns>An enumeration of value-id's.</returns>
        public IEnumerable<Report.Report.ReportResource> GetReportTargets()
        {
            List<Report.Report.ReportResource> targets = new List<Report.Report.ReportResource>();
            foreach (ValueTemplate value in this.Values) 
            {
                targets.Add(value.Resource);
            }
            return targets;
        }

        /// <summary>
        ///     Returns true if this element contains the specified target variable.
        /// </summary>
        /// <param name="target">The id of the target</param>
        /// <returns>True if found, false if not found.</returns>
        public bool ContainsTarget(string target)
        {
            return this.Values.Any(value => value.TargetID == target);
        }
    }
}
