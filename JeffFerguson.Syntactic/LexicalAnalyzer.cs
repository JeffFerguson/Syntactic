using JeffFerguson.Syntactic.RegularExpression;
using System;

namespace JeffFerguson.Syntactic
{
    /// <summary>
    /// A lexical analyzer. Duh.
    /// </summary>
    /// <remarks>
    /// Used by Linguistic, someday.
    /// </remarks>
    public class LexicalAnalyzer
    {
        public void RegisterRegularExpression(string regularExpression, Action<Lexeme> lexemeHandler)
        {
            var tokenizer = new RegularExpressionTokenizer();
            tokenizer.Tokenize(regularExpression);
            var nfa = new Nfa.Nfa(tokenizer.Tokens);
        }
    }
}
