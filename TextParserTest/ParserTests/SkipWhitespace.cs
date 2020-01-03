using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class SkipWhitespace
    {
        [TestMethod]
        public void StringSkipWhitespace()
        {
            var p = CreateReader("   abc   ");
            p.SkipWhitespace();
            var ident = p.PeekWord();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc", ident.ToString());
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 7), p.CharacterPosition);
        }
        [TestMethod]
        public void SkipCommentsSingleLine()
        {
            var p    = CreateReader("abc //stuff\nand more");
            var word = p.ReadWord();
            p.SkipWhitespaceAndCStyleComments();
            var word2 = p.ReadWord();
            Assert.AreEqual("abc", word);
            Assert.AreEqual("and", word2);
        }
        [TestMethod]
        public void SkipCommentsMultiLine()
        {
            var p    = CreateReader("abc /*stuff*/ and more");
            var word = p.ReadWord();
            p.SkipWhitespaceAndCStyleComments();
            var word2 = p.ReadWord();
            Assert.AreEqual("abc", word);
            Assert.AreEqual("and", word2);
        }
    }
}
