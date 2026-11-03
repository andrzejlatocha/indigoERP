-- Zakres wysyłki Głosu Hutnika (app.vWiadomoscZakres2d): jeden adres na pracownika.
-- Zgodne z definicją żywą na JAPET (OBJECT_DEFINITION), na dzień 2026-11-03.
-- Priorytet: e-mail służbowy (kntTypKontaktID = 9) dla każdego pracownika.
-- Fallback: e-mail prywatny (kntTypKontaktID = 22) tylko gdy pracownik ma zgodę GH (pzoZgoda04 = 1).
-- Tylko pracownicy z aktywnym zatrudnieniem (epicaHR.dbo.Angaz, angStatus = 2).
-- Pomijane przypisania usunięte logicznie (PracownikKontakt.Usuniety = 1).
-- Uruchamia użytkownik w SSMS na JAPET (F5). Skrypt zastępuje widok, który już służy do wysyłki.
USE indigoERP;
GO

CREATE OR ALTER VIEW app.vWiadomoscZakres2d AS
WITH kontakty AS (
    SELECT p.prcID,
           p.prcKod,
           p.prcImie,
           p.prcNazwisko,
           k.kntID,
           k.kntKontakt,
           CASE WHEN k.kntTypKontaktID = 9 THEN 1 ELSE 2 END AS priorytet
    FROM epicaHR.dbo.Pracownik AS p
    INNER JOIN epicaHR.dbo.PracownikKontakt AS pk ON p.prcID = pk.pktPracownikID
    INNER JOIN dbo.Kontakt AS k ON pk.pktKontaktID = k.kntID
    INNER JOIN epicaHR.dbo.Angaz AS a ON p.prcID = a.angPracownikID
                                     AND a.angStatus = 2
    WHERE pk.Usuniety = 0
      AND NULLIF(LTRIM(RTRIM(k.kntKontakt)), N'') IS NOT NULL
      AND (k.kntTypKontaktID = 9
           OR (k.kntTypKontaktID = 22
               AND EXISTS (SELECT 1
                           FROM epicaHR.dbo.PracownikZgoda AS pz
                           WHERE pz.pzoPracownikID = p.prcID
                             AND pz.pzoZgoda04 = 1)))
)
SELECT prcID, prcKod, prcImie, prcNazwisko, kntID, kntKontakt
FROM (
    SELECT kontakty.*,
           ROW_NUMBER() OVER (PARTITION BY prcID ORDER BY priorytet, kntID) AS rn
    FROM kontakty
) AS x
WHERE rn = 1;
GO

-- Kontrola (tylko odczyt): każdy pracownik raz
SELECT COUNT(*) AS wiersze, COUNT(DISTINCT prcID) AS pracownicy
FROM app.vWiadomoscZakres2d;
GO
