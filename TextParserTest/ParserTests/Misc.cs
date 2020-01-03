using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest
{
    [TestClass]
    public class Misc
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


    }
}
