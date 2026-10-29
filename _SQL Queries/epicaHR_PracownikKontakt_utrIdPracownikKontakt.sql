-- epicaHR: PracownikKontakt - usuwanie logiczne (INSTEAD OF DELETE zamienia DELETE na Usuniety = 1).
-- Uruchamia użytkownik w SSMS na bazie epicaHR. Idempotentne (CREATE OR ALTER).
USE epicaHR;
GO

CREATE OR ALTER TRIGGER dbo.utrIdPracownikKontakt ON dbo.PracownikKontakt
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.PracownikKontakt
    SET Usuniety = 1
    WHERE pktID IN (SELECT d.pktID FROM deleted AS d);
END;
GO