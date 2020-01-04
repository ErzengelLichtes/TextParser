using System;
using System.Linq;
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
        public void SkipCommentsRange()
        {
            var p    = CreateReader("abc /*stuff*/ and more");
            var word = p.ReadWord();
            p.SkipWhitespaceAndCStyleComments();
            var word2 = p.ReadWord();
            Assert.AreEqual("abc", word);
            Assert.AreEqual("and", word2);
        }
        [TestMethod]
        public void SkipCommentsMultiLine()
        {
            var p    = CreateReader("abc /*stuff\nand such*/ and more");
            var word = p.ReadWord();
            p.SkipWhitespaceAndCStyleComments();
            var word2 = p.ReadWord();
            Assert.AreEqual("abc", word);
            Assert.AreEqual("and", word2);
        }

        [TestMethod]
        [DataRow("no_ws", Parser.IsNewline.NoWhitespace, 6)]
        [DataRow("ws_before    \ntest", Parser.IsNewline.AtNewline, 14)]
        [DataRow("ws_before_after    \n    test", Parser.IsNewline.AtNewline, 20)]
        [DataRow("ws_no_line\tnl_follows\n    test", Parser.IsNewline.WhitespaceOnly, 12)]
        public void SkipWhitespaceToNewline(string lines, Parser.IsNewline expected, int character)
        {
            // ReSharper disable MustUseReturnValue
            var p = CreateReader(lines);
            p.ReadCStyleIdentifier();
            var r = p.SkipWhitespaceToNewline();
            // ReSharper restore MustUseReturnValue
            Assert.AreEqual(expected, r);
            AssertCharacterPosition(new CharacterPosition(1, character), p.CharacterPosition);
        }
    }
}
