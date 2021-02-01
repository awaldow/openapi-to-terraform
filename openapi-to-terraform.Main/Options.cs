using CommandLine;

namespace openapi_to_terraform.Main
{
    public class Options
    {
        [Option('f', "input-file", Required = true, HelpText = "Absolute or relative path to swagger json input file")]
        public string InputFile { get; set; }

        [Option('o', "output-dir", Required = true,  HelpText = "Absolute or relative path of output directory for terraform files")]
        public string OutputDirectory { get; set; }

        [Option('r', "revision-map-file", Required = false, HelpText = "Absolute or relative path to revision mapping json file")]
        public string RevisionFile { get; set; }

        [Option('p', "--policies", Required = false, HelpText = "Absolute or relative path to directory containing APIM policies")]
        public string PoliciesDirectoryPath { get; set; }

        [Option('t', "terraform-vars-file", SetName = "terraform-vars", HelpText = "Absolute or relative path to file mapping terraform variables to output values")]
        public string TerraformVariablesFile { get; set; }
        
        [Option("api-template-file", SetName = "template-provided", HelpText = "Absolute or relative path to desired template of APIs terraform file")]
        public string ApiTemplateFile { get; set; }

        [Option("operation-template-file", SetName = "template-provided", HelpText = "Absolute or relative path to desired template of Operations terraform file")]
        public string OperationTemplateFile { get; set; }
    }
}