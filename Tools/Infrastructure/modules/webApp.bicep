@minLength(1)
@maxLength(5)
@description('An identifier for the project being deployed')
param projectCode string

@minLength(1)
@maxLength(5)
@description('An identifier for the environment being deployed, e.g. TEST/QA/PROD')
param environment string

@minLength(1)
@description('The environment for the ASP.NET Core app')
param aspNetCoreEnvironment string

@minLength(1)
@description('The name of the project configuration section in the ASP.NET Core appsettings.json file')
param aspNetCoreProjectConfigurationSection string

@description('The location for the resources to be created')
param location string

@minLength(1)
@allowed([ 'nonprod', 'prod' ])
@description('The compute required for this environment')
param resourceSizing string

@description('The Log Analytics workspace ID')
param logAnalyticsWorkspaceId string

@description('The Applicaiton Insights connection string')
param appInsightsConnectionString string

@description('The fully qualified domain name (FQDN) of the SQL server')
param dbFqdn string
@description('The name of the database on the SQL server')
param dbName string

@description('The URI of the Azure Key Vault')
param keyVaultUri string

var skuDetails = {
  nonprod: {
    name: 'F1'
    tier: 'Free'
    size: 'F1'
    family: 'F'
  }
  prod: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
  }
}

resource appServiceServerFarm 'Microsoft.Web/serverfarms@2025-03-01' = {
  name: '${projectCode}-ASP01-${environment}'
  location: location
  sku: {
    name: skuDetails[resourceSizing].name
    tier: skuDetails[resourceSizing].tier
    size: skuDetails[resourceSizing].size
    family: skuDetails[resourceSizing].family
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

var webAppName = '${projectCode}-WA01-${environment}'
resource webApp 'Microsoft.Web/sites@2025-03-01' = {
  name: webAppName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    serverFarmId: appServiceServerFarm.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      appSettings: [
        {
          name: '${aspNetCoreProjectConfigurationSection}__ConnectionString'
          value: 'Data Source=${dbFqdn}; Initial Catalog=${dbName}; Encrypt=True;Authentication="Active Directory Default";'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~2'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: aspNetCoreEnvironment
        }
        {
          name: 'KEYVAULT_URI'
          value: keyVaultUri
        }
      ]
    }
  }
}

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: '${webAppName}-DIAGNOSTICSETTINGS'
  scope: webApp
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        category: 'AppServiceHTTPLogs'
        enabled: true
      }
      {
        category: 'AppServiceConsoleLogs'
        enabled: true
      }
      {
        category: 'AppServiceAppLogs'
        enabled: true
      }
      {
        category: 'AppServiceAuditLogs'
        enabled: true
      }
      {
        category: 'AppServiceIPSecAuditLogs'
        enabled: true
      }
      {
        category: 'AppServicePlatformLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

output managedIdentityId string = webApp.identity.principalId