using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using mpl.Exceptions;

namespace mplTests.domain
{
    [TestClass]
    public class AssertionTests
    {
        private PartMocker _mock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mock = new PartMocker();
            _mock.Definitions["bla"] = new Definition(_mock, 0, 0);
            _mock.Definitions["bla"].SetValue(new MplBoolean(false, 0, 0));
            _mock.Definitions["fuu"] = new Definition(_mock, 0, 0);
            _mock.Definitions["fuu"].SetValue(new MplBoolean(true, 0, 0));
            _mock.Scope.Add("bar");
        }

        [TestMethod]
        public void AssertionTest()
        {
            Assertion ass = new Assertion(_mock, 0, 0);
            Part par = ass.GetParent();
            Assert.IsTrue(_mock == par, "The mock instance should be returned by getParent()");
        }

        [TestMethod]
        [ExpectedException(typeof(AssertionException),
            "Assertion failed")]
        public void RunThrowsTest()
        {
            Assertion ass = new Assertion(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Name, 0, 0, "bla"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Exit();
            ass.Run();
        }

        [TestMethod]
        public void RunTest()
        {
            Assertion ass = new Assertion(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Exit();
            ass.Run();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Invalid TokenString for start of assertion body: bla")]
        public void InvalidStartTest()
        {
            Assertion ass = new Assertion(_mock, 0, 0);
            ass.Add(new Token(TokenType.Name, 0, 0, "bla"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Unexpected closing paren detected.")]
        public void BadlyTimedParen()
        {
            Assertion ass = new Assertion(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Line terminator expected. Got bla.")]
        public void RunawayExpression()
        {
            Assertion ass = new Assertion(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Add(new Token(TokenType.Name, 0, 0, "bla"));
        }

        [TestMethod]
        [ExpectedException(typeof(AssertionException),
            "Assertion failed")]
        public void ParenCountingTest()
        {
            Assertion ass = new Assertion(_mock, 0, 0);
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

        [TestMethod]
        public void ExitTest()
        {
            Assertion ass = new Assertion(_mock, 0, 0);
            Assert.IsFalse(ass.Exit());
        }

        [TestMethod]
        public void GetDefinitionTest()
        {
            Assertion ass = new Assertion(_mock, 0, 0);
            Assert.AreEqual(ass.GetDefinition("bla"), _mock.Definitions["bla"]);
            Assert.IsNull(ass.GetDefinition("baz"));
        }
    }
}