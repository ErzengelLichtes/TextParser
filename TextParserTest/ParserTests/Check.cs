using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class Check
    {
        [TestMethod]
        public void EmptyString()
        {
            var p = CreateReader("");
            var r = p.Check("x");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1,1), p.CharacterPosition);
        }
        [TestMethod]
        public void StartGood()
        {
            var p = CreateReader("abc");
            var r = p.Check("a");
            Assert.IsTrue(r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void StartBad()
        {
            var p = CreateReader("abc");
            var r = p.Check("x");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void StartGoodMultiChar()
        {
            var p = CreateReader("abc");
            var r = p.Check("ab");
            Assert.IsTrue(r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void MidGoodMultiChar()
        {
            var p = CreateReader("abc");
            p.Pop(1);
            var r = p.Check("bc");
            Assert.IsTrue(r);
            AssertCharacterPosition(new CharacterPosition(1, 2), p.CharacterPosition);
        }
        [TestMethod]
        public void MidBad()
        {
            var p = CreateReader("abc");
            p.Pop(1);
            var r = p.Check("c");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 2), p.CharacterPosition);
        }
        [TestMethod]
        public void PastEndFalse()
        {
            var p = CreateReader("abc");
            p.Pop(3);
            var r = p.Check("c");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }
        
    }
}
