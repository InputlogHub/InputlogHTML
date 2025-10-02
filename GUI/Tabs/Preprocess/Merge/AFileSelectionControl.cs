using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace GUI.Tabs.Preprocess.Merge
{
	/// <summary>
	/// Delegate function for when the file selection has changed.
	/// </summary>
	/// <param name="sender">Sender of the event.</param>
	/// <param name="selected">True if files have been selected, false if they have been deselected.</param>
	/// <param name="files">The list of all files that have been changed.</param>
	public delegate void SelectionChangedHandler(object sender, bool selected, List<string> files);

	/// <summary>
	/// Class for a control that can select/deselect files.
	/// This class also provides error handling functionality through the Marking of problem files.
	/// </summary>
	[TypeDescriptionProvider(typeof(GUI.ReplaceTypeDescriptionProvider<AFileSelectionControl, MiddleClass>))]
	public abstract class AFileSelectionControl: UserControl
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public AFileSelectionControl() { }

		/// <summary>
		/// Change the selection of some files in this control. If no list of files is specified,
		/// it involves all files.
		/// </summary>
		/// <param name="sender">Sender of the selection change. Use this to make sure you do not loop selections and events.</param>
		/// <param name="selected">Whether the files are selected (=true) or deselected (=false)</param>
		/// <param name="files">The list of files that will have their selection changed. Or null if it involves all files.</param>
		public abstract void ChangeSelection(object sender, bool selected, List<string> files = null);

		/// <summary>
		/// Event that gets called when the controls selection has been changed.
		/// </summary>
		public event SelectionChangedHandler SelectionChanged;

		/// <summary>
		/// Method that gets called when there are some files that have issues. These fils
		/// will be unselected. Optionally the control may display some sort of error 
		/// marking the files as problem files.
		/// </summary>
		/// <param name="files">List of files that has problems and should be unselected</param>
		public abstract void MarkProblemFiles(List<string> files);

		/// <summary>
		/// Call this method to fire the SelectionChanged event.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="selected">Whether to select or deselect the altered files.</param>
		/// <param name="files">List of files.</param>
		protected virtual void OnSelectionChanged(object sender, bool selected, List<string> files = null)
		{
			if (SelectionChanged != null)
			{
				SelectionChanged(sender, selected, files);
			}
		}

		/// <summary>
		/// The total number of items currently selected
		/// </summary>
		public abstract int SelectedItemsCount { get; }

		/// <summary>
		/// The total number of items, (selected + unselected)
		/// </summary>
		public abstract int ItemCount { get; }
	}

	/// <summary>
	/// Middle class that implements the abstract methods so that the designer can open 
	/// controls implementing the abstract class AFileSelectionControl
	/// </summary>
	public class MiddleClass: AFileSelectionControl 
	{
		public override void  ChangeSelection(object sender, bool selected, List<string> files = null){}
		public override void  MarkProblemFiles(List<string> files){}
		public override int ItemCount { get { return 0; } }
		public override int SelectedItemsCount { get { return 0; } }
	}
}
