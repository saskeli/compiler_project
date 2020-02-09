using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using System;
using System.Collections.Generic;
using System.Text;
using mplTests;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class ReaderTests
    {
        private PartMocker mock;
        private InMocker imock;

        [TestInitialize]
        public void TestInitialize()
        {
            mock = new PartMocker();
            Definition def = new Definition(mock, 0, 0) { Name = "bla" };
            mock.Definitions["bla"] = def;
            mock.Definitions["bla"].SetValue(new MplBoolean(false));
            mock.Definitions["fuu"] = new Definition(mock, 0, 0);
            mock.Definitions["fuu"].SetValue(new MplInteger(2, 0, 0));
            mock.Definitions["bar"] = new Definition(mock, 0, 0) { Name = "bar" };
            mock.Definitions["bar"].SetValue(new MplString("bar"));
            mock.Definitions["baz"] = new Definition(mock, 0, 0) { Name = "baz" };
            mock.Definitions["baz"].SetValue(new MplString("baz"));

            imock = new InMocker();
        }

        [TestMethod()]
        public void ReaderTest()
        {
            imock.Inp.Add("POTATO");
            Reader rea = new Reader(mock, 0, 0);
            rea.Add(new Token(TokenType.Name, 0, 0, "bar"));
            rea.Exit();
            Console.SetIn(imock);
            rea.Run();
            string v = ((MplString) mock.Definitions["bar"].GetValue()).Val;
            Assert.AreEqual("POTATO", v);
        }

        [TestMethod()]
        public void RunTest()
        {
            imock.Inp.Add("14");
            Reader rea = new Reader(mock, 0, 0);
            rea.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            rea.Exit();
            Console.SetIn(imock);
            rea.Run();
            int v = ((MplInteger) mock.Definitions["fuu"].GetValue()).Val;
            Assert.AreEqual(14, v);
        }

        [TestMethod()]
        public void GetParentTest()
        {
            Reader rea = new Reader(mock, 0, 0);
            Assert.AreSame(mock, rea.GetParent());
        }

        [TestMethod()]
        public void AddTest()
        {
            imock.Inp.Add("True");
            Reader rea = new Reader(mock, 0, 0);
            rea.Add(new Token(TokenType.Name, 0, 0, "bla"));
            rea.Exit();
            Console.SetIn(imock);
            rea.Run();
            bool v = ((MplBoolean)mock.Definitions["bla"].GetValue()).Val;
            Assert.IsTrue(v);
        }

    }
}