using JeffFerguson.Syntactic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class LexicalAnalyzerTests
    {
        [TestMethod]
        public void NoSpecialCharacters()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("abc", a => { } );
        }

        [TestMethod]
        public void Wildcard()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("a.c", a => { });
        }

        [TestMethod]
        public void StartOfLine()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("^abc", a => { });
        }

        [TestMethod]
        public void EndOfLine()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("abc$", a => { });
        }

        [TestMethod]
        public void CharacterClass()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("[abc]", a => { });
        }

        [TestMethod]
        public void NegatedCharacterClass()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("[#abc]", a => { });
        }

        [TestMethod]
        public void ZeroOrMore()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("a*", a => { });
        }

        [TestMethod]
        public void OneOrMore()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("a+", a => { });
        }

        [TestMethod]
        public void ZeroOrOne()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("a?", a => { });
        }

        [TestMethod]
        public void Or()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("a|b", a => { });
        }

        [TestMethod]
        public void Grouping()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("(a|b)c", a => { });
        }

        [TestMethod]
        public void LiteralCharacter()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("a\\?b", a => { });
        }

        [TestMethod]
        public void QuotedCharacters()
        {
            var lex = new LexicalAnalyzer();
            lex.RegisterRegularExpression("\"*^\"?", a => { });
        }
    }
}
