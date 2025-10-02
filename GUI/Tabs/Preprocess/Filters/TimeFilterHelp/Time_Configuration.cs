using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO.Basic;
using InputLog.Core.Util;

namespace GUI.Tabs.Preprocess.Filters.TimeFilterHelp
{
    /// <summary>
    /// 20131023 Several fixes throughout the code. The id of an event is not its position in the list (index)
    /// but the value of the event "id" property.
    /// Reason: the idfx may have been truncated earlier so that the true first event with index at 0 has a
    /// different nominal "id" value and consequently all other events too.
    /// </summary>
	public partial class TimeConfiguration : Form
	{
		#region members
		/// <summary>
		/// New starting and ending parameters entered by the user in the Filter Panel.
		/// </summary>
		public string NewStartID { get; set; }
		public string NewStopID { get; set; }
        public string InitStartID { get; private set; }
        public string InitStopID { get; private set; }
        public string EventValue { get; private set; }
        public bool ResetTime { get; private set; }

		/// <summary>
		/// The event list to manipulate.
		/// </summary>
		private List<Event> EventList { get; set; }

		/// <summary>
		/// The start offset to subtract from the Start time of an Event to get the relative action and pause times.
		/// </summary>
		private ulong TimeOffset;

		/// <summary>
		/// The number of events to consider. The newer idfx have 'Statistics' as their last event. 
		/// It should count as an event proper.
		/// </summary>
		private int EventCount;

		/// <summary>
		/// Features used for input validation.
		/// </summary>
		private TextBox ThisBBox;
		private TextBox ThisEBox;
		private Label ThisBLabel;
		private Label ThisELabel;

		/// <summary>
		/// Bool to track whether the events have already been read, and/or are 
		/// still up to date.
		/// </summary>
		private bool EventsRead;

        /// <summary>
        /// Multiple files need to be filtered on the first and last keyboard event.
        /// </summary>
        public bool FixedStartEnd { get;  set; }

        /// <summary>
		/// Boolean that describes whether the time filtering is id-based or 
        /// time-based. True if it is id-based, false if it is time-based or FixedStartEnd.
		/// </summary>
		public bool IDBased { get; private set; }

        /// <summary>
        /// Events following after the last instance of the selected value can be removed.
        /// </summary>
        private readonly string[] _eventValues = { "", "=" };
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TimeConfiguration()
		{
			InitializeComponent();
            eventValueCBx.Items.AddRange(_eventValues.ToArray());
            eventValueCBx.SelectedIndex = 0;
        }

		/// <summary>
		/// Obtaining the starting and ending events that may be changed.
		/// Preparing the input screen.
		/// </summary>
		public void ReadEvents(string filePath)
		{
			try
			{
				var eventLogReader = EventLogFactory.CreateFileEventLogReader(filePath, LogFormat.XML);
				// Getting the startoffset.
				var sessionId = eventLogReader.ReadSessionIdentification();
                string offset = sessionId.GetRelativeCreationTime().ToString();

				// Getting the events.
				EventList = eventLogReader.ReadEvents();
				SetDefaultValues(offset);
				EventsRead = true;
				IDBased = true;
                id1Gbx.Select();
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Unable to read events from file. " +
																		"Are you sure you provided a valid logfile?");
			}
		}

		/// <summary>
		/// Callback method for the onShown event of the Time_Configuration dialog.
		/// If the events are not read before the show is called, it is assumed that
        /// multiple files are being processed and only the FixedStartEnd mode is enabled.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e)
		{
			if (!EventsRead)
			{                
                selectIDRb.Enabled = false;
                selectTimeRb.Enabled = false;
			    ShowFixedIDPanel();
			}
			base.OnShown(e);
		}

		/// <summary>
		/// Function to be called when the selected files have been changed.
		/// This requires a reread of the event list.
		/// </summary>
		public void OnSelectedFilesChanged()
		{
			EventsRead = false;
		}

        /// <summary>
        /// A new event value selected from the combobox.
        /// </summary>
        /// <param name="sender">The EventValue ComboBox</param>
        /// <param name="e"></param>
	    private void SelectValueChanged(object sender, EventArgs e)
	    {
            EventValue = eventValueCBx.SelectedItem.ToString();
        }

