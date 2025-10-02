using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using InputLog.Core.Events;
using InputLog.Core.Events.EyeTracking;
using InputLog.Core.Events.EyeTracking.SubParts;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Input;
using InputLog.Core.IO.Basic.Output;
using InputLog.Core.IO.Tobii;
using InputLog.Core.IO.Xml;
using InputLog.Core.Util;
using InputLog.Core.Util.Matching;
using InputLog.Core.Util.Progress;

namespace InputLog.Core.Merging.Sources.ProcessTasks
{
    public class TobiiMergeTask: ProcessTask 
    {
        #region private_Fields
        /// <summary>
        /// An enumeration of all the match to merge, together with their
        /// specified offsets.
        /// </summary>
        private readonly IEnumerable<KeyValuePair<IMatch<string>, int>> MatchesToProcess;

        // Constant strings
        private const string IDFX_EXT = ".idfx";
        private const string TOBII_EXT = ".tsv";
        private const char SEPARATOR = '\t';
        private const string LBL_OLDID = "original_id";

        // TSV files chunks of #bytes size 
        private const int READ_CHUNK_SIZE = 5242880;
        private const int MAX_IN_MEMORY_CHUNKS_SIZE = 52428800;

        private const int LINE_READING_BYTE_CHUNK_SIZE = 524288;
        private const int MAX_LINES_IN_BUFFER = 10000;

        /// <summary>
        /// The session identification information for each idfx file.
        /// </summary>
        private readonly Dictionary<string, SessionIdentification> SessionIDs;

        /// <summary>
        /// The lists of events for each idfx file.
        /// </summary>
        private readonly Dictionary<string, List<Event>> Events;

        /// <summary>
        /// Map that maps Tobii tokens to their index in a tab-separated string.
        /// </summary>
        private Dictionary<string, int> IndexMap;

        /// <summary>
        /// Factory for creating eventLogWriters and readers.
        /// </summary>
        //private readonly EventLogFactory Factory;
        #endregion

        #region threaded_members
        
        private enum State 
        { 
            UNSTARTED,
            RUNNING,
            COMPLETED,
        };

        /// <summary>
        /// Contains the bytes read by the file reader, that 
        /// have not yet been processed by the parsing thread.
        /// </summary>
        private readonly Queue<byte> ByteBuffer;

        /// <summary>
        /// The ManualResetEvent for when new characters have been read.
        /// </summary>
        private readonly ManualResetEvent CharacterEvent;

        /// <summary>
        /// State of the character reader.
        /// </summary>
        private volatile State CharacterState;

        /// <summary>
        /// Queue of read lines that still have to be processed.
        /// </summary>
        private readonly Queue<string[]> LineBuffer;

        /// <summary>
        /// The ManualResetEvent for when new lines have been added.
        /// </summary>
        private readonly ManualResetEvent LineEvent;

        /// <summary>
        /// State of the line reader.
        /// </summary>
        private volatile State LineState;

        /// <summary>
        /// State of the merging of the files.
        /// </summary>
        private volatile State MergeState;

        /// <summary>
        /// Blocks until the header has been read.
        /// </summary>
        private readonly ManualResetEvent HeaderReadEvent;

        /// <summary>
        /// Thread that runs the reading of the tobii file.
        /// </summary>
        private Thread ReadThread;

        /// <summary>
        /// Thread that runs the parsing of the read information.
        /// </summary>
        private Thread ParsingThread;

        /// <summary>
        /// Thread that merges the parsed lines.
        /// </summary>
        private Thread MergingThread;

        /// <summary>
        /// Bool set to true if the processing is terminated from the outside.
        /// </summary>
        private volatile bool ProcessingTerminated;
        #endregion

        private int TobiiLineCounter;

        public TobiiMergeTask(IEnumerable<KeyValuePair<IMatch<string>, int>> matches)
        {
            NumberOfSteps = 1;
            MatchesToProcess = matches;

            SessionIDs = new Dictionary<string, SessionIdentification>();
            Events = new Dictionary<string, List<Event>>();

            CharacterEvent = new ManualResetEvent(false);
            LineEvent = new ManualResetEvent(false);
            HeaderReadEvent = new ManualResetEvent(false);
            CharacterState = State.UNSTARTED;
            LineState = State.UNSTARTED;
            MergeState = State.UNSTARTED;

            ByteBuffer = new Queue<byte>();
            LineBuffer = new Queue<string[]>();
        }

