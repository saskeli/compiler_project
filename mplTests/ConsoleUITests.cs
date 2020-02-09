using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using mplTests;

namespace mpl.Tests
{
    [TestClass()]
    public class ConsoleUITests
    {
        [TestMethod()]
        public void MainTest1()
        {
            string inp = "var X : int := 4 + (6 * 2);\nprint X;\n";
            using (StreamWriter sw = new StreamWriter("test.mpl"))
            {
                sw.Write(inp);
            }
            OutMocker omock = new OutMocker();
            Console.SetOut(omock);
            InMocker imock = new InMocker();
            Console.SetIn(imock);
            ConsoleUI.Main(new[]{"test.mpl"});
            Assert.AreEqual(1, omock.Output.Count);
            Assert.AreEqual("16", omock.Output[0]);
        }

        [TestMethod()]
        public void MainTest2()
        {
            string inp = @"var nTimes : int := 0;
print ""How many times ? "";
read nTimes;
var x : int;
for x in 0..nTimes-1 do
    print x;
    print "" : Hello, World!\n"";
end for;
assert (x = (nTimes - 1));";
            using (StreamWriter sw = new StreamWriter("test.mpl"))
            {
                sw.Write(inp);
            }
            OutMocker omock = new OutMocker();
            Console.SetOut(omock);
            InMocker imock = new InMocker(new []{"1"});
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOuput = {
                "How many times ? ",
                "0",
                " : Hello, World!\n"
            };
            Assert.AreEqual(expectedOuput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOuput.Length; i++)
            {
                Assert.AreEqual(expectedOuput[i], omock.Output[i]);
            }
            
        }

        [TestMethod()]
        public void MainTest3()
        {
            string inp = @"print ""Give a number"";
var n : int;
read n;
var v : int := 1;
var i : int;
for i in 1..n do
    v := v * i;
end for;
print ""The result is: "";
print v;";
            using (StreamWriter sw = new StreamWriter("test.mpl"))
            {
                sw.Write(inp);
            }
            OutMocker omock = new OutMocker();
            Console.SetOut(omock);
            InMocker imock = new InMocker(new[] { "4" });
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOuput = {
                "Give a number",
                "The result is: ",
                "24"
            };
            Assert.AreEqual(expectedOuput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOuput.Length; i++)
            {
                Assert.AreEqual(expectedOuput[i], omock.Output[i]);
            }

        }

        [TestMethod()]
        public void MainTest4()
        {
            string inp = @"var X : int; //Loop var;
var sum : int;
for X in /*here is a range*/ 0..14 do
 /* We are simply counting
    the sum over the range*/
    sum := sum + X;
end for;
print sum;";
            using (StreamWriter sw = new StreamWriter("test.mpl"))
            {
                sw.Write(inp);
            }
            OutMocker omock = new OutMocker();
            Console.SetOut(omock);
            InMocker imock = new InMocker();
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOuput = {
                "105"
            };
            Assert.AreEqual(expectedOuput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOuput.Length; i++)
            {
                Assert.AreEqual(expectedOuput[i], omock.Output[i]);
            }

        }
    }
}