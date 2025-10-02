using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InputLog.Core.Reporting.ReportTemplate.Import
{
    /// <summary>
    ///     Abstract class that defines classes for template importing.
    /// </summary>
    public abstract class TemplateImporter
    {
        // Template level
        public virtual void StartTemplate() { }
        public virtual void EndTemplate() { }

        // Report level
        public virtual void StartReport() { }
        public abstract string CurrentReportTitle { get; }
        public abstract string CurrentReportLocalization { get; }
        public abstract string CurrentReportID { get; }
        public virtual void EndReport() { }

        // Block level
        public virtual void StartBlocks() { }
        public virtual void StartBlock() { }
        public abstract string CurrentBlockTitle { get; }
        public abstract List<ReportTemplate.StructuredText> CurrentBlockPrepend { get; }
        public abstract List<ReportTemplate.StructuredText> CurrentBlockAppend { get; }
        public virtual void EndBlock() { }
        public abstract bool HasMoreBlocks { get; }
        public virtual void EndBlocks() { }

        // Element level
        public virtual void StartElements() { }
        public virtual void StartElement() { }
        public abstract string CurrentElementIntroduction { get; }
        public virtual void EndElement() { }
        public abstract bool HasMoreElements { get; }
        public virtual void EndElements() { }

        // Value level
        public virtual void StartValues() { }
        public virtual void StartValue() { }
        public abstract string CurrentValueLabel { get; }
        public abstract bool CurrentValueLabelIsBold { get; }
        public abstract string CurrentValueValueID { get; }
        public abstract string CurrentValuePrepend { get; }
        public abstract string CurrentValueAppend { get; }
        public virtual void EndValue() { }
        public abstract bool HasMoreValues { get; }
        public virtual void EndValues() { }

        public TemplateImporter()
        {
            _errors = new List<Error>();
        }

        //
        // Error handling
        //
        public class Error
        {
            public string Message { get; set; }
            public string Node { get; set; }
            public Severity Severity { get; set; }
            public Error(string nodeName, string message, Severity severity)
            {
                Message = message;
                Node = nodeName;
                Severity = severity;
            }
        }

        /// <summary>
        ///     Error severity.
        /// </summary>
        public enum Severity
        {
            NONE = 0,
            INFO = 5,
            WARNING = 10,
            ERROR = 20,
            FATAL = 30
        }

        private readonly List<Error> _errors;

        /// <summary>
        ///     Log an error during the importing.
        /// </summary>
        /// <param name="error">Error to report, contains information
        /// about the encountered error.</param>
        public void AddError(Error error)
        {
            Debug.Assert(error != null);
            this._errors.Add(error);
        }

        /// <summary>
        ///     Returns the number of errors encountered
        ///     while importing the template.
        /// </summary>
        public int ErrorCount => _errors.Count;

        /// <summary>
        ///     Returns true if the importer has encountered
        ///     any errors, false if not.
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        /// <summary>
        ///     All the errors encountered importing the template.
        /// </summary>
        public IEnumerable<Error> Errors => _errors;

        /// <summary>
        ///     Highest severity error encountered. If no errors have
        ///     been encountered this returns Severity.NONE
        /// </summary>
        public Severity HighestSeverity
        {
            get
            {
                if (!HasErrors)
                {
                    return Severity.NONE;
                }
                return _errors.Max(error => error.Severity);
            }
        }
    }
}
