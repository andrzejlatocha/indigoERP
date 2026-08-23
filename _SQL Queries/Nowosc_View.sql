-- Widok mvw.vNowosc — dbo.Nowosc wzbogacony o opis/ikonę typu z dic.TypNowosc.
-- Uruchomić na CERES/indigoERP.

CREATE VIEW mvw.vNowosc AS
SELECT
    n.nowID,
    n.nowData,
    n.nowAplikacjaID,
    n.nowURL,
    n.nowWersja,
    n.nowTytul,
    n.nowOpis,
    n.nowStatus,
    n.nowTypNowoscKOD,
    t.tnoOpis,
    t.tnoIkona
FROM dbo.Nowosc n
LEFT JOIN dic.TypNowosc t ON n.nowTypNowoscKOD = t.tnoKOD;
GO
