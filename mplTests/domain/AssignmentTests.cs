using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using System;
using System.Collections.Generic;
using System.Text;
using mpl.Exceptions;
using mplTests.domain;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class AssignmentTests
    {
        private PartMocker mock;

        [TestInitialize]
        public void TestInitialize()
        {
            mock = new PartMocker();
            Definition def = new Definition(mock) {Name = "bla"};
            mock.Definitions["bla"] = def;
            mock.Definitions["bla"].SetValue(new MplBoolean(false));
            mock.Definitions["fuu"] = new Definition(mock);
            mock.Definitions["fuu"].SetValue(new MplBoolean(true));
            mock.Definitions["bar"] = new Definition(mock) {Name = "bar"};
            mock.Definitions["bar"].SetValue(new MplString("bar"));
            mock.Definitions["baz"] = new Definition(mock) { Name = "baz" };
            mock.Definitions["baz"].SetValue(new MplString("baz"));
        }

        [TestMethod()]
        public void AssignmentTest()
        {
            Assignment ass = new Assignment(mock);
            Part par = ass.GetParent();
            Assert.IsTrue(mock == par, "The mock instance should be returned by getParent()");
        }

        [TestMethod()]
        public void AssignmentOverloadTest()
        {
            Definition d = new Definition(mock) {Name = "bar"};
            d.SetValue(new MplInteger(4));
            mock.Definitions["bar"] = d;
            Assignment ass = new Assignment(mock.GetDefinition("bar"), mock);
            Part par = ass.GetParent();
            Assert.IsTrue(mock == par, "The mock instance should be returned by getParent()");
            ass.Add(new Token(TokenType.Number, 0, 0, "6"));
            ass.Exit();
            ass.Run();
            int v = ((MplInteger) d.GetValue()).Val;
            Assert.IsTrue(v == 6, $"Val should be 6. Got {v}.");
        }

        [TestMethod()]
        public void NegationTest()
        {
            Assignment ass = new Assignment(mock);
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

        [TestMethod()]
        public void SimpleAssignmentTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Number, 0, 0, "2"));
            ass.Exit();
            ass.Run();
            int v = ((MplInteger) ass.Value).Val;
            Assert.AreEqual(2, v);
        }

        [TestMethod()]
        public void TestLess()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "<"));
            ass.Add(new Token(TokenType.Number, 0, 0, "2"));
            ass.Exit();
            ass.Run();
            bool v = ((MplBoolean) ass.Value).Val;
            Assert.IsTrue(v);
        }

        [TestMethod()]
        public void DeepAssignmentTest()
        {
            Assignment ass = new Assignment(mock);
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

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected line terminator. Not 4")]
        public void CompletedAssignmentAddTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.String, 0, 0, "fuu"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.String, 0, 0, "bar"));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Invalid token for binary operator )")]
        public void Subpart2HangingParen()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.String, 0, 0, "bar"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod()]
        public void NamedSecondOperandTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Name, 0, 0, "bar"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Name, 0, 0, "baz"));
            ass.Exit();
            ass.Run();
            string v = ((MplString) ass.Value).Val;
            Assert.AreEqual("barbaz", v);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Invalid closing paren\"")]
        public void Sub2BadParenTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "2"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Invalid closing paren\"")]
        public void Sub1BadParenTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "("));
            ass.Add(new Token(TokenType.Number, 0, 0, "1"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Use of undefined variable Alice\"")]
        public void UndefinedSecondOperand()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Name, 0, 0, "bar"));
            ass.Add(new Token(TokenType.Control, 0, 0, "+"));
            ass.Add(new Token(TokenType.Name, 0, 0, "Alice"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Use of undefined variable Alice\"")]
        public void UndefinedFirstOperand()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Name, 0, 0, "Alice"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Expected binary operator. Got Alice\"")]
        public void InvalidBinaryOperatorTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Number, 0, 0, "15"));
            ass.Add(new Token(TokenType.Name, 0, 0, "Alice"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Expected binary operator. Got )\"")]
        public void InvalidOperatorTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Number, 0, 0, "15"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Invalid token for body of negation )\"")]
        public void InvalidTokenInNegationTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Control, 0, 0, ")"));
        }

        [TestMethod()]
        public void NegationOfDefinition()
        {
            Assignment ass = new Assignment(mock.Definitions["bla"], mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Name, 0, 0, "bla"));
            ass.Exit();
            ass.Run();
            Assert.IsTrue(((MplBoolean)mock.Definitions["bla"].GetValue()).Val);
            Assert.IsTrue(((MplBoolean) ass.Value).Val);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"Use of undefined variable Alice\"")]
        public void UndefinedVariableInNegationTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Name, 0, 0, "Alice"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"bar is not boolean. Invalid type for negation\"")]
        public void InvalidVariableInNegationTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Name, 0, 0, "bar"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSyntaxException),
            "Expected \"4 is not boolean.\"")]
        public void InvalidLiteralInNegationTest()
        {
            Assignment ass = new Assignment(mock);
            ass.Add(new Token(TokenType.Control, 0, 0, "!"));
            ass.Add(new Token(TokenType.Number, 0, 0, "4"));
        }
    }
}