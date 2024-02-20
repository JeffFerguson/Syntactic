using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JeffFerguson.Syntactic.UnitTests
{
    [TestClass]
    public class NfaStateCountTests
    {
        /// <summary>
        /// Check the NFA state counts for regular expression abc.
        /// </summary>
        [TestMethod]
        public void NoSpecialCharacters()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("abc", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression a.c.
        /// </summary>
        [TestMethod]
        public void Wildcard()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a.c", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression ^abc.
        /// </summary>
        [TestMethod]
        public void StartOfLine()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("^abc", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression abc$.
        /// </summary>
        [TestMethod]
        public void EndOfLine()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("abc$", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression [abc].
        /// </summary>
        public void CharacterClass()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("[abc]", a => { });
            Assert.AreEqual<int>(2, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression [#abc].
        /// </summary>
        [TestMethod]
        public void NegatedCharacterClass()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("[#abc]", a => { });
            Assert.AreEqual<int>(2, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression a*.
        /// </summary>
        [TestMethod]
        public void ZeroOrMore()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a*", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression a+.
        /// </summary>
        [TestMethod]
        public void OneOrMore()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a+", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression a?.
        /// </summary>
        [TestMethod]
        public void ZeroOrOne()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a?", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression a|b.
        /// </summary>
        [TestMethod]
        public void Or()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a|b", a => { });
            Assert.AreEqual<int>(6, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression (a|b)c.
        /// </summary>
        [TestMethod]
        public void Grouping()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("(a|b)c", a => { });
            Assert.AreEqual<int>(7, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression a\?b.
        /// </summary>
        [TestMethod]
        public void LiteralCharacter()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a\\?b", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Nfa.States.Count);
        }

        /// <summary>
        /// Check the NFA state counts for regular expression "*^"?.
        /// </summary>
        [TestMethod]
        public void QuotedCharacters()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("\"*^\"?", a => { });
            Assert.AreEqual<int>(5, registeredRegularExpression.Nfa.States.Count);
        }
    }
}
