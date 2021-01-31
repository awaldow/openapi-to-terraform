using System.Text;
using Microsoft.OpenApi.Models;
using openapi_to_terraform.Generator.GeneratorModels;

namespace openapi_to_terraform.Generator.Generators
{
    public class ApiGenerator : ITerraformApimGenerator
    {
        public static string GenerateTerraformOutput(OpenApiDocument document)
        {
            var sb = new StringBuilder();
            string api = TerraformApimApi.GenerateBlock(document);
            sb.AppendLine(api);
            string productApi = TerraformApimProductApi.GenerateBlock(document);
            sb.AppendLine(productApi);
            return sb.ToString();
        }
    }
}