using System;
using System.IO;
using CommandLine;
using mpl.domain;
using mpl.Exceptions;

namespace mpl
{
    public class ConsoleUI
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    Run,
                    _ => 1);
        }

        private static int Run(Options options)
        {
            try
            {
                using (StreamReader sr = new StreamReader(File.OpenRead(options.File)))
                {
                    StreamLexer parser = new StreamLexer(sr, options.Verbose, options.Debug);
                    Program prog = parser.Parse();
                    prog.Run();
                }
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case InvalidSyntaxException exception:
                        InvalidSyntaxException ise = exception;
                        OutputEx(ise.Message, ise.Line, ise.Position, options.File);
                        break;
                    case AssertionException exception:
                        AssertionException ae = exception;
                        OutputEx(ae.Message, ae.Line, ae.Position, options.File);
                        break;
                    case MplDivideByZeroException exception:
                        MplDivideByZeroException mdbze = exception;
                        OutputEx(mdbze.Message, mdbze.Line, mdbze.Position, options.File);
                        break;
                    case RuntimeException exception:
                        RuntimeException rte = exception;
                        OutputEx(rte.Message, rte.Line, rte.Position, options.File);
                        break;
                    case UnexpectedCharacterException exception:
                        UnexpectedCharacterException uece = exception;
                        OutputEx(uece.Message, uece.Line, uece.Position, options.File);
                        break;
                    case UnsupportedCharacterException exception:
                        UnsupportedCharacterException usce = exception;
                        OutputEx(usce.Message, usce.Line, usce.Position, options.File);
                        break;
                    default:
                        throw;
                }
            }

            return 0;
        }

        private static void OutputEx(string message, int line, int pos, string filePath)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"{message}\n");
            using (StreamReader sr = new StreamReader(filePath))
            {
                string prev = "";
                string codeLine = "";
                for (int i = 0; i < line; i++)
                {
                    prev = codeLine;
                    codeLine = sr.ReadLine();
                }
                Console.WriteLine(prev);
                Console.WriteLine(codeLine);
            }
            Console.WriteLine($"{new string('-', pos < 1 ? 1 : pos - 1)}^");
        }

        public class Options
        {
            [Option('v', "verbose", Required = false, Default = false,
                HelpText = "Enable output of diagnostic messages")]
            public bool Verbose { get; set; }

            [Option('d', "debug", Required = false, Default = 0,
                HelpText = "Enable detailed diagnostic messages")]
            public int Debug { get; set; }

            [Value(0, Required = true, MetaName = "file",
                HelpText = "input Mini-PL source code file. Required")]
            public string File { get; set; }
        }
    }
}
