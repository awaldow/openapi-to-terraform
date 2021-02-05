namespace openapi_to_terraform.Generator
{
    public abstract class TerraformGenerator
    {
        protected string OutputDir { get; set; }
        protected string TerraformVarSubFile { get; set; }
        protected string ApiTemplateFile { get; set; }
        protected string OperationTemplateFile { get; set; }
        protected string RevisionMappingFile { get; set; }
        protected string PoliciesPath { get; set; }

        public TerraformGenerator(string outputDir, string terraformVariablesFile, string revisionMap, string policiesPath)
        {
            OutputDir = outputDir.EndsWith('/') ? outputDir : outputDir + '/';
            TerraformVarSubFile = terraformVariablesFile;
            RevisionMappingFile = revisionMap;
            PoliciesPath = policiesPath;
        }

        public TerraformGenerator(string outputDir, string apiTemplatePath, string operationTemplatePath, string revisionMap, string policiesPath)
        {
            OutputDir = outputDir.EndsWith('/') ? outputDir : outputDir + '/';
            ApiTemplateFile = apiTemplatePath;
            OperationTemplateFile = operationTemplatePath;
            RevisionMappingFile = revisionMap;
            PoliciesPath = policiesPath;
        }
    }
}