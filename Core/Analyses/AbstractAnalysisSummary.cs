using System;
using InputLog.Core.Util.Progress;
using System.Xml.Schema;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using InputLog.Core.Reporting;

namespace InputLog.Core.Analyses
{
    /// <summary>
    /// Class implementing common functionality of analysis summaries.
    /// </summary>
    public abstract class AbstractAnalysisSummary : ProgressTrackableAction, IAnalysisSummary
    {
        #region Fields
        /// <summary>
        /// Boolean indicating whether this summary has already been disposed or not.
        /// </summary>
        private bool Disposed { get; set; }
        #endregion

        #region Xml Serialization Infrastructure
        public virtual void WriteXml(XmlWriter writer)
        {}

        public virtual void ReadXml(XmlReader reader)
        {}

        public XmlSchema GetSchema()
        {
            return (null);
        }
        #endregion

        #region Reporting Methods

        /// <summary>
        /// Lists all reporting methods of the AnalysisSummary. A reporting method, by convention
        /// is any method who has 'report_' prefixed in its name. Reporting functions should not
        /// take any parameters and return a string representation of their value and they should 
        /// be publicly accessible. 
        /// NOTE: If you wish for an AnalysisSummary to have reporting capabilities too, you need not
        /// override this method. It suffices to implement a public method such as:
        /// public report_NameWhatYouWillBeReturning () { /* logic */ return result.ToString(); }
        /// </summary>
        /// <returns>A list of all methods for reporting that the AnalysisSummary has.</returns>
        public virtual Dictionary<string, InputLog.Core.Reporting.ReportMethod> GetBoundReportTargets()
        {
            var targets = new Dictionary<string, Reporting.ReportMethod>();
            string REPORT_PREFIX = "report_";

            MethodInfo[] methodInfos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder sb = new StringBuilder();
            foreach (MethodInfo mInfo in methodInfos) 
            {
                // The only valid reporting methods are methods with:
                // 1. The report_ prefix
                // 2. ReportElement returntype or a subclass/assignable replacement
                // 3. Takes no parameters
                if (mInfo.Name.StartsWith(REPORT_PREFIX) && 
                    typeof(ReportValue).IsAssignableFrom(mInfo.ReturnType) &&
                    mInfo.GetParameters().Length == 0)
                {
                    sb.Append(mInfo.Name);
                    targets.Add(
                        sb.ToString(),
                        new Reporting.ReportMethod(this, mInfo.Name)
                    );
                    sb.Clear();
                }
            }

            return targets;
        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed && disposing)
            {
                // do nothing
            }
            Disposed = true;
        }

        /// <summary>
        /// Destructor, will only be called whenever Dispose() is not called.
        /// </summary>
        ~AbstractAnalysisSummary()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the object, call this method whenever the object is not needed any more.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass of this class implements a finalizer.
            GC.SuppressFinalize(this);
        }
    }
}