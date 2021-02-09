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

        public static readonly string[] acceptedCommands = { "generate", "_generate", "--help" };
        private static readonly string[] acceptedArgs = { "-a", "--input-assembly-path", "-o", "--output-path", "-f", "--open-api-file" };

        public static ExitCode TryParse(string[] args, out ArgsParser argsParser)
        {
            argsParser = new ArgsParser();
            if (!ArgsParser.acceptedCommands.Any(a => a == args[0]))
            {
                return ExitCode.CommandUnknown;
            }
            else
            {
                argsParser.Command = args[0];
            }

            try
            {
                for (int i = 1; i < args.Length; i += 2)
                {
                    switch (args[i])
                    {
                        case "-a":
                        case "--input-assembly-path":
                            if (acceptedArgs.Contains(args[i + 1]))
                                return ExitCode.Error;
                            argsParser.InputAssemblyPath = args[i + 1];
                            break;
                        case "-o":
                        case "--output-path":
                            if (acceptedArgs.Contains(args[i + 1]))
                                return ExitCode.Error;
                            argsParser.OutputPath = args[i + 1];
                            break;
                        case "-f":
                        case "--open-api-file":
                            if (acceptedArgs.Contains(args[i + 1]))
                                return ExitCode.Error;
                            argsParser.OpenApiPath = args[i + 1];
                            break;
                        default: // If we got here, we have an unbalanced set of args
                            return ExitCode.Error;
                    }
                }
            }
            catch (Exception e)
            {
                // TODO: Handle unbalanced args array here
                Console.WriteLine(e.Message);
                return ExitCode.Error;
            }

            return ExitCode.Success;
        }
    }

    public enum ExitCode
    {
        Success = 0,
        CommandUnknown = 1,
        Error = 2
    }


    public class UnknownCommandException : Exception
    {
        public UnknownCommandException(string message) : base(message) { }
    }
}