using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using InputLog.Core.Events;
using InputLog.Core.Events.DragonNS;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Input;
using InputLog.Core.IO.Basic.Output;
using InputLog.Core.IO.Xml;
using InputLog.Core.Util;
using InputLog.Core.Util.Matching;
using InputLog.Core.Util.Progress;

namespace InputLog.Core.Merging.Sources.ProcessTasks
{
    public class DragonMergeTask: AbstractMergeTask
	{
		#region private_Fields

		/// <summary>
		/// An enumeration of all the match to merge, together with their
		/// specified offsets.
		/// </summary>
		private readonly IEnumerable<KeyValuePair<IMatch<string>, int>> MatchesToProcess;

		// Constant strings
		private const string IDFX_EXT = ".idfx";
		private const string DRAGON_EXT = ".dat";
	    private const string LBL_OLDID = "original_id";

		/// <summary>
		/// The session identification information for each idfx file.
		/// </summary>
		private readonly Dictionary<string, SessionIdentification> SessionIDs;

		/// <summary>
		/// The lists of events for each idfx file.
		/// </summary>
		private readonly Dictionary<string, List<Event>> Events;

		/// <summary>
		/// Factory for creating eventLogWriters and readers.
		/// </summary>
		//private readonly EventLogFactory _factory;

        private DragonNaturallySpeakingData DnsData;

        private string WaveDir;

        private Thread MergingThread;

		#endregion

        public DragonMergeTask(IEnumerable<KeyValuePair<IMatch<string>, int>> matches)
		{
			NumberOfSteps = 1;
			MatchesToProcess = matches;

			SessionIDs = new Dictionary<string, SessionIdentification>();
			Events = new Dictionary<string, List<Event>>();
		}

		/// <summary>
		/// Merge the matches.
		/// </summary>
		public override void Run()
		{
			string currentFile = "";
			try
			{
				ReportProgress(this, new ProgressEventArgs("Started merging", ProgressEventArgs.ProgressCode.STARTED));
				List<Pair<string, IEnumerable<string>>> mergeEntries = 
                    new List<Pair<string, IEnumerable<string>>>(MatchesToProcess.Count());
				List<int> offsets = new List<int>(MatchesToProcess.Count());
				foreach (KeyValuePair<IMatch<string>, int> pair in MatchesToProcess)
				{
					string dat = pair.Key.SelectedItems().Single(path => Path.GetExtension(path) == DRAGON_EXT);
					List<Pair<string, DateTime>> unSortedIdfxs = new List<Pair<string, DateTime>>();
					currentFile = dat;

					foreach (string idfx in pair.Key.SelectedItems().Where(path => Path.GetExtension(path) == IDFX_EXT))
					{
						try
						{
                            EventLogReader reader = EventLogFactory.CreateFileEventLogReader(idfx, LogFormat.XML);
							SessionIDs.Add(idfx, reader.ReadSessionIdentification());
							Events.Add(idfx, reader.ReadEvents());

							unSortedIdfxs.Add(new Pair<string, DateTime>(idfx,SessionIDs[idfx].GetCreationDate()));
						}
						catch (Exception)
						{
							ReportProgress(this, new ProgressEventArgs(
								"Merging failed, could not read idfx file: \"" + idfx + "\"", ProgressEventArgs.ProgressCode.FAILED));
						}
					}

					var sortedIdfx = unSortedIdfxs.OrderBy(entry => entry.Second);

					// Foreach MergeEntry at index i, it's respective offset can be found in the Offsets array at index i.
                    mergeEntries.Add(new Pair<string, IEnumerable<string>>(dat, sortedIdfx.Select(entry => entry.First)));
					offsets.Add(pair.Value);
				}

				NumberOfSteps = Events.Aggregate(0, (sum, pair) => sum + pair.Value.Count) + 1;
				ReportProgress(this, new ProgressEventArgs("Idfx files have been succesfully read.", ProgressEventArgs.ProgressCode.STEP_COMPLETED));

				for (int i = 0; i < mergeEntries.Count; i++)
				{
					Pair<string, IEnumerable<string>> entry = mergeEntries[i];
					int offset = offsets[i];

					Merge(entry, offset);
				}
				ReportProgress(this, new ProgressEventArgs("", ProgressEventArgs.ProgressCode.DONE));
			}
			catch (ThreadAbortException)
			{
				ReportProgress(this, new ProgressEventArgs("Merging failed: thread aborted.", ProgressEventArgs.ProgressCode.FAILED));
			}
			catch (Exception e)
			{
				ReportProgress(this, new ProgressEventArgs("Merging failed when merging file: \"" + currentFile + "\".", ProgressEventArgs.ProgressCode.FAILED));
				MessageLogger.CatchException(this, e, Severity.ERROR);
			}
		}

