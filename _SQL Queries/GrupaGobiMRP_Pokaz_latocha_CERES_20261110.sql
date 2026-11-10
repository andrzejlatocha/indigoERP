-- Pokaz gobiMRP na CERES (2026-11-10): brakujące grupy ról budżetu i członkostwo andrzej.latocha we wszystkich rolach gobiMRP.
-- Uruchamiać TYLKO na CERES (identyfikatory grup poniżej są z CERES 2026-11-10). Skrypt idempotentny.
-- Role z kodu (GobiRoleAccess): Kierownik, Zakupy, Zatwierdz są nowe i nie istniały na CERES.
-- Uwaga: rola Kierownik wymaga, żeby claim wdzID użytkownika był równy wydziałowi budżetu (wdzID z indigoERP).

USE indigoERP;
GO

SET NOCOUNT ON;

-- 1. Brakujące grupy gobiMRP (nazwy ASCII zgodne z kodem).
INSERT INTO dbo.Grupa (groAplikacjaID, groNazwa, groOpis, groDataWaznosc, groGrupaWszyscy, groAdministrator, groBlokada, groEdycja, groSynchronizacjaAD, groStatus)
SELECT 150, n.nazwa, n.opis, '2099-12-31', 0, 0, 0, 0, 0, '2'
FROM (VALUES
    (N'HCZ-gobiMRP-Kierownik', N'Kierownik wydzialu (budzet, etap I)'),
    (N'HCZ-gobiMRP-Zakupy', N'Dzial Zakupow (budzet, etap II)'),
    (N'HCZ-gobiMRP-Zatwierdz', N'Zatwierdzenie budzetowe (etap III)')
) AS n (nazwa, opis)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Grupa AS g WHERE g.groNazwa = n.nazwa AND g.groAplikacjaID = 150 AND ISNULL(g.Usuniety, 0) = 0);
PRINT 'Dodano grup: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- 2. Użytkownik demo.
DECLARE @usr INT = (SELECT usrID FROM dbo.Uzytkownik WHERE usrNazwa = 'andrzej.latocha' AND ISNULL(Usuniety, 0) = 0 AND usrStatus = '2');
IF @usr IS NULL THROW 50000, 'Brak aktywnego uzytkownika andrzej.latocha.', 1;

-- 3. Członkostwa we wszystkich grupach gobiMRP (ID istniejących grup z CERES + nowe grupy po nazwie).
INSERT INTO dbo.GrupaCzlonkowie (uwgGrupaNadrzednaID, uwgGrupaID, uwgUzytkownikID, uwgDomyslna, uwgStatus)
SELECT g.groID, NULL, @usr, 0, '2'
FROM dbo.Grupa AS g
WHERE ISNULL(g.Usuniety, 0) = 0
  AND (g.groID IN (2120, 2353, 2414, 2413, 2409, 2121, 2410, 2412)
       OR (g.groAplikacjaID = 150 AND g.groNazwa IN (N'HCZ-gobiMRP-Kierownik', N'HCZ-gobiMRP-Zakupy', N'HCZ-gobiMRP-Zatwierdz')))
  AND NOT EXISTS (SELECT 1
                  FROM dbo.GrupaCzlonkowie AS c
                  WHERE c.uwgGrupaNadrzednaID = g.groID
                        AND c.uwgUzytkownikID = @usr
                        AND c.uwgGrupaID IS NULL
                        AND (c.uwgStatus = '2' OR c.uwgStatus IS NULL));
PRINT 'Dodano czlonkostw: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- 4. Kontrola (tylko odczyt): grupy gobiMRP użytkownika demo po zmianie.
SELECT g.groID, g.groNazwa
FROM dbo.GrupaCzlonkowie AS c
     INNER JOIN dbo.Grupa AS g ON g.groID = c.uwgGrupaNadrzednaID
WHERE c.uwgUzytkownikID = @usr AND c.uwgGrupaID IS NULL AND (c.uwgStatus = '2' OR c.uwgStatus IS NULL)
  AND g.groAplikacjaID = 150
ORDER BY g.groNazwa;
