-- Dodanie użytkowników do grupy gobiMRP 2409 (HCZ-gobiMRP-Plan) w bazie indigoERP - partia 2026-10-15 (6 osób).
-- Skrypt idempotentny: pomija członkostwa, które już istnieją (aktywne: uwgStatus = '2' lub NULL).
-- Wiersz jak z GrupaController.InsertCzlonek: uwgGrupaNadrzednaID = grupa, uwgGrupaID = NULL, uwgUzytkownikID = użytkownik,
-- uwgDomyslna = 0, uwgStatus = '2', rowguid = NEWID() (domyślnie).
-- Wszystkie loginy jednoznaczne i aktywne (sprawdzone na JAPET). "Marek Bednarski" z listy zdublowany - ujęty raz.
-- Pominięto: "Joanna Ciążkowska" - brak w bazie (najbliższa: Joanna Ciężkowska, joanna.ciezkowska, usrID 3274) - do potwierdzenia.
-- Uruchomić na JAPET/indigoERP (produkcja).

USE indigoERP;
GO

SET NOCOUNT ON;

DECLARE @grupy TABLE (groID INT PRIMARY KEY);
INSERT INTO @grupy (groID) VALUES (2409);

DECLARE @loginy TABLE (usrNazwa VARCHAR(50) PRIMARY KEY);
INSERT INTO @loginy (usrNazwa) VALUES
    ('andrzej.klok'),
    ('krzysztof.fresel'),
    ('marcin.szmigielski'),
    ('piotr.wiszowaty'),
    ('renata.sacharuk'),
    ('marek.bednarski');

-- Kontrola przed zmianą: loginy bez aktywnego użytkownika (powinno być 0 wierszy)
SELECT l.usrNazwa AS brakUzytkownika
FROM @loginy AS l
WHERE NOT EXISTS (SELECT 1 FROM dbo.Uzytkownik AS u WHERE u.usrNazwa = l.usrNazwa AND ISNULL(u.Usuniety, 0) = 0 AND u.usrStatus = '2');

-- Kontrola przed zmianą: grupa musi istnieć (powinien być 1 wiersz)
SELECT g.groID, g.groNazwa, g.groAplikacjaID FROM dbo.Grupa AS g WHERE g.groID IN (SELECT groID FROM @grupy);

BEGIN TRANSACTION;

INSERT INTO dbo.GrupaCzlonkowie (uwgGrupaNadrzednaID, uwgGrupaID, uwgUzytkownikID, uwgDomyslna, uwgStatus)
SELECT g.groID, NULL, u.usrID, 0, '2'
FROM @grupy AS g
     CROSS JOIN @loginy AS l
     INNER JOIN dbo.Uzytkownik AS u ON u.usrNazwa = l.usrNazwa AND ISNULL(u.Usuniety, 0) = 0 AND u.usrStatus = '2'
WHERE NOT EXISTS (SELECT 1
                  FROM dbo.GrupaCzlonkowie AS c
                  WHERE c.uwgGrupaNadrzednaID = g.groID
                        AND c.uwgUzytkownikID = u.usrID
                        AND c.uwgGrupaID IS NULL
                        AND (c.uwgStatus = '2' OR c.uwgStatus IS NULL));

PRINT 'Dodano członkostw: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

COMMIT TRANSACTION;

-- Kontrola po zmianie: liczba aktywnych członków grupy (przed partią: 43, po: 49)
SELECT c.uwgGrupaNadrzednaID AS groID, COUNT(*) AS liczbaCzlonkow
FROM dbo.GrupaCzlonkowie AS c
WHERE c.uwgGrupaNadrzednaID = 2409 AND (c.uwgStatus = '2' OR c.uwgStatus IS NULL)
GROUP BY c.uwgGrupaNadrzednaID;

-- Kontrola po zmianie: brakujący członkowie z listy (powinno być 0 wierszy)
SELECT l.usrNazwa AS brakCzlonkostwa
FROM @loginy AS l
WHERE NOT EXISTS (SELECT 1
                  FROM dbo.GrupaCzlonkowie AS c INNER JOIN dbo.Uzytkownik AS u ON c.uwgUzytkownikID = u.usrID
                  WHERE c.uwgGrupaNadrzednaID = 2409 AND u.usrNazwa = l.usrNazwa AND (c.uwgStatus = '2' OR c.uwgStatus IS NULL));
GO