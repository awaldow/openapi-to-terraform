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
using Xunit;
using Xunit.Abstractions;

namespace openapi_to_terraform.Tests
{
    public class OpenApiParserTests
    {
        private readonly ITestOutputHelper outputHelper;

        public OpenApiParserTests(ITestOutputHelper outputHelper) {
            this.outputHelper = outputHelper;
        }
    }
}