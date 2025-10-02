using System;
using System.IO;
using InputLog.Core.Reporting.Output;
using InputLog.Core.Reporting.ReportTemplate;

namespace InputLog.Core.Reporting
{
    /// <summary>
    ///     The result of a ReportMethod. The report element contains the
    ///     value of what was requested. Since this is the class returned by
    ///     ReportingMethods, it is easy to extend this class to contain for
    ///     example more than one value. And since this class specifies
    ///     its own Formatting method, it can format itself differently
    ///     depending on value, values, and or types of values it wishes to present.
    /// </summary>
    public abstract class ReportValue
    {
        /// <summary>
        ///     The target id identifying this value element
        /// </summary>
        private string _targetId;

        public ReportValue(string target)
        {
            _targetId = target;
        }

        public abstract void Format(Formatter formatter, ValueTemplate template);

        /// <summary>
        ///     The target id identifying this value element
        /// </summary>
        /// <returns>The target id of this element</returns>
        public string GetTargetID()
        {
            return _targetId;
        }
    }

    /// <summary>
    ///     A report element that contains no value. This may happen in the case where an
    ///     analysis may not have run correctly and thus the requested value is not available.
    /// </summary>
    public class EmptyValue : ReportValue
    {
        public EmptyValue(string target) : base(target)
        {
        }

        public override void Format(Formatter formatter, ValueTemplate template)
        {
            formatter.StartValue(template);
            var resource = Report.Report.GetResource(GetTargetID());

            var label = template.Label ?? resource.Label;
            var introduction = resource.Introduction;

            formatter.AddLabeledValue(label, "N/A", introduction);
            formatter.EndValue();
        }
    }

    /// <summary>
    ///     A ReportElement that takes the form of a value with a label and
    ///     an optional introductory/explanatory text.
    /// </summary>
    public class LabeledValue : ReportValue
    {
        /// <summary>
        ///     String representation of the value of the reported element.
        /// </summary>
        private string _value;

        /// <summary>
        ///     Constructs a ReportElement
        /// </summary>
        /// <param name="target">Target id of the element</param>
        /// <param name="value">The value of the report element in string representation.</param>
        public LabeledValue(string target, string value)
            : base(target)
        {
            _value = value;
        }

        /// <summary>
        ///     Format a labeled value element.
        /// </summary>
        /// <param name="formatter">Formatter to be used.</param>
        /// <param name="template">A single value outputted as a part of an element in a report template</param>
        public override void Format(Formatter formatter, ValueTemplate template)
        {
            formatter.StartValue(template);
            var label = string.Empty;
            var introduction = string.Empty;
            try
            {
                if (template.Resource != null)
                {
                    var resource = template.Resource;
                    label = resource.Label;
                    introduction = resource.Introduction;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("undefined" + e.StackTrace);
            }

            formatter.AddLabeledValue(label, _value, introduction);
            formatter.EndValue();
        }
    }

    /// <summary>
    ///     Describes a graph-based reporting element. This consists of a title, a
    ///     graph and possibly an introductory/descriptive text for the graph.
    /// </summary>
    public class GraphValue : ReportValue
    {
        /// <summary>
        ///     In memory stream of the image file.
        /// </summary>
        private MemoryStream _imageStream;

        /// <summary>
        ///     Set to true if this Graph Element represents
        ///     a full page image.
        /// </summary>
        public bool IsFullPageImage;

        public GraphValue(string target, MemoryStream imageStream) : base(target)
        {
            _imageStream = imageStream;
            IsFullPageImage = false;
        }

        /// <summary>
        ///     Format the element.
        /// </summary>
        /// <param name="formatter">formatter to be used.</param>
        /// <param name="template">A single value outputted as a part of an element in a report template</param>
        public override void Format(Formatter formatter, ValueTemplate template)
        {
            formatter.StartValue(template);

            var resource = Report.Report.GetResource(GetTargetID());
            var label = template.Label ?? resource.Label;
            var introduction = resource.Introduction;

            formatter.AddLabeledImage(label, _imageStream, introduction, IsFullPageImage);
            formatter.EndValue();
        }
    }
}
