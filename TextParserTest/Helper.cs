using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;

namespace TextParserTest
{
    class Helper
    {
        public static Parser CreateReader(string str)
        {
            return new Parser(new StringReader(str));
        }
        public static void AssertCharacterPosition(CharacterPosition expected, CharacterPosition actual, string message = null)
        {
            if (message != null) message = message + ".";
            Assert.AreEqual(expected.Line,      actual.Line,      message + "Line");
            Assert.AreEqual(expected.Character, actual.Character, message + "Character");
        }
    }
}
