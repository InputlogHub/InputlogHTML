using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using InputLog.Core.IO;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Copytask
{
    /// <summary>
    ///     Write the copyTask analysis to XML. Structure used in the xml is the module/blocks structure
    ///     with entries in blocks in order to provide easy merging through existing functionality.
    /// </summary>
    public class CopytaskAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        #region Constants

        private const string StylesheetHref = "copytask_analysis.xsl";

        #endregion

        private string _previousGroupingTitle;


        /// <summary>
        ///     Create a new instance of a CopytaskAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path to where the file should be written.</param>
        public CopytaskAnalysisXMLWriter(string destinationFilePath) :
            base(destinationFilePath)
        {
            _previousGroupingTitle = null;
        }

        /// <summary>
        ///     Not implemented.
        /// </summary>
        public override XmlDocument WriteMemory(IAnalysisSummary summary)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Write the analysis of the copyTask. All the data to be written is found
        ///     in the CopytaskAnalysisSummary object passed as parameter.
        /// </summary>
        /// <param name="sessionIdentification">The session information </param>
        /// <param name="extraInfo">Extra parameters that should be outputted.</param>
        /// <param name="summary">Summary contains all the information to be written.</param>
        public override void WriteDocument(
            SessionIdentification sessionIdentification,
            IDictionary<string, IDictionary<string, object>> extraInfo,
            IAnalysisSummary summary
        )
        {
            if (!(summary is CopytaskAnalysisSummary))
                throw new AnalysisWriterException("Provided summary is not of type CopytaskAnalsysiSummary");

            var thisSummary = (CopytaskAnalysisSummary) summary;

            // Start off by adding the summary.QuestionsData as sessionIdentification information
            if (thisSummary.QuestionsData != null)
            {
                if (sessionIdentification != null)
                {
                    sessionIdentification.SessionInfo.Add("Handedness Score",
                        thisSummary.QuestionsData.Handedness.ToString(CultureInfo.InvariantCulture));
                    sessionIdentification.SessionInfo.Add("Computer", thisSummary.QuestionsData.Computer);
                    sessionIdentification.SessionInfo.Add("Keyboard Familiarity", thisSummary.QuestionsData.Keyboard);
                    sessionIdentification.SessionInfo.Add("Browser", thisSummary.QuestionsData.Browser);
                    sessionIdentification.SessionInfo.Add("Dominant Languages", thisSummary.QuestionsData.Language);
                    sessionIdentification.SessionInfo.Add("Disorder", thisSummary.QuestionsData.Disorder.ToString());
                    sessionIdentification.SessionInfo.Add("Education", thisSummary.QuestionsData.Education);
                    sessionIdentification.SessionInfo.Add("Repetition", thisSummary.QuestionsData.Repetition.ToString());
                }
            }

            WriteHeader(StylesheetHref);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
            WriteSummary(thisSummary);
            CopyStyle(new[] {StylesheetHref, COMMON_CSS, COMMON_XSL}, new[] {INPUTLOG_LOGO});
            CopyText(HTML_README);
        }

        /// <summary>
        ///     Write the content of the summary
        /// </summary>
        /// <param name="summary">
        ///     The summary containing all of the data to be
        ///     written.
        /// </param>
        private void WriteSummary(CopytaskAnalysisSummary summary)
        {
            // Write the correctnes overview first
            WriteCorrectnessScores(summary);


            /*
             * Iterate over each group in the summary according to their weight. 
             * The elements with the lightest weight will be returned first, the elements
             * with higher weights will be returned later.
             * 
             * Each group will be written in the analysis as a table.
             */
            foreach (var group in summary.WeightedIterate())
                WriteGroup(group);
        }

        /// <summary>
        ///     Given the summary print the correctness statistics
        /// </summary>
        /// <param name="summary"></param>
        private void WriteCorrectnessScores(CopytaskAnalysisSummary summary)
        {
            WriteElement("group_title", "Correctness Scores");


            // Per component
            WriteModule("Correctness per component", delegate
            {
                // Write Header
                WriteModuleBlock("header", delegate
                {
                    // First column: 
                    WriteModuleElement("component", "Component");

                    // Value columns
                    WriteModuleElement("pch_targetted", "Count (targetted)");
                    WriteModuleElement("pch_not_targetted", "Count (not targetted)");
                    WriteModuleElement("pch_correct", "Correct");
                });

                foreach (var entry in summary.CorrectnessEntries)
                {
                    // Write a row for each entry
                    var componentName = entry.Key;
                    var scores = entry.Value.GetPrettyPrinter();

                    WriteModuleBlock("component_scores__" + componentName.Replace(' ', '_'), delegate
                    {
                        WriteModuleElement("name", componentName);
                        WriteModuleElement("targetted", scores.CountTargeted);
                        WriteModuleElement("not_targetted", scores.CountNotTargeted);
                        WriteModuleElement("correct", scores.Correctness);
                    });
                }
            });

            // Write the stats
            WriteModule("Statistics", delegate
            {
                var statNames = new List<string>();
                // Write headers
                WriteModuleBlock("header", delegate
                {
                    WriteModuleElement("stats_type", "");
                    foreach (var statsType in summary.CorrectnessStatistics.Keys)
                    {
                        var elementName = "stats_" + statsType.Replace(" ", "_").ToLower();

                        this.WriteModuleElement(elementName, statsType);
                        statNames.Add(statsType);
                    }
                    WriteModuleElement("dummy", "");
                });

                // Write Values
                var printer_1 = new PrettyPrintCorrectnessStatistics(summary.CorrectnessStatistics[statNames[0]]);
                var printer_2 = new PrettyPrintCorrectnessStatistics(summary.CorrectnessStatistics[statNames[1]]);

                var zippedEntries = printer_1.Keys()
                    .Zip(printer_1.Values(), (k, v) =>
                    {
                        var list = new List<string>();
                        list.Add(k);
                        list.Add(v);
                        return list;
                    })
                    .Zip(printer_2.Values(), (l, v) =>
                    {
                        l.Add(v);
                        return l;
                    });

                foreach (var entry in zippedEntries)
                {
                    this.WriteModuleBlock("stat_values__" + entry[0].Replace(' ', '_'), delegate
                    {
                        this.WriteModuleElement("stat_type", entry[0]);
                        this.WriteModuleElement("stat_0", entry[1]);
                        this.WriteModuleElement("stat_1", entry[2]);
                        this.WriteModuleElement("dummy", "");
                    });
                }
            });
        }

        /// <summary>
        ///     Write a single group to XML. A single group (table) will be written as a module.
        ///     The module will have different blocks (rows).
        /// </summary>
        /// <param name="groups">Group to write</param>
        private void WriteGroup(CopytaskAnalysisSummary.GroupStatistics groups)
        {
            if (!string.IsNullOrWhiteSpace(groups.GroupingTitle) &&
                (groups.GroupingTitle != _previousGroupingTitle))
            {
                WriteElement("group_title", groups.GroupingTitle);
                _previousGroupingTitle = groups.GroupingTitle;
            }

            WriteModule(groups.GroupName, delegate
            {
                // Write header
                WriteModuleBlock("header", delegate
                {
                    // Header with first column element 'group'.
                    // Underneath in this column will be the names of the different groupings. For the
                    // frequency grouping, e.g.: HF, LF, Indeterminate.
                    WriteModuleElement("group", "");

                    // All rows for this module will have the same headers. But the number of headers
                    // can differ per module. So, we get one of the groupings from this module and 
                    // use that one's list of headers to print the header values for this module.
                    var dummyStats = groups[groups.Values.First(item => true)];
                    var dummyPrinter = dummyStats.GetPrettyPrinter();

                    // Write the names of the other values that are provided, these are the titles of the columns
                    // CountTargeted, CountNotTargeted, Mean, Median, ...
                    foreach (var header in dummyPrinter.Keys())
                        WriteModuleElement(header, header);
                });


                // Write rows.
                // valueId is for instance HF, or LF, or Indeterminate for the Frequency grouping.
                foreach (var valueId in groups.Values)
                    WriteModuleBlock("row-" + valueId, delegate
                    {
                        // Write the 'header' for the row
                        // First column, e.g.: HF, LF, or Indeterminate
                        WriteModuleElement("value", valueId);

                        var stats = groups[valueId];
                        var prettyPrinter = stats.GetPrettyPrinter();
                        var keys = prettyPrinter.Keys().ToList();
                        var values = prettyPrinter.Values().ToList();

                        var zipped = keys.Zip(values, (k, v) => new Pair<string, string>(k, v));
                        foreach (var entry in zipped)
                        {
                            var value = entry.Second;
                            var key = entry.First;

                            WriteModuleElement(key, value);
                        }
                        
                        //// Write the values in the row.
                        //for (var i = 0; i < values.Length; i++)
                        //{
                        //    var value = values[i];
                        //    var key = keys[i];

                        //    WriteModuleElement(key, value);
                        //}
                    });
            });
        }
    }
}