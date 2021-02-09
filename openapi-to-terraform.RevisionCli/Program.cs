using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace openapi_to_terraform.RevisionCli
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var parseSuccessful = ArgsParser.TryParse(args, out ArgsParser argsParsed);
            if (parseSuccessful == ExitCode.Success)
            {
                switch (argsParsed.Command)
                {
                    case "--help":
                        {
                            Console.WriteLine("Usage -- openapi-to-terraform-rev-cli {--help|generate {args}}");
                            Console.WriteLine("\t{args}:");
                            Console.WriteLine("\t\t-a,--input-assembly-file\tpath to dll");
                            Console.WriteLine("\t\t-o,--output-path\t\toutput path for revisions.json, including file name");
                            Console.WriteLine("\t\t-f,--open-api-file\t\tpath to OpenAPI spec");
                            return (int)ExitCode.Success;
                        }
                    case "generate":
                        {
                            var depsFile = argsParsed.InputAssemblyPath.Replace(".dll", ".deps.json");
                            var runtimeConfig = argsParsed.InputAssemblyPath.Replace(".dll", ".runtimeconfig.json");
                            var subProcess = Process.Start("dotnet", string.Format(
                                "exec --depsfile {0} --runtimeconfig {1} {2} _{3}", // note the underscore
                                EscapePath(depsFile),
                                EscapePath(runtimeConfig),
                                EscapePath(typeof(Program).GetTypeInfo().Assembly.Location),
                                string.Join(" ", args)
                            ));

                            subProcess.WaitForExit();
                            return subProcess.ExitCode;
                        }
                    case "_generate":
                        {
                            Console.WriteLine($"Loading assembly from {Path.Combine(Directory.GetCurrentDirectory(), argsParsed.InputAssemblyPath)}");
                            if (!string.IsNullOrEmpty(Path.GetDirectoryName(argsParsed.OutputPath)) && !Directory.Exists(Path.GetDirectoryName(argsParsed.OutputPath)))
                            {
                                Console.WriteLine($"Creating directory {Path.GetDirectoryName(argsParsed.OutputPath)}");
                                Directory.CreateDirectory(Path.GetDirectoryName(argsParsed.OutputPath));
                            }

                            var convertedAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), argsParsed.InputAssemblyPath);
                            Assembly startupAssembly = Assembly.LoadFrom(convertedAssemblyPath);

                            var openApiText = File.ReadAllText(argsParsed.OpenApiPath);
                            var docTree = new OpenApiParser(openApiText).Read();

                            Console.WriteLine($"OpenAPI document parsed from {argsParsed.OpenApiPath}");
                            try
                            {
                                var revisions = RevisionsGenerator.Generate(startupAssembly, docTree);
                                File.WriteAllText(argsParsed.OutputPath, revisions);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                return (int)ExitCode.Error;
                            }

                            return (int)ExitCode.Success;
                        }
                    default:
                        Console.WriteLine($"Command {argsParsed.Command} unknown, see --help for commands and arguments");
                        return (int)ExitCode.CommandUnknown;
                }
            }
            else if (parseSuccessful == ExitCode.CommandUnknown)
            {
                Console.WriteLine($"Command \"{args[0]}\" unknown, see --help for commands and arguments");
                return (int)parseSuccessful;
            }
            else
            {
                Console.WriteLine($"Returning {parseSuccessful}");
                return (int)parseSuccessful;
            }
        }


        private static string EscapePath(string path)
        {
            return path.Contains(" ")
                ? "\"" + path + "\""
                : path;
        }
    }
}
