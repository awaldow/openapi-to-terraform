using System.IO;
using System.Text;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using openapi_to_terraform.Generator.GeneratorModels;

namespace openapi_to_terraform.Generator.VariablesAppliers
{
    public class ApiVariablesApplier : IVariablesApplier
    {
        public static string ApplyVariables(string generatedOutput, string variablesPath)
        {
            // "api_management_service_name": "azurerm_api_management.roombyapim.name",
            // "api_management_resource_group_name": "azurerm_api_management.roombyapim.resource_group_name",
            // "api_path": "users",
            // "api_backend_url": "${data.azurerm_app_service.roombyusersapp.default_site_hostname}/api/",
            // "api_management_product_id": "azurerm_api_management_product.roombyproduct.product_id"
            JObject terraformVariables = JObject.Parse(File.ReadAllText(variablesPath));
            // TODO: substitute above variables in generatedOutput with the values in variablesPath
            throw new System.NotImplementedException();
        }
    }
}