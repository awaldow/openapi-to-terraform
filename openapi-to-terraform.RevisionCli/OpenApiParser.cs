using System.IO;
using Newtonsoft.Json.Linq;

namespace openapi_to_terraform.RevisionCli
{
    public class OpenApiParser
    {
        private readonly string _fileText;
        public OpenApiParser(string fileText)
        {
            _fileText = fileText;
        }

        public OpenApiRoot Read()
        {
            JObject root = JObject.Parse(_fileText);
            return OpenApiRoot.Read(root);
        }
    }
}