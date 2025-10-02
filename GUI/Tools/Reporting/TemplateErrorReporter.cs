using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Reporting.ReportTemplate;
using InputLog.Core.Reporting.ReportTemplate.Import;

namespace GUI.Tools.Reporting
{
    /// <summary>
    ///     Shows errors in templates.
    /// </summary>
    public partial class TemplateErrorReporter : Form
    {
        /// <summary>
        ///     Reference to the ReportTemplateManager instance.
        /// </summary>
        private ReportTemplateManager _manager;

        /// <summary>
        ///     True if an instance of the error reporter has already been opened.
        /// </summary>
        public static bool IsOpened;

        /// <summary>
        ///     Create a new template error reporter dialog. When a parameter is
        ///     specified it should contain the path to a specific template. That template
        ///     will be pre-selected in the Dialog.
        /// </summary>
        /// <param name="selectedTemplate">Path to the template you would like to have selected.</param>
        public TemplateErrorReporter(string selectedTemplate = null)
        {
            InitializeComponent();
            _manager = ReportTemplateManager.TemplateInstance;
            _createTemplateList();
            _initDataGrid();

            // Add event listeners
            TemplateSelector.SelectedIndexChanged += TemplateSelector_SelectedIndexChanged;
            FormClosed += TemplateErrorReporter_FormClosed;
            Load += TemplateErrorReporter_Load;

            // Select default
            if (!string.IsNullOrEmpty(selectedTemplate))
            {
                _selectTemplateByPath(selectedTemplate);
            }
            else
            {
                if (_manager.FoundFaultyTemplates) _selectTemplateByPath(_manager.FirstTemplateWithError);
            }
        }

        private void TemplateErrorReporter_Load(object sender, EventArgs e)
        {
            IsOpened = true;
        }

        private void TemplateErrorReporter_FormClosed(object sender, FormClosedEventArgs e)
        {
            IsOpened = false;
        }

        /// <summary>
        ///     Initialize the data grid.
        /// </summary>
        private void _initDataGrid()
        {
            Data.AutoGenerateColumns = false;
            Data.ReadOnly = true;

            DataGridViewColumn sevColumn = new DataGridViewTextBoxColumn();
            sevColumn.Name = "Severity";
            sevColumn.DataPropertyName = "Severity";
            sevColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Data.Columns.Add(sevColumn);

            DataGridViewColumn nodeColumn = new DataGridViewTextBoxColumn();
            nodeColumn.Name = "Node";
            nodeColumn.DataPropertyName = "Node";
            nodeColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Data.Columns.Add(nodeColumn);

            DataGridViewColumn messageColumn = new DataGridViewTextBoxColumn();
            messageColumn.Name = "Message";
            messageColumn.DataPropertyName = "Message";
            messageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Data.Columns.Add(messageColumn);
        }

        /// <summary>
        ///     Populate the data grid with the errors for a
        ///     given template.
        /// </summary>
        private void _populateDataGrid(IEnumerable<TemplateImporter.Error> errors)
        {
            var source = new BindingSource();
            if (errors != null && errors.Count() > 0)
            {
                foreach (var error in errors) source.Add(error);
            }
            else
            {
                var dummy = new TemplateImporter.Error(
                    "",
                    "No errors in file.",
                    TemplateImporter.Severity.NONE
                );
                source.Add(dummy);
            }

            Data.DataSource = source;
        }

        /// <summary>
        ///     If the user selects a different template, we show that templates
        ///     error messages.
        /// </summary>
        private void TemplateSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = TemplateSelector.SelectedItem;
            if (item != null)
            {
                var template = (TemplateItem) item;
                _populateDataGrid(_manager.Errors[template.Path]);
            }
            else
            {
                Data.DataSource = null;
            }
        }

        /// <summary>
        ///     Set the template with specified path as selected, if the
        ///     path exists among the templates. If it does not exist, this
        ///     function does nothing.
        /// </summary>
        /// <param name="path">Path to template that should be selected in the list.</param>
        private void _selectTemplateByPath(string path)
        {
            foreach (var item in TemplateSelector.Items)
            {
                var template = (TemplateItem) item;
                if (template.Path == path) TemplateSelector.SelectedItem = item;
            }
        }

        /// <summary>
        ///     Adds all the opened templates to the template selection list.
        /// </summary>
        private void _createTemplateList()
        {
            TemplateSelector.Items.Clear();
            foreach (var entry in _manager.OpenedTemplates)
            {
                var item = new TemplateItem(entry.Key, entry.Value);
                TemplateSelector.Items.Add(item);
            }
        }


        /// <summary>
        ///     Refresh the list of templates, and the errors encountered in them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            var selectedItemIndex = TemplateSelector.SelectedIndex;
            _manager.Refresh();
            _createTemplateList();

            // We reselect the item (first setting it to null) so that we 
            // update the errors that are shown.
            TemplateSelector.SelectedIndex = selectedItemIndex;
            if (selectedItemIndex != -1)
            {
                var item = (TemplateItem) TemplateSelector.Items[selectedItemIndex];
                _populateDataGrid(_manager.Errors[item.Path]);
            }
        }


        /// <summary>
        ///     A class that display a template item in a combobox.
        /// </summary>
        public class TemplateItem
        {
            public string Path { get; }

            public string Name
            {
                get
                {
                    var fInfo = new FileInfo(Path);
                    return fInfo.Name;
                }
            }

            public string ID { get; private set; }

            public TemplateItem(string path, string id)
            {
                Path = path;
                ID = id;
            }

            public override string ToString()
            {
                return "(File name: \"" + Name + "\"; ID: \"" + ID + "\") - " + Path;
            }
        }
    }
}
