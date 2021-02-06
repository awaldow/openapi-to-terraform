using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using openapi_to_terraform.Core;

namespace openapi_to_terraform.Generator
{
    public static class RevisionGenerator
    {
        public static string GenerateRevisionsBlock(string inputAssemblyPath, string openApiPath)
        {
            var ret = new ExpandoObject() as IDictionary<string, object>;
            var runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
            string[] runtimeAssemblies = Directory.GetFiles(runtimeDir, "*.dll");
            var mvcRuntimeDir = Path.GetFullPath(Path.Combine(runtimeDir, "../../Microsoft.AspNetCore.App/5.0.2"));
            string[] mvcAssemblies = Directory.GetFiles(mvcRuntimeDir, "*.dll");
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
                    var controllerActionList = assembly.GetTypes()
                        .Where(type => type.BaseType.FullName == "Microsoft.AspNetCore.Mvc.ControllerBase")
                        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                        .Where(x => x.GetCustomAttributesData().Any(a => a.AttributeType.FullName == "openapi_to_terraform.Extensions.Attributes.RevisionAttribute"))
                        .Select(x => new { Action = x.Name, Revisions = ((ReadOnlyCollection<CustomAttributeTypedArgument>)(x.GetCustomAttributesData().SingleOrDefault(a => a.AttributeType.FullName == "openapi_to_terraform.Extensions.Attributes.RevisionAttribute").ConstructorArguments[0].Value)).Select(o => o.Value) })
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

                    return JsonConvert.SerializeObject(ret, Formatting.Indented);
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