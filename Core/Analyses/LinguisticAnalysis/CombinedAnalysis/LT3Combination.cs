using System;
using System.Collections.Generic;
using System.Data;
using InputLog.Core.Analyses.LinguisticAnalysis.Webservice;
using InputLog.Core.Pipes;
using InputLog.Core.Util.Progress;

namespace InputLog.Core.Analyses.LinguisticAnalysis.CombinedAnalysis
{
    internal class LT3Combination : BaseProcess
    {
        #region Fields

        /// <summary>
        /// Table to hold three versions of the input text (final product, inserts in context, deletions in context)
        /// </summary>
        private DataTable _textTable;

        /// <summary>
        /// The tables with the details for every token in the input text.
        /// </summary>
        private DataTable _prefilledTable;

        /// <summary>
        /// The language (ISO code) of the input text.
        /// </summary>
        private readonly string _lang;
        /// <summary>
        /// The CSV string returned by the web service.
        /// </summary>
        private static string CSVOut { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lang">the text language</param>
        public LT3Combination(string lang)
        {
            _lang = lang.ToLower();
        }

        /// <summary>
        /// Processing the raw data with a combination of linguistic tools.
        /// </summary>
        /// <param name="linguisticProcess">The dataset to analyze.</param>
        /// <returns>The results in a dataSet</returns>
        protected override DataSet Process(DataSet linguisticProcess)
        {
            ReportProgress(this, new ProgressEventArgs("Working on the LT3 combined analyses"));

            // Fetching the data.
            _textTable = linguisticProcess.Tables["textStrings"];
            _prefilledTable = linguisticProcess.Tables["infoTable"];
            var row = _textTable.Rows[0];
            var reconstructedTxt = row.ItemArray[1].ToString();

            // Executing the process.
            PerformCombinedAnalysis(reconstructedTxt);

            // Updating the DataSet.
            UpdateDataSet(linguisticProcess);

            // Back into the pipeline.
            return linguisticProcess;
        }

        /// <summary>
        /// Sending the text to the web service with the combined analysis tools and 
        /// sending the returned results to the FillTable method. 
        /// </summary>
        /// <param name="reconstructedTxt">the string to analyze</param>
        private void PerformCombinedAnalysis(string reconstructedTxt)
        {
            var webService = new LinguisticWebserviceHandler();

            //// Debug: write the text sent to a debug file.
            //string appPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            //string filename = "debug.txt";
            //string filePath = Path.Combine(appPath, filename);
            //string uniquePath = PathSanitizer.Uniquify(filePath);
            //Console.WriteLine("Debug file written at: \"" + uniquePath + "\"");
            //File.WriteAllText(uniquePath, reconstructedTxt);

            CSVOut = webService.CallAndWait(_lang, reconstructedTxt);
            FillTable(CSVOut);
        }

        /// <summary>
        /// Putting the linguistic analysis results into the appropriate columns of the dataTable.
        /// </summary>
        /// <param name="analysisResult">CSV string with the results of the analysis by the external service.</param>
        private void FillTable(string analysisResult)
        {
            // CSV string returned by the LT3PreProcessor Server.
            if (analysisResult == null) throw new ArgumentNullException(nameof(analysisResult));

            // Inner loop counter.
            var inLoop = 0;

            // An array with a line with linguistic data for every token.
            var tokenResult = analysisResult.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
           // Console.WriteLine("** Result rows expected: " + tokenResult.Length);

            // Outer loop over the linguistic data.
            for (int outLoop = 0; outLoop < tokenResult.Length && outLoop < _prefilledTable.Rows.Count; outLoop++)
            {
                // Splitting the token line.
                var linguisticParts = tokenResult[outLoop].Split(' ');
               // Console.WriteLine("Token Result Row " + outLoop);

                // Inner loop over the PrefilledTable.
                for (; inLoop < _prefilledTable.Rows.Count;)
                {
                   // Console.WriteLine("Prefilled Row " + inLoop);

                    // If 'true', skip a row when linguistic token and reconstructed token do not match
                    // and both data series needs to synchronize.
                    if (FillDataSlots(_prefilledTable.Rows[inLoop], linguisticParts))
                    {
                        outLoop++;
                    }
                    break;
                }
                inLoop++;
            }
            _prefilledTable.AcceptChanges();
        }

        /// <summary>
        /// Putting data in the slots assigned to the linguistic analysis results.
        /// </summary>
        /// <param name="dataArray">Array with one row of placeholders.</param>
        /// <param name="linguisticParts">Array with one row of linguistic result data.</param>
        /// <returns>bool 'true' if a line in the data table has to be skipped to synchronize linguistic data
        /// with data from token reconstruction.</returns>
        private static bool FillDataSlots(DataRow dataArray, IList<string> linguisticParts)
        {
            var splitS = Array.Empty<string>();
            var charSeparator1 = new[] {'('};
            var charSeparator2 = new[] {'-'};

            // Skipping the first 14 slots that have their content already prefilled with pause time information.
            // PoS and Chunk are split into two parts, each in its own column.
            // The linguistic parts contain 11 elements (0 to 10). Two elements are split, so we need 13 slots in the dataArray.
            // Because the first 13 slots are prefilled with pause calculations, we start at dataArray[14] up to dataArray[26].
            for (var k = 14; k < 27; k++)
            {
                switch (k)
                {
                    // Token
                    case 14:
                        // If the token as it is returned from the linguistic analysis is different 
                        // from the reconstructed token, then skip (return true) this line of linguistic data.
                        if (dataArray[3].ToString().Trim().Equals(linguisticParts[0]))
                        {
                            dataArray[14] = linguisticParts[0];
                        }
                        else
                        {
                            return true;
                        }
                        // Console.WriteLine("col_14 Token: " + dataArray[14]);
                        break;

                    // PoS part A
                    case 15:
                        splitS = linguisticParts[1].Split(charSeparator1, StringSplitOptions.RemoveEmptyEntries);
                        if (splitS.Length == 0)
                        {
                            dataArray[15] = "-";
                            //Console.WriteLine("col_15 No Pos A: " + dataArray[15]);
                            break;
                        }
                        dataArray[15] = splitS[0];
                        //Console.WriteLine("col_15 Pos A: " + dataArray[15]);
                        break;

                    // PoS part B
                    case 16:
                        if (splitS.Length == 1)
                        {
                            dataArray[16] = "-";
                            //Console.WriteLine("col_16 No Pos B: " + dataArray[16]);
                            break;
                        }
                        dataArray[16] = splitS[1].Substring(0, splitS[1].Length - 1);
                        //Console.WriteLine("col_16 Pos B: " + dataArray[16]);
                        break;

                    // PoS Probability
                    case 17:
                        dataArray[17] = linguisticParts[2];
                        //Console.WriteLine("col_17 PosProb: " + dataArray[17]);
                        break;

                    // Lemma
                    case 18:
                        dataArray[18] = linguisticParts[3];
                        //Console.WriteLine("col_18 Lemma: " + dataArray[18]);
                        break;

                    // Lemma Probability
                    case 19:
                        dataArray[19] = linguisticParts[4];
                        //Console.WriteLine("col_19 LemmaProb: " + dataArray[19]);
                        break;

                    // Chunk part A
                    case 20:
                        splitS = linguisticParts[5].Split(charSeparator2, StringSplitOptions.RemoveEmptyEntries);
                        if (splitS.Length == 0)
                        {
                            dataArray[20] = "-";
                            //Console.WriteLine("col_20 No Chunk A: " + dataArray[20]);
                            break;
                        }
                        dataArray[20] = splitS[0];
                        //Console.WriteLine("col_20 Chunk A: " + dataArray[20]);
                        break;

                    // Chunk part B
                    case 21:
                        if (splitS.Length == 1)
                        {
                            dataArray[21] = "-";
                            //Console.WriteLine("col_21 No Chunk B: " + dataArray[21]);
                            break;
                        }
                        dataArray[21] = splitS[1];
                        //Console.WriteLine("col_21 Chunk B: " + dataArray[21]);
                        break;

                    // Log2 of the word frequency. Rounded value. More information:
                    // http://stackoverflow.com/questions/12003719/log-of-a-very-large-number
                    case 24:
                        bool isNumeric = long.TryParse(linguisticParts[8], out _);
                        if (isNumeric)
                        {
                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(linguisticParts[8]);
                            if (bytes[bytes.Length - 1] < 128)
                            {
                                int log = 0;
                                while ((bytes[bytes.Length - 1] >> log) > 0) log++;
                                dataArray[24] = log + bytes.Length*8 - 9;
                            }
                        }
                        else
                        {
                            dataArray[24] = float.NaN;
                        }
                        //Console.WriteLine("col_24 Log Freq: " + dataArray[24] + " org.freq: " + linguisticParts[8]);
                        break;

                     // All other linguistic data
                    default:
                        dataArray[k] = linguisticParts[k - 16];
                        //Console.WriteLine("col_" + k + " Other: " + dataArray[k]);

                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Updating the dataTables.
        /// </summary>
        /// <param name="linguisticProcess"></param>
        private void UpdateDataSet(DataSet linguisticProcess)
        {
            // Removes the old tables.
            linguisticProcess.Tables.Remove("textStrings");
            linguisticProcess.Tables.Remove("infoTable");
            // Replaces original TextTable with the updated version.
            linguisticProcess.Tables.Add(_textTable);
            // Replaces original AnalysisTable with the updated version.
            linguisticProcess.Tables.Add(_prefilledTable);
        }
    }
}