        /// <summary>
        /// Merge the matches.
        /// </summary>
        public override void Run()
        {
            var currentFile = "";
            try
            {
                ReportProgress(this, new ProgressEventArgs("Started merging", ProgressEventArgs.ProgressCode.STARTED));
                var mergeEntries = new List<Pair<string, IEnumerable<string>>>(MatchesToProcess.Count());
                var offsets = new List<int>(MatchesToProcess.Count());

                foreach (KeyValuePair<IMatch<string>, int> pair in MatchesToProcess)
                {
                    string tobii = pair.Key.SelectedItems().Single(path => Path.GetExtension(path) == TOBII_EXT);
                    var sortedIdfxs = new List<Pair<string, DateTime>>();
                    currentFile = tobii;

                    foreach (string idfx in pair.Key.SelectedItems().Where(path => Path.GetExtension(path) == IDFX_EXT))
                    {
                        try
                        {
                            EventLogReader reader = EventLogFactory.CreateFileEventLogReader(idfx, LogFormat.XML);
                            SessionIDs.Add(idfx, reader.ReadSessionIdentification());
                            Events.Add(idfx, reader.ReadEvents());

                            sortedIdfxs.Add(new Pair<string, DateTime>(idfx, SessionIDs[idfx].GetCreationDate()));
                        }
                        catch (Exception)
                        {
                            ReportProgress(this, new ProgressEventArgs(
                                "Merging failed, could not read idfx file: \"" + idfx + "\"", 
                                ProgressEventArgs.ProgressCode.FAILED));
                        }
                    }

                    sortedIdfxs = new List<Pair<string, DateTime>>(sortedIdfxs.OrderBy(entry => entry.Second));

                    // Foreach MergeEntry at index i, it's respective offset can be found in the Offsets array at index i.
                    mergeEntries.Add(new Pair<string, IEnumerable<string>>(tobii, sortedIdfxs.Select(entry => entry.First)));
                    offsets.Add(pair.Value);
                }

                NumberOfSteps = Events.Aggregate(0, (sum, pair) => sum + pair.Value.Count) + 1;
                ReportProgress(this, new ProgressEventArgs("Idfx files have been succesfully read.", 
                    ProgressEventArgs.ProgressCode.STEP_COMPLETED));

                for (int i = 0; i < mergeEntries.Count; i++)
                {
                    if (ProcessingTerminated)
                    {
                        ReportProgress(this, new ProgressEventArgs("Merging failed: processing terminated.", 
                            ProgressEventArgs.ProgressCode.FAILED));
                        return;
                    }

                    Pair<string, IEnumerable<string>> entry = mergeEntries[i];
                    int offset = offsets[i];

                    Merge(entry, offset);
                }
                ReportProgress(this, new ProgressEventArgs("", ProgressEventArgs.ProgressCode.DONE));
            }
            catch (ThreadAbortException)
            {
                ReportProgress(this, new ProgressEventArgs("Merging failed: thread aborted.", 
                    ProgressEventArgs.ProgressCode.FAILED));
                ProcessingTerminated = true;
            }
            catch (Exception e)
            {
                ReportProgress(this, new ProgressEventArgs("Merging failed when merging file: \"" + currentFile + "\".", 
                    ProgressEventArgs.ProgressCode.FAILED));
                MessageLogger.CatchException(this, e, Severity.ERROR);
            }

        }

