/* indigoERP: tabela powiadomien uzytkownika (licznik na naglowku, lista, oznaczanie jako przeczytane).
   Idempotentny. Uruchamia uzytkownik: najpierw CERES, potem JAPET (baza indigoERP). */
IF DB_NAME() <> N'indigoERP' THROW 50000, N'Wymagana baza indigoERP.', 1;

IF OBJECT_ID(N'dbo.Powiadomienie', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Powiadomienie (
        powID                 int IDENTITY(1,1) NOT NULL,
        powAplikacjaID        int NOT NULL,
        powUzytkownikID       int NOT NULL,
        powKodRodzaj          varchar(20) NOT NULL,
        powTytul              nvarchar(200) NOT NULL,
        powTresc              nvarchar(1000) NULL,
        powLink               nvarchar(500) NULL,
        powDataPowiadomienia  datetime NOT NULL CONSTRAINT DF_Powiadomienie_powDataPowiadomienia DEFAULT (GETDATE()),
        powOdczyt             bit NOT NULL CONSTRAINT DF_Powiadomienie_powOdczyt DEFAULT (0),
        powDataOdczytu        datetime NULL,
        DataUtworzenie        datetime NULL CONSTRAINT DF_Powiadomienie_DataUtworzenie DEFAULT (GETDATE()),
        UtworzenieID          int NULL CONSTRAINT DF_Powiadomienie_UtworzenieID DEFAULT (0),
        DataModyfikacja       datetime NULL CONSTRAINT DF_Powiadomienie_DataModyfikacja DEFAULT (GETDATE()),
        ModyfikacjaID         int NULL CONSTRAINT DF_Powiadomienie_ModyfikacjaID DEFAULT (0),
        Usuniety              bit NOT NULL CONSTRAINT DF_Powiadomienie_Usuniety DEFAULT (0),
        rowguid               uniqueidentifier ROWGUIDCOL NOT NULL CONSTRAINT DF_Powiadomienie_rowguid DEFAULT (NEWID()),
        CONSTRAINT PK_Powiadomienie PRIMARY KEY CLUSTERED (powID)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Powiadomienie_01' AND object_id = OBJECT_ID(N'dbo.Powiadomienie'))
    CREATE NONCLUSTERED INDEX IX_Powiadomienie_01
        ON dbo.Powiadomienie (powUzytkownikID, powAplikacjaID, powOdczyt, Usuniety);
GO

IF SCHEMA_ID(N'mvw') IS NULL
    EXEC(N'CREATE SCHEMA mvw');
GO

EXEC(N'CREATE OR ALTER VIEW mvw.Powiadomienie AS
SELECT powID, powAplikacjaID, powUzytkownikID, powKodRodzaj, powTytul, powTresc, powLink,
       powDataPowiadomienia, powOdczyt, powDataOdczytu
FROM dbo.Powiadomienie
WHERE Usuniety = 0;');
GO

/* Kontrola (tylko odczyt). Oczekiwane: tabela i widok nie NULL, 5 indeksow/kluczy 1 (PK + IX). */
SELECT OBJECT_ID(N'dbo.Powiadomienie', N'U') AS TabelaID, OBJECT_ID(N'mvw.Powiadomienie', N'V') AS WidokID;
SELECT COUNT(*) AS LiczbaKolumn FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Powiadomienie');
