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
        /// The next token available for reading after Read() is called successfully.
        /// </summary>
        internal RegularExpressionToken NextToken { get; private set; }

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
        /// <returns>
        /// True if the next token was read; false if there are no more tokens to read.
        /// </returns>
        internal bool Read()
        {
            if(indexOfNextTokenToRead == tokensToRead.Count)
            {
                return false;
            }
            NextToken = tokensToRead[indexOfNextTokenToRead];
            indexOfNextTokenToRead++;
            return true;
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
            if(indexOfNextTokenToRead == tokensToRead.Count)
            {
                peekToken = RegularExpressionToken.NullToken;
                return false;
            }
            peekToken = tokensToRead[indexOfNextTokenToRead];
            return true;
        }
    }
}