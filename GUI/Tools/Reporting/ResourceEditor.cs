using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InputLog.Core.Reporting.Report;

namespace GUI.Tools.Reporting
{
    /// <summary>
    ///     This form allows the user to change the default texts for the 
    ///     reporting resources, and also see which targets are available.
    ///     
    ///     NOTE: currently only supporst VIEWING of data, not editing.
    /// </summary>
    public partial class ResourceEditor : Form
    {
        /// <summary>
        ///     The binding source that binds the resources
        ///     to the data grid.
        /// </summary>
        private BindingSource _source;

        /// <summary>
        ///     Enumeration of all the resources that are currently
        ///     loaded.
        /// </summary>
        private IEnumerable<Report.ReportResource> _loadedResources;

        /// <summary>
        ///     Flag set to true if the resources have been changed.
        /// </summary>
        private bool _dataIsChanged;

        /// <summary>
        ///     Create a new ResourceEditor. This Form displays the different
        ///     reporting resources and their default texts and may allow 
        ///     the user to change those values.
        /// </summary>
        public ResourceEditor()
        {
            InitializeComponent();

            this._initializeDataGrid();

            List<string> languages = Report.GetSupportedLanguages();
            foreach (string language in languages)
            {
                this.LanguageComboBox.Items.Add(language);
            }

            this.LanguageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;
        }

        /// <summary>
        ///     Initializes the data grid by setting the column
        ///     properties and the binding source.
        /// </summary>
        /// for the grid.</param>
        private void _initializeDataGrid()
        {
            this._source = new BindingSource();
            this.Grid.AutoGenerateColumns = false;
            this.Grid.DataSource = this._source;
            this.Grid.AutoSize = true;
            this.Grid.CellValueChanged += Grid_CellValueChanged;

            // Add the columns
            DataGridViewColumn idColumn = new DataGridViewTextBoxColumn();
            idColumn.DataPropertyName = "BaseId";
            idColumn.Name = "ID";
            idColumn.ReadOnly = true;
            idColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            idColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            this.Grid.Columns.Add(idColumn);

            DataGridViewColumn descrColumn = new DataGridViewTextBoxColumn();
            descrColumn.DataPropertyName = "Description";
            descrColumn.Name = "Description";
            descrColumn.ReadOnly = true;
            descrColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            descrColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.Grid.Columns.Add(descrColumn);

            DataGridViewColumn labelColumn = new DataGridViewTextBoxColumn();
            labelColumn.DataPropertyName = "Label";
            labelColumn.Name = "Label";
            labelColumn.ReadOnly = true;
            labelColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            labelColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            this.Grid.Columns.Add(labelColumn);

            DataGridViewColumn introColumn = new DataGridViewTextBoxColumn();
            introColumn.DataPropertyName = "Introduction";
            introColumn.Name = "Introduction";
            introColumn.ReadOnly = true;
            introColumn.SortMode = DataGridViewColumnSortMode.Automatic;
            introColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.Grid.Columns.Add(introColumn);
        }

        /// <summary>
        ///     EventListener gets called when any cell in the dataGrid has been 
        ///     changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this._dataIsChanged = true;
        }

        /// <summary>
        ///     Load the resources coupled to the selected language in the 
        ///     interface.
        /// </summary>
        /// <param name="localization">Determines the resource language. Is converted to a culture.</param>
        private void LoadResources(string localization)
        {
            // This is more of an input argument sanity check than anything else.
            CultureInfo culture;
            try
            {
                culture = CultureInfo.CreateSpecificCulture(localization);
            }
            catch (CultureNotFoundException cnfExc)
            {
                throw new ResourceException("Could not load resources of requested language.", cnfExc);
            }

            Report.LoadResources(localization);
            this._loadedResources = Report.GetResources();

            // Sort resources
            Report.ReportResource[] sortedResources = this._loadedResources.ToArray<Report.ReportResource>();
            Array.Sort(sortedResources, new InputLog.Core.Reporting.Report.Report.ReportResourceIDComparer());

            this._source = new BindingSource();
            this.Grid.DataSource = this._source;
            this._dataIsChanged = false;

            // Display them in ascending order.
            foreach (Report.ReportResource resource in sortedResources) 
            {
                this._source.Add(resource);
            }
        }

        /// <summary>
        ///     When the language gets changed load the resource strings of that 
        ///     new selected language.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string language = (string)this.LanguageComboBox.SelectedItem;
            if (!String.IsNullOrEmpty(language))
            {
                this.LoadResources(language);
            }
        }

    }

    public class ResourceException : Exception
    {
        public ResourceException(string message, Exception inner = null) : base(message, inner) { }
    }
}
