using System;
using System.Data;
using InputLog.Core.Util.Progress;
using InputLog.Core.Pipes;
using InputLog.Core.Util.Server;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace Inputlog.Core.Analyses.LinguisticAnalysis.PartOfSpeech
{
    public class PartOfSpeech : BaseProcess
    {
        #region Fields
        // Table to hold three versions of the linguisticProcess text (final product, inserts in context, deletions in context)
        private DataTable TextTable;
        // The table with the details for every token in the linguisticProcess text.
        private DataTable AnalysisTable;
        #endregion

        /// <summary>
        /// Part of Speech tagger
        /// </summary>
        /// <param name="linguisticProcess"></param>
        /// <returns></returns>
        protected override DataSet Process(DataSet linguisticProcess)
        {
            ReportProgress(this, new ProgressEventArgs("Working on the Part of Speech"));
            AnalysisTable = linguisticProcess.Tables["processes"];
            ExpandAnalysisTable();

            // Loads the text strings.
            TextTable = linguisticProcess.Tables["textStrings"];
            var row = TextTable.Rows[2];
            var wnotation = row.ItemArray[1].ToString();

            // Executes the process.
            PerformPartOfSpeech( wnotation);

            // Updating.
            UpdateDataSet(linguisticProcess);

            // Back into the pipeline.
            return linguisticProcess;
        }

        /// <summary>
        /// NotImplemented
        /// </summary>
        /// <param name="wnotation">The strings to perform the PartOfSpeech on.</param>
        private void PerformPartOfSpeech(string wnotation)
        {
            /*String S = "\"" + wnotation.Replace('\u00B7', ' ').Substring(0, wnotation.IndexOf('.')) + "\"";
            System.Console.WriteLine(S);
            HttpRequestResponse Resp = new HttpRequestResponse("http://localhost:18477/Home/Test");
            Dictionary<String, String> Parameters = new Dictionary<String, String>();
            Parameters["Test"] = S;
            XmlDocument Doc = Resp.SendRequest(Parameters);
            XmlNodeList Values = Doc.GetElementsByTagName("div");
            XmlNode Value = Values.Item(0);
            String POS = Value.InnerText.Split('|')[2];*/
            FillTable("NN");
        }

        /// <summary>
        /// Fills the table with the outcome of the Part of Speech tagger.
        /// </summary>
        /// <param name="o"></param>
        private void FillTable(object o)
        {
            String S = o.ToString();
            String[] Parts = S.Split(' ');
            if (S == null) throw new ArgumentNullException("s");
            int j = 0;
            for (var i = 0; i < AnalysisTable.Rows.Count; i++)
            {
                if (AnalysisTable.Rows[i]["Indices"].ToString().Equals("0") && j < Parts.Length)
                {
                    AnalysisTable.Rows[i]["PoS"] = Parts[j];
                    j++;
                }
                else
                    AnalysisTable.Rows[i]["PoS"] = "PoS" + i;
            }
            AnalysisTable.AcceptChanges();
        }

        /// <summary>
        /// Generates new column for the Part of Speech tagger.
        /// </summary>
        private void ExpandAnalysisTable()
        {
            AnalysisTable.Columns.Add("PoS", typeof(string));
        }

        /// <summary>
        /// Replaces the DataTables in the DataSet with updated versions.
        /// </summary>
        /// <param name="linguisticProcess">The DataSet</param>
        private void UpdateDataSet(DataSet linguisticProcess)
        {
            // Removes the old tables.
            linguisticProcess.Tables.Remove("textStrings");
            linguisticProcess.Tables.Remove("processes");
            // Replaces original TextTable with the updated version.
            linguisticProcess.Tables.Add(TextTable);
            // Replaces original AnalysisTable with the updated version.
            linguisticProcess.Tables.Add(AnalysisTable);
        }
    }
}
