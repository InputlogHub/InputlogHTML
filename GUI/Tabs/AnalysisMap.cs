using System;
using System.Collections.Generic;
using GUI.Tabs.Analyze.AnalysesControls.Bigram;
using GUI.Tabs.Analyze.AnalysesControls.Copytask;
using GUI.Tabs.Analyze.AnalysesControls.Fluency;
using GUI.Tabs.Analyze.AnalysesControls.General;
using GUI.Tabs.Analyze.AnalysesControls.Linear;
using GUI.Tabs.Analyze.AnalysesControls.Linguistic;
using GUI.Tabs.Analyze.AnalysesControls.Pause;
using GUI.Tabs.Analyze.AnalysesControls.ProcessGraph;
using GUI.Tabs.Analyze.AnalysesControls.Revision;
using GUI.Tabs.Analyze.AnalysesControls.SNotation;
using GUI.Tabs.Analyze.AnalysesControls.Source;
using GUI.Tabs.Analyze.AnalysesControls.Summary;
using GUI.Tabs.Analyze.AnalysesControls.Token;
using GUI.Tabs.Analyze.AnalysesControls.WordPauses;
using GUI.Visualization;
using InputLog.Core.Analyses.Bigram;
using InputLog.Core.Analyses.Copytask;
using InputLog.Core.Analyses.Fluency;
using InputLog.Core.Analyses.Focus;
using InputLog.Core.Analyses.General;
using InputLog.Core.Analyses.GeneralEyetrack;
using InputLog.Core.Analyses.Linear;
using InputLog.Core.Analyses.Pause;
using InputLog.Core.Analyses.Revision.Notations;
using InputLog.Core.Analyses.Summary;
using InputLog.Core.Analyses.WordPauses;

namespace GUI.Tabs
{
    /// <summary>
    ///     This class maps analysis names to their different elements.
    ///     It maps analysis names to their analyzer classes, their analyses, and
    ///     their summaries.
    ///     If a new Analysis is added, this is where you should add the extra entries for the analysis.
    /// </summary>
    public static class AnalysisMap
    {
        /*
         * >>>> BEFORE CHANGING <<<<<
         * IMPORTANT NOTE: 
         * When changing these strings (the analysis names) make sure to make those same changes
         * in the Core.Reporting.Resources.ReportMappingResources.resx file as well!
         *  
         * Do not change the name of any analysis here, without changing the references
         * to that name as well in the aforementioned resource file. Otherwise the 
         * reporting functionality will no longer work!!
         * 
         */
        public static readonly string BIGRAM_ANALYSIS = "Bigram Analysis";
        public static readonly string FLUENCY_ANALYSIS = "Fluency Analysis";
        public static readonly string GENERAL_ANALYSIS = "General Analysis";
        public static readonly string GENERAL_EYETRACK_ANALYSIS = "General Eyetrack Analysis";
        public static readonly string LINEAR_ANALYSIS = "Linear Analysis";
        public static readonly string PAUSE_ANALYSIS = "Pause Analysis";
        public static readonly string REVISION_ANALYSIS = "Revision Analysis";
        public static readonly string SUMMARY_ANALYSIS = "Summary Analysis";
        public static readonly string WORD_PAUSE_ANALYSIS = "Word Pause Analysis";
        public static readonly string PROCESS_GRAPH_ANALYSIS = "Process Graph Analysis";
        public static readonly string SNOTATION_ANALYSIS = "S-Notation";
        public static readonly string FOCUS_ANALYSIS = "Focus Analysis";
        public static readonly string LINGUISTIC_ANALYSIS = "Linguistic Analysis";
        public static readonly string TOKEN_ANALYSIS = "Token Analysis";
        public static readonly string COPYTASK_ANALYSIS = "Copytask Analysis";


