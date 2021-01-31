namespace openapi_to_terraform.Generator.TemplateAppliers
{
    public interface ITemplateApplier
    {
        string ApplyTemplate(string generatedOutput, string templatePath);
    }
}