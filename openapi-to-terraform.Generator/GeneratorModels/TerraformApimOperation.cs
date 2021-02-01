using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.GeneratorModels
{
    public class TerraformApimOperation
    {
        public static string GenerateBlock(OpenApiDocument document, KeyValuePair<string, string[]> revisionMap)
        {
            StringBuilder sb = new StringBuilder();
            var split = revisionMap.Key.Split("^");
            var revisionPath = split[0];
            var revisionMethod = split[1];
            foreach (var path in document.Paths.Where(p => p.Key.Equals(revisionPath))) // Find all the paths that match the revision provided
            {
                foreach (var revision in revisionMap.Value) // Iterate through the provided revisions mapped for that operation
                {
                    var apiResourceName = document.Info.Title.ToLower().Replace(" ", "") + $"_rev{revision}"; // This is the same format used when generating the API block
                    foreach (var operation in path.Value.Operations.Where(o => o.Key.ToString() == revisionMethod))
                    {
                        sb.AppendLine($"resource \"azurerm_api_management_api_operation\" \"{operation.Value.OperationId}\" {{");
                        sb.AppendLine($"\tapi_name\t=\t\"azurerm_api_management_api.{apiResourceName}\"");
                        sb.AppendLine($"\tapi_management_name\t=\t\"{{api_management_service_name}}\"");
                        sb.AppendLine($"\tresource_group_name\t=\t\"{{api_management_resource_group_name}}\"");
                        sb.AppendLine($"\tdisplay_name\t=\t\"{operation.Value.Summary}\"");
                        sb.AppendLine($"\tmethod\t=\t\"{operation.Key.ToString()}\"");
                        sb.AppendLine($"\turl_template\t=\t\"{path.Key.ToString()}\"");
                        sb.AppendLine($"\tdescription\t=\t\"{operation.Value.Description}\"");
                        foreach (var parameter in operation.Value.Parameters)
                        {
                            switch (parameter.In)
                            {
                                case (ParameterLocation.Path):
                                    sb.AppendLine("\ttemplate_parameter {");
                                    sb.AppendLine($"\t\tname\t=\t\"{parameter.Name}\"");
                                    sb.AppendLine($"\t\trequired\t=\t\"{parameter.Required}\"");
                                    sb.AppendLine($"\t\ttype\t=\t\"{parameter.Schema.Format}\"");
                                    sb.AppendLine($"\t\tdescription\t=\t\"{parameter.Description}\"");
                                    sb.AppendLine("\t}");
                                    break;
                            }
                        }
                        foreach (var response in operation.Value.Responses)
                        {
                            sb.AppendLine("\tresponse {");
                            sb.AppendLine($"\t\tstatus_code\t=\t{response.Key}");
                            sb.AppendLine($"\t\tdescription\t=\t{response.Value.Description}");
                            foreach (var representation in response.Value.Content)
                            {
                                sb.AppendLine("\t\trepresentation {");
                                sb.AppendLine($"\t\t\tcontent_type\t=\t{representation.Key}");
                                sb.AppendLine("\t\t}");
                            }
                            sb.AppendLine("\t}");
                        }
                        sb.AppendLine("}");
                    }
                }
            }

            return sb.ToString();
        }

        public static string GenerateBlock(OpenApiDocument document, string apiResourceName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var path in document.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    sb.AppendLine($"resource \"azurerm_api_management_api_operation\" \"{operation.Value.OperationId}\" {{");
                    sb.AppendLine($"\tapi_name\t=\t\"azurerm_api_management_api.{apiResourceName}\"");
                    sb.AppendLine($"\tapi_management_name\t=\t\"{{api_management_service_name}}\"");
                    sb.AppendLine($"\tresource_group_name\t=\t\"{{api_management_resource_group_name}}\"");
                    sb.AppendLine($"\tdisplay_name\t=\t\"{operation.Value.Summary}\"");
                    sb.AppendLine($"\tmethod\t=\t\"{operation.Key.ToString()}\"");
                    sb.AppendLine($"\turl_template\t=\t\"{path.Key.ToString()}\"");
                    sb.AppendLine($"\tdescription\t=\t\"{operation.Value.Description}\"");
                    foreach (var parameter in operation.Value.Parameters)
                    {
                        switch (parameter.In)
                        {
                            case (ParameterLocation.Path):
                                sb.AppendLine("\ttemplate_parameter {");
                                sb.AppendLine($"\t\tname\t=\t\"{parameter.Name}\"");
                                sb.AppendLine($"\t\trequired\t=\t\"{parameter.Required}\"");
                                sb.AppendLine($"\t\ttype\t=\t\"{parameter.Schema.Format}\"");
                                sb.AppendLine($"\t\tdescription\t=\t\"{parameter.Description}\"");
                                sb.AppendLine("\t}");
                                break;
                        }
                    }
                    foreach (var response in operation.Value.Responses)
                    {
                        sb.AppendLine("\tresponse {");
                        sb.AppendLine($"\t\tstatus_code\t=\t{response.Key}");
                        sb.AppendLine($"\t\tdescription\t=\t{response.Value.Description}");
                        foreach (var representation in response.Value.Content)
                        {
                            sb.AppendLine("\t\trepresentation {");
                            sb.AppendLine($"\t\t\tcontent_type\t=\t{representation.Key}");
                            sb.AppendLine("\t\t}");
                        }
                        sb.AppendLine("\t}");
                    }
                    sb.AppendLine("}");
                }
            }

            return sb.ToString();
        }
    }
}