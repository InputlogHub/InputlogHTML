using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using GUI.Util;
using InputLog.Core.Util;
using System.Text.RegularExpressions;

namespace GUI.Tabs.Preprocess.Merge
{
	/// <summary>
	/// Control that allows a user to select 'through a checkbox' some files.
	/// The files are grouped based on their extension.
	/// </summary>
	public partial class FileExtSelectionControl : AFileSelectionControl
	{
		#region private_fields
		/// <summary>
		/// The list of files displayed by this control, grouped per extension.
		/// </summary>
		private Dictionary<string, List<string>> FilesByExt;

		/// <summary>
		/// The path common to all files listed here.
		/// </summary>
		private string CommonPath; 

		//
		// Dimensions
		// 
		/// <summary>
		/// We have already calculated the dimenions or not.
		/// </summary>
		private bool DimensionsInitialized = false;

		/// <summary>
		/// The number of columns we should make for optimal spreading 
		/// of the file extension selection controls
		/// </summary>
		private int _columns;
		private int Columns
		{
			set { _columns = value; }
			get
			{
				CheckInitialized();
				return _columns;
			}
		}

		/// <summary>
		/// The number of rows we should make for optimal spreading 
		/// of the file extension selection controls
		/// </summary>
		private int _rows;
		private int Rows
		{
			set { _rows = value; }
			get
			{
				CheckInitialized();
				return _rows;
			}
		}

		/// <summary>
		/// The optimal height for a single file extension selection control
		/// </summary>
		private int _itemHeight;
		private int ItemHeight
		{
			set { _itemHeight = value; }
			get
			{
				CheckInitialized();
				return _itemHeight;
			}
		}

		/// <summary>
		/// The optimal width for a single file extension selection control
		/// </summary>
		private int _itemWidth;
		private int ItemWidth
		{
			set { _itemWidth = value; }
			get
			{
				CheckInitialized();
				return _itemWidth;
			}
		}

		/// <summary>
		/// The maximum number of items the listbox will scale too, in order to
		/// show all items without requiring a vertical scrollbar. If there's more 
		/// items in the listbox than MaxItemsWithoutScroll, the listbox will have
		/// a vertical scrollbar and only display 5 items at a time.
		/// </summary>
		private const int MaxItemsWithoutScroll = 5;

		/// <summary>
		/// Height in pixels from the titles per extension 'checkedlistbox'.
		/// </summary>
		private const int SubTitlePixelHeight = 23;

		/// <summary>
		/// horizontal margin between elements in the flowpanel.
		/// </summary>
		private const int COLMARGIN = 3;

		/// <summary>
		/// 2px margin beneath the listboxes.
		/// </summary>
		private const int BeneathListBoxMargin = 4;

        private Dictionary<string, bool> Selections;
		#endregion

		#region public_fields
		/// <summary>
		/// Returns the total ammount of files being displayed in 
		/// this control.
		/// </summary>
		public override int ItemCount
		{
			get
			{
				return GetListBoxes().Aggregate<CheckedListBox,int>(0, (count, listBox) => count += listBox.Items.Count);
			}
		}

		/// <summary>
		/// Returns the total amount of currently checked files, displayed in this control.
		/// </summary>
		public override int SelectedItemsCount
		{
			get 
			{
				return GetListBoxes().Aggregate<CheckedListBox, int>(0, (count, listBox) => count += listBox.CheckedItems.Count);
			}
		}

		#endregion

		/// <summary>
		/// Create a new FileExtSelectionControl. This control gives the user a selection 
		/// of files which he/she can select or deselect. The files are grouped together in 
		/// CheckedListBoxes by their extension.<br />
		/// </summary>
		/// <param name="files">Files to be displayed.d</param>
		public FileExtSelectionControl(IEnumerable<string> files, IEnumerable<string> selectedFiles)
		{
			InitializeComponent();
			FilesByExt = new Dictionary<string, List<string>>();
            Selections = new Dictionary<string, bool>();
			// Group files by extension.
			foreach (string file in files)
			{
                bool selected = selectedFiles.Contains(file);
                Selections.Add(file, selected);
				List<string> filesOfExt;
				string extension = Path.GetExtension(file);
				FilesByExt.TryGetValue(extension, out filesOfExt);

				if (filesOfExt == null)
				{
					FilesByExt.Add(extension, new List<string>(new string[] { file }));
				}
				else
				{
					filesOfExt.Add(file);
				}
			}

			// Find the common path in all the files.
			CommonPath = StringUtils.FindCommonPath(files);
			Name = CommonPath;
			//BackColor = SystemColors.ControlLight;
			CalculateDimensions();

			// Set control's height and width.
			Height = Rows * (ItemHeight + SubTitlePixelHeight);

			CreateFileSelectionControls();
		}