	    /// <summary>
        ///  Setting the default Start & End ID, and the default Start & End Time
        ///  in the GUI. Data extracted from the *.idfx.
        /// </summary>
        /// <param name="offset">Time laps to subtract from the Start & End Time 
        /// to get a relative action and pause timing.</param>
        private void SetDefaultValues(object offset)
		{
			TimeOffset = (offset == null ? 0 : Convert.ToUInt64(offset));

			// An idfx file has real events up to the 'statistics' part, but older idfx's don't have 'statistics'.
			// We look up the position of a possible 'statistics' element in reverse order to save time.
			// Initial event count value is the maximum events.
			EventCount = EventList.Count - 1;
			for (int i = EventList.Count - 1; i >= 0; i--)
			{
				if (EventList[i].Type.Equals("statistics"))
				{
					EventCount--;
					break;
				}
				// We search backwards for a few steps to be sure that we will not encounter those 'statistics'.
				if (i < EventList.Count - 5) break;
			}

			// Filling the TextBoxes and labels
            InitStartID = EventList[0].Properties["id"];
            InitStopID = EventList[EventCount].Properties["id"];
		    beginIDTbx.Text = InitStartID;
			beginID2Tbx.Text = beginIDTbx.Text;
		    endIDTbx.Text = InitStopID;
			endID2Tbx.Text = endIDTbx.Text;
			endIDLbl.Text = "End ID: " + endIDTbx.Text;
			endID2Lbl.Text = "End ID: " + endIDTbx.Text;
			beginTimeTbx.Text = GetTime(0).ToString();
			endTimeTbx.Text = GetTime(EventCount).ToString();
			startTimeLbl.Text = "Start Time: " + beginTimeTbx.Text;
			endTimeLbl.Text = "End Time: " + endTimeTbx.Text;
			startIDLbl.Text = "Start ID: " + beginIDTbx.Text;
			startID2Lbl.Text = "Start ID: " + beginIDTbx.Text;
			beginTime2Tbx.Text = GetTime(0).ToString();
			startTime2Lbl.Text = "Start Time: " + beginTimeTbx.Text;
			endTime2Tbx.Text = GetTime(EventCount).ToString();
			endTime2Lbl.Text = "End Time: " + endTimeTbx.Text;
            NewStartID = beginIDTbx.Text;
			NewStopID = endIDTbx.Text;
			ResetTime = false;

			selectIDRb.Select();
			id1Gbx.Focus();
		}

		/// <summary>
		/// Looking up the start time of the event, minus the specified time offset.
		/// </summary>
		/// <param name="eventID">The ID of an event.</param>
		/// <returns>The relative Start time for the event.</returns>
		private ulong GetTime(int eventID)
		{
			var thisEvent = EventList[eventID];
			var timedEventPart = Event.GetFirstEventPart<TimedEventPart>(thisEvent);
            if (timedEventPart == null)
			{
				return 0;
			}
			return timedEventPart.StartTime - TimeOffset;
		}

		/// <summary>
		/// Returns the ID of an event when its Start or End Time is given.
		/// </summary>
		/// <param name="time">The relative Start or End Time for this event</param>
		/// <returns>The ID for this event.</returns>
		private int GetID(ulong time)
		{
			var thisTime = time + TimeOffset;
			var closeId = -1;

			for (int i = 0; i <= EventCount; i++)
			{
				var thisEvent = EventList[i];
				var timedEventPart = Event.GetFirstEventPart<TimedEventPart>(thisEvent);
			    if (timedEventPart != null && thisTime.Equals(timedEventPart.StartTime))
			    {
			        closeId = Convert.ToInt32(EventList[i].Properties["id"]);
			        break;
			    }
			}
			return closeId;
		}

		/// <summary>
		/// User input validation utility tool.
		/// </summary>
		/// <param name="pattern">the regex pattern</param>
		/// <param name="text">the text to control</param>
		/// <returns>boolean true if matched</returns>
		private bool IsValid(string pattern, string text)
		{
			return Regex.IsMatch(text, pattern);
		}

