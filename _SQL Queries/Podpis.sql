-- Moduł "Podpisy" — szablony podpisów email (Podpis) + przypisania do użytkownika/grupy
-- (PodpisUzytkownik, PodpisGrupa) + widoki mvw. Uruchomić na CERES/indigoERP przed
-- kompilacją indigoERP.Api.

-- 1. dbo.Podpis (szablony podpisów) + mvw.vPodpis ----------------------------
CREATE TABLE dbo.Podpis (
    pdpID           int             IDENTITY(1,1) NOT NULL,
    pdpNazwa        nvarchar(200)   NOT NULL,
    pdpOpis         nvarchar(500)   NULL,
    pdpTresc        nvarchar(max)   NOT NULL,
    pdpDomyslny     bit             NOT NULL CONSTRAINT DF_Podpis_pdpDomyslny DEFAULT (0),
    pdpStatus       nvarchar(1)     NOT NULL CONSTRAINT DF_Podpis_pdpStatus DEFAULT ('2'),
    DataUtworzenie  datetime        NULL,
    UtworzenieID    int             NULL,
    DataModyfikacja datetime        NULL,
    ModyfikacjaID   int             NULL,
    Usuniety        bit             NULL,
    CONSTRAINT PK_Podpis PRIMARY KEY CLUSTERED (pdpID)
);
GO

CREATE INDEX IX_Podpis01 ON dbo.Podpis (pdpStatus);
GO

CREATE VIEW mvw.vPodpis AS
SELECT pdpID, pdpNazwa, pdpOpis, pdpTresc, pdpDomyslny, pdpStatus
FROM dbo.Podpis
WHERE (Usuniety = 0 OR Usuniety IS NULL);
GO

-- 2. dbo.PodpisUzytkownik (przypisanie bezpośrednie, nadpisuje grupowe) ------
CREATE TABLE dbo.PodpisUzytkownik (
    pduID           int             IDENTITY(1,1) NOT NULL,
    pduPodpisID     int             NOT NULL,
    pduUzytkownikID int             NOT NULL,
    pduStatus       nvarchar(1)     NOT NULL CONSTRAINT DF_PodpisUzytkownik_pduStatus DEFAULT ('2'),
    DataUtworzenie  datetime        NULL,
    UtworzenieID    int             NULL,
    DataModyfikacja datetime        NULL,
    ModyfikacjaID   int             NULL,
    Usuniety        bit             NULL,
    CONSTRAINT PK_PodpisUzytkownik PRIMARY KEY CLUSTERED (pduID),
    CONSTRAINT FK_PodpisUzytkownik_Podpis FOREIGN KEY (pduPodpisID) REFERENCES dbo.Podpis (pdpID),
    CONSTRAINT FK_PodpisUzytkownik_Uzytkownik FOREIGN KEY (pduUzytkownikID) REFERENCES dbo.Uzytkownik (usrID)
);
GO

CREATE INDEX IX_PodpisUzytkownik01 ON dbo.PodpisUzytkownik (pduUzytkownikID, pduStatus);
GO

CREATE VIEW mvw.vPodpisUzytkownik AS
SELECT
    pu.pduID,
    pu.pduPodpisID,
    p.pdpNazwa,
    pu.pduUzytkownikID,
    u.usrImieNazwisko,
    pu.pduStatus
FROM dbo.PodpisUzytkownik pu
INNER JOIN dbo.Podpis p ON p.pdpID = pu.pduPodpisID
INNER JOIN dbo.Uzytkownik u ON u.usrID = pu.pduUzytkownikID
WHERE (pu.Usuniety = 0 OR pu.Usuniety IS NULL);
GO

-- 3. dbo.PodpisGrupa (przypisanie do istniejącej Grupa, w tym grup z AD) -----
CREATE TABLE dbo.PodpisGrupa (
    pdgID           int             IDENTITY(1,1) NOT NULL,
    pdgPodpisID     int             NOT NULL,
    pdgGrupaID      int             NOT NULL,
    pdgStatus       nvarchar(1)     NOT NULL CONSTRAINT DF_PodpisGrupa_pdgStatus DEFAULT ('2'),
    DataUtworzenie  datetime        NULL,
    UtworzenieID    int             NULL,
    DataModyfikacja datetime        NULL,
    ModyfikacjaID   int             NULL,
    Usuniety        bit             NULL,
    CONSTRAINT PK_PodpisGrupa PRIMARY KEY CLUSTERED (pdgID),
    CONSTRAINT FK_PodpisGrupa_Podpis FOREIGN KEY (pdgPodpisID) REFERENCES dbo.Podpis (pdpID),
    CONSTRAINT FK_PodpisGrupa_Grupa FOREIGN KEY (pdgGrupaID) REFERENCES dbo.Grupa (groID)
);
GO

CREATE INDEX IX_PodpisGrupa01 ON dbo.PodpisGrupa (pdgGrupaID, pdgStatus);
GO

CREATE VIEW mvw.vPodpisGrupa AS
SELECT
    pg.pdgID,
    pg.pdgPodpisID,
    p.pdpNazwa,
    pg.pdgGrupaID,
    g.groNazwa,
    pg.pdgStatus
FROM dbo.PodpisGrupa pg
INNER JOIN dbo.Podpis p ON p.pdpID = pg.pdgPodpisID
INNER JOIN dbo.Grupa g ON g.groID = pg.pdgGrupaID
WHERE (pg.Usuniety = 0 OR pg.Usuniety IS NULL);
GO

-- 4. enum.Podpis (lookup do dropdownów przy przypisywaniu) ------------------
CREATE VIEW enum.Podpis AS
SELECT pdpID, pdpNazwa, pdpStatus
FROM dbo.Podpis
WHERE (Usuniety = 0 OR Usuniety IS NULL);
GO
