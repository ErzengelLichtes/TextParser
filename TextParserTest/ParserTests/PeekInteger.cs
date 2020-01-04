using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class PeekInteger
    {
        [TestMethod]
        public void EmptyString()
        {
            var p = CreateReader("");
            var r = p.PeekInteger();
            Assert.IsNull(r);
        }
        [TestMethod]
        public void NotInteger()
        {
            var p = CreateReader("abc");
            var r = p.PeekInteger();
            Assert.IsNull(r);
        }
        [TestMethod]
        public void ReadNotInteger()
        {
            try {
                var p = CreateReader("abc");
                var r = p.ReadInteger();
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                Assert.AreEqual("Expected identifier", e.ErrorMessage);
                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }

        [TestMethod]
        public void SingleDigitIntegerFull()
        {
            var p = CreateReader("1");
            var r = p.PeekInteger();
            Assert.AreEqual("1", r.ToString());
            AssertCharacterPosition(new CharacterPosition(1,1), p.CharacterPosition);
        }
        [TestMethod]
        public void MultiDigitIntegerFull()
        {
            var p = CreateReader("853");
            var r = p.PeekInteger();
            Assert.AreEqual("853", r.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void SingleDigitIntegerSpace()
        {
            var p = CreateReader("1 822 999 asdf");
            var r = p.PeekInteger();
            Assert.AreEqual("1", r.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void MultiDigitIntegerSpace()
        {
            var p = CreateReader("1 822 999 asdf");
            p.Pop(2);
            var r = p.PeekInteger();
            Assert.AreEqual("822", r.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }
        [TestMethod]
        public void SingleDigitIntegerLetter()
        {
            var p = CreateReader("1asdf");
            var r = p.PeekInteger();
            Assert.AreEqual("1", r.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void MultiDigitIntegerLetter()
        {
            var p = CreateReader("853foobar");
            var r = p.PeekInteger();
            Assert.AreEqual("853", r.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void ReadSingleDigitIntegerLetter()
        {
            var p = CreateReader("1asdf");
            var r = p.ReadInteger();
            Assert.AreEqual("1", r);
            AssertCharacterPosition(new CharacterPosition(1, 2), p.CharacterPosition);
        }
        [TestMethod]
        public void ReadMultiDigitIntegerSpace()
        {
            var p = CreateReader("1 822 999 asdf");
            p.Pop(2);
            var r = p.ReadInteger();
            Assert.AreEqual("822", r);
            AssertCharacterPosition(new CharacterPosition(1, 6), p.CharacterPosition);
        }
    }
}