		/// <summary>
		/// Validating suite for the 'Start ID' user input.
		/// Emits a flashing error icon next to the TextBox with the invalid input.
		/// </summary>
		/// <param name="sender">Input as text</param>
		/// <param name="e">Data for a cancelable event </param>
		private void ValidateStartID(object sender, CancelEventArgs e)
		{
			string errorMsg;
            if (!IsValidStartID(ThisBBox.Text, ThisEBox.Text, out errorMsg))
			{
				e.Cancel = true;
                ThisBBox.Text = EventList[0].Properties["id"];
			    NewStartID = ThisBBox.Text;
				ThisBLabel.Text = "Start Time: " + GetTime(0);

				ThisBBox.Select(0, ThisBBox.Text.Length);
				EventErrorProvider.SetError(ThisBBox, errorMsg);
			}
		}

		/// <summary>
		/// Removes error messages (if any) when Start Id input is valid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StartIDValidated(object sender, EventArgs e)
		{
			EventErrorProvider.SetError(ThisBBox, "");
		}

		/// <summary>
		/// Valdation tests for the input of a Start Id: 
        /// unsigned digits only; Start Id should be equal or greater than the Id at EventList[0] 
        /// and Start Id should be smaller than the End Id.
		/// </summary>
		/// <param name="startID">the StartId from the input TextBox</param>
		/// <param name="endID">the orignal EndId, not from the TextBox,
		///  because the Textbox might have been changed already.</param>
		/// <param name="errorMessage">sets the error message to use with an invalid input.</param>
		/// <returns>bool valid is true or false</returns>
		private bool IsValidStartID(string startID, string endID, out string errorMessage)
		{
            var oldId = Convert.ToInt32(EventList[0].Properties["id"]);
            if (IsValid(@"^\d+$", startID) 
                && Convert.ToInt32(startID) >= oldId 
                && Convert.ToInt32(startID) < Convert.ToInt32(endID))
			{
                // startID is an event property. Subtracting the beginID property will give us the index of the EventList. 
                ThisBLabel.Text = "Start Time: " + GetTime(Convert.ToInt32(startID) - oldId);
                if (ResetTime) ThisBLabel.Text = "Start Time: 0";
			    NewStartID = startID;
				errorMessage = "";
				return true;
			}

			errorMessage = $"The start ID should be between {EventList[0].Properties["id"]} and {endID}.";
			return false;
		}

		/// <summary>
		/// Validating suite for the 'End ID' user input.
		/// Emits a flashing error icon next to the TextBox with the invalid input.
		/// </summary>
		/// <param name="sender">Input as text</param>
		/// <param name="e">Data for a cancelable event </param>
		private void ValidateEndID(object sender, CancelEventArgs e)
		{
			string errorMsg;
            if (!IsValidEndID(ThisBBox.Text, ThisEBox.Text, out errorMsg))
			{
				e.Cancel = true;
                ThisEBox.Text = (EventList[EventCount].Properties["id"]);
			    NewStopID = ThisEBox.Text;
				ThisELabel.Text = "End Time: " + GetTime(EventCount);
				ThisEBox.Select(0, ThisEBox.Text.Length);
				EventErrorProvider.SetError(ThisEBox, errorMsg);
			}
		}

		/// <summary>
		/// Removes error messages (if any) when EndId input is valid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EndIDValidated(object sender, EventArgs e)
		{
			EventErrorProvider.SetError(ThisEBox, "");
		}

		/// <summary>
		/// Validation tests for the input of an EndId:
		/// unsigned digits only; EndId should be smaller or equal to number of events 
		/// and greater than the Start Id.
		/// </summary>
		/// <param name="beginID">The original beginId, not the actual beginId from the TextBox, 
		/// because the Textbox might have been changed already.</param>
		/// <param name="endID">the endId from the input TextBox</param>
		/// <param name="errorMessage">sets the error message to use with an invalid input.</param>
		/// <returns>bool valid is true or false</returns>
		private bool IsValidEndID(string beginID, string endID, out string errorMessage)
		{
            if (IsValid(@"^\d+$", endID) && Convert.ToInt32(endID) <= Convert.ToInt32(EventList[EventCount].Properties["id"])
			  && (Convert.ToInt32(endID) > Convert.ToInt32(beginID)))
			{
                // endID is an event property. Subtracting the beginID property will give the index of the EventList. 
                ThisELabel.Text = "End Time: " + GetTime(Convert.ToInt32(endID) - Convert.ToInt32(beginID));
			    NewStopID = endID;
				errorMessage = "";
				return true;
			}

            errorMessage = $"The end ID should be between {beginID} and {EventList[EventCount].Properties["id"]}";
			return false;
		}

