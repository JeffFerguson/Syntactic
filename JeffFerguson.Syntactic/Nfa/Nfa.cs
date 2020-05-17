using JeffFerguson.Syntactic.RegularExpression;
using System;
using System.Collections.Generic;

namespace JeffFerguson.Syntactic.Nfa
{
    internal class Nfa
    {
        private List<NfaState> states;
        private NfaState startState;
        private NfaState endState;

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
            foreach (var currentToken in tokens)
            {
                switch(currentToken.Class)
                {
                    case RegularExpressionToken.CharacterClass.SingleCharacter:
                        {
                            var transitionFromState = endState;
                            var transitionToState = AppendStateToNfa();
                            transitionFromState.AddTransition(transitionToState, currentToken.Character);
                        }
                        break;
                    case RegularExpressionToken.CharacterClass.Wildcard:
                        {
                            var transitionFromState = endState;
                            var transitionToState = AppendStateToNfa();
                            transitionFromState.AddTransition(transitionToState, NfaStateTransition.TransitionOptions.AllCharacters);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"Cannot construct NFA state transition for unknown token class {currentToken.Class}.");
                }
            }
        }

        private void InitializeNfa()
        {
            states = new List<NfaState>();
            startState = AppendStateToNfa();
        }

        private NfaState AppendStateToNfa()
        {
            var newState = new NfaState();
            states.Add(newState);
            endState = newState;
            return newState;
        }
    }
}
