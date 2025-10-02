using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InputLog.Core.Analyses;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;
using InputLog.Core.Events.WordLog;
using InputLog.Core.IO;
using InputLog.Core.Util;
using InputLog.Core.Util.KeyConversion;

namespace InputLog.Core.Preprocessing.Recode
{
    /// <summary>
    /// Rewrite of an idfx file: positions and document length is added in an 'wordlog' 
    /// event part and added to an exisiting idfx that lacked this information.
    /// Kept as an example of the posibility to recode an idfx-file.
    /// </summary>
    public class RecoderIdfx : Preprocessor
    {
        /// <summary>
        /// Reconstruction of an idfx-file. 
        /// All event-types are exposed here to allow changing or deleting, 
        /// according to the recoding needs. 
        /// </summary>
        /// <param name="inputEvents"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override List<Event> Process(List<Event> inputEvents, SessionIdentification sessionId)

        {
            var positionCounter = 0;
            var docLength = 0;
            var recodedEventList = new List<Event>();
            Event previousEvent = null;

            foreach (var inputEvent in inputEvents)
            {
                var newEvent = inputEvent;
                // WINLOG is 'KeyPress' with a capital 'P'
                IEnumerable<KeyPress> winLogs = newEvent.Parts.OfType<KeyPress>();
                // WORDLOG is 'Keypress' with a small 'p'
                IEnumerable<Keypress> wordLogs = newEvent.Parts.OfType<Keypress>();
                var wordLog = newEvent.Parts[0];

                switch (newEvent.Type)
                {
                    case EventType.KEYBOARD:
                        var winLogPart = winLogs.First();
                        if (winLogPart != null)
                        {
                            if (null != previousEvent && previousEvent.Type.Equals("KEYBOARD"))
                            {
                                var prevTime = previousEvent.Parts.OfType<KeyPress>().First().StartTime;

                                if (prevTime > winLogPart.StartTime)
                                {
                                    Debug.WriteLine("Previous startTime at id " + previousEvent.GetId()
                                        + " > current startTime " + winLogPart.StartTime);
                                }
                            }

                            if (winLogPart.Key == KeysEx.VK_BACK || Lexical.IsCtrlBackSpace(winLogPart))
                            {
                                if (positionCounter > 0 && docLength > 0) positionCounter--;
                                if (positionCounter > 0 && docLength > 0) docLength--;
                            }
                            else if (winLogPart.Key != KeysEx.VK_BACK || !Lexical.IsCtrlBackSpace(winLogPart))
                            {
                                if (!Lexical.IsCombinationKey(winLogPart)) positionCounter++;
                                if (!Lexical.IsCombinationKey(winLogPart)) docLength++;
                            }
                        }

                        var keypresses = wordLogs as Keypress[] ?? wordLogs.ToArray();

                        if (keypresses.IsNullOrEmpty())
                        {
                            var wordLogPart = new Keypress
                                              {
                                                  Position = positionCounter,
                                                  DocumentLength = docLength,
                                                  IncludeInReplay = true
                                              };
                            newEvent.Parts.Add(wordLogPart);
                        }    
                        else
                        {
                            var wordLogPart = keypresses.First();
                            if (wordLogPart != null)
                            {
                                wordLogPart.Position = positionCounter;
                                wordLogPart.DocumentLength = docLength;


                                if (null != previousEvent && previousEvent.Type.Equals("KEYBOARD"))
                                {
                                    if (previousEvent.Parts.OfType<Keypress>().First().Position > wordLogPart.Position)
                                    {
                                        Debug.WriteLine("Previous position at id " + previousEvent.GetId() 
                                            + " > current pos " + wordLogPart.Position);
                                    }
                                }
                            }
                        }
                        recodedEventList.Add(newEvent);
                        break;
                    case EventType.MOUSE:
                        recodedEventList.Add(newEvent);
                        break;
                    case EventType.FOCUS:
                        recodedEventList.Add(newEvent);
                        break;
                    case EventType.DOCPATH:
                        recodedEventList.Add(newEvent);
                        break;
                    case EventType.SELECTION:
                        var selection = (SelectionChange) (wordLog);
                        var selectionLength = selection.End - selection.Start;
                        recodedEventList.Add(newEvent);
                        break;
                    //case EventType.REPLACEMENT:                    
                    //    var tmpEvent = newEvent.GetEventCopy();
                    //    var tmpWordLog = (Replacement) tmpEvent.Parts[0];
                    //    var length = tmpWordLog.Length;

                    //    Replacement replace = (Replacement)(wordLog);
                    //    replace.Start = Math.Max(docLength - length, 1);
                    //    replace.End = docLength;
                    //    recodedEventList.Add(newEvent);
                    //     break;
                    case EventType.INSERT:
                        var insert = (Insert)(wordLog);
                        var insertLength = insert.Length;
                        if (insert.Position >= docLength)
                        {
                            ((Insert)(wordLog)).Position = Math.Max(docLength - insertLength, 1);
                        }
                        recodedEventList.Add(newEvent);
                        break;
                    case EventType.STATISTICS:
                        recodedEventList.Add(newEvent);
                        break;
                    case EventType.AUTHORCOMMENT:
                        recodedEventList.Add(newEvent);
                        break;
                    case EventType.PLACEHOLDER:
                        recodedEventList.Add(newEvent);
                        break;
                    case EventType.EYETRACK:
                        recodedEventList.Add(newEvent);
                        break;
                    case EventType.DRAGONNS:
                        recodedEventList.Add(newEvent);
                        break;
                }
                previousEvent = newEvent;
            }
            return recodedEventList;
        }
    }
}