using System;
using System.Data;
using InputLog.Core.Util.Progress;
using InputLog.Core.Pipes;

namespace Inputlog.Core.Analyses.LinguisticAnalysis.Syllabification
{
    public class Syllabification : BaseProcess
    {
        #region Fields
        // Table to hold three versions of the linguisticProcess text (final product, inserts in context, deletions in context)
        private DataTable TextTable;
        // The table with the details for every token in the linguisticProcess text.
        private DataTable AnalysisTable;
        // The language (ISO code) of the linguisticProcess text.
        private readonly String Lang;
        #endregion

        public Syllabification(String lang)
        {
            Lang = lang;
        }

        /// <summary>
        /// Syllabification
        /// </summary>
        /// <param name="linguisticProcess"></param>
        /// <returns></returns>
        protected override DataSet Process(DataSet linguisticProcess)
        {
            ReportProgress(this, new ProgressEventArgs("Working on the Syllabification"));

            AnalysisTable = linguisticProcess.Tables["processes"];
            ExpandAnalysisTable();

            // Dowloadloads the text strings.
            TextTable = linguisticProcess.Tables["textStrings"];
            var row = TextTable.Rows[2];
            var wnotation = row.ItemArray[1].ToString();

            // Executes the process.
            PerformSyllabification(wnotation);

            // Updating.
            UpdateDataSet(linguisticProcess);

            // Back into the pipeline.
            return linguisticProcess;
        }

        /// <summary>
        /// NotImplemented
        /// </summary>
        /// <param name="wnotation">The strings to perform the syllabification on.</param>
        private void PerformSyllabification(string wnotation)
        {
            // Do something with wnotation and put the result in the table.
            String text = wnotation.Replace('\u00B7', ' ');
            //var ssc = new SyllabifyServerClient();
            //String Out = ssc.Syllabify(text, Lang.ToLower());
            //FillTable(Out);
        }
        
        /// <summary>
        /// Fills the table with the outcome of the Syllabification.
        /// </summary>
        /// <param name="txt"></param>
        private void FillTable(Object txt)
        {
            var s = txt.ToString();
            var parts = s.Split(' ');
            if (s == null) throw new ArgumentNullException("txt");
            var j = 0;
            for (var i = 0; i < AnalysisTable.Rows.Count; i++)
            {
                if (AnalysisTable.Rows[i]["Indices"].ToString().Equals("0") && j < parts.Length)
                {
                    AnalysisTable.Rows[i]["Syllable"] = parts[j];
                    j++;
                }
                else
                {
                    AnalysisTable.Rows[i]["Syllable"] = "Syllable_" + i;
                }
            }
            AnalysisTable.AcceptChanges();
        }

        /// <summary>
        /// Generates new column for the Syllabification
        /// </summary>
        private void ExpandAnalysisTable()
        {
            AnalysisTable.Columns.Add("Syllable", typeof(string));
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
