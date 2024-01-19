# Connecting to Azure Databases

During project setup, or when developing on the project, you may need to connect to a SQl Server database hosted in Azure.

## Prerequisites

### Whitelisting your IP address

First you must ensure that your IP address is whitelisted to access teh database. To do this, navigate to the SQL Server
resource in the Azure Portal, and click on `Networking`. From here, you can add your IP address to the Firewall rules section.
Name the rule something meaningful, and enter your IP address in the `Start IP` and `End IP` fields. You can find your IP
address by going to [WhatsisMyIPAddress.com](https://whatismyipaddress.com/). If you are editing the firewall rules from
the machine you want to connect from, you can click `Add your client IPv4 address` to automatically add your IP address,
but remember to add a meaningful name.

By default in this template, the Ghyston IP range is whitelisted, so you can connect from any machine on the Ghyston network.

## Gaining permission

You will need to be granted permission to access the database. To do this, you will need to be added to the SQL server
admin group. This would have been provided during environment setup, but but default should be something like `PRJCT-SQL-UAT`
for a UAT environment.

## Connecting via Azure Portal Query Editor

Open the Azure Portal and navigate to the SQL database resource. Click on `Query Editor (preview)` and login with your
Microsoft Entra account. You should now be able to run queries against the database.

## Connecting via SQL Server Management Studio (SSMS)

Open SSMS and click `Connect`. Select `Database Engine` as the server type, and enter the server name. This can be found
on the SQL Server resource in the Azure Portal, and will look something like `prjct-sql01-uat.database.windows.net`.
Select Azure Active Directory - Universal with MFA as the authentication type and enter your Microsoft Entra ID username.
Click `Connect` and you will be prompted to login. You should now be able to run queries against the database.

## Connecting via Jetbrains Tools

Open the database tool window in your IDE. Click the `+` button and select `Connect to Database...` and then `Add data source manually`.
Select `Azure SQL Database` as the data source type and enter the host. This can be found on the SQL Server resource in
the Azure Portal, and will look something like `prjct-sql01-uat.database.windows.net`. Click `Next`.

Select `Azure Active Directory interactive` as the authentication type and enter your Microsoft Entra ID username.
Click `Connect` and you will be prompted to login. You should now be able to run queries against the database.
