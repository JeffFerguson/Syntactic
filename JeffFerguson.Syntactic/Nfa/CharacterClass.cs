using System.Collections.Generic;
using JeffFerguson.Syntactic.RegularExpression;

namespace JeffFerguson.Syntactic.Nfa
{
    public class CharacterClass
    {
        private bool negativeCharacterClass;
        private List<char> singleCharacters;
        private List<CharacterRange> characterClassRanges;

        internal CharacterClass()
        {
            Initialize();
        }

        internal CharacterClass(List<RegularExpressionToken> tokens)
        {
            Initialize();
            var reader = new TokenReader(tokens);
            var checkedForNegativeClass = false;
            var readToken = RegularExpressionToken.NullToken;
            while (reader.ReadNextToken(out readToken) == true)
            {
                if (checkedForNegativeClass == false)
                {
                    checkedForNegativeClass = true;
                    if (readToken.Class == RegularExpressionToken.CharacterClass.NegateCharacterClass)
                    {
                        negativeCharacterClass = true;
                        continue;
                    }
                }
                reader.Peek(out var tokenAferCurrentToken);
                if (tokenAferCurrentToken.Class == RegularExpressionToken.CharacterClass.CharacterRange)
                {
                    var newRange = new CharacterRange();
                    newRange.FirstCharacterInRange = readToken.Character;
                    reader.ReadNextToken(out readToken);
                    reader.ReadNextToken(out readToken);
                    newRange.LastCharacterInRange = readToken.Character;
                    characterClassRanges.Add(newRange);
                }
                else
                {
                    singleCharacters.Add(readToken.Character);
                }
            }
        }

        private void Initialize()
        {
            negativeCharacterClass = false;
            singleCharacters = new List<char>();
            characterClassRanges = new List<CharacterRange>();
        }
    }
}