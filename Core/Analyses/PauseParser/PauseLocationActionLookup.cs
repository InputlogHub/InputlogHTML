using System;
using System.Diagnostics;

namespace InputLog.Core.Analyses.PauseParser
{
    public class PauseLocationActionLookup : IActionLookup
    {
        #region Fields

        /// <summary>
        /// Reference to PauseLocationMarker that owns this object.
        /// </summary>
        private static PauseLocationMarker _marker;

        #endregion

        public PauseLocationActionLookup(PauseLocationMarker marker)
        {
            _marker = marker;
        }

        public Action FindAction(string actionName)
        {
            if (actionName == null || actionName.Equals("")) return null;

            var actInfo = GetType().GetMethod(actionName);
            if (actInfo == null) return null;

            try
            {
                return (Action) Delegate.CreateDelegate(typeof (Action), actInfo);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Reset()
        {
            Debug.WriteLine("Reset");
            PauseLocationMarker.ResetCurrentFSM();
        }

        #region MarkPauseLocation

        public static void MarkAfterParagraphs()
        {
            _marker.GeneratePauseLocation(PauseLocation.AFTER_PARAGRAPHS);
        }
        
        public static void MarkAfterSentences()
        {
            _marker.GeneratePauseLocation(PauseLocation.AFTER_SENTENCES);
        }
        
        public static void MarkAfterWords()
        {
            _marker.GeneratePauseLocation(PauseLocation.AFTER_WORDS);
        }

        public static void MarkBeforeParagraphs()
        {
            _marker.GeneratePauseLocation(PauseLocation.BEFORE_PARAGRAPHS);
        }

        public static void MarkBeforeSentences()
        {
            _marker.GeneratePauseLocation(PauseLocation.BEFORE_SENTENCES);
        }
        
        public static void MarkBeforeWords()
        {
            _marker.GeneratePauseLocation(PauseLocation.BEFORE_WORDS);
        }
        
        public static void MarkEnd(){
            _marker.GeneratePauseLocation(PauseLocation.END);
        }

        public static void MarkInitial()
        {
            _marker.GeneratePauseLocation(PauseLocation.INITIAL);
        }

        public static void MarkTransition()
        {
            _marker.GeneratePauseLocation(PauseLocation.CHANGE);
        }

        public static void MarkWithinWords()
        {
            _marker.GeneratePauseLocation(PauseLocation.WITHIN_WORDS);
        }

        #endregion

        #region NotifyHandler

        public static void NotifyWord()
        {
            _marker.NotifyWordListeners();
        }

        public static void NotifyWordChar()
        {
            _marker.NotifyWordCharListeners();
        }

        public static void NotifySentence()
        {
            _marker.NotifySentenceListeners();
        }

        public static void NotifySentenceChar()
        {
            _marker.NotifySentenceCharListeners();
        }

        public static void NotifyParagraph()
        {
            _marker.NotifyParagraphListeners();
        }

        public static void NotifyParagraphChar()
        {
            _marker.NotifyParagraphCharListeners();
        }

        #endregion

        #region Log

        public static void LogP0()
        {
            Debug.Write("P0, ");
        }

        public static void LogP1()
        {
            Debug.Write("P1, ");
        }

        public static void LogP2()
        {
            Debug.Write("P2, ");
        }

        public static void LogS1()
        {
            Debug.Write("S1, ");
        }

        public static void LogS2()
        {
            Debug.Write("S2, ");
        }

        public static void LogS3()
        {
            Debug.Write("S3, ");
        }

        public static void LogS4()
        {
            Debug.Write("S4, ");
        }

        public static void LogS5()
        {
            Debug.Write("S5, ");
        }

        public static void LogS6()
        {
            Debug.Write("S6, ");
        }

        public static void LogW1()
        {
            Debug.Write("W1, ");
        }

        public static void LogW2()
        {
            Debug.Write("W2, ");
        }

        public static void LogW3()
        {
            Debug.Write("W3, ");
        }

        public static void LogW4()
        {
            Debug.Write("W4, ");
        }

        public static void LogW5()
        {
            Debug.Write("W5, ");
        }

        public static void LogW6()
        {
            Debug.Write("W6, ");
        }

        public static void LogW7()
        {
            Debug.Write("W7, ");
        }

        public static void LogC2()
        {
            Debug.Write("C2, ");
        }

        public static void LogC3()
        {
            Debug.Write("C3, ");
        }

        public static void LogC4()
        {
            Debug.Write("C4, ");
        }

        #endregion
    }
}