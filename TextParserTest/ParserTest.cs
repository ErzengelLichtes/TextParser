using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest
{
    [TestClass]
    public class ParserTest
    {

        [TestMethod]
        public void NullReaderThrowsException()
        {
            try
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                Parser p = new Parser(reader: null);
                Assert.Fail("Expected an exception");
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("reader", e.ParamName);
            }
        }



        [TestMethod]
        public void EmptyStringReadThrows()
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
        public void StringReadsFirstCharacter()
        {
            var p = CreateReader("abc");
            var r = p.Read();
            Assert.AreEqual('a', r);
            AssertCharacterPosition(new CharacterPosition(1, 2), p.CharacterPosition);
        }
        [TestMethod]
        public void StringReadsFirstTwoCharacters()
        {
            var p = CreateReader("abc");
            var r = p.Read(2);
            Assert.AreEqual("ab", r);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }
        [TestMethod]
        public void StringConsecutiveReads()
        {
            var p = CreateReader("abc");
            var r = p.Read(2);
            var r2 = p.Read(1);
            Assert.AreEqual("ab", r);
            Assert.AreEqual("c", r2);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }
        [TestMethod]
        public void StringConsecutiveReadsPastEnd()
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
        public void StringWordIdentifierNone()
        {
            var p = CreateReader("{abc}");
            var ident = p.PeekWord();
            Assert.IsNull(ident);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void StringWordIdentifier()
        {
            var p = CreateReader("abc");
            var ident = p.PeekWord();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc", ident.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "start");
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void StringWordIdentifierWithExtra()
        {
            var p = CreateReader("abc stuff");
            var ident = p.PeekWord();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc", ident.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "start");
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void StringFilenameIdentifierNone()
        {
            var p = CreateReader("<abc.def>");
            var ident = p.PeekFilename();
            Assert.IsNull(ident);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void StringFilenameIdentifier()
        {
            var p = CreateReader("abc.def");
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
            var p = CreateReader("abc.def stuff");
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
            var p = CreateReader("\"blah abc.def\" stuff");
            var ident = p.PeekFilename();
            Assert.IsNotNull(ident);
            Assert.AreEqual("blah abc.def", ident.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "start");
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 15), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void StringSkipWhitespace()
        {
            var p = CreateReader("   abc   ");
            p.SkipWhitespace();
            var ident = p.PeekWord();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc", ident.ToString());
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 7), p.CharacterPosition);
        }

        [TestMethod]
        public void StringWordAtEof()
        {
            var p = CreateReader("abc");
            p.Pop(3);
            Assert.IsTrue(p.Eof);
        }
        [TestMethod]
        public void StringWordNotAtEof()
        {
            var p = CreateReader("abc");
            Assert.IsFalse(p.Eof);
        }
        [TestMethod]
        public void StringWordNotAtEofNear()
        {
            var p = CreateReader("abc");
            p.Pop(2);
            Assert.IsFalse(p.Eof);
        }

        [TestMethod]
        public void StringReadLineNewline()
        {
            var p = CreateReader("abc\ndef");
            var p1 = p.ReadLine();
            Assert.AreEqual(p1, "abc");
            Assert.AreEqual(2, p.CharacterPosition.Line);
            Assert.AreEqual(1, p.CharacterPosition.Character);
        }
        [TestMethod]
        public void StringReadLineCarriageReturnNewline()
        {
            var p = CreateReader("abc\r\ndef");
            var p1 = p.ReadLine();
            Assert.AreEqual(p1, "abc");
            Assert.AreEqual(2, p.CharacterPosition.Line);
            Assert.AreEqual(1, p.CharacterPosition.Character);
        }
        [TestMethod]
        public void StringReadLineCarriageReturn()
        {
            var p = CreateReader("abc\rdef");
            var p1 = p.ReadLine();
            Assert.AreEqual(p1, "abc");
            Assert.AreEqual(2, p.CharacterPosition.Line);
            Assert.AreEqual(1, p.CharacterPosition.Character);
        }
        [TestMethod]
        public void StringReadLine2Newline()
        {
            var p = CreateReader("abc\ndef\negs");
            var p1 = p.ReadLine();
            var p2 = p.ReadLine();
            Assert.AreEqual(p1, "abc");
            Assert.AreEqual(p2, "def");
            Assert.AreEqual(3, p.CharacterPosition.Line);
            Assert.AreEqual(1, p.CharacterPosition.Character);
        }
        [TestMethod]
        public void StringReadLine2CarriageReturnNewline()
        {
            var p = CreateReader("abc\r\ndef\r\negs");
            var p1 = p.ReadLine();
            var p2 = p.ReadLine();
            Assert.AreEqual(p1, "abc");
            Assert.AreEqual(p2, "def");
            Assert.AreEqual(3, p.CharacterPosition.Line);
            Assert.AreEqual(1, p.CharacterPosition.Character);
        }
        [TestMethod]
        public void StringReadLine2CarriageReturn()
        {
            var p = CreateReader("abc\rdef\regs");
            var p1 = p.ReadLine();
            var p2 = p.ReadLine();
            Assert.AreEqual(p1, "abc");
            Assert.AreEqual(p2, "def");
            Assert.AreEqual(3, p.CharacterPosition.Line);
            Assert.AreEqual(1, p.CharacterPosition.Character);
        }

        [TestMethod]
        public void SkipCommentsSingleLine()
        {
            var p = CreateReader("abc //stuff\nand more");
            var word = p.ReadWord();
            p.SkipWhitespaceAndCStyleComments();
            var word2 = p.ReadWord();
            Assert.AreEqual("abc", word);
            Assert.AreEqual("and", word2);
        }
        [TestMethod]
        public void SkipCommentsMultiLine()
        {
            var p = CreateReader("abc /*stuff*/ and more");
            var word = p.ReadWord();
            p.SkipWhitespaceAndCStyleComments();
            var word2 = p.ReadWord();
            Assert.AreEqual("abc", word);
            Assert.AreEqual("and", word2);
        }
    }
}
