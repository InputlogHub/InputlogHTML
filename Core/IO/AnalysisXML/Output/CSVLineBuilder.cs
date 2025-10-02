using System;
using System.Text;

namespace InputLog.Core.IO.AnalysisXML.Output
{
    internal class CSVLineBuilder
    {
        #region Fields

        /// <summary>
        /// CSV parameters
        /// </summary>
        private const char PREFIX = '\"';
        private const char SUFFIX = '\"';
        public const char SEPARATOR = ';';

        /// <summary>
        /// Do we bother caring about the count or is this a linebuilder that
        /// does not bother about how many elements are on each line?
        /// </summary>
        private readonly bool CountCare;

        private readonly StringBuilder Line;

        /// <summary>
        /// The number of elements in a single line.
        /// </summary>
        private readonly int LineSize;

        /// <summary>
        /// The number of current elements in the line.
        /// </summary>
        private int Count;

        #endregion

        /// <summary>
        /// Create the CSV Line Builder.
        /// </summary>
        /// <param name="lineSize">The maximum size of a line. All lines are 
        /// with less elements are 'enlonged' to the lineSize with empty elements. (optional)</param>
        public CSVLineBuilder(int lineSize = -1)
        {
            LineSize = lineSize;
            CountCare = (lineSize != -1);
            Line = new StringBuilder();
        }

        /// <summary>
        /// Begin a new line.
        /// </summary>
        public void BeginLine()
        {
            Count = 0;
        }

        /// <summary>
        /// Add an element to the line.
        /// </summary>
        /// <param name="element">Element to add to the line</param>
        public void Append(string element)
        {
            if (!CountCare && Line.Length > 0)
            {
                Line.Append(SEPARATOR);
            }
            Line.Append(PREFIX);
            Line.Append(element.Replace("\r", string.Empty).Replace("\n", string.Empty));
            Line.Append(SUFFIX);
            if (CountCare)
            {
                Count++;
                if (Count < LineSize)
                {
                    Line.Append(SEPARATOR);
                }
                else if (Count > LineSize)
                {
                    throw new Exception("Construction of line with more elements than specified lineSize.");
                }
            }
        }

        /// <summary>
        /// Add a string element consisting of a number of element by 
        /// itself to the line.
        /// </summary>
        /// <param name="element">Composed element to add</param>
        /// <param name="elementSize">Size of the composed element = number of elements
        /// in the composed element</param>
        public void Append(string element, int elementSize)
        {
            Line.Append(element);
            if (CountCare)
            {
                Count += (elementSize - 1);
                if (Count < LineSize)
                {
                    Line.Append(SEPARATOR);
                }
                else if (Count > LineSize)
                {
                    throw new Exception("Construction of line with more elements than specified lineSize.");
                }
            }
        }

        /// <summary>
        /// End the construction of the current line, and retrieve the 
        /// construct CSV string. Enlonged to the length of LineSize 
        /// as given during construction of this object.
        /// </summary>
        /// <returns>The constructed CSV line enlonged to LineSize, if LineSize is set.</returns>
        public string EndLine()
        {
            if (CountCare)
            {
                // Enlong string to linesize.
                for (int i = Count; i < LineSize; i++)
                {
                    Line.Append(PREFIX);
                    Line.Append(SUFFIX);
                    if (Count < LineSize)
                    {
                        Line.Append(SEPARATOR);
                    }
                }
            }

            // return the fully constructed csv string
            string line = Line.ToString();
            Line.Clear();
            return line;
        }
    }
}