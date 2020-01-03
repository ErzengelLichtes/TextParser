using System;
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
    }
}