		/// <summary>
		/// Merge the idfx and Dragon file.
		/// </summary>
		/// <param name="files">The Dragon and idfx files to merge. The first entry of the pair is the 
		/// Dragon pair, the second entry is the enumeration of idfx files, sorted on start date.</param>
		/// <param name="offset">The offset to use for the Dragon events, when merging the file. Offset is in 
		/// milliseconds, and should be deducted from the Dragon time.</param>
		private void Merge(Pair<string, IEnumerable<string>> files, int offset)
		{
		    var directoryInfo = new FileInfo(files.First).Directory;
		    if (directoryInfo != null)
		    {
		        string topFolder = directoryInfo.FullName;
		        string firstIdfxFile = files.Second.First();
		        string resultFile = PathSanitizer.Uniquify(
		            Path.Combine(topFolder, 
		                StringUtils.LatinToAscii(SessionIDs[firstIdfxFile].GetParticipant()).Replace(' ','_') 
		                + "_DNSmerge" + "_Offset" + offset  + IDFX_EXT));

		        ReadDragonFile(files.First, topFolder);

		        // Start merging thread.
		        EventLogWriter logWriter = EventLogFactory.CreateFileEventLogWriter(resultFile, LogFormat.XML);
		        ThreadStart mergeThreadStart = delegate { MergeEvents(files,offset, logWriter); };
		        MergingThread = new Thread(mergeThreadStart);
		    }
		    MergingThread.Start();

			// wait until the merging thread is finished for this merge entry.
			MergingThread.Join();
		}

        // Extracts dns file at "filepath", stores audio files in "folder"/wav
        private void ReadDragonFile(string filePath, string folder)
        {
            WaveDir = Path.Combine(folder, "wav");
            Directory.CreateDirectory(WaveDir);
            DnsData = new DragonNaturallySpeakingData(filePath, WaveDir);
        }

