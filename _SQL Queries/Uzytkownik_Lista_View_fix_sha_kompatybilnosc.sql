-- Domyka naprawę mvw.vUzytkownikLista (Uzytkownik_Lista_View_reapply.sql).
-- Po przywróceniu poprawnej, deduplikowanej wersji mvw.vUzytkownikLista okazało się, że warstwa
-- sha.vUzytkownikLista (a przez nią atacamaDMS.rdb.vUzytkownikLista) odwołuje się na sztywno do
-- kolumn usrNazwaAD i groAplikacjaID, które istniały w STAREJ, wadliwej wersji widoku, a których
-- nowa wersja nie ma (ma appID zamiast groAplikacjaID, a usrNazwaAD w ogóle nie wystawiała).
-- Efekt: "Invalid column name 'usrNazwaAD'" / "'groAplikacjaID'" przy odpytywaniu
-- atacamaDMS.rdb.vUzytkownikLista / mvw.vZamowienie.
--
-- Ten skrypt NIE zmienia atacamaDMS.rdb.vUzytkownikLista (ta warstwa ma zostać dokładnie taka,
-- jaka jest - już sprawdzona dla Zapotrzebowań/Zamówień). Naprawia tylko dwie warstwy w indigoERP:
--   1) mvw.vUzytkownikLista - dokłada brakującą kolumnę usrNazwaAD (jest w dbo.Uzytkownik)
--   2) sha.vUzytkownikLista - mapuje nową kolumnę appID z powrotem na starą nazwę groAplikacjaID,
--      żeby rdb.vUzytkownikLista dostawało dokładnie te same nazwy kolumn co wcześniej.
-- Uruchomić na CERES/indigoERP, po Uzytkownik_Lista_View_reapply.sql.

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

CREATE OR ALTER VIEW [sha].[vUzytkownikLista]
AS
	 SELECT vul.usrID
		  , vul.usrPracownikID
		  , vul.usrNazwa
		  , vul.usrNazwaAD
		  , vul.usrImie
		  , vul.usrNazwisko
		  , vul.usrEmail
		  , vul.usrBlokada
		  , vul.usrStatus
		  , vul.appID AS groAplikacjaID
		  , vul.usrFolder
		  , vul.usrZdjecie
		  , vul.wdzKod
		  , vul.wdzSymbol
		  , vul.wdzNazwa
		  , vul.stnKod
		  , vul.stnSymbol
		  , vul.stnNazwa
		  , vul.usrGuid
	 FROM mvw.vUzytkownikLista AS vul;
GO
