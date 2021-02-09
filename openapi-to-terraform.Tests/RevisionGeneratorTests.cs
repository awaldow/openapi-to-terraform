using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using openapi_to_terraform.Extensions.Attributes;
using openapi_to_terraform.RevisionCli;
using Xunit;
using Xunit.Abstractions;

namespace openapi_to_terraform.Tests
{
    public class RevisionCliTests
    {
        private readonly ITestOutputHelper outputHelper;

        public RevisionCliTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact]
        public void argsParser_should_parse_good_args()
        {
            var args = new string[] { "generate", "-a", "testAssemblyPath", "-o", "testOutputPath", "-f", "testOpenApiPath" };
            var longArgs = new string[] { "_generate", "--input-assembly-path", "testAssemblyPath", "--output-path", "testOutputPath", "--open-api-file", "testOpenApiPath" };
            var mixedArgs = new string[] { "generate", "--input-assembly-path", "testAssemblyPath", "-o", "testOutputPath", "--open-api-file", "testOpenApiPath" };

            var shortArgsParserExitCode = ArgsParser.TryParse(args, out ArgsParser shortArgsParser);
            var longArgsParserExitCode = ArgsParser.TryParse(longArgs, out ArgsParser longArgsParser);
            var mixedArgsParserExitCode = ArgsParser.TryParse(mixedArgs, out ArgsParser mixedArgsParser);

            shortArgsParserExitCode.Should().Be(ExitCode.Success);
            shortArgsParser.Command.Should().Be("generate");
            shortArgsParser.InputAssemblyPath.Should().Be("testAssemblyPath");
            shortArgsParser.OutputPath.Should().Be("testOutputPath");
            shortArgsParser.OpenApiPath.Should().Be("testOpenApiPath");

            longArgsParserExitCode.Should().Be(ExitCode.Success);
            longArgsParser.Command.Should().Be("_generate");
            longArgsParser.InputAssemblyPath.Should().Be("testAssemblyPath");
            longArgsParser.OutputPath.Should().Be("testOutputPath");
            longArgsParser.OpenApiPath.Should().Be("testOpenApiPath");


            mixedArgsParserExitCode.Should().Be(ExitCode.Success);
            mixedArgsParser.Command.Should().Be("generate");
            mixedArgsParser.InputAssemblyPath.Should().Be("testAssemblyPath");
            mixedArgsParser.OutputPath.Should().Be("testOutputPath");
            mixedArgsParser.OpenApiPath.Should().Be("testOpenApiPath");
        }

        [Fact]
        public void argsParser_should_fail_on_bad_args()
        {
            var args = new string[] { "generate", "-a", "test", "-o", "-f", "test" };
            var longArgs = new string[] { "_generate", "--input-assembly-path", "--output-path", "test", "--open-api-file", "test" };
            var mixedArgs = new string[] { "generate", "--input-assembly-path", "test", "-o", "test", "--open-api-file" };

            var shortArgsParserExitCode = ArgsParser.TryParse(args, out ArgsParser shortArgsParser);
            var longArgsParserExitCode = ArgsParser.TryParse(args, out ArgsParser longArgsParser);
            var mixedArgsParserExitCode = ArgsParser.TryParse(args, out ArgsParser mixedArgsParser);

            shortArgsParserExitCode.Should().Be(ExitCode.Error);
            longArgsParserExitCode.Should().Be(ExitCode.Error);
            mixedArgsParserExitCode.Should().Be(ExitCode.Error);
        }

        [Fact]
        public void argsParser_should_fail_on_unknown_command()
        {
            var badCommandArgs = new string[] { "tron", "--input-assembly-path", "test", "-o", "test", "--open-api-file", "test" };
            var badCommandArgsParserExitCode = ArgsParser.TryParse(badCommandArgs, out ArgsParser badCommandArgsParser);
            badCommandArgsParserExitCode.Should().Be(ExitCode.CommandUnknown);
        }

