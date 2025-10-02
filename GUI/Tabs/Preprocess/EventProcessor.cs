using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InputLog.Core.Util;

namespace GUI.Tabs.Preprocess
{
    public partial class EventProcessor : UserControl
    {
        #region events_delegates

        /// <summary>
        /// Callback delegate for when the selection of files has been changed.
        /// This has to be pushed through to the Processors.
        /// </summary>
        /// <param name="filePaths">List of paths to the selected files.</param>
        public delegate void OnFileSelectionChange(List<string> filePaths);

        /// <summary>
        /// Event gets triggered when the file selection has been changed.
        /// </summary>
        public event OnFileSelectionChange FileSelectionChanged;

        #endregion

        #region fields

        /// <summary>
        /// Bool keeps track of whether the control has been enabled at some 
        /// point in time.
        /// </summary>
        private bool HasActivated;

        /// <summary>
        /// Background color that should be given to a selected manipulator control.
        /// </summary>
        private readonly Color SelectedColor = Color.AntiqueWhite;

        /// <summary>
        /// Backcolor of a manipulator control before it was selected. 
        /// </summary>
        private Color OldControlBackColor;

        /// <summary>
        /// Backcolor of a manipulator wrapper before it was selected. 
        /// </summary>
        private Color OldWrapperBackColor;

        /// <summary>
        /// The wrapper of the manipulator that is currently selected.
        /// </summary>
        private FilterWrapper ThisSelectedManipulator;

        /// <summary>
        /// The wrapper of the manipulator that is currently selected (or null if no manipulator is selected).
        /// Enable/disables the up and down buttons automatically according to the position of the selected manipulator 
        /// in the list.
        /// </summary>
        private FilterWrapper SelectedManipulator
        {
            get { return ThisSelectedManipulator; }
            set
            {
                ThisSelectedManipulator = value;
                UpdateUpDownButtons();
            }
        }

        /// <summary>
        /// The list of files currently selected to be processed by this
        /// event preprocessor.
        /// </summary>
        private List<string> FilePaths;

        /// <summary>
        /// The list of manipulator controls that has effectively have been configured and accepted.
        /// </summary>
        private readonly List<ProcessControl> AcceptedManipulatorControls;

        /// <summary>
        /// Suppported Manipultors. 
        /// If a new manipulator is added, this is the only place in this class where an update is needed!
        /// </summary>
        private IDictionary<string, Type> AvailableManipulators;

        #endregion

        public EventProcessor()
        {
            InitializeComponent();
            FilePaths = new List<string>();
            AcceptedManipulatorControls = new List<ProcessControl>();
            SelectedManipulator = null;           
        }

        /// <summary>
        /// Returns whether the event processor has processors added to its
        /// panel or not. This will only return true if the pannel has been
        /// activated at some point in time.
        /// </summary>
        public bool HasContent
        {
            get { return Panel.Controls.Count != 0 && HasActivated; }
        }

        /// <summary>
        /// Returns a list of all the active processors. Note that if any processors
        /// have been disabled due to an incorrect amount of files being specified, these
        /// processors will not be included in the returned list.
        /// </summary>
        /// <returns>The list of all the currently active processors.</returns>
        public List<ProcessControl> GetActiveProcessers()
        {           
            return AcceptedManipulatorControls.FindAll(control => control.Enabled);
        }

        /// <summary>
        /// Set the available manipulator controls in the event processors dropdown list.
        /// </summary>
        /// <param name="availableManipulators">A dictionary of manipulator controls available in this event processor.</param>
        public void SetAvailableManipulators(Dictionary<string, Type> availableManipulators)
        {
            DropDown.Items.Clear();
            AvailableManipulators = availableManipulators;
            DropDown.Items.AddRange(AvailableManipulators.Keys.ToArray());
            DropDown.SelectedIndex = 0;
        }

