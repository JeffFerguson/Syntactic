namespace JeffFerguson.Syntactic.RegularExpression
{
    internal struct RegularExpressionToken
    {
        internal enum CharacterClass
        {
            Unknown = 0,
            SingleCharacter,
            Wildcard,
            StartOfLine,
            EndOfLine,
            CharacterClassStart,
            CharacterClassEnd,
            NegateCharacterClass,
            ZeroOrMore,
            OneOrMore,
            ZeroOrOne,
            Or,
            GroupingStart,
            GroupingEnd,
            NextCharIsLiteral,
            CharacterRange,
            NextCharsAreLiteralStart,
            NextCharsAreLiteralEnd
        }

        internal CharacterClass Class;
        internal char Character;
    }
}
