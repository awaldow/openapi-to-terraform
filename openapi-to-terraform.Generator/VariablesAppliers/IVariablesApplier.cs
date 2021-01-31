using System;

namespace openapi_to_terraform.Generator.VariablesAppliers
{
    public interface IVariablesApplier
    {
        static string ApplyVariables(string generatedOutput, string variablesPath) => throw new NotImplementedException();
    }
}