using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl;

namespace mplTests
{
    [TestClass]
    public class ConsoleUITests
    {
        [TestMethod]
        public void MainTest1()
        {
            const string inp = "var X : int := 4 + (6 * 2);\nprint X;\n";
            using (StreamWriter sw = new StreamWriter("test.mpl"))
            {
                sw.Write(inp);
            }
            OutMocker omock = new OutMocker(Encoding.ASCII);
            Console.SetOut(omock);
            InMocker imock = new InMocker();
            Console.SetIn(imock);
            ConsoleUI.Main(new[]{"test.mpl"});
            Assert.AreEqual(1, omock.Output.Count);
            Assert.AreEqual("16", omock.Output[0]);
        }

        [TestMethod]
        public void MainTest2()
        {
            const string inp = @"var nTimes : int := 0;
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
            OutMocker omock = new OutMocker(Encoding.ASCII);
            Console.SetOut(omock);
            InMocker imock = new InMocker(new []{"1"});
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOutput = {
                "How many times ? ",
                "0",
                " : Hello, World!\n"
            };
            Assert.AreEqual(expectedOutput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], omock.Output[i]);
            }
            
        }

        [TestMethod]
        public void MainTest3()
        {
            const string inp = @"print ""Give a number"";
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
            OutMocker omock = new OutMocker(Encoding.ASCII);
            Console.SetOut(omock);
            InMocker imock = new InMocker(new[] { "4" });
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOutput = {
                "Give a number",
                "The result is: ",
                "24"
            };
            Assert.AreEqual(expectedOutput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], omock.Output[i]);
            }

        }

        [TestMethod]
        public void MainTest4()
        {
            const string inp = @"var X : int; //Loop var;
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
            OutMocker omock = new OutMocker(Encoding.ASCII);
            Console.SetOut(omock);
            InMocker imock = new InMocker();
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOutput = {
                "105"
            };
            Assert.AreEqual(expectedOutput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], omock.Output[i]);
            }

        }

        [TestMethod]
        public void MainTest5()
        {
            const string inp = @"var i : int;
for i in 5..0 do
    var s : int;
    var j : int;
    for j in i..0 do
        s := s + j;
    end for;
    print s;
end for;";
            using (StreamWriter sw = new StreamWriter("test.mpl"))
            {
                sw.Write(inp);
            }
            OutMocker omock = new OutMocker(Encoding.ASCII);
            Console.SetOut(omock);
            InMocker imock = new InMocker();
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOutput = {
                "15",
                "10",
                "6",
                "3",
                "1",
                "0"
            };
            Assert.AreEqual(expectedOutput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], omock.Output[i]);
            }
        }

        [TestMethod]
        public void InvalidNameTest()
        {
            const string inp = "var _X : int := 4 + (6 * 2);\nprint X;\n";
            using (StreamWriter sw = new StreamWriter("test.mpl"))
            {
                sw.Write(inp);
            }
            OutMocker omock = new OutMocker(Encoding.ASCII);
            Console.SetOut(omock);
            InMocker imock = new InMocker();
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOutput = {
                "-----------------------------------\n",
                "\"_\" is an invalid start for a name\n\n",
                "\n",
                "var _X : int := 4 + (6 * 2);\n",
                "---^\n"
            };
            Assert.AreEqual(expectedOutput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], omock.Output[i]);
            }
        }

        [TestMethod]
        public void RuntimeExceptionTest()
        {
            const string inp = @"var X: int;
for X in 0..4 do
    X := X + 1;
end for;
print X;";
            using (StreamWriter sw = new StreamWriter("test.mpl"))
            {
                sw.Write(inp);
            }
            OutMocker omock = new OutMocker(Encoding.ASCII);
            Console.SetOut(omock);
            InMocker imock = new InMocker();
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOutput = {
                "-----------------------------------\n",
                "Assignment to locked variable X\n\n",
                "for X in 0..4 do\n",
                "    X := X + 1;\n",
                "-------------^\n"
            };
            Assert.AreEqual(expectedOutput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], omock.Output[i]);
            }
        }

        [TestMethod]
        public void InvalidCharacterTest()
        {
            const string inp = @"var x : int := 1 – 4;";
            using (StreamWriter sw = new StreamWriter("test.mpl"))
            {
                sw.Write(inp);
            }
            OutMocker omock = new OutMocker(Encoding.ASCII);
            Console.SetOut(omock);
            InMocker imock = new InMocker();
            Console.SetIn(imock);
            ConsoleUI.Main(new[] { "test.mpl" });
            string[] expectedOutput = {
                "-----------------------------------\n",
                "Unsupported character encountered at line 1, position 18.\n\n",
                "\n",
                "var x : int := 1 – 4;\n",
                "-----------------^\n"
            };
            Assert.AreEqual(expectedOutput.Length, omock.Output.Count);
            for (int i = 0; i < expectedOutput.Length; i++)
            {
                Assert.AreEqual(expectedOutput[i], omock.Output[i]);
            }
        }

        //TODO: Tests for error messages.
    }
}