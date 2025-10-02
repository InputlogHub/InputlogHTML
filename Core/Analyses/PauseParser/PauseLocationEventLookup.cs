using System;
using System.Collections.Generic;
using InputLog.Core.Events;
using InputLog.Core.Events.WinLog;

namespace InputLog.Core.Analyses.PauseParser
{

    public class PauseLocationEventLookup : IEventLookup
    {

        #region Fields

        private static PauseLocationMarker _plm;
        private static Event _currentEvent;
        private static KeyPress _currentKeyPress;
        private static string _currentKeyPressValue;
        private static bool _foundFirstAlphaNumericEvent;

        #endregion

        public PauseLocationEventLookup(PauseLocationMarker plm)
        {
            _plm = plm;
            _foundFirstAlphaNumericEvent = false;
        }

        public static void SetValues(Event currentEvent)
        {
            _currentEvent = currentEvent;
            if (_currentEvent != null)
            {
                _currentKeyPress = _currentEvent.GetKeypressFromEvent();
                _currentKeyPressValue = _currentEvent.GetKeypressValue();
            }
            else
            {
                _currentKeyPress = null;
                _currentKeyPressValue = string.Empty;
            }
        }

        public Func<bool> FindPredicate(string predicateName, int peekPositions = 0)
        {
            if (predicateName != null && !predicateName.Equals(string.Empty))
            {
                var predInfo = GetType().GetMethod(predicateName);
                if (predInfo != null)
                {
                    //Try Event
                    try
                    {
                        var predicateWoArg = (Func<Event,bool>)Delegate.CreateDelegate(typeof(Func<Event,bool>),
                            predInfo);
                        Func<bool> predicate;
                        if (peekPositions == 0)
                        {
                            predicate = () => predicateWoArg(_currentEvent);
                        }
                        else
                        {
                            predicate = () => predicateWoArg(Peek(peekPositions));
                        }
                        return predicate;
                    }
                    catch
                    {
                        //Pass this and try again
                    }
                    //Try Event,Event
                    if(predicateName.Equals("IsRecordedAfterNext"))
                    {
                        Func<bool> predicate;
                        if (peekPositions == 0)
                        {
                            predicate = () => IsRecordedAfterNext(_currentEvent, Peek(1));
                        }
                        else
                        {
                            predicate = () => IsRecordedAfterNext(Peek(peekPositions),Peek(peekPositions+1));
                        }
                        return predicate;
                    }
                    //Try KeyPress
                    try
                    {
                        var predicateWoArg = (Func<KeyPress, bool>)Delegate.CreateDelegate(typeof(Func<KeyPress, bool>), predInfo);
                        Func<bool> predicate;
                        if (peekPositions == 0)
                        {
                            predicate = () => predicateWoArg(_currentKeyPress);
                        }
                        else
                        {
                            predicate = () => predicateWoArg(EventToKeyPress(Peek(peekPositions)));
                        }
                        return predicate;
                    }
                    catch
                    {
                        //Pass this and try again
                    }
                    //Try KeyPressValue (string)
                    try
                    {
                        var predicateWoArg = (Func<string, bool>)Delegate.CreateDelegate(typeof(Func<string, bool>), predInfo);
                        Func<bool> predicate;
                        if (peekPositions == 0)
                        {
                            predicate = () => predicateWoArg(_currentKeyPressValue);
                        }
                        else
                        {
                            predicate = () => predicateWoArg(EventToKeyPressValue(Peek(peekPositions)));
                        }
                        return predicate;
                    }
                    catch (Exception)
                    {
                        //Pass this and try again
                    }
                    //Try without arguments
                    try
                    {
                        var predicate = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), predInfo);
                        return predicate;
                    }
                    catch (Exception)
                    {
                        //Pass this and return at the end
                    }
                }
            }
            return null;
        }

        private static Event Peek(int i)
        {
            return _plm.Peek(i);
        }
        
        private static KeyPress EventToKeyPress(Event evt)
        {
            return (evt == null)? null : evt.GetKeypressFromEvent();
            
        }
        
        private static string EventToKeyPressValue(Event evt)
        {
            var keyPress = EventToKeyPress(evt);
            return (keyPress == null) ? null : keyPress.Value;
        }
        
        #region Events
        /// <summary>
        /// Always returns true.
        /// </summary>
        /// <returns>True</returns>
        public static bool True() 
        { 
            return true; 
        }

        #region ValueEvents

        public static bool EmptyValue(string keyPressValue)
        {
            return keyPressValue.Length == 0;
        }

        public static bool IsReturn(string keyPressValue)
        {
            return Lexical.IsReturn(keyPressValue);
        }

        public static bool IsSpace(string keyPressValue)
        {
            return Lexical.IsWhiteSpace(keyPressValue);
        }

        public static bool IsTab(string keyPressValue)
        {
            return Lexical.IsTab(keyPressValue);
        }

        public static bool IsSentenceReadingMark(string keyPressValue)
        {
            return Lexical.IsSentenceReadingMark(keyPressValue);
        }

        public static bool IsWordReadingMark(string keyPressValue)
        {
            return Lexical.IsWordReadingMark(keyPressValue);
        }

        public static bool IsAlphaNumeric(string keyPressValue)
        {
            return Lexical.IsAlphaNumeric(keyPressValue);
        }

        public static bool IsFirstAlphaNumericEvent(string keyPressValue)
        {
            if (_foundFirstAlphaNumericEvent) return false;
            _foundFirstAlphaNumericEvent = Lexical.IsAlphaNumeric(keyPressValue);
            return _foundFirstAlphaNumericEvent;
        }

        public static bool IsWithinWordChar(string keyPressValue)
        {
            return Lexical.IsWithinWordChar(keyPressValue);
        }

        public static bool IsSentencePrecedingReadingMark(string keyPressValue)
        {
            return Lexical.IsSentencePrecedingReadingMark(keyPressValue);
        }

        public static bool IsBindingChar(string keyPressValue)
        {
            return Lexical.IsBindingChar(keyPressValue);
        }

        public static bool IsSentenceChar(string keyPressValue)
        {
            return keyPressValue != null
                && !string.Empty.Equals(keyPressValue)
                && (Lexical.IsWithinWordChar(keyPressValue)
                    || Lexical.IsSentencePrecedingReadingMark(keyPressValue)
                    || Lexical.IsWordReadingMark(keyPressValue));
        }

        public static bool IsParagraphChar(string keyPressValue)
        {
            return keyPressValue != null
                && !string.Empty.Equals(keyPressValue)
                && (Lexical.IsWithinWordChar(keyPressValue)
                    || Lexical.IsSentencePrecedingReadingMark(keyPressValue)
                    || Lexical.IsWordReadingMark(keyPressValue)
                    || Lexical.IsSentenceReadingMark(keyPressValue));
        }

        public static bool IsCharacter(string keyPressValue)
        {
            return Lexical.IsCharacter(keyPressValue);
        }

        public static bool IsCapitalLetter(string keyPressValue)
        {
            return Lexical.IsCapitalLetter(keyPressValue);
        }

        public static bool IsDot(string keyPressValue)
        {
            return keyPressValue == ".";
        }

        public static bool IsBackSpace(string keyPressValue)
        {
            return Lexical.IsBackSpace(keyPressValue);
        }

        public static bool IsDigit(string keyPressValue)
        {
            return Lexical.IsDigit(keyPressValue);
        }

        public static bool IsArithmeticOperation(string keyPressValue)
        {
            return Lexical.IsArithmeticOperation(keyPressValue);
        }
        #endregion

        #region KeyPressEvents

        public static bool HasShiftKey(KeyPress keyPress)
        {
            return (keyPress != null) && Lexical.HasShiftKey(keyPress);
        }

        public static bool IsCtrlBackSpace(KeyPress keyPress)
        {
            return (keyPress != null) && Lexical.IsCtrlBackSpace(keyPress);
        }

        public static bool HasCombinationKey(KeyPress keyPress)
        {
            return (keyPress != null) && (keyPress.KeyboardState.Count != 0 && Lexical.HasCombinationKey(keyPress));
        }

        public static bool HasRevisionKey(KeyPress keyPress)
        {
            return (keyPress != null) && Lexical.HasRevisionKey(keyPress);
        }

        #endregion

        #region EventEvents
        public static bool IsRecordedAfterNext(Event currentEvent, Event nextEvent)
        {
            IComparer<Event> comparer = new CompareEventTime();
            if (comparer.Compare(currentEvent, nextEvent) < 0)
            {
                return true;
            }
            return false;
        }
        #endregion
        
        #endregion
    }
}
