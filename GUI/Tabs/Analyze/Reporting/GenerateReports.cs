using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Reporting;
using InputLog.Core.Reporting.Report;
using InputLog.Core.Util;

namespace GUI.Tabs.Analyze.Reporting
{
    public partial class GenerateReports : Form
    {
        public GenerateReports()
        {
            InitializeComponent();

            // Register Event Handlers
            Load += GenerateReportsLoad;
        }

        /// <summary>
        /// Before the window is first displayed, load the data-table with all possible 
        /// reporting targets. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateReportsLoad(object sender, EventArgs e)
        {
            //Dictionary<string, List<string>> possibleTargetsPerAnalysis = new Dictionary<string, List<string>>();

            //// For each analysis find the possible reporting targets
            //foreach (KeyValuePair<string, Type> availableAnalysis in AnalysisMap.ANALYSIS_NAME_TO_ANALYSIS_SUMMARY)
            //{
            //    List<string> possibleTargets = ReportController.IdentifyReportTargets(availableAnalysis.Value);
            //    string analysisName = availableAnalysis.Key;

            //    if (possibleTargetsPerAnalysis.Keys.Contains(analysisName))
            //    {
            //        possibleTargetsPerAnalysis[analysisName].AddRange(possibleTargets);
            //    }
            //    else
            //    {
            //        possibleTargetsPerAnalysis.Add(analysisName, possibleTargets);
            //    }
            //}

            // Initialise the Controls the user uses to select reporting targets and select their blocks.
            // We now have the reporting targets for each analysis and thus can initialize the data.
            //foreach (KeyValuePair<string, List<string>> analysisTargets in possibleTargetsPerAnalysis)
            //{
            //    string analysisName = analysisTargets.Key;
            //    List<string> reportingTargets = analysisTargets.Value;

            //    // Add Control to the list of GUI selector controls that 
            //    // allows the user to select which reporting
            //    // elements of each analysis he or she wishes to add to the report
            //    // 
            //    // Only add the Control if the list of targets for reporting is not empty.
            //    if (reportingTargets.Count > 0)
            //    {
            //        AnalysisReportingTargets reportingTargetsUserControl =
            //            new AnalysisReportingTargets(analysisName, reportingTargets);

            //        InnerLayoutPanel.Controls.Add(reportingTargetsUserControl);
            //    }
            //}

            // Initialise the Controls the user uses to select reporting targets and select their blocks.
            // We now have the reporting targets for each analysis and thus can initialize the data.
            var possibleTargetsPerAnalysis = new FilteredGroupedListAccumulator<Report.ReportResource, string>(
                (resource) => resource.Analysis
            );
            Report.LoadResources("en-us");
            IEnumerable<Report.ReportResource> reportTargets = Report.GetResources();
            possibleTargetsPerAnalysis.Accumulate(reportTargets);
            
            foreach (string analysis in possibleTargetsPerAnalysis.Keys)
            {
                // Add Control to the list of GUI selector controls that 
                // allows the user to select which reporting
                // elements of each analysis he or she wishes to add to the report

                AnalysisReportingTargets reportingTargetsUserControl =
                    new AnalysisReportingTargets(analysis, possibleTargetsPerAnalysis[analysis].ToList());

                InnerLayoutPanel.Controls.Add(reportingTargetsUserControl);

            }
        }

        /// <summary>
        /// Return all the targets that the user wishes to add to the reports. Some characteriscs
        /// about how the user would like to see them reported and the analyses that are required
        /// to be executed for each reportingtarget.
        /// </summary>
        /// <returns>A mapping of all the targets the user wants added to the report and their 
        /// information, mapped to the name of the analysis that must be executed in order 
        /// to acquire the information that needs to be reported.</returns>
        public Dictionary<string, List<ReportingTarget>> GetSelectedReportingTargetsByAnalysis()
        {
            Dictionary<string, List<ReportingTarget>> targetsByAnalysis = 
                new Dictionary<string, List<ReportingTarget>>();

            // Loop over all the analyses with their targets to find out which have been selected
            // and to get the required information about those that have been selected.
            //
            foreach (AnalysisReportingTargets analysisTargets in InnerLayoutPanel.Controls)
            {
                List<ReportingTarget> selectedTargetsForAnalysis = analysisTargets.GetSelectedReportingTargets();

                if (selectedTargetsForAnalysis.Count > 0)
                {
                    targetsByAnalysis.Add(
                        analysisTargets.AnalysisName,
                        selectedTargetsForAnalysis
                    );
                }
            }

            return targetsByAnalysis;
        }

        /// <summary>
        /// Change the appearance of the standard communication label by changing the 
        /// border and its color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommunicationLabelPaint(object sender, PaintEventArgs e)
        {
            // Set the border around the label
            ControlPaint.DrawBorder(e.Graphics,
                CommunicationLabel.DisplayRectangle,
                Color.DodgerBlue,
                ButtonBorderStyle.Solid
            );
        }

        //
        // Selecting and deselecting report elements
        // 
        #region select

        /// <summary>
        /// Select all reporting targets from all the analyzes that have targets available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllButtonClick(object sender, EventArgs e)
        {
            foreach (AnalysisReportingTargets analysisReportingTargets in InnerLayoutPanel.Controls)
            {
                analysisReportingTargets.SelectAll();
            }
        }

        /// <summary>
        /// Deselect any reporting target that is available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeselectAllButtonClick(object sender, EventArgs e)
        {
            foreach (AnalysisReportingTargets analysisReportingTargets in InnerLayoutPanel.Controls)
            {
                analysisReportingTargets.DeselectAll();
            }
        }
        #endregion

        // 
        // Importing and Exporting previously created Reporting Configurations
        // 
        #region configuration saving/loading

        private void ImportButtonClick(object sender, EventArgs e)
        {

        }

        private void ExportButtonClick(object sender, EventArgs e)
        {

        }
        #endregion

        /// <summary>
        /// Edit the existing blocks. This pops up a form that allows the user to 
        /// rename, add and or delete blocks. After the changes have been accepted
        /// they will be saved to the ReportingBlocks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditBlocksClick(object sender, EventArgs e)
        {
            EditReportingBlocks editBlocks = new EditReportingBlocks();
            editBlocks.ShowDialog();
        }
    }
}