        /// <summary>
        /// Merge the idfx and tobii file.
        /// </summary>
        /// <param name="files">The tobii and idfx files to merge. The first entry of the pair is the 
        /// tobii pair, the second entry is the enumeration of idfx files, sorted on start date.</param>
        /// <param name="offset">The offset to use for the tobii events, when merging the file. Offset is in 
        /// milliseconds, and should be deducted from the tobii time.</param>
        private void Merge(Pair<string, IEnumerable<string>> files, int offset)
        {
            try
            {
                CharacterState = State.UNSTARTED;
                LineState = State.UNSTARTED;
                MergeState = State.UNSTARTED;

                var directoryInfo = new FileInfo(files.First).Directory;
                if (directoryInfo != null)
                {
                    string topFolder = directoryInfo.FullName;
                    string firstIdfxFile = files.Second.First();
                    string participant = StringUtils.LatinToAscii(SessionIDs[firstIdfxFile].GetParticipant()).Replace(' ', '_');
                    string resultFile = PathSanitizer.Uniquify(
                        Path.Combine(topFolder, participant + "_ETmerge" + "_Offset" + offset + IDFX_EXT ));

                    CharacterEvent.Reset();
                    HeaderReadEvent.Reset();
                    LineEvent.Reset();

                    // Creating reader thread.
                    ThreadStart readerThreadStart = () => ReadTobiiFile(files.First, files.Second.First());
                    ReadThread = new Thread(readerThreadStart);

                    // Creating splitting thread
                    ParsingThread = new Thread(ParseLines);

                    // Creating merging thread.
                    EventLogWriter logWriter = EventLogFactory.CreateFileEventLogWriter(resultFile, LogFormat.XML);
                    ThreadStart mergeThreadStart = () => MergeEvents(files, offset, logWriter);
                    MergingThread = new Thread(mergeThreadStart);
                }
            }
            catch (ThreadAbortException)
            {
                ProcessingTerminated = true;
                Thread.ResetAbort(); // exit cleanly instead
                return;
            }

            try
            {
                ReadThread.Start();
                ParsingThread.Start();
                MergingThread.Start();

                // Wait for all tasks to finish.
                // NOTE: We do this without the MergingThread.Join() statement so that this thread is not blocked and
                // is able to react to it being terminated from the outside.
                while (CharacterState != State.COMPLETED || LineState != State.COMPLETED || MergeState != State.COMPLETED)
                {
                    Thread.Sleep(250);
                }
            }
            catch (ThreadAbortException)
            {
                ProcessingTerminated = true;

                if (ReadThread.IsAlive)
                {
                    ReadThread.Abort();
                }
                if (ParsingThread.IsAlive)
                {
                    ParsingThread.Abort();
                }
                if (MergingThread.IsAlive)
                {
                    MergingThread.Abort();
                }
            }
        }

        #region file_reading

        /// <summary>
        /// Reads the tobii file and pushes the read data onto a queue of characters
        /// </summary>
        /// <param name="filePath">File to be read.</param>
        /// <param name="firstIdfxFile"></param>
        private void ReadTobiiFile(string filePath, string firstIdfxFile)
        {
            try
            {
                CharacterState = State.RUNNING;

                string[] tobiiLine = null;
                using (var reader = new StreamReader(File.OpenRead(filePath)))
                {
                    string headerLine = reader.ReadLine();

                    // Building the index map.
                    IndexMap = new Dictionary<string, int>(80);
                    if (headerLine != null)
                    {
                        string[] headerParts = headerLine.Split(SEPARATOR);

                        // Identifies and renames duplicate AOI's
                        headerParts = TobiiUtilities.RenameDuplicateAOIs(headerParts);

                        int counter = 0;
                        foreach (string headerToken in headerParts)
                        {
                            IndexMap.Add(headerToken, counter++);
                        }
                    }
                    HeaderReadEvent.Set();

                    string line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        tobiiLine = line.Split(SEPARATOR);
                    }
                }

                if (tobiiLine != null)
                {
                    // User the headerTobiiLine to specify extra sessionId information.
                    Dictionary<string, string> tobiiSession = new Dictionary<string, string>();
                    tobiiSession.Add(TAGS.GN_ExportDate, 
                        tobiiLine[IndexMap[TAGS.GN_ExportDate]]);
                    tobiiSession.Add(TAGS.GN_FixationFilter, 
                        tobiiLine[IndexMap[TAGS.GN_FixationFilter]]);
                    tobiiSession.Add(TAGS.GN_ParticipantName, 
                        tobiiLine[IndexMap[TAGS.GN_ParticipantName]]);
                    tobiiSession.Add(TAGS.GN_RecordingDate, 
                        tobiiLine[IndexMap[TAGS.GN_RecordingDate]]);
                    tobiiSession.Add(TAGS.GN_RecordingDuration, 
                        tobiiLine[IndexMap[TAGS.GN_RecordingDuration]]);
                    tobiiSession.Add(TAGS.GN_RecordingName, 
                        tobiiLine[IndexMap[TAGS.GN_RecordingName]]);
                    tobiiSession.Add(TAGS.GN_RecordingResolution, 
                        tobiiLine[IndexMap[TAGS.GN_RecordingResolution]]);
                    tobiiSession.Add(TAGS.GN_StudioProjectName, 
                        tobiiLine[IndexMap[TAGS.GN_StudioProjectName]]);
                    tobiiSession.Add(TAGS.GN_StudioTestName, 
                        tobiiLine[IndexMap[TAGS.GN_StudioTestName]]);
                    tobiiSession.Add(TAGS.GN_StudioVersionRec, 
                        tobiiLine[IndexMap[TAGS.GN_StudioVersionRec]]);

                    SessionIDs[firstIdfxFile].AddSessionInfo(tobiiSession);
                }

                using (FileStream reader = File.OpenRead(filePath))
                {
                    // Read from the file and buffer the read data
                    var buffer = new byte[READ_CHUNK_SIZE];
                    int charactersRead;
                    while ((charactersRead = reader.Read(buffer, 0, READ_CHUNK_SIZE)) != 0 && !ProcessingTerminated)
                    {
                        lock (ByteBuffer)
                        {
                            for (int i = 0; i < charactersRead; i++)
                            {
                                ByteBuffer.Enqueue(buffer[i]);
                            }

                        }
                        CharacterEvent.Set();

                        bool waitAfterCountCheck = true;
                        while (waitAfterCountCheck)
                        {
                            lock (ByteBuffer)
                            {
                                waitAfterCountCheck = ByteBuffer.Count > MAX_IN_MEMORY_CHUNKS_SIZE;
                            }
                            if (waitAfterCountCheck)
                            {
                                Thread.Sleep(1250);
                            }
                        }
                    }
                    CharacterState = State.COMPLETED;
                    CharacterEvent.Set();
                }
            }
            catch (ThreadAbortException)
            {
                ProcessingTerminated = true;
            }
        }

