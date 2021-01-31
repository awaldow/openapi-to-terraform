using System;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.GeneratorModels
{
    public interface ITerraformBlockGenerator
    {
        static string GenerateBlock(OpenApiDocument document) => throw new NotImplementedException();
    }
}