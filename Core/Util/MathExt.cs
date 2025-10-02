using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace InputLog.Core.Util
{
    /// <summary>
    /// This class some Mathematical Utility functions.
    /// </summary>
    public static class MathExt
    {
        /// <summary>
        /// Epsilon value, an arbitrarily small positive quantity 
        /// used as the lower limit of a calculation.
        /// </summary>
        private const double EPS = 10e-5;

        /// <summary>
        /// 20120406 EVH: The original formula has 'count-1'. However, N-1 is to be used when calculating 
        /// the stdev from a random sample from the population. Our data set covers the entire population, 
        /// therefore division by 'count' is the right thing to do.
        /// 
        /// Formula: sigma = sqrt( variance )
        /// mean = total / count
        /// variance = [sum_of_squares - (count * mean^2)] / (count - 1)  ---> use 'count - 1' for sampled data only
        /// Substitute mean in variance:
        /// variance = [sum_of_squares - (count * (total/count)^2] / (count - 1)
        ///     = [sum_of_squares - (total^2 / count)] / (count - 1)
        /// </summary>
        /// <param name="count">The number of elements</param>
        /// <param name="square">The sum of the squared values of the elements</param>
        /// <param name="total">The sum of the values of the elements</param>
        /// <returns></returns>
        public static double StandardDeviation(double count, double square, double total)
        {
            if (!(count > 2)) return 0;
            var d1 = Math.Pow(total, 2) / count;
            var d2 = square - d1;
            var d3 = d2/count; // (count - 1);  // ---> use 'count - 1' for sampled data only
            return d3 < EPS ? 0 : Math.Sqrt(d3);
        }

        /// <summary>
        /// Formula: sigma = sqrt( variance )
        /// mean = total / count
        /// variance = [sum_of_squares - (count * mean^2)] / (count - 1)  ---> use 'count - 1' for sampled data only
        /// Substitute mean in variance:
        /// variance = [sum_of_squares - (count * (total/count)^2] / (count - 1)
        ///     = [sum_of_squares - (total^2 / count)] / (count - 1)
        /// </summary>
        /// <param name="count">The number of elements</param>
        /// <param name="square">The sum of the squared values of the elements</param>
        /// <param name="total">The sum of the values of the elements</param>
        /// <returns></returns>
        public static double StandardDeviationSample(double count, double square, double total)
        {
            if (!(count > 2)) return 0;
            var d1 = Math.Pow(total, 2) / count;
            var d2 = square - d1;
            var d3 = d2 / (count - 1);
            return d3 < EPS ? 0 : Math.Sqrt(d3);
        }

        /// <summary>
        /// Returns the population standard deviation (not the sample deviation) from a list of values.
        /// </summary>
        /// <param name="values">List of doubles</param>
        /// <returns>The population standard deviation.</returns>
        public static double StDevFromList(IEnumerable<double> values)
        {           
            var enumerable = values as double[] ?? values.ToArray();
            if(enumerable.Length < 3 ) return 0;
            double avg = enumerable.Average();
            return Math.Sqrt(enumerable.Average(v => Math.Pow(v - avg, 2)));
        }

        /// <summary>
        /// Returns true if the modulo 2 division of an integer yields a remainder.
        /// </summary>
        /// <param name="value">the integer value to check</param>
        /// <returns>boolean true if there is a remainder</returns>
        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        /// <summary>
        /// Returns the mean 
        /// </summary>
        /// <param name="count">The number of elements</param>
        /// <param name="total">The sum of the values of the elements</param>
        /// <returns></returns>
        public static double Mean(double count, double total)
        {
            return Math.Abs(count - 0) < EPS ? 0 : total / count;
        }

        /// <summary>
        /// Returns the mean from a list.
        /// </summary>
        /// <param name="elements">List of the elements</param>
        /// <returns></returns>
        public static double MeanFromList(List<double> elements)
        {
            return Math.Abs(elements.Count - 0) < EPS ? 0 : elements.Sum() / elements.Count;
        }

        public static double[] PolynomialFit(double[] xs, double[] ys, int degree)
        {
            List<DenseVector> vectors = new List<DenseVector>();
            if (degree >= xs.Length) degree = xs.Length - 1;
            for (int i = 0; i <= degree; i++)
            {
                var i1 = i;
                vectors.Add(DenseVector.Create(xs.Length, pos => Math.Pow(xs[pos], i1)));
            }
            // build matrices
            var x = DenseMatrix.OfColumns(xs.Length, degree+1, vectors);
            var y = new DenseVector(ys);

            // solve
            return x.QR().Solve(y).ToArray();
        }

        public static double ComputePolynomial(double x, double[] parameters)
        {
            return parameters.Select((t, i) => (t*Math.Pow(x, i))).Sum();
        }

        public static double FindMedian(List<double> sortedNrs)
        {
            return FindMedian(sortedNrs, x => x, (x, y) => (x + y) / 2.0);
        }

        public static double FindMedian(List<ulong> sortedNrs)
        {
            return FindMedian(sortedNrs, x => (double)x, (x, y) => (x + y) / 2.0);
        }

        private static TX FindMedian<T,TX>(IList<T> sortedNrs, Func<T,TX> convFunc, Func<T,T,TX> avgFunc)
        {
            if (sortedNrs.Count == 0) return default(TX);
            if (sortedNrs.Count == 1) return convFunc(sortedNrs[0]);
            if (IsOdd(sortedNrs.Count))
            {
                int mid = (sortedNrs.Count / 2);
                return convFunc(sortedNrs[mid]);
            }
            else
            {
                int mid = (sortedNrs.Count / 2);
                return avgFunc(sortedNrs[mid], sortedNrs[mid - 1]);
            }
        }

        /// <summary>
        /// Returning the median of a list of unsorted elements: it picks the 
        /// middle element when the number of elements is odd or the average of 
        /// the two middles when the number is even.
        /// </summary>
        /// <param name="elements">List with unsorted elements</param>
        /// <returns>The median value.</returns>
        public static double GetMedian(List<double> elements)
        {
            switch (elements.Count)
            {
                case 0:
                    return 0;
                case 1:
                    return elements[0];
                case 2:
                    return (elements[0] + elements[1]) / 2;
            }

            // The list cast into an array and sorted.
            double[] elementArray = elements.ToArray();    
            var sortedNumbers = (double[])elementArray.Clone();
            elementArray.CopyTo(sortedNumbers, 0);
            Array.Sort(sortedNumbers);

            // Getting the median (integer division).
            int size = sortedNumbers.Length;
            var mid = size / 2;
            var median = (IsOdd(size)) ? sortedNumbers[mid]
                : (sortedNumbers[mid] + sortedNumbers[mid - 1]) / 2;
            return median;
        }

        /// <summary>
        ///     Calculates first and third quartiles and the median on a 
        ///     list of unsorted data.
        /// </summary>
        /// <param name="elements">List of element</param>
        /// <returns>
        /// Tuple:
        /// - Item1: boxplot minimum value
        /// - Item2: the first quartile
        /// - Item3: the median (second quartile)
        /// - Item4: the third quartile
        /// - Item5: the boxplot maximum value
        /// </returns>
        public static Tuple<double, double, double, double, double> GetBoxPlotValues(List<double> elements)
        {
            if (elements.Count < 3)
            {
                return new Tuple<double, double, double, double, double>(0, 0, 0, 0, 0);
            }

            double[] sortedNumbers = elements.ToArray<double>();
            Array.Sort(sortedNumbers);

            Func<double, double> noConvert = (n => n);
            Func<double, double, double> average = ((n1, n2) => (n1 + n2) / 2.0);

            double median = FindMedian(
                sortedNumbers,
                noConvert,
                average
            );
            double firstQuartile = FindMedian(
                sortedNumbers.Where(x => x < median).ToList(),
                noConvert,
                average
            );
            double thirdQuartile = FindMedian(
                sortedNumbers.Where(x => x > median).ToList(),
                noConvert,
                average
            );
            double interQ = thirdQuartile - firstQuartile;
            double lowerBound = firstQuartile - (1.5) * interQ;
            double upperBound = thirdQuartile + (1.5) * interQ;


            IEnumerable<double> lowerBoundList = sortedNumbers.Where(x => x >= lowerBound);
            double min = lowerBoundList.Any() ? lowerBoundList.First() : sortedNumbers.First();

            IEnumerable<double> upperBoundList = sortedNumbers.Reverse().Where(x => x <= upperBound);
            double max = upperBoundList.Any() ? upperBoundList.First() : sortedNumbers.Last();

            return new Tuple<double, double, double, double, double>(min, firstQuartile, median, thirdQuartile, max);
        }

        /// <summary>
        /// Calculates a Confidence Interval (CI) around the mean from a list of pause values
        /// with the Student T distribution with n-1 degrees of freedom.
        /// CI = Mean +- Student T * SE, where SE (Standard Error) = stDev/SQRT(n)
        /// When the pauselist contains log data, the mean is geometric.
        /// </summary>
        /// <param name="pauseList">A list containing pause values.</param>
        /// <param name="confidenceLevel">The confidence level has 95% as default.</param>
        /// <returns>a tuple with as first element the low interval boundary (double),
        /// as second the (geometric) mean, thirdly the high interval boundary (double), and
        /// as fourth element the coefficient of variation to replace the standard deviation with
        /// logtransformed data.</returns>
        public static Tuple<double, double, double, double> CalculateInterval(List<double> pauseList, 
            double confidenceLevel = .95)
        {
            Tuple<double, double, double, double> intervals;
            int sampleSize = pauseList.Count;
            if (sampleSize < 3)
            {
                intervals = new Tuple<double, double, double, double>(0, 0, 0, 0);
                return intervals;
            }
            double cLevel = (confidenceLevel + 1) / 2;
            double mean = pauseList.Mean();
            double sd = pauseList.StandardDeviation();
            StudentT student = new StudentT(0, 1, sampleSize - 1);
            double T = FindRoots.OfFunction(x => student.CumulativeDistribution(x) - cLevel, -800, 800);
            // Console.WriteLine("Sample size:" + (sampleSize - 1) + " T = " + T);
            double t = T * (sd / Math.Sqrt(sampleSize));
            // Coefficient of Variation (CV).
            // See: https://en.wikipedia.org/wiki/Coefficient_of_variation
            double cV = Math.Sqrt(Math.Exp(Math.Pow(sd, 2)) - 1);
            if (Math.Abs(cV)> 10E5) cV = double.PositiveInfinity;
            intervals = new Tuple<double, double, double, double>(mean-t, mean, mean+t, cV);
            return intervals;
        }

        /// <summary>
        /// Geometric mean from a list of untransformed data . 
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public static double GeometricMean(List<double> dataList)
        {
            double sum = dataList.Aggregate<double, double>(0, (current, p) => current*p);
            return  Math.Pow(sum, 1.0 / dataList.Count);
        }

        /// <summary>
        /// Returns the smallest and the largest element from a list of ulong pause times.
        /// The Pair.First contains the smallest and Pair.Second the largest time.
        /// </summary>
        /// <param name="list">The list of doubles</param>
        /// <returns>A Pair with the smallest (First) and the largest element (Second)
        ///  from a list of ulong pause times.</returns>
        public static Pair<ulong, ulong> GetMinMax(List<ulong> list)
        {
            Pair<ulong, ulong> minMax = new Pair<ulong, ulong>();
            if (list.IsNullOrEmpty())
            {
                minMax.First = 0;
                minMax.Second = 0;
            }
            else if (list.Count < 2)
            {
                minMax.First = list[0];
                minMax.Second = list[0];
            }
            else
            {
                minMax.First = list.Min();
                minMax.Second = list.Max();
            }
            return minMax;
        }

    }
}