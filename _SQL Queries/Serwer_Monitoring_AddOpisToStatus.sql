-- Dodaje s.srvOpis do widoku mvw.vSerwerStatus (dashboard) — brakowało go w pierwotnej wersji,
-- a jest potrzebny żeby pokazać opis serwera pod nazwą na karcie w Monitoringu.
-- Uruchomić na JAPET/indigoERP.Developer (i produkcyjnej bazie przy wdrożeniu).

ALTER VIEW mvw.vSerwerStatus AS
SELECT  s.srvID, s.srvNazwa, s.srvOpis, s.srvAdres, s.srvPort, s.srvSerwerTypKOD, t.styOpis,
        l.slgDataSprawdzenie, l.slgDostepny, l.slgCzasOdpowiedzMs, l.slgBlad
FROM    dbo.Serwer s
JOIN    dic.SerwerTyp t ON t.styKOD = s.srvSerwerTypKOD
OUTER APPLY (
    SELECT TOP (1) slgDataSprawdzenie, slgDostepny, slgCzasOdpowiedzMs, slgBlad
    FROM   dbo.SerwerLog
    WHERE  slgSerwerID = s.srvID
    ORDER BY slgDataSprawdzenie DESC
) l
WHERE   (s.Usuniety = 0 OR s.Usuniety IS NULL);
GO
