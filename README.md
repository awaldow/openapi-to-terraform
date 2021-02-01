OpenAPI to Terraform
=========

# Getting Started #
1. Install as a [global tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools#install-a-global-tool)
    ```
    dotnet tool install -g --version 0.0.1 openapi-to-terraform
    ```
2. Verify that the tool was installed correctly

    ```
    openapi-to-terraform --help
    ```
3. Generate a set of terraform files from your application's OpenAPI file
    ```
    openapi-to-terraform -i [path to your OpenAPI json] -o [output directory for API and Operation terraform files] -t [path to terraform variables mapping file]
    ```

There are some additional considerations when generating a file:
Terraform Variables File:
* You are required to provide either a terraform variables mapping file (to the -t option) *or* an API template/Operation template file
* The terraform variables mapping file must contain the following keys:
    1. "api_management_service_name" - the string or terraform variable reference to use to populate the api_management_name part of the blocks
    2. "api_management_resource_group_name" - the string or terraform variable reference to use to populate the tresource_group_name part of the blocks
    3. "api_path" - the string or terraform variable reference to use to populate the azurerm_api_management_api.api_path field
    4. "api_backend_url" - the string or terraform variable reference to use to populate the azurerm_api_management_api.service_url
    5. "api_management_product_id" - the string or terraform variable reference to use to populate the azurerm_api_management_product_api.product_id
    6. "api_name" - the string or terraform variable reference to use to populate the azurerm_api_management_operation.api_name field
* NOTE: Because these are meant to represent fields in terraform, changing them will change the plan, which may have unintended consequences in your environment

Revision Mapping File:
* By default, APIs will be generated with revision 1 by default
* If you wish to generate multiple revisions and bind specific controller actions to different revisions, provide a revision mapping file to -r:
    ```
    {
        "/api/v1/Users/{userId}": ["1"],
        "/api/v1/Households/{householdId}": ["1", "2"]
    }
    ```

APIM Policies:
* By default, no policies will be added to Operations/APIs
* If you wish to have policies added to the generated definitions, provide a directory structure containing the policies to -p:
    ```
    policies
    |
    |___{operationId}
    |   |
    |   |___{revision1}
    |   |   |   rev1PolicyFile
    |   |
    |   |___{revision2}
    |       |   rev2PolicyFile
    |
    |___{otherOperationId}
        |   otherPolicyFile
    ```
* Policies are expected to contain the inbound, backend, outbound and onerror policies in one file.
* If you are using revisions, then you are expected to provide a subfolder structure with the policies for each revision in a folder named that rev number
* Otherwise, just put the policy file under the operation ID folder