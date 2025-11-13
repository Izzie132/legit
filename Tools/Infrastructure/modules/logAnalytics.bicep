@minLength(1)
@maxLength(5)
@description('An identifier for the project being deployed')
param projectCode string

@minLength(1)
@maxLength(5)
@description('An identifier for the environment being deployed, e.g. TEST/QA/PROD')
param environment string

@description('The location for the resources to be created')
param location string


resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2025-07-01' = {
  name: '${projectCode}-LOGANALYTICS-${environment}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.id
