# GitHub Actions + Terraform for Azure App Service and SQL

This guide outlines how to create and deploy the Stargate application components (App Service and Azure SQL Database) with GitHub Actions and Terraform. It also notes what exists in `Prototypes` for reference and how to modernize it.

## What We Already Have (Prototypes)
- Azure DevOps pipeline example: `Prototypes/azure-pipelines.yml` uses Terraform 1.0.9 and AzureRM provider `<=2.85` with service principal secrets and SSH keys.
- Terraform for the API: `Prototypes/terraform-webapi/*.tf` builds an `azurerm_app_service` running a container image from ACR; backend is commented-out storage.
- Terraform for the UI: `Prototypes/terraform-ui/*.tf` provisions storage/frontdoor/Application Insights with AzureRM provider v2.
- Gaps: no GitHub Actions, no OIDC auth, old provider version (v2), no Azure SQL example, commented/empty state backend, and container settings hard-coded in variables.

## Target Approach
- Use Terraform `~> 1.8` (or latest) with AzureRM provider `~> 3.100`+ and AzureAD provider `~> 3.x` if you need AAD admins on SQL.
- Store state in an Azure Storage account with Azure AD auth (`use_azuread_auth = true`) to avoid access keys.
- Use GitHub Actions with OpenID Connect (OIDC) to authenticate to Azure — no long-lived secrets.
- Separate workflows: CI for app build/test/package, and CD for Terraform plan/apply (per environment) plus optional app deploy.
- Keep environment-specific values in `environments/<env>.tfvars` and use GitHub Environment protection for apply.

## Suggested Repo Layout
```
infra/
  main.tf
  providers.tf
  variables.tf
  outputs.tf
  appservice.tf
  sql.tf
  backend.hcl                  # backend settings (no secrets)
  environments/
    dev.tfvars
    qa.tfvars
    prod.tfvars
.github/workflows/
  ci.yml                       # build/test, build & push image
  db-ci.yml                    # build/upload DACPAC for SQL project
  infra-plan-apply.yml         # terraform plan/apply
```

## Terraform Skeleton (App Service + Azure SQL)
Use modules if the footprint grows, but a single stack works for now.

```hcl
terraform {
  required_version = ">= 1.8.0"
  backend "azurerm" {}
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.110"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
  tenant_id       = var.tenant_id
  use_oidc        = true        # works with GitHub OIDC
}

resource "azurerm_resource_group" "rg" {
  name     = "${var.prefix}-rg-${var.location_short}"
  location = var.location
  tags     = var.tags
}

resource "azurerm_service_plan" "plan" {
  name                = "${var.prefix}-asp"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "P1v3"
  tags                = var.tags
}

resource "azurerm_linux_web_app" "app" {
  name                = "${var.prefix}-app"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.plan.id
  https_only          = true

  site_config {
    application_stack {
      docker_image_name   = "${var.acr_login_server}/${var.image_name}:${var.image_tag}"
      docker_registry_url = "https://${var.acr_login_server}"
    }
    health_check_path = "/health"
  }

  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "DOCKER_REGISTRY_SERVER_URL"          = "https://${var.acr_login_server}"
    "DOCKER_REGISTRY_SERVER_USERNAME"     = var.acr_username
    "DOCKER_REGISTRY_SERVER_PASSWORD"     = var.acr_password
    "ConnectionStrings__Default"          = azurerm_mssql_server.sql.fully_qualified_domain_name
  }

  connection_string {
    name  = "DefaultConnection"
    type  = "SQLAzure"
    value = azurerm_mssql_server.sql.fully_qualified_domain_name
  }

  identity {
    type = "SystemAssigned"
  }

  tags = var.tags
}

resource "azurerm_mssql_server" "sql" {
  name                         = "${var.prefix}-sql"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_login
  administrator_login_password = var.sql_admin_password
  minimum_tls_version          = "1.2"
  identity {
    type = "SystemAssigned"
  }
  tags = var.tags
}

resource "azurerm_mssql_database" "db" {
  name           = "${var.prefix}-db"
  server_id      = azurerm_mssql_server.sql.id
  sku_name       = "GP_S_Gen5_2"   # adjust to workload
  zone_redundant = false
  tags           = var.tags
}

# (Optional) grant the web app identity access to SQL via AAD admin if configured.
```

