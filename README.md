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
* You are required to provide either a terraform variables mapping file (to the -t option) *or* an API template/Operation template file
* The terraform variables mapping file must contain the following keys:
    1. "api_management_service_name" - the string or terraform variable reference to use to populate the api_management_name part of the blocks
    2. "api_management_resource_group_name" - the string or terraform variable reference to use to populate the tresource_group_name part of the blocks
    3. "api_path" - the string or terraform variable reference to use to populate the azurerm_api_management_api.api_path field
    4. "api_backend_url" - the string or terraform variable reference to use to populate the azurerm_api_management_api.service_url
    5. "api_management_product_id" - the string or terraform variable reference to use to populate the azurerm_api_management_product_api.product_id
    6. "api_name" - the string or terraform variable reference to use to populate the azurerm_api_management_operation.api_name field