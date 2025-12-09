environment                     = "dv1"
action_group_name               = "Strikers-ExitPathwayPlugin-NonProd"
action_group_resource_group     = "dv1_rsg_eu2_ep_pathway"
action_group_subscription       = "ed7f55f8-8333-41d1-b014-dcbf053db018"
resource_group_name             = "dv1_rsg_eu2_ep_pathway"
subscription_id                 = "ed7f55f8-8333-41d1-b014-dcbf053db018"
searchtags = {
        SystemOwner = "Strikers"
        DataClassification = "Proprietary"
        Environment = "NonProd"
  }
app_service_environment_id      = "/subscriptions/ed7f55f8-8333-41d1-b014-dcbf053db018/resourceGroups/dv1_rsg_apphub_ase/providers/Microsoft.Web/hostingEnvironments/eheu2dv1asehosted"
auto_scale_max_throughput       = 4000
instrumentation_key             = "af75d2bd-027b-4500-ae0c-a32634620888"
treatmentplanner_url            = "https://api.carecorenational.com:9134/"
app_service_plan_id             = "/subscriptions/ed7f55f8-8333-41d1-b014-dcbf053db018/resourceGroups/dv1_rsg_eu2_ep_pathway/providers/Microsoft.Web/serverfarms/dv1-asp-esipathwayplugin-api"