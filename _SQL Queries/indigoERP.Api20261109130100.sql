/* 02. Priorytet P przy zderzeniu proponowanych nazw. Cel JAPET.indigoERP. Uruchamia użytkownik.
   Wymaga epicaHR.Api20261109130000.sql. Istniejące konta nigdy nie są przepinane. */
SET XACT_ABORT ON;
IF DB_NAME() <> N'indigoERP' THROW 50000, 'Wymagana baza indigoERP.', 1;
IF OBJECT_ID(N'dbo.aspSynchronizujKontaEpicaHR',N'P') IS NULL THROW 50001, 'Brak istniejącej procedury.', 1;
IF NOT EXISTS (SELECT 1 FROM epicaHR.sys.columns WHERE object_id=OBJECT_ID(N'epicaHR.sha.vPracownikKontoSynchronizacja') AND name=N'prcTypUmowaKOD') THROW 50002, 'Wymagane wdrożenie skryptu epicaHR 01.', 1;
BEGIN TRY
BEGIN TRANSACTION;
EXEC(N'CREATE OR ALTER VIEW rdb.vPracownikKontoSynchronizacja AS SELECT prcID, prcImie, prcNazwisko, prcAktywny, prcZwolniony, prcPonownieZatrudniony, prcDataPonownegoZatrudnienia, prcTypUmowaKOD FROM epicaHR.sha.vPracownikKontoSynchronizacja');
EXEC(N'CREATE OR ALTER VIEW mvw.vPracownikKontoSynchronizacja AS SELECT prcID, prcImie, prcNazwisko, prcAktywny, prcZwolniony, prcPonownieZatrudniony, prcDataPonownegoZatrudnienia, prcTypUmowaKOD FROM rdb.vPracownikKontoSynchronizacja');
EXEC(N'CREATE OR ALTER PROCEDURE dbo.aspSynchronizujKontaEpicaHR 
	@Wykonaj BIT = 0
AS
    BEGIN
	   SET NOCOUNT ON;
	   SET XACT_ABORT ON;
	   IF @Wykonaj IS NULL
		  BEGIN
			 THROW 50003, ''Określ @Wykonaj jako 0 lub 1.'', 1;
		  END;
	   DECLARE @StartedTransaction BIT = 0
		    , @HasSavepoint BIT = 0;

	   BEGIN TRY
		  IF @Wykonaj = 1
			 BEGIN
				IF @@TRANCOUNT = 0
				    BEGIN
					   BEGIN TRANSACTION;
					   SET @StartedTransaction = 1;
				    END;
				    ELSE
				    BEGIN
					   SAVE TRANSACTION SynchronizacjaEpicaHR;
					   SET @HasSavepoint = 1;
				    END;
				DECLARE @lockResult INT;
				EXEC @lockResult = sys.sp_getapplock 
					@Resource = N''indigoERP.aspSynchronizujKontaEpicaHR''
				   , @LockMode = ''Exclusive''
				   , @LockOwner = ''Transaction''
				   , @LockTimeout = 0;
				IF @lockResult < 0
				    BEGIN
					   THROW 50005, ''Inna synchronizacja jest uruchomiona.'', 1;
				    END;
			 END;

		  CREATE TABLE #Pracownik(
			  prcID INT NOT NULL PRIMARY KEY, prcTypUmowaKOD char(1) NOT NULL
			, prcImie VARCHAR(30) NULL
			, prcNazwisko VARCHAR(50) NULL
			, prcAktywny BIT NOT NULL
			, prcZwolniony BIT NOT NULL
			, prcPonownieZatrudniony BIT NOT NULL
			, prcDataPonownegoZatrudnienia DATE NULL
			, usrNazwa VARCHAR(100) NULL);
		  INSERT INTO #Pracownik
				    ( prcID, prcTypUmowaKOD
				    , prcImie
				    , prcNazwisko
				    , prcAktywny
				    , prcZwolniony
				    , prcPonownieZatrudniony
				    , prcDataPonownegoZatrudnienia
				    , usrNazwa
				    )
			    SELECT prcID, prcTypUmowaKOD
				    , prcImie
				    , prcNazwisko
				    , prcAktywny
				    , prcZwolniony
				    , prcPonownieZatrudniony
				    , prcDataPonownegoZatrudnienia
				    , NULLIF(dbo.DisablePolish( LOWER(LTRIM(RTRIM(prcImie)) + ''.'' + LTRIM(RTRIM(prcNazwisko))) ), ''.'')
			    FROM mvw.vPracownikKontoSynchronizacja;

		  IF NOT EXISTS
		  (
			 SELECT 1
			 FROM #Pracownik
			 WHERE prcAktywny = 1
		  )
			 BEGIN
				THROW 50004, ''Brak aktywnych pracowników w źródle; synchronizacja przerwana.'', 1;
			 END;

		  CREATE TABLE #Plan(
			  prcID INT NOT NULL
			, usrID INT NULL
			, usrNazwa VARCHAR(100) NULL
			, Akcja VARCHAR(24) NOT NULL
			, Powod NVARCHAR(300) NOT NULL);

		  INSERT INTO #Plan
				    ( prcID
				    , usrID
				    , usrNazwa
				    , Akcja
				    , Powod
				    )
			    SELECT p.prcID
				    , NULL
				    , p.usrNazwa
				    , CASE
						WHEN NULLIF(LTRIM(RTRIM(p.prcImie)), '''') IS NULL
							OR NULLIF(LTRIM(RTRIM(p.prcNazwisko)), '''') IS NULL
							OR p.usrNazwa IS NULL THEN ''POMIN_BRAK_DANYCH''
						WHEN EXISTS
			    (
				   SELECT 1
				   FROM #Pracownik AS d
				   WHERE d.prcID <> p.prcID
					    AND d.prcAktywny = 1
					    AND d.usrNazwa = p.usrNazwa AND (p.prcTypUmowaKOD = ''T'' OR d.prcTypUmowaKOD = ''P'')
			    )
							OR EXISTS
			    (
				   SELECT 1
				   FROM dbo.Uzytkownik AS u WITH (UPDLOCK, HOLDLOCK)
				   WHERE u.usrNazwa = p.usrNazwa
					    OR u.usrNazwaAD = p.usrNazwa
			    ) THEN ''POMIN_KOLIZJA''
						ELSE ''DODAJ''
					 END
				    , N''Aktywny pracownik bez konta; nazwa według dotychczasowej reguły imię.nazwisko.''
			    FROM #Pracownik AS p
			    WHERE p.prcAktywny = 1
					AND NOT EXISTS
			    (
				   SELECT 1
				   FROM dbo.Uzytkownik AS u WITH (UPDLOCK, HOLDLOCK)
				   WHERE u.usrPracownikID = p.prcID
			    );

		  INSERT INTO #Plan
				    ( prcID
				    , usrID
				    , usrNazwa
				    , Akcja
				    , Powod
				    )
			    SELECT p.prcID
				    , u.usrID
				    , u.usrNazwa
				    , ''BLOKUJ''
				    , N''Zwolniony angaż; brak aktywnego lub oczekującego angażu.''
			    FROM #Pracownik AS p INNER JOIN dbo.Uzytkownik AS u WITH (UPDLOCK, HOLDLOCK) ON u.usrPracownikID = p.prcID
			    WHERE p.prcZwolniony = 1
					AND ISNULL(u.Usuniety, 0) = 0
					AND (ISNULL(u.usrBlokada, 0) = 0
						OR ISNULL(u.usrStatus, '''') <> ''1'');

		  INSERT INTO #Plan
				    ( prcID
				    , usrID
				    , usrNazwa
				    , Akcja
				    , Powod
				    )
			    SELECT p.prcID
				    , u.usrID
				    , u.usrNazwa
				    , ''ODBLOKUJ''
				    , N''Bieżący angaż rozpoczął się po zwolnieniu i po ostatniej zmianie konta; konto ma nazwę AD lub hasło.''
			    FROM #Pracownik AS p INNER JOIN dbo.Uzytkownik AS u WITH (UPDLOCK, HOLDLOCK) ON u.usrPracownikID = p.prcID
			    WHERE p.prcPonownieZatrudniony = 1
					AND ISNULL(u.Usuniety, 0) = 0
					AND u.usrSynchronizacjaAD = 1
					AND u.usrBlokada = 1
					AND u.usrStatus = ''1''
					AND u.DataModyfikacja IS NOT NULL
					AND u.DataModyfikacja < CONVERT(DATETIME, p.prcDataPonownegoZatrudnienia)
					AND (NULLIF(LTRIM(RTRIM(u.usrNazwaAD)), '''') IS NOT NULL
						OR u.usrHaslo IS NOT NULL);

		  INSERT INTO #Plan
				    ( prcID
				    , usrID
				    , usrNazwa
				    , Akcja
				    , Powod
				    )
			    SELECT p.prcID
				    , u.usrID
				    , u.usrNazwa
				    , ''POMIN_ODBLOKOWANIE''
				    , N''Ponowne zatrudnienie: nie można bezpiecznie ustalić starszej blokady lub gotowości konta do logowania.''
			    FROM #Pracownik AS p INNER JOIN dbo.Uzytkownik AS u WITH (UPDLOCK, HOLDLOCK) ON u.usrPracownikID = p.prcID
			    WHERE p.prcPonownieZatrudniony = 1
					AND ISNULL(u.Usuniety, 0) = 0
					AND (u.usrBlokada = 1
						OR u.usrStatus = ''1'')
					AND NOT EXISTS
			    (
				   SELECT 1
				   FROM #Plan AS pl
				   WHERE pl.usrID = u.usrID
					    AND pl.Akcja = ''ODBLOKUJ''
			    );

		  IF @Wykonaj = 1
			 BEGIN
				INSERT INTO dbo.Uzytkownik
						  ( usrNazwa
						  , usrNazwaAD
						  , usrImie
						  , usrNazwisko
						  , usrPracownikID
						  , usrJezyk
						  , usrHaslo
						  , usrNowy
						  , usrZmianaHaslo
						  , usrBlokada
						  , usrEdycja
						  , usrSynchronizacjaAD
						  , usrStatus
						  )
					  SELECT p.usrNazwa
						  , NULL
						  , p.prcImie
						  , p.prcNazwisko
						  , p.prcID
						  , ''pl-PL''
						  , NULL
						  , 0
						  , 0
						  , 1
						  , 1
						  , 1
						  , ''1''
					  FROM #Plan AS pl INNER JOIN #Pracownik AS p ON p.prcID = pl.prcID
					  WHERE pl.Akcja = ''DODAJ'';

				UPDATE u
				  SET 
					 u.usrBlokada = 1
				    , u.usrStatus = ''1''
				    , u.DataModyfikacja = GETDATE()
				FROM dbo.Uzytkownik u INNER JOIN #Plan AS pl ON pl.usrID = u.usrID
				WHERE pl.Akcja = ''BLOKUJ'';

				UPDATE u
				  SET 
					 u.usrBlokada = 0
				    , u.usrStatus = ''2''
				    , u.DataModyfikacja = GETDATE()
				FROM dbo.Uzytkownik u INNER JOIN #Plan AS pl ON pl.usrID = u.usrID
				WHERE pl.Akcja = ''ODBLOKUJ'';

				IF @StartedTransaction = 1
				    BEGIN
					   COMMIT TRANSACTION;
				    END;
			 END;

		  SELECT Akcja
			  , COUNT(*) AS Liczba
		  FROM #Plan
		  GROUP BY Akcja
		  ORDER BY Akcja;
		  SELECT prcID
			  , usrID
			  , usrNazwa
			  , Akcja
			  , Powod
		  FROM #Plan
		  ORDER BY Akcja
			    , prcID
			    , usrID;
	   END TRY
	   BEGIN CATCH
		  IF @StartedTransaction = 1
			AND XACT_STATE() <> 0
			 BEGIN
				ROLLBACK TRANSACTION;
			 END
			 ELSE
			 BEGIN
				IF @HasSavepoint = 1
				   AND XACT_STATE() = 1
				    BEGIN
					   ROLLBACK TRANSACTION SynchronizacjaEpicaHR;
				    END;
			 END;
		  THROW;
	   END CATCH;
    END;');
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
THROW;
END CATCH;
SELECT OBJECT_ID(N'dbo.aspSynchronizujKontaEpicaHR',N'P') AS ProceduraID, COL_LENGTH(N'mvw.vPracownikKontoSynchronizacja', N'prcTypUmowaKOD') AS KolumnaBajty;
GO