		/// <summary>
		/// Validating suite for the 'Start Time' user input.
		/// Emits a flashing error icon next to the TextBox with the invalid input.
		/// </summary>
		/// <param name="sender">Input as text</param>
		/// <param name="e">Data for a cancelable event </param>
		private void ValidateStartTime(object sender, CancelEventArgs e)
		{
			string errorMsg;
			if (!IsValidStartTime(ThisBBox.Text, ThisEBox.Text, out errorMsg))
			{
				e.Cancel = true;
				ThisBBox.Text = GetTime(0).ToString();
                ThisBLabel.Text = "Start ID: " + EventList[0].Properties["id"];
				ThisBBox.Select(0, ThisBBox.Text.Length);
				EventErrorProvider.SetError(ThisBBox, errorMsg);
			}
		}

		/// <summary>
		/// Removes error messages (if any) when Start Time input is valid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StartTimeValidated(object sender, EventArgs e)
		{
			EventErrorProvider.SetError(ThisBBox, "");
		}

		/// <summary>
		/// Validation tests for the input of a Start Time:
		/// the input should consists of unsigned digits only; 
		/// there should exist an id that matches the given Start Time;
		/// the Start Time should be smaller than the End Time.
		/// </summary>
		/// <param name="startTime">the Start Time from the input TextBox</param>
		/// <param name="endTime">the End Time from the input TextBox</param>
		/// <param name="errorMessage">sets the error message to use with an invalid input.</param>
		/// <returns>bool valid is true or false</returns>
		private bool IsValidStartTime(string startTime, string endTime, out string errorMessage)
		{
			if (IsValid(@"^\d+$", startTime))
			{
                if (Convert.ToUInt64(startTime) == 0 && ResetTime)
                {
                    ThisBLabel.Text = "Start ID: " + EventList[0].Properties["id"];
                    errorMessage = "";
                    return true;
                }
                var newID = GetID(Convert.ToUInt64(startTime));
				if (newID == -1)
				{
					errorMessage = string.Format("No matching ID for {0} was found.", ThisBBox.Text);
					return false;
				}

				if (Convert.ToUInt64(startTime) < Convert.ToUInt64(endTime))
				{
                    ThisBLabel.Text = "Start ID: " + newID;
                    NewStartID = newID.ToString();
					errorMessage = "";
					return true;
				}
			}

			errorMessage = $"The start time should be between 0 and {GetTime(EventCount - 1)}";
			return false;
		}

		/// <summary>
		/// Validating suite for the 'End Time' user input.
		/// Emits a flashing error icon next to the TextBox with the invalid input.
		/// </summary>
		/// <param name="sender">Input as text</param>
		/// <param name="e">Data for a cancelable event </param>
		private void ValidateEndTime(object sender, CancelEventArgs e)
		{
			string errorMsg;
			if (!IsValidEndTime(ThisBBox.Text, ThisEBox.Text, out errorMsg))
			{
				e.Cancel = true;
				ThisEBox.Text = GetTime(EventCount).ToString();
				ThisELabel.Text = "End ID: " + Convert.ToInt32(EventList[EventCount].Properties["id"]);
				ThisEBox.Select(0, ThisEBox.Text.Length);
				EventErrorProvider.SetError(ThisEBox, errorMsg);
			}
		}

		/// <summary>
		/// Removes error messages (if any) when End Time input is valid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EndTimeValidated(object sender, EventArgs e)
		{
			EventErrorProvider.SetError(ThisEBox, "");
		}

