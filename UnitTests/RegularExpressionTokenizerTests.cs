using Microsoft.VisualStudio.TestTools.UnitTesting;
using static JeffFerguson.Syntactic.RegularExpression.RegularExpressionToken;

namespace JeffFerguson.Syntactic.UnitTests
{
    [TestClass]
    public class RegularExpressionTokenizerTests
    {
        /// <summary>
        /// Check the tokens generated for regular expression abc.
        /// </summary>
        [TestMethod]
        public void NoSpecialCharacters()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("abc", a => { } );
            Assert.AreEqual<int>(3, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[0].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<char>('b', registeredRegularExpression.Tokens[1].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[2].Class);
            Assert.AreEqual<char>('c', registeredRegularExpression.Tokens[2].Character);
        }

        /// <summary>
        /// Check the tokens generated for regular expression a.c.
        /// </summary>
        [TestMethod]
        public void Wildcard()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a.c", a => { });
            Assert.AreEqual<int>(3, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[0].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.Wildcard, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[2].Class);
            Assert.AreEqual<char>('c', registeredRegularExpression.Tokens[2].Character);
        }

        /// <summary>
        /// Check the tokens generated for regular expression ^abc.
        /// </summary>
        [TestMethod]
        public void StartOfLine()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("^abc", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.StartOfLine, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[1].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[2].Class);
            Assert.AreEqual<char>('b', registeredRegularExpression.Tokens[2].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[3].Class);
            Assert.AreEqual<char>('c', registeredRegularExpression.Tokens[3].Character);
        }

        /// <summary>
        /// Check the tokens generated for regular expression abc$.
        /// </summary>
        [TestMethod]
        public void EndOfLine()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("abc$", a => { });
            Assert.AreEqual<int>(4, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[0].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<char>('b', registeredRegularExpression.Tokens[1].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[2].Class);
            Assert.AreEqual<char>('c', registeredRegularExpression.Tokens[2].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.EndOfLine, registeredRegularExpression.Tokens[3].Class);
        }

        /// <summary>
        /// Check the tokens generated for regular expression [abc].
        /// </summary>
        public void CharacterClass()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("[abc]", a => { });
            Assert.AreEqual<int>(5, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.GroupingStart, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[1].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[2].Class);
            Assert.AreEqual<char>('b', registeredRegularExpression.Tokens[2].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[3].Class);
            Assert.AreEqual<char>('c', registeredRegularExpression.Tokens[3].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.GroupingEnd, registeredRegularExpression.Tokens[4].Class);
        }

        /// <summary>
        /// Check the tokens generated for regular expression [#abc].
        /// </summary>
        [TestMethod]
        public void NegatedCharacterClass()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("[#abc]", a => { });
            Assert.AreEqual<int>(6, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.CharacterClassStart, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.NegateCharacterClass, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[2].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[2].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[3].Class);
            Assert.AreEqual<char>('b', registeredRegularExpression.Tokens[3].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[4].Class);
            Assert.AreEqual<char>('c', registeredRegularExpression.Tokens[4].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.CharacterClassEnd, registeredRegularExpression.Tokens[5].Class);
        }

        /// <summary>
        /// Check the tokens generated for regular expression a*.
        /// </summary>
        [TestMethod]
        public void ZeroOrMore()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a*", a => { });
            Assert.AreEqual<int>(2, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[0].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.ZeroOrMore, registeredRegularExpression.Tokens[1].Class);
        }

        /// <summary>
        /// Check the tokens generated for regular expression a+.
        /// </summary>
        [TestMethod]
        public void OneOrMore()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a+", a => { });
            Assert.AreEqual<int>(2, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[0].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.OneOrMore, registeredRegularExpression.Tokens[1].Class);
        }

        /// <summary>
        /// Check the tokens generated for regular expression a?.
        /// </summary>
        [TestMethod]
        public void ZeroOrOne()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a?", a => { });
            Assert.AreEqual<int>(2, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[0].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.ZeroOrOne, registeredRegularExpression.Tokens[1].Class);
        }

        /// <summary>
        /// Check the tokens generated for regular expression a|b.
        /// </summary>
        [TestMethod]
        public void Or()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a|b", a => { });
            Assert.AreEqual<int>(3, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[0].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.Or, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[2].Class);
            Assert.AreEqual<char>('b', registeredRegularExpression.Tokens[2].Character);
        }

        /// <summary>
        /// Check the tokens generated for regular expression (a|b)c.
        /// </summary>
        [TestMethod]
        public void Grouping()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("(a|b)c", a => { });
            Assert.AreEqual<int>(6, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.GroupingStart, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[1].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.Or, registeredRegularExpression.Tokens[2].Class);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[3].Class);
            Assert.AreEqual<char>('b', registeredRegularExpression.Tokens[3].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.GroupingEnd, registeredRegularExpression.Tokens[4].Class);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[5].Class);
            Assert.AreEqual<char>('c', registeredRegularExpression.Tokens[5].Character);
        }

        /// <summary>
        /// Check the tokens generated for regular expression a\?b.
        /// </summary>
        [TestMethod]
        public void LiteralCharacter()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("a\\?b", a => { });
            Assert.AreEqual<int>(3, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<char>('a', registeredRegularExpression.Tokens[0].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<char>('?', registeredRegularExpression.Tokens[1].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[2].Class);
            Assert.AreEqual<char>('b', registeredRegularExpression.Tokens[2].Character);
        }

        /// <summary>
        /// Check the tokens generated for regular expression "*^"?.
        /// </summary>
        [TestMethod]
        public void QuotedCharacters()
        {
            var lex = new LexicalAnalyzer();
            var registeredRegularExpression = lex.RegisterRegularExpression("\"*^\"?", a => { });
            Assert.AreEqual<int>(3, registeredRegularExpression.Tokens.Count);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[0].Class);
            Assert.AreEqual<char>('*', registeredRegularExpression.Tokens[0].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.SingleCharacter, registeredRegularExpression.Tokens[1].Class);
            Assert.AreEqual<char>('^', registeredRegularExpression.Tokens[1].Character);
            Assert.AreEqual<CharacterClass>(RegularExpression.RegularExpressionToken.CharacterClass.ZeroOrOne, registeredRegularExpression.Tokens[2].Class);
        }
    }
}