        /// <summary>
        /// Reading from the character buffer and parsing the data into lines 
        /// that are subsequently split into string arrays based on their separator.
        /// </summary>
        private void ParseLines()
        {
            try
            {
                LineState = State.RUNNING;

                HeaderReadEvent.WaitOne();
                HeaderReadEvent.Set();
                var separators = new[] { SEPARATOR };
                var buffer = new byte[LINE_READING_BYTE_CHUNK_SIZE];
                string residualString = ""; // the portion of string, left from the previous parse.

                while ((CharacterState != State.COMPLETED || ByteBuffer.Count > 0) && !ProcessingTerminated)
                {
                    CharacterEvent.WaitOne();

                    // Reset buffer
                    Array.Clear(buffer, 0, buffer.Length);
                    int bytesRead = 0;

                    // Reading as much as we can from the charbuffer
                    lock (ByteBuffer)
                    {
                        while (ByteBuffer.Count > 0 && bytesRead < LINE_READING_BYTE_CHUNK_SIZE && !ProcessingTerminated)
                        {
                            buffer[bytesRead] = ByteBuffer.Dequeue();
                            bytesRead += 1;
                        }
                        if (ByteBuffer.Count == 0)
                        {
                            CharacterEvent.Reset();
                        }

                        if (ProcessingTerminated)
                        {
                            return;
                        }
                    }

                    // Reading lines from the characters we have taken from the charBuffer
                    using (var reader = new StreamReader(new MemoryStream(buffer, false)))
                    {
                        var sBuild = new StringBuilder(residualString);
                        residualString = "";

                        while (!reader.EndOfStream && !ProcessingTerminated)
                        {
                            while (!reader.EndOfStream && 
                                (
                                    sBuild.Length == 0 || 
                                    (sBuild[sBuild.Length - 1] != '\n' &&
                                    sBuild[sBuild.Length - 1] != '\r')))
                            {
                                sBuild.Append((char)reader.Read());
                            }

                            if (reader.EndOfStream)
                            {
                                residualString = sBuild.ToString();
                            }
                            else
                            {
                                if (reader.Peek() == '\n' || reader.Peek() == '\r')
                                {
                                    sBuild.Append((char)reader.Read());
                                }
                                string line = sBuild.ToString();
                                string[] parts = line.Split(separators);
                                lock (LineBuffer)
                                {
                                    LineBuffer.Enqueue(parts);
                                    LineEvent.Set();
                                }
                            }

                            sBuild.Clear();
                        }

                        if (ProcessingTerminated)
                        {
                            return;
                        }

                        bool waitAfterLineCountCheck = true;
                        while (waitAfterLineCountCheck)
                        {
                            lock (LineBuffer)
                            {
                                waitAfterLineCountCheck = LineBuffer.Count > MAX_LINES_IN_BUFFER;
                            }
                            if (waitAfterLineCountCheck)
                            {
                                Thread.Sleep(1250);
                            }
                        }
                    }
                }

                // Checking for any left over characters.
                var sb = new StringBuilder(residualString);
                lock (ByteBuffer)
                {
                    while (ByteBuffer.Count > 0)
                    {
                        sb.Append((char)ByteBuffer.Dequeue());
                    }
                }

                string lastLines = sb.ToString();
                sb.Clear();
                using (var reader = new StreamReader(ConvertStringToStream(lastLines)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lock (LineBuffer)
                        {
                            string[] parts = line.Split(separators);
                            if (parts.Count() >= IndexMap.Count)
                            {
                                LineBuffer.Enqueue(parts);
                                LineEvent.Set();
                            }
                        }
                    }
                }
                LineState = State.COMPLETED;
            }
            catch (ThreadAbortException)
            {
                ProcessingTerminated = true;
            }
        }

