﻿using System.Dynamic;

namespace JeffFerguson.Syntactic.Nfa
{
    public class NfaStateTransition
    {
        public enum TransitionOptions
        {
            Epsilon = 0,
            SingleCharacter,
            AllCharacters
        }

        public char SingleCharacter { get; private set; }

        public NfaState ToState { get; private set; }
        public TransitionOptions Option { get; private set; }

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
