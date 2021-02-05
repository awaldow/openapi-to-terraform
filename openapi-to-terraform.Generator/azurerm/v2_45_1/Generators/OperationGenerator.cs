using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using openapi_to_terraform.Generator.azurerm.v2_45_1.GeneratorModels;

namespace openapi_to_terraform.Generator.azurerm.v2_45_1.Generators
{
    public class OperationGenerator
    {
        public static string GenerateTerraformOutput(OpenApiDocument document, string backendUrl, string policyRootDirectory)
        {
            var sb = new StringBuilder();
            string operation = TerraformApimOperation.GenerateBlock(document, document.Info.Title.ToLower().Replace(" ", "") + $"_rev1", backendUrl, policyRootDirectory);
            sb.AppendLine(operation);
            return sb.ToString();
        }

        public static string GenerateTerraformOutput(OpenApiDocument document, string revisionMappingFile, string backendUrl, string policyRootDirectory)
        {
            // The key for this dict will be an OpenApiDocument.OpenApiPathItem.Key, the value is a string array of the revisions to include that operation in
            var revisionsMap = JObject.Parse(File.ReadAllText(revisionMappingFile)).ToObject<Dictionary<string, string[]>>();
            var sb = new StringBuilder();
            foreach (KeyValuePair<string, string[]> revision in revisionsMap)
            {
                string operation = TerraformApimOperation.GenerateBlock(document, revision, backendUrl, policyRootDirectory);
                sb.AppendLine(operation);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}