`backend.hcl` example:
```hcl
resource_group_name  = "<tfstate-rg>"
storage_account_name = "<tfstate-sa>"
container_name       = "tfstate"
key                  = "stargate-app.tfstate"
use_azuread_auth     = true
```

`environments/dev.tfvars` example:
```hcl
prefix              = "sg-dev"
location            = "eastus2"
location_short      = "eus2"
subscription_id     = "00000000-0000-0000-0000-000000000000"
tenant_id           = "00000000-0000-0000-0000-000000000000"
acr_login_server    = "myregistry.azurecr.io"
acr_username        = "myregistry"
acr_password        = "use-GitHub-secret"
image_name          = "stargate-api"
image_tag           = "latest"
sql_admin_login     = "sqladmin"
sql_admin_password  = "use-GitHub-secret"
tags = {
  environment = "dev"
  owner       = "stargate"
}
```

## GitHub Actions: Terraform Plan/Apply (per environment)
```yaml
# .github/workflows/infra-plan-apply.yml
name: Infra Plan & Apply

on:
  workflow_dispatch:
    inputs:
      env:
        description: "Environment (dev/qa/prod)"
        required: true
        default: dev
  pull_request:
    paths: ["infra/**", ".github/workflows/infra-plan-apply.yml"]

permissions:
  id-token: write     # for OIDC
  contents: read

env:
  TF_VERSION: 1.8.5
  WORKING_DIR: infra

jobs:
  plan:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      - uses: hashicorp/setup-terraform@v3
        with:
          terraform_version: ${{ env.TF_VERSION }}
      - name: Terraform Init
        run: terraform -chdir=${{ env.WORKING_DIR }} init -backend-config=backend.hcl
      - name: Terraform Validate
        run: terraform -chdir=${{ env.WORKING_DIR }} validate
      - name: Terraform Plan
        run: terraform -chdir=${{ env.WORKING_DIR }} plan -var-file=environments/${{ github.event.inputs.env || 'dev' }}.tfvars -out=tfplan
      - uses: actions/upload-artifact@v4
        with:
          name: tfplan
          path: infra/tfplan

  apply:
    needs: plan
    if: github.event_name == 'workflow_dispatch'
    runs-on: ubuntu-latest
    environment: ${{ github.event.inputs.env }}
    steps:
      - uses: actions/checkout@v4
      - uses: actions/download-artifact@v4
        with:
          name: tfplan
          path: infra
      - uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      - uses: hashicorp/setup-terraform@v3
        with:
          terraform_version: ${{ env.TF_VERSION }}
      - name: Terraform Init
        run: terraform -chdir=${{ env.WORKING_DIR }} init -backend-config=backend.hcl
      - name: Terraform Apply
        run: terraform -chdir=${{ env.WORKING_DIR }} apply tfplan
```
Notes:
- Use GitHub Environments (`dev`, `qa`, `prod`) to gate `apply` with approvals.
- Store `AZURE_*` IDs as repo or org secrets; use Environment secrets for per-env ACR/SQL passwords.
- If you need Key Vault secrets, add `azure/cli@v2` with `az keyvault secret show` or a Terraform `azurerm_key_vault_secret` data source instead of plaintext secrets.

## GitHub Actions: Build/Publish App (Docker)
```yaml
# .github/workflows/ci.yml
name: Build & Push App Image

on:
  push:
    branches: [main]
    paths: ["src/**", "Dockerfile", ".github/workflows/ci.yml"]
  pull_request:
    paths: ["src/**", "Dockerfile", ".github/workflows/ci.yml"]

env:
  IMAGE_NAME: stargate-api

permissions:
  id-token: write
  contents: read

jobs:
  build-test-push:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "8.0.x"
      - run: dotnet restore
      - run: dotnet test --configuration Release
      - uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      - uses: azure/docker-login@v2
        with:
          login-server: ${{ secrets.ACR_LOGIN_SERVER }}
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}
      - name: Build image
        run: |
          docker build -t ${{ secrets.ACR_LOGIN_SERVER }}/${{ env.IMAGE_NAME }}:${{ github.sha }} .
      - name: Push image
        run: |
          docker push ${{ secrets.ACR_LOGIN_SERVER }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
      - name: Publish image tag for Terraform
        run: echo "IMAGE_TAG=${GITHUB_SHA}" >> $GITHUB_ENV
```
Pass `IMAGE_TAG` into the Terraform workflow (via `workflow_dispatch` input or an environment variable) so App Service pulls the correct image.

