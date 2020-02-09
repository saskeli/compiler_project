using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl.domain;
using System;
using System.Collections.Generic;
using System.Text;
using mplTests;

namespace mpl.domain.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        private OutMocker omock;
        private InMocker imock;

        [TestInitialize]
        public void TestInitialize()
        {
            omock = new OutMocker();
            imock = new InMocker();
        }

        [TestMethod()]
        public void RunTest1()
        {
            Program prog = new Program();

            prog.Add(new Token(TokenType.Name, 0, 0, "var"));
            prog.Add(new Token(TokenType.Name, 0, 0, "X"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":"));
            prog.Add(new Token(TokenType.Name, 0, 0, "int"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":="));
            prog.Add(new Token(TokenType.Number, 0, 0, "4"));
            prog.Add(new Token(TokenType.Control, 0, 0, "+"));
            prog.Add(new Token(TokenType.Control, 0, 0, "("));
            prog.Add(new Token(TokenType.Number, 0, 0, "6"));
            prog.Add(new Token(TokenType.Control, 0, 0, "*"));
            prog.Add(new Token(TokenType.Number, 0, 0, "2"));
            prog.Add(new Token(TokenType.Control, 0, 0, ")"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "print"));
            prog.Add(new Token(TokenType.Name, 0, 0, "X"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            Console.SetOut(omock);
            prog.Run();
            Assert.AreEqual(1, omock.Output.Count);
            Assert.AreEqual("16", omock.Output[0]);
        }

        [TestMethod()]
        public void RunTest2()
        {
            Program prog = new Program();
            prog.Add(new Token(TokenType.Name, 0, 0, "var"));
            prog.Add(new Token(TokenType.Name, 0, 0, "nTimes"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":"));
            prog.Add(new Token(TokenType.Name, 0, 0, "int"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":="));
            prog.Add(new Token(TokenType.Number, 0, 0, "0"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "print"));
            prog.Add(new Token(TokenType.String, 0, 0, "How many Times? "));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "read"));
            prog.Add(new Token(TokenType.Name, 0, 0, "nTimes"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "var"));
            prog.Add(new Token(TokenType.Name, 0, 0, "x"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":"));
            prog.Add(new Token(TokenType.Name, 0, 0, "int"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "for"));
            prog.Add(new Token(TokenType.Name, 0, 0, "x"));
            prog.Add(new Token(TokenType.Name, 0, 0, "in"));
            prog.Add(new Token(TokenType.Number, 0, 0, "0"));
            prog.Add(new Token(TokenType.Control, 0, 0, ".."));
            prog.Add(new Token(TokenType.Name, 0, 0, "nTimes"));
            prog.Add(new Token(TokenType.Control, 0, 0, "-"));
            prog.Add(new Token(TokenType.Number, 0, 0, "1"));
            prog.Add(new Token(TokenType.Name, 0, 0, "do"));
            prog.Add(new Token(TokenType.Name, 0, 0, "print"));
            prog.Add(new Token(TokenType.Name, 0, 0, "x"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "print"));
            prog.Add(new Token(TokenType.String, 0, 0, " : Hello, World!\n"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "end"));
            prog.Add(new Token(TokenType.Name, 0, 0, "for"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "assert"));
            prog.Add(new Token(TokenType.Control, 0, 0, "("));
            prog.Add(new Token(TokenType.Name, 0, 0, "x"));
            prog.Add(new Token(TokenType.Control, 0, 0, "="));
            prog.Add(new Token(TokenType.Control, 0, 0, "("));
            prog.Add(new Token(TokenType.Name, 0, 0, "nTimes"));
            prog.Add(new Token(TokenType.Control, 0, 0, "-"));
            prog.Add(new Token(TokenType.Number, 0, 0, "1"));
            prog.Add(new Token(TokenType.Control, 0, 0, ")"));
            prog.Add(new Token(TokenType.Control, 0, 0, ")"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));

            imock.SetInput(new string[]{"4"});
            Console.SetIn(imock);
            Console.SetOut(omock);
            prog.Run();
            Assert.AreEqual(9, omock.Output.Count);
            string[] expectedOutput = new string[]
            {
                "How many Times? ",
                "0",
                " : Hello, World!\n",
                "1",
                " : Hello, World!\n",
                "2",
                " : Hello, World!\n",
                "3",
                " : Hello, World!\n"
            };
            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], omock.Output[i]);
            }
        }

        [TestMethod()]
        public void RunTest3()
        {
            Program prog = new Program();
            prog.Add(new Token(TokenType.Name, 0, 0, "print"));
            prog.Add(new Token(TokenType.String, 0, 0, "Give a number "));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "var"));
            prog.Add(new Token(TokenType.Name, 0, 0, "n"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":"));
            prog.Add(new Token(TokenType.Name, 0, 0, "int"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "read"));
            prog.Add(new Token(TokenType.Name, 0, 0, "n"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "var"));
            prog.Add(new Token(TokenType.Name, 0, 0, "v"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":"));
            prog.Add(new Token(TokenType.Name, 0, 0, "int"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":="));
            prog.Add(new Token(TokenType.Number, 0, 0, "1"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "var"));
            prog.Add(new Token(TokenType.Name, 0, 0, "i"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":"));
            prog.Add(new Token(TokenType.Name, 0, 0, "int"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "for"));
            prog.Add(new Token(TokenType.Name, 0, 0, "i"));
            prog.Add(new Token(TokenType.Name, 0, 0, "in"));
            prog.Add(new Token(TokenType.Number, 0, 0, "1"));
            prog.Add(new Token(TokenType.Control, 0, 0, ".."));
            prog.Add(new Token(TokenType.Name, 0, 0, "n"));
            prog.Add(new Token(TokenType.Name, 0, 0, "do"));
            prog.Add(new Token(TokenType.Name, 0, 0, "v"));
            prog.Add(new Token(TokenType.Control, 0, 0, ":="));
            prog.Add(new Token(TokenType.Name, 0, 0, "v"));
            prog.Add(new Token(TokenType.Control, 0, 0, "*"));
            prog.Add(new Token(TokenType.Name, 0, 0, "i"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "end"));
            prog.Add(new Token(TokenType.Name, 0, 0, "for"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "print"));
            prog.Add(new Token(TokenType.String, 0, 0, "The result is: "));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));
            prog.Add(new Token(TokenType.Name, 0, 0, "print"));
            prog.Add(new Token(TokenType.Name, 0, 0, "v"));
            prog.Add(new Token(TokenType.Control, 0, 0, ";"));

            imock.SetInput(new string[] { "5" });
            Console.SetIn(imock);
            Console.SetOut(omock);
            prog.Run();
            Assert.AreEqual(3, omock.Output.Count);
            string[] expectedOutput = new string[]
            {
                "Give a number ",
                "The result is: ",
                "120"
            };
            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], omock.Output[i]);
            }
        }
        
    }
}