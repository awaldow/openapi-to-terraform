resource "azurerm_api_management" "apiapim" {
  name                = var.apim_service_name
  location            = azurerm_resource_group.apitest.location
  resource_group_name = azurerm_resource_group.apitest.name
  publisher_name      = "api"
  publisher_email     = "a.wal.bear@gmail.com"

  sku_name = "Consumption_1"

  identity {
    type = "SystemAssigned"
  }

  tags = {
    environment = "test"
  }
}

resource "azurerm_api_management_product" "apiproduct" {
  product_id            = var.api_product_id
  api_management_name   = azurerm_api_management.apiapim.name
  resource_group_name   = azurerm_api_management.apiapim.resource_group_name
  display_name          = "api APIs"
  subscription_required = true
  published             = true
}

resource "azurerm_api_management_api_version_set" "roomsversionset" {
  name                = "rooms"
  api_management_name = azurerm_api_management.apiapim.name
  resource_group_name = azurerm_api_management.apiapim.resource_group_name
  display_name        = "api Rooms API"
  versioning_scheme   = "Segment"
}

resource "azurerm_api_management_api_version_set" "usersversionset" {
  name                = "users"
  api_management_name = azurerm_api_management.apiapim.name
  resource_group_name = azurerm_api_management.apiapim.resource_group_name
  display_name        = "api Users API"
  versioning_scheme   = "Segment"
}