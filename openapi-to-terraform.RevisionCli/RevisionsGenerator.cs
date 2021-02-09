using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using openapi_to_terraform.Extensions.Attributes;

namespace openapi_to_terraform.RevisionCli
{
    public class RevisionsGenerator
    {
        public static string Generate(Assembly startupAssembly, OpenApiRoot docTree)
        {
            var ret = new ExpandoObject() as IDictionary<string, object>;
            var controllersQueryBase = startupAssembly.GetTypes()
                                    .Where(type => type.IsAssignableTo(typeof(ControllerBase)) && type.GetCustomAttribute<RevisionsAttribute>() != null);

            var revisionsFromController = controllersQueryBase
                .Select(x => new { Controller = x.Name.Substring(0, x.Name.IndexOf("Controller") > -1 ? x.Name.Length - "Controller".Length : x.Name.Length), Revisions = x.GetCustomAttribute<RevisionsAttribute>().Revisions.Select(x => x.ToString()) });

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

                return JsonConvert.SerializeObject(ret, Formatting.Indented);
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

                return JsonConvert.SerializeObject(ret, Formatting.Indented);
            }
        }
    }
}