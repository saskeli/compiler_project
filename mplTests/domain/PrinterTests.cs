using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;

namespace mplTests.domain
{
    [TestClass]
    public class PrinterTests
    {
        private PartMocker _mock;
        private OutMocker _omock;

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

            _omock = new OutMocker(Encoding.ASCII);
        }

        [TestMethod]
        public void PrinterTest()
        {
            Printer pri = new Printer(_mock, 0, 0);
            pri.Add(new Token(TokenType.Name, 0, 0, "bar"));
            pri.Add(new Token(TokenType.Control, 0, 0, "+"));
            pri.Add(new Token(TokenType.String, 0, 0, "rab"));
            pri.Exit();
            Console.SetOut(_omock);
            pri.Run();
            Assert.AreEqual(1, _omock.Output.Count);
            Assert.AreEqual("barrab", _omock.Output[0]);
            _omock.Output.Clear();
        }

        [TestMethod]
        public void RunTest()
        {
            Printer pri = new Printer(_mock, 0, 0);
            pri.Add(new Token(TokenType.Number, 0, 0, "4"));
            pri.Add(new Token(TokenType.Control, 0, 0, "-"));
            pri.Add(new Token(TokenType.Number, 0, 0, "3"));
            pri.Exit();
            Console.SetOut(_omock);
            pri.Run();
            Assert.AreEqual(1, _omock.Output.Count);
            Assert.AreEqual("1", _omock.Output[0]);
            _omock.Output.Clear();
        }

        [TestMethod]
        public void GetParentTest()
        {
            Printer pri = new Printer(_mock, 0, 0);
            Assert.AreSame(_mock, pri.GetParent());
        }

        [TestMethod]
        public void AddTest()
        {
            Printer pri = new Printer(_mock, 0, 0);
            pri.Add(new Token(TokenType.Name, 0, 0, "bla"));
            pri.Add(new Token(TokenType.Control, 0, 0, "&"));
            pri.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            pri.Exit();
            Console.SetOut(_omock);
            pri.Run();
            Assert.AreEqual(1, _omock.Output.Count);
            Assert.AreEqual("False", _omock.Output[0]);
            _omock.Output.Clear();
        }
    }
}