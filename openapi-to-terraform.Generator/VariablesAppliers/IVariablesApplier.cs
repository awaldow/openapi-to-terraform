namespace openapi_to_terraform.Generator.VariablesAppliers
{
    public interface IVariablesApplier
    {
        string ApplyVariables(string generatedOutput);
    }
}