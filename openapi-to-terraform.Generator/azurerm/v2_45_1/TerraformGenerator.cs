using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using openapi_to_terraform.Generator.azurerm.v2_45_1.Generators;
using openapi_to_terraform.Generator.azurerm.v2_45_1.VariablesAppliers;

namespace openapi_to_terraform.Generator.azurerm.v2_45_1
{
    public class TerraformGenerator : ITerraformGenerator
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

            var apiFilePath = Path.Combine(OutputDir, $"api.{version}.tf");
            if (!Directory.Exists(Path.Combine(OutputDir)))
            {
                Console.WriteLine($"Creating directory {Path.Combine(OutputDir)}");
                Directory.CreateDirectory(Path.Combine(OutputDir));
            }
            var generatedApiOutput = ApiGenerator.GenerateTerraformOutput(document, revisions);
            System.IO.File.WriteAllText(apiFilePath, ApiVariablesApplier.ApplyVariables(generatedApiOutput, TerraformVarSubFile));

            if (RevisionMappingFile != null)
            {
                var operationFilePath = Path.Combine(OutputDir, $"operations.{version}.tf");
                var generatedOperationOutput = OperationGenerator.GenerateTerraformOutput(document, RevisionMappingFile);

                System.IO.File.WriteAllText(operationFilePath, ApiVariablesApplier.ApplyVariables(generatedOperationOutput, TerraformVarSubFile));
            }
            else
            {
                var operationFilePath = Path.Combine(OutputDir, $"operations.{version}.tf");
                var backendServiceUrl = JObject.Parse(File.ReadAllText(TerraformVarSubFile)).SelectToken("api_backend_url").Value<string>();
                var generatedOperationOutput = OperationGenerator.GenerateTerraformOutput(document, backendServiceUrl);

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