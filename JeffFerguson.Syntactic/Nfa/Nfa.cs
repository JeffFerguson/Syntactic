using JeffFerguson.Syntactic.RegularExpression;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

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
        /// <para>
        /// This constructor constructs an NFA from a regular expression using an algorithm called Thompson's
        /// Construction, developed by Ken Thompson at Bell Labs for the Compatible Time-Sharing System (CTSS)
        /// version of the QED editor, which introduced regular expressions as a feature upgrade from the version
        /// written for the Berkley Timesharing System running on the SDS 940.
        /// </para>
        /// <para>
        /// This implementation is heavily inspired by the text "Compiler Design In C" by Allen I. Holub (1990).
        /// Additional comments within this implementation may reference specific pages in the text.
        /// </para>
        /// <para>
        /// This construction is recursive, which allows it to elegantly handle constructs like grouped
        /// subexpressions. The idea is that an expression is made up of one or more subexpressions, and
        /// each subexpression is created as its own NFA. Subexpression NFAs are appended to each other to
        /// create the NFA for the entire expression.
        /// </para>
        /// </remarks>
        /// <param name="tokens">
        /// A list of regular expression tokens to be used as the source of the NFA construction.
        /// </param>
        internal Nfa(List<RegularExpressionToken> tokens)
        {
            Initialize();
            var tokenReader = new TokenReader(tokens);
            var subexpressionCount = tokenReader.Subexpressions;
            if (subexpressionCount == 1)
            {
                BuildStates(tokenReader);
            }
            else
            {
                for (var subexpressionIndex = 0; subexpressionIndex < subexpressionCount; subexpressionIndex++)
                {
                    var orOperatorDetected = tokenReader.NextTokenIsOrOperator();
                    if (orOperatorDetected == true)
                    {
                        tokenReader.Advance();
                    }
                    var currentSubexpression = tokenReader.GetNextSubexpression();
                    var subexpressionNfa = new Nfa(currentSubexpression);
                    if (orOperatorDetected == true)
                    {
                        JoinAsOr(subexpressionNfa);
                    }
                    else
                    {
                        Append(subexpressionNfa);
                    }
                }
            }
        }

        /// <summary>
        /// Builds an NFA that transitions on a single character.
        /// </summary>
        /// <param name="singleCharacter">
        /// The character that will allow the NFA to transition from the start state
        /// to the end state.
        /// </param>
        internal Nfa(char singleCharacter)
        {
            Initialize();
            EndState = CreateAndAppendNewState();
            StartState.AddTransition(EndState, singleCharacter);
        }

        /// <summary>
        /// Initialize the NFA.
        /// </summary>
        private void Initialize()
        {
            States = new List<NfaState>();
            StartState = CreateAndAppendNewState();
            EndState = StartState;
            markNextNfaStateAsAnchoredToStartOfLine = false;
        }

        /// <summary>
        /// Build states for the tokens in the given token reader.
        /// </summary>
        /// <param name="tokenReader">
        /// The token reader whose tokens should be used during the build process.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The builder found an unsupported token read from the reader.
        /// </exception>
        private void BuildStates(TokenReader tokenReader)
        {
            while (tokenReader.ReadNextToken(out var readToken) == true)
            {
                switch (readToken.Class)
                {
                    case RegularExpressionToken.CharacterClass.SingleCharacter:
                        {
                            AddSingleCharacterStateAndTransition(tokenReader, readToken.Character);
                        }
                        break;
                    case RegularExpressionToken.CharacterClass.Wildcard:
                        {
                            var transitionFromState = EndState;
                            var transitionToState = CreateAndAppendNewState();
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
                            if (EndState != null)
                            {
                                EndState.Anchor = NfaState.AnchorOptions.AnchoredToEndOfLine;
                            }
                        }
                        break;
                    case RegularExpressionToken.CharacterClass.CharacterClassStart:
                        {
                            var characterClassTokens = tokenReader.ReadUntil(RegularExpressionToken.CharacterClass.CharacterClassEnd);
                            var characterClass = new CharacterClass(characterClassTokens);
                            var transitionFromState = EndState;
                            var transitionToState = CreateAndAppendNewState();
                            transitionFromState.AddTransition(transitionToState, characterClass);
                        }
                        break;
                    case RegularExpressionToken.CharacterClass.GroupingStart:
                        {
                            var groupingTokens = tokenReader.ReadUntil(RegularExpressionToken.CharacterClass.GroupingEnd);
                            var groupingNfa = new Nfa(groupingTokens);
                            if (tokenReader.NextTokenIsClosureOperator() == true)
                            {
                                tokenReader.ReadNextToken(out var closureOperator);
                                groupingNfa.ApplyClosureOperator(closureOperator);
                            }
                            Append(groupingNfa);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"Cannot construct NFA state transition for unknown token class {readToken.Class}.");
                }
            }
        }

        /// <summary>
        /// Adds a single character state and transition to the NFA.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the simple case, this method will add a new state to the end of the NFA, mark the new
        /// state as the NFA's end state, and will add the current character as one of the transitions
        /// to the new state. For example, a current single character token of "a" will be built into
        /// an existing NFA as follows:
        /// </para>
        /// <para>
        /// START [0] -> a -> [1]
        ///   END [1]
        /// </para>
        /// <para>
        /// This method will also check for any closure operators that immediately follow the single
        /// character and will build the appropriate extra states to handle the closure. If, for example,
        /// after reading the "a", a zero or more closure operator is found, as in "a*", then the NFA
        /// will be built as follows:
        /// </para>
        /// <para>
        ///       [0] -> a -> [1]
        ///       [1] -> ε -> [0]
        ///       [1] -> ε -> [3]
        /// START [2] -> ε -> [0]
        ///       [2] -> ε -> [3]
        ///   END [3]
        /// </para>
        /// </remarks>
        /// <param name="tokenReader">
        /// The token reader, which should be positioned on a single character token.
        /// </param>
        /// <param name="singleCharacter">
        /// The single character to be added.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Thrown when an unsupported closure operator is found.
        /// </exception>
        private void AddSingleCharacterStateAndTransition(TokenReader tokenReader, char singleCharacter)
        {
            var singleCharacterNfa = new Nfa(singleCharacter);
            if (tokenReader.NextTokenIsClosureOperator() == true)
            {
                tokenReader.ReadNextToken(out var closureOperator);
                singleCharacterNfa.ApplyClosureOperator(closureOperator);
            }
            Append(singleCharacterNfa);
        }

        /// <summary>
        /// Apply a closure operator to the NFA.
        /// </summary>
        /// <para>
        /// Page 83 of Holub (1990) provides illustrations of the various NFA transitions involved
        /// in the closure operator state implementations.
        /// </para>
        /// <param name="closureOperator">
        /// The closure operator to be applied to the NFA.
        /// </param>
        private void ApplyClosureOperator(RegularExpressionToken closureOperator)
        {
            var newStartState = CreateNewState();
            var newEndState = CreateNewState();
            newStartState.AddTransition(StartState);
            EndState.AddTransition(newEndState);
            switch (closureOperator.Class)
            {
                case RegularExpressionToken.CharacterClass.ZeroOrMore:
                    EndState.AddTransition(StartState);
                    newStartState.AddTransition(newEndState);
                    break;
                case RegularExpressionToken.CharacterClass.ZeroOrOne:
                    newStartState.AddTransition(newEndState);
                    break;
                case RegularExpressionToken.CharacterClass.OneOrMore:
                    EndState.AddTransition(StartState);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported closure operator {closureOperator}.");
            }
            StartState = newStartState;
            EndState = newEndState;
        }

        /// <summary>
        /// Append a given NFA to the current NFA.
        /// </summary>
        /// <param name="otherNfa">
        /// The NFA to append to the current NFA.
        /// </param>
        private void Append(Nfa otherNfa)
        {
            if (StartState == EndState)
            {
                States = otherNfa.States;
                StartState = otherNfa.StartState;
                EndState = otherNfa.EndState;
                markNextNfaStateAsAnchoredToStartOfLine = otherNfa.markNextNfaStateAsAnchoredToStartOfLine;
            }
            else
            {
                foreach (var currentStateInOtherNfa in otherNfa.States)
                {
                    if (currentStateInOtherNfa != otherNfa.StartState)
                    {
                        States.Add(currentStateInOtherNfa);
                    }
                }
                EndState.Transition1 = otherNfa.StartState.Transition1;
                EndState.Transition2 = otherNfa.StartState.Transition2;
                EndState = otherNfa.EndState;
            }
        }

        /// <summary>
        /// Join the given NFA to the current NFA in such a way that it supports an Or
        /// construct.
        /// </summary>
        /// <param name="otherNfa">
        /// The NFA to join to the current NFA.
        /// </param>
        private void JoinAsOr(Nfa otherNfa)
        {
            foreach (var currentStateInOtherNfa in otherNfa.States)
            {
                States.Add(currentStateInOtherNfa);
            }
            var newStartState = CreateNewState();
            var newEndState = CreateNewState();
            newStartState.AddTransition(this.StartState);
            newStartState.AddTransition(otherNfa.StartState);
            EndState.AddTransition(newEndState);
            otherNfa.EndState.AddTransition(newEndState);
            StartState = newStartState;
            EndState = newEndState;
        }

        /// <summary>
        /// Create a new NFA state.
        /// </summary>
        /// <remarks>
        /// This method simply creates the new state. No changes are made to the overall
        /// state of the NFA other than the creation of the new NFA state. In particular,
        /// the NFA Start and End states remain unchanged.
        /// </remarks>
        /// <returns>
        /// The newly created NFA state.
        /// </returns>
        private NfaState CreateNewState()
        {
            var newState = new NfaState();
            States.Add(newState);
            return newState;
        }

        /// <summary>
        /// Create a new NFA state and append it to the NFA.
        /// </summary>
        /// <returns>
        /// The newly created NFA state.
        /// </returns>
        private NfaState CreateAndAppendNewState()
        {
            var newState = CreateNewState();
            if (markNextNfaStateAsAnchoredToStartOfLine == true)
            {
                newState.Anchor = NfaState.AnchorOptions.AnchoredToStartOfLine;
                markNextNfaStateAsAnchoredToStartOfLine = false;
            }
            EndState = newState;
            return newState;
        }

        /// <summary>
        /// Repoint any transitions originally pointing to a destination state to a different
        /// destination state.
        /// </summary>
        /// <param name="originalDestinationState">
        /// The original destination state.
        /// </param>
        /// <param name="newDestinationState">
        /// The new destination state. Any transitions that pointed to the original destination
        /// state will be repointed to this destination state instead.
        /// </param>
        private void Repoint(NfaState originalDestinationState, NfaState newDestinationState)
        {
            foreach (var currentState in States)
            {
                if (currentState.Transition1 != null)
                {
                    if (currentState.Transition1.ToState == originalDestinationState)
                    {
                        currentState.Transition1.ToState = newDestinationState;
                    }
                }
                if (currentState.Transition2 != null)
                {
                    if (currentState.Transition2.ToState == originalDestinationState)
                    {
                        currentState.Transition2.ToState = newDestinationState;
                    }
                }
            }
        }
    }
}
