using System;
using CommandLine;
using openapi_to_terraform.Core;
using openapi_to_terraform.Main;
using openapi_to_terraform.Generator;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Collections.Generic;
using Autofac.Core;
using Autofac.Core.Registration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace openapi_to_terraform
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<GenerateTerraformOptions, GenerateRevisionsOptions>(args)
                   .WithParsed<GenerateTerraformOptions>(o =>
                   {
                       RegisterServices();
                       var logger = _serviceProvider.GetAutofacRoot().Resolve<ILogger<Program>>();
                       Console.WriteLine($"Parsing {o.InputFile}, outputting to {o.OutputDirectory}");

                       var serviceName = o.Provider + "_" + o.ProviderVersion;

                       OpenApiParser p = new OpenApiParser(o.InputFile);
                       p.Parse();

                       if (o.TerraformVariablesFile != null)
                       {
                           var parameters = new List<Parameter>();
                           parameters.Add(new NamedParameter("outputDir", o.OutputDirectory));
                           parameters.Add(new NamedParameter("terraformVariablesFile", o.TerraformVariablesFile));
                           parameters.Add(new NamedParameter("revisionMap", o.RevisionFile));
                           parameters.Add(new NamedParameter("policiesPath", o.PoliciesDirectoryPath));

                           try
                           {
                               var generator = _serviceProvider.GetAutofacRoot().ResolveNamed<ITerraformGenerator>(serviceName, parameters);
                               generator.GenerateWithTerraformVars(p.Document);
                           }
                           catch (ComponentNotRegisteredException e)
                           {
                               logger.LogError(e.Message, e);
                               Console.WriteLine($"Service with provider version {serviceName} not found");
                           }
                       }
                       else if (o.ApiTemplateFile != null && o.OperationTemplateFile != null)
                       {
                           var parameters = new List<Parameter>();
                           parameters.Add(new NamedParameter("outputDir", o.OutputDirectory));
                           parameters.Add(new NamedParameter("apiTemplatePath", o.ApiTemplateFile));
                           parameters.Add(new NamedParameter("operationTemplatePath", o.OperationTemplateFile));
                           parameters.Add(new NamedParameter("revisionMap", o.RevisionFile));
                           parameters.Add(new NamedParameter("policiesPath", o.PoliciesDirectoryPath));

                           try
                           {
                               var generator = _serviceProvider.GetAutofacRoot().ResolveNamed<ITerraformGenerator>(serviceName, parameters);
                               generator.GenerateWithTemplateFiles(p.Document);
                           }
                           catch (ComponentNotRegisteredException e)
                           {
                               logger.LogError(e.Message, e);
                               Console.WriteLine($"Service with provider version {serviceName} not found");
                           }
                       }
                       DisposeServices();
                   })
                   .WithParsed<GenerateRevisionsOptions>(o =>
                   {
                       RegisterServices();
                       var logger = _serviceProvider.GetAutofacRoot().Resolve<ILogger<Program>>();
                       Console.WriteLine($"Parsing {o.InputFile} and {o.InputAssemblyPath}, outputting to {o.OutputDirectory}");

                       var revisions = RevisionGenerator.GenerateRevisionsBlock(o.InputAssemblyPath, o.InputFile, o.RoutePrefix);

                       DisposeServices();
                   });
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection().AddLogging();
            var builder = new ContainerBuilder();

            builder = TerraformGeneratorRegistry.RegisterGeneratorTypes(builder);

            builder.Populate(services);

            var appContainer = builder.Build();
            _serviceProvider = new AutofacServiceProvider(appContainer);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