	    /// <summary>
	    /// Merge the idfx and Dragon file. This method does the actual horse work of the merging.
	    /// </summary>
	    /// <param name="files">The Dragon and idfx files to merge. The first entry of the pair is the 
	    /// Dragon pair, the second entry is the enumeration of idfx files, sorted on start date.</param>
	    /// <param name="offset">The offset to use for the Dragon events, when merging the file. Offset is in 
	    /// milliseconds, and should be deducted from the Dragon time.</param>
	    /// <param name="logWriter"></param>
	    private void MergeEvents(Pair<string, IEnumerable<string>> files, int offset, EventLogWriter logWriter)
		{
            DragonNSPart currentPart = DnsData.Next();
			int idCounter = 0;

            double castedOffset = Math.Abs(offset);
            int offSetSign = (offset > 0) ? 1 : -1;

		    bool first = true;

		    ulong timeBase = 0;
            ulong shift = 0;

			foreach (string idfxFile in files.Second)
			{
                var idfxStart = SessionIDs[idfxFile].GetCreationTimestamp();
                var idfxStartOffset = SessionIDs[idfxFile].GetRelativeCreationTime();
				ulong iplTime = 0;
                timeBase = idfxStart - idfxStartOffset;
                if (first)
                {
                    if ((currentPart.TotalStartTime + (offSetSign * castedOffset)) < idfxStart)
                    {
                        first = false;
                        shift = idfxStart - (ulong)(currentPart.TotalStartTime + (offSetSign * castedOffset));
                    }
                    SessionIDs[idfxFile].SetCreationDate(DateTimeUtils.FromTimestamp(currentPart.TotalStartTime));
                    SessionIDs[idfxFile].SetRelativeCreationTime(idfxStartOffset - shift);
                    // Start log by writing the first files session identification.
                    logWriter.Start(SessionIDs[idfxFile], true);
                }
                
				foreach (Event iplEvent in Events[idfxFile])
				{
                    // Ipl time. -> In case the current event does not have timing info, the previous timing info is used instead.
                    TimedEventPart iplTimeInfo = Event.GetFirstEventPart<TimedEventPart>(iplEvent);
                    if (iplTimeInfo != null)
                    {
                        iplTime = timeBase + iplTimeInfo.StartTime;
                        if (shift > 0)
                        {
                            //iplTimeInfo.Shift(shift);
                        }
                    }

                    if (currentPart == null)
                    {
                        WriteNonStatisticIplEvent(logWriter, iplEvent);
                        idCounter++;
                        ReportProgress(this, new ProgressEventArgs(
                            "Wrote inputlog event ID [original/new]: " +
                            iplEvent.Labels[LBL_OLDID] + "/" + idCounter + ".",
                            ProgressEventArgs.ProgressCode.STEP_COMPLETED)
                        );
                        continue;
                    }

                    double dnsTimeInMs = currentPart.TotalStartTime + (offSetSign * castedOffset);
                    double dnsEndTimeInMs = currentPart.TotalEndTime + (offSetSign * castedOffset);
                    currentPart.AddTiming((ulong)dnsTimeInMs - timeBase, (ulong)dnsEndTimeInMs - timeBase);
                    var dnsBeforeIplEvent = (dnsTimeInMs < iplTime);

					while (dnsBeforeIplEvent && currentPart != null)
					{
                        Event newEvent = CreateNewIplEvent(currentPart);
                        logWriter.Write(newEvent);
                        idCounter++;
                        currentPart = DnsData.Next();
                        if (currentPart != null)
                        {
                            dnsTimeInMs = currentPart.TotalStartTime + (offSetSign * castedOffset);
                            dnsEndTimeInMs = currentPart.TotalEndTime + (offSetSign * castedOffset);
                            currentPart.AddTiming((ulong)dnsTimeInMs - timeBase, (ulong)dnsEndTimeInMs - timeBase);
                            dnsBeforeIplEvent = (dnsTimeInMs < iplTime);
                        }
					}
                    first = false;
					WriteNonStatisticIplEvent(logWriter, iplEvent);
					if (iplEvent.Type != "statistics")
					{
						idCounter += 1;
						ReportProgress(this, new ProgressEventArgs(
							"Wrote inputlog event ID [original/new]: " +
							iplEvent.Labels[LBL_OLDID] + "/" + idCounter + ".",
							ProgressEventArgs.ProgressCode.STEP_COMPLETED)
						);
					}
					else
					{
						ReportProgress(this, new ProgressEventArgs("Inputlog statistics event [skipped]", ProgressEventArgs.ProgressCode.STEP_COMPLETED));
					}
				}
			}

			// Write any lines off the dragon file that are left.
			while (currentPart != null)
			{
                double dnsTimeInMs = currentPart.TotalStartTime + (offSetSign * castedOffset);
                double dnsEndTimeInMs = currentPart.TotalEndTime + (offSetSign * castedOffset);
                currentPart.AddTiming((ulong)dnsTimeInMs - timeBase, (ulong)dnsEndTimeInMs - timeBase);
                Event newEvent = CreateNewIplEvent(currentPart);
                logWriter.Write(newEvent);
                idCounter++;
                currentPart = DnsData.Next();
			}

			logWriter.Stop();
		}

		private static void WriteNonStatisticIplEvent(EventLogWriter writer, Event @event)
		{
		    if (@event.Type == "statistics") return;
		    AlterOldIplEventID(@event);
		    writer.Write(@event);
		}

		private static void AlterOldIplEventID(Event @event)
		{
			string oldId;
			@event.Properties.TryGetValue("id", out oldId);

		    if (string.IsNullOrEmpty(oldId)) return;
		    @event.Labels.Add(LBL_OLDID, oldId);
		    @event.Properties.Remove("id");
		}

		private static Event CreateNewIplEvent(DragonNSPart part)
		{
		    Event newIplEvent = new Event {Type = XmlElements.Log.Events.Event.ATTRIBUTES.Type.VALUES.DRAGON_NS};
		    newIplEvent.Parts.Add(part);
			return newIplEvent;
		}
	}
}
