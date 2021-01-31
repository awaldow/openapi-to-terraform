using System;
using CommandLine;
using Microsoft.OpenApi.Models;
using openapi_to_terraform.Core;
using openapi_to_terraform.Main;

namespace openapi_to_terraform
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Console.WriteLine($"Parsing {o.InputFile}, outputting to {o.OutputDirectory}");
                       OpenApiParser p = new OpenApiParser(o.InputFile);
                       p.Parse();
                       OpenApiDocument test = p.Document;
                   });
        }
    }
}
