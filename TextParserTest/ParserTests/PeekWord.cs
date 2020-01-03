using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class PeekWord
    {
        [TestMethod]
        public void StringWordIdentifierNone()
        {
            var p     = CreateReader("{abc}");
            var ident = p.PeekWord();
            Assert.IsNull(ident);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void StringWordIdentifier()
        {
            var p     = CreateReader("abc");
            var ident = p.PeekWord();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc", ident.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "start");
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void StringWordIdentifierWithExtra()
        {
            var p     = CreateReader("abc stuff");
            var ident = p.PeekWord();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc", ident.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "start");
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition, "end");
        }
    }
}
