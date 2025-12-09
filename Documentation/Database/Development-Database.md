# Development Database

The backend of this application integrates with a [SQL Server](https://www.microsoft.com/en-gb/sql-server/sql-server-downloads)
database.

In development, this database is run in a docker container on port 1407. It will have 2 separate databases setup as described below.

| Database Name | User      | Password            | Description                                                   |
| ------------- | --------- | ------------------- | ------------------------------------------------------------- |
| Legit         | Legit     | DefinitelyDurable1! | Used for the main .NET backend while running the application. |
| LegitTest     | LegitTest | TotallyTrusted2!    | Used by the test project when running integration tests.      |

When connecting to the SQL Server, you can either use the above users, or connect using the admin user details:

- User: `SA`
- Password: `SuperSecure0!`
