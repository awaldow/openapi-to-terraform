using System.Text;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.GeneratorModels
{
    public class TerraformApimProductApi : ITerraformBlockGenerator
    {
        public TerraformApimProductApi()
        {
            
        }

        public string GenerateBlock(OpenApiDocument document)
        {
            StringBuilder sb = new StringBuilder();
            string resourceName = document.Info.Title.ToLower().Replace(" ", "");
            sb.AppendLine($"resource \"azurerm_api_management_product_api\" \"{resourceName}productapi\"");
            sb.AppendLine($"\tapi_name\t=\tazurerm_api_management_api.{resourceName}.name");
            sb.AppendLine($"\tproduct_id\t=\t");
            sb.AppendLine($"\tapi_management_name\t=\t");
            sb.AppendLine($"\tresource_group_name\t=\t");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}