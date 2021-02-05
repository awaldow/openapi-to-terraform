using System.IO;
using System.Text;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.azurerm.v2_45_1.GeneratorModels
{
    public class TerraformApimPolicy
    {
        public static string GenerateBlock(OpenApiDocument document, string policyRootDirectory, string operationId = null)
        {
            return GenerateBlock(document, "1", policyRootDirectory, operationId);
        }

        public static string GenerateBlock(OpenApiDocument document, string revision, string policyRootDirectory, string operationId = null)
        {
            StringBuilder sb = new StringBuilder();
            string apiName = document.Info.Title.ToLower().Replace(" ", "") + $"_rev{revision}";
            var policyXml = getPolicyXml(policyRootDirectory, revision, operationId);
            if (string.IsNullOrEmpty(policyXml)) // No policy was found, so don't generate block
            {
                return sb.ToString();
            }

            if (!string.IsNullOrEmpty(operationId))
            {
                string resourceName = $"{apiName}_{operationId}_policy";
                sb.AppendLine($"resource \"azurerm_api_management_api_operation_policy\" \"{resourceName}\" {{");
                sb.AppendLine($"\toperation_id\t=\t{operationId}");
            }
            else
            {
                string resourceName = $"{apiName}_policy";
                sb.AppendLine($"resource \"azurerm_api_management_api_policy\" \"{resourceName}\" {{");
            }
            sb.AppendLine($"\tapi_name\t=\t\"{apiName}\"");
            sb.AppendLine($"\tapi_management_name\t=\t{{api_management_service_name}}");
            sb.AppendLine($"\tresource_group_name\t=\t{{api_management_resource_group_name}}");
            sb.AppendLine($"\txml_content\t=\t<<XML");
            sb.AppendLine(policyXml);
            sb.AppendLine($"\tXML");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string getPolicyXml(string rootDir, string revision, string operationId)
        {
            var policyPath = string.IsNullOrEmpty(operationId) ? Path.Combine(rootDir, revision) : Path.Combine(rootDir, revision, operationId);
            var pathExists = Directory.Exists(policyPath);
            if (pathExists)
            {
                var file = Directory.GetFiles(policyPath, "*.policy");
                if (file.Length == 0) // No policy file found
                {
                    return "";
                }
                else // One or more policy files found, returning first found
                {
                    return File.ReadAllText(file[0]);
                }
            }
            else
            {
                return "";
            }
        }
    }
}