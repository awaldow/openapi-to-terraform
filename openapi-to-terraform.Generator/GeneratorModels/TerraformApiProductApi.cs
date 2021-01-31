using System.Text;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.GeneratorModels
{
    public class TerraformApimProductApi : ITerraformBlockGenerator
    {
        public static string GenerateBlock(OpenApiDocument document)
        {
            StringBuilder sb = new StringBuilder();
            string resourceName = document.Info.Title.ToLower().Replace(" ", "");
            sb.AppendLine($"resource \"azurerm_api_management_product_api\" \"{resourceName}productapi\"");
            sb.AppendLine($"\tapi_name\t=\tazurerm_api_management_api.{resourceName}.name");
            sb.AppendLine($"\tproduct_id\t=\t{{api_management_product_id}}");
            sb.AppendLine($"\tapi_management_name\t=\t{{api_management_service_name}}");
            sb.AppendLine($"\tresource_group_name\t=\t{{api_management_resource_group_name}}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}