using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GUI.Flow;
using GUI.Tabs.Preprocess.Merge.FlowPages;
using InputLog.Core.Util;
using System.IO;

namespace GUI.Tabs.Preprocess.Merge
{
	public partial class MergeSelector : UserControl
	{
		#region private_fields

		/// <summary>
		/// Dictionary with all the different types of files that can serve as input
		/// to merge with their corresponding idfx files. These are coupled with their 
		/// respective merge flow-gui's that handle the actual merging and the getting 
		/// the required user input.
		/// </summary>
		Dictionary<String, Type> InputFileTypes = new Dictionary<string,Type>();
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MergeSelector()
		{
			InitializeComponent();

			// Insert the Available input file types.
            if (Directory.Exists("DNSOld"))
            {
                InputFileTypes.Add("Dragon Naturally Speaking (*.dat)", typeof(DragonFlowController));
            }
			InputFileTypes.Add("Tobii/Eyelink files (*.tsv)", typeof(EyetrackFlowController));

			// Add file types to the dropdownlist.
			//InputFilesDD.Items.AddRange(InputFileTypes.Keys.ToArray());
            //InputFilesDD.SelectedIndex = InputFileTypes.Count - 1;
		}


		private void MergeButton_Click(object sender, EventArgs e)
		{
			try
			{
                AbstractFlowController flowController = (AbstractFlowController)Activator.CreateInstance(InputFileTypes[(string)"test"]);//InputFilesDD.SelectedItem]);
                FlowGUI flowWindow = new FlowGUI(flowController);
                flowWindow.Show();
				/*/ TODO IMPROVE THIS
				if (InputFilesDD.SelectedIndex == 1)
				{
					AbstractFlowController flowController = (AbstractFlowController)Activator.CreateInstance(InputFileTypes[(string)InputFilesDD.SelectedItem]);
					FlowGUI flowWindow = new FlowGUI(flowController);
					flowWindow.Show();
				}
				else
				{
                    AbstractFlowController flowController = (AbstractFlowController)Activator.CreateInstance(InputFileTypes[(string)InputFilesDD.SelectedItem]);
                    FlowGUI flowWindow = new FlowGUI(flowController);
                    flowWindow.Show();
					//MessageBox.Show("Currently not implemented yet.", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}*/
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Merging of files failed!");
			}

		}
	}
}
