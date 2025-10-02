
using System.Collections.Generic;
using System.Diagnostics;
using InputLog.Core.Reporting.Output;
using InputLog.Core.Reporting.ReportTemplate;

namespace InputLog.Core.Reporting.Report
{
    /// <summary>
    /// A single block in a report may hold many values. Elements that are 
    /// put together in a block will be outputted together as well.
    /// </summary>
    public class Block
    {

        /// <summary>
        /// Get the header of the block
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        private List<Element> _elements;

        /// <summary>
        /// Create a new block
        /// </summary>
        /// <param name="heading">The heading of this block</param>
        public Block(string heading)
        {
            Title = heading;
            _elements = new List<Element>();
        }

        public void AddElement(Element element)
        {
            _elements.Add(element);
        }

        public void Format(Formatter formatter, BlockTemplate blockTemplate)
        {
            Debug.Assert(blockTemplate.Elements.Count == _elements.Count);
            formatter.StartBlock(blockTemplate);
            formatter.StartElements(_elements.Count);
            for (int i = 0; i < blockTemplate.Elements.Count; i++)
            {
                ElementTemplate eTemplate = blockTemplate.Elements[i];
                Element element = _elements[i];
                element.Format(formatter, eTemplate);
            }
            formatter.EndElements();
            formatter.EndBlock();
        }
 

    }
}
