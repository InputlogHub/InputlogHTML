using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Output;
using InputLog.Core.Util;

namespace InputLog.Core.IO.Xml
{
    /// <summary>
    /// Class for splitting an IDFX file into parts when a key is pressed
    /// For example a file "Test.idfx" containing "... ENTER ... ENTER ..." may be split into 3 separate IDFXs
    /// These will then be written out as "Test_Segment_1.idfx", "Test_Segment_2.idfx", "Test_Segment_3.idfx"
    /// Relative start times etc are updated accordingly
    /// </summary>
    public class IdfxSegmenter
    {
        private readonly string SplitKey;
        private readonly bool IncludeInitialPause;

        /// <summary>
        /// onstructor, takes the key to split on,
        /// and whether the pause before first keypress needs to be included in each segment
        /// </summary>
        /// <param name="splitKey">the key to split on</param>
        /// <param name="includeInitialPause">include pause before first keypress</param>
        public IdfxSegmenter(string splitKey, bool includeInitialPause)
        {
            SplitKey = splitKey;
            IncludeInitialPause = includeInitialPause;
        }

        /// <summary>
        /// Segments a single file, returns the number of segments found
        /// </summary>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public int Segment(string outputPath)
        {
            // Read event list
            var eventLogReader = EventLogFactory.CreateFileEventLogReader(outputPath, LogFormat.XML);
            SessionIdentification sessionID = eventLogReader.ReadSessionIdentification();
            List<Event> events = eventLogReader.ReadEvents();
            List<List<Event>> segments = new List<List<Event>> {new List<Event>()};

            // Add events to current segment until splitkey is encountered
            int si = 0;
            for (int i = 0; i < events.Count - 1; i++)
            {
                Event e = events[i];
                segments[si].Add(e);
                if (e.GetKeypressKey().Equals(SplitKey))
                {
                    si++;
                    segments.Add(new List<Event>());
                }
            }
            SegmentsToFile(segments, outputPath, sessionID);
            return segments.Count;
        }
 
        /// <summary>
        /// Segment each file in an array of filenames, return total number of segments
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        public int Segment(string[] fileNames)
        {
            return fileNames.Sum(file => Segment(file));
        }

        /// <summary>
        ///  Write a list of segments to separate files named "*_segment_x.idfx"
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="outputPath"></param>
        /// <param name="sessionID"></param>
        private void SegmentsToFile(List<List<Event>> segments, string outputPath, SessionIdentification sessionID)
        {
            string baseFilename = Path.GetFileNameWithoutExtension(outputPath);
            string dir = Path.GetDirectoryName(outputPath);
            var relStartTime = sessionID.GetRelativeCreationTime();
            for (int sj = 0; sj < segments.Count; sj++)
            {
                // Find relative start time and start ID of segment
                List<Event> el = segments[sj];
                if (!IncludeInitialPause)
                {
                    try
                    {
                        relStartTime = Event.SkipToFirstKeypress(el).Second;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
                int startID = int.Parse(el.First().Properties["id"]);
                sessionID.SetRelativeCreationTime(relStartTime);

                // Write events to file
                if (dir != null)
                {
                    string outPath = PathSanitizer.Uniquify(Path.Combine(dir, string.Format("{0}_Segment_{1}.idfx", baseFilename, sj+1)));
                    EventLogWriter xmlWriter = EventLogFactory.CreateFileEventLogWriter(outPath, LogFormat.XML);
                    xmlWriter.Start(sessionID);

                    foreach (Event e in el)
                    {
                        int id = int.Parse(e.Properties["id"]);
                        e.Properties["id"] = (id - startID).ToString();
                        xmlWriter.Write(e);
                    }
                    xmlWriter.Stop();
                }

                // Determine start time of next segment
                relStartTime = Event.GetFirstEventPart<TimedEventPart>(el.Last()).EndTime;
            }
        }

    }
}
