# New Environment Setup

This document describes the process for setting up a new Azure environment

## Preparation for a new environment

Before creating any resources in Azure via Bicep, there are a few pre-requisite steps that must be completed in Azure, Microsoft Entra ID and Azure DevOps.

### Create a new Resource Group in Azure

- The Resource Group should be created in the Azure Subscription required by the project. You may have different subscriptions for Production and Non-Production environments).
- The Resource Group should be named according to the environment it represents, for example a UAT environment should have the name `PRJCT-RG-UAT`.
- Azure users who require access to manage the environment should be given the **Contributor** role on the Resource Group. Depending on the security requirements, this could be done via [Privileged Identity Management](https://learn.microsoft.com/en-us/entra/id-governance/privileged-identity-management/).

### Create Role-Based Access Control (RBAC) Security Groups in Microsoft Entra ID

- Azure users who require access to manage the environment should be added to these groups.

| Group Name             | Use                                                       |
| ---------------------- | --------------------------------------------------------- |
| `RBAC-SQL-PRJCT-{ENV}` | Used to set the Active Directory Admin for the SQL Server |
| `RBAC-KV-PRJCT-{ENV}`  | Granted the "Secret Office" role on the Key Vault         |

### Create a Service Connection in Azure DevOps

- In Azure DevOps, go to Project Settings, then in the Pipelines subsection "Service connections"
- "New service connection" in the top right -> Azure Resource Manager -> Workload Identity federation (automatic)
- This Service Connection should be scoped to the newly created Resource Group in Azure, and named appropriately (e.g. `PRJCT-SERCON-UAT` for a Service Connection from DevOps through to the UAT environment Resource Group)
- Grant access permission to all pipelines, and Save. This will create a App Registration in Azure Entra ID, with a pre-configured federated credential to allow Azure DevOps to connect to the Azure Resource Group.
- Then as an Azure Admin, open the Service Connection in Azure DevOps -> Manage Service Principal -> Branding and Properties section, and give the principal the same name as the connection, as set above

### Set up a new "Environment" in Azure DevOps

- In Azure DevOps, under the "Pipelines" sidebar section, go to "Environments"
- Create a "New environment" in DevOps to represent the environment, with no resources for now, and name it appropriately (e.g. UAT to represent the UAT environment)
- Click into the Environment, and in the triple dots menu at the top right select "Security"
  - Add a Pipeline permission in the bottom card for the main pipeline (currently Build and Deploy) to allow the pipeline to use resources from this Environment
  - Update the inherited Project Administrators permission from Reader to Administrator in the top "User permissions" card
  - Update the inherited Project Valid Users permission from Reader to User in the top "User permissions" card and hit Save
- Click back to the Environment and in the top tabs beneath the environment name, select "Approvals and Checks"
  - Add an "Approvals" check (via the + or the "Add your first check" section) and add the relevant Deployment Approvers group, any description, and hit create

## Code changes for a new environment

As well as configuring things in the cloud, there are a few pre-requisite code changes that must be made to configure a new environment.

### `Web\appsettings.{env}.json`

Each environment should have its own app settings JSON file, which should be named appropriately. For example, `appsettings.uat.json` for the UAT environment. It is typically easiest to copy an existing app settings file and tweak the values contained within as necessary for the new environment.
Anything that is not defined in this file will be taken from the `appsettings.json` file.

### `Tools\Infrastructure\{env}.bicepparam`

Each environment should have its own Bicep parameters file, which should be named appropriately. For example, `uat.bicepparams` for the UAT environment. It is typically easiest to copy an existing parameters file and tweak the values contained within as necessary for the new environment.

The parameters file is split into 2 sections, one for non-secret parameters, and one for secret parameters. The non-secret parameters are as follows and should be defined in the file:

- `environment`: a short name for the environment used in the name of crated resources, e.g. `UAT`
- `aspNetCoreEnvironment`: the ASP.NET Core environment name, e.g. `uat` (this should match the env name used in the appsettings file)
- `sqlAdminGroupName`: the **Name** of the RBAC Security Group that was created to define the Active Directory Admin(s) for the SQL Server resource (e.g. `RBAC-SQL-PRJCT-UAT` for the UAT environment)
- `sqlAdminObjectId` - the Object ID of the RBAC Security Group that was created to define the Active Directory Admin(s) for the SQL Server resource
- `keyVaultAdminGroupObjectId` - the Object ID of the RBAC Security Group that was created to define the Key Vault Secret Officers for the Key Vault resource
- `resourceSizing`: the scale of the resources to be created, e.g. `nonprod` or `prod`

The secure parameters are as follows and should be defined as environment variables when running the Bicep deployment:

- `secretMessagePassword`: the password to be used to access the secret message in the application

## Running Bicep to create Azure Resources

Bicep is an Infrastructure as Code (IaC) solution developed by Microsoft as a slightly nicer abstraction over Azure Resource Manager (ARM) templates.

### Install pre-requisites

Before you can run Bicep (and the related setup steps) you need to install [the Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (`az`).

### Ensure permissions

You should already have the correct permissions at this step, but to be able to create the Azure resources, you must have the "Contributor" permission on the resource group.

### Create the Azure Resources

Open PowerShell in the [root](../../) directory of the repository and run the following steps (substituting the appropriate values for the parameters):

First, login to Azure and select the correct subscription when prompted.

```powershell
az login
```

To verify you are logged in to the correct Azure Subscription, you can run the following command. You should see the Resource Group you are deploying to listed in the output.

```powershell
az group list --output table
```

Finally, run the following command to create the Azure Resources. This will create the resources defined in the `QQProjectName.bicep` file, using the parameters defined in the `<ENVIRONMENT_NAME>.bicepparam` file.
Make sure to add any secure parameters as environment variables before running the command.

```powershell
$env:<ENVIRONMENT_NAME>_SECRET_MESSAGE_PASSWORD = 'Password123'
az deployment group create --template-file ./Tools/Infrastructure/QQProjectName.bicep --parameters ./Tools/Infrastructure/<ENVIRONMENT_NAME>.bicepparam -g <RESOURCE_GROUP_NAME> -c
```

You will now be prompted to review the proposed changes. If everything looks correct, you can confirm the changes and the deployment will begin.

For example, if setting up the UAT environment, this chain of commands might look as follows:

```powershell
az login
az group list --output table
$env:UAT_SECRET_MESSAGE_PASSWORD = 'Password123'
az deployment group create --template-file ./Tools/Infrastructure/QQProjectName.bicep --parameters ./Tools/Infrastructure/uat.bicepparam -g PRJCT-RG-UAT -c
```

This will log in to the correct Azure Subscription, and tell Bicep that we want to create a deployment into the `PRJCT-RG-UAT` Resource Group, using the `PorjectName.bicep` root template file, with parameters from the `uat.bicepparam` file.
The parameters file will load the `secretMessagePassword` parameter from the `UAT_SECRET_MESSAGE_PASSWORD` environment variable, so this need to be set before we run the Bicep command.
Bicep will ask us to review the proposed actions, and if it looks accurate, we can "confirm".

Once you have reviewed and confirmed the changes to be applied, Bicep will run and kick off the deployment. You can check the progress of the deployment in the Azure Portal by navigating to the target Resource Group and selecting the “Settings > Deployments” option from the sidebar.

The deployment may take a little while, and there is always a chance it could fail due to a transient error or race condition. If the deployment does not succeed the first time, simply attempt it again by running the same command. Resources that have already been created and configured by Bicep will be left alone on subsequent deployments, unless relevant changes have been made to the template or parameter files.

### Manual Setup Steps

Running Bicep will create and configure most of the required Azure infrastructure. However due to some of its limitations, some things must be configured manually.

#### Granting database access

Both the Web App and Azure Pipeline require the ability to connect to the database. Bicep has set up the users in the provided Entra Id group as administrators on the SQL Server, but now we need to grant these two services access.

Connect to the Azure SQL Database either using the [Query Editor in Azure Portal](https://learn.microsoft.com/en-us/azure/azure-sql/database/query-editor?view=azuresql), or using SSMS locally.

To allow the Pipeline Agent VM access to run migrations and seed data:

- `CREATE USER [<DEVOPS_SERVICE_CONNECTION_NAME>] FROM EXTERNAL PROVIDER`
- `ALTER ROLE db_datareader ADD MEMBER [<DEVOPS_SERVICE_CONNECTION_NAME>]`
- `ALTER ROLE db_datawriter ADD MEMBER [<DEVOPS_SERVICE_CONNECTION_NAME>]`
- `ALTER ROLE db_ddladmin ADD MEMBER [<DEVOPS_SERVICE_CONNECTION_NAME>]`

To allow the Web App to read and write data:

- `CREATE USER [<WEB_APP_NAME>] FROM EXTERNAL PROVIDER`
- `ALTER ROLE db_datareader ADD MEMBER [<WEB_APP_NAME>]`
- `ALTER ROLE db_datawriter ADD MEMBER [<WEB_APP_NAME>]`

## Set up Automated Deployments

Once the necessary infrastructure has been created in Azure, the final step is to configure the existing CI/CD pipelines to be able to deploy to it.

### Extend the deployment pipeline

Extending the pipeline is easily done by modifying the `build-and-deploy.yaml` file located in [`Tools\Pipelines`](../../Tools/Pipelines).
Within this file, a new `stage` needs to be added at the bottom of the file, with the appropriate configuration.

The easiest way to add a new stage definition is to copy an existing definition from within the same file, and replace the parameters as necessary:

- `stage`: this is an identifier for the stage within the pipeline, and should be set to something meaningful in line with the existing naming conventions (e.g. `uat` for the UAT environment)
- `displayName`: this is the name that will be used in the Azure DevOps UI to identify the stage, and should again be set to something meaningful in line with the existing naming conventions (e.g. `UAT` for the UAT environment)
- `dependsOn` - this is an array of other stages through which the code should have been promoted before being available for deployment to the current stage (for example, code should be tested on the QA before being promoted to the UAT environment, so for the UAT stage this value should be `[ qa ]`)
  - For Production environments this should be set to the name of the final Test/Staging stage, so that code must pass through the entire QA/UAT/Staging pipeline before landing in Production
- `jobs`
  - `template` - this should be set to `deploy-job-template.yaml` to reference the template file that defines common deployment steps
  - `parameters`
    - `deploymentName` - this should be set as appropriate based on the existing naming conventions (e.g. `deploy_uat` for the UAT environment)
    - `environmentName` - this is the name of the **Environment** in Azure DevOps you created in an earlier part of the setup process
    - `azureSubscription` - this is the name of the **Service Connection** in Azure DevOps you created right at the start to allow a connection through to Azure (e.g. `PRJCT-SERCON-UAT` for that UAT environment)
    - `webAppResourceName` - this is the name of the **App Service** resource in Azure that was created by running Bicep (e.g. `PRJCT-WA01-UAT` for the UAT environment)
    - `connectionString` - this is the SQL connection string used by the Pipeline VM to connect to the SQL Server Database, and is easiest defined by copying and modifying another connection string from within the same filed
