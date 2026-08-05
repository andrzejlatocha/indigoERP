-- mvw.vUzytkownikLista na CERES jest z powrotem starą, wadliwą wersją (DISTINCT po
-- GrupaCzlonkowie/vGrupaGrupa/vPracownikAktywnyLista, duplikuje wiersz per członkostwo w grupie,
-- brakuje kolumn appID/prcKod/srlID/srlNazwa/usrToken których oczekuje EF model
-- UzytkownikListaViewModel -> stąd 500 na indigoERP.Api Uzytkownik/Select).
-- Ten skrypt to CREATE OR ALTER wariant Uzytkownik_Lista_View.sql (ta sama, poprawna,
-- deduplikowana przez ROW_NUMBER definicja) - do uruchomienia zamiast CREATE VIEW, bo widok
-- już istnieje. Uruchomić na CERES/indigoERP.

CREATE OR ALTER VIEW mvw.vUzytkownikLista AS
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
