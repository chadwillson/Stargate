variable "subscription_id" {
  default = "846f0f3b-4602-4fea-91df-2abdf8db7f87"
}

variable "environment" {
}

variable "location" {
  default = "eastus2"
}

variable "resource_group_name" {
}

variable "action_group_name" {
	default = "Strikers-Pathway-Dev"
}

variable "action_group_resource_group" {
	default = "dv1_rsg_ep_pathway"
}

variable "action_group_subscription" {
	default = "5fab10df-31da-4b2f-a1a8-4e2f514b07f0"
}

variable "domain" {
  default = "eppathway"
}

variable "name" {
  default = "exitpathwayplugin"
}

variable "app_service_plan_id" {
  default = "dv1-asp-esipathwayplugin-api"
}

variable "app_service_environment_id" {
}

variable "searchtags" {
}

variable "databasename" {
  default = "pathway"
}

variable "containername" {
  default = "exitpathwaydata"
}

variable "instrumentation_key" {
  default = "af75d2bd-027b-4500-ae0c-a32634620888"
}

variable "treatmentplanner_url" {
  default = "https://dev-api.carecorenational.com:9134/"
}

variable "auto_scale_max_throughput" {
  type = number
  default = 4000
  description = "The maximum throughput for autoscaling. Must be increments of 1000."
}

variable "container_registry_username" {
  default = "dv1crep"
}

variable "container_registry_password" {
  default = ""
}

variable "container_image_version" {
  default = "latest"
}