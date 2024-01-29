namespace JeffFerguson.Syntactic.Nfa
{
    /// <summary>
    /// A class used to represent a range of characters.
    /// </summary>
    /// <remarks>
    /// This class represents a character range typically found in regular expressions. For example, a regular expression
    /// might include a sub-expression such as "0-9", which specifies a range of characters from "0" to "9" inclusive, which
    /// specifies a range of characters including all digits. This sub-expression will be represented by an object of this
    /// class, with "0" as the first character in the range and "9" as the last character in the range.
    /// </remarks>
    internal class CharacterRange
    {
        internal char FirstCharacterInRange;
        internal char LastCharacterInRange;

        /// <summary>
        /// Specifies if the supplied character is in the range of characters specified by the object.
        /// </summary>
        /// <param name="characterToTest">
        /// The character be tested.
        /// </param>
        /// <returns>
        /// True if the character is within the range of characters, and false otherwise.
        /// </returns>
        internal bool IsCharacterInRange(char characterToTest)
        {
            if((characterToTest >= FirstCharacterInRange) && (characterToTest <= LastCharacterInRange))
            {
                return true;
            }
            return false;
        }
    }
}