using System;
using System.Text.RegularExpressions;
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
            var noRevPath = "/home/awaldow/source/repos/roomby/roomby.api/Roomby.API.Users/bin/Debug/net5.0/Roomby.API.Users.dll";
            var openApiPath = "samples/sampleOpenApi.json";
            var routePrefix = "/api/v1";

            Program.Main(new[] { "revisions", "-f", openApiPath, "-a", noRevPath , "-o", outputDir, "-p", routePrefix });
        }

        [Fact]
        public void tool_should_generate_specific_revisions_with_attributes()
        {

        }
    }
}