        /// <summary>
        /// Converts a string to a stream, that can be read.
        /// </summary>
        /// <param name="s">String to convert to the stream.</param>
        /// <returns>Stream, containing the content of string s</returns>
        private Stream ConvertStringToStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        #endregion

        /// <summary>
        /// Merging the idfx and tobii file. This method handles the heavy duty work of the merging.
        /// </summary>
        /// <param name="files">The tobii and idfx files to merge. The first entry of the pair is the 
        /// tobii pair, the second entry is the enumeration of idfx files, sorted on start date.</param>
        /// <param name="offset">The offset to use for the tobii events, when merging the file. Offset is in 
        /// milliseconds, and should be deducted from the tobii time.</param>
        /// <param name="logWriter"></param>
        private void MergeEvents(Pair<string, IEnumerable<string>> files, int offset, EventLogWriter logWriter)
        {
            try
            {
                MergeState = State.RUNNING;
                EyetrackPart previousEyetrack = null;
                string[] tobiiLine = null;
                AOIPart.ResetAOIs();

                TobiiLineCounter = 0;
                int idCounter = 0;
                bool logStarted = false;

                ulong castedOffset = (offset > 0) ? (ulong)offset : (ulong)(-offset);

                bool tobiiEventCompleted = false;
                bool tobiiBeforeIplEvent = true;

                // Reading the first 'header' line and ignore it.
                ReadTobiiLine(ref tobiiLine);
                tobiiLine = null;

                ulong idfxStart = 0;
                ulong idfxStartOffset = 0;

                foreach (string idfxFile in files.Second)
                {
                    idfxStart = (ulong)new TimeSpan(SessionIDs[idfxFile].GetCreationDate().Ticks).TotalMilliseconds;
                    idfxStartOffset = SessionIDs[idfxFile].GetRelativeCreationTime();
                    ulong iplTime = 0;

                    foreach (Event iplEvent in Events[idfxFile])
                    {
                        if (ProcessingTerminated)
                        {
                            return;
                        }

                        // Reading the tobii string and getting the start time.
                        // If the tobii start time is before the iplEvent we create the tobii event in full 
                        // and then print it.
                        // If the tobii start time is after the iplEvent we first print the iplEvent and 
                        // go to the next cycle of the loop. After each iplEvent that is written we fire 'STEP_COMPLETED'
                        //
                        bool wroteIplEvent = false;

                        // Ipl time. -> In case the current event does not have timing info, 
                        // the previous timing info is used instead.
                        var iplTimeInfo = Event.GetFirstEventPart<TimedEventPart>(iplEvent);
                        if (iplTimeInfo != null)
                        {
                            iplTime = idfxStart - idfxStartOffset + iplTimeInfo.StartTime;
                        }

                        // 1. There are still lines, or there are still lines waiting to be read. AND (
                        // 2. The tobii event has not been completed, but it has already been started. OR
                        // 3. The tobii event happens before the inputlog event (thus the inputlog event 
                        // will not be written until the tobii event is completed) OR
                        // 4. The tobii event does not happen before the iplEvent and, the iplEvent has not been written yet,
                        // we need to read a new one.
                        // 5. If processing has been terminated, just stop.
                        // )
                        while ((LinesRemaining() || LineState != State.COMPLETED) &&
                                (
                                    (!tobiiEventCompleted && previousEyetrack != null) 
                                    || tobiiBeforeIplEvent 
                                    || !wroteIplEvent
                                )
                                && !ProcessingTerminated
                            )
                        {
                            ReadTobiiLine(ref tobiiLine);

                            if (tobiiLine == null)
                            {
                                if (LineState != State.COMPLETED)
                                {
                                    throw new MergeException("Data line null while merge was not complete.");
                                }
                                else
                                {
                                    if (!logStarted)
                                    {
                                        throw new MergeException("Log has not been started yet. " +
                                                                 "Are you sure there is data in the eyetrack file?");
                                    }

                                    // No more tobii lines, but apparenlty more IPL events, write them.
                                    WriteNonStatisticIplEvent(logWriter, iplEvent);
                                    idCounter += 1;
                                    ReportProgress(this, new ProgressEventArgs(
                                        "Wrote inputlog event ID [original/new]: " +
                                        iplEvent.Labels[LBL_OLDID] + "/" + idCounter.ToString() + ".",
                                        ProgressEventArgs.ProgressCode.STEP_COMPLETED)
                                    );
                                    // We don't have to check time, tobiiBeforeIpl, merging of tobii lines etc. 
                                    // Just simply exit this while loop when the inputlog event has been written.
                                    break;
                                }
                            }

                            // Calculates the time of that one tobii line.
                            var tmpEyetrack = new EyetrackPart();
                            tmpEyetrack.Initialize(tobiiLine, IndexMap);
                            var tobiiTime = tmpEyetrack.GetFirstSubPart<TimePart>();
                            tobiiTime.IdfxStartOffsetMs = idfxStartOffset;
                            tobiiTime.IdfxStartRecordingMs = idfxStart;

                            // Is this tobii event started before the iplEvent?
                            ulong tobiiTimeInMs;
                            if (offset > 0)
                            {
                                tobiiTimeInMs = tobiiTime.InterpretData().FullTimeInMs - castedOffset;
                                tobiiBeforeIplEvent = (tobiiTimeInMs < iplTime);
                            }
                            else
                            {
                                tobiiTimeInMs = tobiiTime.InterpretData().FullTimeInMs + castedOffset;
                                tobiiBeforeIplEvent = (tobiiTimeInMs < iplTime);
                            }

                            // Starts the log, but calculates first if the relativeCreationDate has to be altered
                            // in the session information.
                            if (!logStarted)
                            {
                                var newStartOffset = (ulong)IPLTimeConverter.Initialize(
                                    idfxStart,
                                    idfxStartOffset,
                                    tobiiLine[IndexMap[TAGS.GN_RecordingDate]],
                                    tobiiLine[IndexMap[TAGS.TS_Local]],
                                    tobiiLine[IndexMap[TAGS.TS_Recording]]
                                );

                                if (SessionIDs[idfxFile].HasRelativeCreationTime())
                                {
                                    ulong relativeCreationTime = SessionIDs[idfxFile].GetRelativeCreationTime();

                                    if (relativeCreationTime > newStartOffset)
                                    {
                                        SessionIDs[idfxFile].SetRelativeCreationTime(newStartOffset);
                                    }
                                }

                                // Starting the log by writing the first files session identification.
                                logWriter.Start(SessionIDs[files.Second.First()], false);
                                logStarted = true;
                            }


                            // Are we already reading a tobii event that this line is part off?
                            if (previousEyetrack != null)
                            {
                                if (previousEyetrack.CanMerge(tmpEyetrack))
                                {
                                    // Yes, previous event was already being read, this line belongs to it, merge.
                                    previousEyetrack.Merge(tmpEyetrack);
                                    tobiiLine = null;
                                }
                                else
                                {
                                    // No, previous event was being read, but this is a new event line.
                                    // Save the event.
                                    previousEyetrack.CloseOfEvent(tobiiLine, IndexMap);
                                    var newEvent = CreateNewIplEvent(previousEyetrack);
                                    logWriter.Write(newEvent);
                                    idCounter += 1;

                                    if (TobiiLineCounter % 100 == 0)
                                    {
                                        ReportProgress(this, new ProgressEventArgs(files.First + ": parsed line #" +
                                            TobiiLineCounter));
                                    }

                                    if (tobiiBeforeIplEvent)
                                    {
                                        // The new tobii event is before the ipl event, so just start reading it.
                                        previousEyetrack = new EyetrackPart();
                                        previousEyetrack.Initialize(tobiiLine, IndexMap);
                                        var tp = previousEyetrack.GetFirstSubPart<TimePart>();
                                        tp.IdfxStartOffsetMs = idfxStartOffset;
                                        tp.IdfxStartRecordingMs = idfxStart;
                                        tobiiLine = null;
                                    }
                                    else
                                    {
                                        tobiiEventCompleted = true;
                                        // This saves us the trouble of checking the CanMerge again for the next line.
                                        previousEyetrack = null; 
                                    }
                                }
                            }
                            else
                            {
                                // We aren't reading any tobii events yet. 
                                if (tobiiBeforeIplEvent)
                                {
                                    // Tobii event is before the ipl event, we just start reading it.
                                    previousEyetrack = tmpEyetrack;
                                    tobiiEventCompleted = false;
                                    tobiiLine = null;
                                }
                                else
                                {
                                    // The ipl event is first. Write it. 
                                    WriteNonStatisticIplEvent(logWriter, iplEvent);
                                    wroteIplEvent = true;
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
                                        ReportProgress(this, new ProgressEventArgs("Inputlog statistics event [skipped]", 
                                            ProgressEventArgs.ProgressCode.STEP_COMPLETED));
                                    }
                                }
                            }
                        }

                        if (ProcessingTerminated)
                        {
                            return;
                        }


                        //In the case it was the tobii lines that were finished first, write the remaining inputlog events.
                        if (!LinesRemaining() && LineState == State.COMPLETED && !ProcessingTerminated)
                        {
                            // The ipl event is first. Write it. 
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
                                ReportProgress(this, new ProgressEventArgs("Inputlog statistics event [skipped]", 
                                    ProgressEventArgs.ProgressCode.STEP_COMPLETED));
                            }
                        }
                    }
                }

