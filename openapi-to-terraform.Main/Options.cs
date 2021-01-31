using CommandLine;

namespace openapi_to_terraform.Main
{
    public class Options
    {
        [Option('f', "input-file", Required = true, HelpText = "Absolute or relative path to swagger json input file")]
        public string InputFile { get; set; }
    }
}