		/// <summary>
		/// Validation tests for the input of a End Time:
		/// the input should consists of unsigned digits only; 
		/// there should be an id that matches the given End Time;
		/// the End Time should be equal or smaller than the End Time for the last event; 
		/// the End Time should be greater than the Start Time.
		/// </summary>
		/// <param name="startTime">the Start Time from the input TextBox</param>
		/// <param name="endTime">the End Time from the input TextBox</param>
		/// <param name="errorMessage">sets the error message to use with an invalid input.</param>
		/// <returns>bool valid is true or false</returns>
		private bool IsValidEndTime(string startTime, string endTime, out string errorMessage)
		{
			if (IsValid(@"^\d+$", endTime))
			{
				var newID = GetID(Convert.ToUInt64(endTime));
				if (newID == -1)
				{
					errorMessage = string.Format("No matching ID for {0} was found.", endTime);
					return false;
				}

				if (Convert.ToUInt64(endTime) <= GetTime(EventCount)
					&& Convert.ToUInt64(endTime) > Convert.ToUInt64(startTime))
				{
                    ThisELabel.Text = "End ID: " + newID;
				    NewStopID = newID.ToString();
					errorMessage = "";
					return true;
				}
			}
            errorMessage = $"The end time should be between {startTime} and {GetTime(EventCount)}.";
			return false;
		}

		/// <summary>
		/// Radio button 'Select ID' selection changed: showing the panel where times can be changed.
		/// Enables/disables controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectIDRbCheckedChanged(object sender, EventArgs e)
		{
			if (selectIDRb.Checked)
			{
                ShowVariableIDPanel();
			}
			else
			{
				idPanel.Visible = false;
				idPanel.SendToBack();
			}
		}

		/// <summary>
		/// Radio button 'Select Time' selection changed: showing the panel where id's can be changed.
		/// Enables/disables controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectTimeRbCheckedChanged(object sender, EventArgs e)
		{
			if (selectTimeRb.Checked)
			{
                timePanel.Visible = true;
                timePanel.BringToFront();
                timeGbx1.Focus();
                ResetTime = false;
                FixedStartEnd = false;
                IDBased = false;
			}
			else
			{
				timePanel.Visible = false;
				timePanel.SendToBack();
			}
		}

        /// <summary>
        /// Fixed Start and End ID keeping the start time of the first keyboard event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixedKeepTimeRbCheckedChanged(object sender, EventArgs e)
        {
            ResetTime = false;
        }

        /// <summary>
        /// Fixed Start and End ID with the start time of the first keyboard event
        /// set to zero.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixedZeroTimeRbCheckedChanged(object sender, EventArgs e)
        {
            ResetTime = true;
        }

        public void ShowFixedIDPanel()
        {
            fixedPanel.Visible = true;
            fixedPanel.BringToFront();
            selectIDRb.Visible = false;
            selectTimeRb.Visible = false;
        }

        public void ShowVariableIDPanel()
        {
            fixedPanel.Visible = false;
            fixedPanel.SendToBack();

            idPanel.Visible = true;
			idPanel.BringToFront();
			id1Gbx.Focus();
			ResetTime = false;
            FixedStartEnd = false;
            IDBased = true;
        }

		/// <summary>
		/// Entering the groupBox where the id's (start and/or end) are changed and the start time stays unaltered.
		/// Entering values triggers a validation process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Id1GbxEnter(object sender, EventArgs e)
		{
			id1Gbx.Focus();

            beginID1Lbl.ForeColor = SystemColors.ControlText;        
			endID1Lbl.ForeColor = SystemColors.ControlText;
			beginIDTbx.ForeColor = SystemColors.WindowText;
			endIDTbx.ForeColor = SystemColors.WindowText;

			beginID2Lbl.ForeColor = SystemColors.ControlDark;
			endID3Lbl.ForeColor = SystemColors.ControlDark;
			beginID2Tbx.ForeColor = SystemColors.ControlDark;
			endID2Tbx.ForeColor = SystemColors.ControlDark;

			ResetTime = false;
			ThisBBox = beginIDTbx;
			ThisEBox = endIDTbx;
			ThisBLabel = startTimeLbl;
			ThisELabel = endTimeLbl;
		}

