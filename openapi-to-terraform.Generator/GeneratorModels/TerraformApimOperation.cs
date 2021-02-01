using System.Text;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.GeneratorModels
{
    public class TerraformApimOperation
    {
        public static string GenerateBlock(OpenApiDocument document)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var path in document.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    sb.AppendLine($"resource \"azurerm_api_management_api_operation\" \"{operation.Value.OperationId}\" {{");
                    sb.AppendLine($"\tapi_name\t=\t\"{{api_name}}\"");
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