using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using mplTests;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class LoopTests
    {
        private PartMocker mock;

        [TestInitialize]
        public void TestInitialize()
        {
            mock = new PartMocker();
            mock.Definitions["bla"] = new Definition(mock, 0, 0) { Name = "bla" };
            mock.Definitions["bla"].SetValue(new MplInteger(0, 0, 0));
            mock.Definitions["fuu"] = new Definition(mock, 0, 0) { Name = "fuu" };
            mock.Definitions["fuu"].SetValue(new MplInteger(0, 0, 0));
        }

        [TestMethod()]
        public void LoopTest()
        {
            Loop loo = new Loop(mock, 0, 0);
            loo.Add(new Token(TokenType.Name, 0, 0, "bla"));
            loo.Add(new Token(TokenType.Name, 0, 0, "in"));
            loo.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            loo.Add(new Token(TokenType.Control, 0, 0, ".."));
            loo.Add(new Token(TokenType.Number, 0, 0, "4"));
            loo.Add(new Token(TokenType.Control, 0, 0, "+"));
            loo.Add(new Token(TokenType.Number, 0, 0, "2"));
            loo.Add(new Token(TokenType.Name, 0, 0, "do"));
            loo.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            loo.Add(new Token(TokenType.Control, 0, 0, ":="));
            loo.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            loo.Add(new Token(TokenType.Control, 0, 0, "+"));
            loo.Add(new Token(TokenType.Name, 0, 0, "bla"));
            loo.Exit();
            loo.Add(new Token(TokenType.Name, 0, 0, "end"));
            loo.Add(new Token(TokenType.Name, 0, 0, "for"));
            loo.Exit();
            loo.Run();
            int v = ((MplInteger) mock.Definitions["bla"].GetValue()).Val;
            Assert.AreEqual(6, v);
            v = ((MplInteger) mock.Definitions["fuu"].GetValue()).Val;
            Assert.AreEqual(21, v);
        }

        [TestMethod()]
        public void RunTest()
        {
            Loop loo = new Loop(mock, 0, 0);
            loo.Add(new Token(TokenType.Name, 0, 0, "bla"));
            loo.Add(new Token(TokenType.Name, 0, 0, "in"));
            loo.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            loo.Add(new Token(TokenType.Control, 0, 0, ".."));
            loo.Add(new Token(TokenType.Number, 0, 0, "4"));
            loo.Add(new Token(TokenType.Control, 0, 0, "-"));
            loo.Add(new Token(TokenType.Number, 0, 0, "8"));
            loo.Add(new Token(TokenType.Name, 0, 0, "do"));
            loo.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            loo.Add(new Token(TokenType.Control, 0, 0, ":="));
            loo.Add(new Token(TokenType.Name, 0, 0, "fuu"));
            loo.Add(new Token(TokenType.Control, 0, 0, "+"));
            loo.Add(new Token(TokenType.Name, 0, 0, "bla"));
            loo.Exit();
            loo.Add(new Token(TokenType.Name, 0, 0, "end"));
            loo.Add(new Token(TokenType.Name, 0, 0, "for"));
            loo.Exit();
            loo.Run();
            int v = ((MplInteger)mock.Definitions["bla"].GetValue()).Val;
            Assert.AreEqual(-4, v);
            v = ((MplInteger)mock.Definitions["fuu"].GetValue()).Val;
            Assert.AreEqual(-10, v);
        }

        [TestMethod()]
        public void GetParentTest()
        {
            Loop loo = new Loop(mock, 0, 0);
            Assert.AreSame(mock, loo.GetParent());
        }

    }
}