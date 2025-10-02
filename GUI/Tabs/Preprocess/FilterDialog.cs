using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GUI.Tabs.Preprocess.Filters;
using GUI.Tabs.Preprocess.Recoders;
using InputLog.Core.Util;

namespace GUI.Tabs.Preprocess
{
    /// <summary>
    /// Dialog that lets the user specify manipulators for events. 
    /// This dialog does not permanently store the specified manipulators;
    /// this is done by the Analyze tab itself. When the user clicks the "configure manipulators" link, this dialog is shown
    /// and the Analyze tab passes previously specified manipulators to this dialog.
    /// </summary>
    public partial class PostProcessDialog : Form
    {
        /// <summary>
        /// The list of manipulator controls that has effectively have been configured and accepted.
        /// </summary>
        private readonly List<ProcessControl> AcceptedManipulatorControls;

        /// <summary>
        /// Suppported Manipultors. 
        /// If a new manipulator is added, this is the only place in this class where an update is needed!
        /// </summary>
        public readonly IDictionary<string, Type> AvailableManipulators = new Dictionary<string, Type>
        {
          {EventType.NAME, typeof (EventType)},
          {Time.NAME, typeof (Time)},
          {Window.NAME, typeof (Window)},
		  {FocusRewrite.NAME, typeof(FocusRewrite)},
          {IdfxRecoder.NAME, typeof(IdfxRecoder)},
          {RemoveTaskbar.NAME, typeof(RemoveTaskbar)},
         };

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
        /// Constructs the Manipulator dialog.
        /// </summary>
        public PostProcessDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructs the manipulator dialog using an existing set of controls.
        /// </summary>
        /// <param name="manipulatorControls">The list of manipulator controls that need to be added to the dialog.</param>
        public PostProcessDialog(List<ProcessControl> manipulatorControls)
        {
            InitializeComponent();
            ManipulatorList.Items.AddRange(AvailableManipulators.Keys.ToArray());
            ManipulatorList.SelectedIndex = 0;
            AcceptedManipulatorControls = manipulatorControls;
            foreach (ProcessControl manipulatorControl in manipulatorControls)
            {
                SelectedManipulatorsPanel.Controls.Add(new FilterWrapper(SelectedManipulatorsPanel, manipulatorControl,
                                                                         RemoveManipulatorWrapper));
                // re-add selection listener (it gets lost while passing the control between the caller of the dialog 
                // and the dialog itself as the dialog is destroyed).
                manipulatorControl.Click += ManipulatorSelectionListener;
            }
            SelectedManipulator = null;
        }

        /// <summary>
        /// The wrapper of the manipulator that is currently selected (or null if no manipulator is selected).
        /// Enable/disables the up and down buttons automatically according to the position of the selected manipulator 
        /// in the list.
        /// </summary>
        private FilterWrapper SelectedManipulator
        {
            get { return selectedManipulator; }
            set
            {
                selectedManipulator = value;
                UpdateUpDownButtons();
            }
        }

        /// <summary>
        /// Returns a list of the manipulator controls that are specified by this manipulator dialog.
        /// </summary>
        /// <returns>a list of manipulator controls that are specified by this manipulator dialog.</returns>
        public List<ProcessControl> GetManipulatorControls()
        {
            return AcceptedManipulatorControls;
        }

        /// <summary>
        /// Close handler for the inserted ManipulatorWrappers.
        /// </summary>
        /// <param name="sender">The closed Manipulator.</param>
        /// <param name="eventArgs">The event arguments (empty).</param>
        public void RemoveManipulatorWrapper(object sender, EventArgs eventArgs)
        {
            var wrapper = (Control) sender;
            try
            {
                // remove control from panel
                SelectedManipulatorsPanel.Controls.Remove(wrapper);
                // Force perform layout so possible hiding of scroll bar is taken into account when setting the width
                SelectedManipulatorsPanel.PerformLayout();

                // fix selected manipulator and up and down buttons (this must be done after the control is deleted as
                // the position of the deleted control effects the enablement of the up and down buttons).
                if (sender == SelectedManipulator)
                {
                    SelectedManipulator = null;
                }
                else
                {
                    UpdateUpDownButtons();
                }
            }
            finally
            {
                wrapper.Dispose();
            }
        }

