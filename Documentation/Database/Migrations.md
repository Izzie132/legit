# Migration
Database migrations are handled by the `Tools/Migrations` project, using [DbUp](https://dbup.readthedocs.io/en/latest/).

## Creating a new migration
To create a new migration, create a new SQL files within `Tools/Migrations/Scripts` with a name that follows the pattern
`M<yyyy><MM><dd>T<HH><mm>_<MIGRATION_NAME>.sql`. For example, if a migration was created to add a users table,
it might have a name like `M20230523T1225_CreateUsersTable.sql`

Within this migration, you can add the SQL required to be run against the database. Remember that all migrations with an
earlier datestamp will have been run before the this one. For the above example migration, we might expect the contents to
look something like the following.

```sql
CREATE TABLE Users (
    Id INT IDENTITY(1000,1) PRIMARY KEY,
    Name NVARCHAR(MAX) NOT NULL,
    Email NVARCHAR(MAX) NOT NULL,
);
```

## Running the migrations
To run the migrations, you can open a terminal in the `Tools/Migrations` directory and run `dotnet run <CONNECTION_STRING>`.

By default, this will ensure that the target database has had all existing migrations run against it. DbUp tracks what 
migrations have been run against the database using the `SchemaVersions` table.
There a multiple options that you can pass into this command to customise how it works:

- `--cleanFirst`: this will fully wipe the database before then running all the migrations in order
- `--quiet`: this will suppress any console outputs unless there is an error

In local development, you can use the pre-defined NUKE Build tasks to handle running database migrations:

- `MigrateDevelopmentDatabase`: this will run the migrations against the development database
- `MigrateTestDatabase`: this will run the migrations against the test database
- `ResetDevelopmentDatabase`: this will clean and run the migrations against the development database
- `ResetTestDatabase`: this will clean and run the migrations against the test database
