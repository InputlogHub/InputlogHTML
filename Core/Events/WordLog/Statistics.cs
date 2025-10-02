using Microsoft.Office.Interop.Word;

namespace InputLog.Core.Events.WordLog
{
    /// <summary>
    /// An EventPart that contains the document statistics.
    /// </summary>
    public sealed class Statistics : IEventPart
    {
        #region Fields
        /// <summary>
        /// Count of characters excluding spaces.
        /// </summary>
        public int CharCountWithoutSpaces;
        public int StartCharCountWithoutSpaces;

        /// <summary>
        /// Count of characters including spaces.
        /// </summary>
        public int CharCountWithSpaces;
        public int StartCharCountWithSpaces;

        /// <summary>
        /// Count of Far East characters.
        /// </summary>
        public int FarEastCharCount;
        public int StartFarEastCharCount;
        /// <summary>

        /// Count of lines.
        /// </summary>
        public int LineCount;
        public int StartLineCount;

        /// <summary>
        /// Count of pages.
        /// </summary>
        public int PageCount;
        public int StartPageCount;

        /// <summary>
        /// Count of paragraphs.
        /// </summary>
        public int ParagraphCount;
        public int StartParagraphCount;

        /// <summary>
        /// Count of words.
        /// </summary>
        public int WordCount;
        public int StartWordCount;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="wholestory">A range over the whole final document story.</param>
        public Statistics(Range wholestory)
        {
            CharCountWithoutSpaces = wholestory.ComputeStatistics(WdStatistic.wdStatisticCharacters);
            CharCountWithSpaces = wholestory.ComputeStatistics(WdStatistic.wdStatisticCharactersWithSpaces);
            FarEastCharCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticFarEastCharacters);
            LineCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticLines);
            PageCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticPages);
            ParagraphCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticParagraphs);
            WordCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticWords);
        }

        /// <summary>
        /// Constructor for the xml-reader
        /// </summary>
        /// <param name="charCntExclSpaces">Char count excluding spaces.</param>
        /// <param name="charCntInclSpaces">Char count including spaces.</param>
        /// <param name="farEastCharCnt">Char count of Chinese & other Asian symbols.</param>
        /// <param name="lineCnt">Line count.</param>
        /// <param name="pageCnt">Page count.</param>
        /// <param name="paragraphCnt">Paragraph count.</param>
        /// <param name="wordCnt">Word count.</param>
        /// <param name="startCharCntExclSpaces">Char count excluding spaces before logging.</param>
        /// <param name="startCharCntInclSpaces">Char count including spaces before logging.</param>
        /// <param name="startFarEastCharCount">Char count including spaces before logging.</param>
        /// <param name="startLineCnt">Line count before logging.</param>
        /// <param name="startPageCnt">Page count before logging.</param>
        /// <param name="startParagraphCnt">Paragraph count before logging.</param>
        /// <param name="startWordCnt">Word count before logging.</param>
        public Statistics(int charCntExclSpaces, int charCntInclSpaces, int farEastCharCnt, int lineCnt, int pageCnt, 
            int paragraphCnt, int wordCnt, int startCharCntExclSpaces, int startCharCntInclSpaces, int startFarEastCharCount,
            int startLineCnt, int startPageCnt, int startParagraphCnt, int startWordCnt)
        {
            CharCountWithoutSpaces = charCntExclSpaces;
            CharCountWithSpaces = charCntInclSpaces;
            FarEastCharCount = farEastCharCnt;
            LineCount = lineCnt;
            PageCount = pageCnt;
            ParagraphCount = paragraphCnt;
            WordCount = wordCnt;
            StartCharCountWithoutSpaces = startCharCntExclSpaces;
            StartCharCountWithSpaces = startCharCntInclSpaces;
            StartFarEastCharCount = startFarEastCharCount;
            StartLineCount = startLineCnt;
            StartPageCount = startPageCnt;
            StartParagraphCount = startParagraphCnt;
            StartWordCount = startWordCnt;
        }

        /// <summary>
        /// Constructor.
        /// Overload allowing the reporting of document statistics before the
        /// actual logging started, i.e. the count of characters, words, etc. of the template
        /// or reused document. They might be empty if a new document was opened.
        /// </summary>
        /// <param name="wholestory">A range over the whole final document story.</param>
        /// <param name="startStats">Variables from the document before logging.</param>
        public Statistics(Range wholestory, Statistics startStats)
        {
            CharCountWithoutSpaces = wholestory.ComputeStatistics(WdStatistic.wdStatisticCharacters);
            CharCountWithSpaces = wholestory.ComputeStatistics(WdStatistic.wdStatisticCharactersWithSpaces);
            FarEastCharCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticFarEastCharacters);
            LineCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticLines);
            PageCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticPages);
            ParagraphCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticParagraphs);
            WordCount = wholestory.ComputeStatistics(WdStatistic.wdStatisticWords);
            StartCharCountWithoutSpaces = startStats.CharCountWithoutSpaces;
            StartCharCountWithSpaces = startStats.CharCountWithSpaces;
            StartFarEastCharCount = startStats.FarEastCharCount;
            StartLineCount = startStats.LineCount;
            StartPageCount = startStats.PageCount;
            StartParagraphCount = startStats.ParagraphCount;
            StartWordCount = startStats.WordCount;
        }

        public void AddStats(Statistics stat)
        {
            CharCountWithoutSpaces += stat.CharCountWithoutSpaces;
            CharCountWithSpaces += stat.CharCountWithSpaces;
            FarEastCharCount += stat.FarEastCharCount;
            LineCount += stat.LineCount;
            PageCount += stat.PageCount;
            ParagraphCount += stat.ParagraphCount;
            WordCount += stat.WordCount;
            StartCharCountWithoutSpaces += stat.StartCharCountWithoutSpaces;
            StartCharCountWithSpaces += stat.StartCharCountWithSpaces;
            StartFarEastCharCount += stat.StartFarEastCharCount;
            StartLineCount += stat.StartLineCount;
            StartPageCount += stat.StartPageCount;
            StartParagraphCount += stat.StartParagraphCount;
            StartWordCount += stat.StartWordCount;
        }

        /// <summary>
        /// Parameterless constructor for ReadXml().
        /// </summary>
        public Statistics()
        {
        }
    }
}