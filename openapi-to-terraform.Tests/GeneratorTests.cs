using System;
using Xunit;

namespace openapi_to_terraform.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void tool_should_reject_mixing_variable_groups() 
        {
            var outputDir = "output";
            var sampleOpenApi = "sampleOpenApi.json";
            var terraformSubVarsFile = "sampleTerraformVars.json";
            var apiTemplateFile = "sampleApiTemplate.tf";

            Action act = () => Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "--api-template-file", apiTemplateFile });
            act.Should().Throw<MutuallyExclusiveSetError>();
        }

        [Fact]
        public void tool_should_generate_one_api_block_without_revisions_mapping()
        {
            var outputDir = "output";
            var sampleOpenApi = "sampleOpenApi.json";
            var terraformSubVarsFile = "sampleTerraformVars.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile });

            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "operations.1.tf")).Should().BeTrue();

            var apiText = File.ReadAllText(Path.Combine(outputDir, "1", "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(1);
        }

        [Fact]
        public void tool_should_generate_two_api_blocks_with_sample_revisions_mapping()
        {
            var outputDir = "output";
            var sampleOpenApi = "sampleOpenApi.json";
            var terraformSubVarsFile = "sampleTerraformVars.json";
            var revisionsMappingFile = "sampleRevisionMap.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "-r", revisionsMappingFile });

            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "operations.1.tf")).Should().BeTrue();

            var apiText = File.ReadAllText(Path.Combine(outputDir, "1", "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(2);
        }
    }
}
