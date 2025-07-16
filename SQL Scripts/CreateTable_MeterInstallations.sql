CREATE TABLE RegisteredMeters (
    MPAN NUMERIC(13, 0) NOT NULL,
    MeterSerial VARCHAR(10) NOT NULL,
    DateOfInstallation DATE NOT NULL,
    AddressLine1 VARCHAR(40) NULL,
    PostCode VARCHAR(10) NULL,

    -- Primary Key
    CONSTRAINT PK_MeterInstallations PRIMARY KEY (MPAN, MeterSerial, DateOfInstallation),

    -- MPAN must be exactly 13 digits
    CONSTRAINT CK_MPAN_ExactLength CHECK (LEN(CAST(MPAN AS VARCHAR)) = 13),

    -- MeterSerial must be between 1 and 10 characters
    CONSTRAINT CK_MeterSerial_Length CHECK (LEN(MeterSerial) BETWEEN 1 AND 10),

    -- DateOfInstallation must be in the past
    CONSTRAINT CK_DateOfInstallation_Past CHECK (DateOfInstallation < CAST(GETDATE() AS DATE)),

    -- PostCode must be 'XX9 9XX'
    CONSTRAINT CK_PostCode_Format CHECK (
        PostCode IS NULL OR
        PostCode LIKE '[A-Z][A-Z][0-9] [0-9][A-Z][A-Z]'
    )
);