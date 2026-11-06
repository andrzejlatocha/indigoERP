-- indigoERP: dic.TypPlikGrupa (alias tpg) - grupy typów plików, pole tpkGrupa w dic.TypPlik wskaże grupę
-- Skrypt idempotentny. Cel: JAPET (indigoERP, potwierdzone 2026-11-04). Domyślnie test: @Zatwierdz = 0 -> ROLLBACK.
-- Uruchamia użytkownik.

SET NOCOUNT ON;
DECLARE @Zatwierdz bit = 0;

BEGIN TRANSACTION;

IF OBJECT_ID(N'dic.TypPlikGrupa', N'U') IS NULL
BEGIN
    CREATE TABLE dic.TypPlikGrupa (
        tpgID           int IDENTITY(1,1) NOT NULL,
        tpgNazwa        nvarchar(200) NOT NULL,
        tpgStatus       char(1) NOT NULL CONSTRAINT DF_TypPlikGrupa_tpgStatus DEFAULT '2',
        DataUtworzenie  datetime NULL CONSTRAINT DF_TypPlikGrupa_DataUtworzenie DEFAULT GETDATE(),
        UtworzenieID    int NULL CONSTRAINT DF_TypPlikGrupa_UtworzenieID DEFAULT 0,
        DataModyfikacja datetime NULL CONSTRAINT DF_TypPlikGrupa_DataModyfikacja DEFAULT GETDATE(),
        ModyfikacjaID   int NULL CONSTRAINT DF_TypPlikGrupa_ModyfikacjaID DEFAULT 0,
        Usuniety        bit NOT NULL CONSTRAINT DF_TypPlikGrupa_Usuniety DEFAULT 0,
        rowguid         uniqueidentifier ROWGUIDCOL NOT NULL CONSTRAINT DF_TypPlikGrupa_rowguid DEFAULT NEWID(),
        CONSTRAINT PK_TypPlikGrupa PRIMARY KEY CLUSTERED (tpgID)
    );
END;

-- Kontrola stanu (tylko odczyt)
SELECT OBJECT_ID(N'dic.TypPlikGrupa', N'U') AS TypPlikGrupaIstnieje, (SELECT COUNT(*) FROM dic.TypPlikGrupa) AS LiczbaGrup;

IF @Zatwierdz = 1
    COMMIT TRANSACTION;
ELSE
    ROLLBACK TRANSACTION;
