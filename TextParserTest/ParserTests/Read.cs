using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class Read
    {
        [TestMethod]
        public void EmptyStringThrows()
        {
            var p = CreateReader("");
            try
            {
                var result = p.Read();
                Assert.Fail("Expected an exception");
            }
            catch (CompilerException e)
            {
                Assert.AreEqual("Read past the end of the file", e.ErrorMessage);
                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }

        [TestMethod]
        public void FirstCharacter()
        {
            var p = CreateReader("abc");
            var r = p.Read();
            Assert.AreEqual('a', r);
            AssertCharacterPosition(new CharacterPosition(1, 2), p.CharacterPosition);
        }
        [TestMethod]
        public void FirstTwoCharacters()
        {
            var p = CreateReader("abc");
            var r = p.Read(2);
            Assert.AreEqual("ab", r);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }
        [TestMethod]
        public void ConsecutiveReads()
        {
            var p  = CreateReader("abc");
            var r  = p.Read(2);
            var r2 = p.Read(1);
            Assert.AreEqual("ab", r);
            Assert.AreEqual("c",  r2);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }
        [TestMethod]
        public void ConsecutiveReadsPastEnd()
        {
            var p = CreateReader("abc");
            var r = p.Read(3);
            try
            {
                var r2 = p.Read(1);
            }
            catch (CompilerException e)
            {
                Assert.AreEqual("Read past the end of the file", e.ErrorMessage);
                AssertCharacterPosition(new CharacterPosition(1, 4), e.CharacterPosition);
            }
        }
    }
}
