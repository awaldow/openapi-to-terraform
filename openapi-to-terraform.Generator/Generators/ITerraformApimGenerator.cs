using System;
using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator.Generators
{
    public interface ITerraformApimGenerator
    {
        static string GenerateTerraformOutput() => throw new NotImplementedException();
    }
}