/*
    indigoERP - grupy i role dla bibliotek atacamaDMS z transkrypcji (wdrozenie 2026-10-28)
    Biblioteki: Umowy, NDA, Zgody, Uchwaly, Wnioski, Polecenia sluzbowe.
    Poziomy: Administrator, Obsluga, Przegladanie (6 x 3 = 18 grup, 18 rol, 18 powiazan).

    Serwer: JAPET, baza indigoERP. Uruchamiac w SSMS:
      1. @Zatwierdz = 0 - test z ROLLBACK (domyslnie), sprawdzic wynik kontrolny,
      2. @Zatwierdz = 1 - zapis z COMMIT, wykonuje uzytkownik po sprawdzeniu wynikow.

    Zakres:
      - dbo.Grupa:          HCZ-atacamaDMS-<Poziom> biblioteki - <nazwa>, groAplikacjaID = 210 (atacamaDMS),
      - dbo.Rola:           ta sama nazwa, srlAplikacjaID = 210,
      - dbo.RolaCzlonkowie: powiazanie roli z grupa (cwrGrupaID, cwrUzytkownikID = NULL).
      Czlonkow grup nie dodajemy - robi to IT / administrator biblioteki.

    Skrypt jednorazowy: jesli ktorakolwiek z tych nazw grupy lub roli juz istnieje, zatrzymuje sie bez zmian (THROW 52004).
    Nie tworzy biblioteki, poziomow ani BibliotekaDostep w atacamaDMS - to osobne skrypty biblioteki.
    Nazwy z polskimi znakami budowane przez NCHAR() (zob. feedback sqlcmd / polskie znaki).

    Parametr: @Zatwierdz  0 = test (ROLLBACK), 1 = zapis (COMMIT)
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Zatwierdz bit = 0;
DECLARE @AppID int = 210;
DECLARE @Opis nvarchar(200) = N'Biblioteka atacamaDMS (wdrozenie 2026-10-28)';

IF DB_NAME() <> N'indigoERP'
    THROW 52000, N'Skrypt przeznaczony dla bazy indigoERP.', 1;
IF UPPER(CONVERT(nvarchar(128), SERVERPROPERTY('ServerName'))) NOT IN (N'JAPET', N'JAPET.HCZ.COM.PL')
    THROW 52001, N'Skrypt przeznaczony dla serwera JAPET.', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.Aplikacja WHERE appID = @AppID)
    THROW 52002, N'Brak aplikacji atacamaDMS (appID 210) w dbo.Aplikacja.', 1;

-- Nazwy bibliotek i poziomow
DECLARE @Biblioteki TABLE (bibNr int IDENTITY(1,1) PRIMARY KEY, bibNazwa nvarchar(100) NOT NULL);
INSERT @Biblioteki (bibNazwa) VALUES
    (N'Umowy'),
    (N'NDA'),
    (N'Zgody'),
    (N'Uchwa' + NCHAR(322) + N'y'),
    (N'Wnioski'),
    (N'Polecenia s' + NCHAR(322) + N'u' + NCHAR(380) + N'bowe');

DECLARE @Poziomy TABLE (pozNr int IDENTITY(1,1) PRIMARY KEY, pozNazwa nvarchar(50) NOT NULL);
INSERT @Poziomy (pozNazwa) VALUES
    (N'Administrator'),
    (N'Obs' + NCHAR(322) + N'uga'),
    (N'Przegl' + NCHAR(261) + N'danie');

DECLARE @Nazwy TABLE (nazNr int IDENTITY(1,1) PRIMARY KEY, nazNazwa nvarchar(200) NOT NULL);
INSERT @Nazwy (nazNazwa)
SELECT CONCAT(N'HCZ-atacamaDMS-', p.pozNazwa, N' biblioteki - ', b.bibNazwa)
FROM @Biblioteki AS b
CROSS JOIN @Poziomy AS p
ORDER BY b.bibNr, p.pozNr;

IF (SELECT COUNT(*) FROM @Nazwy) <> 18
    THROW 52010, N'Oczekiwano 18 nazw (6 bibliotek x 3 poziomy).', 1;

-- Skrypt jednorazowy: zadna z nazw nie moze istniec ani w grupach, ani w rolach
IF EXISTS (SELECT 1 FROM dbo.Grupa AS g JOIN @Nazwy AS n ON n.nazNazwa = g.groNazwa)
    THROW 52004, N'Grupa o nazwie biblioteki juz istnieje - skrypt byl wykonany lub nazwy sa zajete. Wynik sprawdzic kontrola.', 1;
IF EXISTS (SELECT 1 FROM dbo.Rola AS r JOIN @Nazwy AS n ON n.nazNazwa = r.srlNazwa)
    THROW 52004, N'Rola o nazwie biblioteki juz istnieje - skrypt byl wykonany lub nazwy sa zajete. Wynik sprawdzic kontrola.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @i int = 1;
    DECLARE @n nvarchar(200);
    DECLARE @groID int;
    DECLARE @srlID int;

    WHILE @i <= (SELECT COUNT(*) FROM @Nazwy)
    BEGIN
        SELECT @n = nazNazwa FROM @Nazwy WHERE nazNr = @i;

        INSERT dbo.Grupa (groAplikacjaID, groNazwa, groOpis, groGrupaWszyscy, groAdministrator, groBlokada, groEdycja, groSynchronizacjaAD, groStatus)
        VALUES (@AppID, @n, @Opis, 0, 0, 0, 0, 0, '2');
        SET @groID = SCOPE_IDENTITY();

        INSERT dbo.Rola (srlAplikacjaID, srlNazwa, srlOpis, srlEdycja, srlBlokada, srlStatus)
        VALUES (@AppID, @n, @Opis, 0, 0, '2');
        SET @srlID = SCOPE_IDENTITY();

        INSERT dbo.RolaCzlonkowie (cwrUzytkownikID, cwrRolaID, cwrGrupaID, cwrStatus)
        VALUES (NULL, @srlID, @groID, '2');

        SET @i = @i + 1;
    END;

    -- Kontrola przed zakonczeniem transakcji
    SELECT N'Grupy' AS Etap, COUNT(*) AS Liczba
    FROM dbo.Grupa AS g JOIN @Nazwy AS n ON n.nazNazwa = g.groNazwa AND g.groAplikacjaID = @AppID;

    SELECT N'Role' AS Etap, COUNT(*) AS Liczba
    FROM dbo.Rola AS r JOIN @Nazwy AS n ON n.nazNazwa = r.srlNazwa AND r.srlAplikacjaID = @AppID;

    SELECT N'Powiazania rola-grupa' AS Etap, COUNT(*) AS Liczba
    FROM dbo.RolaCzlonkowie AS c
    JOIN dbo.Grupa AS g ON g.groID = c.cwrGrupaID
    JOIN @Nazwy AS n ON n.nazNazwa = g.groNazwa;

    IF (SELECT COUNT(*) FROM dbo.Grupa AS g JOIN @Nazwy AS n ON n.nazNazwa = g.groNazwa AND g.groAplikacjaID = @AppID) <> 18
       OR (SELECT COUNT(*) FROM dbo.Rola AS r JOIN @Nazwy AS n ON n.nazNazwa = r.srlNazwa AND r.srlAplikacjaID = @AppID) <> 18
       OR (SELECT COUNT(*) FROM dbo.RolaCzlonkowie AS c JOIN dbo.Grupa AS g ON g.groID = c.cwrGrupaID JOIN @Nazwy AS n ON n.nazNazwa = g.groNazwa) <> 18
        THROW 52020, N'Kontrola liczb: oczekiwano 18 grup, 18 rol i 18 powiazan.', 1;

    SELECT N'Parametr' AS Etap, @Zatwierdz AS Zatwierdz;

    IF @Zatwierdz = 1
    BEGIN
        COMMIT TRANSACTION;
        PRINT N'COMMIT - zmiany zapisane';
    END
    ELSE
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT N'ROLLBACK - tryb testowy, nic nie zapisano';
    END;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
