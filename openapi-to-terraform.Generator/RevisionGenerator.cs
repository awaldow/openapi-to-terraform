using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using openapi_to_terraform.Core;
using openapi_to_terraform.Extensions.Attributes;

namespace openapi_to_terraform.Generator
{
    public static class RevisionGenerator
    {
        public static string GenerateRevisionsFile(string inputAssemblyPath, string openApiPath, string routePrefix)
        {
            var ret = new ExpandoObject() as IDictionary<string, object>;
            //string[] assemblies = Directory.GetFiles(inputAssemblyPath, "*.dll");
            var mvcRuntimeDir = "/usr/share/dotnet/shared/Microsoft.AspNetCore.App/5.0.2"; // TODO: Find a way to determine this at runtime, runtimeenvironment points to the console app libs which don't have aspnetcore mvc stuff
            string[] mvcAssemblies = Directory.GetFiles(mvcRuntimeDir, "*.dll");
            string[] runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            string[] dllAssemblies = Directory.GetFiles(Path.GetDirectoryName(inputAssemblyPath), "*.dll");
            var paths = new List<string>(mvcAssemblies);
            paths.AddRange(runtimeAssemblies);
            paths.AddRange(dllAssemblies);
            var resolver = new PathAssemblyResolver(paths);
            var mlc = new MetadataLoadContext(resolver);

            using (mlc)
            {
                Assembly assembly = mlc.LoadFromAssemblyPath(inputAssemblyPath);
                var parser = new OpenApiParser(openApiPath);
                parser.Parse();

                try
                {
                    var types = assembly.GetTypes();

                    var controllerActionList = assembly.GetTypes()
                        .Where(type => type.BaseType.FullName == "Microsoft.AspNetCore.Mvc.ControllerBase")
                        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                        .Where(x => x.GetCustomAttributesData().Any(a => a.AttributeType == typeof(RevisionAttribute)))
                        .Select(x => new { Action = x.Name, Revisions = (x.GetCustomAttributesData().SingleOrDefault(a => a.AttributeType == typeof(RevisionAttribute))) })
                        .OrderBy(x => x.Action).ToList();

                    if (controllerActionList.Count() == 0) // No RevisionAttributes found, so just do all operations mapped to ["1"]
                    {
                        foreach (var path in parser.Document.Paths)
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
                        foreach (var path in parser.Document.Paths)
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

                    return JsonConvert.SerializeObject(ret);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return "";
                }
            }
        }
    }
}