		/// <summary>
		/// Add the selection listboxes to the FlowPanel so that the files
		/// are grouped by extension.
		/// </summary>
        private void CreateFileSelectionControls()
		{
			// Create the listboxes and add the files - standard checked
			TableLayoutPanel[] panels = new TableLayoutPanel[FilesByExt.Keys.Count];
			int count = 0;
			foreach (string ext in FilesByExt.Keys)
			{
				// Create a TableLayoutPanel to put the listbox with its title in.
				TableLayoutPanel tableLayout = new TableLayoutPanel();
				tableLayout.RowCount = 2;
				tableLayout.ColumnCount = 1;
				tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, SubTitlePixelHeight));
				tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
				tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
				tableLayout.Margin = new System.Windows.Forms.Padding(COLMARGIN, 0, COLMARGIN, 0);
				tableLayout.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
				

				// Make the label.
				Label listName = new Label();
				listName.Text = ext + " files";
				listName.Font = new System.Drawing.Font(listName.Font, FontStyle.Bold);
				listName.TextAlign = ContentAlignment.MiddleCenter;
				listName.Anchor = AnchorStyles.Right | AnchorStyles.Left;

				// Create listbox with correct attributes & parameters
				ColoredCheckedListBox listBox = new ColoredCheckedListBox();
				listBox.HorizontalScrollbar = true;
				listBox.Dock = DockStyle.Fill;
				listBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, BeneathListBoxMargin);
				listBox.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
				listBox.ItemCheck += HandleCheckChange;
				listBox.SpecialColor = Color.Red;

				// Add the files to the listbox
                foreach (string file in FilesByExt[ext])
                {
                    string name = file.Substring(CommonPath.Length,file.Length-CommonPath.Length);
                    listBox.Items.Add(name, Selections[file]);
                }
				listBox.Name = ext;

				// Add controls to tableLayout
				tableLayout.Controls.Add(listName, 0, 0);
				tableLayout.Controls.Add(listBox, 0, 1);

