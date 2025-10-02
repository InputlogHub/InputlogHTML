using System.Collections.Generic;

namespace InputLog.Core.Util
{
    public class Sort
    {

        /// <summary>
        /// Own implementation of mergesort (https://en.wikipedia.org/wiki/Array_merging).
        /// This is to ensure stability of the sorting algorithm, as stability is sometimes ignored in built-in implementation.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the list to be sorted</typeparam>
        /// <param name="listToSort">List to be sorted</param>
        /// <param name="comparer">Comparer for 2 elements</param>
        /// <param name="sortedList">Sorted list</param>
        /// <returns>Indices of values in the unsorted list, in the order of the sorted list</returns>
        /// <remarks>The sorted output is a new list, containing the old values.</remarks>
        /// <remarks>C#'s implementation of List.Sort seems to use quicksort (a sorting algorithm deemed to be unstable).</remarks>
        public static int[] MergeSort<T>(List<T> listToSort, out List<T> sortedList, IComparer<T> comparer = null)
        {
            //Usual fail safety
            if (listToSort == null)
            {
                //Can't sort this
                sortedList = null;
                return null;
            }

            //Prepare indices
            int[] indices = new int[listToSort.Count];
            for (int i = 0; i < listToSort.Count; i++)
            {
                indices[i] = i;
            }
            
            if (listToSort.Count <= 1)
            {
                //No need to/can't sort this
                sortedList = new List<T>();
                if (listToSort.Count == 1)
                {
                    sortedList.Add(listToSort[0]);
                }
                //Avoid overhead of calling MergeSortBack unnecessarily
            }
            else
            {
                T[] a = listToSort.ToArray();
                MergeSortBack(a, 0, a.Length - 1, comparer ?? Comparer<T>.Default, indices);
                sortedList = new List<T>(a);
            }
            return indices;
        }

        /// <summary>
        /// Backend for the mergesort algorithm, allowing recursive calls
        /// </summary>
        /// <typeparam name="T">Type of the elements to be sorted</typeparam>
        /// <param name="a">Array containing all elements</param>
        /// <param name="p">Starting index of the current mergesort step in the array</param>
        /// <param name="r">Ending index of the current mergesort step in the array </param>
        /// <param name="compareFunc">Comparison function</param>
        /// <param name="indices">Indices of values in the unsorted list, in the order of the sorted list</param>
        private static void MergeSortBack<T>(T[] a, int p, int r, IComparer<T> compareFunc, int[] indices)
        {
            if (p < r)//As long as the starting index does not exceed the end index
            {
                int q = (p + r)/2;
                MergeSortBack(a, p, q, compareFunc, indices);//Sort one half
                MergeSortBack(a, q + 1, r, compareFunc, indices);//Sort the second half
                Merge(a, p, q, r, compareFunc, indices);//Merge them in the correct order
            }
        }

        /// <summary>
        /// Merge operation, merges 2 parts of an array
        /// </summary>
        /// <typeparam name="T">Type of the elements to be sorted</typeparam>
        /// <param name="a">Array to be sorted</param>
        /// <param name="p">Starting index of the first part to merge</param>
        /// <param name="q">Mid between starting and ending indexes (end of the first part, beginning of the second part)</param>
        /// <param name="r">Ending index of the second part to merge</param>
        /// <param name="compareFunc">Comparison function</param>
        /// <param name="I">Indices of values in the unsorted list, in the order of the sorted list</param>
        private static void Merge<T>(T[] a, int p, int q, int r, IComparer<T> compareFunc, int[] I)
        {
            int n1 = q - p + 1;//Capacity of first part
            int n2 = r - q;//Capacity of second part
            T[] l = new  T[n1];
            T[] u = new T[n2];
            int[] indL = new int[n1];
            int[] indR = new int[n2];

            //Split in left and right part and initialise them
            for (int i = 0; i < n1; i++)
            {
                l[i] = a[p + i];
                indL[i] = I[p + i];
            }
            for (int j = 0; j < n2; j++)
            {
                u[j] = a[q + j+1];
                indR[j] = I[q + j+1];
            }

            int lIndex = 0;
            int rIndex = 0;
            for (int k = p; k <= r; k++)//k is index in merged range in A
            {
                //Safety measures
                bool lEmpty = lIndex >= n1;
                bool rEmpty = rIndex >= n2;

                if (!lEmpty && (rEmpty || compareFunc.Compare(l[lIndex], u[rIndex]) <= 0))
                {
                    a[k] = l[lIndex];
                    I[k] = indL[lIndex];
                    lIndex += 1;
                }
                else if (!rEmpty)
                {
                    a[k] = u[rIndex];
                    I[k] = indR[rIndex];
                    rIndex += 1;
                }
                else
                {
                    return;
                }
            }
        }
    }
}
