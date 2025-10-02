using System;

namespace GUI.Util
{
    /// <summary>
    /// Class providing helper methods for common input validation tasks.
    /// </summary>
    public static class InputValidation
    {
        /// <summary>
        /// Asserts that a given string can be parsed to a given type.
        /// Supported types: int, uint, short, ushort, long, ulong, double, float, char.
        /// If the string cannot be parsed, an exception is handed to MessageLogger
        /// together with the given message.
        /// </summary>
        /// <param name="str">String to try and parse.</param>
        /// <param name="min">The optional minimum value</param>
        /// <param name="max">Optional maximum value</param>
        /// <returns>true if the given string can be parsed to a long, int, etc. false otherwise.</returns>
        public static bool AssertParse<T>(String str, ulong min = ulong.MinValue, ulong max = ulong.MaxValue)
        {
            
            try
            {
                if (typeof(T) == typeof(int))
                {
                    int i = int.Parse(str);
                    return i >= (int) min && i <= (int) max;
                }
                if (typeof(T) == typeof(uint))
                {
                    uint i = uint.Parse(str);
                    return i >= min && i <= max;
                }
                if (typeof(T) == typeof(short))
                {
                    short i = short.Parse(str);
                    return i >= (short) min && i <= (short) max;
                }
                if (typeof(T) == typeof(ushort))
                {
                    ushort i = ushort.Parse(str);
                    return i >= min && i <= max;
                }
                if (typeof(T) == typeof(long))
                {
                    long i = long.Parse(str);
                    return i >= (long) min && i <= (long) max;
                }
                if (typeof(T) == typeof(ulong))
                {
                    ulong i = ulong.Parse(str);
                    return i >= min && i <= max;
                }
                if (typeof(T) == typeof(double))
                {
                    double i = double.Parse(str);
                    return i >= min && i <= max;
                }
                if (typeof(T) == typeof(float))
                {
                    float i = float.Parse(str);
                    return i >= min && i <= max;
                }
                if (typeof(T) == typeof(char))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
    }
}