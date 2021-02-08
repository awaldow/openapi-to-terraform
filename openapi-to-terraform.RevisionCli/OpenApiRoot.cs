using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace openapi_to_terraform.RevisionCli
{
    public class OpenApiRoot
    {
        public List<OpenApiPath> Paths { get; set; }
        public OpenApiRoot()
        {
            Paths = new List<OpenApiPath>();
        }

        public static OpenApiRoot Read(JObject docRoot)
        {
            var ret = new OpenApiRoot();
            var paths = docRoot.SelectToken("paths");
            foreach (var path in paths)
            {
                ret.Paths.Add(OpenApiPath.Read(path));
            }
            return ret;
        }
    }

    public class OpenApiPath
    {
        public string Key { get; set; }
        public List<OpenApiOperation> Operations { get; set; }
        public OpenApiPath()
        {
            Operations = new List<OpenApiOperation>();
        }

        public static OpenApiPath Read(JToken path)
        {
            var ret = new OpenApiPath();
            ret.Key = path.ToObject<JProperty>().Name;
            foreach (var operationList in path.ToObject<JProperty>().Children())
            {
                foreach (var operation in operationList.Children())
                {
                    ret.Operations.Add(OpenApiOperation.Read(operation));
                }
            }
            return ret;
        }
    }

    public class OpenApiOperation
    {
        public string Key { get; set; }
        public string OperationId { get; set; }

        public OpenApiOperation()
        {

        }

        public static OpenApiOperation Read(JToken operation)
        {
            var ret = new OpenApiOperation();
            ret.Key = operation.ToObject<JProperty>().Name;
            ret.OperationId = operation.Children().Select(m => (string)m.SelectToken("operationId")).Single();//["operationId"];
            //ret.OperationId = operationId[0].Children()["operationId"].Value<string>();
            return ret;
        }
    }
}