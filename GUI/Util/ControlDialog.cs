using System.Text;
using System.Windows.Forms;

namespace GUI.Util
{
	public partial class ControlDialog : Form
	{
		public ControlDialog()
		{
			InitializeComponent();
			DataList.KeyUp += new KeyEventHandler(ControlDialog_KeyUp);
			this.TopMost = true;
		}

		private void ControlDialog_KeyUp(object sender, KeyEventArgs args)
		{
			if (args.KeyCode == Keys.C && args.Control)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < DataList.SelectedItems.Count; i++)
				{
					ListViewItem item = DataList.SelectedItems[i];
					for (int j = 0; j < DataList.Columns.Count; j++)
					{
						sb.AppendLine(DataList.Columns[j].Text + ": " + item.SubItems[j].Text);
					}
					sb.AppendLine();
				}
				SetClipboardHelper clipboardH = new SetClipboardHelper(DataFormats.Text, sb.ToString());
				clipboardH.Go();
			}
		}

	}
}
