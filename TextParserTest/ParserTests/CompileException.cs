using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class CompileException
    {
        [TestMethod]
        public void Start()
        {
            var p = CreateReader("a");
            var r = p.CompileException("test");
            Assert.AreEqual("test", r.ErrorMessage);
            AssertCharacterPosition(new CharacterPosition(1,1), r.CharacterPosition);
        }
        [TestMethod]
        public void Mid()
        {
            var p = CreateReader("a");
            p.Pop(1);
            var r = p.CompileException("msg");
            Assert.AreEqual("msg", r.ErrorMessage);
            AssertCharacterPosition(new CharacterPosition(1, 2), r.CharacterPosition);
        }
        [TestMethod]
        public void CharacterPosNewline()
        {
            var p = CreateReader("abc\ndef");
            var r = p.GetCharacterPositionAfter(4);
            AssertCharacterPosition(new CharacterPosition(2, 1), r);
        }
    }
}
