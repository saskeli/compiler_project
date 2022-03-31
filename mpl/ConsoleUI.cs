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
                    StreamLexer parser = new StreamLexer(sr, options.Verbose, options.Debug, options.MultiError);
                    Program prog = parser.Parse();
                    if (options.Compile)
                    {
                        prog.Output(options.Output);
                    }
                    else
                    {
                        prog.Run();
                    }

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
                    case MultiException exception:
                        MultiException me = exception;
                        OutputMultiEx(me, options.File);
                        break;
                    default:
                        throw;
                }
            }

            return 0;
        }

        private static void OutputMultiEx(MultiException me, string filePath)
        {
            Console.Write("-----------------------------------\n");
            Console.Write($"Encountered {me.Exceptions.Count} exceptions during parsing:\n");
            foreach (Exception ex in me.Exceptions)
            {
                switch (ex)
                {
                    case InvalidSyntaxException exception:
                        InvalidSyntaxException ise = exception;
                        OutputEx(ise.Message, ise.Line, ise.Position, filePath);
                        break;
                    case AssertionException exception:
                        AssertionException ae = exception;
                        OutputEx(ae.Message, ae.Line, ae.Position, filePath);
                        break;
                    case MplDivideByZeroException exception:
                        MplDivideByZeroException mdbze = exception;
                        OutputEx(mdbze.Message, mdbze.Line, mdbze.Position, filePath);
                        break;
                    case RuntimeException exception:
                        RuntimeException rte = exception;
                        OutputEx(rte.Message, rte.Line, rte.Position, filePath);
                        break;
                    case UnexpectedCharacterException exception:
                        UnexpectedCharacterException uece = exception;
                        OutputEx(uece.Message, uece.Line, uece.Position, filePath);
                        break;
                    case UnsupportedCharacterException exception:
                        UnsupportedCharacterException usce = exception;
                        OutputEx(usce.Message, usce.Line, usce.Position, filePath);
                        break;
                    default:
                        throw ex;
                }
            }
        }

        private static void OutputEx(string message, int line, int pos, string filePath)
        {
            Console.Write("-----------------------------------\n");
            Console.Write($"{message}\n\n");
            using (StreamReader sr = new StreamReader(filePath))
            {
                string prev = "";
                string codeLine = "";
                for (int i = 0; i < line; i++)
                {
                    prev = codeLine;
                    codeLine = sr.ReadLine();
                }
                Console.Write($"{prev}\n");
                Console.Write($"{codeLine}\n");
            }
            Console.Write($"{new string('-', pos < 1 ? 1 : pos - 1)}^\n");
        }

        public class Options
        {
            [Option('c', "compile", Required = false, Default = false,
                HelpText = "Enable output of compiled Nasm instead of executing interpreted input.")]
            public bool Compile { get; set; }

            [Option('v', "verbose", Required = false, Default = false,
                HelpText = "Enable output of diagnostic messages.")]
            public bool Verbose { get; set; }

            [Option('d', "debug", Required = false, Default = 0,
                HelpText = "Enable detailed diagnostic messages.")]
            public int Debug { get; set; }

            [Option('m', "multi-error", Required = false, Default = false,
                HelpText = "Report multiple errors during single run.")]
            public bool MultiError { get; set; }

            [Value(0, Required = true, MetaName = "file",
                HelpText = "Input Mini-PL source code file.")]
            public string File { get; set; }

            [Value(1, Required = false, MetaName = "output", Default = "",
                HelpText = "Name of ouput file when generating Nasm.")]
            public string Output { get; set; }
        }
    }
}
