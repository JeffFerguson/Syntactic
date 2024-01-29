using System;

namespace JeffFerguson.Syntactic.Nfa
{
    public class NfaState
    {
        public enum AnchorOptions
        {
            NotAnchored = 0,
            AnchoredToStartOfLine,
            AnchoredToEndOfLine
        }

        public NfaStateTransition transition1 { get; private set; }
        public  NfaStateTransition transition2 { get; private set; }

        public AnchorOptions Anchor { get; set; }

        internal NfaState()
        {
            transition1 = null;
            transition2 = null;
            Anchor = AnchorOptions.NotAnchored;
        }

        internal void AddTransition(NfaState transitionToState, char singleCharacter)
        {
            var newTransition = new NfaStateTransition(transitionToState, singleCharacter);
            AddExistingTransitionToState(newTransition);
        }

        internal void AddTransition(NfaState transitionToState, NfaStateTransition.TransitionOptions option)
        {
            var newTransition = new NfaStateTransition(transitionToState, option);
            AddExistingTransitionToState(newTransition);
        }

        private void AddExistingTransitionToState(NfaStateTransition transitionToAdd)
        {
            if (transition1 == null)
            {
                transition1 = transitionToAdd;
            }
            else if (transition2 == null)
            {
                transition2 = transitionToAdd;
            }
            else
            {
                throw new NotSupportedException("Maximum number of NFA transitions already used.");
            }
        }
    }
}
