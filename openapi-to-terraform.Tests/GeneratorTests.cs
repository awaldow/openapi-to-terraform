using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace openapi_to_terraform.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void tool_should_fail_with_bad_provider_settings()
        {
            var outputDir = Regex.Replace($"output_without_api_revisions_{DateTime.Now.ToString()}", @"[\s:\/]", "_");
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "-p", "aws" });

            Directory.Exists(outputDir).Should().BeFalse();
            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "--provider-version", "v0.1" });

            Directory.Exists(outputDir).Should().BeFalse();
            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "-p", "aws", "--provider-version", "v0.1" });

            Directory.Exists(outputDir).Should().BeFalse();
        }

        [Fact]
        public void tool_should_generate_one_api_block_without_revisions_mapping()
        {
            var outputDir = Regex.Replace($"output_without_api_revisions_{DateTime.Now.ToString()}", @"[\s:\/]", "_");
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile });

            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "operations.1.tf")).Should().BeTrue();

            File.Copy("samples/sampleMainModule.tf", Path.Combine(outputDir, "sampleMainModule.tf"));
            File.Copy("samples/sampleMainApim.tf", Path.Combine(outputDir, "sampleMainApim.tf"));
            File.Copy("samples/sampleApimVariables.tf", Path.Combine(outputDir, "sampleApimVariables.tf"));

            var apiText = File.ReadAllText(Path.Combine(outputDir, "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(1);

            var operationText = File.ReadAllText(Path.Combine(outputDir, "operations.1.tf"));
            int operationPolicyBlockCount = Regex.Matches(operationText, "resource \"azurerm_api_management_api_operation\"").Count;
            operationPolicyBlockCount.Should().Be(5); // not using the revisions file is less restrictive, so you would expect all 5 operations to show up

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

            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "operations.1.tf")).Should().BeTrue();

            File.Copy("samples/sampleMainModule.tf", Path.Combine(outputDir, "sampleMainModule.tf"));
            File.Copy("samples/sampleMainApim.tf", Path.Combine(outputDir, "sampleMainApim.tf"));
            File.Copy("samples/sampleApimVariables.tf", Path.Combine(outputDir, "sampleApimVariables.tf"));

            var apiText = File.ReadAllText(Path.Combine(outputDir, "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(2);

            var operationText = File.ReadAllText(Path.Combine(outputDir, "operations.1.tf"));
            int operationBlockCount = Regex.Matches(operationText, "resource \"azurerm_api_management_api_operation\"").Count;
            operationBlockCount.Should().Be(3); // Only 3 are mapped in the revisions file

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
        public void tool_should_generate_revision_restricted_policies_if_given_policy_root_dir_and_revision_map()
        {
            var outputDir = Regex.Replace($"output_with_api_revisions_{DateTime.Now.ToString()}", @"[\s:\/]", "_");
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";
            var revisionsMappingFile = "samples/sampleRevisionMap.json";
            var policyRootDir = "samples/samplePoliciesRev";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "-r", revisionsMappingFile, "--policies", policyRootDir });

            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "operations.1.tf")).Should().BeTrue();

            File.Copy("samples/sampleMainModule.tf", Path.Combine(outputDir, "sampleMainModule.tf"));
            File.Copy("samples/sampleMainApim.tf", Path.Combine(outputDir, "sampleMainApim.tf"));
            File.Copy("samples/sampleApimVariables.tf", Path.Combine(outputDir, "sampleApimVariables.tf"));

            var apiText = File.ReadAllText(Path.Combine(outputDir, "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(2);
            int policyBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api_policy\"").Count;
            policyBlockCount.Should().Be(2);

            var operationText = File.ReadAllText(Path.Combine(outputDir, "operations.1.tf"));
            int operationPolicyBlockCount = Regex.Matches(operationText, "resource \"azurerm_api_management_api_operation_policy\"").Count;
            operationPolicyBlockCount.Should().Be(3); // Because revision mapping is being used, we would expect only 3 policies since there are only three operations

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
        public void tool_should_generate_all_available_policies_if_given_policy_root_dir_and_no_revision_map()
        {
            var outputDir = Regex.Replace($"output_with_api_revisions_{DateTime.Now.ToString()}", @"[\s:\/]", "_");
            var sampleOpenApi = "samples/sampleOpenApi.json";
            var terraformSubVarsFile = "samples/sampleTerraformVars.json";
            var policyRootDir = "samples/samplePoliciesRev";

            Program.Main(new[] { "-f", sampleOpenApi, "-o", outputDir, "-t", terraformSubVarsFile, "--policies", policyRootDir });

            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "api.1.tf")).Should().BeTrue();
            File.Exists(Path.Combine(outputDir, "operations.1.tf")).Should().BeTrue();

            File.Copy("samples/sampleMainModule.tf", Path.Combine(outputDir, "sampleMainModule.tf"));
            File.Copy("samples/sampleMainApim.tf", Path.Combine(outputDir, "sampleMainApim.tf"));
            File.Copy("samples/sampleApimVariables.tf", Path.Combine(outputDir, "sampleApimVariables.tf"));

            var apiText = File.ReadAllText(Path.Combine(outputDir, "api.1.tf"));
            int apiBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api\"").Count;
            apiBlockCount.Should().Be(1);
            int policyBlockCount = Regex.Matches(apiText, "resource \"azurerm_api_management_api_policy\"").Count;
            policyBlockCount.Should().Be(1);

            var operationText = File.ReadAllText(Path.Combine(outputDir, "operations.1.tf"));
            int operationBlockCount = Regex.Matches(operationText, "resource \"azurerm_api_management_api_operation\"").Count;
            operationBlockCount.Should().Be(5); // All available actions should be available
            int operationPolicyBlockCount = Regex.Matches(operationText, "resource \"azurerm_api_management_api_operation_policy\"").Count;
            operationPolicyBlockCount.Should().Be(2); // Because revision mapping is being used, we would expect only 2 policies since there are only 2 defined for rev1 in the folder structure

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
