using System.Collections.Generic;
using System.Text;
using Microsoft.OpenApi.Models;
using openapi_to_terraform.Generator.azurerm.v2_45_1.GeneratorModels;

namespace openapi_to_terraform.Generator.azurerm.v2_45_1.Generators
{
    public class ApiGenerator
    {
        public static string GenerateTerraformOutput(OpenApiDocument document, List<string> revisions, string policyRootDirectory)
        {
            var sb = new StringBuilder();
            foreach (var revision in revisions)
            {
                string api = TerraformApimApi.GenerateBlock(document, revision);
                sb.AppendLine(api);
                string productApi = TerraformApimProductApi.GenerateBlock(document, revision);
                sb.AppendLine(productApi);
                string apiPolicy = TerraformApimPolicy.GenerateBlock(document, revision, policyRootDirectory);
                if (!string.IsNullOrEmpty(apiPolicy))
                {
                    sb.AppendLine(apiPolicy);
                }
            }
            return sb.ToString();
        }
    }
}