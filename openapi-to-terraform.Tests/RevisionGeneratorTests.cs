using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace openapi_to_terraform.Tests
{
    public class RevisionGeneratorTests
    {
        private readonly ITestOutputHelper outputHelper;

        public RevisionGeneratorTests(ITestOutputHelper outputHelper) {
            this.outputHelper = outputHelper;
        }

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

            var depsFile = noRevPath.Replace(".dll", ".deps.json");
            var runtimeConfig = noRevPath.Replace(".dll", ".runtimeconfig.json");
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = string.Format(
                   "exec --depsfile {0} --runtimeconfig {1} {2} _{3}", // note the underscore
                   EscapePath(depsFile),
                   EscapePath(runtimeConfig),
                   EscapePath(typeof(openapi_to_terraform.RevisionCli.Program).GetTypeInfo().Assembly.Location),
                   string.Join(" ", new[] { "generate", "-f", openApiPath, "-a", noRevPath, "-o", outputPath }))
            };
            // var subProcess = Process.Start("dotnet", string.Format(
            //     "exec --depsfile {0} --runtimeconfig {1} {2} _{3}", // note the underscore
            //     EscapePath(depsFile),
            //     EscapePath(runtimeConfig),
            //     EscapePath(typeof(openapi_to_terraform.RevisionCli.Program).GetTypeInfo().Assembly.Location),
            //     string.Join(" ", new[] { "generate", "-f", openApiPath, "-a", noRevPath, "-o", outputPath })
            // ));
            var subProcess = Process.Start(startInfo);
            subProcess.WaitForExit();
            var output = subProcess.StandardOutput.ReadToEnd();
            outputHelper.WriteLine(output);

            subProcess.ExitCode.Should().NotBe(null);
            // subProcess.ExitCode.Should().Be(-2147450751);
            Directory.Exists(outputDir).Should().BeTrue();
            File.Exists(outputPath).Should().BeTrue();

            JObject revisions = JObject.Parse(File.ReadAllText(outputPath));
            revisions.Count.Should().Be(0); // No controllers in file have revision attribute, so empty json output
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
        }

        private static string EscapePath(string path)
        {
            return path.Contains(" ")
                ? "\"" + path + "\""
                : path;
        }

        [Fact]
        public void tool_should_generate_specific_revisions_with_attributes()
        {
            var outputDir = Regex.Replace($"output_with_revision_attributes_{DateTime.Now.ToString()}", @"[\s:\/]", "_");
            var outputPath = Path.Combine(outputDir, "revisions.json");
            //var noRevPath = "samples/sampleApi/sample.API.noRev.dll";
            var noRevPath = "../../../../openapi-to-terraform.Tests.TestAPI/bin/Debug/net5.0/openapi-to-terraform.Tests.TestAPI.dll";
            var openApiPath = "samples/testApi.v1.revs.json";

            var depsFile = noRevPath.Replace(".dll", ".deps.json");
            var runtimeConfig = noRevPath.Replace(".dll", ".runtimeconfig.json");
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = string.Format(
                    "exec --depsfile {0} --runtimeconfig {1} {2} _{3}", // note the underscore
                    EscapePath(depsFile),
                    EscapePath(runtimeConfig),
                    EscapePath(typeof(openapi_to_terraform.RevisionCli.Program).GetTypeInfo().Assembly.Location),
                    string.Join(" ", new[] { "generate", "-f", openApiPath, "-a", noRevPath, "-o", outputPath }))
            };
            // var subProcess = Process.Start("dotnet", string.Format(
            //     "exec --depsfile {0} --runtimeconfig {1} {2} _{3}", // note the underscore
            //     EscapePath(depsFile),
            //     EscapePath(runtimeConfig),
            //     EscapePath(typeof(openapi_to_terraform.RevisionCli.Program).GetTypeInfo().Assembly.Location),
            //     string.Join(" ", new[] { "generate", "-f", openApiPath, "-a", noRevPath, "-o", outputPath })
            // ));
            var subProcess = Process.Start(startInfo);

            subProcess.WaitForExit();
            var output = subProcess.StandardOutput.ReadToEnd();
            outputHelper.WriteLine(output);

            subProcess.ExitCode.Should().NotBe(null);
            // subProcess.ExitCode.Should().Be(-2147450751);
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