        public static readonly Dictionary<string, string> ANALYSIS_NAME_TO_ANALYZER_NAME =
            new Dictionary<string, string>
            {
                {GENERAL_ANALYSIS, GeneralAnalyzer.NAME},
                {SUMMARY_ANALYSIS, SummaryAnalyzer.NAME},
                {PAUSE_ANALYSIS, PauseAnalyzer.NAME},
                {PROCESS_GRAPH_ANALYSIS, ProcessGraphAnalyzer.NAME},
                {FLUENCY_ANALYSIS, FluencyAnalyzer.NAME},
                {LINEAR_ANALYSIS, LinearAnalyzer.NAME},
                {FOCUS_ANALYSIS, FocusAnalyzer.NAME},
                {REVISION_ANALYSIS, RevisionMatrixAnalyzer.NAME},
                {SNOTATION_ANALYSIS, SNotationAnalyzer.NAME},
                {BIGRAM_ANALYSIS, BigramAnalyzer.NAME},
                {WORD_PAUSE_ANALYSIS, WordPausesAnalyzer.NAME},
                {LINGUISTIC_ANALYSIS, LinguisticAnalyzer.NAME},
                {TOKEN_ANALYSIS, TokenAnalyzer.NAME},
                {GENERAL_EYETRACK_ANALYSIS, GeneralEyetrackAnalyzer.NAME},
                {COPYTASK_ANALYSIS, CopytaskAnalyzer.NAME}
            };

        public static readonly Dictionary<string, Type> ANALYZER_NAME_TO_ANALYZER_CONTROL =
            new Dictionary<string, Type>
            {
                {ANALYSIS_NAME_TO_ANALYZER_NAME[GENERAL_ANALYSIS], typeof (GeneralAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[SUMMARY_ANALYSIS], typeof (SummaryAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[PAUSE_ANALYSIS], typeof (PauseAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[PROCESS_GRAPH_ANALYSIS], typeof (ProcessGraphAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[FLUENCY_ANALYSIS], typeof (FluencyAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[LINEAR_ANALYSIS], typeof (LinearAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[FOCUS_ANALYSIS], typeof (FocusAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[REVISION_ANALYSIS], typeof (RevisionMatrixAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[SNOTATION_ANALYSIS], typeof (SNotationAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[BIGRAM_ANALYSIS], typeof (BigramAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[WORD_PAUSE_ANALYSIS], typeof (WordPausesAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[LINGUISTIC_ANALYSIS], typeof (LinguisticAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[TOKEN_ANALYSIS], typeof (TokenAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[GENERAL_EYETRACK_ANALYSIS], typeof (GeneralEyetrackAnalysisControl)},
                {ANALYSIS_NAME_TO_ANALYZER_NAME[COPYTASK_ANALYSIS], typeof (CopytaskAnalysisControl)}
            };

        public static readonly Dictionary<string, Type> ANALYSIS_NAME_TO_ANALYSIS_SUMMARY =
            new Dictionary<string, Type>
            {
                {BIGRAM_ANALYSIS, typeof (BigramAnalysisSummary)},
                {FLUENCY_ANALYSIS, typeof (FluencyAnalysisSummary)},
                {GENERAL_ANALYSIS, typeof (GeneralAnalysisSummary)},
                {GENERAL_EYETRACK_ANALYSIS, typeof (GeneralEyetrackAnalysisSummary)},
                {LINEAR_ANALYSIS, typeof (LinearAnalysisSummary)},
                {PAUSE_ANALYSIS, typeof (CompoundPauseAnalysisSummary)},
                {REVISION_ANALYSIS, typeof (RevisionMatrixSummary)},
                {SUMMARY_ANALYSIS, typeof (SummaryAnalysisSummary)},
                {WORD_PAUSE_ANALYSIS, typeof (WordPausesAnalysisSummary)},
                {PROCESS_GRAPH_ANALYSIS, typeof (ProcessGraph)},
                {SNOTATION_ANALYSIS, typeof (SNotationSummary)},
                {FOCUS_ANALYSIS, typeof (FocusAnalysisSummary)},
                {COPYTASK_ANALYSIS, typeof (CopytaskAnalysisSummary)}
            };
    }
}
