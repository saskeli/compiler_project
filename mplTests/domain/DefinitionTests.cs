using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;

namespace mplTests.domain
{
    [TestClass]
    public class DefinitionTests
    {
        private PartMocker _mock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mock = new PartMocker();
            Definition def = new Definition(_mock, 0, 0) { Name = "bla" };
            _mock.Definitions["bla"] = def;
            _mock.Definitions["bla"].SetValue(new MplBoolean(false, 0, 0));
            _mock.Definitions["fuu"] = new Definition(_mock, 0, 0);
            _mock.Definitions["fuu"].SetValue(new MplBoolean(true, 0, 0));
            _mock.Definitions["bar"] = new Definition(_mock, 0, 0) { Name = "bar" };
            _mock.Definitions["bar"].SetValue(new MplString("bar", 0, 0));
            _mock.Definitions["baz"] = new Definition(_mock, 0, 0) { Name = "baz" };
            _mock.Definitions["baz"].SetValue(new MplString("baz", 0, 0));
        }

        [TestMethod]
        public void DefinitionTest()
        {
            Definition def = new Definition(_mock, 4, 0, 0);
            Assert.IsTrue(def.GetValue() is MplInteger);
            Assert.AreEqual(4, ((MplInteger)def.GetValue()).Val);

            def = new Definition(_mock, "bla", 0, 0);
            Assert.IsTrue(def.GetValue() is MplString);
            Assert.AreEqual("bla", ((MplString)def.GetValue()).Val);
        }

        [TestMethod]
        public void RunTest()
        {
            Definition def = new Definition(_mock, 0, 0);
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

        [TestMethod]
        public void GetParentTest()
        {
            Definition def = new Definition(_mock, 0, 0);
            Assert.IsTrue(_mock == def.GetParent());
        }

        [TestMethod]
        public void ExitTest()
        {
            Definition def = new Definition(_mock, 0, 0);
            def.Add(new Token(TokenType.Name, 0, 0, "Alice"));
            def.Add(new Token(TokenType.Control, 0, 0, ":"));
            def.Add(new Token(TokenType.Name, 0, 0, "bool"));
            def.Exit();
            def.Run();
            Assert.IsFalse(((MplBoolean) def.GetValue()).Val);
        }

        [TestMethod]
        public void GetDefinitionTest()
        {
            Definition def = new Definition(_mock, 0, 0);
            Assert.AreEqual(_mock.Definitions["bla"], def.GetDefinition("bla"));
            Assert.IsNull(def.GetDefinition("Alice"));
        }

        [TestMethod]
        public void GetValueTest()
        {
            Definition def = new Definition(_mock, 0, 0);
            def.Add(new Token(TokenType.Name, 0, 0, "Alice"));
            def.Add(new Token(TokenType.Control, 0, 0, ":"));
            def.Add(new Token(TokenType.Name, 0, 0, "string"));
            Assert.AreEqual("", ((MplString) def.GetValue()).Val);
        }

        [TestMethod]
        public void SetValueTest()
        {
            Definition def = new Definition(_mock, 0, 0);
            def.Add(new Token(TokenType.Name, 0, 0, "Alice"));
            def.Add(new Token(TokenType.Control, 0, 0, ":"));
            def.Add(new Token(TokenType.Name, 0, 0, "string"));
            def.Exit();
            def.Run();
            def.SetValue(new MplString("Tomaatti", 0, 0));
            Assert.AreEqual("Tomaatti", ((MplString)def.GetValue()).Val);
        }

        [TestMethod]
        public void GetTypeTest()
        {
            Definition def = new Definition(_mock, 0, 0);
            def.Add(new Token(TokenType.Name, 0, 0, "Alice"));
            def.Add(new Token(TokenType.Control, 0, 0, ":"));
            def.Add(new Token(TokenType.Name, 0, 0, "string"));
            Assert.AreEqual(Type.String, def.GetType());
        }
    }
}