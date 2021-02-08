OpenAPI to Terraform
=========

## Getting Started
1. Install as a [global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools#install-a-global-tool)
    ```
    dotnet tool install -g --version 0.0.9 openapi-to-terraform
    ```
2. Verify that the tool was installed correctly

    ```
    openapi-to-terraform --help
    ```
3. Generate a set of terraform files from your application's OpenAPI file
    ```
    openapi-to-terraform gen-tf -f [openApiPath] -o [outputDir] -t [terraformVarsJson]
    ```
    Where 
    * [openApiPath] = path to OpenAPI file
    * [outputDir] = output directory for API and Operation terraform files
    * [terraformVarsJson] = path to terraform variables mapping file

There are some additional considerations when generating a file:

Terraform Variables File:
* You are required to provide either a terraform variables mapping file (to the -t option) *or* an API template/Operation template file
* The terraform variables mapping file must contain the following keys:
    * "api_management_service_name" - the string or terraform variable reference to use to populate the api_management_name part of the blocks
    * "api_management_resource_group_name" - the string or terraform variable reference to use to populate the tresource_group_name part of the blocks
    * "api_path" - the string or terraform variable reference to use to populate the azurerm_api_management_api.api_path field
    * "api_backend_url" - the string or terraform variable reference to use to populate the azurerm_api_management_api.service_url
    * "api_management_product_id" - the string or terraform variable reference to use to populate the azurerm_api_management_product_api.product_id
    * "api_management_version_set_id" - the string or terraform variable reference to use to populate the azurerm_api_management_api.version_set_id field
* NOTE: Because these are meant to represent fields in terraform, changing them will change the plan, which may have unintended consequences in your environment

Revision Mapping File:
* By default, APIs will be generated with revision 1 by default
* If you wish to generate multiple revisions and bind specific controller actions to different revisions, provide a revision mapping file to -r:
    ```
    {
        "/api/v1/Users/{userId}^get": ["1"],
        "/api/v1/Households/{householdId}^get": ["1", "2"]
    }
    ```
* NOTE: For now, all path^method pairs must be mapped; for example, if there was a PUT for /api/v1/Users/{userId} but /api/v1/Users/{userId}^put is not present in the 
  revision mapping, it will not be present in the output

APIM Policies (Not Implemented):
* By default, no policies will be added to Operations/APIs
* If you wish to have policies added to the generated definitions, provide a directory structure containing the policies for a specific version to --policies:
    ```
    {policiesRootFolder}
    |
    |___{revision1}
    |   |   rev1ApiPolicyFile
    |   |
    |   |___{operationId}
    |       |   rev1PolicyFile
    |
    |___{revision2}
        |   rev2ApiPolicyFile
        |
        |___{otherOperationId}
            |   rev2PolicyFile
    ```
* Policies are expected to contain the inbound, backend, outbound and onerror policies in exactly one file. The first file found by Directory.GetFiles(policyPath, "*.policy") is the one that will be used, so add multiple files at your peril.
* If you are using revisions, then you are expected to provide a subfolder structure with the policies for each revision in a folder named that rev number, as seen in the example
* If you only have one revision, just use one revision folder
* Each {policiesRootFolder} should correspond to one version
* To make policy editing easier (and until we have a real language server for APIM policy files), you should consider installing the snippets at [Azure/api-management-policy-snippets](https://github.com/Azure/api-management-policy-snippets)
* The above link also has a [set of common policy expressions](https://github.com/Azure/api-management-policy-snippets/tree/master/policy-expressions) that may help

Tool Versions:
* Currently, the tool will only generate terraform files conformant to hashicorp/azurerm version "~> 2.45.1"
* The sample files (which are used for unit tests and validation) use terraform "> 0.13.0"

Revision CLI
============
## Getting started
1. Install as a [global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools#install-a-global-tool)
    ```
    dotnet tool install -g --version 0.0.1 openapi-to-terraform-rev-cli
    ```
2. Verify that the tool was installed correctly

    ```
    openapi-to-terraform-rev-cli --help
    ```
3. Generate a set of terraform files from your application's OpenAPI file
    ```
    openapi-to-terraform-rev-cli generate -f [openApiPath] -o [outputDir] -a [inputAssemblyPath]
    ```
    Where 
    * [openApiPath] = path to OpenAPI file
    * [outputDir] = output directory for API and Operation terraform files
    * [inputAssemblyPath] = path to dll to search for RevisionAttribute-d actions and generate revisions json file

If you want to generate a file with revisions, download [openapi-to-terraform.Extensions]() and use the RevisionAttribute on your controller actions to define the revisions to include the action in via an int[]. Eventually, you will be able to define the revisions array on the controller itself if you just want to control revisions for the whole controller.