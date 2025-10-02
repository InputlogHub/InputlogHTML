using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Reporting.Output;
using InputLog.Core.Reporting.ReportTemplate;

namespace InputLog.Core.Reporting.Report
{
    /// <summary>
    ///     A block contains one or multiple Elements, in which each element
    ///     is one bullet item but can consists of multiple values retrieved
    ///     from summaries.
    /// </summary>
    public class Element
    {
        /// <summary>
        ///     List of all values within this element.
        /// </summary>
        private List<ReportValue> Values;

        /// <summary>
        ///     Create a new Report Element.
        /// </summary>
        public Element()
        {
            this.Values = new List<ReportValue>();
        }

        /// <summary>
        ///     Add a value to the Element. Values are shown in 
        ///     order of appearance.
        /// </summary>
        /// <param name="value">The value to add to this element.</param>
        public void AddValue(ReportValue value)
        {
            this.Values.Add(value);
        }

        /// <summary>
        ///     Format an element using the specified formatter and
        ///     the specified element template
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="elementTemplate"></param>
        public void Format(Formatter formatter, ElementTemplate elementTemplate)
        {
            formatter.StartElement(elementTemplate);
            formatter.StartValues(this.Values.Count);
            foreach (ValueTemplate template in elementTemplate.Values)
            {
                ReportValue value = this.Values.Single((e) => e.GetTargetID() == template.TargetID);
                value.Format(formatter, template);
            }
            formatter.EndValues();
            formatter.EndElement();
        }
    }
}
