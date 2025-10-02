using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Util
{

    /// <summary>
    /// This class implements a min-priority queue.
    /// </summary>
    /// <typeparam name="T">Type of the elements in the queue</typeparam>
    public class PriorityQueue<T>
    {

        #region Fields

        /// <summary>
        /// The queue itself
        /// </summary>
        private List<T> Queue;

        /// <summary>
        /// Comparer to compare elements of the queue.
        /// </summary>
        private IComparer<T> Comparer;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="coll">Collection to initialise contents of priority queue</param>
        /// <param name="comparer">Customized comparison method</param>
        /// <remarks>The elements of the collection are enqueued in order of appearance.</remarks>
        public PriorityQueue(ICollection<T> coll = null, IComparer<T> comparer = null)
        {
            Comparer = (comparer != null) ? comparer : Comparer<T>.Default;
            Queue = new List<T>();
            if (coll != null)
            {
                foreach (T elem in coll)
                {
                    this.Enqueue(elem);
                }
            }
        }

        /// <summary>
        /// Gets the number of elements actually contained in the priority queue.
        /// </summary>
        /// <returns>Number of elements in this priority queue</returns>
        public int Count()
        {
            return Queue.Count;
        }

        /// <summary>
        /// Peeks at the top element of the queue. This is the element with the highest priority, i.e. minimum value.
        /// </summary>
        /// <returns>Element with the highest priority</returns>
        public T Peek()
        {
            return (Count() > 0)? Queue[0] : default(T);
        }

        /// <summary>
        /// Adds an element to the priority queue.
        /// </summary>
        /// <param name="elem">Element to be added</param>
        /// <remarks>If the added element has an equal priority to another element already contained in this priority queue, the new element is regarded as having a lower priority than the existing one.</remarks>
        public void Enqueue(T elem)
        {
            Queue.Insert(SearchInsertIndex(elem), elem);
        }

        /// <summary>
        /// Searches the correct position in the queue to insert the new element.
        /// </summary>
        /// <param name="elem">Element to be added</param>
        /// <returns>Index specifying where to insert in the queue</returns>
        /// <remarks>If there are elements with the same priority, the new element should be added after them.</remarks>
        private int SearchInsertIndex(T elem)
        {
            int count = Count();
            if (count == 0 || Comparer.Compare(elem, Queue[count-1]) > 0) //Only element or greater than last element
            {
                return count;    //Add to the back of the list
            }
            else if (Comparer.Compare(elem, Peek()) < 0) //Less than first element
            {
                return 0;        //Add to the front of the list
            }
            else
            {
                /*
                 * Binary jumping phase (to get to the element with an equal key), followed by sequential access until a greater key.
                 *  When amount of duplicate entries is likely to be small, this is an O(log n) operation.
                 */
                int startIndex = BinarySearch(elem, 0, count-1);
                return SkipToCorrectPosition(elem, startIndex);
            }
        }

        /// <summary>
        /// Binary search2 for insertion position.
        /// </summary>
        /// <param name="addedElem">Element to insert</param>
        /// <param name="start">Starting index of search2</param>
        /// <param name="end">Ending index of search2</param>
        /// <returns>Index of _an_ equal element in the queue</returns>
        /// <remarks>Beware that this returns the index of an arbitrary-positioned element equal to the added element.</remarks>
        private int BinarySearch(T addedElem, int start, int end)
        {
            if (start < end)//As long as the starting index does not exceed the end index
            {
                int mid = (start + end) / 2;
                bool equalToMid = Comparer.Compare(addedElem, Queue[mid]) == 0;
                bool lesserThanMid = Comparer.Compare(addedElem, Queue[mid]) < 0;
                
                if(equalToMid)
                {
                    return mid;
                }
                else if(lesserThanMid)
                {
                    return BinarySearch(addedElem, start, mid);
                }
                else
                {
                    return BinarySearch(addedElem, mid + 1, end);
                }
            }
            else
            {
                return start;
            }
        }

        /// <summary>
        /// Skips to the correct insertion index, starting from an index where an equal element is located. 
        /// </summary>
        /// <param name="addedElem">Element to be added</param>
        /// <param name="startIndex">Index of an element equal to the added element</param>
        /// <returns>Index of the last element equal to the added element, + 1 (this is the index to insert at, to insert at the back of equal elements)</returns>
        private int SkipToCorrectPosition(T addedElem, int startIndex)
        {
            for (int i = (startIndex > 0 )? startIndex-1 : 0; i < Count() - 1; i++)
            {
                bool greaterThanOrEqualToPrev = Comparer.Compare(addedElem, Queue[i]) >= 0;
                bool lessThanNext = Comparer.Compare(addedElem, Queue[i + 1]) < 0;
                if (greaterThanOrEqualToPrev && lessThanNext)//Element belongs between i and i+1
                {
                    //So insert at index i+1
                    return i + 1;
                }
            }
            //Should not happen, but safety measure: add to the back of the queue
            return Count();
        }


        /// <summary>
        /// Extracts the element with the highest priority, i.e. minimum value.
        /// </summary>
        /// <returns>Element with the highest priority</returns>
        public T Dequeue()
        {
            T minElem = Peek();
            if(!IsEmpty())
            {
                Queue.RemoveAt(0);
            }
            return minElem;
        }


        /// <summary>
        /// Checks whether this queue is empty.
        /// </summary>
        /// <returns>True if the queue is empty, otherwise false</returns>
        public bool IsEmpty()
        {
            return Count() == 0;
        }


        /// <summary>
        /// Checks whether the priority queue contains a certain element.
        /// </summary>
        /// <param name="elem">Element to search2 for</param>
        /// <param name="comparer">Custom equality comparer</param>
        /// <returns>True if the element is in the queue, otherwise false</returns>
        /// <remarks>This uses the specified Comparer of the constructor, when the IEqualityComparer is left unspecified or its value is null. If no Comparer was specified in the constructor, the default one is used.</remarks>
        public bool Contains(T elem, IEqualityComparer<T> comparer = null)
        {
            Func<T, bool> predicate;
            if(comparer != null){
                predicate = (T qElem) => { return comparer.Equals(qElem, elem); };
            }
            else
            { 
                predicate = (T qElem) => { return this.Comparer.Compare(qElem, elem) == 0; };
            }

            //Get first equal element
            T firstEqualElem = Queue.FirstOrDefault<T>(predicate);


            if( (comparer != null && comparer.Equals(default(T), firstEqualElem)) 
                || (Comparer.Compare(default(T), firstEqualElem) == 0))
            {
                //It is default, so search2 failed
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns a string representation of the queue contents.
        /// </summary>
        /// <returns>String representation of the queue contents</returns>
        public override string ToString()
        {
            string content = "{";
            for (int i = 0; i < Queue.Count()-1; i++)
            {
                content += Queue[i].ToString() + ", ";
            }
            content += Queue[Count()-1].ToString() + "}";
            return content;
        }

    }
}
