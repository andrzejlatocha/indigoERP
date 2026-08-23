-- Moduł "Wiadomości" — flaga "publikuj publicznie" (zastąpienie linku do zewnętrznego
-- Głos Hutnika na wenus.hcz.com.pl własną, ogólnodostępną stroną w indigoERP).
--
-- WAŻNE: dbo.Wiadomosc to współdzielony log wiadomości systemowych (cerberTMS=130,
-- atacamaDMS=210, cerberTMS.Portal=132), zawierający m.in. maile resetu hasła —
-- NIE WOLNO wystawić go publicznie wprost. Dlatego:
--   1) nowa kolumna wiaPubliczna (NULL/0 domyślnie — bez wpływu na 931 istniejących
--      wierszy z innych aplikacji, które o niej nie wiedzą),
--   2) osobny, wąski widok mvw.vWiadomoscPubliczna — twardo ograniczony do
--      wiaAplikacjaID=900 (indigoERP) + wiaPubliczna=1 + wiaStatus='2', i tylko z
--      kolumnami temat/treść/data (bez adresu e-mail, bez ID użytkownika).
-- Uruchomić na CERES/indigoERP.

USE indigoERP;
GO

ALTER TABLE dbo.Wiadomosc ADD wiaPubliczna bit NULL CONSTRAINT DF_Wiadomosc_wiaPubliczna DEFAULT (0);
GO

CREATE OR ALTER VIEW mvw.vWiadomoscHistoria AS
SELECT
    w.wiaID,
    w.wiaAplikacjaID,
    w.wiaUzytkownikID,
    w.wiaEmail,
    w.wiaTemat,
    w.wiaTresc,
    w.wiaData,
    w.wiaStatus,
    w.wiaPubliczna,
    wy.wwyID,
    wy.wwyData,
    wy.wwyStatus
FROM dbo.Wiadomosc w
LEFT JOIN dbo.WiadomoscWysylka wy ON wy.wwyWiadomoscID = w.wiaID
WHERE (w.Usuniety = 0 OR w.Usuniety IS NULL);
GO

CREATE OR ALTER VIEW mvw.vWiadomoscPubliczna AS
SELECT wiaID, wiaTemat, wiaTresc, wiaData
FROM dbo.Wiadomosc
WHERE wiaAplikacjaID = 900
    AND wiaPubliczna = 1
    AND wiaStatus = '2'
    AND (Usuniety = 0 OR Usuniety IS NULL);
GO
