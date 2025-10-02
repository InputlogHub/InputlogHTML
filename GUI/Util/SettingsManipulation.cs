using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using InputLog.Core.Util.KeyConversion;
using log4net;

namespace GUI.Util
{
    /// <summary>
    /// This class provides some helper methods to do manipulation of the Settings.
    /// Common tasks include serializing and deserializing certain objects.
    /// (Developer Note: Although this can be done by using VS' own mechanisms (implement serialization interfaces),
    /// doing so is not always without a hassle. For simple classes (and things like enumerations), 
    /// it is easier to just add some logic to this class).
    /// </summary>
    public static class SettingsManipulation
    {
        #region Fields

        /// <summary>
        /// Log4Net MessageLogger.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        /// <summary>
        /// Deserializes a given string collection to a list of keys.
        /// </summary>
        /// <returns>List of keys that was represented by the given string collection.</returns>
        /// <exception cref="ArgumentNullException">If stringCollection is null.</exception>
        public static List<KeysEx> DeserializeGroupedKeyList(StringCollection stringCollection)
        {
            if (stringCollection == null) throw new ArgumentNullException("stringCollection"
                , "The passed string collection cannot be null");
            List<KeysEx> returnList = new List<KeysEx>();
            foreach (string keyString in stringCollection)
            {
                // This extra check is a precaution: it has happened in the past that empty strings are passed to this function
                // which would normally result in an exception. There was a particularly nasty bug in which many spaces where 
                // passed due to a wrong configuration of the default settings in the settings file. This led to 
                // terrible performance. This additional check (IsNullOrWhiteSpace) ensures that if such a misconfiguration 
                // should happen again, it will not be problematic.
                if (!String.IsNullOrWhiteSpace(keyString))
                {
                    try
                    {
                        returnList.Add((KeysEx)Enum.Parse(typeof(KeysEx), keyString, true));
                    }
                    catch (Exception e)
                    {
                        // log, do not take further action (user should not be notified of an exception occuring here).
                        log.Warn("Unable to read a key from the grouped keys", e);
                    }
                }
            }
            return returnList;
        }

        /// <summary>
        /// Serializes a given list of keys to a string collection.
        /// </summary>
        /// <param name="keys">List of keys to store.</param>
        /// <returns>A String collection representing the given list of keys.</returns>
        /// <exception cref="ArgumentNullException">If keys is null.</exception>
        public static StringCollection SerializeGroupedKeyList(List<KeysEx> keys)
        {
            if (keys == null) throw new ArgumentNullException("keys", " The passed list of keys cannot be null");
            StringCollection stringCollection = new StringCollection();
            foreach (KeysEx key in keys)
            {
                stringCollection.Add(key.ToString());
            }
            return stringCollection;
        }

        //TODO the following code has not been used nor tested yet
        ///// <summary>
        ///// Serializes a given dictionary to a string collection. 
        //// The Key and Values in the stringDictionary must have a proper implemented ToString() method.
        ///// </summary>
        ///// <param name="dictionary">Dictionary to store.</param>
        ///// <returns>A String collection representing the given dictionary.</returns>
        ///// <exception cref="ArgumentNullException">If keys is null.</exception>
        //public static StringCollection SerializeDictionary(Dictionary<Object, Object> dictionary) {
        //    if (dictionary == null) throw new ArgumentNullException("dictionary", " The passed dictionary cannot be null");
        //    StringCollection stringCollection = new StringCollection();
        //    foreach (Object key in dictionary.Keys) {
        //        stringCollection.Add(key.ToString());
        //        stringCollection.Add(dictionary[key].ToString());
        //    }
        //    return stringCollection;
        //}

        ///// <summary>
        ///// Serializes a given dictionary to a string collection. 
        //// The Key and Values in the stringDictionary must have a proper implemented ToString() method.
        ///// </summary>
        ///// <param name="dictionary">Dictionary to store.</param>
        ///// <returns>A String collection representing the given dictionary.</returns>
        ///// <exception cref="ArgumentNullException">If keys is null.</exception>
        //public static Dictionary<K, V> DerializeDictionary<K, V>(StringCollection stringCollection) {
        //    if (stringCollection == null) throw new ArgumentNullException("stringCollection", 
        //           + "The passed string collection cannot be null");
        //    if ((stringCollection.Count % 2) != 0) throw new ArgumentException("stringCollection", 
        //    "The passed string collection must contain an even number of strings");
        //    Dictionary<K, V> dictionary = new Dictionary<K, V>();
        //    for (int i = 0; i < stringCollection.Count; i += 2) {
        //        try {
        //            dictionary[(K)ParseString<K>(stringCollection[i])] = (V)ParseString<V>(stringCollection[i + 1]);
        //        } catch (FormatException) {
        //            // failed to parse string, continue with other strings
        //        }
        //    }
        //    return dictionary;
        //}

        ///// <summary>
        ///// Parses a string to a object of type T. Currently supports types String and Int32.
        ///// </summary>
        ///// <typeparam name="T">Type to parse string to.</typeparam>
        ///// <param name="str">String to parse (the string should represent an object of type T).</param>
        ///// <returns>an object of type T, parsed from the given string</returns>
        //private static Object ParseString<T>(string str) {
        //    Type type = typeof(T);
        //    if (type.Equals(typeof(String))) {
        //        return str;
        //    } else if (type.Equals(typeof(Int32))) {
        //        return Int32.Parse(str);
        //    }
        //    return null;
        //}
    }
}