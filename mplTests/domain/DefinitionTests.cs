using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using mplTests.domain;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class DefinitionTests
    {
        private PartMocker mock;

        [TestInitialize]
        public void TestInitialize()
        {
            mock = new PartMocker();
            Definition def = new Definition(mock) { Name = "bla" };
            mock.Definitions["bla"] = def;
            mock.Definitions["bla"].SetValue(new MplBoolean(false));
            mock.Definitions["fuu"] = new Definition(mock);
            mock.Definitions["fuu"].SetValue(new MplBoolean(true));
            mock.Definitions["bar"] = new Definition(mock) { Name = "bar" };
            mock.Definitions["bar"].SetValue(new MplString("bar"));
            mock.Definitions["baz"] = new Definition(mock) { Name = "baz" };
            mock.Definitions["baz"].SetValue(new MplString("baz"));
        }

        [TestMethod()]
        public void DefinitionTest()
        {
            Definition def = new Definition(mock, 4);
            Assert.IsTrue(def.GetValue() is MplInteger);
            Assert.AreEqual(4, ((MplInteger)def.GetValue()).Val);

            def = new Definition(mock, "bla");
            Assert.IsTrue(def.GetValue() is MplString);
            Assert.AreEqual("bla", ((MplString)def.GetValue()).Val);
        }

        [TestMethod()]
        public void RunTest()
        {
            Definition def = new Definition(mock);
            def.Add(new Token(TokenType.Name, 0, 0, "Nom"));
            def.Add(new Token(TokenType.Control, 0, 0, ":"));
            def.Add(new Token(TokenType.Name, 0, 0, "string"));
            def.Add(new Token(TokenType.Control, 0, 0, ":="));
            def.Add(new Token(TokenType.Number, 0, 0, "1"));
            def.Add(new Token(TokenType.Control, 0, 0, "+"));
            def.Add(new Token(TokenType.Number, 0, 0, "1"));
            def.Exit();
            def.Run();
            Assert.AreEqual(2, ((MplInteger) def.GetValue()).Val);
        }

        [TestMethod()]
        public void GetParentTest()
        {
            Definition def = new Definition(mock);
            Assert.IsTrue(mock == def.GetParent());
        }

        [TestMethod()]
        public void ExitTest()
        {
            Definition def = new Definition(mock);
            def.Add(new Token(TokenType.Name, 0, 0, "Alice"));
            def.Add(new Token(TokenType.Control, 0, 0, ":"));
            def.Add(new Token(TokenType.Name, 0, 0, "bool"));
            def.Exit();
            def.Run();
            Assert.IsFalse(((MplBoolean) def.GetValue()).Val);
        }

        [TestMethod()]
        public void GetDefinitionTest()
        {
            Definition def = new Definition(mock);
            Assert.AreEqual(mock.Definitions["bla"], def.GetDefinition("bla"));
            Assert.IsNull(def.GetDefinition("Alice"));
        }

        [TestMethod()]
        public void GetValueTest()
        {
            Definition def = new Definition(mock);
            def.Add(new Token(TokenType.Name, 0, 0, "Alice"));
            def.Add(new Token(TokenType.Control, 0, 0, ":"));
            def.Add(new Token(TokenType.Name, 0, 0, "string"));
            Assert.AreEqual("", ((MplString) def.GetValue()).Val);
        }

        [TestMethod()]
        public void SetValueTest()
        {
            Definition def = new Definition(mock);
            def.Add(new Token(TokenType.Name, 0, 0, "Alice"));
            def.Add(new Token(TokenType.Control, 0, 0, ":"));
            def.Add(new Token(TokenType.Name, 0, 0, "string"));
            def.Exit();
            def.Run();
            def.SetValue(new MplString("Tomaatti"));
            Assert.AreEqual("Tomaatti", ((MplString)def.GetValue()).Val);
        }

        [TestMethod()]
        public void GetTypeTest()
        {
            Definition def = new Definition(mock);
            def.Add(new Token(TokenType.Name, 0, 0, "Alice"));
            def.Add(new Token(TokenType.Control, 0, 0, ":"));
            def.Add(new Token(TokenType.Name, 0, 0, "string"));
            Assert.AreEqual(Type.String, def.GetType());
        }
    }
}