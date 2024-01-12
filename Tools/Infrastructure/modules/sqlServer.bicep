@minLength(1)
@maxLength(5)
@description('An identifier for the project being deployed')
param projectCode string

@minLength(1)
@maxLength(5)
@description('An identifier for the environemnt being deployed, e.g. TEST/QA/PROD')
param environment string

@description('The location for the resources to be created')
param location string

@allowed([ 'nonprod', 'prod' ])
@description('The compute required for this environment')
param resourceSizing string

var skuDetails = {
  nonprod: {
    name: 'Basic'
    tier: 'Basic'
  }
  prod: {
    name: 'Standard'
    tier: 'S0'
  }
}

@description('The ID of the Microsoft Entra ID Tenant where the admin group resides')
param tenantId string
@description('The name of the Microsoft Entra ID group to use for admin access to the SQL server')
param sqlAdminGroupName string
@description('The object ID of the Microsoft Entra ID group to use for admin access to the SQL server')
param sqlAdminGroupObjectId string


resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: toLower('${projectCode}-SQL01-${environment}')
  location: location
  properties: {
    administrators: {
      azureADOnlyAuthentication: true
      administratorType: 'ActiveDirectory'
      principalType: 'Group'
      login: sqlAdminGroupName
      sid: sqlAdminGroupObjectId
      tenantId: tenantId
    }
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: '${projectCode}-SQLDB01-${environment}'
  location: location
  sku: {
    name: skuDetails[resourceSizing].name
    tier: skuDetails[resourceSizing].tier
  }
  properties: {
    autoPauseDelay: -1 // Disabled
  }
}

resource allowGhystonIps 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  name: 'Ghyston IP Range'
  parent: sqlServer
  properties: {
    startIpAddress: '31.221.86.250'
    endIpAddress: '31.221.86.254'
  }
}

resource allowAllAzureIps 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  name: 'AllowAllWindowsAzureIps'
  parent: sqlServer
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}


output sqlDatabaseName string = sqlDatabase.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
