-- Zmiana zakresu Głosu Hutnika: każdy unikalny adres służbowy oraz prywatny przy zgodzie.
-- Baza docelowa: JAPET / indigoERP. Skrypt uruchamia użytkownik w SSMS.
USE [indigoERP];
GO
IF SCHEMA_ID(N'app') IS NULL OR OBJECT_ID(N'app.vWiadomoscZakres2d', N'V') IS NULL
    THROW 50000, N'Brak istniejącego widoku app.vWiadomoscZakres2d.', 1;
ELSE
EXEC(N'CREATE OR ALTER VIEW app.vWiadomoscZakres2d AS
WITH kontakty AS
(
    SELECT p.prcID, p.prcKod, p.prcImie, p.prcNazwisko, k.kntID,
           LTRIM(RTRIM(k.kntKontakt)) AS kntKontakt,
           ROW_NUMBER() OVER (PARTITION BY p.prcID, LOWER(LTRIM(RTRIM(k.kntKontakt)))
                              ORDER BY CASE WHEN k.kntTypKontaktID = 9 THEN 0 ELSE 1 END, k.kntID) AS rn
    FROM epicaHR.dbo.Pracownik AS p
    JOIN epicaHR.dbo.PracownikKontakt AS pk ON pk.pktPracownikID = p.prcID AND pk.Usuniety = 0
    JOIN dbo.Kontakt AS k ON k.kntID = pk.pktKontaktID
    WHERE NULLIF(LTRIM(RTRIM(k.kntKontakt)), N'''') IS NOT NULL
      AND EXISTS (SELECT 1 FROM epicaHR.dbo.Angaz AS a WHERE a.angPracownikID = p.prcID AND a.angStatus = 2)
      AND (k.kntTypKontaktID = 9 OR (k.kntTypKontaktID = 22 AND EXISTS
          (SELECT 1 FROM epicaHR.dbo.PracownikZgoda AS z WHERE z.pzoPracownikID = p.prcID AND z.pzoZgoda04 = 1)))
)
SELECT prcID, prcKod, prcImie, prcNazwisko, kntID, kntKontakt FROM kontakty WHERE rn = 1;');
GO
SELECT COUNT(*) AS adresy, COUNT(DISTINCT prcID) AS osoby FROM app.vWiadomoscZakres2d;
SELECT prcID, LOWER(LTRIM(RTRIM(kntKontakt))) AS adres, COUNT(*) AS wystapienia FROM app.vWiadomoscZakres2d
GROUP BY prcID, LOWER(LTRIM(RTRIM(kntKontakt))) HAVING COUNT(*) > 1;
GO
