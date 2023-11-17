-- Disable temporal table system versioning (cannot drop versioned tables)
WHILE(EXISTS(SELECT 1 FROM SYS.TABLES WHERE temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE'))
BEGIN
  DECLARE @disableSystemVersioningSql NVARCHAR(2000)
  SELECT TOP 1 @disableSystemVersioningSql = ('ALTER TABLE ' + name + ' SET (SYSTEM_VERSIONING = OFF);')
  FROM SYS.TABLES WHERE temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE'
  EXEC (@disableSystemVersioningSql)
END

-- Drop all foreign keys
WHILE(EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'))
BEGIN
  DECLARE @dropFkSql NVARCHAR(2000)
  SELECT TOP 1 @dropFkSql = ('ALTER TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']')
  FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
  WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'
  EXEC (@dropFkSql)
END

-- Drop all tables
WHILE(EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'))
BEGIN
  DECLARE @dropTableSql NVARCHAR(2000)
  SELECT TOP 1 @dropTableSql = ('DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + ']')
  FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'
  EXEC (@dropTableSql)
END

-- Drop all sequences
WHILE(EXISTS(SELECT 1 FROM SYS.SEQUENCES))
BEGIN
    DECLARE @dropSequenceSql NVARCHAR(2000)
    SELECT TOP 1 @dropSequenceSql = ('DROP SEQUENCE ' + NAME)
    FROM SYS.SEQUENCES
    EXEC (@dropSequenceSql)
END

-- Drop all functions
WHILE(EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE type_desc = 'SQL_SCALAR_FUNCTION'))
BEGIN
    DECLARE @dropFunctionSql NVARCHAR(2000)
    SELECT TOP 1 @dropFunctionSql = ('DROP FUNCTION ' + NAME)
    FROM SYS.OBJECTS WHERE type_desc = 'SQL_SCALAR_FUNCTION'
    EXEC (@dropFunctionSql)
END

-- Drop all views (apart from the 'database_firewall_rules' view)
WHILE(EXISTS(SELECT 1 FROM SYS.OBJECTS WHERE type_desc = 'VIEW' AND NAME != 'database_firewall_rules'))
BEGIN
    DECLARE @dropViewSql NVARCHAR(2000)
    SELECT TOP 1 @dropViewSql = ('DROP VIEW ' + NAME)
    FROM SYS.OBJECTS WHERE type_desc = 'VIEW' AND NAME != 'database_firewall_rules'
    EXEC (@dropViewSql)
END
