using System.Text;
using Microsoft.OpenApi.Models;
using openapi_to_terraform.Generator.GeneratorModels;

namespace openapi_to_terraform.Generator.Generators
{
    public class OperationGenerator : ITerraformApimGenerator
    {
        public OperationGenerator()
        {
            
        }

        public string GenerateTerraformOutput(OpenApiDocument document)
        {
            var sb = new StringBuilder();
            string operation = new TerraformApimOperation().GenerateBlock(document);
            sb.AppendLine(operation);
            return sb.ToString();
        }
    }
}