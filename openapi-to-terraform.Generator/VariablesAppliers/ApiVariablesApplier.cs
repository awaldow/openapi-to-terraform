using System.Text;
using Microsoft.OpenApi.Models;
using openapi_to_terraform.Generator.GeneratorModels;

namespace openapi_to_terraform.Generator.VariablesAppliers
{
    public class ApiVariablesApplier : IVariablesApplier
    {
        private string VariablesPath { get; set; }
        public ApiVariablesApplier(string variablesPath)
        {
            VariablesPath = VariablesPath;
        }

        public string ApplyVariables(string generatedOutput)
        {
            throw new System.NotImplementedException();
        }
    }
}