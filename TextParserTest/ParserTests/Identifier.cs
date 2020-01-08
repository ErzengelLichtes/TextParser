using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class Identifier
    {
        [TestMethod]
        [DataRow("abc 123", "abc")]
        [DataRow("_abc 123", "_abc")]
        [DataRow("abc123 and stuff", "abc123")]
        [DataRow("_abc123", "_abc123")]
        public void ReadCStyle(string line, string result)
        {
            var p = CreateReader(line);
            var r = p.ReadCStyleIdentifier();
            Assert.AreEqual(result, r);
            AssertCharacterPosition(new CharacterPosition(1, result.Length + 1), p.CharacterPosition);
        }
        [TestMethod]
        [DataRow("abc 123",          "abc")]
        [DataRow("_abc 123",         "_abc")]
        [DataRow("abc123 and stuff", "abc123")]
        [DataRow("_abc123",          "_abc123")]
        public void PeekCStyle(string line, string result)
        {
            var p = CreateReader(line);
            var r = p.PeekCStyleIdentifier();
            Assert.IsNotNull(r);
            Assert.AreEqual(result, r.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
            r.Pop();
            AssertCharacterPosition(new CharacterPosition(1, result.Length + 1), p.CharacterPosition);
        }
        [TestMethod]
        public void PeekCStyleFail()
        {
            var p = CreateReader("  test");
            var r = p.PeekCStyleIdentifier(skip:false);
            Assert.IsNull(r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void ReadCStyleFail()
        {
            try
            {
                var p = CreateReader("  test");
                var r = p.ReadCStyleIdentifier(skip:false);
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "identifier");

                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }

        [TestMethod]
        public void ReadQuoteString()
        {
            var p = CreateReader("\"test\" asdf");
            var r = p.ReadQuotedString();
            Assert.AreEqual("test", r);
            AssertCharacterPosition(new CharacterPosition(1, 7), p.CharacterPosition);
        }
        [TestMethod]
        public void ReadQuoteStringWithWhitespace()
        {
            var p = CreateReader("\"   \" asdf");
            var r = p.ReadQuotedString();
            Assert.AreEqual("   ", r);
            AssertCharacterPosition(new CharacterPosition(1, 6), p.CharacterPosition);
        }
        [TestMethod]
        public void ReadQuoteStringFail()
        {
            try
            {
                var p = CreateReader("asdf");
                var r = p.ReadQuotedString();
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "\"");

                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }

    }
}
