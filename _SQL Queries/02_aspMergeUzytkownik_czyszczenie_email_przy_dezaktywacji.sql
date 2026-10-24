-- 02: aspMergeUzytkownik - przy dezaktywacji konta (status 1) czyścimy też adres e-mail (usrEmail = '').
-- Treść procedury pochodzi z ŻYWEJ definicji z JAPET/indigoERP (OBJECT_DEFINITION, 2026-10-15); jedyna zmiana to dodana
-- linia "usrEmail = ''" w pierwszym UPDATE (dezaktywacja). CREATE PROCEDURE -> CREATE OR ALTER PROCEDURE.
-- Po powrocie pracownika (status 2) adres jest pusty - uzupełnia go okno "Dane z M365" na stronie Użytkownicy.
-- Przed uruchomieniem porównać z aktualną definicją na serwerze (zgodnie z zasadą: ALTER tylko z żywej definicji).

USE indigoERP;
GO
CREATE OR ALTER PROCEDURE [dbo].[aspMergeUzytkownik]
AS
	BEGIN
		--MERGE INTO dbo.Uzytkownik AS T
		--USING rdb.vPracownikSynchronizacja AS S
		--ON t.usrPracownikID = s.usrPracownikID
		--	WHEN MATCHED AND t.usrPracownikID IS NOT NULL
		--					 OR t.usrNazwa <> s.usrNazwa
		--					 --OR t.usrNazwaAD <> s.usrNazwaAD
		--					 OR t.usrImie <> s.usrImie
		--					 OR t.usrNazwisko <> s.usrNazwisko
		--					 OR t.usrJezyk <> s.usrJezyk
		--					 OR t.usrTelefon <> s.usrTelefon
		--					 OR t.usrEmail <> s.usrEmail
		--	  THEN UPDATE SET 
		--					  t.usrNazwa = s.usrNazwa
		--					  --, t.usrNazwaAD = s.usrNazwaAD
		--					, t.usrImie = s.usrImie
		--					, t.usrNazwisko = s.usrNazwisko
		--					, t.usrPracownikID = s.usrPracownikID
		--					, t.usrJezyk = s.usrJezyk
		--					, t.usrTelefon = s.usrTelefon
		--					, t.usrEmail = s.usrEmail
		--	WHEN NOT MATCHED BY TARGET
		--	  THEN
		--	  INSERT(usrNazwa
		--		   , usrNazwaAD
		--		   , usrImie
		--		   , usrNazwisko
		--		   , usrPracownikID
		--		   , usrJezyk
		--		   , usrTelefon
		--		   , usrEmail
		--		   , usrHaslo
		--		   , usrDataWaznoscHaslo
		--		   , usrNowy
		--		   , usrZmianaHaslo
		--		   , usrBlokada
		--		   , usrEdycja
		--		   , usrSynchronizacjaAD
		--		   , usrGuidAD
		--		   , usrStatus)
		--	  VALUES ( s.usrNazwa
		--			 , NULL 
		--			 , s.usrImie
		--			 , s.usrNazwisko
		--			 , s.usrPracownikID
		--			 , s.usrJezyk
		--			 , s.usrTelefon
		--			 , s.usrEmail
		--			 , s.usrHaslo
		--			 , s.usrDataWaznoscHaslo
		--			 , 0
		--			 , 0
		--			 , 0
		--			 , 1
		--			 , 1
		--			 , s.usrGuidAD
		--			 , s.usrStatus );

		INSERT INTO dbo.Uzytkownik
			   ( usrNazwa
			   , usrNazwaAD
			   , usrImie
			   , usrNazwisko
			   , usrPracownikID
			   , usrJezyk
			   , usrTelefon
			   , usrEmail
			   , usrHaslo
			   , usrDataWaznoscHaslo
			   , usrNowy
			   , usrZmianaHaslo
			   , usrBlokada
			   , usrEdycja
			   , usrSynchronizacjaAD
			   , usrGuidAD
			   , usrStatus ) 
			   SELECT t.usrNazwa
					, NULL
					, t.usrImie
					, t.usrNazwisko
					, t.usrPracownikID
					, t.usrJezyk
					, t.usrTelefon
					, t.usrEmail
					, t.usrHaslo
					, t.usrDataWaznoscHaslo
					, 0
					, 0
					, 0
					, 1
					, 1
					, s.usrGuidAD
					, s.usrStatus
			   FROM rdb.vPracownikSynchronizacja AS t LEFT OUTER JOIN dbo.Uzytkownik AS s ON t.usrPracownikID = s.usrPracownikID
			   WHERE s.usrID IS NULL
				    AND t.usrNazwa IS NOT NULL;

		UPDATE dbo.Uzytkownik
		  SET 
			  usrStatus = '1'
			, usrBlokada = 1
			, usrEmail = '' -- konto nieaktywne nie ma adresu e-mail (reguła: status 1)
			, DataModyfikacja = GETDATE()
		WHERE NOT EXISTS
					   (
							 SELECT *
							 FROM rdb.vPracownikSynchronizacja AS vps
							 WHERE dbo.Uzytkownik.usrPracownikID = vps.usrPracownikID
					   )
			  AND usrSynchronizacjaAD = 1
			  AND usrBlokada = 0
			  AND ISNULL(usrPracownikID, 0) > 0;

		UPDATE dbo.Uzytkownik
		SET usrStatus = '2'
		  , usrBlokada = 0
		  , DataModyfikacja = GETDATE()
		WHERE EXISTS 
				    (
					 SELECT *
					 FROM rdb.vPracownikSynchronizacja AS vps
					 WHERE dbo.Uzytkownik.usrPracownikID = vps.usrPracownikID
				    )
		AND usrSynchronizacjaAD = 1
		AND usrBlokada = 1
		AND ISNULL(usrPracownikID, 0) > 0;

		--dodaj użytkowników do grupy wszyscy
		SELECT g.groID
			, u.usrID
		INTO #Grupa
		FROM dbo.Uzytkownik AS u
		   , dbo.Grupa AS g
		WHERE g.groGrupaWszyscy = 1;

		INSERT INTO dbo.GrupaCzlonkowie
			   ( uwgGrupaNadrzednaID
			   , uwgUzytkownikID
			   , uwgDomyslna
			   , uwgStatus ) 
		  SELECT g.groID
				, g.usrID
				, 0
				, '2'
		  FROM #Grupa AS g LEFT OUTER JOIN dbo.GrupaCzlonkowie AS gc ON g.groID = gc.uwgGrupaNadrzednaID
														  AND g.usrID = gc.uwgUzytkownikID
		  WHERE gc.uwgID IS NULL;

		DROP TABLE #Grupa;
	END;
GO
