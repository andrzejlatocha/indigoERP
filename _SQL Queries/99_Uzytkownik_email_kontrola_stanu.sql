-- 99: Kontrola stanu adresów e-mail użytkowników indigoERP (TYLKO ODCZYT).
USE indigoERP;
GO

SELECT usrStatus
     , CASE WHEN LTRIM(RTRIM(ISNULL(usrEmail, ''))) = '' THEN 'bez e-mail' ELSE 'z e-mail' END AS email
     , COUNT(*) AS liczba
FROM dbo.Uzytkownik
WHERE ISNULL(Usuniety, 0) = 0
GROUP BY usrStatus, CASE WHEN LTRIM(RTRIM(ISNULL(usrEmail, ''))) = '' THEN 'bez e-mail' ELSE 'z e-mail' END
ORDER BY usrStatus, email;

-- Powinno zwrócić 0 po skrypcie 01:
SELECT COUNT(*) AS nieaktywniZAdresem
FROM dbo.Uzytkownik
WHERE ISNULL(Usuniety, 0) = 0 AND usrStatus = '1' AND LTRIM(RTRIM(ISNULL(usrEmail, ''))) <> '';

-- Procedura synchronizacji: czy zawiera regułę czyszczenia adresu przy dezaktywacji (powinno być 1 po skrypcie 02):
SELECT CASE WHEN OBJECT_DEFINITION(OBJECT_ID('dbo.aspMergeUzytkownik')) LIKE N'%usrEmail = ''''%' THEN 1 ELSE 0 END AS regulaWProcedurze;
GO