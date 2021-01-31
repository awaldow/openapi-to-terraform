using System.Text;
using Microsoft.OpenApi.Models;
using openapi_to_terraform.Generator.GeneratorModels;

namespace openapi_to_terraform.Generator.Generators
{
    public class ApiGenerator : ITerraformApimGenerator
    {
        public ApiGenerator()
        {
            
        }

        public string GenerateTerraformOutput(OpenApiDocument document)
        {
            var sb = new StringBuilder();
            string api = new TerraformApimApi().GenerateBlock(document);
            sb.AppendLine(api);
            string productApi = new TerraformApimProductApi().GenerateBlock(document);
            sb.AppendLine(productApi);
            return sb.ToString();
        }
    }
}