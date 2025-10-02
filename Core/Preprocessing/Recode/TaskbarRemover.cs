using System.Collections.Generic;
using System.Linq;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.IO;

namespace InputLog.Core.Preprocessing.Recode
{
    public class TaskbarRemover : Preprocessor
    {
        /// <summary>
        /// Removing 'TASKBAR' and 'Switching between Tasks'.
        /// </summary>
        /// <param name="inputEvents"></param>
        /// <param name="sessionId"></param>
        /// <returns>A list of events where empty TASKBAR and 'Switch between Tasks' focus changes have been  
        /// replaced with the source following them.</returns>
        public override List<Event> Process(List<Event> inputEvents, SessionIdentification sessionId)
        {
            var recodedEventList = new List<Event>();
            int tabId = -1;
            bool tabPreceding = false;
            var focusList = new List<KeyValuePair<int, string>>();
            string[] keys = { "VK_TAB", "VK_LMENU", "VK_RMENU", "VK_MENU" };
            // To find more Microsoft translations, see: https://www.microsoft.com/en-us/language/StyleGuides?rtc=2
            // and look up 'switch' in the localization style guide of the selected language.
            // Consider making an enum of this list ;-)
            string[] switches = { "schakelen", "switching", "basculer", "wechseln" };


            // First step collect a list of all real and bogus focus events with their id.
            foreach (var inputEvent in inputEvents)
            {
                var currentId = inputEvent.GetId();
                // The 'VK_TAB' key and similar precede a 'switch between tasks'. It is needed
                // because the phrase 'switch between task' is translated in all local settings.
                if (keys.Any(inputEvent.GetKeypressKey().Contains))
                {
                    tabPreceding = true;
                    tabId = currentId;
                }
                if (inputEvent.Type.Equals(EventType.MOUSE))
                {
                    if (tabPreceding)
                    {
                        tabId = currentId;
                    }
                }
                else if(inputEvent.Type.Equals(EventType.FOCUS))
                {
                    var title = inputEvent.GetWindowTitle(inputEvent).ToLowerInvariant();
                    if (title.Equals("taskbar"))
                    {
                        if (tabPreceding)
                        {
                            tabId = currentId;
                        }
                        title = "REMOVE";
                    }
                    else if (tabPreceding && currentId - tabId == 1)
                    {
                        // The 'VK_TAB' always comes before a 'switch between tasks', but sometimes
                        // it also pops up before regular focus changes. To minimize the risk of 
                        // removing a bonafide source, we check if we have it in our list.
                        // Not waterproof at all, of course.
                        if (!focusList.Exists(element => element.Value.Equals(title) && !switches.Any(element.Value.Contains)))
                        {
                            title = "REMOVE";
                        }
                        tabPreceding = false;
                    }
                    KeyValuePair<int, string> focusPair = new KeyValuePair<int, string>(currentId, title);
                    focusList.Add(focusPair);
                }
            }

            // Second step to replace all focus events that are either 'TASKBAR' or 'Switch Between Tasks'
            // with the first focus having real content.
            var restart = 0;
            var forwaredFocus = -1;

            foreach (var inputEvent in inputEvents)
            {
                var newEvent = inputEvent;
                if (inputEvent.Type.Equals(EventType.FOCUS))
                {
                    var currentId = inputEvent.GetId();
                    for (var i = restart; i < focusList.Count; i++)
                    {
                        var currentKey = focusList[i].Key;
                        if (focusList[i].Key.Equals(currentId) && focusList[i].Value.Equals("REMOVE"))
                        {
                            for (var j = i; j < focusList.Count; j++)
                            {
                                if (focusList[j].Value.Equals("REMOVE")) continue;
                                var focusEventParts = newEvent.Parts.OfType<FocusChange>();
                                var focusChanges = focusEventParts as FocusChange[] ?? focusEventParts.ToArray();
                                focusChanges.First().WindowTitle = focusList[j].Value; 
                                // Remember the id of the focus that replaced the "REMOVE".
                                // It's taken out when we encounter it later.
                                forwaredFocus = focusList[j].Key;
                                break;
                            }
                        }
                        if (currentKey > currentId)
                        {
                            restart = i;
                            break;
                        }
                    }
                }
                recodedEventList.Add(newEvent);

                if (newEvent.GetId().Equals(forwaredFocus))
                {
                    recodedEventList.Remove(newEvent);
                }
            }           
            return recodedEventList;
        }  
    }
}