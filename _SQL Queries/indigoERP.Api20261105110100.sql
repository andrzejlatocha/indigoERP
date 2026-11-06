/* 02: synchronizacja kont z epicaHR. JAPET.indigoERP.
   Wymaga wcześniejszego wykonania epicaHR.Api20261105110000.sql.
   NIE uruchamia procedury i NIE zmienia dotychczasowego aspMergeUzytkownik ani zadań.
   Pierwsze uruchomienie: EXEC dbo.aspSynchronizujKontaEpicaHR @Wykonaj=0;
   Wykonanie jest jawne: @Wykonaj=1. Nowe konta są zablokowane, nie mają hasła ani nazwy AD;
   wymagają weryfikacji tożsamości i konfiguracji SSO przed aktywacją.
   Ponowne zatrudnienie odblokowuje wcześniej przygotowane konto; procedura nie czyści e-maila. */
IF DB_NAME() <> N'indigoERP'
    THROW 50000, 'Wymagana baza indigoERP.', 1;
IF OBJECT_ID(N'epicaHR.sha.vPracownikKontoSynchronizacja', N'V') IS NULL
    THROW 50001, 'Brak widoku epicaHR.sha.vPracownikKontoSynchronizacja.', 1;
IF SCHEMA_ID(N'rdb') IS NULL OR SCHEMA_ID(N'mvw') IS NULL OR OBJECT_ID(N'dbo.Uzytkownik', N'U') IS NULL
    OR OBJECT_ID(N'dbo.DisablePolish', N'FN') IS NULL
    THROW 50002, 'Brak wymaganych schematów, tabeli lub funkcji DisablePolish.', 1;
GO
CREATE OR ALTER VIEW rdb.vPracownikKontoSynchronizacja
AS
    SELECT prcID, prcImie, prcNazwisko, prcAktywny, prcZwolniony, prcPonownieZatrudniony, prcDataPonownegoZatrudnienia
    FROM epicaHR.sha.vPracownikKontoSynchronizacja;
GO
CREATE OR ALTER VIEW mvw.vPracownikKontoSynchronizacja
AS
    SELECT prcID, prcImie, prcNazwisko, prcAktywny, prcZwolniony, prcPonownieZatrudniony, prcDataPonownegoZatrudnienia
    FROM rdb.vPracownikKontoSynchronizacja;
