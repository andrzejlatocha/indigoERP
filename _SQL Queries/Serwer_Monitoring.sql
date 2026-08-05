-- Monitoring serwerów (WIN/SQL/ORA) — Serwer, SerwerLog, SerwerTyp + widoki
-- Uruchomić na: JAPET / indigoERP.Developer (przed kompilacją indigoERP.Api)

-- 1. dic.SerwerTyp (słownik typów serwera) + enum.SerwerTyp -----------------
CREATE TABLE dic.SerwerTyp (
    styKOD          nvarchar(3)     NOT NULL,
    styOpis         nvarchar(100)   NULL,
    styStatus       nvarchar(1)     NOT NULL CONSTRAINT DF_SerwerTyp_styStatus DEFAULT ('2'),
    DataUtworzenie  datetime        NULL,
    UtworzenieID    int             NULL,
    DataModyfikacja datetime        NULL,
    ModyfikacjaID   int             NULL,
    Usuniety        bit             NULL,
    CONSTRAINT PK_SerwerTyp PRIMARY KEY CLUSTERED (styKOD)
);
GO

INSERT INTO dic.SerwerTyp (styKOD, styOpis, styStatus) VALUES
    ('WIN', N'Windows Server', '2'),
    ('SQL', N'SQL Server',     '2'),
    ('ORA', N'Oracle',         '2');
GO

CREATE VIEW enum.SerwerTyp AS
SELECT styKOD, styOpis, styStatus
FROM dic.SerwerTyp
WHERE (Usuniety = 0 OR Usuniety IS NULL);
GO

-- 2. dbo.Serwer (rejestr monitorowanych serwerów) + mvw.vSerwerLista --------
CREATE TABLE dbo.Serwer (
    srvID           int             IDENTITY(1,1) NOT NULL,
    srvSerwerTypKOD       nvarchar(3)     NOT NULL,
    srvNazwa        nvarchar(100)   NOT NULL,
    srvAdres           nvarchar(45)    NOT NULL,
    srvPort         int             NULL,
    srvOpis         nvarchar(255)   NULL,
    srvStatus       nvarchar(1)     NOT NULL CONSTRAINT DF_Serwer_srvStatus DEFAULT ('2'),
    DataUtworzenie  datetime        NULL,
    UtworzenieID    int             NULL,
    DataModyfikacja datetime        NULL,
    ModyfikacjaID   int             NULL,
    Usuniety        bit             NULL,
    CONSTRAINT PK_Serwer PRIMARY KEY CLUSTERED (srvID),
    CONSTRAINT FK_Serwer_SerwerTyp FOREIGN KEY (srvSerwerTypKOD) REFERENCES dic.SerwerTyp (styKOD)
);
GO

CREATE VIEW mvw.vSerwerLista AS
SELECT srvID, srvSerwerTypKOD, srvNazwa, srvAdres, srvPort, srvOpis, srvStatus
FROM dbo.Serwer
WHERE (Usuniety = 0 OR Usuniety IS NULL);
GO

-- 3. dbo.SerwerLog (historia sprawdzeń, append-only, bez AuditableEntity) ---
CREATE TABLE dbo.SerwerLog (
    slgID              int             IDENTITY(1,1) NOT NULL,
    slgSerwerID        int             NOT NULL,
    slgDataSprawdzenie datetime        NOT NULL CONSTRAINT DF_SerwerLog_slgDataSprawdzenie DEFAULT (GETDATE()),
    slgDostepny        bit             NOT NULL,
    slgCzasOdpowiedzMs int             NULL,
    slgBlad            nvarchar(500)   NULL,
    CONSTRAINT PK_SerwerLog PRIMARY KEY CLUSTERED (slgID),
    CONSTRAINT FK_SerwerLog_Serwer FOREIGN KEY (slgSerwerID) REFERENCES dbo.Serwer (srvID)
);
GO

CREATE INDEX IX_SerwerLog01 ON dbo.SerwerLog (slgSerwerID, slgDataSprawdzenie DESC);
GO

-- 4. mvw.vSerwerStatus (dashboard: ostatni log per serwer) ------------------
CREATE VIEW mvw.vSerwerStatus AS
SELECT  s.srvID, s.srvNazwa, s.srvAdres, s.srvPort, s.srvSerwerTypKOD, t.styOpis,
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
