using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class Has
    {
        [TestMethod]
        public void BeginningGood()
        {
            var p = CreateReader("abc");
            var r = p.Has("ab");
            Assert.IsTrue(r);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }
        [TestMethod]
        public void BeginningBad()
        {
            var p = CreateReader("abc");
            var r = p.Has("c");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void MiddleGood()
        {
            var p = CreateReader("abc");
            p.Pop();
            var r = p.Has("b");
            Assert.IsTrue(r);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }
        [TestMethod]
        public void MiddleBad()
        {
            var p = CreateReader("abc");
            p.Pop();
            var r = p.Has("x");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 2), p.CharacterPosition);
        }
        [TestMethod]
        public void EndGood()
        {
            var p = CreateReader("abc");
            p.Pop(2);
            var r = p.Has("c");
            Assert.IsTrue(r);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }
        [TestMethod]
        public void EndBad()
        {
            var p = CreateReader("abc");
            p.Pop(2);
            var r = p.Has("x");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }
        [TestMethod]
        public void EndBadTooLong()
        {
            var p = CreateReader("abc");
            p.Pop(2);
            var r = p.Has("cd");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }
        [TestMethod]
        public void PastEnd()
        {
            var p = CreateReader("abc");
            p.Pop(3);
            var r = p.Has("c");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }
    }
}