		/// <summary>
		/// Entering the groupBox where the id's (start and/or end) are changed and the start time is set to zero.
		/// Entering values triggers a validation process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void IdGbx2Enter(object sender, EventArgs e)
		{
			idGbx2.Focus();

			beginID2Lbl.ForeColor = SystemColors.ControlText;
			endID3Lbl.ForeColor = SystemColors.ControlText;
			beginID2Tbx.ForeColor = SystemColors.WindowText;
			endID2Tbx.ForeColor = SystemColors.WindowText;

			beginID1Lbl.ForeColor = SystemColors.ControlDark;
			endID1Lbl.ForeColor = SystemColors.ControlDark;
			beginIDTbx.ForeColor = SystemColors.ControlDark;
			endIDTbx.ForeColor = SystemColors.ControlDark;

			ResetTime = true;
			ThisBBox = beginID2Tbx;
			ThisEBox = endID2Tbx;
			ThisBLabel = startTime2Lbl;
			ThisELabel = endTime2Lbl;
		}

		/// <summary>
		/// Entering the groupBox where the time (start and/or end) is changed and the start time stays unaltered.
		/// Entering values triggers a validation process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimeGbx1Enter(object sender, EventArgs e)
		{
			timeGbx1.Focus();

			eventLbl2.ForeColor = SystemColors.ControlText;
			eventLbl.ForeColor = SystemColors.ControlText;
			beginTimeTbx.ForeColor = SystemColors.WindowText;
			endTimeTbx.ForeColor = SystemColors.WindowText;

			event2Lbl.ForeColor = SystemColors.ControlDark;
			event3Lbl.ForeColor = SystemColors.ControlDark;
			beginTime2Tbx.ForeColor = SystemColors.ControlDark;
			endTime2Tbx.ForeColor = SystemColors.ControlDark;

			ResetTime = false;
			ThisBBox = beginTimeTbx;
			ThisEBox = endTimeTbx;
			ThisBLabel = startIDLbl;
			ThisELabel = endIDLbl;
		}

		/// <summary>
		/// Entering the groupBox where the time (start and/or end) is changed 
		/// and the start time is set to zero.
		/// Entering values triggers a validation process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimeGbx2Enter(object sender, EventArgs e)
		{
			timeGbx2.Focus();

			event2Lbl.ForeColor = SystemColors.ControlText;
			event3Lbl.ForeColor = SystemColors.ControlText;
			beginTime2Tbx.ForeColor = SystemColors.WindowText;
			endTime2Tbx.ForeColor = SystemColors.WindowText;

			eventLbl2.ForeColor = SystemColors.ControlDark;
			eventLbl.ForeColor = SystemColors.ControlDark;
			beginTimeTbx.ForeColor = SystemColors.ControlDark;
			endTimeTbx.ForeColor = SystemColors.ControlDark;

			ResetTime = true;
			ThisBBox = beginTime2Tbx;
			ThisEBox = endTime2Tbx;
			ThisBLabel = startID2Lbl;
			ThisELabel = endID2Lbl;
		}

		/// <summary>
		/// The start value currently specified by the user.
		/// </summary>
		/// <returns></returns>
		public string GetStartBox()
		{
            return ThisBBox != null ? ThisBBox.Text : "0";
		}

		/// <summary>
		/// The end value currently specified by the user.
		/// </summary>
		/// <returns></returns>
		public string GetEndBox()
		{
            return ThisEBox != null ? ThisEBox.Text : "0";
		}

		/// <summary>
		/// The start label currently derived from the user input.
		/// </summary>
		/// <returns></returns>
		public string GetStartLabel()
		{
            return ThisBLabel != null ? ThisBLabel.Text : "0";
		}

		/// <summary>
		/// The end label currently derived from the user input.
		/// </summary>
		/// <returns></returns>
		public string GetEndLabel()
		{
            return ThisELabel != null ? ThisELabel.Text : "0";
		}

        /// <summary>
        /// Exit the configuration panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptBtnClick(object sender, EventArgs e)
        {         
            if (FixedStartEnd)
            {
                NewStartID = fixedSkipToFirstKeyRb.Checked ? "fix" : GetStartBox();
                NewStopID = fixedSkipToFinalKeyRb.Checked ? "fix" : GetEndBox();
                ActiveForm?.Close();
            }

            else if(NewStartID.Equals(EventList[0].Properties["id"]) 
                && NewStopID.Equals(EventList[EventCount].Properties["id"]))
            {
                ActiveForm?.Close();
            }
        }

        private void CancelBtnClick(object sender, EventArgs e)
        {
            Close();
        }
	}
}
