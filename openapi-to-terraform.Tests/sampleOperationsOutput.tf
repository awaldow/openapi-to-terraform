resource "azurerm_api_management_api_operation" "GetHouseholdAsync" {
  operation_id        = "gethouseholdasync"
  api_name            = azurerm_api_management_api.usersapiv1.name
  api_management_name = azurerm_api_management_api.usersapiv1.api_management_name
  resource_group_name = azurerm_api_management_api.usersapiv1.resource_group_name
  display_name        = "GetHouseholdAsync(Guid householdId)"
  method              = "GET"
  url_template        = "/api/v1/Households/{householdId}"
  description         = "Returns the Household object for householdId"

  template_parameter {
      name = "Household ID"
      required = true
      type = "uuid"
      description = "Guid of the Household to GET"
  }

  response {
    status_code = 200
  }

  response {
    status_code = 500
  }

  response {
    status_code = 401
  }

  response {
    status_code = 403
  }

  response {
    status_code = 404
  }
}