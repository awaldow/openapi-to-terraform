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
            dynamic ret = new ExpandoObject();
            //string[] assemblies = Directory.GetFiles(inputAssemblyPath, "*.dll");
            string[] assemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            var paths = new List<string>(assemblies);
            paths.Add(inputAssemblyPath);
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

                    var typesWithMyAttribute =
                        from t in assembly.GetTypes().AsParallel()
                        let attributes = t.GetCustomAttributesData().Where(c => c.AttributeType == typeof(RevisionAttribute)).ToList()
                        where attributes != null && attributes.Count() > 0
                        select new { Type = t, Attributes = attributes.Cast<RevisionAttribute>() };

                    var controllerActionList = assembly.GetTypes()
                        .Where(type => typeof(Controller).IsAssignableFrom(type))
                        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                        .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                        .Select(x => new { Controller = x.DeclaringType.Name, Action = x.Name, ReturnType = x.ReturnType.Name, Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))) })
                        .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

                    // if (typesWithRevisionAttribute.Count() == 0) // No RevisionAttributes found, so just do all operations mapped to ["1"]
                    // {
                    //     foreach (var path in parser.Document.Paths)
                    //     {
                    //         foreach (var operation in path.Value.Operations)
                    //         {
                    //             var keyString = path.Key + "^" + operation.Key.ToString().ToLower();
                    //             ret[keyString] = "[\"1\"]";
                    //         }
                    //     }
                    // }
                    // else // Obey the found revision attributes
                    // {

                    // }

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