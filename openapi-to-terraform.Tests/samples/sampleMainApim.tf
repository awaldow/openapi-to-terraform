resource "azurerm_api_management" "roombyapim" {
  name                = var.apim_service_name
  location            = azurerm_resource_group.roombytest.location
  resource_group_name = azurerm_resource_group.roombytest.name
  publisher_name      = "Roomby"
  publisher_email     = "a.wal.bear@gmail.com"

  sku_name = "Consumption_1"

  identity {
    type = "SystemAssigned"
  }

  tags = {
    environment = "test"
  }
}

resource "azurerm_api_management_product" "roombyproduct" {
  product_id            = var.roomby_product_id
  api_management_name   = azurerm_api_management.roombyapim.name
  resource_group_name   = azurerm_api_management.roombyapim.resource_group_name
  display_name          = "Roomby APIs"
  subscription_required = true
  published             = true
}

resource "azurerm_api_management_api_version_set" "roomsversionset" {
  name                = "rooms"
  api_management_name = azurerm_api_management.roombyapim.name
  resource_group_name = azurerm_api_management.roombyapim.resource_group_name
  display_name        = "Roomby Rooms API"
  versioning_scheme   = "Segment"
}

resource "azurerm_api_management_api_version_set" "usersversionset" {
  name                = "users"
  api_management_name = azurerm_api_management.roombyapim.name
  resource_group_name = azurerm_api_management.roombyapim.resource_group_name
  display_name        = "Roomby Users API"
  versioning_scheme   = "Segment"
}