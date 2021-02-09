variable "resource_group_name" {
  default = "rg-api-test"
  description = "The name of the resource group"
}

variable "resource_group_location" {
  description = "The location of the resource group (West US, Central US, etc.)"
}

variable "storage_account_name" {
  default = "stapitest"
  description = "The name of the storage account for api"
}

variable "tasks_table_name" {
  default = "apitasks"
}

variable "purchases_table_name" {
  default = "apipurchases"
}

variable "apim_service_name" {
  default = "api-api-test"
}

variable "users_api_name" {
  default = "users"
}

variable "rooms_api_name" {
  default = "rooms"
}

variable "api_product_id" {
    default = "api"
}

variable "app_service_plan_name" {
  default = "asp-api-test"
  description = "The name of the app service plan"
}

variable "rooms_app_service_name" {
  default = "app-api-rooms-test"
  description = "The Room app service prefix"
}

variable "users_app_service_name" {
  default = "app-api-users-test"
  description = "The Users app service prefix"
}

variable "users_app_service_resource_group" {
  default = "rg-api-users-test"
  description = "The name of the resource group"
}

variable "rooms_app_service_resource_group" {
  default = "rg-api-rooms-test"
  description = "The name of the resource group"
}

variable "application_insights_name" {
  default = "appi-api-rooms-test"
  description = "The name of the application insights service for api"
}

variable "sql_server_admin" {
  description = "The name of the SQL server admin account"
}

variable "sql_server_admin_pass" {
  description = "The name of the SQL server admin account password"
}

variable "sql_server_name" {
  default = "sqlserver-api-test"
  description = "The name of the Azure SQL Server instance for api"
}

variable "sqlstorage_account_name" {
  default = "stapisqltest"
  description = "The name of the storage account for the Azure SQL Server"
}