using System;
using System.Data;
using InputLog.Core.Util.Progress;
using InputLog.Core.Pipes;

namespace Inputlog.Core.Analyses.LinguisticAnalysis.Chunking
{
    public class Chunking : BaseProcess
    {
        #region Fields
        // Table to hold three versions of the linguisticProcess text (final product, inserts in context, deletions in context)
        private DataTable TextTable;
        // The table with the details for every token in the linguisticProcess text.
        private DataTable AnalysisTable;
        #endregion

        /// <summary>
        /// Chunker
        /// </summary>
        /// <param name="linguisticProcess"></param>
        /// <returns></returns>
        protected override DataSet Process(DataSet linguisticProcess)
        {
            ReportProgress(this, new ProgressEventArgs("Working on the Chunker"));
            AnalysisTable = linguisticProcess.Tables["processes"];
            ExpandAnalysisTable();

            // Dowloadloads the text strings.
            TextTable = linguisticProcess.Tables["textStrings"];
            var row = TextTable.Rows[2];
            var wnotation = row.ItemArray[1].ToString();

            // Executes the process.
            PerformChunk(wnotation);
            // Updating.
            UpdateDataSet(linguisticProcess);
            // Back into the pipeline.
            return linguisticProcess;
        }

        /// <summary>
        /// NotImplemented
        /// </summary>
        /// <param name="wnotation">The strings to perform the chunkung on.</param>
        private void PerformChunk(string wnotation)
        {
            // Do something with wnotation and put the result in the table.
            FillTable("Chunk_");
        }

        /// <summary>
        /// Fills the table with the outcome of the chunker.
        /// </summary>
        /// <param name="txt"></param>
        private void FillTable(object txt)
        {
            if (txt == null) throw new ArgumentNullException("txt");
            for (var i = 0; i < AnalysisTable.Rows.Count; i++)
            {
                AnalysisTable.Rows[i]["Chunk"] = txt.ToString() + i;
            }
            AnalysisTable.AcceptChanges();
        }

        /// <summary>
        /// Generates new column for the chunker.
        /// </summary>
        private void ExpandAnalysisTable()
        {
            AnalysisTable.Columns.Add("Chunk", typeof(string));
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
