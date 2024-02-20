using System;
using System.Collections.Generic;
using JeffFerguson.Syntactic.RegularExpression;

namespace JeffFerguson.Syntactic.Nfa
{
    /// <summary>
    /// A class to read regular expression tokens from a defined list of tokens.
    /// </summary>
    /// <remarks>
    /// The token reader is a forward-only reader of a set of tokens. The reader keeps the notion
    /// of a "current position", and the "currently read" token is the token at the reader's current
    /// position. The current position starts at 0 and advances by one each time a token is read.
    /// Call Read() to read the current token. The current token will be made available in a
    /// property called "NextToken". The call to Read() will return false when no more tokens are
    /// available to be read. A call to Reset() will reset the current position to 0.
    /// </remarks>
    internal class TokenReader
    {
        private List<RegularExpressionToken> tokensToRead;
        private int indexOfNextTokenToRead;

        /// <summary>
        /// Gets the number of subexpressions found in the expression managed by the token reader.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property will return the count for all subexpressions regardless of the current
        /// position of the reader.
        /// </para>
        /// <para>
        /// This property can be called at any time. The current position of the reader when the
        /// property is called will be retained after the call and will not be modified.
        /// </para>
        /// </remarks>
        internal int Subexpressions
        {
            get
            {
                var savedPosition = indexOfNextTokenToRead;
                bool keepLookingForSubexpressions = true;
                var subexpressionCount = 0;
                while (keepLookingForSubexpressions == true)
                {
                    var subexpression = GetNextSubexpression();
                    if (subexpression.Count == 0)
                    {
                        keepLookingForSubexpressions = false;
                    }
                    else
                    {
                        subexpressionCount++;
                    }
                }
                indexOfNextTokenToRead = savedPosition;
                return subexpressionCount;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tokens">
        /// The list of tokens to be read by the reader.
        /// </param>
        internal TokenReader(List<RegularExpressionToken> tokens)
        {
            tokensToRead = tokens;
        }

        /// <summary>
        ///  Resets the reader so that, when the next read occurs, the first token is read.
        /// </summary>
        internal void Reset()
        {
            indexOfNextTokenToRead = 0;
        }

        /// <summary>
        /// Advance the reader by one token without returning the token that was skipped over.
        /// </summary>
        internal void Advance()
        {
            if (indexOfNextTokenToRead < tokensToRead.Count)
            {
                indexOfNextTokenToRead++;
            }
        }

        /// <summary>
        /// Reads the next token.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the read operation is successful, the next token to read will be made available 
        /// in the reader's "NextToken" property.
        /// </para>
        /// <para>
        /// If the read operation is unsuccessful, then the reader can be reused to read the
        /// token list if Reset() is called.
        /// </para>
        /// </remarks>
        /// <param name="readToken">
        /// The next token, or a null token if no more tokens are available.
        /// </param>
        /// <returns>
        /// True if the next token was read; false if there are no more tokens to read.
        /// </returns>
        internal bool ReadNextToken(out RegularExpressionToken readToken)
        {
            if (indexOfNextTokenToRead == tokensToRead.Count)
            {
                readToken = RegularExpressionToken.NullToken;
                return false;
            }
            readToken = tokensToRead[indexOfNextTokenToRead];
            indexOfNextTokenToRead++;
            return true;
        }

        /// <summary>
        /// Read tokens until a specified token is found.
        /// </summary>
        /// <param name="readTerminator">
        /// The character class that will terminate the operation when read from the reader. The terminating character
        /// will not be included in the returned list.
        /// </param>
        /// <returns>
        /// The tokens read by the reader from the current position up to and including the token before the
        /// terminator.
        /// </returns>
        internal List<RegularExpressionToken> ReadUntil(RegularExpressionToken.CharacterClass readTerminator)
        {
            var readTokens = new List<RegularExpressionToken>();
            var keepReading = true;
            while (keepReading == true)
            {
                if (ReadNextToken(out var readToken) == false)
                {
                    keepReading = false;
                    continue;
                }
                if (readToken.Class == readTerminator)
                {
                    keepReading = false;
                    continue;
                }
                readTokens.Add(readToken);
            }
            return readTokens;
        }

        /// <summary>
        /// Returns the next character to be returned after the next call to Read() without actually
        /// advancing the read pointer.
        /// </summary>
        /// <param name="peekToken">
        /// The peeked token, or a null token if no more tokens are available.
        /// </param>
        /// <returns>
        /// True if a token was output, or false if there are no more tokens available.
        /// </returns>
        internal bool Peek(out RegularExpressionToken peekToken)
        {
            if (indexOfNextTokenToRead == tokensToRead.Count)
            {
                peekToken = RegularExpressionToken.NullToken;
                return false;
            }
            peekToken = tokensToRead[indexOfNextTokenToRead];
            return true;
        }

        /// <summary>
        /// Gets the next subexpression from the token reader.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A subexpression, for the purposes of this code, is one of the following items, followed
        /// optionally by a closure operator:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// a single character
        /// </item>
        /// <item>
        /// a grouping
        /// </item>
        /// <item>
        /// a character class
        /// </item>
        /// </list>
        /// <para>
        /// For example, the regular expression "ab+c" has three subexpressions:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// a
        /// </item>
        /// <item>
        /// b+
        /// </item>
        /// <item>
        /// c
        /// </item>
        /// </list>
        /// <para>
        /// The idea of subexpressions when building an NFA is that an NFA is built for a
        /// subexpression, which is then appended to the final NFA that will represent the
        /// NFA for the entire regular expression. Returning to the regular expression
        /// "ab+c" for a moment, the NFA construction is built as follows:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// Build an NFA for the subexpression "a" and append it to the NFA that repesents
        /// the entire regular expression (which does not exist since this is the first
        /// subexpression to be processed)
        /// </item>
        /// <item>
        /// Build an NFA for the subexpression "b+" and append it to the NFA that repesents
        /// the entire regular expression (which, at this point, has an NFA that represents
        /// "a")
        /// </item>
        /// <item>
        /// Build an NFA for the subexpression "c" and append it to the NFA that repesents
        /// the entire regular expression (which, at this point, has an NFA that represents
        /// "ab+")
        /// </item>
        /// </list>
        /// <para>
        /// The OR operator is not part of a subexpression. The general handling of the OR
        /// operator is that the presense of the OR operator is noted, and the subexpression
        /// to the right of the OR operator is processed. Once that processing is complete,
        /// the states needed for the OR operator is added to the combination of the two
        /// subexpressions. For example, the regular expression "a|b+" is handled as follows:
        /// <list type="bullet">
        /// <item>
        /// Build an NFA for the subexpression "a" and append it to the NFA that repesents
        /// the entire regular expression (which does not exist since this is the first
        /// subexpression to be processed)
        /// </item>
        /// <item>
        /// Note the presense of the OR operator and skip over its token
        /// </item>
        /// <item>
        /// Build an NFA for the subexpression "b+"
        /// </item>
        /// <item>
        /// Note that the "right side" processing of the OR operator is complete and build
        /// the additional states needed for an OR machine, using the "a" and the "b+"
        /// subexpression state machines to merge into the various parts of the OR machine
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// Since the Or operator is skipped over during subexpression retrieval, it will not appear
        /// in any subexpressions and its presense must be considered between any two subexpressions.
        /// Returning to the example of the regular expression "a|b", the expression has two
        /// subexpressions:
        /// <list type="bullet">
        /// <item>
        /// a
        /// </item>
        /// <item>
        /// b
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// Any code that is processing subexpressions must consider the possitiblity that an Or
        /// operator exists between the two subexpressions. This is not necessarily the case, of
        /// course, since both regular expressions "ab" and "a|b" have the same two subexpressions,
        /// but the relationship between the two subexpressions must be considered in either case.
        /// </para>
        /// <para>
        /// Start-of-line and end-of-line anchors are considered to be part of a subexpression.
        /// </para>
        /// </remarks>
        /// <returns>
        /// A list of tokens representing the next subexpression. The list will be empty
        /// if no subexpression is available.
        /// </returns>
        internal List<RegularExpressionToken> GetNextSubexpression()
        {
            var subexpression = new List<RegularExpressionToken>();
            var readToken = RegularExpressionToken.NullToken;

            if (NextTokenIsAnchor() == true)
            {
                ReadNextToken(out readToken);
                subexpression.Add(readToken);
            }
            if (NextTokenIsOrOperator() == true)
            {
                ReadNextToken(out readToken); // skip over the OR and throw it away
            }
            if (ReadNextToken(out readToken) == false)
            {
                return subexpression;
            }
            switch (readToken.Class)
            {
                case RegularExpressionToken.CharacterClass.SingleCharacter:
                case RegularExpressionToken.CharacterClass.Wildcard:
                case RegularExpressionToken.CharacterClass.StartOfLine:
                case RegularExpressionToken.CharacterClass.EndOfLine:
                    subexpression.Add(readToken);
                    break;
                case RegularExpressionToken.CharacterClass.CharacterClassStart:
                    subexpression.Add(readToken);
                    var characterClassSubexpression = ReadUntil(RegularExpressionToken.CharacterClass.CharacterClassEnd);
                    foreach (var readUntilToken in characterClassSubexpression)
                    {
                        subexpression.Add(readUntilToken);
                    }
                    subexpression.Add(new RegularExpressionToken { Class = RegularExpressionToken.CharacterClass.CharacterClassEnd, Character = (char)0x00 });
                    break;
                case RegularExpressionToken.CharacterClass.GroupingStart:
                    subexpression.Add(readToken);
                    var groupingSubexpression = ReadUntil(RegularExpressionToken.CharacterClass.GroupingEnd);
                    foreach (var readUntilToken in groupingSubexpression)
                    {
                        subexpression.Add(readUntilToken);
                    }
                    subexpression.Add(new RegularExpressionToken { Class = RegularExpressionToken.CharacterClass.GroupingEnd, Character = (char)0x00 });
                    break;
                case RegularExpressionToken.CharacterClass.Or:
                    break;
                default:
                    throw new NotSupportedException($"Unhandled character class {readToken.Class} found while getting the next subexpression.");
            }
            if (NextTokenIsClosureOperator() == true)
            {
                ReadNextToken(out readToken);
                subexpression.Add(readToken);
            }
            if (NextTokenIsAnchor() == true)
            {
                ReadNextToken(out readToken);
                subexpression.Add(readToken);
            }
            return subexpression;
        }

        /// <summary>
        /// Take a peek at the next token and determine whether or not the token is a
        /// closure operator.
        /// </summary>
        /// <returns>
        /// True if the next token is a closure operator; false otherwise.
        /// </returns>
        internal bool NextTokenIsClosureOperator()
        {
            if (Peek(out var peekedToken) == false)
            {
                return false;
            }
            if (peekedToken.Class == RegularExpressionToken.CharacterClass.ZeroOrMore)
            {
                return true;
            }
            if (peekedToken.Class == RegularExpressionToken.CharacterClass.ZeroOrOne)
            {
                return true;
            }
            if (peekedToken.Class == RegularExpressionToken.CharacterClass.OneOrMore)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Take a peek at the next token and determine whether or not the token is the
        /// Or operator.
        /// </summary>
        /// <returns>
        /// True if the next token is the Or operator; false otherwise.
        /// </returns>
        internal bool NextTokenIsOrOperator()
        {
            if (Peek(out var peekedToken) == false)
            {
                return false;
            }
            if (peekedToken.Class == RegularExpressionToken.CharacterClass.Or)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Take a peek at the next token and determine whether or not the token is an
        /// anchor.
        /// </summary>
        /// <returns>
        /// True if the next token is a anchor; false otherwise.
        /// </returns>
        internal bool NextTokenIsAnchor()
        {
            if (Peek(out var peekedToken) == false)
            {
                return false;
            }
            if (peekedToken.Class == RegularExpressionToken.CharacterClass.StartOfLine)
            {
                return true;
            }
            if (peekedToken.Class == RegularExpressionToken.CharacterClass.EndOfLine)
            {
                return true;
            }
            return false;
        }
    }
}