using System.IO;
using Microsoft.OpenApi.Models;
using openapi_to_terraform.Generator.Generators;

namespace openapi_to_terraform.Generator
{
    public class Generator
    {
        private string OutputDir { get; set; }
        public Generator(string outputDir)
        {
            OutputDir = outputDir.EndsWith('/') ? outputDir : outputDir + '/';
        }

        public async void Generate(OpenApiDocument document)
        {
            var version = document.Info.Version;
            await System.IO.File.WriteAllTextAsync(Path.Combine(OutputDir, version, $"api.{version}.tf"), new ApiGenerator().GenerateTerraformOutput(document));
            await System.IO.File.WriteAllTextAsync(Path.Combine(OutputDir, version, $"operations.{version}.tf"), new OperationGenerator().GenerateTerraformOutput(document));
        }
    }
}