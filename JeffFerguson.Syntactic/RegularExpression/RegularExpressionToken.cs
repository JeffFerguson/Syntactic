namespace JeffFerguson.Syntactic.RegularExpression
{
    public struct RegularExpressionToken
    {
        public enum CharacterClass
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

        public CharacterClass Class { get; internal set; }
        public char Character { get; internal set; }
        internal static RegularExpressionToken NullToken = new() { Class = CharacterClass.Unknown, Character = (char)0x00 };
    }
}
