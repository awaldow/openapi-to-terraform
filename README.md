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
    openapi-to-terraform -i [path to your OpenAPI json] -o [output directory for API and Operation terraform files]
    ```