/*
  indigoERP - uspGetUzytkownikSso: logowanie SSO czyta rdb.vPracownikSso (aktywny angaż) zamiast rdb.vPracownikLista.
  Jedyna zmiana: źródło złączenia vehpl. Wymaga indigoERP20261127090000_rdb_vPracownikSso.sql. Uruchamia użytkownik w SSMS na JAPET.
*/
SET NOCOUNT ON;
GO
CREATE OR ALTER PROCEDURE [dbo].[uspGetUzytkownikSso] 
	@usrNazwa VARCHAR(50)
   , @sesKomputer VARCHAR(100) = 'N/A'
   , @appID INT = 0
AS
    BEGIN
	   DECLARE @sesGuid VARCHAR(36);
	   SET @sesGuid = CAST(NEWID() AS VARCHAR(36));

	   SELECT DISTINCT 
			u.usrID
		   , ISNULL(vehpl.prcID, 0) AS usrPracownikID
		   , u.usrNazwa
		   , u.usrImie
		   , u.usrNazwisko
		   , u.usrInicjal
		   , ISNULL(u.usrEmail, 'brak@poczta.com') AS usrEmail
		   , ISNULL(u.usrTelefon, 'brak') AS usrTelefon
		   , ISNULL(u.usrKomorka, 'brak') AS usrKomorka
		   , a.appID
		   , a.appFolder
		   , UPPER(REPLACE(u.rowguid, '-', '')) AS usrFolder
		   , CAST(ISNULL(vehpl.prcZdjecie, 'empty.png') AS VARCHAR(4000)) AS usrZdjecie
		   , ISNULL(ISNULL(vehpl.wdzID, usrWydzialID), 0) AS wdzID
		   , ISNULL(vehpl.wdzKod, '00') AS wdzKod
		   , ISNULL(vehpl.wdzSymbol, '') AS wdzSymbol
		   , ISNULL(vehpl.wdzNazwa, '') AS wdzNazwa
		   , ISNULL(vehpl.stnKod, '') AS stnKod
		   , ISNULL(vehpl.stnSymbol, '') AS stnSymbol
		   , ISNULL(vehpl.stnNazwa, 'Pracownik obcy') AS stnNazwa
		   , ISNULL(u.usrDataWaznoscHaslo, '2099-12-31') AS usrDataWaznoscHaslo
		   , CASE
			    WHEN a.appNazwa = vur.srlNazwa THEN ''
			    ELSE a.appNazwa + '|' + vur.srlNazwa
			END AS srlNazwa
		   , vur.srlID
		   , u.rowguid AS usrGuid
		   , '???' AS usrToken
	   INTO #Uzytkownik
	   FROM dbo.Uzytkownik AS u INNER JOIN app.vUzytkownikRolaGrupowane AS vur ON u.usrID = vur.usrID
																   AND u.Usuniety = 0
																   AND vur.appID = @appID
						   INNER JOIN dbo.Aplikacja AS a ON vur.appID = a.appID
						   LEFT OUTER JOIN rdb.vPracownikSso AS vehpl ON u.usrPracownikID = vehpl.prcID
																 AND vehpl.angStatus = '2'
	   WHERE(u.usrNazwa = @usrNazwa
		    OR u.usrNazwaAD = @usrNazwa)
		   AND u.usrBlokada = 0
		   AND u.usrStatus = '2';

	   DECLARE @wiersz INT = @@ROWCOUNT;
	   DECLARE @usrID INT;

	   SELECT TOP (1) @usrID = u.usrID
	   FROM #Uzytkownik AS u
	   ORDER BY NEWID();

	   INSERT INTO dbo.Sesja
			( sesAplikacjaID
			, sesUzytkownikID
			, sesGuid
			, sesData
			, sesOpis
			, sesUzytkownikNazwa
			, sesKomputer
			, sesWersja
			, sesWynik
			, sesStatus
			)
	   VALUES
			( @appID
			, @usrID
			, @sesGuid
			, GETDATE()
			, 'Logowanie SSO (trusted subsystem, uspGetUzytkownikSso)'
			, @usrNazwa
			, @sesKomputer
			, '3.0'
			, CASE
				 WHEN @wiersz = 1 THEN 'Logowanie prawidlowe'
				 WHEN @wiersz = 0 THEN 'Logowanie nieprawidlowe'
				 WHEN @wiersz > 1 THEN 'Za duzo rekordow'
				 ELSE 'Blad'
			  END
			, CASE
				 WHEN @wiersz = 1 THEN '2'
				 WHEN @wiersz = 0 THEN '1'
				 WHEN @wiersz > 1 THEN '1'
				 ELSE '0'
			  END
			);

	   IF @wiersz = 1
		  BEGIN
			 SELECT u.usrID
				 , u.usrPracownikID
				 , u.usrNazwa
				 , u.usrImie
				 , u.usrNazwisko
				 , u.usrInicjal
				 , u.usrTelefon
				 , u.usrKomorka
				 , u.usrEmail
				 , 0 AS usrKlientID
				 , 0 AS usrDostawcaID
				 , 0 AS usrOdbiorcaID
				 , 0 AS usrSpedytorID
				 , u.appID
				 , u.appFolder
				 , u.usrFolder
				 , u.usrZdjecie
				 , u.wdzID
				 , u.wdzKod
				 , u.wdzSymbol
				 , u.wdzNazwa
				 , u.stnKod
				 , u.stnSymbol
				 , u.stnNazwa
				 , u.usrDataWaznoscHaslo
				 , u.srlID
				 , u.srlNazwa
				 , u.usrGuid
				 , u.usrToken
				 , @sesGuid AS sesGuid
				 , @wiersz
			 FROM #Uzytkownik AS u;
		  END;
		  ELSE
		  BEGIN
			 SELECT-1 AS usrID
				, -1 AS usrPracownikID
				, '---------------------------------------------' AS usrNazwa
				, '' AS usrImie
				, '' AS usrNazwisko
				, '' AS usrInicjal
				, '' AS usrTelefon
				, '' AS usrKomorka
				, 'brak@poczta.com' AS usrEmail
				, 0 AS usrKlientID
				, 0 AS usrDostawcaID
				, 0 AS usrOdbiorcaID
				, 0 AS usrSpedytorID
				, @appID AS appID
				, UPPER(REPLACE(NEWID(), '-', '')) AS appFolder
				, UPPER(REPLACE(NEWID(), '-', '')) AS usrFolder
				, 'empty.jpg' AS usrZdjecie
				, 0 AS wdzID
				, '' AS wdzKod
				, '' AS wdzSymbol
				, '' AS wdzNazwa
				, '' AS stnKod
				, '' AS stnSymbol
				, 'Pracownik obcy' AS stnNazwa
				, CAST(GETDATE() AS DATE) AS usrDataWaznoscHaslo
				, '' AS srlID
				, '' AS srlNazwa
				, NEWID() AS usrGuid
				, '???' AS usrToken
				, '00000000-0000-0000-0000-000000000000' AS sesGuid
				, @wiersz;
		  END;
    END;

GO
