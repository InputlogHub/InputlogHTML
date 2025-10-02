using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Util.Progress;
using System.IO;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Input;
using InputLog.Core.IO;
using InputLog.Core.Events;
using InputLog.Core.IO.Tobii;
using InputLog.Core.IO.Xml.Output;
using InputLog.Core.IO.Xml.Input;
using InputLog.Core.Util.Matching;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.EyeTracking.SubParts;
using InputLog.Core.Util.KeyConversion;
using System.Threading.Tasks;
using InputLog.Core.Util;
using System.Threading;

namespace InputLog.Core.Merging.Sources.ProcessTasks
{
    public class TobiiOffsetCalculator : IOffsetCalculator
	{

		#region private_fields

		/// <summary>
		/// All matches to calculate offset for.
		/// </summary>
		private List<IMatch<string>> Matches;

		/// <summary>
		/// Map from token to index in the TSV file.
		/// If a token appears on the 5th position in the TSV file, the index will be 5.
		/// </summary>
		//private Dictionary<string, int> IndexMap;

		private const string IDFX_EXT = ".idfx";
		private const string TOBII_EXT = ".tsv";
		private const char CSVDELIMITER = '\t';
		private const int REQUIREDMATCHES = 5;
		private const int MAXMATCHES = 6;
		private char[] Delimiters = { CSVDELIMITER };

		/// <summary>
		/// Number of milliseconds to artificially increase the offset with.
		/// </summary>
		private const int PUSH_UP_OFFSET = 3;

		/// <summary>
		/// Holds all the session information, keyed to the file name.
		/// </summary>
		private Dictionary<string, SessionIdentification> SessionIds;

		/// <summary>
		/// Holds all the Events, keyed to the file name.
		/// </summary>
		private Dictionary<string, List<Event>> Events;

		/// <summary>
		/// Lock making sure that the EventReader is only accessed in a 
        /// thread-safe way.
		/// </summary>
		private Object EventReaderLock = new Object();

		/// <summary>
		/// Maps common keys from tobii to inputlog keys.
		/// </summary>
		private Dictionary<string, string> CommonKeyMap = new Dictionary<string, string>()
		{
			{ "A", KeysEx.VK_A.ToString() },
			{ "E", KeysEx.VK_E.ToString() },
			{ "I", KeysEx.VK_I.ToString() },
		};

		/// <summary>
		/// Encapuslation class.
		/// </summary>
		private class MatchEncapsulator
		{
			public IMatch<string> Value;
			public MatchEncapsulator(IMatch<string> match)
			{
				Value = match;
			}
		}
		#endregion

		#region public_fields
		/// <summary>
		/// Array of the offsets.
		/// </summary>
		public int?[] Offsets;
		#endregion


		public TobiiOffsetCalculator(List<IMatch<string>> matches)
		{
			if (matches == null)
			{
				throw new ArgumentNullException("matches", "Matches can not be null, you must specify at least one match.");
			}

			foreach (IMatch<string> match in matches)
			{
				if (match.SelectedItems().Count == 0)
				{
					continue;
				}
				if (!(match.SelectedItems().Any(file => Path.GetExtension(file) == IDFX_EXT)))
				{
					throw new ArgumentException("A match for merging must contain at least one inputlog (.idfx) file. Make sure" +
					 " that all matches contain at least one inputlog file.");
				}
				try
				{
					match.SelectedItems().Single(file => Path.GetExtension(file) == TOBII_EXT);
				}
				catch (Exception)
				{
					throw new ArgumentException("A match for merging must contain exactly one tobii (.tsv) file. Make sure" +
					" that all matches contain at least one tobii (.tsv) file.");
				}
			}

			NumberOfSteps = matches.Count*2;
			Events = new Dictionary<string, List<Event>>();
			SessionIds = new Dictionary<string, SessionIdentification>();
			Offsets = new int?[matches.Count];
			Matches = matches;
		}

        public override int?[] GetOffsets()
        {
            return Offsets;
        }

