-- Dodanie użytkowników do grup gobiMRP: 2409 (HCZ-gobiMRP-Plan) i 2353 (HCZ-gobiMRP-Budzet) w bazie indigoERP.
-- Skrypt idempotentny: pomija członkostwa, które już istnieją (aktywne: uwgStatus = '2' lub NULL).
-- Wiersz jak z GrupaController.InsertCzlonek: uwgGrupaNadrzednaID = grupa, uwgGrupaID = NULL, uwgUzytkownikID = użytkownik,
-- uwgDomyslna = 0 (domyślnie), uwgStatus = '2' (domyślnie), rowguid = NEWID() (domyślnie).
-- Użytkownicy wskazani przez usrNazwa (login) - rozwiązane z listy imion i nazwisk 2026-10-14 (34 z 35 osób).
-- NIE znaleziono "Alicja Maćkowiak" - w bazie jest "Alicja Maćkowska-Zapała" (alicja.mackowska, usrID 4786);
-- odkomentować wiersz poniżej TYLKO jeśli to ta osoba.
-- Uruchomić na JAPET/indigoERP (produkcja); na CERES identyfikatory grup mogą być inne - sprawdzić sekcję kontrolną.

USE indigoERP;
GO

SET NOCOUNT ON;

DECLARE @grupy TABLE (groID INT PRIMARY KEY);
INSERT INTO @grupy (groID) VALUES (2409), (2353);

DECLARE @loginy TABLE (usrNazwa VARCHAR(50) PRIMARY KEY);
INSERT INTO @loginy (usrNazwa) VALUES
    ('lukasz.rzepa'), ('arkadiusz.krawczyk'), ('jaroslaw.duszynski'), ('agnieszka.cieslak'), ('tomasz.pasamonik'),
    ('renata.warzecha'), ('renata.gawor'), ('nikola.wolinska'), ('adam.zurek'), ('jaroslaw.kossek'),
    ('jaroslaw.jagiello'), ('grazyna.falana'), ('marek.matczak'), ('karolina.kacperak'), ('elzbieta.adamczyk'),
    ('agnieszka.szybowska'), ('pawel.juszczyk'), ('witold.gawron'), ('piotr.flis'), ('michal.betka'),
    ('arkadiusz.pyrkosz'), ('slawomir.radwanski'), ('agata.pantak'), ('dariusz.szyda'), ('agnieszka.pokora'),
    ('robert.walasik'), ('katarzyna.gega'), ('piotr.rybczynski'), ('radoslaw.szelag'), ('piotr.gajewski'),
    ('jakub.rogowski'), ('andrzej.latocha'), ('dominik.kopec'), ('slawomir.margas');
    -- , ('alicja.mackowska')   -- Alicja Maćkowiak? (patrz uwaga na początku skryptu)

-- Kontrola przed zmianą: loginy bez aktywnego użytkownika (powinno być 0 wierszy)
SELECT l.usrNazwa AS brakUzytkownika
FROM @loginy AS l
WHERE NOT EXISTS (SELECT 1 FROM dbo.Uzytkownik AS u WHERE u.usrNazwa = l.usrNazwa AND ISNULL(u.Usuniety, 0) = 0 AND u.usrStatus = '2');

-- Kontrola przed zmianą: grupy muszą istnieć (powinny być 2 wiersze)
SELECT g.groID, g.groNazwa, g.groAplikacjaID FROM dbo.Grupa AS g WHERE g.groID IN (SELECT groID FROM @grupy);

BEGIN TRANSACTION;

INSERT INTO dbo.GrupaCzlonkowie (uwgGrupaNadrzednaID, uwgGrupaID, uwgUzytkownikID, uwgDomyslna, uwgStatus)
SELECT g.groID, NULL, u.usrID, 0, '2'
FROM @grupy AS g
     CROSS JOIN @loginy AS l
     INNER JOIN dbo.Uzytkownik AS u ON u.usrNazwa = l.usrNazwa AND ISNULL(u.Usuniety, 0) = 0 AND u.usrStatus = '2'
WHERE EXISTS (SELECT 1 FROM dbo.Grupa AS gr WHERE gr.groID = g.groID)
      AND NOT EXISTS (SELECT 1
                      FROM dbo.GrupaCzlonkowie AS c
                      WHERE c.uwgGrupaNadrzednaID = g.groID
                            AND c.uwgUzytkownikID = u.usrID
                            AND c.uwgGrupaID IS NULL
                            AND (c.uwgStatus = '2' OR c.uwgStatus IS NULL));

PRINT 'Dodano członkostw: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

COMMIT TRANSACTION;

-- Kontrola po zmianie: liczba aktywnych członków obu grup oraz brakujący z listy (powinno być 0 wierszy)
SELECT c.uwgGrupaNadrzednaID AS groID, COUNT(*) AS liczbaCzlonkow
FROM dbo.GrupaCzlonkowie AS c
WHERE c.uwgGrupaNadrzednaID IN (SELECT groID FROM @grupy) AND (c.uwgStatus = '2' OR c.uwgStatus IS NULL)
GROUP BY c.uwgGrupaNadrzednaID;

SELECT g.groID, l.usrNazwa AS brakCzlonkostwa
FROM @grupy AS g CROSS JOIN @loginy AS l
WHERE NOT EXISTS (SELECT 1
                  FROM dbo.GrupaCzlonkowie AS c INNER JOIN dbo.Uzytkownik AS u ON c.uwgUzytkownikID = u.usrID
                  WHERE c.uwgGrupaNadrzednaID = g.groID AND u.usrNazwa = l.usrNazwa AND (c.uwgStatus = '2' OR c.uwgStatus IS NULL));
GO
