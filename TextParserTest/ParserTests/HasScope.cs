using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TextParser;
using static TextParserTest.Helper;

namespace TextParserTest.ParserTests
{
    [TestClass]
    public class HasScope
    {
        class ScopeTest
        {
            public Parser Parser;
            public bool   CallbackCalled;

            public ScopeTest(string lines)
            {
                Parser         = CreateReader(lines);
                CallbackCalled = false;
            }

            public bool FullCallback()
            {
                ActionCallback();
                return true;
            }

            public void ActionCallback()
            {
                CallbackCalled = true;
                Parser.Pop();
            }
        }

        [TestMethod]
        public void NoScope()
        {
            var s = new ScopeTest("no scope");
            var p = s.Parser;
            var r = p.HasScope("("
                             , ")"
                             , s.FullCallback);
            Assert.IsFalse(r);
            Assert.IsFalse(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }

        [TestMethod]
        public void NoScopeAction()
        {
            var s = new ScopeTest("no scope");
            var p = s.Parser;
            var r = p.HasScope("("
                             , ")"
                             , new Action(s.ActionCallback));
            Assert.IsFalse(r);
            Assert.IsFalse(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 1), p.CharacterPosition);
        }

        [TestMethod]
        public void EmptyScope()
        {
            var s = new ScopeTest("()");
            var p = s.Parser;
            var r = p.HasScope("("
                             , ")"
                             , s.FullCallback);
            Assert.IsTrue(r);
            Assert.IsFalse(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }

        [TestMethod]
        public void StuffInScope()
        {
            var s = new ScopeTest("(a)");
            var p = s.Parser;
            var r = p.HasScope("("
                             , ")"
                             , s.FullCallback);
            Assert.IsTrue(r);
            Assert.IsTrue(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }

        [TestMethod]
        public void StuffInScopeFail()
        {
            try
            {
                var p = CreateReader("(a)");
                var r = p.HasScope("("
                                 , ")"
                                 , () => false);
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, ")");

                AssertCharacterPosition(new CharacterPosition(1, 2), e.CharacterPosition);
            }
        }

        [TestMethod]
        public void EmptySingleScope()
        {
            var s = new ScopeTest("''");
            var p = s.Parser;
            var r = p.HasScope("'"
                             , s.FullCallback);
            Assert.IsTrue(r);
            Assert.IsFalse(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }

        [TestMethod]
        public void StuffInSingleScope()
        {
            var s = new ScopeTest("'a'");
            var p = s.Parser;
            var r = p.HasScope("'"
                             , s.FullCallback);
            Assert.IsTrue(r);
            Assert.IsTrue(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }

        [TestMethod]
        public void StuffInSingleScopeFail()
        {
            try
            {
                var p = CreateReader("'a'");
                var r = p.HasScope("'"
                                 , () => false);
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "'");

                AssertCharacterPosition(new CharacterPosition(1, 2), e.CharacterPosition);
            }
        }

        [TestMethod]
        public void ExpectNoScope()
        {
            try
            {
                var s = new ScopeTest("no scope");
                var p = s.Parser;
                p.ExpectScope("("
                            , ")"
                            , s.FullCallback);
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "(");

                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }

        [TestMethod]
        public void ExpectNoScopeAction()
        {
            try
            {
                var s = new ScopeTest("no scope");
                var p = s.Parser;
                p.ExpectScope("("
                            , ")"
                            , new Action(s.ActionCallback));
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "(");

                AssertCharacterPosition(new CharacterPosition(1, 1), e.CharacterPosition);
            }
        }

        [TestMethod]
        public void ExpectEmptyScope()
        {
            var s = new ScopeTest("()");
            var p = s.Parser;
            p.ExpectScope("("
                        , ")"
                        , s.FullCallback);
            Assert.IsFalse(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }

        [TestMethod]
        public void ExpectStuffInScope()
        {
            var s = new ScopeTest("(a)");
            var p = s.Parser;
            p.ExpectScope("("
                        , ")"
                        , s.FullCallback);
            Assert.IsTrue(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }

        [TestMethod]
        public void ExpectStuffInScopeFail()
        {
            try
            {
                var p = CreateReader("(a)");
                p.ExpectScope("("
                            , ")"
                            , () => false);
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, ")");

                AssertCharacterPosition(new CharacterPosition(1, 2), e.CharacterPosition);
            }
        }

        [TestMethod]
        public void ExpectEmptySingleScope()
        {
            var s = new ScopeTest("''");
            var p = s.Parser;
            p.ExpectScope("'"
                        , s.FullCallback);
            Assert.IsFalse(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 3), p.CharacterPosition);
        }

        [TestMethod]
        public void ExpectStuffInSingleScope()
        {
            var s = new ScopeTest("'a'");
            var p = s.Parser;
            p.ExpectScope("'"
                        , s.FullCallback);
            Assert.IsTrue(s.CallbackCalled);
            AssertCharacterPosition(new CharacterPosition(1, 4), p.CharacterPosition);
        }

        [TestMethod]
        public void ExpectStuffInSingleScopeFail()
        {
            try
            {
                var p = CreateReader("'a'");
                p.ExpectScope("'"
                            , () => false);
                Assert.Fail("Expected exception");
            }
            catch (CompilerException e)
            {
                StringAssert.Contains(e.Message, "Expected");
                StringAssert.Contains(e.Message, "'");

                AssertCharacterPosition(new CharacterPosition(1, 2), e.CharacterPosition);
            }
        }
    }
}