		public override void Run()
		{
			try
			{
				ReportProgress(this, new ProgressEventArgs("Processing started", ProgressEventArgs.ProgressCode.STARTED));

				Task[] taskArray = new Task[Matches.Count];
				for (int i = 0; i < Matches.Count; i++)
				{
					taskArray[i] = new Task((obj) =>
					{
						MatchEncapsulator mEncap = (MatchEncapsulator)obj;
						HandleMatch(mEncap.Value);
					},
					new MatchEncapsulator(Matches[i]));
					taskArray[i].Start();
				}

				Task.WaitAll(taskArray);

				if (Offsets.Count() != Matches.Count)
				{
					ReportProgress(this, new ProgressEventArgs("Determining offset failed: \"Number of calculated offsets does " +
						"not match number of active matches\"", ProgressEventArgs.ProgressCode.FAILED));
				}
				/*
				if (Offsets.Any(nullableValue => !nullableValue.HasValue))
				{
					int index = Array.IndexOf(Offsets, Offsets.First(nullableValue => !nullableValue.HasValue));
					string matchName = StringUtils.FindCommonPath(Matches[index].SelectedItems());
					ReportProgress(this, new ProgressEventArgs
						("Determining offset failed: \"Offset value unknown for match: " + matchName + "\"",
						ProgressEventArgs.ProgressCode.FAILED)
					);
				}
				 * */
				else
				{
					ReportProgress(this, new ProgressEventArgs("Determining offset completed.", ProgressEventArgs.ProgressCode.DONE));
				}
			}
			catch (ThreadAbortException) {}
			catch (Exception e)
			{
				try
				{
					ReportProgress(this, new ProgressEventArgs("Determining offset failed.", ProgressEventArgs.ProgressCode.FAILED));
				}
				catch (ThreadAbortException) {}
				finally
				{
					MessageLogger.CatchException(this, e, Severity.INFO);
				}
			}
		}

		/// <summary>
		/// Handle the offset caculation for one match.
		/// </summary>
		/// <param name="match">Match of IPL and Tobii files to calculate the offset for.</param>
		public void HandleMatch(IMatch<string> match)
		{
			if (match.SelectedItems().Count == 0)
			{
				Offsets[Matches.IndexOf(match)] = null;
				return;
			}

			// Separate the tobii files from the idfx files.
			List<string> idfxFiles = new List<string>();
			string tobiiFile = "";
			foreach (string file in match.SelectedItems())
			{
				string ext = Path.GetExtension(file);
				if (ext == TOBII_EXT)
				{
					tobiiFile = file;
				}
				else if (ext == IDFX_EXT)
				{
					idfxFiles.Add(file);
				}
			}

			List<KeyValuePair<string, DateTime>> sortedIdfxs;
			try
			{
				sortedIdfxs = ReadIdfxFiles(idfxFiles);

				// Read tobii file and determine offset.
				int? offset = DetermineOffset(tobiiFile, sortedIdfxs);
				Offsets[Matches.IndexOf(match)] = offset;
			}
			catch (Exception exc)
			{
				MessageLogger.CatchException(this, exc, Severity.ERROR, "Processing failed, exception caught: \"" + exc.Message + "\"");
				ReportProgress(this, new ProgressEventArgs("Processing failed, exception caught: \"" + exc.Message + "\"",
					ProgressEventArgs.ProgressCode.FAILED));
			}

		}

