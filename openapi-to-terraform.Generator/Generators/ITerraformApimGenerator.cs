using System;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.Generators
{
    public interface ITerraformApimGenerator
    {
        static string GenerateTerraformOutput(OpenApiDocument document) => throw new NotImplementedException();
    }
}