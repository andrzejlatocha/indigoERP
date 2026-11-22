/*
    Role i grupy kaizenITSM w indigoERP, aplikacja 360.
    Uruchomić w SSMS po połączeniu z właściwym serwerem, np. JAPET.
    @Zatwierdz = 0: próba z ROLLBACK; @Zatwierdz = 1: COMMIT.
    Skrypt nie dodaje użytkowników, nie modyfikuje istniejących ról, grup ani AD.
*/
USE [indigoERP];
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;

DECLARE @AplikacjaID int = 360;
DECLARE @Zatwierdz bit = 1;
DECLARE @BlokadaWynik int;
DECLARE @Role TABLE (Nazwa nvarchar(100) NOT NULL PRIMARY KEY);
INSERT @Role (Nazwa) VALUES
    (N'HCZ-kaizenITSM-Administrator'),
    (N'HCZ-kaizenITSM-Zgłoszenia-Użytkownik'),
    (N'HCZ-kaizenITSM-Zgłoszenia-Obsługa'),
    (N'HCZ-kaizenITSM-Wniosek-Użytkownik'),
    (N'HCZ-kaizenITSM-Wniosek-Obsługa'),
    (N'HCZ-kaizenITSM-Zmiana-Użytkownik'),
    (N'HCZ-kaizenITSM-Zmiana-Obsługa'),
    (N'HCZ-kaizenITSM-Zmiana-Akceptacja'),
    (N'HCZ-kaizenITSM-Wiedza-Administrator'),
    (N'HCZ-kaizenITSM-Wiedza-Użytkownik'),
    (N'HCZ-kaizenITSM-Zastępstwo'),
    (N'HCZ-kaizenITSM-Kontakt'),
    (N'HCZ-kaizenITSM-Dokumentacja-Administrator'),
    (N'HCZ-kaizenITSM-Dokumentacja-Użytkownik'),
    (N'HCZ-kaizenITSM-Ankieta-Administrator'),
    (N'HCZ-kaizenITSM-Ankieta-Użytkownik'),
    (N'HCZ-kaizenITSM-Protokół-Administrator'),
    (N'HCZ-kaizenITSM-Katalog-Administrator'),
    (N'HCZ-kaizenITSM-Problem-Administrator'),
    (N'HCZ-kaizenITSM-Rejestr-Administrator'),
    (N'HCZ-kaizenITSM-Zasób'),
    (N'HCZ-kaizenITSM-Zasób-CMDB'),
    (N'HCZ-kaizenITSM-Zasób-Relacja'),
    (N'HCZ-kaizenITSM-Zasób-Oprogramowanie'),
    (N'HCZ-kaizenITSM-Zasób-Agent'),
    (N'HCZ-kaizenITSM-Zasób-Agent-Raport'),
    (N'HCZ-kaizenITSM-Zasób-Historia'),
    (N'HCZ-kaizenITSM-Zasób-Bezpieczeństwo'),
    (N'HCZ-kaizenITSM-Zasób-Spis'),
    (N'HCZ-kaizenITSM-Zasób-Licencja'),
    (N'HCZ-kaizenITSM-Zasób-Mapa');

IF (SELECT COUNT(*) FROM @Role) <> 31 THROW 51000, N'Nieprawidłowa liczba nazw ról.', 1;
IF OBJECT_ID(N'dbo.Aplikacja', N'U') IS NULL OR OBJECT_ID(N'dbo.Rola', N'U') IS NULL
    OR OBJECT_ID(N'dbo.Grupa', N'U') IS NULL OR OBJECT_ID(N'dbo.RolaCzlonkowie', N'U') IS NULL
    THROW 51001, N'Brak wymaganych tabel indigoERP.', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.Aplikacja WHERE appID=@AplikacjaID AND appNazwa=N'kaizenITSM'
    AND appStatus='2' AND ISNULL(Usuniety,0)=0)
    THROW 51002, N'Nie znaleziono aktywnej aplikacji kaizenITSM o ID 360.', 1;
IF EXISTS (SELECT 1 FROM @Role WHERE CONVERT(nvarchar(100),CONVERT(varchar(100),Nazwa)) COLLATE Polish_CI_AS <> Nazwa COLLATE Polish_CI_AS)
    THROW 51003, N'Baza nie może bezstratnie zapisać polskich znaków w nazwach varchar.', 1;

