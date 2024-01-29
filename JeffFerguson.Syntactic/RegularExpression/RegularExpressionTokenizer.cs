using System;
using System.Collections.Generic;

namespace JeffFerguson.Syntactic.RegularExpression
{
    internal class RegularExpressionTokenizer
    {
        private const char Wildcard = '.';
        private const char StartOfLine = '^';
        private const char EndOfLine = '$';
        private const char CharacterClassStart = '[';
        private const char CharacterClassEnd = ']';
        private const char NegateCharacterClass = '#'; // usually ^ but that clashes with StartOfLine
        private const char ZeroOrMore = '*';
        private const char OneOrMore = '+';
        private const char ZeroOrOne = '?';
        private const char Or = '|';
        private const char GroupingStart = '(';
        private const char GroupingEnd = ')';
        private const char NextCharIsLiteral = '\\';
        private const char CharacterRange = '-';
        private const char NextCharsAreLiteralStart = '"';
        private const char NextCharsAreLiteralEnd = '"';

        internal List<RegularExpressionToken> Tokens { get; private set; }

        internal void Tokenize(string regularExpression)
        {
            Tokens = new List<RegularExpressionToken>();
            var nextCharIsLiteral = false;
            var lookForLiteralEnd = false;

            Tokens.Capacity = regularExpression.Length;
            foreach(var currentCharacter in regularExpression)
            {
                if (nextCharIsLiteral == true)
                {
                    AddSingleCharacter(Tokens, currentCharacter);
                    nextCharIsLiteral = false;
                    continue;
                }
                if (lookForLiteralEnd == true)
                {
                    if(currentCharacter == NextCharsAreLiteralEnd)
                    {
                        lookForLiteralEnd = false;
                    }
                    else
                    {
                        AddSingleCharacter(Tokens, currentCharacter);
                    }
                    continue;
                }
                switch(currentCharacter)
                {
                    case Wildcard:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.Wildcard);
                        break;
                    case StartOfLine:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.StartOfLine);
                        break;
                    case EndOfLine:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.EndOfLine);
                        break;
                    case CharacterClassStart:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.CharacterClassStart);
                        break;
                    case CharacterClassEnd:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.CharacterClassEnd);
                        break;
                    case NegateCharacterClass:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.NegateCharacterClass);
                        break;
                    case ZeroOrMore:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.ZeroOrMore);
                        break;
                    case OneOrMore:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.OneOrMore);
                        break;
                    case ZeroOrOne:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.ZeroOrOne);
                        break;
                    case Or:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.Or);
                        break;
                    case GroupingStart:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.GroupingStart);
                        break;
                    case GroupingEnd:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.GroupingEnd);
                        break;
                    case CharacterRange:
                        AddToken(Tokens, RegularExpressionToken.CharacterClass.CharacterRange);
                        break;
                    case NextCharsAreLiteralStart:
                        lookForLiteralEnd = true;
                        break;
                    case NextCharIsLiteral:
                        nextCharIsLiteral = true;
                        break;
                    default:
                        AddSingleCharacter(Tokens, currentCharacter);
                        break;
                }
            }
            Tokens.TrimExcess();
        }

        /// <summary>
        /// Adds a token to a list of regular expression tokens.
        /// </summary>
        /// <param name="tokens">
        /// The list of regular expreswsion tokens to which the single character should be added.
        /// </param>
        /// <param name="tokenClassToAdd">
        /// The token to add to the list.
        /// </param>
        private void AddToken(List<RegularExpressionToken> tokens, RegularExpressionToken.CharacterClass tokenClassToAdd)
        {
            tokens.Add(new RegularExpressionToken
                {
                    Class = tokenClassToAdd,
                    Character = (char)0x00
                }
            );
        }

        /// <summary>
        /// Adds a single character to a list of regular expression tokens.
        /// </summary>
        /// <param name="tokens">
        /// The list of regular expression tokens to which the single character should be added.
        /// </param>
        /// <param name="currentCharacter">
        /// The character to add to the list.
        /// </param>
        private void AddSingleCharacter(List<RegularExpressionToken> tokens, char currentCharacter)
        {
            tokens.Add(new RegularExpressionToken
                {
                    Class = RegularExpressionToken.CharacterClass.SingleCharacter,
                    Character = currentCharacter
                }
            );
        }
    }
}
