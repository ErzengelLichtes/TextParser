﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class PeekWord
    {
        [TestMethod]
        public void StringWordIdentifierNone()
        {
            var p     = CreateReader("{abc}");
            var ident = p.PeekWord();
            Assert.IsNull(ident);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }
        [TestMethod]
        public void StringWordIdentifier()
        {
            var p     = CreateReader("abc");
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
            var p     = CreateReader("abc stuff");
            var ident = p.PeekWord();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc", ident.ToString());
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "start");
            ident.Pop();
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition, "end");
        }
        
        [TestMethod]
        public void ReadWordIdentifierNone()
        {
            try
            { 
                var p     = CreateReader("{abc}");
                var ident = p.ReadWord();
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "word");

                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }
        [TestMethod]
        public void ReadWordIdentifier()
        {
            var p     = CreateReader("abc");
            var ident = p.ReadWord();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc", ident);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void ReadWordIdentifierWithExtra()
        {
            var p     = CreateReader("abc stuff");
            var ident = p.ReadWord();
            Assert.IsNotNull(ident);
            Assert.AreEqual("abc", ident);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void HasWordIdentifier()
        {
            var p     = CreateReader("test and stuff");
            var r = p.HasWord("test");
            Assert.IsTrue(r);
            AssertCharacterPosition(new CharacterPosition(1, 5), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void HasWordIdentifierFail()
        {
            var p = CreateReader("tester and stuff");
            var r = p.HasWord("test");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void HasWordIdentifierFailUnderscore()
        {
            var p = CreateReader("test_ and stuff");
            var r = p.HasWord("test");
            Assert.IsFalse(r);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void ExpectWordIdentifier()
        {
            var p = CreateReader("test and stuff");
            p.ExpectWord("test");
            AssertCharacterPosition(new CharacterPosition(1, 5), p.CharacterPosition, "end");
        }
        [TestMethod]
        public void ExpectWordIdentifierFail()
        {
            try { 
                var p = CreateReader("tester and stuff");
                p.ExpectWord("test");
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "test");

                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }
        [TestMethod]
        public void ExpectWordIdentifierFailUnderscore()
        {
            try { 
                var p = CreateReader("test_ and stuff");
                p.ExpectWord("test");
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "test");

                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }
    }
}
