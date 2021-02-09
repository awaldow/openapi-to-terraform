using CommandLine;

namespace openapi_to_terraform.Main
{
    [Verb("gen-tf", HelpText = "Generates terraform files")]
    public class GenerateTerraformOptions
    {
        [Option('f', "input-file", Required = true, HelpText = "Absolute or relative path to OpenAPI input file")]
        public string InputFile { get; set; }

        [Option('o', "output-dir", Required = true, HelpText = "Absolute or relative path of output directory for terraform files")]
        public string OutputDirectory { get; set; }

        [Option('r', "revision-map-file", Required = false, HelpText = "(Not Implemented) Absolute or relative path to revision mapping json file")]
        public string RevisionFile { get; set; }

        [Option('t', "terraform-vars-file", SetName = "terraform-vars", HelpText = "Absolute or relative path to file mapping terraform variables to output values")]
        public string TerraformVariablesFile { get; set; }

        [Option('p', "provider", Default = "azurerm", Required = false, HelpText = "One of { azurerm }. Defaults to azurerm")]
        public string Provider { get; set; }

        [Option("provider-version", Default = "v2.45.1", Required = false, HelpText = "Version of the provider to use. Defaults to v2.45.1")]
        public string ProviderVersion { get; set; }

        [Option("api-template-file", SetName = "template-provided", HelpText = "(Not Implemented) Absolute or relative path to desired template of APIs terraform file")]
        public string ApiTemplateFile { get; set; }

        [Option("operation-template-file", SetName = "template-provided", HelpText = "(Not Implemented) Absolute or relative path to desired template of Operations terraform file")]
        public string OperationTemplateFile { get; set; }

        [Option("policies", Required = false, HelpText = "(Not Implemented) Absolute or relative path to directory containing APIM policies. Requires -r/--revision-map-file to be provided if revision dependent policies are desired")]
        public string PoliciesDirectoryPath { get; set; }
    }
}