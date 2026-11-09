/* Jednorazowe wykonanie po zatwierdzeniu użytkownika. Oczekiwane wartości są sprawdzane w transakcji.
   Wymaga zmiennych sqlcmd ExpectedCount i ExpectedHash z kopii statusów wykonanej tuż przed uruchomieniem.
   Nie zmienia definicji procedur ani zadań SQL Agent. Wyniki procedury zawierają dane osobowe. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET LOCK_TIMEOUT 10000;
IF DB_NAME() <> N'indigoERP' THROW 50100, 'Nieprawidłowa baza.', 1;
BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @n int, @hash int;
    SELECT @n = COUNT(*),
           @hash = ISNULL(CHECKSUM_AGG(BINARY_CHECKSUM(usrID, usrStatus, usrBlokada, DataModyfikacja)),0)
    FROM dbo.Uzytkownik WITH (UPDLOCK, HOLDLOCK);
    IF @n <> $(ExpectedCount) OR @hash <> $(ExpectedHash)
        THROW 50101, 'Stan kont zmienił się po wykonaniu kopii; przerwano synchronizację.', 1;

    SELECT usrID, usrPracownikID, usrStatus, usrBlokada, DataModyfikacja
    INTO #Przed FROM dbo.Uzytkownik;
    DECLARE @GrupyPrzed bigint = (SELECT COUNT_BIG(*) FROM dbo.GrupaCzlonkowie WITH (UPDLOCK, HOLDLOCK));

    EXEC dbo.aspSynchronizujKontaEpicaHR @Wykonaj = 1;

    DECLARE @Nowe int, @NoweGotowe int, @NoweGrupa int, @Zablokowane int, @Odblokowane int, @Inne int, @Zmienione int, @GrupyPo bigint;
    SELECT @Nowe = COUNT(*),
           @NoweGotowe = COALESCE(SUM(CASE WHEN u.usrBlokada = 1 AND u.usrStatus = '1'
                                                  AND u.usrNazwaAD IS NULL AND u.usrHaslo IS NULL THEN 1 ELSE 0 END),0)
    FROM dbo.Uzytkownik u LEFT JOIN #Przed b ON b.usrID = u.usrID WHERE b.usrID IS NULL;
    SELECT @NoweGrupa = COUNT(*) FROM dbo.Uzytkownik u LEFT JOIN #Przed b ON b.usrID = u.usrID
    WHERE b.usrID IS NULL AND EXISTS(SELECT 1 FROM dbo.GrupaCzlonkowie g
                                     WHERE g.uwgUzytkownikID = u.usrID AND g.uwgGrupaNadrzednaID = 0);
    SELECT @Zablokowane = COALESCE(SUM(CASE WHEN (ISNULL(b.usrBlokada,0) = 0 OR ISNULL(b.usrStatus,'') <> '1')
                                              AND u.usrBlokada = 1 AND u.usrStatus = '1'
                                              AND p.prcZwolniony = 1 THEN 1 ELSE 0 END),0),
           @Odblokowane = COALESCE(SUM(CASE WHEN b.usrBlokada = 1 AND b.usrStatus = '1'
                                             AND u.usrBlokada = 0 AND u.usrStatus = '2'
                                             AND p.prcPonownieZatrudniony = 1 THEN 1 ELSE 0 END),0),
           @Zmienione = COALESCE(SUM(CASE WHEN ISNULL(CONVERT(int,b.usrBlokada),9) <> ISNULL(CONVERT(int,u.usrBlokada),9)
                                              OR ISNULL(b.usrStatus,'') <> ISNULL(u.usrStatus,'') THEN 1 ELSE 0 END),0)
    FROM #Przed b JOIN dbo.Uzytkownik u ON u.usrID = b.usrID
    LEFT JOIN mvw.vPracownikKontoSynchronizacja p ON p.prcID = u.usrPracownikID;
    SET @Inne = @Zmienione - @Zablokowane - @Odblokowane;
    SELECT @GrupyPo = COUNT_BIG(*) FROM dbo.GrupaCzlonkowie;
    IF @Nowe <> 65 OR @NoweGotowe <> 65 OR @NoweGrupa <> 65 OR @Zablokowane <> 11 OR @Odblokowane <> 2
       OR @Inne <> 0 OR @GrupyPo - @GrupyPrzed <> 65
        THROW 50102, 'Wynik synchronizacji różni się od zatwierdzonego podglądu; transakcja wycofana.', 1;

    COMMIT TRANSACTION;
    SELECT 'ZATWIERDZONO' AS Wynik, @Nowe AS Nowe, @Zablokowane AS Zablokowane, @Odblokowane AS Odblokowane,
           @NoweGrupa AS NoweWGrupieWszyscy, @Inne AS InneZmiany;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
