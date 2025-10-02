using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GUI.Properties;

namespace GUI.Tabs.Preprocess.Merge
{
	/// <summary>
	/// A wrapper for controls that should be displayed full width in a flow control pannel.
	/// </summary>
	public partial class FlowControlWrapper : AFileSelectionControl
	{
		#region public_fields
		/// <summary>
		/// The control that gets displayed by this wrapper.
		/// </summary>
		public Control ContentControl { get; private set; }

		/// <summary>
		/// Returns whether or not this control has been checked or not.
		/// </summary>
		public bool Selected
		{
			get { return Checkbox.Checked; }
		}

		/// <summary>
		/// Number of items that are presented in this flow control wrapper's 
		/// inner control.
		/// </summary>
		public override int ItemCount
		{
			get
			{
				return SelectionControl.ItemCount;
			}
		}

		/// <summary>
		/// Number of items that has been selected in thsi flow control wrapper's 
		/// inner control.
		/// </summary>
		public override int SelectedItemsCount
		{
			get
			{
				return SelectionControl.SelectedItemsCount;
			}
		}
		#endregion

		#region private_fields
		/// <summary>
		/// The parent control that hosts this FlowControlWrapper instance.
		/// </summary>
		private Control ParentControl;

		/// <summary>
		/// Called whenever the flow control wrapper is closed.
		/// </summary>
		private readonly EventHandler CloseHandle;
		#endregion

		#region protected_fields
		protected AFileSelectionControl SelectionControl
		{
			get
			{
				return (AFileSelectionControl)ContentControl;
			}
		}
		#endregion


		public FlowControlWrapper(Control parent, AFileSelectionControl content, EventHandler closeHandle)
		{
			InitializeComponent();

			ParentControl = parent;
			ContentControl = content;
			content.Parent = this;

			// Set display settings etc for the content
			content.Dock = DockStyle.Bottom;
			content.Visible = true;
			//content.BackColor = SystemColors.ControlDark;
			Layout.Controls.Add(content,0,1);

			Checkbox.Text = content.Name;
			
			CloseHandle += closeHandle;
			ParentControl.Layout += OnLayout;
			content.SelectionChanged += HandleSelectionChanged;
			AdjustSize();
		}

		/// <summary>
		/// Callback when the layout of the parent control is changed.
		/// We will need to adjust our width to the new layout.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The event arguments.</param>
		private void OnLayout(object sender, LayoutEventArgs e)
		{
			AdjustSize();
		}

		/// <summary>
		/// Adjusts the size of the wrapper to the size of the parent control.
		/// </summary>
		private void AdjustSize()
		{
			// Make this container as large as the parent container
			Width = ParentControl.ClientSize.Width - Margin.Left - Margin.Right;
			Height = WrapperLayout.GetRowHeights()[0] + ContentControl.Height;
			MaximumSize = new Size(Width, ParentControl.ClientSize.Height);
		}

		//
		// Event handling
		//
		#region Eventhandling
		/// <summary>
		/// Click handler for the close picture.
		/// Calls the close handle.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event arguments.</param>
		private void PictClose_Click(object sender, EventArgs e)
		{
			CloseHandle(this, new EventArgs());
		}
		
		/// <summary>
		/// Mouse leave handler for the close picture.
		/// (changes the picture).
		/// </summary>a
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event arguments.</param>
		private void PictClose_MouseLeave(object sender, EventArgs e)
		{
			ClosePicture.Image = Resources.cross_grey;
		}

		/// <summary>
		/// Mouse enter handler for the close picture.
		/// (changes the picture).
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Event arguments.</param>
		private void PictClose_MouseEnter(object sender, EventArgs e)
		{
			ClosePicture.Image = Resources.cross;
		}
		#endregion

