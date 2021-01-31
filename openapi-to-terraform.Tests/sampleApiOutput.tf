resource "azurerm_api_management_api" "usersapiv1" {
  name                = var.users_api_name
  api_management_name = azurerm_api_management.roombyapim.name
  resource_group_name = azurerm_api_management.roombyapim.resource_group_name
  display_name        = "Users API"
  revision            = "1"
  version             = "v1"
  version_set_id      = azurerm_api_management_version_set.usersversionset.id
  path                = "users"
  protocols           = ["https"]
  service_url         = "${data.azurerm_app_service.roombyusersapp.default_site_hostname}/api/"
}

resource "azurerm_api_management_product_api" "usersproductapi" {
  api_name            = azurerm_api_management_api.usersapiv1.name
  product_id          = azurerm_api_management_product.roombyproduct.product_id
  api_management_name = azurerm_api_management.roombyapim.name
  resource_group_name = azurerm_api_management.roombyapim.resource_group_name
}

resource "azurerm_api_management_api" "roomsapiv1" {
  name                = var.rooms_api_name
  api_management_name = azurerm_api_management.roombyapim.name
  resource_group_name = azurerm_api_management.roombyapim.resource_group_name
  revision            = "1"
  display_name        = "Rooms API"
  revision            = "1"
  version             = "v1"
  version_set_id      = azurerm_api_management_version_set.usersversionset.id
  path                = "rooms"
  protocols           = ["https"]
  service_url         = "${data.azurerm_app_service.roombyroomsapp.default_site_hostname}/api/"
}

resource "azurerm_api_management_product_api" "roomsproductapi" {
  api_name            = azurerm_api_management_api.roomsapiv`.name
  product_id          = azurerm_api_management_product.roombyproduct.product_id
  api_management_name = azurerm_api_management.roombyapim.name
  resource_group_name = azurerm_api_management.roombyapim.resource_group_name
}