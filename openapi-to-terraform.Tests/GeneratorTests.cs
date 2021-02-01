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
            var outputDir = "output_without_api_revisions";
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile });

            Directory.Exists(outputDir).Should().BeTrue();
            Directory.Exists(Path.Combine(outputDir, "1")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "operations.1.tf")).Should().BeTrue();

            var apiText = File.ReadAllText(Path.Combine(outputDir, "1", "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(1);

            if (Directory.Exists("output_without_api_revisions"))
            {
                Directory.Delete("output_without_api_revisions", true);
            }
        }

        [Fact]
        public void tool_should_generate_two_api_blocks_with_sample_revisions_mapping()
        {
            var outputDir = "output_with_api_revisions";
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";
            var revisionsMappingFile = "samples/sampleRevisionMap.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "-r", revisionsMappingFile });

            Directory.Exists(outputDir).Should().BeTrue();
            Directory.Exists(Path.Combine(outputDir, "1")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "1", "operations.1.tf")).Should().BeTrue();

            var apiText = File.ReadAllText(Path.Combine(outputDir, "1", "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(2);

            if (Directory.Exists("output_with_api_revisions"))
            {
                Directory.Delete("output_with_api_revisions", true);
            }
        }
    }
}