		/// <summary>
		/// Determine the average offset between the idfx file and the tobii file.
		/// The offset specifies how much the tobii values must be altered in order to match the idfx times. <br />
		/// The algorithm searches for common key matches between idfx and tobii starting from 750ms earlier (in the tobii file)
		/// than the first event in the idfx file.
		/// </summary>
		/// <param name="tobiiFile">Tobii file to match the idfx file with.</param>
		/// <param name="sortedIdfxs">The idfx files, sorted, where we should determine the offset for. 
		/// Offset is calculated as the maximum offset encountered over the idfx files.</param>
		/// <returns>The calculated offset to be added to the tobiiFile times in order to match to the idfx events</returns>
		private int? DetermineOffset(string tobiiFile, List<KeyValuePair<string, DateTime>> sortedIdfxs)
		{
			try
			{
				using (StreamReader reader = new StreamReader(File.OpenRead(tobiiFile)))
				{
					string headerLine = reader.ReadLine();
					string dataLine = reader.ReadLine();

					var indexMap = BuildIndexDictionary(headerLine);
					DateTime tobiiStartTime = DateTime.Parse(GetTobiiFullStringTime(dataLine.Split(Delimiters), indexMap));
					DateTime currentTobiiTime = new DateTime(tobiiStartTime.Ticks);

					int returnOffset = 0;
					foreach (KeyValuePair<string, DateTime> idfxPair in sortedIdfxs)
					{
						// Loop until we are somewhere at the right location tobii file.
						DateTime searchTime = idfxPair.Value.AddMilliseconds(-750);
						string line;
						while ((line = reader.ReadLine()) != null && currentTobiiTime < searchTime)
						{
							string[] parts = line.Split(Delimiters, StringSplitOptions.None);
							currentTobiiTime = DateTime.Parse(GetTobiiFullStringTime(parts, indexMap));
						}

						// Find matches between keyboard events.
						int? offsetForFile = FindKeyboardMatchesReturnOffset(reader, idfxPair.Key, indexMap);

						// Take the highest offset of the different idfx files.
						returnOffset = Math.Max(returnOffset, (offsetForFile.HasValue) ? offsetForFile.Value : 0);
					}

					ReportProgress(this, new ProgressEventArgs("Finished determining offset for file: \"" + tobiiFile + "\"",
						ProgressEventArgs.ProgressCode.STEP_COMPLETED));

					return returnOffset;

				}
			}
			catch (Exception exc)
			{
				ReportProgress(this, new ProgressEventArgs("Could not read file: \"" + tobiiFile + "\"\n Exception Caught: \"" + exc.Message + "\"",
						ProgressEventArgs.ProgressCode.FAILED));
				throw;
			}
		}

		/// <summary>
		/// Get the full length time string for any specific line in the log. The timestamp will be the 
		/// timestamp for that entry in the log.
		/// </summary>
		/// <param name="parts"></param>
		/// <returns></returns>
		private string GetTobiiFullStringTime(string[] parts, Dictionary<string, int> indexMap)
		{
			TimePart tobiiStartTimePart = new TimePart(parts[indexMap[TAGS.GN_RecordingDate]],
							parts[indexMap[TAGS.TS_Recording]],
							parts[indexMap[TAGS.TS_Local]],
							parts[indexMap[TAGS.TS_EyeTracker]]
						);
			return tobiiStartTimePart.InterpretData().FullLocalTimestamp;
		}

		
		private int? FindKeyboardMatchesReturnOffset(StreamReader reader, string idfxFile, Dictionary<string, int> indexMap)
		{
			List<Event> kbEvents = Events[idfxFile].FindAll(@event => Event.GetFirstEventPart<KeyPress>(@event) != null);
			ulong idfxStart = (ulong) new TimeSpan(SessionIds[idfxFile].GetCreationDate().Ticks).TotalMilliseconds;
			ulong idfxStartOffset = SessionIds[idfxFile].GetRelativeCreationTime();

			int? offset = null;
			int nrOfMatches = 0;

			List<KeyValuePair<KeyboardEventPart, DateTime>> commonTobiiKeys = new List<KeyValuePair<KeyboardEventPart, DateTime>>();
			
			string line;
			while((line = reader.ReadLine()) != null && nrOfMatches < REQUIREDMATCHES)
			{
				string[] parts = line.Split(Delimiters);
				if (!String.IsNullOrEmpty(parts[indexMap[TAGS.RE_KeyPressEvent]]))
				{

					if (CommonKeyMap.ContainsKey(parts[indexMap[TAGS.RE_KeyPressEvent]]))
					{
						KeyboardEventPart kbPart = new KeyboardEventPart();
						kbPart.Initialize(parts, indexMap);
						DateTime keyTime = DateTime.Parse(GetTobiiFullStringTime(parts, indexMap));
						commonTobiiKeys.Add(new KeyValuePair<KeyboardEventPart,DateTime>(kbPart, keyTime));
						nrOfMatches++;
					}
				}
			}

			int commonKeyCounter = 0;
			offset = 0;
			//int offsetSum = 0;
			foreach (Event @event in kbEvents)
			{
				// Make sure we don't go out of bounds.
				if (commonKeyCounter == commonTobiiKeys.Count)
				{
					break;
				}

				KeyPress winKP = Event.GetFirstEventPart<KeyPress>(@event);
				KeyboardEventPart tobiiKey = commonTobiiKeys[commonKeyCounter].Key;

				if (CommonKeyMap.ContainsValue(winKP.Key.ToString()) &&
					winKP.Key.ToString() == CommonKeyMap[tobiiKey.KeyPressEvent])
				{

					TimeSpan tobiiKeyTime = new TimeSpan(commonTobiiKeys[commonKeyCounter].Value.Ticks);
					ulong iplKeyTime = idfxStart - idfxStartOffset + Event.GetFirstEventPart<TimedEventPart>(@event).StartTime;

					int iplTobiiOffset = (tobiiKeyTime.TotalMilliseconds > iplKeyTime) ?
							((-1)*(int)(tobiiKeyTime.TotalMilliseconds - iplKeyTime)) :
							((int)(iplKeyTime - tobiiKeyTime.TotalMilliseconds));

					if (iplTobiiOffset < 0)
					{
						offset = Math.Min((offset.HasValue ? offset.Value : 0), iplTobiiOffset);
					}
					//offsetSum += iplTobiiOffset;

					commonKeyCounter++;
				}
				else
				{
					// we don't have a match, just ignore it.
				}
			}

			//return offsetSum / REQUIREDMATCHES + PUSH_UP_OFFSET;
			return Math.Abs(offset.Value) + PUSH_UP_OFFSET;
		}

