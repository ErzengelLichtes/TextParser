using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class Expect
    {
        [TestMethod]
        public void StringExpectFirstTwoCharacters()
        {
            var p = CreateReader("abc");
            p.Expect("ab");
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }
        [TestMethod]
        public void StringExpectWrongCharactersThrows()
        {
            var p = CreateReader("abc");
            try
            {
                p.Expect("c");
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                Assert.AreEqual("Expected c", e.ErrorMessage);
                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }
        [TestMethod]
        public void ExpectDictionaryOne()
        {
            var p = CreateReader("foo");
            int r = 0;
            p.Expect(new Dictionary<string, Action<Parser>>()
                     {
                         ["foo"] = parser => r = 1,
                         ["bar"] = parser => r = 2,
                     });
            Assert.AreEqual(1, r, "No or incorrect callback");
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }
        [TestMethod]
        public void ExpectDictionaryTwo()
        {
            var p = CreateReader("bar");
            int r = 0;
            p.Expect(new Dictionary<string, Action<Parser>>()
                     {
                         ["foo"] = parser => r = 1,
                         ["bar"] = parser => r = 2,
                     });
            Assert.AreEqual(2, r, "No or incorrect callback");
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }
        [TestMethod]
        public void ExpectDictionaryBad()
        {
            var p = CreateReader("abc");
            try
            {
            #pragma warning disable 219
                int r = 0;
            #pragma warning restore 219
                p.Expect(new Dictionary<string, Action<Parser>>()
                         {
                             ["foo"] = parser => r = 1,
                             ["bar"] = parser => r = 2,
                         });
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.ErrorMessage, "foo");
                StringAssert.Contains(e.ErrorMessage, "bar");
                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }
    }
}
