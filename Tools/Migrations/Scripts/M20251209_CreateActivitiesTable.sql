CREATE TABLE Activities
(
    Id               INT IDENTITY(1000,1) PRIMARY KEY,
    UserId           NVARCHAR(100) NOT NULL,
    DateOfActivity   DATETIME       NOT NULL,
    Title            NVARCHAR(MAX) NOT NULL,
    Description      NVARCHAR(MAX),
    DistanceInMeters DECIMAL(18, 6) NOT NULL
);

CREATE TABLE Units
(
    Id                 INT IDENTITY(1,1) PRIMARY KEY,
    Name               NVARCHAR(50) NOT NULL,
    DisplayName        NVARCHAR(100) NOT NULL,
    ConversionToMeters DECIMAL(18, 6) NOT NULL
);

