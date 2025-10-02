using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events;
using InputLog.Core.Analyses.General;
using InputLog.Core.IO;
using InputLog.Core.Analyses;

namespace InputLog.Core.Mining.Process
{
    public class WordCaseSplitter: ICaseSplitter
    {
        private Dictionary<string, List<Event>> Cases;
        private List<Event> Events;
        private int LastAddedEventID;
        private string CurrentCase;
        private int WordCounter;

        public Dictionary<string, List<Event>> Split(List<Event> events)
        {
            Cases = new Dictionary<string, List<Event>>();
            var pauseLocationMarker = new PauseLocationMarker();
            Events = events;
            LastAddedEventID = -1;
            // Adding event handlers to the PauseLocationMarker.
            pauseLocationMarker.WordEventHandler += StartCase;
            pauseLocationMarker.SentenceEventHandler += StartCase;
            pauseLocationMarker.ParagraphEventHandler += StartCase;
            pauseLocationMarker.WordCharEventHandler += AddToCase;
            //pauseLocationMarker.SentenceCharEventHandler += EndChar;
            //pauseLocationMarker.ParagraphCharEventHandler += EndChar;
            pauseLocationMarker.Start(events);
            return Cases;
        }

        private void AddToCase(object sender, PauseLocationEventArgs eventArgs)
        {
            for (int i = LastAddedEventID + 1; i < Events.Count; i++)
            {
                LastAddedEventID++;
                Cases[CurrentCase].Add(Events[i]);
                if (Events[i].Equals(eventArgs.Event))
                {
                    break;
                }
            }
        }

        private void StartCase(object sender, PauseLocationEventArgs eventArgs)
        {
            WordCounter++;
            CurrentCase = "Word_" + WordCounter;
            Cases.Add(CurrentCase, new List<Event>());
            LastAddedEventID = eventArgs.Event.GetId() - 1;
        }
    }
}
