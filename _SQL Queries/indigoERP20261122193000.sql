/* Zmiana nazw ról i grup kaizenITSM bez zmiany identyfikatorów ani członkostw.
   Wykonać w SSMS na JAPET.indigoERP. @Zatwierdz=0 wycofuje próbę; 1 zapisuje. */
USE [indigoERP];
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
DECLARE @Zatwierdz bit=1, @Lock int;
DECLARE @Nazwy TABLE(Stara nvarchar(100) NOT NULL, Nowa nvarchar(100) NOT NULL);
INSERT @Nazwy VALUES
(N'HCZ-kaizenITSM-Dokumentacja-Administrator',N'HCZ-kaizenITSM-Centrum Dokumentacji IT-Administrator'),
(N'HCZ-kaizenITSM-Dokumentacja-Użytkownik',N'HCZ-kaizenITSM-Centrum Dokumentacji IT-Użytkownik');
IF NOT EXISTS(SELECT 1 FROM dbo.Aplikacja WHERE appID=360 AND appNazwa=N'kaizenITSM' AND appStatus='2' AND ISNULL(Usuniety,0)=0)
    THROW 51001,N'Brak aktywnej aplikacji kaizenITSM (360).',1;
IF EXISTS(SELECT 1 FROM @Nazwy WHERE CONVERT(nvarchar(100),CONVERT(varchar(100),Nowa)) COLLATE Polish_BIN2 <> Nowa COLLATE Polish_BIN2)
    THROW 51002,N'Nie można bezstratnie zapisać nowej nazwy.',1;
BEGIN TRY
    BEGIN TRANSACTION;
    EXEC @Lock=sys.sp_getapplock @Resource=N'indigoERP.kaizenITSM.role.360',@LockMode='Exclusive',@LockOwner='Transaction',@LockTimeout=10000;
    IF @Lock<0 THROW 51003,N'Nie uzyskano blokady konfiguracji ról.',1;
    IF EXISTS(SELECT n.Stara FROM @Nazwy n LEFT JOIN dbo.Rola r WITH(UPDLOCK,HOLDLOCK)
        ON r.srlNazwa COLLATE Polish_BIN2 IN (n.Stara COLLATE Polish_BIN2,n.Nowa COLLATE Polish_BIN2)
        GROUP BY n.Stara HAVING COUNT(r.srlID)<>1)
        THROW 51004,N'Brak lub duplikat starej/nowej roli; wymagana analiza.',1;
    IF EXISTS(SELECT n.Stara FROM @Nazwy n LEFT JOIN dbo.Grupa g WITH(UPDLOCK,HOLDLOCK)
        ON g.groNazwa COLLATE Polish_BIN2 IN (n.Stara COLLATE Polish_BIN2,n.Nowa COLLATE Polish_BIN2)
        GROUP BY n.Stara HAVING COUNT(g.groID)<>1)
        THROW 51005,N'Brak lub duplikat starej/nowej grupy; wymagana analiza.',1;
    IF EXISTS(SELECT 1 FROM @Nazwy n JOIN dbo.Rola r ON r.srlNazwa COLLATE Polish_BIN2 IN (n.Stara COLLATE Polish_BIN2,n.Nowa COLLATE Polish_BIN2)
        WHERE r.srlAplikacjaID<>360 OR ISNULL(r.srlStatus,'')<>'2' OR ISNULL(r.srlBlokada,0)<>0 OR ISNULL(r.Usuniety,0)<>0)
        THROW 51006,N'Rola jest nieaktywna albo należy do innej aplikacji.',1;
    IF EXISTS(SELECT 1 FROM @Nazwy n JOIN dbo.Grupa g ON g.groNazwa COLLATE Polish_BIN2 IN (n.Stara COLLATE Polish_BIN2,n.Nowa COLLATE Polish_BIN2)
        WHERE g.groAplikacjaID<>360 OR ISNULL(g.groStatus,'')<>'2' OR ISNULL(g.groBlokada,0)<>0 OR ISNULL(g.Usuniety,0)<>0
            OR ISNULL(g.groSynchronizacjaAD,0)<>0)
        THROW 51007,N'Grupa jest nieaktywna, należy do innej aplikacji lub jest synchronizowana z AD.',1;
    IF EXISTS(SELECT 1 FROM @Nazwy n JOIN dbo.Rola r ON r.srlNazwa COLLATE Polish_BIN2 IN (n.Stara COLLATE Polish_BIN2,n.Nowa COLLATE Polish_BIN2)
        JOIN dbo.Grupa g ON g.groNazwa COLLATE Polish_BIN2 IN (n.Stara COLLATE Polish_BIN2,n.Nowa COLLATE Polish_BIN2)
        WHERE (SELECT COUNT(*) FROM dbo.RolaCzlonkowie c WHERE c.cwrRolaID=r.srlID AND c.cwrGrupaID=g.groID
            AND c.cwrUzytkownikID IS NULL AND c.cwrStatus='2')<>1)
        THROW 51008,N'Brak jednoznacznego aktywnego powiązania roli i grupy.',1;
    UPDATE r SET srlNazwa=n.Nowa,DataModyfikacja=GETDATE(),ModyfikacjaID=0
        FROM dbo.Rola r JOIN @Nazwy n ON r.srlNazwa COLLATE Polish_BIN2=n.Stara COLLATE Polish_BIN2;
    UPDATE g SET groNazwa=n.Nowa,DataModyfikacja=GETDATE(),ModyfikacjaID=0
        FROM dbo.Grupa g JOIN @Nazwy n ON g.groNazwa COLLATE Polish_BIN2=n.Stara COLLATE Polish_BIN2;
    SELECT @@SERVERNAME Serwer,DB_NAME() Baza,n.Nowa Nazwa,r.srlID RolaID,g.groID GrupaID,c.cwrID PowiazanieID
    FROM @Nazwy n JOIN dbo.Rola r ON r.srlNazwa COLLATE Polish_BIN2=n.Nowa COLLATE Polish_BIN2
    JOIN dbo.Grupa g ON g.groNazwa COLLATE Polish_BIN2=n.Nowa COLLATE Polish_BIN2
    JOIN dbo.RolaCzlonkowie c ON c.cwrRolaID=r.srlID AND c.cwrGrupaID=g.groID AND c.cwrUzytkownikID IS NULL AND c.cwrStatus='2';
    IF @Zatwierdz=1 BEGIN COMMIT TRANSACTION; PRINT N'COMMIT — nazwy zmienione.'; END
    ELSE BEGIN ROLLBACK TRANSACTION; PRINT N'ROLLBACK — próba bez zapisu.'; END;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
