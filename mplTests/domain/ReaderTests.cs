using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;

namespace mplTests.domain
{
    [TestClass]
    public class ReaderTests
    {
        private PartMocker _mock;
        private InMocker _imock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mock = new PartMocker();
            Definition def = new Definition(_mock, 0, 0) { Name = "bla" };
            _mock.Definitions["bla"] = def;
            _mock.Definitions["bla"].SetValue(new MplBoolean(false, 0, 0));
            _mock.Definitions["fuu"] = new Definition(_mock, 0, 0);
            _mock.Definitions["fuu"].SetValue(new MplInteger(2, 0, 0));
            _mock.Definitions["bar"] = new Definition(_mock, 0, 0) { Name = "bar" };
            _mock.Definitions["bar"].SetValue(new MplString("bar", 0, 0));
            _mock.Definitions["baz"] = new Definition(_mock, 0, 0) { Name = "baz" };
            _mock.Definitions["baz"].SetValue(new MplString("baz", 0, 0));

            _imock = new InMocker();
        }

        [TestMethod]
        public void ReaderTest()
        {
            _imock.Inp.Add("POTATO");
            Reader rea = new Reader(_mock, 0, 0);
            rea.Add(new Token(TokenType.Name, 0, 0, "bar"));
            rea.Exit();
            Console.SetIn(_imock);
            rea.Run();
            string v = ((MplString) _mock.Definitions["bar"].GetValue()).Val;
            Assert.AreEqual("POTATO", v);
        }

        [TestMethod]
        public void RunTest()
        {
            _imock.Inp.Add("14");
            Reader rea = new Reader(_mock, 0, 0);
            rea.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            rea.Exit();
            Console.SetIn(_imock);
            rea.Run();
            int v = ((MplInteger) _mock.Definitions["fuu"].GetValue()).Val;
            Assert.AreEqual(14, v);
        }

        [TestMethod]
        public void GetParentTest()
        {
            Reader rea = new Reader(_mock, 0, 0);
            Assert.AreSame(_mock, rea.GetParent());
        }

        [TestMethod]
        public void AddTest()
        {
            _imock.Inp.Add("True");
            Reader rea = new Reader(_mock, 0, 0);
            rea.Add(new Token(TokenType.Name, 0, 0, "bla"));
            rea.Exit();
            Console.SetIn(_imock);
            rea.Run();
            bool v = ((MplBoolean)_mock.Definitions["bla"].GetValue()).Val;
            Assert.IsTrue(v);
        }

    }
}