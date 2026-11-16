/* indigoERP: harmonogram wysylki wiadomosci (jak job w SQL Server Agent).
   Pola na dbo.Wiadomosc: rodzaj cyklu, poczatek, koniec (opcjonalny), nastepny termin, aktywnosc.
   Idempotentny. Uruchamia uzytkownik; najpierw CERES, potem JAPET. */
IF DB_NAME() <> N'indigoERP' THROW 50000, N'Wymagana baza indigoERP.', 1;

IF COL_LENGTH(N'dbo.Wiadomosc', N'wiaKodHarmonogram') IS NULL
    ALTER TABLE dbo.Wiadomosc ADD wiaKodHarmonogram varchar(1) NULL;
IF COL_LENGTH(N'dbo.Wiadomosc', N'wiaDataHarmonogramStart') IS NULL
    ALTER TABLE dbo.Wiadomosc ADD wiaDataHarmonogramStart datetime NULL;
IF COL_LENGTH(N'dbo.Wiadomosc', N'wiaDataHarmonogramKoniec') IS NULL
    ALTER TABLE dbo.Wiadomosc ADD wiaDataHarmonogramKoniec datetime NULL;
IF COL_LENGTH(N'dbo.Wiadomosc', N'wiaDataNastepnejWysylki') IS NULL
    ALTER TABLE dbo.Wiadomosc ADD wiaDataNastepnejWysylki datetime NULL;
IF COL_LENGTH(N'dbo.Wiadomosc', N'wiaHarmonogramAktywny') IS NULL
    ALTER TABLE dbo.Wiadomosc ADD wiaHarmonogramAktywny bit NOT NULL CONSTRAINT DF_Wiadomosc_wiaHarmonogramAktywny DEFAULT 0;
GO

/* Kontrola (tylko odczyt). Oczekiwane: 5 wierszy. */
SELECT name AS Kolumna FROM sys.columns
 WHERE object_id = OBJECT_ID(N'dbo.Wiadomosc')
   AND name IN (N'wiaKodHarmonogram', N'wiaDataHarmonogramStart', N'wiaDataHarmonogramKoniec', N'wiaDataNastepnejWysylki', N'wiaHarmonogramAktywny');
