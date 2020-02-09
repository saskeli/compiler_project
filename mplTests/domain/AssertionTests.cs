using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using mpl.Exceptions;
using mplTests.domain;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class AssertionTests
    {
        private PartMocker mock;

        [TestInitialize]
        public void TestInitialize()
        {
            mock = new PartMocker();
            mock.Definitions["bla"] = new Definition(mock);
            mock.Definitions["bla"].SetValue(new MplBoolean(false));
            mock.Definitions["fuu"] = new Definition(mock);
            mock.Definitions["fuu"].SetValue(new MplBoolean(true));
            mock.Scope.Add("bar");
        }

        [TestMethod()]
        public void AssertionTest()
        {
            Assertion ass = new Assertion(mock);
            Part par = ass.GetParent();
            Assert.IsTrue(mock == par, "The mock instance should be returned by getParent()");
        }

        [TestMethod()]
        [ExpectedException(typeof(AssertionException),
            "Assertion failed")]
        public void RunThrowsTest()
        {
            Assertion ass = new Assertion(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Name, 0, 0, "bla"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Exit();
            ass.Run();
        }

        [TestMethod()]
        public void RunTest()
        {
            Assertion ass = new Assertion(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Exit();
            ass.Run();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Invalid token for start of assertion body: bla")]
        public void InvalidStartTest()
        {
            Assertion ass = new Assertion(mock);
            ass.Add(new Token(TokenType.Name, 0, 0, "bla"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Unexpected closing paren detected.")]
        public void BadlyTimedParen()
        {
            Assertion ass = new Assertion(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Line terminator expected. Got bla.")]
        public void RunawayExpression()
        {
            Assertion ass = new Assertion(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Add(new Token(TokenType.Name, 0, 0, "bla"));
        }

        [TestMethod()]
        [ExpectedException(typeof(AssertionException),
            "Assertion failed")]
        public void ParenCountingTest()
        {
            Assertion ass = new Assertion(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Add(new Token(TokenType.Control, 0, 0, "="));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Exit();
            ass.Run();
        }

        [TestMethod()]
        public void ExitTest()
        {
            Assertion ass = new Assertion(mock);
            Assert.IsFalse(ass.Exit());
        }

        [TestMethod()]
        public void GetDefinitionTest()
        {
            Assertion ass = new Assertion(mock);
            Assert.AreEqual(ass.GetDefinition("bla"), mock.Definitions["bla"]);
            Assert.IsNull(ass.GetDefinition("baz"));
        }
    }
}