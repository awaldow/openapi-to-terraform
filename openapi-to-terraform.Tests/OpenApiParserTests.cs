using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using openapi_to_terraform.RevisionCli;
using Xunit;
using Xunit.Abstractions;

namespace openapi_to_terraform.Tests
{
    public class OpenApiParserTests
    {
        private readonly ITestOutputHelper outputHelper;

        public OpenApiParserTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact]
        public void json_parser_should_find_one_path_one_operation()
        {
            var openApiJson = "samples/testApi.v1.revs.json";
            var openApiText = File.ReadAllText(openApiJson);
            var openApiRoot = new OpenApiParser(openApiText).Read();

            openApiRoot.Paths.Count().Should().Be(1);
            openApiRoot.Paths.Select(o => o.Operations).Count().Should().Be(1);
        }

        [Fact]
        public void json_parser_should_find_four_paths_five_operations()
        {
            var openApiJson = "samples/sampleOpenApi.json";
            var openApiText = File.ReadAllText(openApiJson);
            var openApiRoot = new OpenApiParser(openApiText).Read();

            openApiRoot.Paths.Count().Should().Be(4);
            openApiRoot.Paths.SelectMany(o => o.Operations).Count().Should().Be(5);
        }
    }
}