## GitHub Actions: Build & Deploy SQL DACPAC
The repo already includes `Stargate.Database/Stargate.Database.sqlproj`. Use Windows runners for SSDT/MSBuild. This sample builds the DACPAC, uploads it, and optionally deploys it with `azure/sql-action`.

```yaml
# .github/workflows/db-ci.yml
name: Build SQL DACPAC

on:
  push:
    branches: [main]
    paths:
      - "Stargate.Database/**"
      - ".github/workflows/db-ci.yml"
  pull_request:
    paths:
      - "Stargate.Database/**"
      - ".github/workflows/db-ci.yml"
  workflow_dispatch:
    inputs:
      deploy:
        description: "Deploy DACPAC after build (requires env approvals)"
        default: false
        type: boolean
      env:
        description: "Environment name (dev/qa/prod)"
        default: dev
        type: choice
        options: [dev, qa, prod]

permissions:
  id-token: write
  contents: read

env:
  SQLPROJ_PATH: Stargate.Database/Stargate.Database.sqlproj
  DACPAC_PATH: Stargate.Database/bin/Release/Stargate.Database.dacpac

jobs:
  build-dacpac:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup MSBuild
        uses: microsoft/setup-msbuild@v2
      - name: Build DACPAC
        run: msbuild ${{ env.SQLPROJ_PATH }} /p:Configuration=Release /t:Build
      - name: Upload DACPAC
        uses: actions/upload-artifact@v4
        with:
          name: stargate-dacpac
          path: ${{ env.DACPAC_PATH }}

  deploy-dacpac:
    needs: build-dacpac
    if: github.event_name == 'workflow_dispatch' && github.event.inputs.deploy == 'true'
    runs-on: windows-latest
    environment: ${{ github.event.inputs.env }}
    steps:
      - uses: actions/checkout@v4
      - uses: actions/download-artifact@v4
        with:
          name: stargate-dacpac
          path: artifacts
      - uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      - name: Deploy DACPAC to Azure SQL
        uses: azure/sql-action@v2
        with:
          # Example secret name: SQL_CONN_STR_DEV / SQL_CONN_STR_QA / SQL_CONN_STR_PROD
          connection-string: ${{ secrets[format('SQL_CONN_STR_{0}', toUpper(github.event.inputs.env || 'dev'))] }}
          path: artifacts/Stargate.Database.dacpac
          action: publish
          # Optional: arguments: "/p:BlockOnPossibleDataLoss=false"
```
Notes:
- Keep deploy off for PRs; trigger `workflow_dispatch` with `deploy=true` for controlled releases.
- If you prefer one secret, replace the expression with `secrets.AZURE_SQL_CONNECTION_STRING`.
- The DACPAC lands at `Stargate.Database/bin/Release/Stargate.Database.dacpac`; adjust `DACPAC_PATH` if you change configurations.
- To push the DACPAC to a feed instead of deploying, skip the `deploy-dacpac` job and use `upload-artifact` or a NuGet publish step.
- Tie Terraform outputs (SQL server/FQDN) to the connection string secrets so infra + data deploy stay aligned.

## Migration Steps From Prototypes
- Upgrade Terraform/AzureRM: move from provider v2 (`Prototypes/terraform-webapi/main.tf`) to `~>3.100` and switch to `azurerm_linux_web_app` with `use_oidc`.
- Add backend config: replace commented backend in `Prototypes/terraform-webapi/main.tf` with `backend.hcl` using Azure AD auth.
- Parameterize secrets: remove hard-coded instrumentation keys/ACR passwords in `variables.tf`; use GitHub Secrets or Key Vault.
- Replace Azure DevOps pipeline (`Prototypes/azure-pipelines.yml`) with the GitHub Actions samples above; map service principals to federated credentials for OIDC.
- Add SQL resources: introduce `azurerm_mssql_server` and `azurerm_mssql_database` plus firewall rules as needed (e.g., `start_ip_address`/`end_ip_address` or private endpoints).

## Verification Checklist
- `terraform fmt`, `validate`, and `plan` succeed for each `*.tfvars`.
- App Service deploys and resolves the container image tag pushed by CI.
- Web app identity (and/or SQL AAD admin) can connect to the SQL database; TLS 1.2 enforced.
- Remote state stored in Azure Storage with role-based access, not access keys.
