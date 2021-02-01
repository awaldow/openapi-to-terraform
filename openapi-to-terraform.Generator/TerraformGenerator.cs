using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task GenerateWithTerraformVars(OpenApiDocument document)
        {
            var version = document.Info.Version;
            var revisions = new List<string>();

            if (RevisionMappingFile != null)
            {
                revisions.AddRange(GetRevisions(JObject.Parse(RevisionMappingFile)));
            }
            else
            {
                revisions.Add("1");
            }

            var apiFilePath = Path.Combine(OutputDir, version, $"api.{version}.tf");
            var generatedApiOutput = ApiGenerator.GenerateTerraformOutput(document, revisions);
            await System.IO.File.WriteAllTextAsync(apiFilePath, ApiVariablesApplier.ApplyVariables(generatedApiOutput, TerraformVarSubFile));

            var operationFilePath = Path.Combine(OutputDir, version, $"operations.{version}.tf");
            var generatedOperationOutput = OperationGenerator.GenerateTerraformOutput(document);

            await System.IO.File.WriteAllTextAsync(operationFilePath, ApiVariablesApplier.ApplyVariables(generatedOperationOutput, TerraformVarSubFile));
        }

        private List<string> GetRevisions(JObject revisionMapping)
        {
            return revisionMapping.ToObject<Dictionary<string, string[]>>().Values.SelectMany(i => i).Distinct().ToList();
        }

        public async Task GenerateWithTemplateFiles(OpenApiDocument document)
        {
            throw new NotImplementedException();
        }
    }
}