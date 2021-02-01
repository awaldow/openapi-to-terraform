using System;

namespace openapi_to_terraform.Generator.TemplateAppliers
{
    public interface ITemplateApplier
    {
        static string ApplyTemplate(string generatedOutput, string templatePath) => throw new NotImplementedException();
    }
}