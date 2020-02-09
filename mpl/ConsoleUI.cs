using System;
using System.IO;
using CommandLine;
using mpl.domain;
using Type = mpl.domain.Type;

namespace mpl
{
    public class ConsoleUI
    {
        public static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    options => Run(options),
                    _ => 1);
        }

        private static int Run(Options options)
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(options.File)))
            {
                StreamLexer parser = new StreamLexer(sr, options.Verbose, options.Debug);
                Program prog = parser.Parse();
                prog.Run();
            }

            return 0;
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
