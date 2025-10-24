using 'QQProjectName.bicep'

// Non Secure Parameters
param environment = 'EXMPL'
param aspNetCoreEnvironment = 'Example'
param sqlAdminGroupName = 'RBAC-SQL-PRJCT-EXMPL'
param sqlAdminGroupObjectId = '34946780-7ccc-4735-99f8-8f047cf118a9'
param keyVaultAdminGroupObjectId = '34946780-7ccc-4735-99f8-8f047cf118a9'
param resourceSizing = 'nonprod'

// Secure Parameters
param secretMessagePassword = readEnvironmentVariable('EXAMPLE_SECRET_MESSAGE_PASSWORD')
