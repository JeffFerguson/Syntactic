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
        public RegisteredRegularExpression RegisterRegularExpression(string regularExpression, Action<Lexeme> lexemeHandler)
        {
            var registeredRegularExpression = new RegisteredRegularExpression();
            var tokenizer = new RegularExpressionTokenizer();
            tokenizer.Tokenize(regularExpression);
            registeredRegularExpression.Tokens = tokenizer.Tokens;
            registeredRegularExpression.Nfa = new Nfa.Nfa(registeredRegularExpression.Tokens);
            return registeredRegularExpression;
        }
    }
}
