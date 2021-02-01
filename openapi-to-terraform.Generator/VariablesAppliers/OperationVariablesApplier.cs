using System.IO;
using Newtonsoft.Json.Linq;

namespace openapi_to_terraform.Generator.VariablesAppliers
{
    public class OperationVariablesApplier : IVariablesApplier
    {
        public static string ApplyVariables(string generatedOutput, string variablesPath)
        {
            //  "api_management_service_name": "azurerm_api_management.roombyapim.name",
            //  "api_management_resource_group_name": "azurerm_api_management.roombyapim.resource_group_name"
            //  "api_name": "users"
            JObject terraformVariables = JObject.Parse(File.ReadAllText(variablesPath));
            throw new System.NotImplementedException();
        }
    }
}