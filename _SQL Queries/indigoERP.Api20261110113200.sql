/* 03. Dane APT w indigoERP i SSO; wymagany uprzednio skrypt epicaHR 02.
   Wydział i stanowisko pochodzą z angażu; nazwa stanowiska otrzymuje sufiks (APT). */
IF DB_NAME() <> N'indigoERP' THROW 50000, 'Wymagana baza indigoERP.', 1;
IF OBJECT_ID(N'epicaHR.sha.vPracownikListaAPT', N'V') IS NULL THROW 50001, 'Najpierw wdróż widok APT w epicaHR.', 1;
IF OBJECT_ID(N'rdb.vPracownikLista', N'V') IS NULL THROW 50002, 'Brak widoku pracowników indigoERP.', 1;
EXEC(N'CREATE OR ALTER VIEW [rdb].[vPracownikLista]
AS
	 SELECT vpl.prcID
		  , vpl.prcTypUmowaKOD
		  , vpl.prcKod
		  , vpl.prcNazwisko
		  , vpl.prcImie
		  , vpl.prcDrugieImie
		  , vpl.prcDataUrodzenie
		  , vpl.prcZdjecie
		  , vpl.angID
		  , vpl.angDataRozpoczecie
		  , vpl.angDataWystawienie
		  , vpl.angNumer
		  , vpl.angPozaSchemat
		  , vpl.angPraca
		  , vpl.angKodFepKOD
		  , vpl.angDataZakonczenie
		  , vpl.angPrzyczynaZakonczenieID
		  , vpl.angStatus
		  , vpl.dtaData
		  , vpl.wdzID
		  , vpl.wdzKod
		  , vpl.wdzSymbol
		  , vpl.wdzNazwa
		  , vpl.stnNazwaPelna
		  , vpl.stnID
		  , vpl.stnKod
		  , vpl.stnSymbol
		  , vpl.stnNazwa
		  , vpl.stnRodzajStanowiskoKOD
		  , vpl.stnKontoKosztKOD
		  , vpl.stnStatus
		  , vpl.aprPrzerwa
		  , vpl.pzaOpis
	 FROM epicaHR.sha.vPracownikLista AS vpl
UNION ALL
SELECT apt.prcID,
       apt.prcTypUmowaKOD,
       apt.prcKod,
       apt.prcNazwisko,
       apt.prcImie,
       apt.prcDrugieImie,
       apt.prcDataUrodzenie,
       apt.prcZdjecie,
       apt.angID,
       apt.angDataRozpoczecie,
       apt.angDataWystawienie,
       apt.angNumer,
       apt.angPozaSchemat,
       apt.angPraca,
       apt.angKodFepKOD,
       apt.angDataZakonczenie,
       apt.angPrzyczynaZakonczenieID,
       apt.angStatus,
       apt.dtaData,
       apt.wdzID,
       apt.wdzKod,
       apt.wdzSymbol,
       apt.wdzNazwa,
       CASE WHEN apt.stnNazwaPelna IS NULL THEN NULL ELSE CONCAT(apt.stnNazwaPelna, N'' (APT)'') END AS stnNazwaPelna,
       apt.stnID,
       apt.stnKod,
       apt.stnSymbol,
       CASE WHEN apt.stnNazwa IS NULL THEN NULL ELSE CONCAT(apt.stnNazwa, N'' (APT)'') END AS stnNazwa,
       apt.stnRodzajStanowiskoKOD,
       apt.stnKontoKosztKOD,
       apt.stnStatus,
       apt.aprPrzerwa,
       apt.pzaOpis
FROM epicaHR.sha.vPracownikListaAPT AS apt;');
SELECT OBJECT_ID(N'rdb.vPracownikLista', N'V') AS WidokID;
