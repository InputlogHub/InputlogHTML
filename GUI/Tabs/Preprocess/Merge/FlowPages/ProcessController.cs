using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
	public partial class ProcessController : UserControl
	{
		private string FolderPath;

		private int CurrentNrOfLines; // Current nr of lines in the process control window
		private const int MAX_NR_OF_LINES = 1500; // max nr of lines
		private const int REMOVE_NR_OF_LINES_BY = 500; // when maximum has been hit, remove the first x nr of lines
		private readonly Queue<int> CharactersToRemove;
		private int CurrentCharCount;
		private readonly int NewLineSize;

		public ProcessController()
		{
			InitializeComponent();
			OpenFolderLbl.Visible = false;
		    OpenFolderLbl.Click += delegate { Process.Start("Explorer", "/n,/root," + FolderPath); };

			CharactersToRemove = new Queue<int>();
			NewLineSize = Environment.NewLine.Length;
		}

	    private delegate void AlterOpenFolderLblDelegate(string folderPath, bool show);
		public void AlterOpenFolderLbl(string folderPath, bool show)
		{
			if (InvokeRequired)
			{
				var del = new AlterOpenFolderLblDelegate(AlterOpenFolderLbl);
				Invoke(del, new object[] { folderPath, show });
			}
			else
			{
				OpenFolderLbl.Text = "Open directory: \"" + folderPath + "\"";
				OpenFolderLbl.Visible = show;
				FolderPath = folderPath;
			}
		}

		public void WriteProcessInfo(string message)
		{
			AppendTextTs(message);
		}

	    private delegate void AppendTextDelegate(string message);
		
        public void AppendTextTs(string message)
		{
			if (InvokeRequired)
			{
				var del = new AppendTextDelegate(AppendTextTs);
				if (!IsDisposed)
				{
					Invoke(del, new object[] { message });
				}
			}
			else
			{
				if (!Console.IsDisposed)
				{
					CurrentCharCount += message.Length + NewLineSize;
					++CurrentNrOfLines;

					if (CurrentNrOfLines % REMOVE_NR_OF_LINES_BY == 0)
					{
						CharactersToRemove.Enqueue(CurrentCharCount);
						CurrentCharCount = 0;
					}

					if (CurrentNrOfLines > MAX_NR_OF_LINES)
					{
						CurrentNrOfLines -= REMOVE_NR_OF_LINES_BY;
						int toRemove = CharactersToRemove.Dequeue();
						Console.Text = Console.Text.Remove(0, toRemove);
					}

					Console.AppendText(message + Environment.NewLine);
					Console.SelectionStart = Console.Text.Length;
					Console.ScrollToCaret();
				}
			}
		}
	}
}
