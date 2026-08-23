-- Moduł "Wiadomości" w indigoERP — WYŁĄCZNIE nowe, dodatkowe widoki. Nie modyfikuje
-- żadnej istniejącej tabeli (dbo.Wiadomosc/WiadomoscUzytkownik/WiadomoscDokument/
-- WiadomoscWysylka/WiadomoscWzorzec to już istniejący, współdzielony między appkami
-- (cerberTMS=130, atacamaDMS=210, cerberTMS.Portal=132) log wiadomości systemowych —
-- 931 istniejących wierszy w dbo.Wiadomosc na CERES, więc świadomie NIE robimy tu
-- żadnego ALTER TABLE/DROP na tych tabelach. Uruchomić na CERES/indigoERP.
--
-- mvw.vWiadomosc i mvw.vWiadomoscLista JUŻ ISTNIEJĄ (sprawdzone na CERES) — nie
-- tworzymy ich ponownie.

USE indigoERP;
GO

-- 1. mvw.vWiadomoscWzorzec — lista wzorców (dla strony Wzorce.razor) ---------
CREATE VIEW mvw.vWiadomoscWzorzec AS
SELECT msgID, msgAplikacjaID, msgTyp, msgJezyk, msgTemat, msgTresc, msgStatus
FROM dbo.WiadomoscWzorzec;
GO

-- 2. enum.WiadomoscWzorzec — lookup do dropdowna przy wyborze wzorca ---------
CREATE VIEW enum.WiadomoscWzorzec AS
SELECT msgID, msgTemat, msgStatus
FROM dbo.WiadomoscWzorzec;
GO

-- 3. mvw.vWiadomoscHistoria — historia wysłanych wiadomości (Wiadomosc +
--    WiadomoscWysylka), BEZ zależności od WiadomoscUzytkownik (ta tabela ma
--    0 wierszy w praktyce — istniejące 931 wiadomości systemowych jej nie
--    używa, więc mvw.vWiadomosc/vWiadomoscLista — oparte na INNER JOIN do niej —
--    nie nadają się do listowania tej historii).
CREATE VIEW mvw.vWiadomoscHistoria AS
SELECT
    w.wiaID,
    w.wiaAplikacjaID,
    w.wiaUzytkownikID,
    w.wiaEmail,
    w.wiaTemat,
    w.wiaTresc,
    w.wiaData,
    w.wiaStatus,
    wy.wwyID,
    wy.wwyData,
    wy.wwyStatus
FROM dbo.Wiadomosc w
LEFT JOIN dbo.WiadomoscWysylka wy ON wy.wwyWiadomoscID = w.wiaID
WHERE (w.Usuniety = 0 OR w.Usuniety IS NULL);
GO
