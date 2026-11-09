-- indigoERP: dic.TypPlik - pola grupy, domyslnosci i obowiazkowosci + widok enum.TypPlik.
-- Cel: CERES (baza testowa indigoERP). Na JAPET struktura jest juz zmieniona.
-- Skrypt idempotentny: drugie uruchomienie nic nie zmienia. Uruchamia uzytkownik w SSMS (F5).
-- Wymaga wczesniej: dic.TypPlikGrupa (20261104_TypPlikGrupa\01_typplikgrupa.sql).
USE indigoERP;
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'dic.TypPlikGrupa', N'U') IS NULL
BEGIN
    THROW 50000, N'Brak tabeli dic.TypPlikGrupa: najpierw uruchom 20261104_TypPlikGrupa\01_typplikgrupa.sql.', 1;
END
GO

IF COL_LENGTH(N'dic.TypPlik', N'tpkTypPlikGrupaID') IS NULL
    ALTER TABLE dic.TypPlik ADD tpkTypPlikGrupaID int NULL;
GO

IF COL_LENGTH(N'dic.TypPlik', N'tpkDomyslny') IS NULL
    ALTER TABLE dic.TypPlik ADD tpkDomyslny bit NULL CONSTRAINT DF_TypPlik_tpkDomyslny DEFAULT ((0));
GO

IF COL_LENGTH(N'dic.TypPlik', N'tpkObowiazkowy') IS NULL
    ALTER TABLE dic.TypPlik ADD tpkObowiazkowy bit NOT NULL CONSTRAINT DF_TypPlik_tpkObowiazkowy DEFAULT ((0));
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TypPlik_TypPlikGrupa')
    ALTER TABLE dic.TypPlik WITH CHECK ADD CONSTRAINT FK_TypPlik_TypPlikGrupa
        FOREIGN KEY (tpkTypPlikGrupaID) REFERENCES dic.TypPlikGrupa (tpgID);
GO

CREATE OR ALTER VIEW enum.TypPlik
AS
    SELECT tp.tpkID
         , tp.tpkAplikacjaID
         , tp.tpkNazwa AS tpkOpis
         , tp.tpkTypPlikGrupaID
         , tp.tpkDomyslny
         , tp.tpkObowiazkowy
         , tp.tpkStatus
    FROM dic.TypPlik AS tp;
GO

-- Kontrola (tylko odczyt): oczekiwane 3 nowe kolumny w dic i w enum
SELECT TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE (TABLE_SCHEMA = N'dic' AND TABLE_NAME = N'TypPlik')
   OR (TABLE_SCHEMA = N'enum' AND TABLE_NAME = N'TypPlik')
ORDER BY TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME;
GO
