provider "azurerm" {
    features {}
    subscription_id = var.subscription_id
    skip_provider_registration = true
}

terraform {
    required_providers {
        azurerm = {
          source  = "hashicorp/azurerm"
          version = "<=2.85"
        }
      }

    backend "azurerm"{
       #resource_group_name   = "dv1_rsg_eu2_ep_pathway"
       #storage_account_name  = "dv1eppathway"
       #container_name        = "terraform"
       #key                   = "exitpathwayplugin.tfstate"
       #access_key = ""
    }
}

provider "azurerm" {
  features {}
  alias           = "action_group_provider"
  subscription_id = var.action_group_subscription
}

data "azurerm_resource_group" "resource_group" {
    name = var.resource_group_name
}