-- Cel: JAPET / indigoERP. Uruchamia użytkownik po zapisaniu wyników do pliku.
-- Blokuje wskazane role i grupy oraz usuwa bezpośrednie przypisania użytkowników.
-- Powiązania grup z rolami i zagnieżdżenia grup pozostają bez zmian.
IF DB_NAME() <> N'indigoERP' THROW 50000, N'Nieprawidłowa baza danych.', 1;
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    SELECT groID INTO #Grupy FROM dbo.Grupa WITH (UPDLOCK, HOLDLOCK)
    WHERE groNazwa LIKE N'PL-SCX-APP-helpDESK%' OR groNazwa LIKE N'PL-SCX-APP-simpleLMS%';
    SELECT srlID INTO #Role FROM dbo.Rola WITH (UPDLOCK, HOLDLOCK)
    WHERE srlNazwa LIKE N'PL-SCX-APP-helpDESK%' OR srlNazwa LIKE N'PL-SCX-APP-simpleLMS%';
    IF NOT EXISTS (SELECT 1 FROM #Grupy) OR NOT EXISTS (SELECT 1 FROM #Role)
        THROW 50001, N'Nie znaleziono grup lub ról; przerwano operację.', 1;
    -- Przed usunięciem zapisz wyniki tego zapytania jako materiał do odtworzenia.
    SELECT N'GrupaCzlonkowie przed usunięciem' AS Zestaw, c.*
    FROM dbo.GrupaCzlonkowie c JOIN #Grupy g ON g.groID = c.uwgGrupaNadrzednaID
    WHERE c.uwgUzytkownikID IS NOT NULL;
    SELECT N'RolaCzlonkowie przed usunięciem' AS Zestaw, c.*
    FROM dbo.RolaCzlonkowie c JOIN #Role r ON r.srlID = c.cwrRolaID
    WHERE c.cwrUzytkownikID IS NOT NULL;
    UPDATE g SET groBlokada = 1 FROM dbo.Grupa g JOIN #Grupy x ON x.groID = g.groID;
    UPDATE r SET srlBlokada = 1 FROM dbo.Rola r JOIN #Role x ON x.srlID = r.srlID;
    DELETE c FROM dbo.GrupaCzlonkowie c JOIN #Grupy g ON g.groID = c.uwgGrupaNadrzednaID
    WHERE c.uwgUzytkownikID IS NOT NULL;
    DELETE c FROM dbo.RolaCzlonkowie c JOIN #Role r ON r.srlID = c.cwrRolaID
    WHERE c.cwrUzytkownikID IS NOT NULL;
    SELECT N'Grupa' AS Typ, g.groID AS ID, g.groNazwa AS Nazwa, g.groBlokada AS Blokada,
        (SELECT COUNT(*) FROM dbo.GrupaCzlonkowie c WHERE c.uwgGrupaNadrzednaID = g.groID AND c.uwgUzytkownikID IS NOT NULL) AS Uzytkownicy
    FROM dbo.Grupa g JOIN #Grupy x ON x.groID = g.groID
    UNION ALL
    SELECT N'Rola', r.srlID, r.srlNazwa, r.srlBlokada,
        (SELECT COUNT(*) FROM dbo.RolaCzlonkowie c WHERE c.cwrRolaID = r.srlID AND c.cwrUzytkownikID IS NOT NULL)
    FROM dbo.Rola r JOIN #Role x ON x.srlID = r.srlID;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;