using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using openapi_to_terraform.Generator.Attributes;
using openapi_to_terraform.Generator.azurerm.v2_45_1.Generators;
using openapi_to_terraform.Generator.azurerm.v2_45_1.VariablesAppliers;

namespace openapi_to_terraform.Generator.azurerm.v2_45_1
{
    [ProviderVersion("azurerm_v2.45.1")]
    public class Azurerm_v2_45_1_TerraformGenerator : TerraformGenerator, ITerraformGenerator
    {
        private readonly ILogger<Azurerm_v2_45_1_TerraformGenerator> logger;
        public Azurerm_v2_45_1_TerraformGenerator(
                ILogger<Azurerm_v2_45_1_TerraformGenerator> logger,
                string outputDir,
                string terraformVariablesFile,
                string revisionMap,
                string policiesPath
            ) : base(outputDir, terraformVariablesFile, revisionMap, policiesPath)
        {
            this.logger = logger;
        }

        public Azurerm_v2_45_1_TerraformGenerator(
                ILogger<Azurerm_v2_45_1_TerraformGenerator> logger,
                string outputDir,
                string apiTemplatePath,
                string operationTemplatePath,
                string revisionMap,
                string policiesPath
            ) : base(outputDir, apiTemplatePath, operationTemplatePath, revisionMap, policiesPath)
        {
            this.logger = logger;
        }

        public void GenerateWithTerraformVars(OpenApiDocument document)
        {
            logger.LogDebug("In GenerateWithTerraformVars");
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
            var generatedApiOutput = ApiGenerator.GenerateTerraformOutput(document, revisions, PoliciesPath);
            System.IO.File.WriteAllText(apiFilePath, ApiVariablesApplier.ApplyVariables(generatedApiOutput, TerraformVarSubFile));

            if (RevisionMappingFile != null)
            {
                var operationFilePath = Path.Combine(OutputDir, $"operations.{version}.tf");
                var generatedOperationOutput = OperationGenerator.GenerateTerraformOutput(document, RevisionMappingFile, PoliciesPath);

                System.IO.File.WriteAllText(operationFilePath, ApiVariablesApplier.ApplyVariables(generatedOperationOutput, TerraformVarSubFile));
            }
            else
            {
                var operationFilePath = Path.Combine(OutputDir, $"operations.{version}.tf");
                var backendServiceUrl = JObject.Parse(File.ReadAllText(TerraformVarSubFile)).SelectToken("api_backend_url").Value<string>();
                var generatedOperationOutput = OperationGenerator.GenerateTerraformOutput(document, backendServiceUrl, PoliciesPath);

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