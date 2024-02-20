using System;

namespace JeffFerguson.Syntactic.Nfa
{
    /// <summary>
    /// A representation of a single state for an NFA.
    /// </summary>
    public class NfaState
    {
        public enum AnchorOptions
        {
            NotAnchored = 0,
            AnchoredToStartOfLine,
            AnchoredToEndOfLine
        }

        public NfaStateTransition Transition1 { get; internal set; }
        public NfaStateTransition Transition2 { get; internal set; }

        public AnchorOptions Anchor { get; set; }

        private Guid id;

        /// <summary>
        /// Construct a new NFA state.
        /// </summary>
        internal NfaState()
        {
            Transition1 = null;
            Transition2 = null;
            Anchor = AnchorOptions.NotAnchored;
            id = Guid.NewGuid();
        }

        /// <summary>
        /// Add a transition from the current state to the given state. The transition will
        /// not be associated with a character or character class. The transition will be
        /// considered "empty" (also knows as an "epsilon" transition).
        /// </summary>
        /// <param name="transitionToState">
        /// The state to which the transition should point.
        /// </param>
        internal void AddTransition(NfaState transitionToState)
        {
            AddTransition(transitionToState, NfaStateTransition.TransitionOptions.Epsilon);
        }

        /// <summary>
        /// Add a transition from the current state to the given state. The transition will
        /// be associated with the given character.
        /// </summary>
        /// <param name="transitionToState">
        /// The state to which the transition should point.
        /// </param>
        /// <param name="singleCharacter">
        /// The single character associated with the transition.
        /// </param>
        internal void AddTransition(NfaState transitionToState, char singleCharacter)
        {
            var newTransition = new NfaStateTransition(transitionToState, singleCharacter);
            AddExistingTransitionToState(newTransition);
        }

        /// <summary>
        /// Add a transition from the current state to the given state. The transition will
        /// be associated with the given character class.
        /// </summary>
        /// <param name="transitionToState">
        /// The state to which the transition should point.
        /// </param>
        /// <param name="characterClass">
        /// The character class associated with the transition.
        /// </param>
        internal void AddTransition(NfaState transitionToState, CharacterClass characterClass)
        {
            var newTransition = new NfaStateTransition(transitionToState, characterClass);
            AddExistingTransitionToState(newTransition);
        }

        /// <summary>
        /// Add a transition from the current state to the given state. The transition will
        /// be associated with the given transition options.
        /// </summary>
        /// <param name="transitionToState">
        /// The state to which the transition should point.
        /// </param>
        /// <param name="option">
        /// The transition options associated with the transition.
        /// </param>
        internal void AddTransition(NfaState transitionToState, NfaStateTransition.TransitionOptions option)
        {
            var newTransition = new NfaStateTransition(transitionToState, option);
            AddExistingTransitionToState(newTransition);
        }

        private void AddExistingTransitionToState(NfaStateTransition transitionToAdd)
        {
            if (Transition1 == null)
            {
                Transition1 = transitionToAdd;
            }
            else if (Transition2 == null)
            {
                Transition2 = transitionToAdd;
            }
            else
            {
                throw new NotSupportedException("Maximum number of NFA state transitions already used.");
            }
        }

        public override bool Equals(object otherObject)
        {
            if (otherObject == null || GetType() != otherObject.GetType())
            {
                return false;
            }
            var otherNfaState = otherObject as NfaState;
            return id == otherNfaState.id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(NfaState left, NfaState right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(NfaState left, NfaState right)
        {
            return !(left == right);
        }
    }
}
