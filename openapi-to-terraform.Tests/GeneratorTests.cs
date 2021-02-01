using System;
using System.IO;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace openapi_to_terraform.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void tool_should_generate_one_api_block_without_revisions_mapping()
        {
            var outputDir = $"output_without_api_revisions_{DateTime.Now.ToString()}".Replace("/", "_");
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile });

            File.Copy("samples/sampleMainModule.tf", Path.Combine(outputDir, "1", "sampleMainModule.tf"));
            File.Copy("samples/sampleMainApim.tf", Path.Combine(outputDir, "1", "sampleMainApim.tf"));
            File.Copy("samples/sampleApimVariables.tf", Path.Combine(outputDir, "1", "sampleApimVariables.tf"));

            Directory.Exists(outputDir).Should().BeTrue();
            Directory.Exists(Path.Combine(outputDir, "1")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "operations.1.tf")).Should().BeTrue();

            var apiText = File.ReadAllText(Path.Combine(outputDir, "1", "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(1);

            // TODO: Keep this commented until we have a way to inject a basic APIM instance and run 
            //  terraform validate and use its output to determine whether we're breaking any terraform rules
            // if (Directory.Exists(outputDir))
            // {
            //     Directory.Delete(outputDir, true);
            // }
        }

        [Fact]
        public void tool_should_generate_two_api_blocks_with_sample_revisions_mapping()
        {
            var outputDir = $"output_with_api_revisions_{DateTime.Now.ToString()}".Replace("/", "_");
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";
            var revisionsMappingFile = "samples/sampleRevisionMap.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "-r", revisionsMappingFile });

            File.Copy("samples/sampleMainModule.tf", Path.Combine(outputDir, "1"));
            File.Copy("samples/sampleMainApim.tf", Path.Combine(outputDir, "1"));
            File.Copy("samples/sampleApimVariables.tf", Path.Combine(outputDir, "1", "sampleApimVariables.tf"));

            Directory.Exists(outputDir).Should().BeTrue();
            Directory.Exists(Path.Combine(outputDir, "1")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "operations.1.tf")).Should().BeTrue();

            var apiText = File.ReadAllText(Path.Combine(outputDir, "1", "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(2);

            // TODO: Keep this commented until we have a way to inject a basic APIM instance and run 
            //  terraform validate and use its output to determine whether we're breaking any terraform rules
            // if (Directory.Exists(outputDir))
            // {
            //     Directory.Delete(outputDir, true);
            // }
        }
    }
}
