using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace openapi_to_terraform.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void tool_should_generate_one_api_block_without_revisions_mapping()
        {
            var outputDir = Regex.Replace($"output_without_api_revisions_{DateTime.Now.ToString()}", @"[\s:\/]", "_");
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile });

            File.Copy("samples/sampleMainModule.tf", Path.Combine(outputDir, "sampleMainModule.tf"));
            File.Copy("samples/sampleMainApim.tf", Path.Combine(outputDir, "sampleMainApim.tf"));
            File.Copy("samples/sampleApimVariables.tf", Path.Combine(outputDir, "sampleApimVariables.tf"));

            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "operations.1.tf")).Should().BeTrue();

            var apiText = File.ReadAllText(Path.Combine(outputDir, "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(1);

            var terraformInit = "terraform init -backend=false";
            System.Diagnostics.Process.Start("bash", terraformInit).WaitForExit();

            Process validate = new Process();
            validate.StartInfo.UseShellExecute = false;
            validate.StartInfo.RedirectStandardOutput = true;
            validate.StartInfo.FileName = "terraform";
            validate.StartInfo.Arguments = "validate -json";
            validate.Start();
            string output = validate.StandardOutput.ReadToEnd();
            validate.WaitForExit();
            bool valid = (bool)(((dynamic)JsonConvert.DeserializeObject(output))["valid"]);

            valid.Should().Be(true);

            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
        }

        [Fact]
        public void tool_should_generate_two_api_blocks_with_sample_revisions_mapping()
        {
            var outputDir = Regex.Replace($"output_with_api_revisions_{DateTime.Now.ToString()}", @"[\s:\/]", "_");
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";
            var revisionsMappingFile = "samples/sampleRevisionMap.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "-r", revisionsMappingFile });

            File.Copy("samples/sampleMainModule.tf", Path.Combine(outputDir, "sampleMainModule.tf"));
            File.Copy("samples/sampleMainApim.tf", Path.Combine(outputDir, "sampleMainApim.tf"));
            File.Copy("samples/sampleApimVariables.tf", Path.Combine(outputDir,"sampleApimVariables.tf"));

            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "operations.1.tf")).Should().BeTrue();

            var apiText = File.ReadAllText(Path.Combine(outputDir, "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(2);

            var terraformInit = "terraform init -backend=false";
            System.Diagnostics.Process.Start("bash", terraformInit).WaitForExit();

            Process validate = new Process();
            validate.StartInfo.UseShellExecute = false;
            validate.StartInfo.RedirectStandardOutput = true;
            validate.StartInfo.FileName = "terraform";
            validate.StartInfo.Arguments = "validate -json";
            validate.Start();
            string output = validate.StandardOutput.ReadToEnd();
            validate.WaitForExit();
            bool valid = (bool)(((dynamic)JsonConvert.DeserializeObject(output))["valid"]);

            valid.Should().Be(true);

            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
        }
    }
}
