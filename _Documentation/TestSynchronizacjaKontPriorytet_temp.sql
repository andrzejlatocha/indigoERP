/* Test izolowany: tworzy wyłącznie lokalne obiekty # w tempdb w ramach tej sesji.
   Używa projektowanej definicji procedury z priorytetem P i zastępuje tylko nazwy źródła,
   tabeli docelowej i zasobu applock ich odpowiednikami tymczasowymi.
   Nie używa @Wykonaj=1 na produkcyjnej tabeli Uzytkownik i nie zużywa jej IDENTITY.
   Wyniki procedury zawierają dane osobowe: nie zapisuj ich do pliku publicznego.
   Trigger produkcyjny nie jest uruchamiany na tabeli tymczasowej. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
IF DB_NAME() <> N'indigoERP' THROW 50000, 'Wymagana baza indigoERP.', 1;
IF OBJECT_ID(N'dbo.aspSynchronizujKontaEpicaHR', N'P') IS NULL THROW 50001, 'Brak procedury synchronizacji.', 1;
DECLARE @KontaPrzed int, @KontaHashPrzed int, @GrupyPrzed bigint, @IdentityPrzed numeric(38,0);
SELECT @KontaPrzed = COUNT(*),
       @KontaHashPrzed = CHECKSUM_AGG(BINARY_CHECKSUM(usrID, usrPracownikID, usrNazwa, usrBlokada, usrStatus, DataModyfikacja))
FROM dbo.Uzytkownik;
SELECT @GrupyPrzed = COUNT_BIG(*) FROM dbo.GrupaCzlonkowie;
SELECT @IdentityPrzed = IDENT_CURRENT('dbo.Uzytkownik');

SELECT p.prcID
	   , p.prcImie
	   , p.prcNazwisko
	   , CONVERT(BIT,
			   CASE
				  WHEN EXISTS
   (
	  SELECT 1
	  FROM epicaHR.dbo.Angaz AS a
	  WHERE a.angPracownikID = p.prcID
		   AND a.Usuniety = 0
		   AND a.angStatus = '2'
		   AND a.angDataRozpoczecie <= CONVERT(DATE, GETDATE())
		   AND (a.angDataZakonczenie IS NULL
			   OR a.angDataZakonczenie >= CONVERT(DATE, GETDATE()))
   ) THEN 1
				  ELSE 0
			   END) AS prcAktywny
	   , CONVERT(BIT,
			   CASE
				  WHEN EXISTS
   (
	  SELECT 1
	  FROM epicaHR.dbo.Angaz AS a
	  WHERE a.angPracownikID = p.prcID
		   AND a.Usuniety = 0
		   AND a.angStatus = '5'
		   AND a.angDataZakonczenie < CONVERT(DATE, GETDATE())
   )
					  AND NOT EXISTS
   (
	  SELECT 1
	  FROM epicaHR.dbo.Angaz AS a
	  WHERE a.angPracownikID = p.prcID
		   AND a.Usuniety = 0
		   AND a.angStatus IN('2', '9')
   ) THEN 1
				  ELSE 0
			   END) AS prcZwolniony
	   , CONVERT(BIT,
			   CASE
				  WHEN EXISTS
   (
	  SELECT 1
	  FROM epicaHR.dbo.Angaz AS aktualny
	  WHERE aktualny.angPracownikID = p.prcID
		   AND aktualny.Usuniety = 0
		   AND aktualny.angStatus = '2'
		   AND aktualny.angDataRozpoczecie <= CONVERT(DATE, GETDATE())
		   AND (aktualny.angDataZakonczenie IS NULL
			   OR aktualny.angDataZakonczenie >= CONVERT(DATE, GETDATE()))
		   AND EXISTS
	  (
		 SELECT 1
		 FROM epicaHR.dbo.Angaz AS poprzedni
		 WHERE poprzedni.angPracownikID = p.prcID
			  AND poprzedni.Usuniety = 0
			  AND poprzedni.angStatus = '5'
			  AND poprzedni.angDataZakonczenie < aktualny.angDataRozpoczecie
	  )
   ) THEN 1
				  ELSE 0
			   END) AS prcPonownieZatrudniony
	   , (SELECT MAX(aktualny.angDataRozpoczecie)
		 FROM epicaHR.dbo.Angaz AS aktualny
		 WHERE aktualny.angPracownikID = p.prcID
			  AND aktualny.Usuniety = 0
			  AND aktualny.angStatus = '2'
			  AND aktualny.angDataRozpoczecie <= CONVERT(DATE, GETDATE())
			  AND (aktualny.angDataZakonczenie IS NULL
				  OR aktualny.angDataZakonczenie >= CONVERT(DATE, GETDATE()))
			  AND EXISTS
   (
	  SELECT 1
	  FROM epicaHR.dbo.Angaz AS poprzedni
	  WHERE poprzedni.angPracownikID = p.prcID
		   AND poprzedni.Usuniety = 0
		   AND poprzedni.angStatus = '5'
		   AND poprzedni.angDataZakonczenie < aktualny.angDataRozpoczecie
   )) AS prcDataPonownegoZatrudnienia
   , p.prcTypUmowaKOD AS prcTypUmowaKOD
INTO #TestZrodlo FROM epicaHR.dbo.Pracownik AS p
   WHERE p.prcTypUmowaKOD IN ('P', 'T')
	    AND p.Usuniety = 0;

CREATE TABLE #TestKonto
(
    usrID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    usrNazwa varchar(100) COLLATE DATABASE_DEFAULT NULL,
    usrNazwaAD varchar(100) COLLATE DATABASE_DEFAULT NULL,
    usrImie varchar(100) COLLATE DATABASE_DEFAULT NULL,
    usrNazwisko varchar(150) COLLATE DATABASE_DEFAULT NULL,
    usrPracownikID int NULL,
    usrJezyk char(5) COLLATE DATABASE_DEFAULT NULL,
    usrHaslo varbinary(250) NULL,
    usrNowy bit NULL DEFAULT (1),
    usrZmianaHaslo bit NULL DEFAULT (0),
    usrBlokada bit NULL DEFAULT (0),
    usrEdycja bit NULL DEFAULT (1),
    usrSynchronizacjaAD bit NULL DEFAULT (0),
    usrStatus char(1) COLLATE DATABASE_DEFAULT NULL DEFAULT ('2'),
    DataModyfikacja datetime NULL DEFAULT (GETDATE()),
    Usuniety bit NULL DEFAULT (0)
);
SET IDENTITY_INSERT #TestKonto ON;
INSERT #TestKonto (usrID, usrNazwa, usrNazwaAD, usrImie, usrNazwisko, usrPracownikID, usrJezyk, usrHaslo,
                   usrNowy, usrZmianaHaslo, usrBlokada, usrEdycja, usrSynchronizacjaAD, usrStatus, DataModyfikacja, Usuniety)
SELECT usrID, usrNazwa, usrNazwaAD, usrImie, usrNazwisko, usrPracownikID, usrJezyk, usrHaslo,
       usrNowy, usrZmianaHaslo, usrBlokada, usrEdycja, usrSynchronizacjaAD, usrStatus, DataModyfikacja, Usuniety
FROM dbo.Uzytkownik;
SET IDENTITY_INSERT #TestKonto OFF;
SELECT usrID, usrPracownikID, usrNazwa, usrBlokada, usrStatus, Usuniety INTO #StanPrzed FROM #TestKonto;

DECLARE @def nvarchar(max) = N'CREATE PROCEDURE dbo.aspSynchronizujKontaEpicaHR 
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
    END;';
IF @def IS NULL OR CHARINDEX(N'@Wykonaj', @def) = 0
    THROW 50002, 'Brak dostępu do definicji lub nieoczekiwany format procedury.', 1;
IF @def NOT LIKE N'%mvw.vPracownikKontoSynchronizacja%' OR @def NOT LIKE N'%dbo.Uzytkownik%'
    THROW 50003, 'Nieoczekiwane odwołania procedury; test przerwany.', 1;
SET @def = N'CREATE PROCEDURE #TestSynchronizacji ' + SUBSTRING(@def, CHARINDEX(N'@Wykonaj', @def), LEN(@def));
SET @def = REPLACE(@def, N'mvw.vPracownikKontoSynchronizacja', N'#TestZrodlo');
SET @def = REPLACE(@def, N'dbo.Uzytkownik', N'#TestKonto');
SET @def = REPLACE(@def, N'indigoERP.aspSynchronizujKontaEpicaHR', N'indigoERP.TestSynchronizacji.Tymczasowy');
IF @def LIKE N'%dbo.Uzytkownik%' OR @def LIKE N'%mvw.vPracownikKontoSynchronizacja%'
    THROW 50004, 'Nie zastąpiono odwołań docelowych; test przerwany.', 1;
EXEC sys.sp_executesql @def;

PRINT 'FAZA_PODGLAD';
EXEC #TestSynchronizacji @Wykonaj = 0;
PRINT 'FAZA_ZAPIS_TYMCZASOWY';
EXEC #TestSynchronizacji @Wykonaj = 1;

SELECT 'TEST_NOWE' AS Test, COUNT(*) AS Liczba
FROM #TestKonto k LEFT JOIN #StanPrzed b ON b.usrID = k.usrID WHERE b.usrID IS NULL;
SELECT 'TEST_NOWE_BLOKADA' AS Test, COUNT(*) AS Liczba
FROM #TestKonto k LEFT JOIN #StanPrzed b ON b.usrID = k.usrID
WHERE b.usrID IS NULL AND k.usrBlokada = 1 AND k.usrStatus = '1' AND k.usrNazwaAD IS NULL AND k.usrHaslo IS NULL;
SELECT 'TEST_ZABLOKOWANE' AS Test, COUNT(*) AS Liczba
FROM #TestKonto k JOIN #StanPrzed b ON b.usrID = k.usrID
WHERE (ISNULL(b.usrBlokada,0) = 0 OR ISNULL(b.usrStatus,'') <> '1') AND k.usrBlokada = 1 AND k.usrStatus = '1';
SELECT 'TEST_ODBLOKOWANE' AS Test, COUNT(*) AS Liczba
FROM #TestKonto k JOIN #StanPrzed b ON b.usrID = k.usrID
WHERE b.usrBlokada = 1 AND b.usrStatus = '1' AND k.usrBlokada = 0 AND k.usrStatus = '2';

SELECT usrID, usrNazwa, usrNazwaAD, usrPracownikID, usrHaslo, usrBlokada, usrStatus, DataModyfikacja
INTO #StanPo1 FROM #TestKonto;
PRINT 'FAZA_POWTORZENIE';
EXEC #TestSynchronizacji @Wykonaj = 1;
SELECT 'TEST_ZMIANY_DRUGI_PRZEBIEG' AS Test, COUNT(*) AS Liczba
FROM #TestKonto k FULL JOIN #StanPo1 b ON b.usrID = k.usrID
WHERE k.usrID IS NULL OR b.usrID IS NULL OR ISNULL(CONVERT(int,k.usrBlokada), 9) <> ISNULL(CONVERT(int,b.usrBlokada), 9)
   OR ISNULL(k.usrStatus, '') <> ISNULL(b.usrStatus, '') OR ISNULL(k.usrNazwa, '') <> ISNULL(b.usrNazwa, '')
   OR ISNULL(k.usrNazwaAD, '') <> ISNULL(b.usrNazwaAD, '')
   OR ISNULL(k.usrPracownikID, -1) <> ISNULL(b.usrPracownikID, -1)
   OR ISNULL(k.DataModyfikacja, CONVERT(datetime,'19000101')) <> ISNULL(b.DataModyfikacja, CONVERT(datetime,'19000101'));
DECLARE @KontaPo int, @KontaHashPo int, @GrupyPo bigint, @IdentityPo numeric(38,0);
SELECT @KontaPo = COUNT(*),
       @KontaHashPo = CHECKSUM_AGG(BINARY_CHECKSUM(usrID, usrPracownikID, usrNazwa, usrBlokada, usrStatus, DataModyfikacja))
FROM dbo.Uzytkownik;
SELECT @GrupyPo = COUNT_BIG(*) FROM dbo.GrupaCzlonkowie;
SELECT @IdentityPo = IDENT_CURRENT('dbo.Uzytkownik');
SELECT 'TEST_PRODUKCJA_BEZ_ZMIAN' AS Test,
       CASE WHEN @KontaPrzed = @KontaPo AND ISNULL(@KontaHashPrzed,0) = ISNULL(@KontaHashPo,0)
                 AND @GrupyPrzed = @GrupyPo AND @IdentityPrzed = @IdentityPo THEN 1 ELSE 0 END AS Wynik;
PRINT 'KONIEC_TESTU';
