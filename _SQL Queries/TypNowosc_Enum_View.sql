-- Widok enum.TypNowosc — lookup nad dic.TypNowosc (bez filtra Usuniety, bo dic.TypNowosc
-- nie ma pól audytowych/soft-delete — filtrowanie po statusie robi kontroler).
-- Uruchomić na CERES/indigoERP.

CREATE VIEW enum.TypNowosc AS
SELECT tnoKOD, tnoIkona, tnoOpis, tnoStatus
FROM dic.TypNowosc;
GO
