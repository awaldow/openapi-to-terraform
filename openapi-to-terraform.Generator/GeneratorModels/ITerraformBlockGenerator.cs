using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.GeneratorModels
{
    public interface ITerraformBlockGenerator
    {
        string GenerateBlock(OpenApiDocument document);
    }
}