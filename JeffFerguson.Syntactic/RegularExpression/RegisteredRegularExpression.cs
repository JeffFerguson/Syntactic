using System.Collections.Generic;

namespace JeffFerguson.Syntactic.RegularExpression
{
    public class RegisteredRegularExpression
    {
        public List<RegularExpressionToken> Tokens { get; internal set; }
        public Nfa.Nfa Nfa { get; internal set; }
    }
}