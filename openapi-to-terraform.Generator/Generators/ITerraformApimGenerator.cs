using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.Generators
{
    public interface ITerraformApimGenerator
    {
        string GenerateTerraformOutput(OpenApiDocument document);
    }
}