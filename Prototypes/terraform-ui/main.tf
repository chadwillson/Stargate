# locals
locals {
  ui_blob_storage_name = "eh${var.region}${var.environment}pcviewmicroui"
}

# this line is imported so that backend connection is extablished in the pipeline
terraform {
  backend "azurerm" {}
}

# Configure the Microsoft Azure Provider
provider "azurerm" {
  features {}
  subscription_id            = var.subscription_id
  skip_provider_registration = true
}

resource "azurerm_storage_account" "microUI_patient_centric_view_sa" {
  name                      = "eh${var.region}${var.environment}${var.name}microui"
  resource_group_name       = var.resource_group_name
  location                  = var.location
  account_tier              = "Standard"
  account_kind              = "StorageV2"
  account_replication_type  = "LRS"
  enable_https_traffic_only = true
  allow_blob_public_access  = false

  min_tls_version = "TLS1_2"

  static_website {
    index_document = "patient-centric-view.bundle.js"
  }

  network_rules {
    default_action = "Allow"
    #ip_rules                   = ["198.27.9.0/24", "199.204.159.0/24"]
    #virtual_network_subnet_ids = [azurerm_subnet.subnet.id]
  }

  tags = var.tags

  lifecycle {
    ignore_changes = [
      tags,
    ]
  }
}

resource "azurerm_application_insights" "microui-patientcentric-appinsights" {
  name                = "eh${var.region}${var.environment}-ai-${var.name}"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = var.application_type
  retention_in_days   = var.retention_in_days
  tags                = var.tags
}
resource "azurerm_frontdoor" "microUI_patient_centric_view_front_door" {
  name                                         = "eh${var.region}${var.environment}${var.name}frontdoor"
  location                                     = "Global"
  resource_group_name                          = var.resource_group_name
  enforce_backend_pools_certificate_name_check = false

  routing_rule {
    name               = "patientCentricViewRoutingRule"
    accepted_protocols = ["Http", "Https"]
    patterns_to_match  = ["/*"]
    frontend_endpoints = ["eh${var.region}${var.environment}${var.name}frontdoor"]
    forwarding_configuration {
      forwarding_protocol = "MatchRequest"
      backend_pool_name   = "PatientCentricViewStorage"
    }
  }

  backend_pool_load_balancing {
    name = "eh${var.region}${var.environment}${var.name}LoadBalancesettings"
  }

  backend_pool_health_probe {
    name = "eh${var.region}${var.environment}${var.name}HealthProbeSetting"
  }

  backend_pool {
    name = "PatientCentricViewStorage"
    backend {
      host_header = "eh${var.region}${var.environment}${var.name}microui.z20.web.core.windows.net"
      address     = "eh${var.region}${var.environment}${var.name}microui.z20.web.core.windows.net"
      http_port   = 80
      https_port  = 443
      weight      = 100
    }

    load_balancing_name = "eh${var.region}${var.environment}${var.name}LoadBalancesettings"
    health_probe_name   = "eh${var.region}${var.environment}${var.name}HealthProbeSetting"
  }

  frontend_endpoint {
    name      = "eh${var.region}${var.environment}${var.name}frontdoor"
    host_name = "eh${var.region}${var.environment}${var.name}frontdoor.azurefd.net"
  }

  # Comment out when you need to make a change to backend pool, but put back in after to prevent backends 
  # being enabled when disabled and causing issues with the yaml
  # lifecycle {
  #   ignore_changes = [
  #     backend_pool,
  #   ]
  # }
}
