-- Widok mvw.vNowoscUzytkownik — dbo.NowoscUzytkownik (co użytkownik już widział) wzbogacony
-- o nowAplikacjaID z dbo.Nowosc (NowoscUzytkownik sam nie ma kolumny aplikacji), żeby dało się
-- filtrować "co ten użytkownik już widział w tej aplikacji" bez JOIN-a po stronie C#.
-- Uruchomić na CERES/indigoERP.

CREATE VIEW mvw.vNowoscUzytkownik AS
SELECT
    nu.nusID,
    nu.nusNowoscID,
    nu.nusUzytkownikID,
    nu.nusStatus,
    n.nowAplikacjaID
FROM dbo.NowoscUzytkownik nu
INNER JOIN dbo.Nowosc n ON nu.nusNowoscID = n.nowID;
GO
