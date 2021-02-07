using System;
using System.Linq;

namespace openapi_to_terraform.RevisionCli
{
    public class ArgsParser
    {
        public string Command { get; set; }
        public string InputAssemblyPath { get; set; }
        public string OpenApiPath { get; set; }
        public string OutputPath { get; set; }

        public readonly string[] args;
        public readonly string[] acceptedCommands = { "generate", "_generate", "--help" };

        public ArgsParser(string[] args)
        {
            this.args = args;
        }
    }

    public static class ArgParserExtensions
    {
        public static ArgsParser Parse(this ArgsParser parser)
        {
            if (!parser.acceptedCommands.Any(a => a == parser.args[0]))
            {
                throw new UnknownCommandException($"Unknown command {parser.args[0]}");
            }
            else
            {
                parser.Command = parser.args[0];
            }

            try
            {
                for (int i = 1; i < parser.args.Length; i += 2)
                {
                    switch (parser.args[i])
                    {
                        case "-a":
                        case "--input-assembly-path":
                            parser.InputAssemblyPath = parser.args[i + 1];
                            break;
                        case "-o":
                        case "--output-path":
                            parser.OutputPath = parser.args[i + 1];
                            break;
                        case "-f":
                        case "--open-api-file":
                            parser.OpenApiPath = parser.args[i + 1];
                            break;
                    }
                }
            }
            catch(Exception e) {
                // TODO: Handle unbalanced args array here
                Console.WriteLine(e.Message);
            }

            return parser;
        }
    }

    public class UnknownCommandException : Exception
    {
        public UnknownCommandException(string message) : base(message) { }
    }
}