-- Widok mvw.vUzytkownikAplikacjaDostep - pary (użytkownik, aplikacja), w których użytkownik ma co najmniej jedną rolę
-- (bezpośrednio albo przez grupę) - ta sama definicja "ma rolę w aplikacji" co w procedurze logowania
-- dbo.uspGetUzytkownik / uspGetUzytkownikSso (app.vUzytkownikRola). Służy stronie głównej indigoERP do ukrywania
-- kart aplikacji, do których użytkownik nie ma dostępu. Zmiana czysto addytywna (nowy widok), nic nie usuwa.
-- Uruchomić na CERES i JAPET/indigoERP.

USE indigoERP;
GO

CREATE OR ALTER VIEW mvw.vUzytkownikAplikacjaDostep AS
SELECT DISTINCT
       vur.usrID
     , vur.appID
FROM app.vUzytkownikRola AS vur INNER JOIN dbo.Uzytkownik AS u ON vur.usrID = u.usrID
                                                              AND ISNULL(u.Usuniety, 0) = 0
WHERE vur.appID IS NOT NULL;
GO

-- Kontrola (tylko odczyt): liczba aplikacji z dostępem na użytkownika - przykładowo pierwsi 10
SELECT TOP (10) usrID, COUNT(*) AS liczbaAplikacji
FROM mvw.vUzytkownikAplikacjaDostep
GROUP BY usrID
ORDER BY usrID;
GO
