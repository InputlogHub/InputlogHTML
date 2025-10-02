using System.Windows.Forms;

namespace GUI.Tabs.Preprocess.Filters.TimeFilterHelp
{
	/// <summary>
	/// Helper class that displays the 4 different summaries that are possible
	/// for the time filter. That is:
	/// Keeping start time - with time parameters
	/// Keeping start time - with id parameters
	/// Resetting start time - with time parameters
	/// Resetting start time - with id parameters
	/// </summary>
	public partial class TimeConfigurationInfoPanel : UserControl
	{
		/// <summary>
		/// Construct the summary panel.
		/// </summary>
		public TimeConfigurationInfoPanel()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Set the title of the panel
		/// </summary>
		/// <param name="title">Title of the panel.</param>
		public void SetTitle(string title)
		{
			Title.Text = title;
		}

		/// <summary>
		/// Set the labels for the left side.
		/// e.g. "Start time:" ; "End time:"
		/// </summary>
		/// <param name="startLbl">Start label</param>
		/// <param name="endLbl">End label</param>
		public void SetLeftLabels(string startLbl, string endLbl)
		{
			StartLeftLbl.Text = startLbl;
			EndLeftLbl.Text = endLbl;
		}

		/// <summary>
		/// Set the labels on the right side of the summary
		/// e.g. "Start ID: 0" ; "End ID: 322"
		/// </summary>
		/// <param name="startLbl">Start label</param>
		/// <param name="endLbl">End label</param>
		public void SetRightLabels(string startLbl, string endLbl)
		{
			StartRightLbl.Text = startLbl;
			EndRightLbl.Text = endLbl;
		}

		/// <summary>
		/// Set the values on the left hand side.
		/// E.g: "123456" ; "1234567"
		/// </summary>
		/// <param name="startValue">Start value</param>
		/// <param name="endValue">End value</param>
		public void SetLeftValues(string startValue, string endValue)
		{
			StartLeftTbx.Text = startValue;
			EndLeftTbx.Text = endValue;
		}
	}
}
