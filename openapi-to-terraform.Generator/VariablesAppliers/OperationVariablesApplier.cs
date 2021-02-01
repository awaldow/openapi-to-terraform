using System.IO;
using Newtonsoft.Json.Linq;

namespace openapi_to_terraform.Generator.VariablesAppliers
{
    public class OperationVariablesApplier
    {
        public static string ApplyVariables(string generatedOutput, string variablesPath)
        {
            //  "api_management_service_name": "azurerm_api_management.roombyapim.name",
            //  "api_management_resource_group_name": "azurerm_api_management.roombyapim.resource_group_name"
            JObject terraformVariables = JObject.Parse(File.ReadAllText(variablesPath));
            var serviceName = terraformVariables.SelectToken("api_management_service_name").Value<string>();
            var resourceGroupName = terraformVariables.SelectToken("api_management_resource_group_name").Value<string>();

            var outputVariablesApplied = generatedOutput.Replace("{api_management_service_name}", serviceName);
            outputVariablesApplied = outputVariablesApplied.Replace("{api_management_resource_group_name}", resourceGroupName);
            return outputVariablesApplied;
        }
    }
}