        [Fact]
        public void tool_should_generate_controller_revisions_if_attribute_on_controller()
        {
#if DEBUG
            var assemblyPath = "../../../../openapi-to-terraform.Tests.TestAPIControllerRevisions/bin/Debug/net5.0/openapi-to-terraform.Tests.TestAPIControllerRevisions.dll";
#else
            var assemblyPath = "../../../../openapi-to-terraform.Tests.TestAPIControllerRevisions/bin/Release/net5.0/openapi-to-terraform.Tests.TestAPIControllerRevisions.dll";
#endif
            var convertedAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyPath);
            var openApiPath = "samples/controllerRevs.v1.json";
            var openApiText = File.ReadAllText(openApiPath);

            var docTree = new OpenApiParser(openApiText).Read();
            Assembly startupAssembly = Assembly.LoadFrom(convertedAssemblyPath);
            var revisions = RevisionsGenerator.Generate(startupAssembly, docTree);
            IDictionary<string, JToken> revisionsFileParsed = JObject.Parse(revisions);
            revisionsFileParsed.Keys.Count.Should().Be(1); // Only one controller with one action in openapi file, so should be one key
            var revisionsCount = revisionsFileParsed.First().Value.Count();
            revisionsCount.Should().Be(2); // The controller has two revisions defined, so there should be two here
        }

        [Fact]
        public void tool_should_generate_action_revisions_if_attributes_on_actions()
        {
#if DEBUG
            var assemblyPath = "../../../../openapi-to-terraform.Tests.TestAPI/bin/Debug/net5.0/openapi-to-terraform.Tests.TestAPI.dll";
#else
            var assemblyPath = "../../../../openapi-to-terraform.Tests.TestAPI/bin/Release/net5.0/openapi-to-terraform.Tests.TestAPI.dll";
#endif
            var convertedAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyPath);
            var openApiPath = "samples/actionRevs.v1.json";
            var openApiText = File.ReadAllText(openApiPath);

            var docTree = new OpenApiParser(openApiText).Read();
            Assembly startupAssembly = Assembly.LoadFrom(convertedAssemblyPath);
            var revisions = RevisionsGenerator.Generate(startupAssembly, docTree);
            IDictionary<string, JToken> revisionsFileParsed = JObject.Parse(revisions);
            revisionsFileParsed.Keys.Count.Should().Be(2); 
            var revisionsFirstCount = revisionsFileParsed.First().Value.Count();
            revisionsFirstCount.Should().Be(2); 
            var revisionsSecondCount = revisionsFileParsed.Last().Value.Count();
            revisionsSecondCount.Should().Be(1); 
        }

        [Fact]
        public void tool_should_generate_no_revisions_if_no_attributes()
        {
#if DEBUG
            var assemblyPath = "../../../../openapi-to-terraform.Tests.TestAPINoRevs/bin/Debug/net5.0/openapi-to-terraform.Tests.TestAPINoRevs.dll";
#else
            var assemblyPath = "../../../../openapi-to-terraform.Tests.TestAPINoRevs/bin/Release/net5.0/openapi-to-terraform.Tests.TestAPINoRevs.dll";
#endif
            var convertedAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyPath);
            var openApiPath = "samples/noRevs.v1.json";
            var openApiText = File.ReadAllText(openApiPath);

            var docTree = new OpenApiParser(openApiText).Read();
            Assembly startupAssembly = Assembly.LoadFrom(convertedAssemblyPath);
            var revisions = RevisionsGenerator.Generate(startupAssembly, docTree);
            IDictionary<string, JToken> revisionsFileParsed = JObject.Parse(revisions);
            revisionsFileParsed.Keys.Count.Should().Be(1); 
            var revisionsFirstCount = revisionsFileParsed.First().Value.Count();
            revisionsFirstCount.Should().Be(1); 
        }
    }
}