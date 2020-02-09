using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using System;
using System.Collections.Generic;
using System.Text;
using mplTests;
using mplTests.domain;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class PrinterTests
    {
        private PartMocker mock;
        private OutMocker omock;

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

            omock = new OutMocker();
        }

        [TestMethod()]
        public void PrinterTest()
        {
            Printer pri = new Printer(mock);
            pri.Add(new Token(TokenType.Name, 0, 0, "bar"));
            pri.Add(new Token(TokenType.Control, 0, 0, "+"));
            pri.Add(new Token(TokenType.String, 0, 0, "rab"));
            pri.Exit();
            Console.SetOut(omock);
            pri.Run();
            Assert.AreEqual(1, omock.Output.Count);
            Assert.AreEqual("barrab", omock.Output[0]);
            omock.Output.Clear();
        }

        [TestMethod()]
        public void RunTest()
        {
            Printer pri = new Printer(mock);
            pri.Add(new Token(TokenType.Number, 0, 0, "4"));
            pri.Add(new Token(TokenType.Control, 0, 0, "-"));
            pri.Add(new Token(TokenType.Number, 0, 0, "3"));
            pri.Exit();
            Console.SetOut(omock);
            pri.Run();
            Assert.AreEqual(1, omock.Output.Count);
            Assert.AreEqual("1", omock.Output[0]);
            omock.Output.Clear();
        }

        [TestMethod()]
        public void GetParentTest()
        {
            Printer pri = new Printer(mock);
            Assert.AreSame(mock, pri.GetParent());
        }

        [TestMethod()]
        public void AddTest()
        {
            Printer pri = new Printer(mock);
            pri.Add(new Token(TokenType.Name, 0, 0, "bla"));
            pri.Add(new Token(TokenType.Control, 0, 0, "&"));
            pri.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            pri.Exit();
            Console.SetOut(omock);
            pri.Run();
            Assert.AreEqual(1, omock.Output.Count);
            Assert.AreEqual("False", omock.Output[0]);
            omock.Output.Clear();
        }
    }
}