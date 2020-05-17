using System.Dynamic;

namespace JeffFerguson.Syntactic.Nfa
{
    internal class NfaStateTransition
    {
        internal enum TransitionOptions
        {
            Epsilon = 0,
            SingleCharacter,
            AllCharacters
        }

        private char SingleCharacter;

        internal NfaState ToState { get; private set; }
        internal TransitionOptions Option { get; private set; }

        internal NfaStateTransition()
        {
            ToState = null;
            Option = TransitionOptions.Epsilon;
        }

        internal NfaStateTransition(NfaState transitionTo)
            : this(transitionTo, TransitionOptions.Epsilon)
        {
        }

        internal NfaStateTransition(NfaState transitionTo, char singleCharacter)
        {
            SingleCharacter = singleCharacter;
            ToState = transitionTo;
            Option = TransitionOptions.SingleCharacter;
        }

        internal NfaStateTransition(NfaState transitionTo, TransitionOptions option)
        {
            ToState = transitionTo;
            Option = option;
        }
    }
}
