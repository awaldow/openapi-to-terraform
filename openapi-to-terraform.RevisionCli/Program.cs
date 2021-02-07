using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using openapi_to_terraform.Extensions.Attributes;

namespace openapi_to_terraform.RevisionCli
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var argsParsed = new ArgsParser(args).Parse();
            switch (argsParsed.Command)
            {
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
                        var startupAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(Directory.GetCurrentDirectory(), argsParsed.InputAssemblyPath));
                        var ret = new ExpandoObject() as IDictionary<string, object>;

                        using FileStream fs = File.OpenRead(argsParsed.OpenApiPath);
                        var document = new OpenApiStreamReader().Read(fs, out var diagnostic);

                        try
                        {
                            var controllerActionList = startupAssembly.GetTypes()
                                .Where(type => type.IsAssignableTo(typeof(ControllerBase)))
                                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                                .Where(x => x.GetCustomAttributes().Any(a => a.GetType() == typeof(RevisionAttribute)))
                                .Select(x => new { Action = x.Name, Revisions = ((RevisionAttribute)x.GetCustomAttributes().SingleOrDefault(a => a.GetType() == typeof(RevisionAttribute))).Revisions.Select(r => r.ToString()) })
                                .OrderBy(x => x.Action).ToList();

                            if (controllerActionList.Count() == 0) // No RevisionAttributes found, so just do all operations mapped to ["1"]
                            {
                                foreach (var path in document.Paths)
                                {
                                    foreach (var operation in path.Value.Operations)
                                    {
                                        var keyString = path.Key + "^" + operation.Key.ToString().ToLower();
                                        ret.Add(keyString, "[\"1\"]");
                                    }
                                }
                            }
                            else // Obey the found revision attributes
                            {
                                foreach (var path in document.Paths)
                                {
                                    foreach (var operation in path.Value.Operations)
                                    {
                                        if (controllerActionList.Any(ca => ca.Action == operation.Value.OperationId))
                                        {
                                            var keyString = path.Key + "^" + operation.Key.ToString().ToLower();
                                            ret.Add(keyString, controllerActionList.SingleOrDefault(ca => ca.Action == operation.Value.OperationId).Revisions);
                                        }
                                    }
                                }
                            }

                            var revisionsJson = JsonConvert.SerializeObject(ret, Formatting.Indented);
                            if(!Directory.Exists(Path.GetDirectoryName(argsParsed.OutputPath))) {
                                Directory.CreateDirectory(Path.GetDirectoryName(argsParsed.OutputPath));
                            }
                            File.WriteAllText(argsParsed.OutputPath, revisionsJson);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            return (int)ExitCode.Error;
                        }

                        return (int)ExitCode.Success;
                    }
                default:
                    Console.WriteLine($"Command {argsParsed.Command} unknown");
                    return (int)ExitCode.CommandUnknown;
            }

        }

        enum ExitCode
        {
            Success = 0,
            CommandUnknown = 1,
            Error = 2
        }

        private static string EscapePath(string path)
        {
            return path.Contains(" ")
                ? "\"" + path + "\""
                : path;
        }
    }
}