        /// <summary>
        /// Close handler for the inserted ManipulatorWrappers.
        /// </summary>
        /// <param name="sender">The closed Manipulator.</param>
        /// <param name="eventArgs">The event arguments (empty).</param>
        private void RemoveManipulatorWrapper(object sender, EventArgs eventArgs)
        {
            var wrapper = (Control)sender;
            try
            {
                Panel.Controls.Remove(wrapper);
                Panel.PerformLayout();

                if (sender == SelectedManipulator) SelectedManipulator = null;
                else UpdateUpDownButtons();
            }
            finally
            {
                wrapper.Dispose();
                UpdateActiveManipulators();
            }
        }

        /// <summary>
        /// Click event for the AddFilterButton button.
        /// Adds an manipulatorControl to the SelectedManipulatorsPanel.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void AddButtonClick(object sender, EventArgs e)
        {
            var manipulatorControl = new ProcessControl();
            try
            {
                AddButton.Enabled = false;   
                string manipulatorName = DropDown.SelectedItem.ToString();
                var selectedProcessor = AvailableManipulators[manipulatorName];

                manipulatorControl = (ProcessControl)Activator.CreateInstance(selectedProcessor);
                manipulatorControl.Click += ManipulatorSelectionListener;

                FileSelectionChanged += manipulatorControl.OnFileSelectionChange;
                manipulatorControl.OnFileSelectionChange(FilePaths);
                
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
            finally
            {
                if (ValidatePaths(manipulatorControl))
                {
                    var wrapper = new FilterWrapper(Panel, manipulatorControl, RemoveManipulatorWrapper);
                    manipulatorControl.FilePaths = FilePaths;

                    wrapper.SuspendLayout();
                    Panel.Controls.Add(wrapper);
                    ManipulatorSelectionListener(manipulatorControl, null);
                    Panel.ScrollControlIntoView(wrapper);
                    wrapper.ResumeLayout();
                    UpdateActiveManipulators();
                }
                else
                {
                    manipulatorControl.Click -= ManipulatorSelectionListener;
                    FileSelectionChanged -= manipulatorControl.OnFileSelectionChange;
                }
            }
        }

        /// <summary>
        /// Checks whether the amount of currently selected files
        /// is compatible with all the selected filters or not. 
        /// If it isn't this method will show a popup box that describes the 
        /// exact nature of the filecount incompatibility.
        /// </summary>
        /// <param name="newProcessor">New processor being added, or null if no processor
        /// is being added.</param>
        /// <returns>True if the filecount is compatible, false if it is not.</returns>
        public bool ValidatePaths(ProcessControl newProcessor = null)
        {
            bool compatible = true;
            string rationale = "";

            if (FilePaths.Any())
            {
                foreach (var acceptedControl in AcceptedManipulatorControls)
                {
                    if (FilePaths.Count > 1 && !acceptedControl.MultipleFileCompatible)
                    {
                        compatible = false;
                        rationale += "[" + acceptedControl.Name + "]: Can only process one file at a time. \n";
                        acceptedControl.Enabled = false;
                    }
                    else
                    {
                        acceptedControl.Enabled = true;
                    }
                }

                if (newProcessor != null && FilePaths.Count > 1 && !newProcessor.MultipleFileCompatible)
                {
                    compatible = false;
                    rationale += "[" + newProcessor.Name + "]: Can only process one file at a time. \n";
                    newProcessor.Enabled = false;
                }
                else if (newProcessor != null)
                {
                    newProcessor.Enabled = true;
                }
            }
            else
            {
                compatible = false;
                rationale = "You must specify at least 1 file.";

                if (newProcessor != null) newProcessor.Enabled = false;
                foreach (var acceptedControl in AcceptedManipulatorControls)
                    acceptedControl.Enabled = false;
            }

            if (!compatible) MessageBox.Show(this, rationale, "Error:", MessageBoxButtons.OK);

            return compatible;
        }

        /// <summary>
        /// Event listener for the selection of a manipulator control.
        /// Sets the active selected manipulator control and colors
        /// the selected control to indicate its selected state.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ManipulatorSelectionListener(object sender, EventArgs e)
        {
            if (SelectedManipulator != null)
            {
                SelectedManipulator.BackColor = OldWrapperBackColor;
                SelectedManipulator.Control.BackColor = OldControlBackColor;
            }

            var control = (ProcessControl)sender;
            SelectedManipulator = (FilterWrapper)control.Parent;
            OldWrapperBackColor = SelectedManipulator.BackColor;
            OldControlBackColor = SelectedManipulator.Control.BackColor;

            SelectedManipulator.BackColor = SelectedColor;
            SelectedManipulator.Control.BackColor = SelectedColor;
        }

        /// <summary>
        /// Update the list with active manipulator controls to reflect all the different
        /// processors that have currently been added to the flowPannel.
        /// </summary>
        private void UpdateActiveManipulators()
        {
            AcceptedManipulatorControls.Clear();
            foreach (var wrapper in Panel.Controls.OfType<FilterWrapper>())
                AcceptedManipulatorControls.Add(wrapper.Control);
            AddButton.Enabled = true;
        }

        /// <summary>
        /// Click event for the UpButton.
        /// Moves the selected manipulator control up (if any).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void UpButtonClick(object sender, EventArgs e)
        {
            if (SelectedManipulator == null) return;
            int manipulatorIndex = Panel.Controls.GetChildIndex(SelectedManipulator);
            if (manipulatorIndex == 0) return;

            Panel.Controls.SetChildIndex(SelectedManipulator, manipulatorIndex - 1);
            UpdateUpDownButtons();
        }

        /// <summary>
        /// Click event for the DownButton.
        /// Moves the selected manipulator control down (if any).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DownButtonClick(object sender, EventArgs e)
        {
            if (SelectedManipulator == null) return;
            int manipulatorIndex = Panel.Controls.GetChildIndex(SelectedManipulator);
            if (manipulatorIndex >= Panel.Controls.Count - 1) return;

            Panel.Controls.SetChildIndex(SelectedManipulator, manipulatorIndex + 1);
            UpdateUpDownButtons();
        }

        /// <summary>
        /// Enables or disables the up and down buttons according to
        /// the position of the SelectedManipulator in the list
        /// of manipulators.
        /// </summary>
        private void UpdateUpDownButtons()
        {
            if (SelectedManipulator == null || Panel.Controls.Count <= 1)
            {
                UpButton.Enabled = false;
                DownButton.Enabled = false;
                return;
            }

            if (Panel.Controls.Count == 0) return;
            int manipulatorIndex = Panel.Controls.GetChildIndex(SelectedManipulator);

            UpButton.Enabled = (manipulatorIndex > 0);
            DownButton.Enabled = (manipulatorIndex < Panel.Controls.Count - 1);
        }

        /// <summary>
        /// Event callback for when the enabled stated of this control changes.
        /// Used to keep track of whether the control has been activated
        /// at some point in time or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventProcessorEnabledChanged(object sender, EventArgs e)
        {
            if (Enabled) HasActivated = true;
        }

        /// <summary>
        /// Returns whether the currently specified number of files, n, for
        /// the preprocessing of this EventProcessor is compatible with the 
        /// currently selected preprocessors.
        /// </summary>
        /// <param name="n">The number of files selected.</param>
        /// <returns>True if the number of files is compatible, false if it is not.</returns>
        public bool IsCompatibleNumberOfFiles(int n)
        {
            if (n <= 0) return false;

            return AcceptedManipulatorControls.All(control => n <= 1 || control.MultipleFileCompatible);
        }

        /// <summary>
        /// Callback function for when the selected files for this
        /// preprocessor have been updated
        /// </summary>
        /// <param name="filePaths">The filepaths to all selected files.</param>
        public void UpdateSelectedFiles(IList<string> filePaths)
        {
            FilePaths = new List<string>(filePaths);
            if (FileSelectionChanged != null)
                FileSelectionChanged(FilePaths);
        }
    }
}