BEGIN TRY
    BEGIN TRANSACTION;
    EXEC @BlokadaWynik = sys.sp_getapplock @Resource=N'indigoERP.kaizenITSM.role.360',
        @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=10000;
    IF @BlokadaWynik < 0 THROW 51004, N'Nie można uzyskać blokady konfiguracji ról.', 1;

    -- Nazwy muszą jednoznacznie wskazywać istniejący, aktywny obiekt właściwej aplikacji.
    IF EXISTS (SELECT n.Nazwa FROM @Role n JOIN dbo.Rola r WITH (UPDLOCK,HOLDLOCK) ON r.srlNazwa=n.Nazwa
               GROUP BY n.Nazwa HAVING COUNT(*) > 1)
        THROW 51005, N'Istnieją zduplikowane nazwy ról; przerwano bez zmian.', 1;
    IF EXISTS (SELECT n.Nazwa FROM @Role n JOIN dbo.Grupa g WITH (UPDLOCK,HOLDLOCK) ON g.groNazwa=n.Nazwa
               GROUP BY n.Nazwa HAVING COUNT(*) > 1)
        THROW 51006, N'Istnieją zduplikowane nazwy grup; przerwano bez zmian.', 1;
    IF EXISTS (SELECT 1 FROM @Role n JOIN dbo.Rola r WITH (UPDLOCK,HOLDLOCK) ON r.srlNazwa=n.Nazwa
               WHERE r.srlAplikacjaID<>@AplikacjaID OR ISNULL(r.Usuniety,0)<>0 OR ISNULL(r.srlBlokada,0)<>0
                   OR ISNULL(r.srlStatus,'')<>'2' OR r.srlNazwa COLLATE Polish_BIN2 <> n.Nazwa COLLATE Polish_BIN2)
        THROW 51007, N'Istniejąca rola jest nieaktywna, ma inną aplikację lub inną pisownię; sprawdź dane.', 1;
    IF EXISTS (SELECT 1 FROM @Role n JOIN dbo.Grupa g WITH (UPDLOCK,HOLDLOCK) ON g.groNazwa=n.Nazwa
               WHERE g.groAplikacjaID<>@AplikacjaID OR ISNULL(g.Usuniety,0)<>0 OR ISNULL(g.groBlokada,0)<>0
                   OR ISNULL(g.groStatus,'')<>'2' OR g.groNazwa COLLATE Polish_BIN2 <> n.Nazwa COLLATE Polish_BIN2)
        THROW 51008, N'Istniejąca grupa jest nieaktywna, ma inną aplikację lub inną pisownię; sprawdź dane.', 1;

    DECLARE @NoweRole TABLE (Nazwa varchar(100) NOT NULL);
    DECLARE @NoweGrupy TABLE (Nazwa varchar(100) NOT NULL);
    DECLARE @NowePowiazania TABLE (RolaID int NOT NULL, GrupaID int NOT NULL);

    INSERT dbo.Rola (srlAplikacjaID,srlNazwa,srlOpis,srlEdycja,srlBlokada,srlStatus)
        OUTPUT inserted.srlNazwa INTO @NoweRole(Nazwa)
    SELECT @AplikacjaID,n.Nazwa,N'Rola aplikacji kaizenITSM',0,0,'2'
    FROM @Role n WHERE NOT EXISTS (SELECT 1 FROM dbo.Rola r WITH (UPDLOCK,HOLDLOCK) WHERE r.srlNazwa=n.Nazwa);

    INSERT dbo.Grupa (groAplikacjaID,groNazwa,groOpis,groGrupaWszyscy,groAdministrator,groBlokada,groEdycja,groSynchronizacjaAD,groStatus)
        OUTPUT inserted.groNazwa INTO @NoweGrupy(Nazwa)
    SELECT @AplikacjaID,n.Nazwa,N'Grupa aplikacji kaizenITSM',0,0,0,0,0,'2'
    FROM @Role n WHERE NOT EXISTS (SELECT 1 FROM dbo.Grupa g WITH (UPDLOCK,HOLDLOCK) WHERE g.groNazwa=n.Nazwa);

    IF EXISTS (SELECT r.srlID,g.groID FROM @Role n JOIN dbo.Rola r ON r.srlNazwa=n.Nazwa
               JOIN dbo.Grupa g ON g.groNazwa=n.Nazwa JOIN dbo.RolaCzlonkowie c WITH (UPDLOCK,HOLDLOCK)
                   ON c.cwrRolaID=r.srlID AND c.cwrGrupaID=g.groID AND c.cwrUzytkownikID IS NULL
               GROUP BY r.srlID,g.groID HAVING COUNT(*)>1 OR MIN(ISNULL(c.cwrStatus,''))<>'2')
        THROW 51009, N'Powiązanie roli z grupą jest zduplikowane lub nieaktywne; sprawdź dane.', 1;

    INSERT dbo.RolaCzlonkowie (cwrRolaID,cwrGrupaID,cwrUzytkownikID,cwrStatus)
        OUTPUT inserted.cwrRolaID,inserted.cwrGrupaID INTO @NowePowiazania(RolaID,GrupaID)
    SELECT r.srlID,g.groID,NULL,'2' FROM @Role n
    JOIN dbo.Rola r ON r.srlNazwa=n.Nazwa AND r.srlAplikacjaID=@AplikacjaID
    JOIN dbo.Grupa g ON g.groNazwa=n.Nazwa AND g.groAplikacjaID=@AplikacjaID
    WHERE NOT EXISTS (SELECT 1 FROM dbo.RolaCzlonkowie c WITH (UPDLOCK,HOLDLOCK)
                      WHERE c.cwrRolaID=r.srlID AND c.cwrGrupaID=g.groID AND c.cwrUzytkownikID IS NULL);

    IF (SELECT COUNT(*) FROM @Role n JOIN dbo.Rola r ON r.srlNazwa=n.Nazwa AND r.srlAplikacjaID=@AplikacjaID
        JOIN dbo.Grupa g ON g.groNazwa=n.Nazwa AND g.groAplikacjaID=@AplikacjaID
        JOIN dbo.RolaCzlonkowie c ON c.cwrRolaID=r.srlID AND c.cwrGrupaID=g.groID
            AND c.cwrUzytkownikID IS NULL AND c.cwrStatus='2') <> 31
        THROW 51010, N'Kontrola końcowa nie potwierdziła 31 aktywnych powiązań.', 1;

    SELECT @@SERVERNAME AS Serwer,DB_NAME() AS Baza,@AplikacjaID AS AplikacjaID,@Zatwierdz AS Zatwierdz,
        (SELECT COUNT(*) FROM @NoweRole) AS NoweRole,(SELECT COUNT(*) FROM @NoweGrupy) AS NoweGrupy,
        (SELECT COUNT(*) FROM @NowePowiazania) AS NowePowiazania;
    SELECT n.Nazwa,r.srlID AS RolaID,g.groID AS GrupaID,c.cwrID AS PowiazanieID,
        CASE WHEN nr.Nazwa IS NULL THEN N'istniała' ELSE N'dodana' END AS Rola,
        CASE WHEN ng.Nazwa IS NULL THEN N'istniała' ELSE N'dodana' END AS Grupa,
        CASE WHEN np.RolaID IS NULL THEN N'istniało' ELSE N'dodane' END AS Powiazanie
    FROM @Role n JOIN dbo.Rola r ON r.srlNazwa=n.Nazwa AND r.srlAplikacjaID=@AplikacjaID
    JOIN dbo.Grupa g ON g.groNazwa=n.Nazwa AND g.groAplikacjaID=@AplikacjaID
    JOIN dbo.RolaCzlonkowie c ON c.cwrRolaID=r.srlID AND c.cwrGrupaID=g.groID AND c.cwrUzytkownikID IS NULL AND c.cwrStatus='2'
    LEFT JOIN @NoweRole nr ON nr.Nazwa=n.Nazwa LEFT JOIN @NoweGrupy ng ON ng.Nazwa=n.Nazwa
    LEFT JOIN @NowePowiazania np ON np.RolaID=r.srlID AND np.GrupaID=g.groID ORDER BY n.Nazwa;

    IF @Zatwierdz=1 BEGIN COMMIT TRANSACTION; PRINT N'COMMIT — role, grupy i powiązania zapisane.'; END
    ELSE BEGIN ROLLBACK TRANSACTION; PRINT N'ROLLBACK — próba zakończona bez zapisu.'; END;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
