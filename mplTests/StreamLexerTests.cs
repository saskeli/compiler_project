using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mpl;
using mpl.domain;

namespace mplTests
{
    [TestClass]
    public class StreamLexerTests
    {
        [TestMethod]
        public void StreamLexerTest1()
        {
            ParserMock par = new ParserMock(0, false);
            const string inp = "var X : int := 4 + (6 * 2);\nprint X;";
            StreamReader sr = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(inp)));
            StreamLexer lexer = new StreamLexer(sr, false, 0, false, par);
            lexer.Parse();
            Token[] expectedTokens = {
                new Token(TokenType.Name, 1, 3, "var"), 
                new Token(TokenType.Name, 1, 5, "X"),
                new Token(TokenType.Control, 1, 7, ":"), 
                new Token(TokenType.Name, 1, 11, "int"), 
                new Token(TokenType.Control, 1, 14, ":="), 
                new Token(TokenType.Number, 1, 16, "4"), 
                new Token(TokenType.Control, 1, 18, "+"), 
                new Token(TokenType.Control, 1, 20, "("), 
                new Token(TokenType.Number, 1, 21, "6"), 
                new Token(TokenType.Control, 1, 23, "*"), 
                new Token(TokenType.Number, 1, 25, "2"), 
                new Token(TokenType.Control, 1, 26, ")"), 
                new Token(TokenType.Control, 1, 27, ";"), 
                new Token(TokenType.Name, 2, 5, "print"), 
                new Token(TokenType.Name, 2, 7, "X"), 
                new Token(TokenType.Control, 2, 8, ";") 
            };
            Assert.AreEqual(expectedTokens.Length, par.Tokens.Count);
            for (int i = 0; i < expectedTokens.Length; i++)
            {
                Assert.AreEqual(expectedTokens[i], par.Tokens[i]);
            }
        }

        [TestMethod]
        public void StreamLexerTest2()
        {
            ParserMock par = new ParserMock(0, false);
            const string inp = @"var nTimes : int := 0;
print ""How many times ? "";
read nTimes;
var x : int;
for x in 0..nTimes-1 do
    print x;
    print "" : Hello, World!\n"";
end for;
assert (x = nTimes);";
            StreamReader sr = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(inp)));
            StreamLexer lexer = new StreamLexer(sr, false, 0, false, par);
            lexer.Parse();
            Token[] expectedTokens = {
                new Token(TokenType.Name, 1, 3, "var"),
                new Token(TokenType.Name, 1, 10, "nTimes"),
                new Token(TokenType.Control, 1, 12, ":"),
                new Token(TokenType.Name, 1, 16, "int"),
                new Token(TokenType.Control, 1, 19, ":="),
                new Token(TokenType.Number, 1, 21, "0"),
                new Token(TokenType.Control, 1, 22, ";"),
                new Token(TokenType.Name, 2, 5, "print"),
                new Token(TokenType.String, 2, 25, "How many times ? "),
                new Token(TokenType.Control, 2, 26, ";"),
                new Token(TokenType.Name, 3, 4, "read"), 
                new Token(TokenType.Name, 3, 11, "nTimes"), 
                new Token(TokenType.Control, 3, 12, ";"), 
                new Token(TokenType.Name, 4, 3, "var"), 
                new Token(TokenType.Name, 4, 5, "x"), 
                new Token(TokenType.Control, 4, 7, ":"), 
                new Token(TokenType.Name, 4, 11, "int"), 
                new Token(TokenType.Control, 4, 12, ";"), 
                new Token(TokenType.Name, 5, 3, "for"), 
                new Token(TokenType.Name, 5, 5, "x"), 
                new Token(TokenType.Name, 5, 8, "in"),
                new Token(TokenType.Number, 5, 10, "0"),
                new Token(TokenType.Control, 5, 12, ".."), 
                new Token(TokenType.Name, 5, 18, "nTimes"), 
                new Token(TokenType.Control, 5, 19, "-"), 
                new Token(TokenType.Number, 5, 20, "1"), 
                new Token(TokenType.Name, 5, 23, "do"), 
                new Token(TokenType.Name, 6, 9, "print"), 
                new Token(TokenType.Name, 6, 11, "x"), 
                new Token(TokenType.Control, 6, 12, ";"), 
                new Token(TokenType.Name, 7, 9, "print"), 
                new Token(TokenType.String, 7, 30, " : Hello, World!\n"), 
                new Token(TokenType.Control, 7, 31, ";"), 
                new Token(TokenType.Name, 8, 3, "end"), 
                new Token(TokenType.Name, 8, 7, "for"), 
                new Token(TokenType.Control, 8, 8, ";"), 
                new Token(TokenType.Name, 9, 6, "assert"), 
                new Token(TokenType.Control, 9, 8, "("), 
                new Token(TokenType.Name, 9, 9, "x"), 
                new Token(TokenType.Control, 9, 11, "="), 
                new Token(TokenType.Name, 9, 18, "nTimes"), 
                new Token(TokenType.Control, 9, 19, ")"), 
                new Token(TokenType.Control, 9, 20, ";")
            };
            Assert.AreEqual(expectedTokens.Length, par.Tokens.Count);
            for (int i = 0; i < expectedTokens.Length; i++)
            {
                Assert.AreEqual(expectedTokens[i], par.Tokens[i]);
            }
        }

        [TestMethod]
        public void StreamLexerTest3()
        {
            ParserMock par = new ParserMock(0, false);
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
            StreamReader sr = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(inp)));
            StreamLexer lexer = new StreamLexer(sr, false, 0, false, par);
            lexer.Parse();
            Token[] expectedTokens = {
                new Token(TokenType.Name, 1, 5, "print"),
                new Token(TokenType.String, 1, 21, "Give a number"),
                new Token(TokenType.Control, 1, 22, ";"),
                new Token(TokenType.Name, 2, 3, "var"),
                new Token(TokenType.Name, 2, 5, "n"),
                new Token(TokenType.Control, 2, 7, ":"),
                new Token(TokenType.Name, 2, 11, "int"),
                new Token(TokenType.Control, 2, 12, ";"),
                new Token(TokenType.Name, 3, 4, "read"),
                new Token(TokenType.Name, 3, 6, "n"),
                new Token(TokenType.Control, 3, 7, ";"),
                new Token(TokenType.Name, 4, 3, "var"),
                new Token(TokenType.Name, 4, 5, "v"),
                new Token(TokenType.Control, 4, 7, ":"),
                new Token(TokenType.Name, 4, 11, "int"),
                new Token(TokenType.Control, 4, 14, ":="),
                new Token(TokenType.Number, 4, 16, "1"),
                new Token(TokenType.Control, 4, 17, ";"),
                new Token(TokenType.Name, 5, 3, "var"),
                new Token(TokenType.Name, 5, 5, "i"),
                new Token(TokenType.Control, 5, 7, ":"),
                new Token(TokenType.Name, 5, 11, "int"),
                new Token(TokenType.Control, 5, 12, ";"),
                new Token(TokenType.Name, 6, 3, "for"),
                new Token(TokenType.Name, 6, 5, "i"),
                new Token(TokenType.Name, 6, 8, "in"),
                new Token(TokenType.Number, 6, 10, "1"),
                new Token(TokenType.Control, 6, 12, ".."),
                new Token(TokenType.Name, 6, 13, "n"),
                new Token(TokenType.Name, 6, 16, "do"),
                new Token(TokenType.Name, 7, 5, "v"),
                new Token(TokenType.Control, 7, 8, ":="),
                new Token(TokenType.Name, 7, 10, "v"),
                new Token(TokenType.Control, 7, 12, "*"),
                new Token(TokenType.Name, 7, 14, "i"),
                new Token(TokenType.Control, 7, 15, ";"),
                new Token(TokenType.Name, 8, 3, "end"),
                new Token(TokenType.Name, 8, 7, "for"),
                new Token(TokenType.Control, 8, 8, ";"),
                new Token(TokenType.Name, 9, 5, "print"),
                new Token(TokenType.String, 9, 23, "The result is: "),
                new Token(TokenType.Control, 9, 24, ";"),
                new Token(TokenType.Name, 10, 5, "print"),
                new Token(TokenType.Name, 10, 7, "v"), 
                new Token(TokenType.Control, 10, 8, ";")
            };
            Assert.AreEqual(expectedTokens.Length, par.Tokens.Count);
            for (int i = 0; i < expectedTokens.Length; i++)
            {
                Assert.AreEqual(expectedTokens[i], par.Tokens[i]);
            }
        }

        [TestMethod]
        public void StreamLexerTest4()
        {
            ParserMock par = new ParserMock(0, false);
            const string inp = @"var X : int; //Loop var;
var sum : int;
for X in /*here is a range*/ 0..14 do
 /* We are simply counting
    the sum over the range*/
    sum := sum + X;
end for;
print sum;";
            StreamReader sr = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(inp)));
            StreamLexer lexer = new StreamLexer(sr, false, 0, false, par);
            lexer.Parse();
            Token[] expectedTokens = {
                new Token(TokenType.Name, 1, 3, "var"),
                new Token(TokenType.Name, 1, 5, "X"),
                new Token(TokenType.Control, 1, 7, ":"),
                new Token(TokenType.Name, 1, 11, "int"),
                new Token(TokenType.Control, 1, 12, ";"),
                new Token(TokenType.Name, 2, 3, "var"),
                new Token(TokenType.Name, 2, 7, "sum"), 
                new Token(TokenType.Control, 2, 9, ":"), 
                new Token(TokenType.Name, 2, 13, "int"), 
                new Token(TokenType.Control, 2, 14, ";"), 
                new Token(TokenType.Name, 3, 3, "for"), 
                new Token(TokenType.Name, 3, 5, "X"), 
                new Token(TokenType.Name, 3, 8, "in"), 
                new Token(TokenType.Number, 3, 30, "0"), 
                new Token(TokenType.Control, 3, 32, ".."), 
                new Token(TokenType.Number, 3, 34, "14"), 
                new Token(TokenType.Name, 3, 37, "do"), 
                new Token(TokenType.Name, 6, 7, "sum"), 
                new Token(TokenType.Control, 6, 10, ":="), 
                new Token(TokenType.Name, 6, 14, "sum"), 
                new Token(TokenType.Control, 6, 16, "+"), 
                new Token(TokenType.Name, 6, 18, "X"), 
                new Token(TokenType.Control, 6, 19, ";"), 
                new Token(TokenType.Name, 7, 3, "end"), 
                new Token(TokenType.Name, 7, 7, "for"), 
                new Token(TokenType.Control, 7, 8, ";"), 
                new Token(TokenType.Name, 8, 5, "print"), 
                new Token(TokenType.Name, 8, 9, "sum"), 
                new Token(TokenType.Control, 8, 10, ";")
            };
            Assert.AreEqual(expectedTokens.Length, par.Tokens.Count);
            for (int i = 0; i < expectedTokens.Length; i++)
            {
                Assert.AreEqual(expectedTokens[i], par.Tokens[i]);
            }
        }
    }
}