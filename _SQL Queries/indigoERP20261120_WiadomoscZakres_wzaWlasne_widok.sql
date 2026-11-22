-- indigoERP: enum.WiadomoscZakres zwraca kolumne wzaWlasne (dbo.WiadomoscZakres.wzaWlasne juz istnieje).
-- Definicja widoku odczytana z CERES i JAPET (OBJECT_DEFINITION) i rozszerzona o wzaWlasne.
-- Uruchamia uzytkownik w SSMS: najpierw CERES, potem JAPET. Skrypt idempotentny (ALTER VIEW).

IF COL_LENGTH('dbo.WiadomoscZakres', 'wzaWlasne') IS NULL
BEGIN
    RAISERROR('Brak kolumny dbo.WiadomoscZakres.wzaWlasne - najpierw dodaj kolumne.', 16, 1);
END
GO

IF COL_LENGTH('dbo.WiadomoscZakres', 'wzaWlasne') IS NOT NULL
BEGIN
    EXEC('ALTER VIEW enum.WiadomoscZakres
AS
	SELECT wz.wzaID
		, wz.wzaAplikacjaID
		, wz.wzaNazwa
		, wz.wzaWidok
		, wz.wzaStatus
		, wz.wzaWlasne
	FROM dbo.WiadomoscZakres AS wz;');
END
GO

-- Kontrola (tylko odczyt).
SELECT c.name FROM sys.columns c WHERE c.object_id = OBJECT_ID('enum.WiadomoscZakres') AND c.name = 'wzaWlasne';
