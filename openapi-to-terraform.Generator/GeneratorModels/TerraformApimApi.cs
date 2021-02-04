using System.Text;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.GeneratorModels
{
    public class TerraformApimApi
    {
        public static string GenerateBlock(OpenApiDocument document)
        {
            return GenerateBlock(document, "1");
        }

        public static string GenerateBlock(OpenApiDocument document, string revision)
        {
            StringBuilder sb = new StringBuilder();
            string resourceName = document.Info.Title.ToLower().Replace(" ", "") + $"_rev{revision}";
            sb.AppendLine($"resource \"azurerm_api_management_api\" \"{resourceName}\" {{");
            sb.AppendLine($"\tname\t=\t\"{resourceName}\"");
            sb.AppendLine($"\tapi_management_name\t=\t{{api_management_service_name}}");
            sb.AppendLine($"\tresource_group_name\t=\t{{api_management_resource_group_name}}");
            sb.AppendLine($"\tdisplay_name\t=\t\"{document.Info.Title}\"");
            sb.AppendLine($"\trevision\t=\t\"{revision}\"");
            sb.AppendLine($"\tversion\t=\t\"v{document.Info.Version}\"");
            sb.AppendLine($"\tversion_set_id\t=\t{{api_management_version_set_id}}");
            sb.AppendLine($"\tpath\t=\t\"{{api_path}}\"");
            sb.AppendLine($"\tprotocols\t=\t[\"https\"]");
            sb.AppendLine($"\tservice_url\t=\t\"{{api_backend_url}}\"");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}