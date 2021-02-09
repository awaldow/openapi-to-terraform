using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using openapi_to_terraform.RevisionCli;
using Xunit;
using Xunit.Abstractions;

namespace openapi_to_terraform.Tests
{
    public class RevisionGeneratorTests
    {
        private readonly ITestOutputHelper outputHelper;

        public RevisionGeneratorTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact]
        public void tool_should_generate_controller_revisions_if_attribute_on_controller()
        {
            var assemblyPath = "../../../../openapi-to-terraform.Tests.TestAPI/bin/Debug/net5.0/openapi-to-terraform.Tests.TestAPI.dll";
            var convertedAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyPath);
            var openApiPath = "samples/testApi.v1.revs.json";
            var openApiText = File.ReadAllText(openApiPath);

            var docTree = new OpenApiParser(openApiText).Read();
            Assembly startupAssembly = Assembly.LoadFrom(convertedAssemblyPath);
            var revisions = RevisionsGenerator.Generate(startupAssembly, docTree);
            IDictionary<string, JToken> revisionsFileParsed = JObject.Parse(revisions);
            revisionsFileParsed.Keys.Count.Should().Be(1); // Only one controller with one action in openapi file, so should be one key
            var revisionsCount = revisionsFileParsed.First().Value.Count();
            revisionsCount.Should().Be(2); // The controller has two revisions defined, so there should be two here
        }
    }
}