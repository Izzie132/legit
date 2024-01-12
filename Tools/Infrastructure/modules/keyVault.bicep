@minLength(1)
@maxLength(5)
@description('An identifier for the project being deployed')
param projectCode string

@minLength(1)
@maxLength(5)
@description('An identifier for the environment being deployed, e.g. TEST/QA/PROD')
param environment string

@description('The Azure tenant where the requested resource group resides')
param tenantId string

@description('The location for the resources to be created')
param location string

@description('The object ID of the managed identity for the web application')
param webAppManagedIdentityId string

@description('The object ID of the group that will be granted admin access to the key vault')
param keyVaultAdminGroupObjectId string

@secure()
@description('The value of the secret message password to be stored in the key vault')
param secretMessagePassword string

resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: '${projectCode}-KV-${environment}'
  location: location
  properties: {
    accessPolicies: []
    enableRbacAuthorization: true
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: tenantId
  }
}

resource secret 'Microsoft.KeyVault/vaults/secrets@2023-02-01' = {
  parent: keyVault
  name: 'ProjectName--SecretMessagePassword'
  properties: {
    value: secretMessagePassword
  }
}

@description('This is the built-in Key Vault Secret User role. See https://docs.microsoft.com/azure/role-based-access-control/built-in-roles#key-vault-secrets-user')
resource keyVaultSecretUserRoleRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: '4633458b-17de-408a-b874-0445c86b69e6'
}

@description('This is the built-in Key Vault Secret Officer role. See https://docs.microsoft.com/azure/role-based-access-control/built-in-roles#key-vault-secrets-officer')
resource keyVaultSecretOfficerRoleRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
}

@description('Grant the web app managed identity with key vault secret user role permissions over the key vault. This allows reading secret contents')
resource keyVaultSecretUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  scope: keyVault
  name: guid(resourceGroup().id, webAppManagedIdentityId, keyVaultSecretUserRoleRoleDefinition.id)
  properties: {
    roleDefinitionId: keyVaultSecretUserRoleRoleDefinition.id
    principalId: webAppManagedIdentityId
    principalType: 'ServicePrincipal'
  }
}

@description('Grant the key vault admin group with key vault secret officer role permissions over the key vault. This allows managing secrets')
resource keyVaultSecretOfficerRoleAssignment 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  scope: keyVault
  name: guid(resourceGroup().id, keyVaultAdminGroupObjectId, keyVaultSecretOfficerRoleRoleDefinition.id)
  properties: {
    roleDefinitionId: keyVaultSecretOfficerRoleRoleDefinition.id
    principalId: keyVaultAdminGroupObjectId
    principalType: 'Group'
  }
}