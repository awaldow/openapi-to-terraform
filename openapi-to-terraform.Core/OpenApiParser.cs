using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace openapi_to_terraform.Core
{
    public class OpenApiParser
    {
        private string OpenApiFilePath { get; set; }
        public OpenApiDocument Document { get; set; }

        public OpenApiParser(string openApiPath)
        {
            OpenApiFilePath = openApiPath;
        }

        public void Parse()
        {
            using FileStream fs = File.OpenRead(OpenApiFilePath);
            Document = new OpenApiStreamReader().Read(fs, out var diagnostic);
        }
    }
}