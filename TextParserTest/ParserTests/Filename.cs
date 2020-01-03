using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class Filename
    {
        [TestMethod]
        public void StringFilenameIdentifierNone()
        {
            var p     = CreateReader("<abc.def>");
            var ident = p.PeekFilename();
            Assert.IsNull(ident);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void StringFilenameIdentifier()
        {
            var p     = CreateReader("abc.def");
            var ident = p.PeekFilename();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc.def", ident.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "start");
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 8), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void StringFilenameIdentifierWithExtra()
        {
            var p     = CreateReader("abc.def stuff");
            var ident = p.PeekFilename();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc.def", ident.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "start");
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 8), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void StringQuotedFilenameIdentifierWithExtra()
        {
            var p     = CreateReader("\"blah abc.def\" stuff");
            var ident = p.PeekFilename();
            Assert.IsNotNull(ident);
            Assert.AreEqual("blah abc.def", ident.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "start");
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 15), p.CharacterPosition, "end");
        }
    }
}
