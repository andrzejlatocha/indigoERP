-- Widok mvw.vUzytkownikLista — jeden wiersz na użytkownika (grid Administrator/Uzytkownik/Index),
-- zastępuje ręczne FromSqlRaw z CTE w UzytkownikController.BuildAdministrationUserQuery, które
-- rzucało "FromSql or SqlQuery was called with non-composable SQL" po skomponowaniu z LINQ join/Where
-- (SQL Server nie pozwala na WITH wewnątrz podzapytania, którym EF opakowuje FromSqlRaw przy kompozycji).
-- Uruchomić na CERES/indigoERP (serwer deweloperski; JAPET/indigoERP.Developer już nieaktualne).
-- UWAGA: sha.vUzytkownikLista (i przez nią atacamaDMS.rdb.vUzytkownikLista) mapuje kolumny z tego
-- widoku na sztywno po nazwie - przy zmianie zestawu/nazw kolumn tutaj zaktualizuj też
-- sha.vUzytkownikLista, patrz Uzytkownik_Lista_View_fix_sha_kompatybilnosc.sql.

CREATE VIEW mvw.vUzytkownikLista AS
WITH details AS
(
    SELECT
        usrID,
        usrPracownikID,
        prcKod,
        usrNazwa,
        usrImie,
        usrNazwisko,
        usrInicjal,
        usrTelefon,
        usrKomorka,
        usrEmail,
        appID,
        appFolder,
        usrFolder,
        usrZdjecie,
        wdzID,
        wdzKod,
        wdzSymbol,
        wdzNazwa,
        stnKod,
        stnSymbol,
        stnNazwa,
        usrDataWaznoscHaslo,
        srlID,
        srlNazwa,
        usrGuid,
        usrToken,
        ROW_NUMBER() OVER (PARTITION BY usrID ORDER BY usrID) AS rowNumber
    FROM mvw.vUzytkownik
)
SELECT
    u.usrID,
    ISNULL(u.usrPracownikID, 0) AS usrPracownikID,
    d.prcKod,
    u.usrNazwa,
    u.usrNazwaAD,
    u.usrImie,
    u.usrNazwisko,
    u.usrImieNazwisko,
    u.usrInicjal,
    u.usrJezyk,
    u.usrTelefon,
    d.usrKomorka,
    u.usrEmail,
    ISNULL(d.usrDataWaznoscHaslo, ISNULL(u.usrDataWaznoscHaslo, CONVERT(datetime, '20991231', 112))) AS usrDataWaznoscHaslo,
    u.usrNowy,
    u.usrBlokada,
    u.usrEdycja,
    u.usrStatus,
    ISNULL(d.appID, 0) AS appID,
    d.appFolder,
    d.usrFolder,
    d.usrZdjecie,
    d.wdzID,
    d.wdzKod,
    d.wdzSymbol,
    d.wdzNazwa,
    d.stnKod,
    d.stnSymbol,
    d.stnNazwa,
    d.srlID,
    d.srlNazwa,
    d.usrGuid,
    d.usrToken
FROM dbo.Uzytkownik u
LEFT JOIN details d ON u.usrID = d.usrID AND d.rowNumber = 1
WHERE ISNULL(u.Usuniety, 0) = 0;
GO
