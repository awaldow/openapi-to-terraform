using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace openapi_to_terraform.Tests
{
    public class RevisionGeneratorTests
    {
        [Fact]
        public void tool_should_fail_with_bad_assembly()
        {
        }

        [Fact]
        public void tool_should_generate_broad_revisions_without_attributes()
        {
            var outputDir = Regex.Replace($"output_without_revision_attributes_{DateTime.Now.ToString()}", @"[\s:\/]", "_");
            var outputPath = Path.Combine(outputDir, "revisions.json");
            //var noRevPath = "samples/sampleApi/sample.API.noRev.dll";
            var noRevPath = "../../../../openapi-to-terraform.Tests.TestAPI/bin/Debug/net5.0/openapi-to-terraform.Tests.TestAPI.dll";
            var openApiPath = "samples/testApi.v2.noRev.json";

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var exitCode = openapi_to_terraform.RevisionCli.Program.Main(new[] { "generate", "-f", openApiPath, "-a", noRevPath, "-o", outputPath });

            exitCode.Should().Be(0);
            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(outputPath).Should().BeTrue();

            JObject revisions = JObject.Parse(File.ReadAllText(outputPath));
            revisions.Count.Should().Be(0); // No controllers in file have revision attribute, so empty json output
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
        }

        [Fact]
        public void tool_should_generate_specific_revisions_with_attributes()
        {
            var outputDir = Regex.Replace($"output_with_revision_attributes_{DateTime.Now.ToString()}", @"[\s:\/]", "_");
            var outputPath = Path.Combine(outputDir, "revisions.json");
            //var noRevPath = "samples/sampleApi/sample.API.noRev.dll";
            var noRevPath = "../../../../openapi-to-terraform.Tests.TestAPI/bin/Debug/net5.0/openapi-to-terraform.Tests.TestAPI.dll";
            var openApiPath = "samples/testApi.v1.revs.json";

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var exitCode = openapi_to_terraform.RevisionCli.Program.Main(new[] { "generate", "-f", openApiPath, "-a", noRevPath, "-o", outputPath });

            exitCode.Should().Be(0);
            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(outputPath).Should().BeTrue();

            JObject revisions = JObject.Parse(File.ReadAllText(outputPath));
            revisions.Count.Should().Be(1); // Only 1 controller action has the Revision attributes on it, so only expect one output
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
        }
    }
}