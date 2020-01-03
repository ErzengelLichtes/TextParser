using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class Peek
    {
        [TestMethod]
        public void EmptyStringReturnsNull()
        {
            var p      = CreateReader("");
            var result = p.Peek();
            Assert.IsNull(result);
        }
        [TestMethod]
        public void FirstCharacter()
        {
            var p = CreateReader("abc");
            var r = p.Peek();
            Assert.AreEqual('a', r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void SecondCharacter()
        {
            var p = CreateReader("abc");
            var r = p.Peek(1);
            Assert.AreEqual('b', r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void PastEndReturnsNull()
        {
            var p = CreateReader("abc");
            var r = p.Peek(3);
            Assert.IsNull(r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
    }
}
