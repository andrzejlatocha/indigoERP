SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- Przywraca zgodność mvw.vUzytkownikLista z UzytkownikListaViewModel.
-- usrNazwaAD istnieje w dbo.Uzytkownik i jest używane przez Administrator/Uzytkownik.
USE indigoERP;
GO

CREATE OR ALTER VIEW mvw.vUzytkownikLista
AS
WITH details AS
(
    SELECT usrID, usrPracownikID, prcKod, usrNazwa, usrImie, usrNazwisko, usrInicjal, usrTelefon, usrKomorka, usrEmail,
        appID, appFolder, usrFolder, usrZdjecie, wdzID, wdzKod, wdzSymbol, wdzNazwa, stnKod, stnSymbol, stnNazwa,
        usrDataWaznoscHaslo, srlID, srlNazwa, usrGuid, usrToken,
        ROW_NUMBER() OVER (PARTITION BY usrID ORDER BY usrID) AS rowNumber
    FROM mvw.vUzytkownik
)
SELECT u.usrID, ISNULL(u.usrPracownikID, 0) AS usrPracownikID, d.prcKod, u.usrNazwa, u.usrNazwaAD, u.usrImie,
    u.usrNazwisko, u.usrImieNazwisko, u.usrInicjal, u.usrJezyk, u.usrTelefon, d.usrKomorka, u.usrEmail,
    ISNULL(d.usrDataWaznoscHaslo, ISNULL(u.usrDataWaznoscHaslo, CONVERT(DATETIME, '20991231', 112))) AS usrDataWaznoscHaslo,
    u.usrNowy, u.usrBlokada, u.usrEdycja, u.usrStatus, ISNULL(d.appID, 0) AS appID, d.appFolder, d.usrFolder, d.usrZdjecie,
    d.wdzID, d.wdzKod, d.wdzSymbol, d.wdzNazwa, d.stnKod, d.stnSymbol, d.stnNazwa, d.srlID, d.srlNazwa, d.usrGuid,
    d.usrToken,
    CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM app.LoginBlokada AS lgb
        WHERE lgb.lgbUsrNazwa = u.usrNazwa
            AND lgb.lgbDataOdblokowania IS NULL
            AND lgb.lgbDataOd <= GETDATE()
            AND lgb.lgbDataDo >= GETDATE()
    ) THEN 1 ELSE 0 END AS BIT) AS usrZablokowanyLogowaniem
FROM dbo.Uzytkownik AS u
LEFT JOIN details AS d ON u.usrID = d.usrID AND d.rowNumber = 1
WHERE ISNULL(u.Usuniety, 0) = 0;
GO