GO
CREATE OR ALTER PROCEDURE dbo.aspSynchronizujKontaEpicaHR
    @Wykonaj bit = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @Wykonaj IS NULL THROW 50003, 'Określ @Wykonaj jako 0 lub 1.', 1;
    DECLARE @StartedTransaction bit = 0, @HasSavepoint bit = 0;

    BEGIN TRY
    IF @Wykonaj = 1
    BEGIN
        IF @@TRANCOUNT = 0
        BEGIN
            BEGIN TRANSACTION;
            SET @StartedTransaction = 1;
        END
        ELSE
        BEGIN
            SAVE TRANSACTION SynchronizacjaEpicaHR;
            SET @HasSavepoint = 1;
        END;
        DECLARE @lockResult int;
        EXEC @lockResult = sys.sp_getapplock @Resource = N'indigoERP.aspSynchronizujKontaEpicaHR',
             @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 0;
        IF @lockResult < 0 THROW 50005, 'Inna synchronizacja jest uruchomiona.', 1;
    END;

    CREATE TABLE #Pracownik
    (
        prcID int NOT NULL PRIMARY KEY,
        prcImie varchar(30) NULL,
        prcNazwisko varchar(50) NULL,
        prcAktywny bit NOT NULL,
        prcZwolniony bit NOT NULL,
        prcPonownieZatrudniony bit NOT NULL,
        prcDataPonownegoZatrudnienia date NULL,
        usrNazwa varchar(100) NULL
    );
    INSERT #Pracownik (prcID, prcImie, prcNazwisko, prcAktywny, prcZwolniony, prcPonownieZatrudniony,
                       prcDataPonownegoZatrudnienia, usrNazwa)
    SELECT prcID, prcImie, prcNazwisko, prcAktywny, prcZwolniony, prcPonownieZatrudniony, prcDataPonownegoZatrudnienia,
           NULLIF(dbo.DisablePolish(LOWER(LTRIM(RTRIM(prcImie)) + '.' + LTRIM(RTRIM(prcNazwisko)))), '.')
    FROM mvw.vPracownikKontoSynchronizacja;

    IF NOT EXISTS (SELECT 1 FROM #Pracownik WHERE prcAktywny = 1)
        THROW 50004, 'Brak aktywnych pracowników w źródle; synchronizacja przerwana.', 1;

    CREATE TABLE #Plan
    (
        prcID int NOT NULL, usrID int NULL, usrNazwa varchar(100) NULL,
        Akcja varchar(24) NOT NULL, Powod nvarchar(300) NOT NULL
    );

    INSERT #Plan (prcID, usrID, usrNazwa, Akcja, Powod)
    SELECT p.prcID, NULL, p.usrNazwa,
           CASE
             WHEN NULLIF(LTRIM(RTRIM(p.prcImie)), '') IS NULL OR NULLIF(LTRIM(RTRIM(p.prcNazwisko)), '') IS NULL
                  OR p.usrNazwa IS NULL THEN 'POMIN_BRAK_DANYCH'
             WHEN EXISTS (SELECT 1 FROM #Pracownik d WHERE d.prcID <> p.prcID AND d.prcAktywny = 1 AND d.usrNazwa = p.usrNazwa)
                  OR EXISTS (SELECT 1 FROM dbo.Uzytkownik u WITH (UPDLOCK, HOLDLOCK)
                             WHERE u.usrNazwa = p.usrNazwa OR u.usrNazwaAD = p.usrNazwa) THEN 'POMIN_KOLIZJA'
             ELSE 'DODAJ'
           END,
           N'Aktywny pracownik bez konta; nazwa według dotychczasowej reguły imię.nazwisko.'
    FROM #Pracownik AS p
    WHERE p.prcAktywny = 1
      AND NOT EXISTS (SELECT 1 FROM dbo.Uzytkownik u WITH (UPDLOCK, HOLDLOCK) WHERE u.usrPracownikID = p.prcID);

    INSERT #Plan (prcID, usrID, usrNazwa, Akcja, Powod)
    SELECT p.prcID, u.usrID, u.usrNazwa, 'BLOKUJ', N'Zwolniony angaż; brak aktywnego lub oczekującego angażu.'
    FROM #Pracownik p
    INNER JOIN dbo.Uzytkownik u WITH (UPDLOCK, HOLDLOCK) ON u.usrPracownikID = p.prcID
    WHERE p.prcZwolniony = 1 AND ISNULL(u.Usuniety, 0) = 0
      AND (ISNULL(u.usrBlokada, 0) = 0 OR ISNULL(u.usrStatus, '') <> '1');

    INSERT #Plan (prcID, usrID, usrNazwa, Akcja, Powod)
    SELECT p.prcID, u.usrID, u.usrNazwa, 'ODBLOKUJ',
           N'Bieżący angaż rozpoczął się po zwolnieniu i po ostatniej zmianie konta; konto ma nazwę AD lub hasło.'
    FROM #Pracownik p
    INNER JOIN dbo.Uzytkownik u WITH (UPDLOCK, HOLDLOCK) ON u.usrPracownikID = p.prcID
    WHERE p.prcPonownieZatrudniony = 1 AND ISNULL(u.Usuniety, 0) = 0
      AND u.usrSynchronizacjaAD = 1 AND u.usrBlokada = 1 AND u.usrStatus = '1'
      AND u.DataModyfikacja IS NOT NULL AND u.DataModyfikacja < CONVERT(datetime, p.prcDataPonownegoZatrudnienia)
      AND (NULLIF(LTRIM(RTRIM(u.usrNazwaAD)), '') IS NOT NULL OR u.usrHaslo IS NOT NULL);

    INSERT #Plan (prcID, usrID, usrNazwa, Akcja, Powod)
    SELECT p.prcID, u.usrID, u.usrNazwa, 'POMIN_ODBLOKOWANIE',
           N'Ponowne zatrudnienie: nie można bezpiecznie ustalić starszej blokady lub gotowości konta do logowania.'
    FROM #Pracownik p INNER JOIN dbo.Uzytkownik u WITH (UPDLOCK, HOLDLOCK) ON u.usrPracownikID = p.prcID
    WHERE p.prcPonownieZatrudniony = 1 AND ISNULL(u.Usuniety, 0) = 0
      AND (u.usrBlokada = 1 OR u.usrStatus = '1')
      AND NOT EXISTS (SELECT 1 FROM #Plan pl WHERE pl.usrID = u.usrID AND pl.Akcja = 'ODBLOKUJ');

    IF @Wykonaj = 1
    BEGIN
        INSERT dbo.Uzytkownik
            (usrNazwa, usrNazwaAD, usrImie, usrNazwisko, usrPracownikID, usrJezyk, usrHaslo,
             usrNowy, usrZmianaHaslo, usrBlokada, usrEdycja, usrSynchronizacjaAD, usrStatus)
        SELECT p.usrNazwa, NULL, p.prcImie, p.prcNazwisko, p.prcID, 'pl-PL', NULL,
               0, 0, 1, 1, 1, '1'
        FROM #Plan AS pl INNER JOIN #Pracownik p ON p.prcID = pl.prcID
        WHERE pl.Akcja = 'DODAJ';

        UPDATE u SET u.usrBlokada = 1, u.usrStatus = '1', u.DataModyfikacja = GETDATE()
        FROM dbo.Uzytkownik u INNER JOIN #Plan AS pl ON pl.usrID = u.usrID
        WHERE pl.Akcja = 'BLOKUJ';

        UPDATE u SET u.usrBlokada = 0, u.usrStatus = '2', u.DataModyfikacja = GETDATE()
        FROM dbo.Uzytkownik u INNER JOIN #Plan AS pl ON pl.usrID = u.usrID
        WHERE pl.Akcja = 'ODBLOKUJ';

        IF @StartedTransaction = 1 COMMIT TRANSACTION;
    END;

    SELECT Akcja, COUNT(*) AS Liczba FROM #Plan GROUP BY Akcja ORDER BY Akcja;
    SELECT prcID, usrID, usrNazwa, Akcja, Powod FROM #Plan ORDER BY Akcja, prcID, usrID;
    END TRY
    BEGIN CATCH
        IF @StartedTransaction = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        ELSE IF @HasSavepoint = 1 AND XACT_STATE() = 1 ROLLBACK TRANSACTION SynchronizacjaEpicaHR;
        THROW;
    END CATCH;
END;
GO
SELECT OBJECT_ID(N'dbo.aspSynchronizujKontaEpicaHR', N'P') AS ProceduraID,
       OBJECT_ID(N'mvw.vPracownikKontoSynchronizacja', N'V') AS WidokID;
GO
