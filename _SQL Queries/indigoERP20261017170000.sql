IF COL_LENGTH(N'dbo.WiadomoscPracownik', N'wprBlad') IS NULL
BEGIN
    ALTER TABLE dbo.WiadomoscPracownik ADD wprBlad varchar(4000) NULL;
END;
GO

CREATE OR ALTER VIEW mvw.vWiadomoscPracownik
AS
    SELECT w.wiaID
        , w.wiaAplikacjaID
        , w.wiaEmail
        , w.wiaTemat
        , w.wiaTresc
        , w.wiaData
        , w.wiaGrupa
        , w.wiaSesja
        , w.wiaPubliczna
        , w.wiaStatus
        , wp.wprID
        , wp.wprEmail
        , wp.wprBlad
        , wp.wprStatus
        , p.prcID
        , p.prcKod
        , p.prcNazwisko
        , p.prcImie
    FROM dbo.Wiadomosc AS w
        INNER JOIN dbo.WiadomoscPracownik AS wp ON w.wiaID = wp.wprWiadomoscID
        INNER JOIN enum.Pracownik AS p ON wp.wprPracownikID = p.prcID;
GO
