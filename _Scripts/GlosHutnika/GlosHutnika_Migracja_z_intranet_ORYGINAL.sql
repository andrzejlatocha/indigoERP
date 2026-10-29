-- Migracja historycznych wiadomosci 'Glos Hutnika' z WENUS/intranet (dbo.Wiadomosc/WiadomoscDokument WHERE wdz_grupa='GH') do indigoERP.
-- Wygenerowane automatycznie ze zrodla danych (160 wierszy), PRZYGOTOWANE, NIE URUCHOMIONE.
--
-- UWAGA: wdoKatalog przenosi wartosc source.wdo_katalog WERBATIM (usunieto tylko stary
-- absolutny prefiks 'd:\Shared Folders\dokumenty\'). Dla 62 z 160 wierszy (starsze wpisy,
-- mniej wiecej do konca 2023) ten tekst w bazie zrodlowej ma sklejona nazwe folderu ze
-- znacznikiem czasu BEZ separatora \ (np. '...A604E63A20231013124803' zamiast
-- '...A604E63A\13102023124803') -- nie wiadomo bez sprawdzenia fizycznych folderow, czy to
-- byl tylko blad zapisu w kolumnie, czy tak samo nazwany jest realny folder na dysku.
-- Zweryfikowac po migracji, ze linki do zalacznikow tych 62 starszych wpisow dzialaja;
-- jesli nie, poprawic dbo.WiadomoscDokument.wdoKatalog recznie dla tych wierszy.
USE indigoERP;
GO

INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 22 z 15 czerwca 2022', N'', '2022-06-23 11:49:33.040', '2', 1, '2022-06-23 11:49:33.040', '2022-06-23 11:49:33.040', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 22 z 15 czerwca 2022', N'GH-22-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220623104126', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 23 z 24 czerwca 2022', N'PAMIĘTAJ w Liberty Częstochowa trwa ankieta
„Peakon – Twój Głos”. Każdy pracownik
do 10 lipca ma okazję anonimowo wyrazić
swoją opinię o przedsiębiorstwie.', '2022-06-24 10:22:54.917', '2', 1, '2022-06-24 10:22:54.917', '2022-06-24 10:22:54.917', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 23 z 24 czerwca 2022', N'GH-23-2022.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20220624101850', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 24 z 1 lipca 2022', N'PAMIĘTAJ w Liberty Częstochowa trwa ankieta
„Peakon – Twój Głos”. Każdy pracownik
do 10 lipca ma okazję anonimowo wyrazić
swoją opinię o przedsiębiorstwie.', '2022-07-01 11:38:13.123', '2', 1, '2022-07-01 11:38:13.123', '2022-07-01 11:38:13.123', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 24-2022 z 1 lipca', N'GH-24-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220701113714', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 25 z 8 lipca 2022', N'PAMIĘTAJ w Liberty Częstochowa trwa ankieta
„Peakon – Twój Głos”. Każdy pracownik
do 10 lipca ma okazję anonimowo wyrazić
swoją opinię o przedsiębiorstwie.', '2022-07-08 10:53:29.010', '2', 1, '2022-07-08 10:53:29.010', '2022-07-08 10:53:29.010', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 25 z 8 lipca 2022', N'GH-25-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220708105214', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 26 z 15 lipca 2022', N'GH-26-2022.pdf', '2022-07-15 11:20:03.623', '2', 1, '2022-07-15 11:20:03.623', '2022-07-15 11:20:03.623', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 26 z 15 lipca 2022', N'GH-26-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220715111812', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 27 z 22 lipca 2022', N'GH-27-2022.pdf', '2022-07-22 12:08:26.890', '2', 1, '2022-07-22 12:08:26.890', '2022-07-22 12:08:26.890', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 27 z 22 lipca 2022', N'GH-27-2022.pdf', N'0000000\F76A18BBEDA948C2B951C47150A2777820220722120802', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 28 z 29 lipca 2022', N'GH-28-2022.pdf', '2022-07-29 12:16:11.443', '2', 1, '2022-07-29 12:16:11.443', '2022-07-29 12:16:11.443', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 28 z 29 lipca 2022', N'GH-28-2022.pdf', N'0000000\F76A18BBEDA948C2B951C47150A2777820220729121548', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 29 z 5 sierpnia 2022', N'GH-29-2022.pdf', '2022-08-05 12:06:53.240', '2', 1, '2022-08-05 12:06:53.240', '2022-08-05 12:06:53.240', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 29 z 5 sierpnia 2022', N'GH-29-2022.pdf', N'0000000\F76A18BBEDA948C2B951C47150A2777820220805120607', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 30 z 19 sierpnia 2022', N'GH-30-2022.pdf', '2022-08-19 14:02:17.990', '2', 1, '2022-08-19 14:02:17.990', '2022-08-19 14:02:17.990', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 30 z 19 sierpnia 2022', N'GH-30-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220819140027', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 31 z 2 września 2022', N'Zapraszamy do lektury', '2022-09-02 11:39:29.263', '2', 1, '2022-09-02 11:39:29.263', '2022-09-02 11:39:29.263', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 31 z 2 września 2022', N'GH-31-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220902113806', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 32 z 9 września 2022', N'Dziś głównie o bezpieczeństwie w pracy', '2022-09-09 12:55:20.683', '2', 1, '2022-09-09 12:55:20.683', '2022-09-09 12:55:20.683', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 32 z 9 września 2022', N'GH-32-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220909125415', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 33 z 16 września 2022', N'GH-33-2022.pdf', '2022-09-16 13:24:34.970', '2', 1, '2022-09-16 13:24:34.970', '2022-09-16 13:24:34.970', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 33 z 16 września 2022', N'GH-33-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220916132412', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 34 z 23 września 2022', N'Zapraszamy do lektury!', '2022-09-23 11:15:24.883', '2', 1, '2022-09-23 11:15:24.883', '2022-09-23 11:15:24.883', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 34 z 23 września 2022', N'GH-34-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220923111416', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 35 z 30 września 2022', N'GH-35-2022.pdf', '2022-09-30 10:56:36.050', '2', 1, '2022-09-30 10:56:36.050', '2022-09-30 10:56:36.050', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 35 z 30 września 2022', N'GH-35-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820220930105611', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 36 z 07 pździernika 2022', N'Zapraszamy do lektury', '2022-10-07 09:26:36.660', '2', 1, '2022-10-07 09:26:36.660', '2022-10-07 09:26:36.660', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 36 z 07 pździernika 2022', N'GH-36-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221007092513', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 37 z 14 pździernika 2022', N'Zapraszamy do lektury', '2022-10-14 10:53:13.560', '2', 1, '2022-10-14 10:53:13.560', '2022-10-14 10:53:13.560', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 37 z 14 pździernika 2022', N'GH-37-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221014105237', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 38 z 21 pździernika 2022', N'Zapraszamy do lektury', '2022-10-21 13:20:23.860', '2', 1, '2022-10-21 13:20:23.860', '2022-10-21 13:20:23.860', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 38 z 21 pździernika 2022', N'GH-38-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221021131952', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 39 z 28 pździernika 2022', N'Zapraszamy do lektury', '2022-10-28 11:49:05.183', '2', 1, '2022-10-28 11:49:05.183', '2022-10-28 11:49:05.183', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 39 z 28 pździernika 2022', N'GH-39-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221028114807', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 40 z 4 listopada 2022', N'Zapraszamy do lektury', '2022-11-04 13:04:56.993', '2', 1, '2022-11-04 13:04:56.993', '2022-11-04 13:04:56.993', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 40 z 4 listopada 2022', N'GH-40-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221104130420', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 41 z 10 listopada 2022', N'Zapraszamy do lektury nowego wydania Głosu Hutnika', '2022-11-10 14:56:20.883', '2', 1, '2022-11-10 14:56:20.883', '2022-11-10 14:56:20.883', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 41 z 10 listopada 2022', N'GH-41-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221110145537', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 42 z 18 listopada 2022', N'Zapraszamy do lektury', '2022-11-18 13:21:20.420', '2', 1, '2022-11-18 13:21:20.420', '2022-11-18 13:21:20.420', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 42 z 18 listopada 2022', N'GH-42-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221118132030', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 43 z 25 listopada 2022', N'Zapraszamy do lektury', '2022-11-25 12:44:13.250', '2', 1, '2022-11-25 12:44:13.250', '2022-11-25 12:44:13.250', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 43 z 25 listopada 2022', N'GH-43-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221125124334', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 44 z 2 grudnia 2022', N'Zapraszamy do lektury', '2022-12-02 12:55:15.093', '2', 1, '2022-12-02 12:55:15.093', '2022-12-02 12:55:15.093', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 44 z 2 grudnia 2022', N'GH-44-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221202125436', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 45 z 9 grudnia 2022', N'Zapraszamy do lektury', '2022-12-09 12:29:19.790', '2', 1, '2022-12-09 12:29:19.790', '2022-12-09 12:29:19.790', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 45 z 9 grudnia 2022', N'GH-45-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221209122805', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 46 z 16 grudnia 2022', N'Zapraszamy do lektury', '2022-12-16 11:47:52.973', '2', 1, '2022-12-16 11:47:52.973', '2022-12-16 11:47:52.973', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 46 z 16 grudnia 2022', N'GH-46-2022.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820221216114717', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 1 z 5 sytcznia 2023', N'Witamy w nowym roku i zapraszamy do lektury', '2023-01-05 12:41:03.160', '2', 1, '2023-01-05 12:41:03.160', '2023-01-05 12:41:03.160', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 1 z 5 sytcznia 2023', N'GH-01-2023.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820230105124011', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 2 z 13 sytcznia 2023', N'Dziś piątek trzynastego ale nowy numer Głosu Hutnika znowu w Waszych skrzynkach', '2023-01-13 11:39:47.640', '2', 1, '2023-01-13 11:39:47.640', '2023-01-13 11:39:47.640', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 2 z 13 sytcznia 2023', N'GH-02-2023.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820230113113831', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 3 z 20 sytcznia 2023', N'Zapraszamy do lektury', '2023-01-20 11:57:48.500', '2', 1, '2023-01-20 11:57:48.500', '2023-01-20 11:57:48.500', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 3 z 20 sytcznia 2023', N'GH-03-2023.pdf', N'000000\F76A18BBEDA948C2B951C47150A2777820230120115714', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 4 z 27 sytcznia 2023', N'Zapraszamy do lektury', '2023-01-27 11:48:40.017', '2', 1, '2023-01-27 11:48:40.017', '2023-01-27 11:48:40.017', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 4 z 27 sytcznia 2023', N'GH-04-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230127114532', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 5 z 3 lutego 2023', N'GH-05-2023.pdf', '2023-02-03 08:34:29.700', '2', 1, '2023-02-03 08:34:29.700', '2023-02-03 08:34:29.700', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 5 z 3 lutego', N'GH-05-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230203083308', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 6 z 10 lutego 2023', N'Życzymy miłej lektury', '2023-02-10 09:23:36.530', '2', 1, '2023-02-10 09:23:36.530', '2023-02-10 09:23:36.530', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 6 z 10 lutego 2023', N'GH-06-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230210092142', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 7 z 17 lutego 2023', N'Życzymy miłej lektury', '2023-02-17 11:12:03.377', '2', 1, '2023-02-17 11:12:03.377', '2023-02-17 11:12:03.377', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 7 z 17 lutego 2023', N'GH-07-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230217111038', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 8 z 24 lutego 2023', N'Życzymy miłej lektury', '2023-02-24 10:13:19.307', '2', 1, '2023-02-24 10:13:19.307', '2023-02-24 10:13:19.307', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 8 z 24 lutego 2023', N'GH-08-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230224101200', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 9 z 3 marca 2023', N'Życzymy miłej lektury', '2023-03-03 11:34:33.543', '2', 1, '2023-03-03 11:34:33.543', '2023-03-03 11:34:33.543', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 9 z 3 marca 2023', N'GH-09-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230303113330', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 10 z 10 marca 2023', N'Życzymy miłej lektury', '2023-03-10 12:59:39.837', '2', 1, '2023-03-10 12:59:39.837', '2023-03-10 12:59:39.837', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 10 z 10 marca 2023', N'GH nr 10-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230310122313', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 11 z 17 marca 2023', N'Życzymy miłej lektury', '2023-03-17 13:47:45.457', '2', 1, '2023-03-17 13:47:45.457', '2023-03-17 13:47:45.457', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 11 z 17 marca 2023', N'GH 11-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230317134628', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 12 z 24 marca 2023', N'Życzymy miłej lektury', '2023-03-24 12:04:56.797', '2', 1, '2023-03-24 12:04:56.797', '2023-03-24 12:04:56.797', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 12 z 24 marca 2023', N'GH 12-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230324120405', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 13 z 31 marca 2023', N'Życzymy miłej lektury.', '2023-03-31 14:10:00.560', '2', 1, '2023-03-31 14:10:00.560', '2023-03-31 14:10:00.560', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 13 z 31 marca 2023', N'GH-13-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230331140801', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 14 z 7 kwietnia', N'Życzymy Wesołych Świąt i miłej lektury', '2023-04-07 10:27:18.883', '2', 1, '2023-04-07 10:27:18.883', '2023-04-07 10:27:18.883', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 14 z 7 kwietnia', N'GH-14-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230407102532', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 15 z 14 kwietnia 2023', N'GH-15-2023.pdf', '2023-04-14 11:55:13.027', '2', 1, '2023-04-14 11:55:13.027', '2023-04-14 11:55:13.027', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 15 z 14 kwietnia 2023', N'GH-15-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230414115156', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 16 z 21 kwietnia', N'Życzymy miłej lektury', '2023-04-21 14:15:20.970', '2', 1, '2023-04-21 14:15:20.970', '2023-04-21 14:15:20.970', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 16 z 21 kwietnia', N'GH-16-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230421141338', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 17 z 28 kwietnia 2023', N'Życzymy miłej lektury na Majówkę', '2023-04-28 11:50:00.313', '2', 1, '2023-04-28 11:50:00.313', '2023-04-28 11:50:00.313', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 17 z 28 kwietnia 2023', N'GH-17-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230428114741', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 17 z 28 kwietnia 2023', N'Życzymy miłej lektury na Majówkę', '2023-04-28 12:00:12.830', '2', 1, '2023-04-28 12:00:12.830', '2023-04-28 12:00:12.830', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 17 z 28 kwietnia 2023', N'GH-17-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230428115815', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 18 z 12 maja 2023', N'Życzymy miłej lektury!', '2023-05-12 13:52:48.713', '2', 1, '2023-05-12 13:52:48.713', '2023-05-12 13:52:48.713', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 18 z 12 maja 2023', N'GH-18-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230512135136', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 19 z 19 maja 2023', N'GH-19-2023.pdf', '2023-05-19 11:20:07.410', '2', 1, '2023-05-19 11:20:07.410', '2023-05-19 11:20:07.410', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 19 z 19 maja 2023', N'GH-19-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230519111831', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 20 z 26 maja 2023', N'Życzymy miłej lektury', '2023-05-26 06:02:52.810', '2', 1, '2023-05-26 06:02:52.810', '2023-05-26 06:02:52.810', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 20 z 26 maja 2023', N'GH-20-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230526060110', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 21 z 2 czerwca 2023', N'Miłej lektury', '2023-06-02 16:10:23.847', '2', 1, '2023-06-02 16:10:23.847', '2023-06-02 16:10:23.847', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 21 z 2 czerwca 2023', N'GH-21-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230602160841', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 22 z 23 czerwca 2023', N'Życzymy miłej lektury', '2023-06-23 15:00:52.910', '2', 1, '2023-06-23 15:00:52.910', '2023-06-23 15:00:52.910', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 22 z 23 czerwca 2023', N'GH-22-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230623145917', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 23 z 30 czerwca 2023', N'Życzymy miłej lektury', '2023-06-30 11:31:46.803', '2', 1, '2023-06-30 11:31:46.803', '2023-06-30 11:31:46.803', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 23 z 30 czerwca 2023', N'GH-23-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230630112959', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 24 z 7 lipca 2023', N'Życzymy miłej lektury', '2023-07-07 13:34:44.253', '2', 1, '2023-07-07 13:34:44.253', '2023-07-07 13:34:44.253', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 24 z 7 lipca 2023', N'GH-24-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230707133303', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 25 z 14 lipca 2023', N'Życzymy miłej lektury', '2023-07-14 14:22:13.023', '2', 1, '2023-07-14 14:22:13.023', '2023-07-14 14:22:13.023', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 25 z 14 lipca 2023', N'GH-25-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230714142033', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 26 z 21 lipca 2023', N'Życzymy miłej lektury', '2023-07-21 12:36:21.100', '2', 1, '2023-07-21 12:36:21.100', '2023-07-21 12:36:21.100', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 26 z 21 lipca 2023', N'GH-26-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230721123308', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 27 z 28 lipca 2023', N'GH-27-2023.pdf', '2023-07-28 14:32:38.417', '2', 1, '2023-07-28 14:32:38.417', '2023-07-28 14:32:38.417', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 27 z 28 lipca 2023', N'GH-27-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230728143112', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 28 na 4 sierpnia 2023', N'GH-28-2023.pdf', '2023-08-04 13:43:38.577', '2', 1, '2023-08-04 13:43:38.577', '2023-08-04 13:43:38.577', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 28 na 4 sierpnia 2023', N'GH-28-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230804134206', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 29 na 11 sierpnia 2023', N'Życzymy miłej lektury', '2023-08-11 13:12:05.867', '2', 1, '2023-08-11 13:12:05.867', '2023-08-11 13:12:05.867', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 29 na 11 sierpnia 2023', N'GH-29-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230811131108', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 30 z 1 września 2023', N'GH-30-2023.pdf', '2023-09-01 14:30:11.890', '2', 1, '2023-09-01 14:30:11.890', '2023-09-01 14:30:11.890', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 30 z 1 września 2023', N'GH-30-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230901142723', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 31 z 8 września 2023', N'Życzymy miłej lektury', '2023-09-08 10:55:23.320', '2', 1, '2023-09-08 10:55:23.320', '2023-09-08 10:55:23.320', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 31 z 8 września 2023', N'GH-31-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230908104916', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 32 z 15 września 2023', N'Życzymy miłej lektury', '2023-09-15 10:14:45.850', '2', 1, '2023-09-15 10:14:45.850', '2023-09-15 10:14:45.850', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 32 z 15 września 2023', N'GH-32-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230915101317', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 33 z 22 września 2023', N'Życzymy miłej lektury', '2023-09-22 11:32:38.743', '2', 1, '2023-09-22 11:32:38.743', '2023-09-22 11:32:38.743', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 33 z 22 września 2023', N'GH-33-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230922113033', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 34 z 29 września 2023', N'Życzymy miłej lektury', '2023-09-29 13:00:24.687', '2', 1, '2023-09-29 13:00:24.687', '2023-09-29 13:00:24.687', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 34 z 29 września 2023', N'GH-34-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20230929125542', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 35 z 6 października 2023', N'Życzymy miłej lektury', '2023-10-06 10:54:42.580', '2', 1, '2023-10-06 10:54:42.580', '2023-10-06 10:54:42.580', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 35 z 6 października 2023', N'GH-35-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20231006105229', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 36 z 13 października 2023', N'Życzymy miłej lektury', '2023-10-13 12:58:13.927', '2', 1, '2023-10-13 12:58:13.927', '2023-10-13 12:58:13.927', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 36 z 13 października 2023', N'GH-36-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A20231013124803', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głoś Hutnika nr 37 z 20 października 2023', N'GH-37-2023.pdf', '2023-10-20 12:54:19.787', '2', 1, '2023-10-20 12:54:19.787', '2023-10-20 12:54:19.787', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głoś Hutnika nr 37 z 20 października 2023', N'GH-37-2023.pdf', N'000000\F76A18BBEDA948C2B951C47150A27778\20102023125402', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głoś Hutnika nr 37 z 20 października 2023', N'GH-37-2023.pdf', '2023-10-23 12:54:19.787', '2', 1, '2023-10-23 12:54:19.787', '2023-10-23 12:54:19.787', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głoś Hutnika nr 37 z 20 października 2023', N'GH-37-2023.pdf', N'000000\F76A18BBEDA948C2B951C47150A27778\20102023125402', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 38 z 27 października 2023', N'Życzymy miłej lektury', '2023-10-27 13:30:48.763', '2', 1, '2023-10-27 13:30:48.763', '2023-10-27 13:30:48.763', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 38 z 27 października 2023', N'GH-38-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\27102023132859', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 39 z 10 listopada 2023', N'Życzymy miłej lektury', '2023-11-10 12:20:30.367', '2', 1, '2023-11-10 12:20:30.367', '2023-11-10 12:20:30.367', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 39 z 10 listopada 2023', N'GH-39-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\10112023121659', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 40-2023', N'Życzymy miłej lektury', '2023-11-17 14:45:15.230', '2', 1, '2023-11-17 14:45:15.230', '2023-11-17 14:45:15.230', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 40-2023', N'GH-40-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\17112023144329', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 41 z 1 grudnia 2023', N'Życzymy miłej lektury', '2023-12-01 12:32:20.487', '2', 1, '2023-12-01 12:32:20.487', '2023-12-01 12:32:20.487', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 41 z 1 grudnia 2023', N'GH-41-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\01122023122729', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 42 z 8 grudnia', N'Życzymy miłej lektury', '2023-12-08 13:47:45.137', '2', 1, '2023-12-08 13:47:45.137', '2023-12-08 13:47:45.137', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 42 z 8 grudnia', N'GH-42-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\08122023134553', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 43 z 15 grudnia', N'Z życzeniami świątecznymi.', '2023-12-15 13:55:32.230', '2', 1, '2023-12-15 13:55:32.230', '2023-12-15 13:55:32.230', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 43 z 15 grudnia', N'GH-43-2023.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\15122023135005', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 1 z 9 lutego 2024', N'GH-01-2024.pdf', '2024-02-09 10:33:56.550', '2', 1, '2024-02-09 10:33:56.550', '2024-02-09 10:33:56.550', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 1 z 9 lutego 2024', N'GH-01-2024.pdf', N'000000\F76A18BBEDA948C2B951C47150A27778\09022024103322', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 2 z 23 lutego 2024', N'GH-02-2024.pdf', '2024-02-23 11:00:51.557', '2', 1, '2024-02-23 11:00:51.557', '2024-02-23 11:00:51.557', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 2 z 23 lutego 2024', N'GH-02-2024.pdf', N'000000\F76A18BBEDA948C2B951C47150A27778\23022024110001', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 3 z 1 marca 2024', N'GH-03-2024.pdf', '2024-03-01 12:02:42.547', '2', 1, '2024-03-01 12:02:42.547', '2024-03-01 12:02:42.547', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 3 z 1 marca 2024', N'GH-03-2024.pdf', N'000000\F76A18BBEDA948C2B951C47150A27778\01032024120209', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 4 z 15 marca 2024', N'Życzymy miłej lektury', '2024-03-15 14:15:03.860', '2', 1, '2024-03-15 14:15:03.860', '2024-03-15 14:15:03.860', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 4 z 15 marca 2024', N'GH-04-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\15032024141301', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 5 z 22 marca 2024', N'Życzymy miłej lektury', '2024-03-22 12:26:24.373', '2', 1, '2024-03-22 12:26:24.373', '2024-03-22 12:26:24.373', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 5 z 22 marca 2024', N'GH-05-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\22032024122136', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 6 z 29 marca 2024', N'Życzymy Wesołych Świąt i miłej lektury', '2024-03-29 12:08:24.907', '2', 1, '2024-03-29 12:08:24.907', '2024-03-29 12:08:24.907', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 6 z 29 marca 2024', N'GH-06-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\29032024115823', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 7 z 5 kwietnia 2024', N'Życzymy miłej lektury', '2024-04-05 14:46:30.837', '2', 1, '2024-04-05 14:46:30.837', '2024-04-05 14:46:30.837', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 7 z 5 kwietnia 2024', N'GH-07-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\05042024144226', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 8 z 12 kwietnia', N'Życzymy miłej lektury', '2024-04-12 14:05:12.527', '2', 1, '2024-04-12 14:05:12.527', '2024-04-12 14:05:12.527', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 8 z 12 kwietnia', N'GH-08-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\12042024140344', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 9 z 19 kwietnia 2024', N'Życzymy miłej lektury', '2024-04-19 16:40:35.117', '2', 1, '2024-04-19 16:40:35.117', '2024-04-19 16:40:35.117', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 9 z 19 kwietnia 2024', N'GH-09-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\19042024163914', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 10 z 26 kwietnia 2024', N'Życzymy miłej lektury', '2024-04-26 15:43:01.650', '2', 1, '2024-04-26 15:43:01.650', '2024-04-26 15:43:01.650', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 10 z 26 kwietnia 2024', N'GH-10-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\26042024154206', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 11 z 17 maja 2024', N'Życzymy miłej lektury', '2024-05-17 13:10:23.163', '2', 1, '2024-05-17 13:10:23.163', '2024-05-17 13:10:23.163', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 11 z 17 maja 2024', N'GH-112024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\17052024130909', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 12 z 24 maja', N'GH-12-2024.pdf', '2024-05-24 18:04:54.653', '2', 1, '2024-05-24 18:04:54.653', '2024-05-24 18:04:54.653', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 12 z 24 maja', N'GH-12-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\24052024180415', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 13 z 7 czerwca 2024', N'Życzymy miłej lektury', '2024-06-07 14:06:35.457', '2', 1, '2024-06-07 14:06:35.457', '2024-06-07 14:06:35.457', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 13 z 7 czerwca 2024', N'GH-13-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\07062024140459', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 14 z 26 czerwca 2024', N'Życzymy miłej lektury', '2024-06-26 14:09:00.060', '2', 1, '2024-06-26 14:09:00.060', '2024-06-26 14:09:00.060', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 14 z 26 czerwca 2024', N'GH-14-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\26062024140733', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 15 z 5 lipca 2024', N'Życzymy miłej lektury', '2024-07-05 12:41:45.353', '2', 1, '2024-07-05 12:41:45.353', '2024-07-05 12:41:45.353', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 15 z 5 lipca 2024', N'GH-15-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\05072024123915', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 16-2024 z 1 sierpnia 2024', N'Życzymy miłej lektury', '2024-08-01 10:11:39.790', '2', 1, '2024-08-01 10:11:39.790', '2024-08-01 10:11:39.790', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 16-2024 z 1 sierpnia 2024', N'GH-16-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\01082024100922', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 17 z 14 sierpnia 2024', N'Życzymy miłej lektury', '2024-08-14 12:07:41.753', '2', 1, '2024-08-14 12:07:41.753', '2024-08-14 12:07:41.753', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 17 z 14 sierpnia 2024', N'GH-17-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\14082024120618', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 19-2024 z 20 września 2024', N'Życzymy miłej lektury', '2024-09-20 13:27:09.493', '2', 1, '2024-09-20 13:27:09.493', '2024-09-20 13:27:09.493', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 19-2024 z 20 września 2024', N'GH-19-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\20092024132511', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 20 z 4 października 2024', N'GH-20-2024.pdf', '2024-10-04 15:15:28.003', '2', 1, '2024-10-04 15:15:28.003', '2024-10-04 15:15:28.003', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 20 z 4 października 2024', N'GH-20-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\04102024151239', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 21 z 11 października 2024', N'Życzymy miłej lektury', '2024-10-11 17:37:26.170', '2', 1, '2024-10-11 17:37:26.170', '2024-10-11 17:37:26.170', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 21 z 11 października 2024', N'GH-21-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\11102024173424', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 22 z 18 października 2024', N'Życzymy miłej lektury', '2024-10-18 15:31:09.667', '2', 1, '2024-10-18 15:31:09.667', '2024-10-18 15:31:09.667', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 22 z 18 października 2024', N'GH-22-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\18102024152929', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 23 z 25 października 2024', N'Życzymy miłej lektury', '2024-10-25 17:08:11.530', '2', 1, '2024-10-25 17:08:11.530', '2024-10-25 17:08:11.530', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 23 z 25 października 2024', N'GH-23-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\25102024170645', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 24 z 8 listopada 2024', N'Życzymy miłej lektury', '2024-11-08 15:45:41.910', '2', 1, '2024-11-08 15:45:41.910', '2024-11-08 15:45:41.910', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 24 z 8 listopada 2024', N'GH-24-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\08112024154413', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 25 z 15 listopada 2024', N'Życzymy miłej lektury', '2024-11-15 15:34:17.837', '2', 1, '2024-11-15 15:34:17.837', '2024-11-15 15:34:17.837', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 25 z 15 listopada 2024', N'GH-25-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\15112024153230', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 26 z 22 listopad 2024', N'Życzymy miłej lektury', '2024-11-22 13:19:21.427', '2', 1, '2024-11-22 13:19:21.427', '2024-11-22 13:19:21.427', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 26 z 22 listopad 2024', N'GH-26-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\22112024131735', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 27 z 6 grudnia 2024', N'Życzymy miłej lektury', '2024-12-06 14:03:08.853', '2', 1, '2024-12-06 14:03:08.853', '2024-12-06 14:03:08.853', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 27 z 6 grudnia 2024', N'GH-27-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\06122024140133', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 28 z 20 grudnia 2024', N'Życzymy wesołych świąt i miłej lektury', '2024-12-20 18:58:10.770', '2', 1, '2024-12-20 18:58:10.770', '2024-12-20 18:58:10.770', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 28 z 20 grudnia 2024', N'GH-28-2024.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\20122024185524', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 1 z 10 stycznia 2025', N'Życzymy miłej lektury', '2025-01-10 19:33:29.883', '2', 1, '2025-01-10 19:33:29.883', '2025-01-10 19:33:29.883', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 1 z 10 stycznia 2025', N'GH-01-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\10012025193151', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 2 z 24 stycznia 2025', N'Życzymy miłej lektury', '2025-01-24 14:19:07.113', '2', 1, '2025-01-24 14:19:07.113', '2025-01-24 14:19:07.113', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 2 z 24 stycznia 2025', N'GH-02-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\24012025141735', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 3 z 31 stycznia 2025', N'GH-03-2025.pdf', '2025-01-31 15:14:50.487', '2', 1, '2025-01-31 15:14:50.487', '2025-01-31 15:14:50.487', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 3 z 31 stycznia 2025', N'GH-03-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\31012025151325\', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 4 z 14 lutego 2025', N'Życzymy miłej lektury', '2025-02-14 11:21:39.350', '2', 1, '2025-02-14 11:21:39.350', '2025-02-14 11:21:39.350', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 4 z 14 lutego 2025', N'GH-04-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\14022025111943', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 5 z 21 lutego 2025', N'Życzymy miłej lektury', '2025-02-21 15:45:34.310', '2', 1, '2025-02-21 15:45:34.310', '2025-02-21 15:45:34.310', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 5 z 21 lutego 2025', N'GH-05-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\21022025154401', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 6 z 3 marca 2025', N'GH-06-2025.pdf', '2025-03-03 17:45:11.270', '2', 1, '2025-03-03 17:45:11.270', '2025-03-03 17:45:11.270', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 6 z 3 marca 2025', N'GH-06-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\03032025174357', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 7 z 14 marca 2025', N'Życzymy miłej lektury', '2025-03-14 09:01:14.177', '2', 1, '2025-03-14 09:01:14.177', '2025-03-14 09:01:14.177', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 7 z 14 marca 2025', N'GH-07-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\14032025085835', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 8 z 25 marca 2025', N'Życzymy miłej lektury', '2025-03-25 17:08:58.463', '2', 1, '2025-03-25 17:08:58.463', '2025-03-25 17:08:58.463', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 8 z 25 marca 2025', N'GH-08-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\25032025170724', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 9 z 4 kwietnia 2025', N'Życzymy miłej lektury', '2025-04-03 16:28:56.747', '2', 1, '2025-04-03 16:28:56.747', '2025-04-03 16:28:56.747', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 9 z 4 kwietnia 2025', N'GH-09-2025a.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\03042025162519', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 10 z 18 kwietnia 2025', N'Zapraszamy do lektury', '2025-04-19 09:12:36.873', '2', 1, '2025-04-19 09:12:36.873', '2025-04-19 09:12:36.873', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 10 z 18 kwietnia 2025', N'GH-10-2025.pdf', N'000000\F76A18BBEDA948C2B951C47150A27778\19042025091215', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 11 z 25 kwietnia 2025', N'Życzymy miłej lektury', '2025-04-25 13:53:58.693', '2', 1, '2025-04-25 13:53:58.693', '2025-04-25 13:53:58.693', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 11 z 25 kwietnia 2025', N'GH-11-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\25042025135205', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 12 z 9 maja 2025', N'Życzymy miłej lektury', '2025-05-09 16:57:28.777', '2', 1, '2025-05-09 16:57:28.777', '2025-05-09 16:57:28.777', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 12 z 9 maja 2025', N'GH-12-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\09052025165600', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 13 z 16 maja 2025', N'GH-13-2025.pdf', '2025-05-16 16:52:11.600', '2', 1, '2025-05-16 16:52:11.600', '2025-05-16 16:52:11.600', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 13 z 16 maja 2025', N'GH-13-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\16052025165059', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 14 z 23 maja 2025', N'GH-14-2025.pdf', '2025-05-23 15:26:43.920', '2', 1, '2025-05-23 15:26:43.920', '2025-05-23 15:26:43.920', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 14 z 23 maja 2025', N'GH-14-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\23052025152550', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 15 z 30 maja 2025', N'Życzymy miłej lektury', '2025-05-30 17:47:17.437', '2', 1, '2025-05-30 17:47:17.437', '2025-05-30 17:47:17.437', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 15 z 30 maja 2025', N'GH-15-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\30052025174622', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 16 z 6 czerwca 2025', N'GH-16-2025.pdf', '2025-06-06 16:19:19.363', '2', 1, '2025-06-06 16:19:19.363', '2025-06-06 16:19:19.363', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 16 z 6 czerwca 2025', N'GH-16-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\06062025161833', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 17 z 18 czerwca 2025', N'GH-17-2025.pdf', '2025-06-18 15:27:04.507', '2', 1, '2025-06-18 15:27:04.507', '2025-06-18 15:27:04.507', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 17 z 18 czerwca 2025', N'GH-17-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\18062025152535', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 18 z 4 lipca 2025', N'Życzymy miłej lektury', '2025-07-03 17:14:46.530', '2', 1, '2025-07-03 17:14:46.530', '2025-07-03 17:14:46.530', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 18 z 4 lipca 2025', N'GH-18-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\03072025171219', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 19 z 18 lipca 2025', N'Życzymy miłej lektury', '2025-07-18 16:27:11.847', '2', 1, '2025-07-18 16:27:11.847', '2025-07-18 16:27:11.847', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 19 z 18 lipca 2025', N'GH-19-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\18072025162614', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 20 z 25 lipca 2025', N'GH-20-2025.pdf', '2025-07-25 13:27:54.827', '2', 1, '2025-07-25 13:27:54.827', '2025-07-25 13:27:54.827', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 20 z 25 lipca 2025', N'GH-20-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\25072025132631', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 21 z 1 sierpnia 2025', N'GH-21-2025.pdf', '2025-08-01 15:50:11.797', '2', 1, '2025-08-01 15:50:11.797', '2025-08-01 15:50:11.797', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 21 z 1 sierpnia 2025', N'GH-21-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\01082025154908', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 22 z 11 sierpnia', N'GH-22-2025.pdf', '2025-08-11 09:22:47.290', '2', 1, '2025-08-11 09:22:47.290', '2025-08-11 09:22:47.290', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 22 z 11 sierpnia', N'GH-22-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\11082025092143', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 23 z 14 sierpnia 2025', N'GH-23-2025.pdf', '2025-08-14 18:06:54.140', '2', 1, '2025-08-14 18:06:54.140', '2025-08-14 18:06:54.140', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 23 z 14 sierpnia 2025', N'GH-23-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\14082025180616', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 24 z 5 września 2025', N'Życzymy miłej lektury', '2025-09-05 15:24:46.673', '2', 1, '2025-09-05 15:24:46.673', '2025-09-05 15:24:46.673', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 24 z 5 września 2025', N'GH-24-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\05092025152340', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 25 z 12 września 2025', N'Życzymy miłej lektury', '2025-09-12 16:17:42.887', '2', 1, '2025-09-12 16:17:42.887', '2025-09-12 16:17:42.887', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 25 z 12 września 2025', N'GH-25-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\12092025161622', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 26 z 19 września 2025', N'Życzymy miłej lektury', '2025-09-19 14:38:41.770', '2', 1, '2025-09-19 14:38:41.770', '2025-09-19 14:38:41.770', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 26 z 19 września 2025', N'GH-26-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\19092025082811', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 27 z 26 września', N'GH-27-2025.pdf', '2025-09-26 17:11:47.377', '2', 1, '2025-09-26 17:11:47.377', '2025-09-26 17:11:47.377', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 27 z 26 września', N'GH-27-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\26092025171107', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 28 z 3 października 2025', N'GH-28-2025.pdf', '2025-10-03 15:46:49.087', '2', 1, '2025-10-03 15:46:49.087', '2025-10-03 15:46:49.087', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 28 z 3 października 2025', N'GH-28-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\03102025154543', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 29 z 17 października 2025', N'Życzymy miłej lektury', '2025-10-17 15:43:17.510', '2', 1, '2025-10-17 15:43:17.510', '2025-10-17 15:43:17.510', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 29 z 17 października 2025', N'GH-29-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\17102025154225', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 30 z 24 października 2025', N'GH-30-2025.pdf', '2025-10-24 16:07:55.430', '2', 1, '2025-10-24 16:07:55.430', '2025-10-24 16:07:55.430', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 30 z 24 października 2025', N'GH-30-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\24102025160711', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 31 na 31 października', N'GH-31-2025.pdf', '2025-10-31 13:15:10.010', '2', 1, '2025-10-31 13:15:10.010', '2025-10-31 13:15:10.010', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 31 na 31 października', N'GH-31-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\31102025131407', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 32 z 7 listopada 2025', N'GH-32-2025.pdf', '2025-11-07 14:18:36.123', '2', 1, '2025-11-07 14:18:36.123', '2025-11-07 14:18:36.123', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 32 z 7 listopada 2025', N'GH-32-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\07112025141751', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 33 z 21 listopada 2025', N'Życzymy miłej lektury', '2025-11-21 14:52:01.653', '2', 1, '2025-11-21 14:52:01.653', '2025-11-21 14:52:01.653', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 33 z 21 listopada 2025', N'GH-33-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\21112025145027', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 33 z 21 listopada 2025', N'GH-33-2025.pdf', '2025-11-21 14:56:41.350', '2', 1, '2025-11-21 14:56:41.350', '2025-11-21 14:56:41.350', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 33 z 21 listopada 2025', N'GH-33-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\21112025145608', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 34 z 28 listopada 2025', N'Życzymy miłej lektury', '2025-11-28 14:52:53.407', '2', 1, '2025-11-28 14:52:53.407', '2025-11-28 14:52:53.407', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 34 z 28 listopada 2025', N'GH-34-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\28112025145146', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 35 z 5 grudnia 2025', N'Życzymy miłej lektury', '2025-12-05 15:58:11.713', '2', 1, '2025-12-05 15:58:11.713', '2025-12-05 15:58:11.713', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 35 z 5 grudnia 2025', N'GH-35-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\05122025155713', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 36 z 23 grudnia 2025', N'Życzymy wesołych świąt i miłej lektury!', '2025-12-23 18:15:14.517', '2', 1, '2025-12-23 18:15:14.517', '2025-12-23 18:15:14.517', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 36 z 23 grudnia 2025', N'GH-36-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\23122025181404', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 37 z 31 grudnia 2025', N'Życzymy miłej lektury i szczęśliwego Nowego Roku!', '2025-12-31 14:48:12.800', '2', 1, '2025-12-31 14:48:12.800', '2025-12-31 14:48:12.800', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 37 z 31 grudnia 2025', N'GH-37-2025.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\31122025144629', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 1 z 9 stycznia 2026', N'GH-01-2026.pdf', '2026-01-09 16:44:23.497', '2', 1, '2026-01-09 16:44:23.497', '2026-01-09 16:44:23.497', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 1 z 9 stycznia 2026', N'GH-01-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\09012026164332', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 2 z 23 stycznia 2026', N'Życzymy miłej lektury', '2026-01-23 13:59:10.803', '2', 1, '2026-01-23 13:59:10.803', '2026-01-23 13:59:10.803', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 2 z 23 stycznia 2026', N'GH-02-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\23012026135817', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 3 z 30 stycznia 2026', N'Życzymy miłej lektury', '2026-01-30 14:51:06.350', '2', 1, '2026-01-30 14:51:06.350', '2026-01-30 14:51:06.350', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 3 z 30 stycznia 2026', N'GH-03-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\30012026145023', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 4 z 13 lutego 2026', N'GH-04-2026.pdf', '2026-02-13 15:27:43.247', '2', 1, '2026-02-13 15:27:43.247', '2026-02-13 15:27:43.247', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 4 z 13 lutego 2026', N'GH-04-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\13022026152624', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 5 z 20 lutego 2026', N'Życzymy miłej lektury', '2026-02-20 14:37:32.097', '2', 1, '2026-02-20 14:37:32.097', '2026-02-20 14:37:32.097', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 5 z 20 lutego 2026', N'GH-05-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\20022026143642', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 6 z 27 lutego 2026', N'GH-06-2026.pdf', '2026-02-27 14:54:49.290', '2', 1, '2026-02-27 14:54:49.290', '2026-02-27 14:54:49.290', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 6 z 27 lutego 2026', N'GH-06-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\27022026145348', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 7 z 6 marca 2026', N'Zapraszamy do lektury', '2026-03-06 18:20:46.713', '2', 1, '2026-03-06 18:20:46.713', '2026-03-06 18:20:46.713', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 7 z 6 marca 2026', N'GH-07-2026.pdf', N'000000\F76A18BBEDA948C2B951C47150A27778\06032026181958', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 8 z 16 marca 2026', N'GH-08-2026.pdf', '2026-03-16 15:57:13.463', '2', 1, '2026-03-16 15:57:13.463', '2026-03-16 15:57:13.463', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 8 z 16 marca 2026', N'GH-08-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\16032026155616', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 9 z 27 marca 2026', N'GH-09-2026.pdf', '2026-03-27 14:28:55.887', '2', 1, '2026-03-27 14:28:55.887', '2026-03-27 14:28:55.887', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 9 z 27 marca 2026', N'GH-09-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\27032026142817', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 10 z 3 kwietnia 2026', N'Życzymy Wesołych Świąt i miłej lektury', '2026-04-03 14:29:58.453', '2', 1, '2026-04-03 14:29:58.453', '2026-04-03 14:29:58.453', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 10 z 3 kwietnia 2026', N'GH-10-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\03042026142822', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 11 z 10 kwietnia 2026', N'GH-11-2026.pdf', '2026-04-10 15:22:54.477', '2', 1, '2026-04-10 15:22:54.477', '2026-04-10 15:22:54.477', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 11 z 10 kwietnia 2026', N'GH-11-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\10042026152145', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 12 z 17 kwietnia 2026', N'Życzymy miłej lektury', '2026-04-17 14:44:13.660', '2', 1, '2026-04-17 14:44:13.660', '2026-04-17 14:44:13.660', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 12 z 17 kwietnia 2026', N'GH-12-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\17042026144300', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 13 z 30 kwietnia 2026', N'GH-13-2026.pdf', '2026-04-30 15:20:57.767', '2', 1, '2026-04-30 15:20:57.767', '2026-04-30 15:20:57.767', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 13 z 30 kwietnia 2026', N'GH-13-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\30042026152002', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 14 z 8 maja 2026', N'GH-14-2026.pdf', '2026-05-08 17:24:22.590', '2', 1, '2026-05-08 17:24:22.590', '2026-05-08 17:24:22.590', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 14 z 8 maja 2026', N'GH-14-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\08052026172339', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 15 z 22 maja 2026', N'GH-15-2026.pdf', '2026-05-22 16:06:37.793', '2', 1, '2026-05-22 16:06:37.793', '2026-05-22 16:06:37.793', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 15 z 22 maja 2026', N'GH-15-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\22052026160604', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika - Węglokoks nr 1 maj 2026', N'Szanowni Czytelnicy
Od dzisiaj, oprócz tradycyjnego Głosu Hutnika Huty Częstochowa, raz w miesiącu będziemy publikować wydanie Głosu Hutnika Grupy Kapitałowej Węglokoks S. A. z informacjami na temat innych spółek hutniczych. Życzymy miłej lektury.
', '2026-06-02 15:41:09.453', '2', 1, '2026-06-02 15:41:09.453', '2026-06-02 15:41:09.453', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika - Węglokoks nr 1 maj 2026', N'GH-maj26-nr1.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\02062026153643', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 16 z 3 czerwca 2026', N'Życzymy miłej lektury', '2026-06-03 17:34:34.590', '2', 1, '2026-06-03 17:34:34.590', '2026-06-03 17:34:34.590', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 16 z 3 czerwca 2026', N'GH-16-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\03062026173316', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 17 z 19 czerwca 2026', N'GH-17-2026.pdf', '2026-06-19 15:18:30.313', '2', 1, '2026-06-19 15:18:30.313', '2026-06-19 15:18:30.313', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 17 z 19 czerwca 2026', N'GH-17-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\19062026151801', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika - Węglokoks nr 2 czeerwiec 2026', N'Czerwcowe wydanie Głosu Hutnika Grupy Węglokoks', '2026-06-26 14:19:35.813', '2', 1, '2026-06-26 14:19:35.813', '2026-06-26 14:19:35.813', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika - Węglokoks nr 2 czeerwiec 2026', N'GłosHutnika-czerwiec2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\26062026141626', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 18 z 3 lipca 2026', N'Życzymy miłej lektury', '2026-07-03 15:19:09.707', '2', 1, '2026-07-03 15:19:09.707', '2026-07-03 15:19:09.707', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 18 z 3 lipca 2026', N'GH-18-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\03072026151830', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 19 z 10 lipca 2026', N'GH-19-2026.pdf', '2026-07-10 15:18:32.613', '2', 1, '2026-07-10 15:18:32.613', '2026-07-10 15:18:32.613', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 19 z 10 lipca 2026', N'GH-19-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\10072026151734', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 20 z 24 lipca 2026', N'GH-20-2026.pdf', '2026-07-24 14:35:39.770', '2', 1, '2026-07-24 14:35:39.770', '2026-07-24 14:35:39.770', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 20 z 24 lipca 2026', N'GH-20-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\24072026143449', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika - Weglokoks lipiec 2026', N'Głos Hutnika grupy Węglokoks S. A.Życzymy miłej lektury.', '2026-07-28 10:43:37.703', '2', 1, '2026-07-28 10:43:37.703', '2026-07-28 10:43:37.703', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika - Weglokoks lipiec 2026', N'GłosHutnika-lipiec2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\28072026104144', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 21 z 7 sierpnia 2026', N'Życzymy miłej lektury', '2026-08-07 11:48:00.190', '2', 1, '2026-08-07 11:48:00.190', '2026-08-07 11:48:00.190', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 21 z 7 sierpnia 2026', N'GH-21-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\07082026114614', '2');
GO
INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaEmail, wiaTemat, wiaTresc, wiaData, wiaStatus, wiaPubliczna, DataUtworzenie, DataModyfikacja, Usuniety) VALUES (900, NULL, N'Głos Hutnika nr 22 z 14 sierpnia 2026', N'Życzymy miłej lektury', '2026-08-14 12:20:07.860', '2', 1, '2026-08-14 12:20:07.860', '2026-08-14 12:20:07.860', 0);
INSERT INTO dbo.WiadomoscDokument (wdoWiadomoscID, wdoTyp, wdoNazwa, wdoPlik, wdoKatalog, wdoStatus) VALUES (SCOPE_IDENTITY(), N'pdf', N'Głos Hutnika nr 22 z 14 sierpnia 2026', N'GH-22-2026.pdf', N'000000\9D9E98F4EEEEE91180D25404A604E63A\14082026121727', '2');
GO