                if (ProcessingTerminated)
                {
                    return;
                }

                // Writes any lines off the tobii file that are left.
                if (LinesRemaining() || tobiiLine != null)
                {
                    WriteRemainingTobiiLines(previousEyetrack, tobiiLine, logWriter, idfxStart, idfxStartOffset);
                }

                MergeState = State.COMPLETED;

            }
            catch (ThreadAbortException)
            {
                ProcessingTerminated = true;
            }
            finally
            {
                logWriter.Stop();
            }
        }

        private void WriteRemainingTobiiLines(EyetrackPart previousEyetrack, string[] tobiiLine, EventLogWriter logWriter,
            ulong idfxStart, ulong idfxStartOffset)
        {
            if (previousEyetrack == null)
            {
                previousEyetrack = new EyetrackPart();
                previousEyetrack.Initialize(tobiiLine, IndexMap);
                tobiiLine = null;
            }

            while (LinesRemaining() || LineState != State.COMPLETED)
            {
                ReadTobiiLine(ref tobiiLine);
                var tmp = new EyetrackPart();
                tmp.Initialize(tobiiLine, IndexMap);
                var tobiiTime = tmp.GetFirstSubPart<TimePart>();
                tobiiTime.IdfxStartRecordingMs = idfxStart;
                tobiiTime.IdfxStartOffsetMs = idfxStartOffset;

                if (previousEyetrack.CanMerge(tmp))
                {
                    previousEyetrack.Merge(tmp);
                    tobiiLine = null;
                }
                else
                {
                    // Event is finished, write it.
                    var newEvent = CreateNewIplEvent(previousEyetrack);
                    logWriter.Write(newEvent);
                    previousEyetrack = tmp;
                }
            }

            if (tobiiLine == null)
            {
                var newEvent = CreateNewIplEvent(previousEyetrack);
                logWriter.Write(newEvent);
            }
        }

        private bool LinesRemaining()
        {
            bool linesRemaining;
            lock (LineBuffer)
            {
                linesRemaining = LineBuffer.Count > 0;
            }
            return linesRemaining;
        }

        private void ReadTobiiLine(ref string[] tobiiLine)
        {
            // Reading one tobii line
            if (tobiiLine != null) return;
            LineEvent.WaitOne();
            lock (LineBuffer)
            {
                tobiiLine = LineBuffer.Dequeue();
                TobiiLineCounter += 1;
                if (LineBuffer.Count < 1)
                {
                    LineEvent.Reset();
                }
            }
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

        private static Event CreateNewIplEvent(IEventPart previousEyetrack)
        {
            var newIplEvent = new Event {Type = XmlElements.Log.Events.Event.ATTRIBUTES.Type.VALUES.EYETRACKER};
            newIplEvent.Parts.Add(previousEyetrack);
            return newIplEvent;
        }
    }
}
