-- Widok mvw.vUzytkownikRolaEfektywna — jedna wspólna definicja "efektywnego" członkostwa
-- użytkownik<->rola (bezpośrednie z dbo.RolaCzlonkowie + dziedziczone przez grupę z
-- mvw.vUzytkownikRolaLista, z priorytetem dla wiersza z widoku przy duplikacie).
--
-- Zastępuje dwie identyczne kopie tej samej logiki (CTE + ROW_NUMBER) trzymane osobno jako
-- FromSqlInterpolated w RolaController.SelectUzytkownikByRola i UzytkownikController.SelectRolaByUzytkownik.
-- Filtrowanie po appID/srlStatus/usrStatus zostaje po stronie C# jako zwykłe .Where() na tym widoku —
-- nie ma potrzeby przekazywania parametrów do samego widoku (SQL Server nie pozwala widokom
-- przyjmować parametrów), bo te filtry nie wpływają na to, który wiersz wygrywa w deduplikacji.
--
-- Uruchomić na JAPET/indigoERP.Developer (i produkcyjnej bazie przy wdrożeniu).

CREATE VIEW mvw.vUzytkownikRolaEfektywna AS
WITH combined AS
(
    SELECT cwrID, appID, usrID, usrNazwa, usrNazwaAD, usrStatus, srlID, srlNazwa, srlOpis, srlStatus,
           uwgPoziom, uwgDziedziczy, 0 AS zrodloPriorytet
    FROM mvw.vUzytkownikRolaLista

    UNION ALL

    SELECT c.cwrID, ISNULL(r.srlAplikacjaID, 0) AS appID, u.usrID, u.usrNazwa, NULL AS usrNazwaAD, u.usrStatus,
           r.srlID, r.srlNazwa, r.srlOpis, r.srlStatus, 0 AS uwgPoziom, CAST(0 AS bit) AS uwgDziedziczy, 1 AS zrodloPriorytet
    FROM dbo.RolaCzlonkowie AS c
    INNER JOIN dbo.Uzytkownik AS u ON c.cwrUzytkownikID = u.usrID
    INNER JOIN dbo.Rola AS r ON c.cwrRolaID = r.srlID
    WHERE c.cwrUzytkownikID IS NOT NULL
        AND c.cwrRolaID IS NOT NULL
        AND (c.cwrStatus = '2' OR c.cwrStatus IS NULL)
        AND ISNULL(u.Usuniety, 0) = 0
),
ranked AS
(
    SELECT *, ROW_NUMBER() OVER (PARTITION BY cwrID, usrID, srlID ORDER BY zrodloPriorytet) AS rn
    FROM combined
)
SELECT cwrID, appID, usrID, usrNazwa, usrNazwaAD, usrStatus, srlID, srlNazwa, srlOpis, srlStatus, uwgPoziom, uwgDziedziczy
FROM ranked
WHERE rn = 1;
GO
