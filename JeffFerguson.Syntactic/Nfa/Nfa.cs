using JeffFerguson.Syntactic.RegularExpression;
using System;
using System.Collections.Generic;

namespace JeffFerguson.Syntactic.Nfa
{
    public class Nfa
    {
        public List<NfaState> States { get; private set; }
        public NfaState StartState { get; private set; }
        public NfaState EndState { get; private set; }
        private bool markNextNfaStateAsAnchoredToStartOfLine;

        /// <summary>
        /// Construct an NFA from a list of regular expression tokens.
        /// </summary>
        /// <remarks>
        /// This constructor constructs an NFA from a regular expression using an algorithm called Thompson's
        /// Construction, developed by Ken Thompson at Bell Labs for the Compatible Time-Sharing System (CTSS)
        /// version of the QED editor, which introduced regular expressions as a feature upgrade from the version
        /// written for the Berkley Timesharing System running on the SDS 940.
        /// </remarks>
        /// <param name="tokens">
        /// A list of regular expression tokens to be used as the source of the NFA construction.
        /// </param>
        internal Nfa(List<RegularExpressionToken> tokens)
        {
            InitializeNfa();
            var tokenReader = new TokenReader(tokens);
            tokenReader.Reset();
            while(tokenReader.Read() == true)
            {
                switch(tokenReader.NextToken.Class)
                {
                    case RegularExpressionToken.CharacterClass.SingleCharacter:
                    {
                        var transitionFromState = EndState;
                        var transitionToState = AppendStateToNfa();
                        transitionFromState.AddTransition(transitionToState, tokenReader.NextToken.Character);
                    }
                    break;
                    case RegularExpressionToken.CharacterClass.Wildcard:
                    {
                        var transitionFromState = EndState;
                        var transitionToState = AppendStateToNfa();
                        transitionFromState.AddTransition(transitionToState, NfaStateTransition.TransitionOptions.AllCharacters);
                    }
                    break;
                    case RegularExpressionToken.CharacterClass.StartOfLine:
                    {
                        markNextNfaStateAsAnchoredToStartOfLine = true;
                    }
                    break;
                    case RegularExpressionToken.CharacterClass.EndOfLine:
                    {
                        if(EndState != null)
                        {
                            EndState.Anchor = NfaState.AnchorOptions.AnchoredToEndOfLine;
                        }
                    }
                    break;
                    case RegularExpressionToken.CharacterClass.CharacterClassStart:
                    {
                        var characterClassTokens = ReadUntil(tokenReader, RegularExpressionToken.CharacterClass.CharacterClassEnd);
                        var characterClass = new CharacterClass(characterClassTokens);                           
                    }
                    break;
                    default:
                        throw new NotSupportedException($"Cannot construct NFA state transition for unknown token class {tokenReader.NextToken.Class}.");
                }
            }
        }

        private void InitializeNfa()
        {
            States = new List<NfaState>();
            StartState = AppendStateToNfa();
            markNextNfaStateAsAnchoredToStartOfLine = false;
        }

        private NfaState AppendStateToNfa()
        {
            var newState = new NfaState();
            if(markNextNfaStateAsAnchoredToStartOfLine == true)
            {
                newState.Anchor = NfaState.AnchorOptions.AnchoredToStartOfLine;
                markNextNfaStateAsAnchoredToStartOfLine = false;
            }
            States.Add(newState);
            EndState = newState;
            return newState;
        }

        /// <summary>
        /// Read tokens until a specified token is found.
        /// </summary>
        /// <param name="reader">
        /// The reader to use during the operation. The reader will begin reading at its current position and will
        /// not be reset.
        /// </param>
        /// <param name="readTerminator">
        /// The character class that will terminate the operation when read from the reader. The terminating character
        /// will not be included in the returned list.
        /// </param>
        /// <returns>
        /// The tokens read by the reader from the current position up to and including the token before the
        /// terminator.
        /// </returns>
        private List<RegularExpressionToken> ReadUntil(TokenReader reader, RegularExpressionToken.CharacterClass readTerminator)
        {  
            var readTokens = new List<RegularExpressionToken>();
            var keepReading = true;
            while(keepReading == true)
            {
                if(reader.Read() == false)
                {
                    keepReading = false;
                    continue;
                }
                if(reader.NextToken.Class == readTerminator)
                {
                    keepReading = false;
                    continue;
                }
                readTokens.Add(reader.NextToken);              
            }
            return readTokens;      
        }
    }
}