		#region selection_changing
		/// <summary>
		/// Event handler for when the checkbox check status is changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Checkbox_CheckedChanged(object sender, EventArgs e)
		{
			// Inner control should change _all his file selections_ to the current
			// checkbox selection status.
			SelectionControl.ChangeSelection(this, Checkbox.Checked);
		}

		/// <summary>
		/// Change the selection of some files in this control. If no list of files is specified,
		/// it involves all files.
		/// </summary>
		/// <param name="sender">Sender of the selection change. Use this to make sure you do not loop selections and events.</param>
		/// <param name="selected">Whether the files are selected (=true) or deselected (=false)</param>
		/// <param name="files">The list of files that will have their selection changed. Or null if it involves all files.</param>
		public override void ChangeSelection(object sender, bool selected, List<string> files = null)
		{
			if (sender != this)
			{
				//CheckBox_ChangeChecked_TS(selected, true);
				SelectionControl.ChangeSelection(sender, selected, files);
				OnSelectionChanged(sender, selected, files);
			}
		}

		/// <summary>
		/// Method that gets called when there are some files that have issues. These fils
		/// will be unselected. Optionally the control may display some sort of error 
		/// marking the files as problem files.
		/// </summary>
		/// <param name="files">List of files that has problems and should be unselected</param>
		public override void MarkProblemFiles(List<string> files)
		{
			SelectionControl.MarkProblemFiles(files);
		}

		/// <summary>
		/// Handle a selectionChanged event from the SelectionControl. 
		/// </summary>
		/// <param name="sender">Sender of the event, if we were the original sender of the changes, ignore it.</param>
		/// <param name="selected">True if selected, false if not.</param>
		/// <param name="files">List of files to be selected/deselected</param>
		private void HandleSelectionChanged(object sender, bool selected, List<string> files)
		{
			// This delays the handling of the event as soon as all events are dispatched,
			// side effects are complete and the UI Thread goes idle again.
			//
			// This makes sure that the CheckedListBoxes of the inner FileExtSelectionControls have 
			// actually updated their CheckedItems before this method begins. 
			// CheckedListBox does not have a CheckedItemsChanged event, only an ItemCheck event 
			// which is the event which is called BEFORE the checkedState is actually changed.
			this.BeginInvoke((MethodInvoker)delegate
			{
				if (sender != this)
				{
					if ((SelectionControl.SelectedItemsCount > 0) != Selected)
					{
						CheckBox_ChangeChecked_TS(selected, false);
					}

					OnSelectionChanged(sender, selected, files);
				}
			});
		}
		#endregion

		#region thread_safe_control_updating
		/// <summary>
		/// Change the checked status of our checkbox in a threadsafe way.
		/// </summary>
		/// <param name="check">True if the checkbox should be checked, false if not.</param>
		/// <param name="ripple">Ripple the event to through to all listeners on the checkbox status if true, false if not.</param>
		private delegate void Checkbox_ChangeCheck(bool check, bool ripple);
		private void CheckBox_ChangeChecked_TS(bool check, bool ripple)
		{
			if (Checkbox.InvokeRequired)
			{
				Checkbox_ChangeCheck checkDelegate = CheckBox_ChangeChecked_TS;
				Invoke(checkDelegate, new object[] { check, ripple });
			}
			else
			{
				if (!ripple)
				{
					Checkbox.CheckedChanged -= Checkbox_CheckedChanged;
				}


				// If we want to ripple the fact that the Checkbox.Checked status is the value of 'check'
				// to all listeners, but the value is not actually changing, we have to toggle the value
				// beforeHand without triggering the event.
				if (ripple && Checkbox.Checked == check)
				{
					Checkbox.CheckedChanged -= Checkbox_CheckedChanged;
					Checkbox.Checked = !check;
					Checkbox.CheckedChanged += Checkbox_CheckedChanged;
				}
				Checkbox.Checked = check;
				if (!ripple)
				{
					Checkbox.CheckedChanged += Checkbox_CheckedChanged;
				}
			}
		}
		#endregion
	}
}
