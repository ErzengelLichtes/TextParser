using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;

namespace TextParserTest
{
    [TestClass]
    public class GenericParserFactoryTest
    {
        [TestMethod]
        public void CreateParser()
        {
            var f = new GenericParserFactory<Parser>("filename", new StringReader("abc"));
            var p = f.CreateDefaultParser();
            Assert.IsNotNull(p);
            Assert.IsInstanceOfType(p, typeof(Parser));
            var r = p.Read(3);
            Assert.AreEqual("abc", r);
        }
        [TestMethod]
        public void CreateSubParser()
        {
            var f = new GenericParserFactory<Parser>("filename", new StringReader("abc"));
            var p = f.CreateSubParser("bc", new CharacterPosition(1, 2));
            Assert.IsNotNull(p);
            Assert.IsInstanceOfType(p, typeof(Parser));
            var r = p.Read(2);
            Assert.AreEqual("bc", r);
        }
    }
}
