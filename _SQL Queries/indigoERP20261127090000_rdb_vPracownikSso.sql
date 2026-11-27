/*
  indigoERP - rdb.vPracownikSso: widok dla logowania SSO, wskazuje na epicaHR.sha.vPracownikSso.
  Istniejący rdb.vPracownikLista pozostaje bez zmian. Wymaga wcześniej skryptu epicaHR20261127090000_sha_vPracownikSso.sql.
  Uruchamia użytkownik w SSMS na JAPET (po kopii bazy).
*/
SET NOCOUNT ON;
GO
IF DB_NAME() <> N'indigoERP'
BEGIN
    DECLARE @baza sysname = DB_NAME();
    RAISERROR(N'Skrypt przeznaczony dla bazy indigoERP (bieżąca: %s).', 16, 1, @baza);
    SET NOEXEC ON;
END
GO
IF OBJECT_ID(N'epicaHR.sha.vPracownikSso', N'V') IS NULL
BEGIN
    RAISERROR(N'Brak epicaHR.sha.vPracownikSso - najpierw skrypt epicaHR. Przerwano.', 16, 1);
    SET NOEXEC ON;
END
GO
CREATE OR ALTER VIEW [rdb].[vPracownikSso]
AS
SELECT vps.prcID, vps.angStatus, vps.wdzID, vps.wdzKod, vps.wdzSymbol, vps.wdzNazwa,
       vps.stnKod, vps.stnSymbol, vps.stnNazwa, vps.prcZdjecie
FROM epicaHR.sha.vPracownikSso AS vps;
GO
SET NOEXEC OFF;
GO