@description('The Azure tenant where the requested resource group resides')
param tenantId string = subscription().tenantId
@description('The Azure location for all resources to be created')
param location string = resourceGroup().location

@minLength(1)
@maxLength(5)
@description('An identifier for the project being deployed, used as a prefix for all deployed resources')
param projectCode string = 'PRJCT'

@minLength(1)
@description('The name of the project configuration section in the ASP.NET Core appsettings.json file')
param aspNetCoreProjectConfigurationSection string = 'QQProjectName'

@minLength(1)
@maxLength(5)
@description('A suffix used in resource naming to denote the environment - this should be a short abbreviation (such as `DEV`) to avoid name length restrictions on e.g. the Azure Key Vault')
param environment string

@description('Used to set the ASPNETCORE_ENVIRONMENT environment variable on the Web App, which determines the appsettings.*.json file to use')
param aspNetCoreEnvironment string

@description('The name of the Microsoft Entra ID group to use for admin access to the SQL server')
param sqlAdminGroupName string
@description('The object ID of the Microsoft Entra ID group to use for admin access to the SQL server')
param sqlAdminGroupObjectId string

@description('The object ID of the Microsoft Entra ID group to use for admin access to the Key Vault')
param keyVaultAdminGroupObjectId string
@secure()
@description('The value of the secret message password to be stored in the key vault')
param secretMessagePassword string

@allowed([ 'nonprod', 'prod' ])
@description('The compute level required for the deployed resources')
param resourceSizing string

module sqlServer 'modules/sqlServer.bicep' = {
  name: 'sqlServer'
  params: {
    projectCode: projectCode
    environment: environment
    location: location
    resourceSizing: resourceSizing
    sqlAdminGroupName: sqlAdminGroupName
    sqlAdminGroupObjectId: sqlAdminGroupObjectId
    tenantId: tenantId
  }
}

module logAnalyticsWorkspace 'modules/logAnalytics.bicep' = {
  name: 'logAnalyticsWorkspace'
  params: {
    projectCode: projectCode
    environment: environment
    location: location
  }
}

module appInsights 'modules/appInsights.bicep' = {
  name: 'appInsights'
  params: {
    projectCode: projectCode
    environment: environment
    location: location
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.logAnalyticsWorkspaceId
  }
}

// This avoids a circular dependency. Make sure to keep this in sync with the value in the keyVault.bicep file.
var keyVaultUri = 'https://${toLower(projectCode)}-kv-${toLower(environment)}.vault.azure.net/'

module webApp 'modules/webApp.bicep' = {
  name: 'webApp'
  params: {
    projectCode: projectCode
    environment: environment
    location: location
    resourceSizing: resourceSizing
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.logAnalyticsWorkspaceId
    appInsightsConnectionString: appInsights.outputs.appInsightsConnectionString
    aspNetCoreEnvironment: aspNetCoreEnvironment
    aspNetCoreProjectConfigurationSection: aspNetCoreProjectConfigurationSection
    dbFqdn: sqlServer.outputs.sqlServerFqdn
    dbName: sqlServer.outputs.sqlDatabaseName
    keyVaultUri: keyVaultUri
  }
}

module keyVault 'modules/keyVault.bicep' = {
  name: 'keyVault'
  params: {
    projectCode: projectCode
    environment: environment
    tenantId: tenantId
    location: location
    webAppManagedIdentityId: webApp.outputs.managedIdentityId
    keyVaultAdminGroupObjectId: keyVaultAdminGroupObjectId
    secretMessagePassword: secretMessagePassword
  }
}