				panels[count++] = tableLayout;
			}

			FlowPanel.Controls.AddRange(panels);
		}

		/// <summary>
		/// Calculate and set the size (width/heigth) of the FileExtSelectionControl
		/// depending on the contents in the FlowPanel.
		/// </summary>
		private void CalculateDimensions()
		{
			DimensionsInitialized = true;
			int mod2 = FilesByExt.Keys.Count % 2;
			int mod3 = FilesByExt.Keys.Count % 3;

			int nrOfColumns = 0;
			if (mod3 == 0)
			{
				nrOfColumns = 3;
			}
			else if (mod2 == 0)
			{
				nrOfColumns = 2;
			}
			else
			{
				nrOfColumns = (Math.Max(mod2, mod3) == mod3 && mod2 != mod3) ? 3 : 2;
			}

			Columns = nrOfColumns;
			Rows = FilesByExt.Keys.Count / nrOfColumns;

			// Calculate listbox max Width and max Height
			int colMargins = 2 * COLMARGIN + (Columns) * COLMARGIN;
			ItemWidth = (int)Math.Floor((double)((FlowPanel.Width - colMargins) / Columns)); // 3px padding somewhere?
			ListBox listBox = new ListBox();
			Graphics gr = listBox.CreateGraphics();
			float maxHeight = FilesByExt.Values.ToList().
				Max(fileList => fileList.Max(file => gr.MeasureString(file, listBox.Font).Height));
			ItemHeight = (int)(MaxItemsWithoutScroll * (maxHeight+5)) + BeneathListBoxMargin; // +5 pixels because the select box is bigger.
		}

		/// <summary>
		/// Relayout function that gets called when the parent gets lay-ed out.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnParentSizeChange(object sender, EventArgs args)
		{
			AdjustSize();
		}

		/// <summary>
		/// Register the layout function with the parent control.
		/// </summary>
		/// <param name="parent">The parent control</param>
		public void RegisterSizeChangeListener(Control parent)
		{
			if (parent != null)
			{
				parent.Paint += OnParentSizeChange;
				//parent.SizeChanged += OnParentSizeChange;
			}
		}

		/// <summary>
		/// Unregister the layout function with the parent control.
		/// </summary>
		/// <param name="parent">The parent control</param>
		public void UnregisterSizeChangeListener(Control parent)
		{
			if (parent != null)
			{
				parent.Paint -= OnParentSizeChange;
				//parent.SizeChanged -= OnParentSizeChange;
			}
		}

		/// <summary>
		/// Adjust the sizes of the elements in the flow panel depending on the 
		/// updated sizes of the parent container.
		/// </summary>
		private void AdjustSize()
		{
			CalculateDimensions();

			foreach (TableLayoutPanel tPanel in FlowPanel.Controls)
			{
				tPanel.Width = ItemWidth;
				tPanel.Height = ItemHeight + SubTitlePixelHeight;
			}

		}


		/// <summary>
		/// Throws an exception if intialization of dimensions has not yet 
		/// been executed.
		/// </summary>
		private void CheckInitialized()
		{
			if (!DimensionsInitialized)
			{
				throw new OperationCanceledException("Must intiliaze dimensions before this value can be requested");
			}
		}

		//
		// Change the selections
		//
		#region selection_changing
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
				if (files != null)
				{
					files.ForEach(file => file = RemoveCommonPath(file));
				}

				List<string> AlteredFiles = new List<string>();

				foreach(ColoredCheckedListBox listBox in GetListBoxes())
				{
					for (int i = 0; i < listBox.Items.Count; i++ )
					{
						if (files == null || files.Contains(listBox.Items[i]))
						{
							listBox.SetItemCheckState(i,((selected) ? CheckState.Checked : CheckState.Unchecked));
							AlteredFiles.Add((string)listBox.Items[i]);
						}
					}
				}

				OnSelectionChanged(sender, selected, AlteredFiles);
			}
		}

		/// <summary>
		/// Returns the list of all ColoredCheckListBoxes in the FlowPanel's controls.
		/// - Wades through the TableLayoutPanels first -
		/// </summary>
		/// <returns></returns>
		private List<ColoredCheckedListBox> GetListBoxes()
		{
			List<ColoredCheckedListBox> list = new List<ColoredCheckedListBox>(FlowPanel.Controls.Count);
			foreach (Control control in FlowPanel.Controls)
			{
				TableLayoutPanel layout = control as TableLayoutPanel;
				IEnumerable<ColoredCheckedListBox> boxes = layout.Controls.OfType<ColoredCheckedListBox>();
				if (boxes.Count() > 0)
				{
					list.AddRange(boxes);
				}
			}
			return list;
		}

		/// <summary>
		/// Method that gets called when there are some files that have issues. These fils
		/// will be unselected. Optionally the control may display some sort of error 
		/// marking the files as problem files.
		/// </summary>
		/// <param name="files">List of files that has problems and should be unselected</param>
		public override void MarkProblemFiles(List<string> files)
		{
			// First we create the list of our own files, since those are the ones the
			// common path is calculated on.
			// Then for the intersection of ourFiles with the list of files marked as
			// problematic we add them to the listBox specialItems list.
			//
			List<string> ourFiles = new List<string>();
			List<ColoredCheckedListBox> ourListBoxes = GetListBoxes();
			foreach (ColoredCheckedListBox listBox in ourListBoxes)
			{
				for (int i = 0; i < listBox.Items.Count; i++)
				{
					ourFiles.Add((string)listBox.Items[i]);
				}
			}

			foreach (string file in ourFiles)
			{
				if (files.Contains(AddCommonPath(file)))
				{
					foreach (var listBox in ourListBoxes)
					{
						if (listBox.Items.Contains(file))
						{
							int index = listBox.Items.IndexOf(file);
							if (listBox.CheckedIndices.Contains(index))
							{
								SetItemCheckState_TS(listBox, index, CheckState.Unchecked);
							}

							// After adding the index to the specialItems, the checked-state of
							// that item can no longer be changed.
							listBox.SpecialItems.Add(index);
						}
					}
				}
			}
		}

		private delegate void SetItemCheckStateDelegate(CheckedListBox listbox, int index, CheckState state);
		private void SetItemCheckState_TS(CheckedListBox listbox, int index, CheckState state)
		{
			if (listbox.InvokeRequired)
			{
				SetItemCheckStateDelegate del = SetItemCheckState_TS;
				this.Invoke(del, new object[] { listbox, index, state });
			}
			else
			{
				listbox.SetItemCheckState(index, state);
			}
		}



		/// <summary>
		/// Handle the check change of internal checkedListBoxes. 
		/// </summary>
		/// <param name="sender">Sender of the event</param>
		/// <param name="args">Arguments</param>
		private void HandleCheckChange(object sender, ItemCheckEventArgs args)
		{
			// In the case where a user tries to change the checked state of a 
			// special item the CurrentValue will be equal to the NewValue, as
			// SpecialItems are not allowed to have their check-state changed.
			if (args.CurrentValue != args.NewValue)
			{
				ColoredCheckedListBox listBox = sender as ColoredCheckedListBox;
				List<string> alteredFiles = new List<string>(new string[] { (string)listBox.Items[args.Index] });

				OnSelectionChanged(this, (args.NewValue == CheckState.Checked), alteredFiles);
			}
		}

		protected override void OnSelectionChanged(object sender, bool selected, List<string> files = null)
		{
			if (files != null)
			{
				files = files.Select(fileName => AddCommonPath(fileName)).ToList();
			}

			base.OnSelectionChanged(sender, selected, files);
		}
		#endregion

		#region file_path_construction
		/// <summary>
		/// Add the common path to fileNames.
		/// </summary>
		/// <param name="file">File without common path</param>
		/// <returns>File path preceded by the common path</returns>
		private string AddCommonPath(string file)
		{
			return CommonPath + file;
		}

		/// <summary>
		/// Remove the common path from a fileName.
		/// </summary>
		/// <param name="file">File preceded by the CommonPath.</param>
		/// <returns>Substring of the fileName with the CommonPath removed.</returns>
		private string RemoveCommonPath(string file)
		{
			return file.Substring(Math.Max(0, CommonPath.Length - 1));
		}
		#endregion
	}
}
