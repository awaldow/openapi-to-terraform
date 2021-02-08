using System.Linq;
using System.Reflection;
using Autofac;
using openapi_to_terraform.Core.Extensions.Attributes;

namespace openapi_to_terraform.Generator
{
    public static class TerraformGeneratorRegistry
    {
        // To register a new provider implementation, do the following:
        // 1. Create a folder structure with the provider name, then the version you want to add support for
        // 2. There must be a class that inherits from TerraformGenerator and implements ITerraformGenerator in that folder structure
        // 3. That class must also have a ProviderVersion attribute with the {provider}_{providerVersion} string to register here
        public static ContainerBuilder RegisterGeneratorTypes(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(TerraformGenerator).Assembly)
                .Where(t => t.IsSubclassOf(typeof(TerraformGenerator)) && t.GetInterfaces().Contains(typeof(ITerraformGenerator)))
                .Named<ITerraformGenerator>(t => t.GetCustomAttribute<ProviderVersionAttribute>()?.ProviderVersion);
            return builder;
        }
    }
}