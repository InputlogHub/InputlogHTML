using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using InputLog.Core.Util.Matching;
using InputLog.Core.Util;
using GUI.Util;

namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
	public partial class OffsetControler : UserControl
	{
		#region private_members
		/// <summary>
		/// Offsets to be displayed in the control.
		/// </summary>
		private Dictionary<IMatch<string>, int> Offsets;

		/// <summary>
		/// The offset entries for each match.
		/// </summary>
		private List<OffsetEntry> Entries;
		#endregion

		/// <summary>
		/// Construct the offset controller.
		/// </summary>
		/// <param name="infoText"></param>
		public OffsetControler(string infoText)
		{
			InitializeComponent();
			InfoLbl.Text = infoText;

			VisibleChanged += HandleVisibleChanged;
		}

		/// <summary>
		/// This call is made so that when the control finally becomes visible, and it 
		/// has not yet filled its flowpanel with the subcontrols because the handle was not 
		/// yet created, the controls will be added now. 
		/// As, upon being visible, a control should have its handle created.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HandleVisibleChanged(object sender, EventArgs args)
		{
			if (Visible == true)
			{
				SetOffsets(Offsets);
			}
		}

		/// <summary>
		/// Set the offsets to be displayed by the control.
		/// If the offsets are null, nothing will be displayed. 
		/// </summary>
		/// <param name="offsets">The offsets to be displayed.</param>
		private delegate void SetOffsetsDelegate(Dictionary<IMatch<string>, int> offsets);
		public void SetOffsets(Dictionary<IMatch<string>, int> offsets)
		{
			Offsets = offsets;

			if (IsHandleCreated)
			{
				Invoke((MethodInvoker)(() => FlowPanel.Controls.Clear()));
				Entries = new List<OffsetEntry>();

				if (Offsets != null)
				{
					AddControls();
				}
			}
		}

		/// <summary>
		/// Get the offsets as they are currently defined on the controller.
		/// </summary>
		/// <returns>The offset for each match, as currently specified on the controller.</returns>
		public List<KeyValuePair<IMatch<string>,int>> GetOffsets()
		{
			List<KeyValuePair<IMatch<string>, int>> offsets = new List<KeyValuePair<IMatch<string>, int>>();
			foreach (OffsetEntry entry in Entries)
			{
				KeyValuePair<IMatch<string>, int> pair = new KeyValuePair<IMatch<string>, int>(entry.Match, entry.Offset);
				offsets.Add(pair);
			}

			return offsets;
		}

		/// <summary>
		/// Helper function adds an offset control to the flow panel for each match
		/// in the offsets list.
		/// </summary>
		private void AddControls()
		{
			Invoke((MethodInvoker)(() => SuspendLayout()));
			foreach (var pair in Offsets)
			{
				OffsetEntry entry = new OffsetEntry(pair.Key, pair.Value, this);
				Entries.Add(entry);
				AddControlToFlowPanel_TS(entry.Content);
			}
			Invoke((MethodInvoker)(() => ResumeLayout()));
		}

		#region thread_safe_helper_methods
		private delegate void AddControlToFlowPanelDelegate(Control control);
		private void AddControlToFlowPanel_TS(Control control)
		{
			if (InvokeRequired)
			{
				AddControlToFlowPanelDelegate del = new AddControlToFlowPanelDelegate(AddControlToFlowPanel_TS);
				Invoke(del, new object[] { control });
			}
			else
			{
				if (IsHandleCreated)
				{
					FlowPanel.Controls.Add(control);
				}
			}
		}
		#endregion

		/// <summary>
		/// A single TableLayout entry for an offset.
		/// </summary>
		public class OffsetEntry
		{
			/// <summary>
			/// The panel of the entry.
			/// </summary>
			public Control Content { get; private set; }

			/// <summary>
			/// The current offset value specified.
			/// This is either the value as it is currently specified in the control, or if the control
			/// has not yet been created, it is the default value the OffsetEntry was created with.
			/// </summary>
			private int _offset;
			public int Offset
			{
				get
				{
					if (Content != null)
					{
						NumericUpDown OffsetSpin = null;
						foreach (Control c in Content.Controls)
						{
							if (c is NumericUpDown)
							{
								OffsetSpin = c as NumericUpDown;
								break;
							}
						}
						if (OffsetSpin != null)
						{
							_offset = ReadOffsetValue(OffsetSpin);
						}
					}
					return _offset;
				}
				set
				{
					_offset = value;
					if (Content != null)
					{
						NumericUpDown OffsetSpin = null;
						foreach(Control c in Content.Controls)
						{
							if (c is NumericUpDown)
							{
								OffsetSpin = c as NumericUpDown;
								break;
							}
						}
						if (OffsetSpin != null)
						{
							OffsetSpin.SetPropertyThreadSafe(() => OffsetSpin.Value, value);
						}
					}
				}
			}

			/// <summary>
			/// The match in this offset entry.
			/// </summary>
			public IMatch<string> Match { get; private set; }

			/// <summary>
			/// Id of the match, this is the largest common path
			/// of the selecteditems in the match.
			/// </summary>
			public string MatchID
			{
				get
				{
					return StringUtils.FindCommonPath(Match.SelectedItems());
				}
			}

			/// <summary>
			/// The parent control of this OffsetEntry.
			/// </summary>
			private Control Parent;

			/// <summary>
			/// Construct the OffsetEntry.
			/// </summary>
			/// <param name="match">The match in this entry.</param>
			/// <param name="initialOffset">The initial offset for this entry.</param>
			public OffsetEntry(IMatch<string> match, int initialOffset, Control parentControl)
			{
				Offset = initialOffset;
				Match = match;
				Parent = parentControl;

				InitializeComponents();
				Parent.SizeChanged += HandleSizeChanged;
			}

			/// <summary>
			/// Read the offset value from the numericupdown control in a threadsafe manner.
			/// </summary>
			/// <param name="control">NumericUpDown control to read the value from.</param>
			/// <returns>The value from the spinner, as an int.</returns>
			private int ReadOffsetValue(NumericUpDown control)
			{
				if (control.InvokeRequired)
				{
					Func<NumericUpDown, int> del = new Func<NumericUpDown, int>(ReadOffsetValue);
					return (int) control.Invoke(del, new object[] { control });
				}
				else
				{
					return (int) control.Value;
				}
			}

			private void HandleSizeChanged(object sender, EventArgs args)
			{
				InitializeComponents();
			}

			/// <summary>
			/// Initialize the graphical part of this component.
			/// </summary>
			private void InitializeComponents()
			{
				TextBox textBox = new TextBox();
				textBox.AppendLine(MatchID);
				foreach(string item in Match.SelectedItems())
				{
					textBox.AppendLine( "\t- " + item);
				}
				Graphics txtGraphics = textBox.CreateGraphics();
				SizeF sizeOfTxt = txtGraphics.MeasureString(textBox.Text, textBox.Font);
				SizeF oneLineSize = txtGraphics.MeasureString("one line", textBox.Font);

				textBox.WordWrap = false;
				textBox.Margin = new Padding(3, 3, 50, 3);
				textBox.Dock = DockStyle.Fill;
				textBox.Multiline = true;
				textBox.MaxLength = 0; // no upper limit.


				TableLayoutPanel layout = new TableLayoutPanel();
				layout.RowCount = 1;
				layout.ColumnCount = 2;
				layout.Width = Parent.Width - Parent.Margin.Horizontal - 15; // -15 px for possible scrollbars.
				layout.Height = (int)Math.Floor(sizeOfTxt.Height + 1) + textBox.Margin.Vertical
					+ (int)Math.Floor(oneLineSize.Height + 1) + 10; // txtBox size + margins + extra line + arbitrary 10px

				layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // layout.width - 100px
				layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); // 100px width
				layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
				layout.Margin = new Padding(0, 0, 0, 15); // 15 px for possible bottom scrollbar.


				NumericUpDown numericScroll = new NumericUpDown();
				numericScroll.Anchor = AnchorStyles.Left | AnchorStyles.Right;
				numericScroll.DecimalPlaces = 0;
				numericScroll.TextAlign = HorizontalAlignment.Center;
				numericScroll.Minimum = Decimal.MinValue;
				numericScroll.Maximum = Decimal.MaxValue;
				numericScroll.Value = (Offset > numericScroll.Maximum) ? numericScroll.Maximum : 
					(Offset < numericScroll.Minimum) ? numericScroll.Minimum : Offset;
				numericScroll.Minimum = Decimal.MinValue;
				numericScroll.Maximum = Decimal.MaxValue;

				layout.Controls.Add(textBox, 0, 0);
				layout.Controls.Add(numericScroll, 1, 0);

				Content = layout;
			}
		}

		private void FlowPanel_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}
