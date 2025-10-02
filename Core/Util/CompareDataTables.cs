using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using InputLog.Core.Analyses;

namespace InputLog.Core.Util
{
    /// <summary>
    ///     Compares two data tables and create two new tables based on the compared results.
    ///     Both table1 and table2 should have the same structure.
    /// </summary>
    public class CompareDataTables
    {
        #region Fields
        /// <summary>
        ///     Rows common in both tables.
        /// </summary>
        public DataTable CommonTable { get; private set; }

        /// <summary>
        ///     Rows different in both tables.
        /// </summary>
        public DataTable DiffTable { get; private set; }

        /// <summary>
        /// The field to base the comparison on.
        /// </summary>
        private string DataField { get; set; }
        #endregion

        /// <summary>
        ///     Comparing two DataTables and producing a new table with the common rows and one with the diff.
        /// </summary>
        /// <param name="table1">The first table.</param>
        /// <param name="table2">The second table to compare with the first.</param>
        /// <param name="field">The field to base the comparison on.</param>
        public void CompareTables(DataTable table1, DataTable table2, string field)
        {
            if (!CheckStructure(table1, table2))
            {
                throw new AnalysisException("Could not compare DataTables in LinguisticAnalysis.");
            }

            DataField = field;
            CommonTable = new DataTable();
            CommonTable = table1.Clone();
            DiffTable = new DataTable();
            DiffTable = table1.Clone();
            var rowValue = new Object[1];
            var dtTableCompared = new DataTable();
            var dtTableSearched = new DataTable();
            if (table1.Rows.Count >= table2.Rows.Count)
            {
                dtTableCompared = table1.Copy();
                dtTableSearched = table2.Copy();
            }
            else if (table1.Rows.Count < table2.Rows.Count)
            {
                dtTableCompared = table2.Copy();
                dtTableSearched = table1.Copy();
            }

            foreach (DataRow row in dtTableCompared.Rows)
            {
                rowValue[0] = row[DataField];
                if (dtTableSearched.Rows.Count >= 0)
                {
                    dtTableSearched.DefaultView.Sort = DataField;
                    int intRowFound = dtTableSearched.DefaultView.Find(rowValue[0]);
                    if (intRowFound <= -1)
                    {
                        DataRow newRow = DiffTable.NewRow();
                        newRow.ItemArray = row.ItemArray;
                        DiffTable.Rows.Add(newRow);
                    }
                    else
                    {
                        DataRow newRow = CommonTable.NewRow();
                        newRow.ItemArray = row.ItemArray;
                        CommonTable.Rows.Add(newRow);
                    }
                }
            }
            foreach (DataRow row in dtTableSearched.Rows)
            {
                rowValue[0] = row["Field1"];
                if (dtTableCompared.Rows.Count >= 0)
                {
                    dtTableCompared.DefaultView.Sort = DataField;
                    int intRowFound = dtTableCompared.DefaultView.Find(rowValue[0]);
                    if (intRowFound <= -1)
                    {
                        DataRow newRow = DiffTable.NewRow();
                        newRow.ItemArray = row.ItemArray;
                        DiffTable.Rows.Add(newRow);
                    }
                    else
                    {
                        DataRow newRow = CommonTable.NewRow();
                        newRow.ItemArray = row.ItemArray;
                        CommonTable.Rows.Add(newRow);
                    }
                }
            }
        }

        /// <summary>
        ///     Two DataTables have the same structure if the number of columns is the same in both tables
        ///     and if for each data column in the first DataTable a column exists in the other table that
        ///     also is of the same type, regardless of order.
        /// </summary>
        /// <param name="table1">The first table.</param>
        /// <param name="table2">The second table to compare with the first.</param>
        /// <returns></returns>
        private static bool CheckStructure(DataTable table1, DataTable table2)
        {
            var colDef1 = new List<int>();
            var colDef2 = new List<int>();

            if (table1.Columns.Count == table2.Columns.Count)
            {
                colDef1.AddRange(from DataColumn col in table1.Columns select GetHashCode(col));
                colDef2.AddRange(from DataColumn col in table1.Columns select GetHashCode(col));
                return colDef1.Intersect(colDef2).Count() == colDef1.Count;
            }
            return false;
        }

        private static int GetHashCode(DataColumn col)
        {
            int hash = 17;
            hash = 31*hash + col.ColumnName.GetHashCode();
            hash = 31*hash + col.DataType.GetHashCode();

            return hash;
        }
    }
}