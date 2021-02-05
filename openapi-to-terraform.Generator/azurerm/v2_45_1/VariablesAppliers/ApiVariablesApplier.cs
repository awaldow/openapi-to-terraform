using System.IO;
using Newtonsoft.Json.Linq;

namespace openapi_to_terraform.Generator.azurerm.v2_45_1.VariablesAppliers
{
    public class ApiVariablesApplier
    {
        public static string ApplyVariables(string generatedOutput, string variablesPath)
        {
            // "api_management_service_name": "azurerm_api_management.roombyapim.name",
            // "api_management_resource_group_name": "azurerm_api_management.roombyapim.resource_group_name",
            // "api_path": "users",
            // "api_backend_url": "${data.azurerm_app_service.roombyusersapp.default_site_hostname}/api/",
            // "api_management_product_id": "azurerm_api_management_product.roombyproduct.product_id"
            JObject terraformVariables = JObject.Parse(File.ReadAllText(variablesPath));
            var serviceName = terraformVariables.SelectToken("api_management_service_name").Value<string>();
            var resourceGroupName = terraformVariables.SelectToken("api_management_resource_group_name").Value<string>();
            var apiPath = terraformVariables.SelectToken("api_path").Value<string>();
            var apiBackendUrl = terraformVariables.SelectToken("api_backend_url").Value<string>();
            var apiManagementProductId = terraformVariables.SelectToken("api_management_product_id").Value<string>();
            var apiVersionSetId = terraformVariables.SelectToken("api_management_version_set_id").Value<string>();

            var outputVariablesApplied = generatedOutput.Replace("{api_management_service_name}", serviceName);
            outputVariablesApplied = outputVariablesApplied.Replace("{api_management_resource_group_name}", resourceGroupName);
            outputVariablesApplied = outputVariablesApplied.Replace("{api_path}", apiPath);
            outputVariablesApplied = outputVariablesApplied.Replace("{api_backend_url}", apiBackendUrl);
            outputVariablesApplied = outputVariablesApplied.Replace("{api_management_product_id}", apiManagementProductId);
            outputVariablesApplied = outputVariablesApplied.Replace("{api_management_version_set_id}", apiVersionSetId);
            return outputVariablesApplied;
        }
    }
}