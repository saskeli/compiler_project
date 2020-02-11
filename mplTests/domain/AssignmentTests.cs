using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using mpl.Exceptions;

namespace mplTests.domain
{
    [TestClass]
    public class AssignmentTests
    {
        private PartMocker _mock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mock = new PartMocker();
            Definition def = new Definition(_mock, 0, 0) {Name = "bla"};
            _mock.Definitions["bla"] = def;
            _mock.Definitions["bla"].SetValue(new MplBoolean(false, 0, 0));
            _mock.Definitions["fuu"] = new Definition(_mock, 0, 0);
            _mock.Definitions["fuu"].SetValue(new MplBoolean(true, 0, 0));
            _mock.Definitions["bar"] = new Definition(_mock, 0, 0) {Name = "bar"};
            _mock.Definitions["bar"].SetValue(new MplString("bar", 0, 0));
            _mock.Definitions["baz"] = new Definition(_mock, 0, 0) { Name = "baz" };
            _mock.Definitions["baz"].SetValue(new MplString("baz", 0, 0));
        }

        [TestMethod]
        public void AssignmentTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            Part par = ass.GetParent();
            Assert.IsTrue(_mock == par, "The mock instance should be returned by getParent()");
        }

        [TestMethod]
        public void AssignmentOverloadTest()
        {
            Definition d = new Definition(_mock, 0, 0) {Name = "bar"};
            d.SetValue(new MplInteger(4, 0, 0));
            _mock.Definitions["bar"] = d;
            Assignment ass = new Assignment(_mock.GetDefinition("bar"), _mock, 0, 0);
            Part par = ass.GetParent();
            Assert.IsTrue(_mock == par, "The mock instance should be returned by getParent()");
            ass.Add(new Token(TokenType.Number, 0, 0, "6"));
            ass.Exit();
            ass.Run();
            int v = ((MplInteger) d.GetValue()).Val;
            Assert.IsTrue(v == 6, $"Val should be 6. Got {v}.");
        }

        [TestMethod]
        public void NegationTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Control,  0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "="));
            ass.Add(new Token(TokenType.Number, 0, 0, "2"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Exit();
            ass.Run();
            bool v = ((MplBoolean) ass.Value).Val;
            Assert.IsTrue(v, $"!(2 = 1) should be true. Got {v}");
        }

        [TestMethod]
        public void SimpleAssignmentTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Number, 0, 0, "2"));
            ass.Exit();
            ass.Run();
            int v = ((MplInteger) ass.Value).Val;
            Assert.AreEqual(2, v);
        }

        [TestMethod]
        public void TestLess()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "<"));
            ass.Add(new Token(TokenType.Number, 0, 0, "2"));
            ass.Exit();
            ass.Run();
            bool v = ((MplBoolean) ass.Value).Val;
            Assert.IsTrue(v);
        }

        [TestMethod]
        public void DeepAssignmentTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));

            ass.Add(new Token(TokenType.Control, 0, 0, "+"));

            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));

            ass.Exit();
            ass.Run();
            int v = ((MplInteger) ass.Value).Val;
            Assert.AreEqual(6, v);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected line terminator. Not 4")]
        public void CompletedAssignmentAddTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.String, 0, 0, "fuu"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.String, 0, 0, "bar"));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Invalid TokenString for binary operator )")]
        public void Subpart2HangingParen()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.String, 0, 0, "bar"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod]
        public void NamedSecondOperandTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Name, 0, 0, "bar"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Name, 0, 0, "baz"));
            ass.Exit();
            ass.Run();
            string v = ((MplString) ass.Value).Val;
            Assert.AreEqual("barbaz", v);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Invalid closing paren\"")]
        public void Sub2BadParenTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "2"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Invalid closing paren\"")]
        public void Sub1BadParenTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Use of undefined variable Alice\"")]
        public void UndefinedSecondOperand()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Name, 0, 0, "bar"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Name, 0, 0, "Alice"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Use of undefined variable Alice\"")]
        public void UndefinedFirstOperand()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Name, 0, 0, "Alice"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Expected binary operator. Got Alice\"")]
        public void InvalidBinaryOperatorTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Number, 0, 0, "15"));
            ass.Add(new Token(TokenType.Name, 0, 0, "Alice"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Expected binary operator. Got )\"")]
        public void InvalidOperatorTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Number, 0, 0, "15"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Invalid TokenString for body of negation )\"")]
        public void InvalidTokenInNegationTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod]
        public void NegationOfDefinition()
        {
            Assignment ass = new Assignment(_mock.Definitions["bla"], _mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Name, 0, 0, "bla"));
            ass.Exit();
            ass.Run();
            Assert.IsTrue(((MplBoolean)_mock.Definitions["bla"].GetValue()).Val);
            Assert.IsTrue(((MplBoolean) ass.Value).Val);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Use of undefined variable Alice\"")]
        public void UndefinedVariableInNegationTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Name, 0, 0, "Alice"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"bar is not boolean. Invalid type for negation\"")]
        public void InvalidVariableInNegationTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Name, 0, 0, "bar"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"4 is not boolean.\"")]
        public void InvalidLiteralInNegationTest()
        {
            Assignment ass = new Assignment(_mock, 0, 0);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
        }
    }
}