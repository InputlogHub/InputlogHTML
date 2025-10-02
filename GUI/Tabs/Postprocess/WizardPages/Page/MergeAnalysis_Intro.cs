using System;
using System.Collections.Generic;
using GUI.Wizard;
using InputLog.Core.Merging;
using GUI.Tabs.Postprocess.WizardPages.Form;
using InputLog.Core.Merging.Analyses;

namespace GUI.Tabs.Postprocess.WizardPages.Page
{
    /// <summary>
    /// First Page of the MergeAnalysis Wizard.
    /// </summary>
    internal class MergeAnalysis_Intro : WizardPage
    {
        #region Fields

        /// <summary>
        /// The data key that is used for setting the analysis type
        /// selected in this form.
        /// </summary>
        public const string ANALYSIS_KEY = "analysis_type";

        #endregion

        /// <summary>
        /// Construct the first page of the merge analysis.
        /// </summary>
        public MergeAnalysis_Intro()
        {
            MergeAnalysis_Intro_Form introForm = new MergeAnalysis_Intro_Form();
            FileSelectPage fileSelectPage = new FileSelectPage {Previous = this};
            introForm.Changed += delegate { fileSelectPage.Reset(); };
            Control = introForm;
            Next = fileSelectPage;
        }

        /// <summary>
        /// Get the data filled in at this page. For the first merge analysis page
        /// this will be whether we will be information about which type of analysis
        /// files we will be merging.
        /// </summary>
        /// <returns>A dictionary with 'analysis_type' as key and some type of merging
        /// algorithm associated with the analysis type. This is either 'vertical' or 
        /// 'horizontal'.</returns>
        protected override Dictionary<string, Object> GetData()
        {
            var controlForm = ((MergeAnalysis_Intro_Form) Control);

            // Which type of analysis will we perform?
            if (controlForm.MergeHorizontalIsChecked())
            {
                if (Data.ContainsKey(ANALYSIS_KEY))
                {
                    Data[ANALYSIS_KEY] = AnalysisMerge.Direction.HORIZONTAL;
                }
                else
                {
                    Data.Add(ANALYSIS_KEY, AnalysisMerge.Direction.HORIZONTAL);
                }
            }
            else if (controlForm.MergeVerticalIsChecked())
            {
                if (Data.ContainsKey(ANALYSIS_KEY))
                {
                    Data[ANALYSIS_KEY] = AnalysisMerge.Direction.VERTICAL;
                }
                else
                {
                    Data.Add(ANALYSIS_KEY, AnalysisMerge.Direction.VERTICAL);
                }
            }

            // Merge with data from previous pages (if any).
            return base.GetData();
        }
    }
}