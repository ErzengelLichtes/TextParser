using System;
using System.Runtime.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;

namespace TextParserTest
{
    [TestClass]
    public class CharacterPositionTest
    {
        [TestMethod]
        public void DefaultConstructorIsZero()
        {
            var r = new CharacterPosition();
            Assert.AreEqual(0, r.Character, nameof(r.Character));
            Assert.AreEqual(0, r.Line, nameof(r.Line));
        }
        [TestMethod]
        public void DefinedConstructorNoChange1()
        {
            var r = new CharacterPosition(line: 2, character: 1);
            Assert.AreEqual(1, r.Character, nameof(r.Character));
            Assert.AreEqual(2, r.Line, nameof(r.Line));
        }
        [TestMethod]
        public void DefinedConstructorNoChange2()
        {
            var r = new CharacterPosition(line: 15, character: 2);
            Assert.AreEqual(2, r.Character, nameof(r.Character));
            Assert.AreEqual(15, r.Line, nameof(r.Line));
        }
        [TestMethod]
        public void ToStringShowsValue()
        {
            var r = new CharacterPosition(line: 2, character: 1);
            string actualString = r.ToString();
            Assert.AreEqual("Line: 2 Character: 1", actualString);
        }
    }
}
