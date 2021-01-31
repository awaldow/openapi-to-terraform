using CommandLine;

namespace openapi_to_terraform.Main
{
    public class Options
    {
        [Option('f', "input-file", Required = true, HelpText = "Absolute or relative path to swagger json input file")]
        public string InputFile { get; set; }

        [Option('o', "output-dir", Required = true, HelpText = "Absolute or relative path of output directory for terraform files")]
        public string OutputDirectory { get; set; }

        [Option('r', "revision-map-file", Required = false, HelpText = "Absolute or relative path to revision mapping json file")]
        public string RevisionFile { get; set; }

        [Option("api-template-file", Required = false, HelpText = "Absolute or relative path to desired template of APIs terraform file")]
        public string ApiTemplateFile { get; set; }

        [Option("operation-template-file", Required = false, HelpText = "Absolute or relative path to desired template of Operations terraform file")]
        public string OperationTemplateFile { get; set; }
    }
}