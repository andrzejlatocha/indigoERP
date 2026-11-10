/* 01. Status kont P i T; JAPET.indigoERP. Skrypt nie uruchamia synchronizacji.
   Podgląd: EXEC dbo.aspSynchronizujKontaEpicaHR @Wykonaj=0;
   Zapis: EXEC dbo.aspSynchronizujKontaEpicaHR @Wykonaj=1;
   Aktywny angaż odblokowuje również konto zablokowane ręcznie lub bez nazwy AD. */
IF DB_NAME() <> N'indigoERP' THROW 50000, 'Wymagana baza indigoERP.', 1;
IF OBJECT_ID(N'dbo.aspSynchronizujKontaEpicaHR', N'P') IS NULL THROW 50001, 'Brak procedury synchronizacji.', 1;
IF COL_LENGTH(N'mvw.vPracownikKontoSynchronizacja', N'prcTypUmowaKOD') IS NULL THROW 50002, 'Brak źródła P/T.', 1;
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

          INSERT INTO #Plan (prcID, usrID, usrNazwa, Akcja, Powod)
          SELECT p.prcID, u.usrID, u.usrNazwa, ''BLOKUJ'', N''Brak aktywnego angażu na bieżący dzień.''
          FROM #Pracownik AS p INNER JOIN dbo.Uzytkownik AS u WITH (UPDLOCK, HOLDLOCK) ON u.usrPracownikID = p.prcID
          WHERE p.prcAktywny = 0 AND ISNULL(u.Usuniety, 0) = 0
            AND (ISNULL(u.usrBlokada, 0) <> 1 OR ISNULL(u.usrStatus, '''') <> ''1'');

          INSERT INTO #Plan (prcID, usrID, usrNazwa, Akcja, Powod)
          SELECT p.prcID, u.usrID, u.usrNazwa, ''ODBLOKUJ'', N''Aktywny angaż na bieżący dzień.''
          FROM #Pracownik AS p INNER JOIN dbo.Uzytkownik AS u WITH (UPDLOCK, HOLDLOCK) ON u.usrPracownikID = p.prcID
          WHERE p.prcAktywny = 1 AND ISNULL(u.Usuniety, 0) = 0
            AND (ISNULL(u.usrBlokada, 1) <> 0 OR ISNULL(u.usrStatus, '''') <> ''2'');

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
						  , 0
						  , 1
						  , 1
						  , ''2''
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
SELECT OBJECT_ID(N'dbo.aspSynchronizujKontaEpicaHR', N'P') AS ProceduraID;
