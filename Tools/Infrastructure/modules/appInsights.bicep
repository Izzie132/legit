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

@description('')
param logAnalyticsWorkspaceId string


resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${projectCode}-APPINSIGHTS-${environment}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

output appInsightsConnectionString string = appInsights.properties.ConnectionString
