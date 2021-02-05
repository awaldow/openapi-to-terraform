using Microsoft.OpenApi.Models;

namespace openapi_to_terraform.Generator {
    interface ITerraformGenerator {
        void GenerateWithTerraformVars(OpenApiDocument document);
        void GenerateWithTemplateFiles(OpenApiDocument document);
    }
}