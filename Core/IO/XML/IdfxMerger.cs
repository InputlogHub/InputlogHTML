using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Output;
using InputLog.Core.Util;

namespace InputLog.Core.IO.Xml
{
    public class IdfxMerger
    {
        private readonly bool IncludeInitial;

        public IdfxMerger(bool includeInitial)
        {
            IncludeInitial = includeInitial;
        }

        public void Merge(string[] fileNames)
        {

            List<Pair<DateTime, List<Event>>> sortedEventLists = new List<Pair<DateTime, List<Event>>>();
            List<Pair<DateTime, SessionIdentification>> sortedSessionIds = new List<Pair<DateTime, SessionIdentification>>();
            foreach (string file in fileNames)
            {
                var eventLogReader = EventLogFactory.CreateFileEventLogReader(file, LogFormat.XML);
                SessionIdentification sessionID = eventLogReader.ReadSessionIdentification();
                List<Event> events = eventLogReader.ReadEvents();
                DateTime startTime = sessionID.GetCreationDate();
                sortedEventLists.Add(new Pair<DateTime, List<Event>>(startTime, events));
                sortedSessionIds.Add(new Pair<DateTime, SessionIdentification>(startTime, sessionID));
            }
            sortedEventLists.Sort((p1, p2) => p1.First.CompareTo(p2.First));
            sortedSessionIds.Sort((p1, p2) => p1.First.CompareTo(p2.First));

            SessionIdentification outSess = sortedSessionIds.First().Second;
            string baseFilename = Path.GetFileNameWithoutExtension(fileNames[0]);
            string dir = Path.GetDirectoryName(fileNames[0]);
            if (dir != null)
            {
                string outPath = PathSanitizer.Uniquify(Path.Combine(dir, baseFilename + "_MERGED.idfx"));
                EventLogWriter xmlWriter = EventLogFactory.CreateFileEventLogWriter(outPath, LogFormat.XML);
                xmlWriter.Start(outSess);

                string outPathStatistics = PathSanitizer.Uniquify(Path.Combine(dir, baseFilename + "_MERGED_statistics.idfx"));
                string outPathComments = PathSanitizer.Uniquify(Path.Combine(dir, baseFilename + "_MERGED_comments.idfx"));
                EventLogWriter xmlAllStatisticsWriter = EventLogFactory.CreateFileEventLogWriter(outPathStatistics, LogFormat.XML);
                EventLogWriter xmlAllCommentWriter = EventLogFactory.CreateFileEventLogWriter(outPathComments, LogFormat.XML);

                //writer to write away all statistics and author comments for each file to be merged
                xmlAllStatisticsWriter.Start(outSess);
                xmlAllCommentWriter.Start(outSess);
                int idOffset = 0;
                ulong timeOffset = 0;
                ulong endTime = 0;
                Statistics stats = null;
                AuthorComment comments = null;
                List<Statistics> allStatistics = new List<Statistics>();
                List<AuthorComment> allComments = new List<AuthorComment>();

                foreach (var descr in sortedEventLists)
                {
                    List<Event> events = descr.Second;
                    if (endTime > 0)
                    {
                        var beginTime = Event.GetFirstEventPart<TimedEventPart>(events.First()).EndTime;
                        timeOffset = beginTime - endTime - 1;
                    }
                    int ec = 0;
                    foreach (Event even in events)
                    {
                        Statistics thisStats = Event.GetFirstEventPart<Statistics>(even);
                        AuthorComment thisComment = Event.GetFirstEventPart<AuthorComment>(even);
                        if (thisStats != null)
                        {
                            allStatistics.Add(thisStats);                             
                            stats = thisStats;
                        }
                        else 
                        {
                            if (idOffset > 0)
                            {
                                even.ChangeId(even.GetId() + idOffset);
                            }
                            if (!IncludeInitial)
                            {
                                TimedEventPart time = Event.GetFirstEventPart<TimedEventPart>(even);
                                if (time != null)
                                {
                                    if (timeOffset > 0)
                                    {
                                        time.StartTime -= timeOffset;
                                        time.EndTime -= timeOffset;
                                    }
                                    endTime = time.EndTime;
                                }
                            }
                            xmlWriter.Write(even);
                            ec++;
                        }
                        if (thisComment != null)
                        {
                            allComments.Add(thisComment);
                            comments = thisComment;
                        }
                    }
                    idOffset += ec;
                }
                if (stats != null)
                {
                    Event even = new Event { Type = XmlElements.Log.Events.Event.ATTRIBUTES.Type.VALUES.STATISTICS };
                    even.Parts.Add(stats);
                    even.ChangeId(idOffset);
                    xmlWriter.Write(even);
                }

                if (comments != null)
                {
                    Event even = new Event { Type = XmlElements.Log.Events.Event.ATTRIBUTES.Type.VALUES.AUTHORCOMMENT };
                    even.Parts.Add(comments);
                    even.ChangeId(idOffset+1);
                    xmlWriter.Write(even);
                }

                int statisticsEventId = 0;
                int commentEventId = 0;

                if (allStatistics.Count > 1)
                {
                    foreach (Statistics statistics in allStatistics)
                    {
                        Event even = new Event { Type = XmlElements.Log.Events.Event.ATTRIBUTES.Type.VALUES.STATISTICS };
                        even.Parts.Add(statistics);
                        even.ChangeId(statisticsEventId++);
                        xmlAllStatisticsWriter.Write(even);
                    }
                }

                if (allComments.Count > 0)
                {
                    foreach (AuthorComment comment in allComments)
                    {
                        Event even = new Event { Type = XmlElements.Log.Events.Event.ATTRIBUTES.Type.VALUES.AUTHORCOMMENT };
                        even.Parts.Add(comment);
                        even.ChangeId(commentEventId++);
                        xmlAllCommentWriter.Write(even);
                    }
                }

                xmlWriter.Stop();
                xmlAllStatisticsWriter.Stop();
                xmlAllCommentWriter.Stop();
            }
        }
    }
}
