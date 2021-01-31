using System;
using CommandLine;

namespace openapi_to_terraform
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Console.WriteLine($"Parsing {o.InputFile}");
                   });
        }
    }
}
