using System.Text;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.GeneratorModels
{
    public class TerraformApimApi : ITerraformBlockGenerator
    {
        public TerraformApimApi()
        {
            
        }

        public string GenerateBlock(OpenApiDocument document)
        {
            StringBuilder sb = new StringBuilder();
            string resourceName = document.Info.Title.ToLower().Replace(" ", "");
            sb.AppendLine($"resource \"azurerm_api_management_api\" \"{resourceName}\" {{");
            sb.AppendLine($"\tname\t=\t{resourceName}");
            sb.AppendLine($"\tapi_management_name\t=\t");
            sb.AppendLine($"\tresource_group_name\t=\t");
            sb.AppendLine($"\tdisplay_name\t=\t{document.Info.Title}");
            sb.AppendLine($"\trevision\t=\t1");
            sb.AppendLine($"\version\t=\t{document.Info.Version}");
            sb.AppendLine($"\version_set_id\t=\t");
            sb.AppendLine($"\tpath\t=\t");
            sb.AppendLine($"\tprotocols\t=\t[\"https\"]");
            sb.AppendLine($"\tservice_url\t=\t");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}