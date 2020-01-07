using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.SkipWhitespaceParserTests
{
    [TestClass]
    public class Has
    {
        [TestMethod]
        [DataRow("test", "test", 1, 5)]
        [DataRow("    foo", "foo", 1, 8)]
        [DataRow(" \n    foobar", "foobar", 2, 11)]
        public void HasCorrect(string toparse, string expected, int line, int character)
        {
            var p = CreateWsReader(toparse);
            var r = p.Has(expected);
            Assert.IsTrue(r);
            AssertCharacterPosition(new CharacterPosition(line,character), p.CharacterPosition);
        }
    }
}
