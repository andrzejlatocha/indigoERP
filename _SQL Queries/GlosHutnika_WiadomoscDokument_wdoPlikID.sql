-- Głos Hutnika / wysyłka wiadomości: powiązanie załącznika z rekordem dbo.Plik (wzorzec epicaHR).
-- Idempotentne. Uruchamia użytkownik w SSMS na JAPET (indigoERP). Stare załączniki zostają bez wdoPlikID (fallback w Workerze).
USE indigoERP;
GO

IF COL_LENGTH('dbo.WiadomoscDokument', 'wdoPlikID') IS NULL
    ALTER TABLE dbo.WiadomoscDokument ADD wdoPlikID int NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WiadomoscDokument01' AND object_id = OBJECT_ID('dbo.WiadomoscDokument'))
    CREATE INDEX IX_WiadomoscDokument01 ON dbo.WiadomoscDokument (wdoPlikID);
GO

-- Kontrola stanu (tylko odczyt)
SELECT COUNT(*) AS dokumenty_razem,
       COUNT(wdoPlikID) AS dokumenty_z_rekordem_Plik,
       COUNT(*) - COUNT(wdoPlikID) AS dokumenty_bez_rekordu_Plik_stare
FROM dbo.WiadomoscDokument;
GO
