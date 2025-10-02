using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InputLog.Core.Events;
using InputLog.Core.Events.WordLog;

namespace InputLog.Core.IO.Convert.TranslogConverter
{
    /// <summary>
    /// Derives a few numbers from the final text.
    /// </summary>
    public class TextStatistics : ILConvertible
    {
        #region Fields

        private readonly List<Event> EventList;
        private readonly string FinalText;
        private readonly TranslogReader.DocParam Parameter;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">The final production</param>
        /// <param name="param">Document parameters: length, position, event id.</param>
        public TextStatistics(string text, TranslogReader.DocParam param)
        {
            FinalText = text;
            Parameter = param;
            EventList = new List<Event>();
        }

        /// <summary>
        /// Converting a few numbers about the produced text into a 'statistics' event.
        /// </summary>
        /// <returns>List with the statistics event</returns>
        public IEnumerable<Event> ConvertToEvent()
        {
            // Analyzing the text.  
            MatchCollection words = Regex.Matches(FinalText, @"[\S]+");
            int wordCnt = words.Count;
            int charCntExclSpaces = FinalText.Count(c => !char.IsWhiteSpace(c));
            int charCntInclSpaces = FinalText.Length;
            int farEastCharCnt = 0;
            int count = 1;
            int start = 0;
            while ((start = FinalText.IndexOf('\n', start)) != -1)
            {
                count++;
                start++;
            }
            int paragraphCnt = count;
            int lineCnt  = count;
            // The input is HTML. Estimating 250 words per A4 page.
            int pageCnt = (wordCnt - 1) / 250 + 1;

            // For the time being, Translog does not provide the number of characters, words, etc. 
            // of the start document before logging.
            int startWordCnt = 0;
            int startCharCntExclSpaces = 0;
            int startCharCntInclSpaces = 0;
            int startFarEastCharCnt = 0;
            int startParagraphCnt = 0;
            int startLineCnt = 0;
            int startPageCnt = 0;

            // Putting the numbers into a new event.
            var thisEvent = new Event();
            thisEvent.Properties["type"] = "statistics";
            thisEvent.Properties["id"] = (Parameter.Id++).ToString();
            thisEvent.Parts.Add(new Statistics(charCntExclSpaces, charCntInclSpaces, farEastCharCnt, lineCnt, pageCnt, paragraphCnt, wordCnt,
                startCharCntExclSpaces, startCharCntInclSpaces, startFarEastCharCnt, startLineCnt, startPageCnt, startParagraphCnt, startWordCnt));
           
            EventList.Add(thisEvent);
            return EventList;
        }
    }
}