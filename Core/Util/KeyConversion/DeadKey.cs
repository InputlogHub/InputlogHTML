using System.Collections.Generic;

namespace InputLog.Core.Util.KeyConversion
{
    /// <summary>
    /// This class represents a deadkey and contains all the possible
    /// characters that can be combined with it (together with the
    /// resulting character).
    /// </summary>
    public class DeadKey
    {
        #region Fields
        /// <summary>
        /// Maps each base char for which this char acts a diacritic to the combined char.
        /// </summary>
        private readonly Dictionary<char, char> CharMap = new Dictionary<char, char>();

        /// <summary>
        /// The actual character the instance represents.
        /// </summary>
        public char DeadCharacter { get; private set; }

        /// <summary>
        /// Returns the number of character to which this DeadKey can act as a diacritic.
        /// </summary>
        public int Count { get { return CharMap.Count; } }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="deadCharacter">The actual character this instance represents.</param>
        public DeadKey(char deadCharacter)
        {
            DeadCharacter = deadCharacter;
        }

        /// <summary>
        /// Adds a character to the set of characters to which this DeadKey can act
        /// as a diacritic.
        /// </summary>
        /// <param name="baseCharacter">The unmodified character.</param>
        /// <param name="combinedCharacter">The character combined with the DeadKey.</param>
        public void AddBaseChar(char baseCharacter, char combinedCharacter)
        {
            CharMap[baseCharacter] = combinedCharacter;
        }

        /// <summary>
        /// Returns true if the DeadKey can act as a diacritic to the given base character.
        /// </summary>
        /// <param name="baseCharacter">The character where to check it for.</param>
        /// <returns>True if the DeadKey can act as a diacritic to the given base character.</returns>
        public bool ContainsBaseCharacter(char baseCharacter)
        {
            return CharMap.ContainsKey(baseCharacter);
        }

        /// <summary>
        /// Returns the combined character of the DeadKey and the given base character.
        /// If they cannot be combined into one character, the given base character is
        /// appended after the DeadKey character and returned.
        /// </summary>
        /// <param name="baseChar">The character where to get the combined key for.</param>
        /// <returns>String containing either only the combined character or, if no such
        /// character exists, baseChar appended to this.DeadCharacter.</returns>
        public string GetCombinedCharacter(char baseChar)
        {
            string result;

            if (CharMap.ContainsKey(baseChar))
            {
                result = CharMap[baseChar].ToString();
            }
            else
            {
                result = DeadCharacter + baseChar.ToString();
            }

            return result;
        }
    }
}