using System.Collections.Generic;
using JeffFerguson.Syntactic.RegularExpression;

namespace JeffFerguson.Syntactic.Nfa
{
    internal class CharacterClass
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
            while(reader.Read() == true)
            {
                if(checkedForNegativeClass == false)
                {
                    checkedForNegativeClass = true;
                    if(reader.NextToken.Class == RegularExpressionToken.CharacterClass.NegateCharacterClass)
                    {
                        negativeCharacterClass = true;
                        continue;
                    }                   
                }
                reader.Peek(out var tokenAferCurrentToken);
                if(tokenAferCurrentToken.Class == RegularExpressionToken.CharacterClass.CharacterRange)
                {
                    var newRange = new CharacterRange();
                    newRange.FirstCharacterInRange = reader.NextToken.Character;
                    reader.Read();
                    reader.Read();
                    newRange.LastCharacterInRange = reader.NextToken.Character;
                    characterClassRanges.Add(newRange);
                }
                else
                {
                    singleCharacters.Add(reader.NextToken.Character);
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