terraform {
  required_version = "> 0.13.0"

  backend "azurerm" {
  }

  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "~> 2.45.1"
    }
  }
}

provider "azurerm" {  
  features {
    key_vault {
      purge_soft_delete_on_destroy = true
    }
  }
}

data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "apitest" {
  name     = var.resource_group_name
  location = var.resource_group_location
}

resource "azurerm_application_insights" "apiappi" {
  name                = var.application_insights_name
  location            = azurerm_resource_group.apitest.location
  resource_group_name = azurerm_resource_group.apitest.name
  application_type    = "web"

  tags = {
    environment = "test"
  }
}

resource "azurerm_app_service_plan" "apiplan" {
  name                = var.app_service_plan_name
  location            = azurerm_resource_group.apitest.location
  resource_group_name = azurerm_resource_group.apitest.name

  sku {
    tier = "Basic"
    size = "B1"
  }

  tags = {
    environment = "test"
  }
}

resource "azurerm_app_service" "apiuserstest" {
  name                = var.users_app_service_name
  location            = azurerm_resource_group.apitest.location
  resource_group_name = azurerm_resource_group.apitest.name
  app_service_plan_id = azurerm_app_service_plan.apiplan.id
  https_only = true

  site_config {
    dotnet_framework_version = "v5.0"
    windows_fx_version = "DOTNET|5.0"
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"             = "Staging"
    "ASPNETCORE_HTTPS_PORT"              = 443
    "WEBSITE_HTTPLOGGING_RETENTION_DAYS" = 1
    "WEBSITE_RUN_FROM_PACKAGE"           = 1
  }

  identity {
    type = "SystemAssigned"
  }

  tags = {
    environment = "test"
  }
}

resource "azurerm_storage_account" "apisqlstorage" {
  name = var.sqlstorage_account_name
  resource_group_name      = azurerm_resource_group.apitest.name
  location                 = azurerm_resource_group.apitest.location
  account_tier             = "Standard"
  account_replication_type = "RAGRS"

  tags = {
    environment = "test"
  }
}

resource "azurerm_sql_server" "apisqlserver" {
  name                         = var.sql_server_name
  resource_group_name          = azurerm_resource_group.apitest.name
  location                     = azurerm_resource_group.apitest.location
  version                      = "12.0"
  administrator_login          = var.sql_server_admin
  administrator_login_password = var.sql_server_admin_pass

  tags = {
    environment = "test"
  }
}

resource "azurerm_storage_account" "apistorage" {
  name = var.storage_account_name
  resource_group_name      = azurerm_resource_group.apitest.name
  location                 = azurerm_resource_group.apitest.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = {
    environment = "test"
  }
}

resource "azurerm_storage_table" "apitaskstable" {
  name = var.tasks_table_name
  storage_account_name = azurerm_storage_account.apistorage.name
}

resource "azurerm_storage_table" "apipurchasestable" {
  name = var.purchases_table_name
  storage_account_name = azurerm_storage_account.apistorage.name
}

resource "azurerm_storage_container" "apiicons" {
  name                  = "icons"
  storage_account_name  = azurerm_storage_account.apistorage.name
  container_access_type = "private"
}

output "resource_group" {
  value = var.resource_group_name
}
