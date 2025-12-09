variable "subscription_id" {
  default = "846f0f3b-4602-4fea-91df-2abdf8db7f87" #sandbox
}

variable "name" {
  default = "pcview"
}

variable "region" {
  default = "eu2"
}

variable "location" {
  default = "East US 2"
}

variable "resource_group_name" {
}

variable "tags" {
}

variable "environment" {
}

variable "originsCORS" {
}

variable "application_type" {
  type = string
  default = "web"
}

variable "retention_in_days" {
  type = string
  default = 90
}

