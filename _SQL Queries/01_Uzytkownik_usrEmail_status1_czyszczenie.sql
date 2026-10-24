-- 01: Jednorazowe wyczyszczenie adresów e-mail kont nieaktywnych (usrStatus = '1') w indigoERP.dbo.Uzytkownik.
-- Reguła użytkownika: konto ze statusem 1 nie ma adresu e-mail. Przed zmianą stare wartości trafiają do tabeli
-- tmp.UzytkownikEmailKopia_20261015 (można je odtworzyć). Skrypt idempotentny; uruchomić na JAPET/indigoERP (produkcja).

USE indigoERP;
GO

SET NOCOUNT ON;

-- Kontrola przed zmianą: ilu nieaktywnych użytkowników ma adres
SELECT COUNT(*) AS doWyczyszczenia
FROM dbo.Uzytkownik
WHERE ISNULL(Usuniety, 0) = 0
      AND usrStatus = '1'
      AND LTRIM(RTRIM(ISNULL(usrEmail, ''))) <> '';
GO

BEGIN TRANSACTION;

IF OBJECT_ID('tmp.UzytkownikEmailKopia_20261015') IS NULL
    BEGIN
        CREATE TABLE tmp.UzytkownikEmailKopia_20261015
        (
            usrID INT NOT NULL PRIMARY KEY
          , usrNazwa VARCHAR(50) NULL
          , usrEmailPrzed VARCHAR(255) NULL
          , DataKopii DATETIME NOT NULL DEFAULT GETDATE()
        );
    END;

INSERT INTO tmp.UzytkownikEmailKopia_20261015 (usrID, usrNazwa, usrEmailPrzed)
SELECT u.usrID, u.usrNazwa, u.usrEmail
FROM dbo.Uzytkownik AS u
WHERE ISNULL(u.Usuniety, 0) = 0
      AND u.usrStatus = '1'
      AND LTRIM(RTRIM(ISNULL(u.usrEmail, ''))) <> ''
      AND NOT EXISTS (SELECT 1 FROM tmp.UzytkownikEmailKopia_20261015 AS k WHERE k.usrID = u.usrID);

UPDATE dbo.Uzytkownik
SET usrEmail = ''
  , DataModyfikacja = GETDATE()
WHERE ISNULL(Usuniety, 0) = 0
      AND usrStatus = '1'
      AND LTRIM(RTRIM(ISNULL(usrEmail, ''))) <> '';

PRINT 'Wyczyszczono adresów: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

COMMIT TRANSACTION;
GO

-- Kontrola po zmianie (powinno być 0) i liczba zapisanych kopii
SELECT COUNT(*) AS pozostaloZAdresem
FROM dbo.Uzytkownik
WHERE ISNULL(Usuniety, 0) = 0
      AND usrStatus = '1'
      AND LTRIM(RTRIM(ISNULL(usrEmail, ''))) <> '';

SELECT COUNT(*) AS wKopii FROM tmp.UzytkownikEmailKopia_20261015;
GO

-- Odtworzenie (tylko w razie potrzeby):
-- UPDATE u SET u.usrEmail = k.usrEmailPrzed FROM dbo.Uzytkownik u JOIN tmp.UzytkownikEmailKopia_20261015 k ON k.usrID = u.usrID;