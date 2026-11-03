/*
  Doraźne wznowienie wysyłki wiadomości 1095 ("Głos Hutnika 29/2026") przygotowanej w oknie „Nowy numer”.
  Wiadomości z formularza nie mają wzorca (wiaWzorzecID IS NULL), a worker bez wzorca ich nie podejmował.
  Ten skrypt ustawia aktywny wzorzec, żeby worker podjął wiadomość w obecnej wersji.
  Po wdrożeniu poprawki workera (wiadomości bez wzorca są podejmowane) skrypt nie jest potrzebny.

  PRZED URUCHOMIENIEM ustaw @wzorzecID (ID aktywnego wzorca z WiadomoscWzorzec, np. jak w wiadomościach 1093/1094).
  Skrypt wysyła wiadomość do adresatów z dbo.WiadomoscPracownik (status '2'). Uruchamia użytkownik w SSMS na JAPET (F5).
*/
USE indigoERP;
GO

SET NOCOUNT ON;

DECLARE @wiaID int = 1095;
DECLARE @wzorzecID int = NULL;   -- ustaw ID wzorca przed uruchomieniem
DECLARE @nadawca nvarchar(200) = N'indigoERP@huta-czestochowa.pl';
DECLARE @ileOdbiorcow int = (SELECT COUNT(*) FROM dbo.WiadomoscPracownik WHERE wprWiadomoscID = @wiaID AND wprStatus = '2' AND ISNULL(wprEmail, '') <> '');
DECLARE @wzorzecOk bit = CASE WHEN @wzorzecID IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.WiadomoscWzorzec WHERE msgID = @wzorzecID AND msgStatus = '2') THEN 1 ELSE 0 END;

-- Kontrola (tylko odczyt)
SELECT w.wiaID, w.wiaStatus, w.wiaEmail, w.wiaWzorzecID, @wzorzecID AS NowyWzorzecID, @wzorzecOk AS WzorzecAktywny, @ileOdbiorcow AS OdbiorcyDoWysylki
FROM dbo.Wiadomosc w
WHERE w.wiaID = @wiaID;

IF @wzorzecOk = 1
AND @ileOdbiorcow > 0
AND EXISTS (SELECT 1 FROM dbo.Wiadomosc WHERE wiaID = @wiaID AND wiaStatus = '2' AND wiaWzorzecID IS NULL)
BEGIN
    UPDATE dbo.Wiadomosc
    SET wiaWzorzecID = @wzorzecID,
        wiaEmail = ISNULL(NULLIF(wiaEmail, ''), @nadawca)
    WHERE wiaID = @wiaID;

    PRINT CONCAT(N'Wiadomość ', @wiaID, N' otrzymała wzorzec ', @wzorzecID, N' - worker podejmie wysyłkę do ', @ileOdbiorcow, N' adresatów.');
END
ELSE
BEGIN
    PRINT N'Warunki nie są spełnione - nic nie zmieniono. Sprawdź @wzorzecID i wynik kontroli powyżej.';
END;
