using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using openapi_to_terraform.Extensions.Attributes;

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
                            var startupAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(Directory.GetCurrentDirectory(), argsParsed.InputAssemblyPath));
                            var ret = new ExpandoObject() as IDictionary<string, object>;

                            var openApiText = File.ReadAllText(argsParsed.OpenApiPath);
                            var docTree = new OpenApiParser(openApiText).Read();

                            Console.WriteLine($"OpenAPI document parsed from {argsParsed.OpenApiPath}");
                            try
                            {
                                var controllersQueryBase = startupAssembly.GetTypes()
                                    .Where(type => type.IsAssignableTo(typeof(ControllerBase)) && type.GetCustomAttribute<RevisionsAttribute>() != null);

                                var revisionsFromController = controllersQueryBase
                                    .Select(x => new { Controller = x.Name.Substring(0, x.Name.IndexOf("Controller") > -1 ? x.Name.Length - "Controller".Length : x.Name.Length ), Revisions = x.GetCustomAttribute<RevisionsAttribute>().Revisions.Select(x => x.ToString()) });

                                if (revisionsFromController.Count() > 0) // RevisionsAttribute is on controllers, obey that
                                {
                                    Console.WriteLine($"RevisionAttributes found on controllers, so generating revisions file obeying attribute values");
                                    foreach (var path in docTree.Paths)
                                    {
                                        foreach (var operation in path.Operations)
                                        {
                                            var revisions = revisionsFromController.SingleOrDefault(r => path.Key.Contains(r.Controller)).Revisions;
                                            var keyString = path.Key + "^" + operation.Key.ToString().ToLower();
                                            ret.Add(keyString, revisions);
                                        }
                                    }

                                    var revisionsJson = JsonConvert.SerializeObject(ret, Formatting.Indented);
                                    File.WriteAllText(argsParsed.OutputPath, revisionsJson);
                                }
                                else // Otherwise, check for revisions attributes on controller actions
                                {
                                    var controllerActionList = startupAssembly.GetTypes()
                                                                        .Where(type => type.IsAssignableTo(typeof(ControllerBase)))
                                                                        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                                                                        .Where(x => x.GetCustomAttributes().Any(a => a.GetType() == typeof(RevisionsAttribute)))
                                                                        .Select(x => new { Action = x.Name, Revisions = ((RevisionsAttribute)x.GetCustomAttributes().SingleOrDefault(a => a.GetType() == typeof(RevisionsAttribute))).Revisions.Select(r => r.ToString()) })
                                                                        .OrderBy(x => x.Action).ToList();

                                    if (controllerActionList.Count() == 0) // No RevisionAttributes found, so just do all operations mapped to ["1"]
                                    {
                                        Console.WriteLine($"No RevisionAttributes found, so generating revisions file with all actions having [\"1\"]");
                                        var revision = new string[] { "1" };
                                        foreach (var path in docTree.Paths)
                                        {
                                            foreach (var operation in path.Operations)
                                            {
                                                var keyString = path.Key + "^" + operation.Key.ToString().ToLower();
                                                ret.Add(keyString, revision);
                                            }
                                        }
                                    }
                                    else // Obey the found revision attributes
                                    {
                                        Console.WriteLine($"RevisionAttributes found, so generating revisions file obeying attribute values");
                                        foreach (var path in docTree.Paths)
                                        {
                                            foreach (var operation in path.Operations)
                                            {
                                                if (controllerActionList.Any(ca => ca.Action == operation.OperationId))
                                                {
                                                    var keyString = path.Key + "^" + operation.Key.ToString().ToLower();
                                                    ret.Add(keyString, controllerActionList.SingleOrDefault(ca => ca.Action == operation.OperationId).Revisions);
                                                }
                                            }
                                        }
                                    }

                                    var revisionsJson = JsonConvert.SerializeObject(ret, Formatting.Indented);
                                    File.WriteAllText(argsParsed.OutputPath, revisionsJson);
                                }
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