        /// <summary>
        /// Click event for the AddFilterButton button.
        /// Adds an manipulatorControl to the SelectedManipulatorsPanel.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void AddManipulatorButtonClick(object sender, EventArgs e)
        {
            var manipulatorControl = new ProcessControl();
            try
            {
                string manipulatorName = ManipulatorList.SelectedItem.ToString();
                Type selectedAnalysis = AvailableManipulators[manipulatorName];
                // note: there should always be such a value in the dictionary, 
                // as the only way to get a value in the list is by adding it to the dictionary

                // create the control
                manipulatorControl = (ProcessControl) Activator.CreateInstance(selectedAnalysis);
                manipulatorControl.Click += ManipulatorSelectionListener;
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
            finally
            {
                var wrapper = new FilterWrapper(SelectedManipulatorsPanel, manipulatorControl, RemoveManipulatorWrapper);

                // Suspend layout event so scroll bar is drawn before the wrapper gets the layout events and 
                // tries to adjust its size without taking the scroll bar into account.
                wrapper.SuspendLayout();
                // add control
                SelectedManipulatorsPanel.Controls.Add(wrapper);

                // trigger a select event for the newly added control
                ManipulatorSelectionListener(manipulatorControl, null);
                SelectedManipulatorsPanel.ScrollControlIntoView(wrapper);
                wrapper.ResumeLayout();
            }
        }

        /// <summary>
        /// Event listener for the selection of a manipulator control.
        /// Sets the active selected manipulator control and colors the selected control to indicate its selected state.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        public void ManipulatorSelectionListener(object sender, EventArgs e)
        {
            // unselect previous selected control by changing the colors
            if (SelectedManipulator != null)
            {
                SelectedManipulator.BackColor = OldWrapperBackColor;
                SelectedManipulator.Control.BackColor = OldControlBackColor;
            }

            // select new control
            var control = (ProcessControl) sender;
            SelectedManipulator = (FilterWrapper) control.Parent;
            OldWrapperBackColor = SelectedManipulator.BackColor;
            OldControlBackColor = SelectedManipulator.Control.BackColor;

            SelectedManipulator.BackColor = SelectedColor;
            SelectedManipulator.Control.BackColor = SelectedColor;
        }


        /// <summary>
        /// Click event for the OKButton button.
        /// Closes the window.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OKButtonClick(object sender, EventArgs e)
        {
            if (SelectedManipulator != null)
            {
                // set backcolor of currently selected control back to normal
                SelectedManipulator.Control.BackColor = OldControlBackColor;
            }
            AcceptedManipulatorControls.Clear();
            foreach (FilterWrapper wrapper in SelectedManipulatorsPanel.Controls.OfType<FilterWrapper>())
            {
                AcceptedManipulatorControls.Add(wrapper.Control);
                // Remove click handler. Normally this is done automatically if this dialog is disposed.
                // However, if a fellow developer (that might be you ;-) ) does not dispose the dialog and reuses the
                // manipulators control, we might have a problem (so deleted them here to be sure => safety).
                wrapper.Control.Click -= ManipulatorSelectionListener;
            }
            SelectedManipulatorsPanel.Controls.Clear();
            Close();
        }

        /// <summary>
        /// Click event for the OKButton button.
        /// Cancels all configured event manipulators and closes the window.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void CancelButtonClick(object sender, EventArgs e)
        {
            if (SelectedManipulator != null)
            {
                // set backcolor of currently selected control back to normal
                SelectedManipulator.Control.BackColor = OldControlBackColor;
            }
            // remove event listeners for safety
            foreach (FilterWrapper wrapper in SelectedManipulatorsPanel.Controls.OfType<FilterWrapper>())
            {
                // Remove click handler. Normally this is done automatically if this dialog is disposed.
                // However, if a fellow developer (that might be you ;-) ) does not dispose the dialog and reuses the
                // manipulators control, we might have a problem (so deleted them here to be sure => safety).
                wrapper.Control.Click -= ManipulatorSelectionListener;
            }
            SelectedManipulatorsPanel.Controls.Clear();
            Close();
        }

        /// <summary>
        /// Click event for the UpButton.
        /// Moves the selected manipulator control up (if any).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void UpButtonClick(object sender, EventArgs e)
        {
            if (SelectedManipulator != null)
            {
                int manipulatorIndex = SelectedManipulatorsPanel.Controls.GetChildIndex(SelectedManipulator);
                if (manipulatorIndex > 0)
                {
                    SelectedManipulatorsPanel.Controls.SetChildIndex(SelectedManipulator, manipulatorIndex - 1);
                    UpdateUpDownButtons();
                }
            }
        }

        /// <summary>
        /// Click event for the DownButton.
        /// Moves the selected manipulator control down (if any).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DownButtonClick(object sender, EventArgs e)
        {
            if (SelectedManipulator != null)
            {
                int manipulatorIndex = SelectedManipulatorsPanel.Controls.GetChildIndex(SelectedManipulator);
                if (manipulatorIndex < SelectedManipulatorsPanel.Controls.Count - 1)
                {
                    SelectedManipulatorsPanel.Controls.SetChildIndex(SelectedManipulator, manipulatorIndex + 1);
                    UpdateUpDownButtons();
                }
            }
        }

        /// <summary>
        /// Enables or disables the up and down buttons according to the position of the SelectedManipulator in the list
        /// of manipulators.
        /// </summary>
        private void UpdateUpDownButtons()
        {
            if (SelectedManipulator == null || SelectedManipulatorsPanel.Controls.Count <= 1)
            {
                UpButton.Enabled = false;
                DownButton.Enabled = false;
                return;
            }
            if (SelectedManipulatorsPanel.Controls.Count > 0)
            {
                int manipulatorIndex = SelectedManipulatorsPanel.Controls.GetChildIndex(SelectedManipulator);
                UpButton.Enabled = manipulatorIndex > 0;
                DownButton.Enabled = manipulatorIndex < (SelectedManipulatorsPanel.Controls.Count - 1);
            }
        }
    }
}