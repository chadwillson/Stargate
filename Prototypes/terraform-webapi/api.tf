resource "azurerm_app_service" "exit_pathway_api" {
  name                = "${var.environment}-${var.name}-api"
  location            = data.azurerm_resource_group.resource_group.location
  resource_group_name = data.azurerm_resource_group.resource_group.name
  app_service_plan_id = var.app_service_plan_id
  https_only = true

  site_config {
	app_command_line		 = ""
	linux_fx_version		 = "DOCKER|dv1crep.azurecr.io/evicorexitpathwaypluginapi:${var.container_image_version}"
	always_on				 = true
	health_check_path        = "/health"
	remote_debugging_enabled = false
  }

  app_settings = {
	"WEBSITES_ENABLE_APP_SERVICE_STORAGE"                           = "false"
	"DOCKER_REGISTRY_SERVER_URL"                                    = "https://dv1crep.azurecr.io"
	"DOCKER_REGISTRY_SERVER_USERNAME"                               = var.container_registry_username
	"DOCKER_REGISTRY_SERVER_PASSWORD"                               = var.container_registry_password
	#"CosmosDbOptions__Endpoint"                                    = module.cosmosdb-account.endpoint
	#"CosmosDbOptions__Key"                                         = module.cosmosdb-account.secondary_master_key
	#"CosmosDbOptions__DatabaseName"                                = module.cosmosdb-sql-database.name
	#"CosmosDbOptions__ContainerNames__NonClinicalEventData"        = var.containernameeventdata
	#"CosmosDbOptions__ContainerNames__NonClinicalWorklistData"     = var.containername
	"ApplicationInsights__InstrumentationKey"						= var.instrumentation_key	
	"TreatmentPlannerBaseUrl"										= var.treatmentplanner_url
  }

  lifecycle {
	ignore_changes = [
	  tags,
	]
  }
}

output "api_url" {
  value = azurerm_app_service.exit_pathway_api.default_site_hostname
}