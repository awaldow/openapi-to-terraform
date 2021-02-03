using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using openapi_to_terraform.Generator.Generators;
using openapi_to_terraform.Generator.VariablesAppliers;

namespace openapi_to_terraform.Generator
{
    public class TerraformGenerator
    {
        private string OutputDir { get; set; }
        private string TerraformVarSubFile { get; set; }
        private string ApiTemplateFile { get; set; }
        private string OperationTemplateFile { get; set; }
        private string RevisionMappingFile { get; set; }

        public TerraformGenerator(string outputDir, string terraformVariablesFile, string revisionMap)
        {
            OutputDir = outputDir.EndsWith('/') ? outputDir : outputDir + '/';
            TerraformVarSubFile = terraformVariablesFile;
            RevisionMappingFile = revisionMap;
        }
        public TerraformGenerator(string outputDir, string apiTemplatePath, string operationTemplatePath, string revisionMap)
        {
            OutputDir = outputDir.EndsWith('/') ? outputDir : outputDir + '/';
            ApiTemplateFile = apiTemplatePath;
            OperationTemplateFile = operationTemplatePath;
            RevisionMappingFile = revisionMap;
        }

        public void GenerateWithTerraformVars(OpenApiDocument document)
        {
            var version = document.Info.Version;
            var revisions = new List<string>();

            if (RevisionMappingFile != null)
            {
                var revisionsMappingParsed = GetRevisions(JObject.Parse(File.ReadAllText(RevisionMappingFile)));
                revisions.AddRange(revisionsMappingParsed);
            }
            else
            {
                revisions.Add("1");
            }

            var apiFilePath = Path.Combine(OutputDir, version, $"api.{version}.tf");
            if (!Directory.Exists(Path.Combine(OutputDir, version)))
            {
                Console.WriteLine($"Creating directory {Path.Combine(OutputDir, version)}");
                Directory.CreateDirectory(Path.Combine(OutputDir, version));
            }
            var generatedApiOutput = ApiGenerator.GenerateTerraformOutput(document, revisions);
            System.IO.File.WriteAllText(apiFilePath, ApiVariablesApplier.ApplyVariables(generatedApiOutput, TerraformVarSubFile));

            if (RevisionMappingFile != null)
            {
                var operationFilePath = Path.Combine(OutputDir, version, $"operations.{version}.tf");
                var generatedOperationOutput = OperationGenerator.GenerateTerraformOutput(document, RevisionMappingFile);

                System.IO.File.WriteAllText(operationFilePath, ApiVariablesApplier.ApplyVariables(generatedOperationOutput, TerraformVarSubFile));
            }
            else
            {
                var operationFilePath = Path.Combine(OutputDir, version, $"operations.{version}.tf");
                var generatedOperationOutput = OperationGenerator.GenerateTerraformOutput(document);

                System.IO.File.WriteAllText(operationFilePath, ApiVariablesApplier.ApplyVariables(generatedOperationOutput, TerraformVarSubFile));
            }
        }

        private List<string> GetRevisions(JObject revisionMapping)
        {
            return revisionMapping.ToObject<Dictionary<string, string[]>>().Values.SelectMany(i => i).Distinct().ToList();
        }

        public void GenerateWithTemplateFiles(OpenApiDocument document)
        {
            throw new NotImplementedException();
        }
    }
}