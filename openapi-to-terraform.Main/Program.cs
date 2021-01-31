﻿using System;
using CommandLine;
using Microsoft.OpenApi.Models;
using openapi_to_terraform.Core;
using openapi_to_terraform.Main;
using openapi_to_terraform.Generator;
using System.Threading.Tasks;

namespace openapi_to_terraform
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(async o =>
                   {
                       Console.WriteLine($"Parsing {o.InputFile}, outputting to {o.OutputDirectory}");
                       OpenApiParser p = new OpenApiParser(o.InputFile);
                       p.Parse();
                       if (o.TerraformVariablesFile != null)
                       {
                           var generator = new TerraformGenerator(o.OutputDirectory, o.TerraformVariablesFile, o.RevisionFile);
                           await generator.GenerateWithTerraformVars(p.Document);
                       }
                       else if (o.ApiTemplateFile != null && o.OperationTemplateFile != null)
                       {
                           var generator = new TerraformGenerator(o.OutputDirectory, o.ApiTemplateFile, o.OperationTemplateFile, o.RevisionFile);
                           await generator.GenerateWithTemplateFiles(p.Document);
                       }
                   });
        }
    }
}
