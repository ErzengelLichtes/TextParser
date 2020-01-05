using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class ReadLine
    {
        [TestMethod]
        [DataRow("no ws\ndef")]
        [DataRow("ws after\n    def")]
        [DataRow("ws before after  \n    def")]
        [DataRow("ws before  \ndef")]
        public void StringReadLineNewline(string line)
        {
            var p = CreateReader(line);
            var p1 = p.ReadLine();
            Assert.AreEqual(p1, line.Split('\n').First());
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
        [DataRow("asdf\ntest")]
        [DataRow("asdf\r\ntest")]
        [DataRow("asdf\rtest")]
        public void HasNewline(string line)
        {
            var p  = CreateReader(line);
            p.ReadWord();
            var r = p.HasNewline();
            Assert.IsTrue(r);
            AssertCharacterPosition(new CharacterPosition(2,1), p.CharacterPosition);
        }
        [TestMethod]
        public void HasNoNewline()
        {
            var p = CreateReader("asdf\ntest");
            var r = p.HasNewline();
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        [DataRow("asdf\ntest")]
        [DataRow("asdf\r\ntest")]
        [DataRow("asdf\rtest")]
        public void ExpectNewline(string line)
        {
            var p = CreateReader(line);
            p.ReadWord();
            p.ExpectNewline();
            AssertCharacterPosition(new CharacterPosition(2, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void ExpectNewlineFail()
        {
            try { 
                var p = CreateReader("asdf\ntest");
                p.ExpectNewline();
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "newline");

                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }
    }
}
