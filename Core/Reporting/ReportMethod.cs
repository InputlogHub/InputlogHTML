using System;
using InputLog.Core.Analyses;

namespace InputLog.Core.Reporting
{
    /// <summary>
    /// Function that may be called on the AnalysisSummary that reports a 
    /// value as specified by this function.
    /// </summary>
    public class ReportMethod
    {
        /// <summary>
        /// The AbstractAnalysisSummary that is bound to this report function.
        /// Executing this method will call the BoundMethod in this ReportMethod binding it
        /// to the BoundSummary (using BoundSummary as 'this' object for the ReportMethod).
        /// </summary>
        private readonly AbstractAnalysisSummary BoundSummary;

        /// <summary>
        /// The name of the method bound to this ReportMethod object. It is the method that will be called
        /// and bound to BoundSummary when using this ReportMethod.
        /// </summary>
        private readonly string BoundMethodName;

        /// <summary>
        /// Create the report method class.
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="reportingMethod"></param>
        public ReportMethod(AbstractAnalysisSummary summary, string reportingMethod)
        {
            BoundMethodName = reportingMethod;
            BoundSummary = summary;
        }


        /// <summary>
        /// Invode the bound method on the summary its bound too, returning the result of the
        /// method as a string.
        /// </summary>
        /// <returns>A string with the result of calling the method.</returns>
        public ReportValue Invoke()
        {
            // The requirements for this cast to ReportElement are checked in the 
            // getReportTargets() method of the AbstractAnalysisSummary. Only reporting
            // methods that return a ReportElement-assignable type are considered as ReportMethods.
            //
            return (ReportValue) BoundSummary.GetType().InvokeMember(
                BoundMethodName,
                System.Reflection.BindingFlags.InvokeMethod,
                Type.DefaultBinder,
                BoundSummary,
                new object[] {}
            );
        }
        


    }
}