		/// <summary>
		/// Read the idfx information, the events and the session identification, and return an ordered
		/// list of the idfx files, ordered based on their creation date.
		/// </summary>
		/// <param name="idfxFiles">List of idfx files to read, and order.</param>
		/// <returns>An ordered list of idfx files, sorted in ascending order.</returns>
		private List<KeyValuePair<string, DateTime>> ReadIdfxFiles(List<string> idfxFiles)
		{
			List<KeyValuePair<string, DateTime>> startTimesIDFX = new List<KeyValuePair<string, DateTime>>(idfxFiles.Count);

			// Read idfx files.
			foreach (string file in idfxFiles)
			{
				try
				{
					EventLogReader reader;
					lock (EventReaderLock)
					{
						reader = EventLogFactory.CreateFileEventLogReader(file, LogFormat.XML);
					}
					try
					{
						SessionIdentification sID = reader.ReadSessionIdentification();
						List<Event> events = reader.ReadEvents();
						SessionIds.Add(file, sID);
						Events.Add(file, events);
					}
					catch (Exception e)
					{
						throw e;
					}

					// Save their start time separately
					startTimesIDFX.Add(new KeyValuePair<string, DateTime>(file, SessionIds[file].GetCreationDate()));
				}
				catch (Exception exc)
				{
					ReportProgress(this, new ProgressEventArgs("Could not read file: \"" + file + "\"\n Exception Caught: \"" + exc.Message + "\"",
						ProgressEventArgs.ProgressCode.FAILED));
					throw;
				}
			}

			ReportProgress(this, new ProgressEventArgs("Reading IDFX files completed.", ProgressEventArgs.ProgressCode.STEP_COMPLETED));

			// Sort the files based on their start time.
			startTimesIDFX.OrderBy(pair => pair.Value);
			return startTimesIDFX;
		}

		/// <summary>
		/// Builds a dictionary from token to index in the csv file.
		/// </summary>
		/// <param name="header">The headerline containing all the header tokens</param>
		private Dictionary<string, int> BuildIndexDictionary(string header)
		{
			Dictionary<string, int> indexMap = new Dictionary<string, int>(100);
			string[] tokens = header.Split(new char[] { CSVDELIMITER });
			tokens = TobiiUtilities.RenameDuplicateAOIs(tokens);
			for (int i = 0; i < tokens.Length; i++)
			{
				indexMap.Add(tokens[i], i);
			}
			return indexMap;
		}
	}
}
