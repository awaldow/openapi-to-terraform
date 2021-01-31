using System.Text;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.GeneratorModels
{
    public class TerraformApimApi : ITerraformBlockGenerator
    {
        public static string GenerateBlock(OpenApiDocument document)
        {
            StringBuilder sb = new StringBuilder();
            string resourceName = document.Info.Title.ToLower().Replace(" ", "");
            sb.AppendLine($"resource \"azurerm_api_management_api\" \"{resourceName}\" {{");
            sb.AppendLine($"\tname\t=\t{resourceName}");
            sb.AppendLine($"\tapi_management_name\t=\t{{api_management_service_name}}");
            sb.AppendLine($"\tresource_group_name\t=\t{{api_management_resource_group_name}}");
            sb.AppendLine($"\tdisplay_name\t=\t{document.Info.Title}");
            sb.AppendLine($"\trevision\t=\t1");
            sb.AppendLine($"\version\t=\t{document.Info.Version}");
            sb.AppendLine($"\version_set_id\t=\t");
            sb.AppendLine($"\tpath\t=\t{{api_path}}");
            sb.AppendLine($"\tprotocols\t=\t[\"https\"]");
            sb.AppendLine($"\tservice_url\t=\t{{api_backend_url}}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}