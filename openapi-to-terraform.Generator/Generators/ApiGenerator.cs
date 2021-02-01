using System.Collections.Generic;
using System.Text;
using Microsoft.OpenApi.Models;
using openapi_to_terraform.Generator.GeneratorModels;

namespace openapi_to_terraform.Generator.Generators
{
    public class ApiGenerator
    {
        public static string GenerateTerraformOutput(OpenApiDocument document, List<string> revisions)
        {
            var sb = new StringBuilder();
            foreach (var revision in revisions)
            {
                string api = TerraformApimApi.GenerateBlock(document, revision);
                sb.AppendLine(api);
            }
            string productApi = TerraformApimProductApi.GenerateBlock(document);
            sb.AppendLine(productApi);
            return sb.ToString();
        }
    }
}