## 2026-11-13 - Głos Hutnika: zamykanie okna wysyłki, komunikat po zapisie, stopka i nagłówek

- **Zgłoszenie:** na /wiadomosc okno wysyłki (GlosWysylkaOkno) nie zamykało się po zapisie/wysyłce, pojawiał się tylko toast. Po sukcesie strona nie odświeżała danych. Stopka nie była widoczna, a nazwa strony stała tuż przy logo.
- **Diagnoza:** GlosWysylkaOkno zamykało okno przez DialogService.Close (zwykły dialog), a okno boczne (OpenSideAsync) wymaga CloseSide. Stopka: site.css ustawia .footer jako position:absolute względem widoku.
- **Zmiana:** GlosWysylkaOkno.razor: wszystkie zamknięcia przez CloseSide; sukces zamyka okno z wynikiem GlosWynik (nowy plik GlosWynik.cs); błąd zostaje w oknie z toastem. GlosZarzadzanie.razor: po sukcesie przeładowanie strony z komunikatem w adresie (glosSeverity/glosTytul/glosTresc), komunikat pokazywany po renderowaniu i usuwany z adresu (wwwroot/js/glos-hutnika.js). WiadomosciPanel.razor: usunięty parametr OnZmieniono. glos-hutnika.css: stopka static, body w kolumnie, odstęp gap 1rem między logo a tytułem.
- **Decyzja techniczna:** po sukcesie strona przeładowuje się (lista Razor po stronie serwera odświeża się tylko tak); komunikat przechodzi przez adres, bo przeładowanie kasuje toasty.
- **Status wdrożenia:** zmiany niezacommitowane, bez publikacji.
- **Weryfikacja:** dotnet build indigoERP.Blazor: 0 błędów. Strona wymaga SSO, więc nie sprawdzono w przeglądarce.

## 2026-11-13 - Akceptacja tokenów podpisanych GatewayKey (401 z atacamaDMS)

- **Zgłoszenie:** atacamaDMS (licznik powiadomień `WiadomoscUzytkownik/SelectCountByUzytkownik`) dostaje 401 „Wymagane jest uwierzytelnienie” przy tokenie użytkownika z authorizationSRV.
- **Diagnoza:** authorizationSRV podpisuje tokeny użytkowników kluczem `Jwt:Key` mapowanym ze zmiennej systemowej `GatewayKey` (wspólny sekret). indigoERP.Api sprawdzał podpis tylko kluczem `Jwt:Key` (`indigoERP_JwtKey`), więc każdy token użytkownika był odrzucany. atacamaDMS.Api akceptuje oba klucze, dlatego działa.
- **Zmiana:** `indigoERP.Api\Services\JwtSigningKeys.cs` (nowy) buduje listę kluczy walidacji: zawsze `Jwt:Key`, dodatkowo `GatewayKey`, gdy jest ustawiony. `indigoERP.Api\Program.cs`: odczyt opcjonalnej zmiennej `GatewayKey` (proces, potem Machine) i `IssuerSigningKeys = JwtSigningKeys.Create(...)` zamiast pojedynczego `IssuerSigningKey`.
- **Decyzja techniczna:** `GatewayKey` opcjonalny (jak w Development w atacamaDMS.Api), żeby brak zmiennej nie zatrzymywał startu. `indigoERP_JwtKey` nie zmieniony, bo indigoERP może nim podpisywać własne tokeny.
- **Baza danych / Zmiana:** brak zmian w bazie.
- **Testy:** nowe `indigoERP.Api.Tests\JwtSigningKeysTests.cs` (5 testów: token z GatewayKey przechodzi; token z Jwt:Key nadal przechodzi; token z GatewayKey bez skonfigurowanego GatewayKey odrzucany; obcy klucz odrzucany; brak Jwt:Key zgłasza błąd konfiguracji). Cały `indigoERP.Api.Tests`: 73/73 zaliczone.
- **Status wdrożenia:** opublikowane na CERES (`V:\inetpub\wwwroot\indigoERP.Api`) jako `3.26.1113.11427` (`Version` w `appsettings.json` repo i serwera). Tryb offline od 10:46 (planowane przywrócenie 11:46), robocopy `/MIR` kod 3 (OK), kopia `appsettings.json.prod-backup`, zmieniony tylko `Version`. Zmienna `GatewayKey` na CERES istnieje (skrót sha256 `58599D3ED4CB`, zgodny z authorizationSRV). Pula `indigoERP.Api` do restartu przez użytkownika (`GatewayKey` czytany przy starcie procesu).
- **Weryfikacja po wdrożeniu:** `WiadomoscUzytkownik/SelectCountByUzytkownik/210/{usrID}` z tokenem użytkownika zwraca 200 zamiast 401.

## 2026-11-13 - Wygląd podstron: O systemie, Głos Hutnika, Wiadomości

- **Zgłoszenie:** na `/about` tytuły dolnych kart mają być w linii z ikonami (strona się mieści); na `/gloshutnika` i innych podstronach logo jest mniejsze niż na stronie głównej, brakuje odstępu między logo a tytułem i stopki.
- **Zmiana:** `OProgramie/OProgramieStrona.razor` (karta `op-feature`: ikona i tytuł w jednym wierszu); `wwwroot/css/glos-hutnika.css` (logo 44 px jak na stronie głównej, `margin-right: 1rem`); `Pages/GlosHutnika.cshtml` i `Pages/WiadomoscPubliczna.cshtml` (statyczna stopka z `.footer` z `site.css`, bez panelu serwera).
- **Decyzja:** podstrony `.cshtml` nie używają layoutu, więc stopkę dodano jako statyczny HTML, a nie komponent `Footer` (ten wymaga DI i trybu renderowania).
- **Status wdrożenia:** niewdrożone.
- **Weryfikacja:** build `indigoERP.Blazor` 0 błędów (15 ostrzeżeń istniejących, bez zmian w zmienionych plikach). Nie sprawdzono w przeglądarce (logowanie SSO).

## 2026-11-11 - Pakiety NuGet do wersji 11 (net11.0)

- **Zmiana:** `Microsoft.EntityFrameworkCore*` (Api, Domain, Services, Worker, WebUI) 10.0.15 → 11.0.0; `Microsoft.Extensions.Http.Resilience` i `Microsoft.Extensions.ServiceDiscovery` (ServiceDefaults) 10.11.0 → 11.0.0; `Microsoft.NET.Test.Sdk` 18.11.0 → 18.11.1 i `xunit.v3` 4.0.2 → 4.0.3 (testy Api, Services, Worker, Blazor); `bunit` 2.14.0 → 2.15.2 (Blazor.Tests).
- **Dodatkowo:** `NuGet.Protocol` 7.9.0 → 8.0.0 (indigoERP.Api; zmiana wersji głównej, zaakceptowana przy aktualizacji pakietów).
- **Weryfikacja:** build `indigoERP.sln` 0 błędów (997 ostrzeżeń, wcześniej 881: dodatkowe NU1608); testy Api, Services, Blazor, Worker — wszystkie zaliczone.
- **Otwarte:** NU1608 — `Biwen.EFCore.UseRowNumberForPaging` 2.4.0 wymaga EF Core 10 (zainstalowano 11); sprawdzić stronicowanie na żywej bazie przed publikacją. Zmiany w `csproj` są niezacommitowane na `master` (w `developer` nadal net10 / stare wersje).

## 2026-11-11 - Migracja WebUI i Test poza głównym rozwiązaniem

- Projekty indigoERP.WebUI i indigoERP.Test przeniesiono z net7.0 na net11.0; zaktualizowano zależności WebUI, usunięto przestarzałą kompilację widoków Razor w czasie działania oraz zastąpiono usunięte rozszerzenie sprawdzania pustego tekstu standardową metodą string.
- Build WebUI bez błędów, 25 ostrzeżeń; Test bez błędów i ostrzeżeń. WebUI korzysta z EF Core 10.0.15 wspólnego z pozostałymi projektami; podczas restore użyto tymczasowej konfiguracji źródeł NuGet. Nie sprawdzono logowania, UI ani rzeczywistych uprawnień; bez zmian bazy i publikacji.
# Codex Changes

## 2026-11-10 — rozpoczęta migracja .NET 11

- **Zgłoszenie:** aktualizacja VisualStudioCore, bez publikacji.
- **Zmiany:** 12 projektów rozwiązania indigoERP.sln przełączono na net11.0 i zaktualizowano pakiety, w tym ASP.NET Core 11, Aspire 13.6.5, Radzen 13 i xUnit v3. Dodatek starego stronicowania SQL zastąpiono Biwen.EFCore.UseRowNumberForPaging 2.4.0; EF Core pozostawiono na 10.0.15 z powodu braku wsparcia EF 11 przez ten dodatek. Projekty spoza rozwiązania `indigoERP.Test` i `indigoERP.WebUI` nadal wymagają analizy. Zachowano zastane zmiany, w tym zmiany staged.
- **Baza i wdrożenie:** bez zmian bazy i bez publikacji. Nie zmieniano lokalnego pliku NuGet.config wskazującego nieistniejące źródło DevExpress 23.1; restore wykonano z tymczasową konfiguracją.
- **Weryfikacja:** build SDK 11.0.100 bez błędów z 881 ostrzeżeniami pierwszej kompilacji; 153/153 testów przeszło, w tym test generowania ROW_NUMBER bez połączenia z bazą. Kontrola zależności nie wykazała pakietów deprecated ani znanych podatności. Migracja jest nieukończona: ostrzeżenia, kompatybilność SQL na serwerze, rzeczywisty interfejs, Worker, Agent i uprawnienia wymagają sprawdzenia.
## 2026-11-10 - Trasy Przepisy wewnętrzne w indigoERP (PrzepisyWewnetrzne/...)

- **Zgłoszenie:** trasy `PolecenieSluzbowe/SelectPubliczne` i `PolecenieSluzbowe/Pdf` mają nazywać się `PrzepisyWewnetrzne/...`.
- **Zmiana:** `indigoERP.Services\...\PolecenieSluzboweService.cs` (ścieżki wołane z Blazora do indigoERP.Api); kontroler `indigoERP.Api\Controllers\PolecenieSluzboweController.cs` przemianowany na `PrzepisyWewnetrzneController.cs` (klasa, plik i test `PrzepisyWewnetrzneControllerTests.cs`), dzięki czemu trasa `api/PrzepisyWewnetrzne/...` wynika z konwencji `api/[controller]/[action]`.
- **Decyzja:** zmiana nazwy klasy zamiast wpisanej na sztywno trasy, bo `AllControllersContractTests` wymusza konwencję tras.
- **Status wdrożenia:** niewdrożone.
- **Weryfikacja:** `indigoERP.Api.Tests` 67 z 67; build `indigoERP.Services` 0 błędów.

## 2026-11-10 - Przepisy wewnętrzne zamiast Polecenia służbowe (biblioteka 13)

- **Zgłoszenie:** w menu pozycja „Polecenia służbowe” ma nazywać się „Przepisy wewnętrzne”, a strona `/polecenie-sluzbowe` ma pobierać pliki z biblioteki 13 (Przepisy wewnętrzne) w atacamaDMS.
- **Zmiana:** `Shared\MainHeader.razor` (etykieta menu); `Pages\PolecenieSluzbowe\Index.razor` (tytuł, kolumny, kafelki, dane z przepisów); `indigoERP.Domain\ViewModels\_Wspolne\PrzepisWewnetrznyViewModel.cs` zamiast `PolecenieSluzboweViewModel` (usunięty); `indigoERP.Api\Services\AtacamaPolecenieClient.cs` i `Controllers\PolecenieSluzboweController.cs` (ścieżki `PrzepisyWewnetrzne/SelectPubliczne` i `Pdf/{pliID}` w atacamaDMS.Api); `indigoERP.Services\...\PolecenieSluzboweService.cs`; test `PolecenieSluzboweControllerTests.cs` przepisany na nowy model.
- **Decyzja:** adres `/polecenie-sluzbowe` i nazwy klas proxy zostają bez zmian. Kafelek na stronie głównej (`Pages\Home.razor`) ma tytuł „Przepisy wewnętrzne”.
- **Decyzja:** lista obejmuje aktywne pliki PDF biblioteki 13 (bez usuniętych i wyrejestrowanych) i nie sprawdza uprawnień do biblioteki, tak jak wcześniej dla poleceń służbowych.
- **Status wdrożenia:** niewdrożone. Najpierw publikacja atacamaDMS.Api (endpoint `PrzepisyWewnetrzne`), potem indigoERP.
- **Weryfikacja:** `indigoERP.Api.Tests` 67 z 67 zaliczonych. Build `indigoERP.Blazor` 0 błędów, bez ostrzeżeń w zmienionych plikach.

## 2026-11-10 - Plec/Select czyta z widoku enum.Plec (500 w formularzach pracownika)

- **Zgłoszenie:** formularz dodawania pracownika (epicaHR, pp.hcz.com.pl/epicaHR/kadry/pracownik/insert) zwraca 500 przy ładowaniu listy plci.
- **Diagnoza:** PlecController.Select czyta encję Plec (dic.Plec), która dziedziczy po AuditableEntity, a tabela dic.Plec nie ma kolumn DataUtworzenie, UtworzenieID, DataModyfikacja, ModyfikacjaID, Usuniety - zapytanie SQL kończy się błędem. Ten sam błąd na CERES (/api/Plec/Select?pleStatus=2 → 500).
- **Zmiana:** Controllers\Slownik\PlecController.cs - Select czyta z PlecEnumerator (widok enum.Plec: pleKOD, pleOpis, pleStatus), zwraca IEnumerable<PlecEnumerator>. Kształt JSON zgodny z epicaHR.Domain.Entities.Wspolne.Plec.
- **Decyzja:** zmiana kontrolera zamiast DDL (dodanie kolumn audytowych do dic.Plec) - bez zmiany bazy; alternatywa DDL pozostaje otwarta.
- **Weryfikacja:** nowe testy PlecControllerTests (2) - filtrowanie po statusie, pusta lista; indigoERP.Api.Tests 67 z 67 zaliczonych; build indigoERP.Api 0 błędów.
- **Status wdrożenia:** opublikowane na CERES 2026-11-10 z czystej kopii master (3.26.1110.11418, tryb offline do 08:33, robocopy z zachowaniem ppsettings.json i web.config). Plec/Select na https://test.hcz.com.pl:9900 zwraca 200 z danymi. Na DEIMOS niewdrożone (czeka na zgodę). Uwaga: ppsettings.json na CERES ma wersję 3.26.1110.11422 (nie nadpisana) - do wyjaśnienia.

## 2026-11-06 - Polecenia służbowe: strona w indigoERP przez atacamaDMS.Api (proxy w indigoERP.Api)

- **Zgłoszenie:** strona „Polecenia służbowe” dla wszystkich użytkowników indigoERP: lista lub siatka (kafelki po 6 w wierszu, tytuł skrócony, pełny tytuł w tooltipie), PDF otwierany w oknie, wyszukiwanie po tytule, numerze i treści; kafelek na stronie głównej i pozycja w menu po Głosie Hutnika.
- **Zmiana:** `indigoERP.Api`: `Controllers\PolecenieSluzboweController.cs` (`SelectPubliczne`, `Pdf`), `Services\AtacamaServiceTokenProvider.cs` (token serwisowy indigoERP -> atacamaDMS z sekretem `ServiceClient_indigoERP`), `Services\AtacamaPolecenieClient.cs` (wywołania `atacamaDMS.Api`), `appsettings.json` sekcja `AtacamaDms:BaseUrl` = `https://app.hcz.com.pl:9210/api/`, rejestracja w `Program.cs`. `indigoERP.Services`: `IPolecenieSluzboweService` / `PolecenieSluzboweService`. `indigoERP.Domain`: `PolecenieSluzboweViewModel`. `indigoERP.Blazor`: `Pages\PolecenieSluzbowe\Index.razor` (+ `.css`), `PolecenieSluzbowePdfOkno.razor`, kafelek i styl w `Home.razor(.css)` (siatka skrótów: 8 równych wierszy), pozycja w `MainHeader.razor`.
- **Decyzja techniczna:** Blazor woła tylko swoje API; token serwisowy z rolą `HCZ-atacamaDMS-Integracja` (rola i uprawnienie indigoERP 900 -> atacamaDMS 210 dodane w bazie indigoERP, skrypt `_SQL Queries\20261106_atacama_integracja\01_rola_integracja.sql`) jest tylko po stronie API. PDF przekazywany jako data URI do iframe (iframe nie wysyła tokenu, więc nie może pobrać pliku sam).
- **Status wdrożenia:** zmiana w kodzie, niewdrożona. Do wdrożenia: `AtacamaDms:BaseUrl` w appsettings API na DEIMOS i CERES; sekret `ServiceClient_indigoERP` musi być ustawiony na serwerze.
- **Weryfikacja:** build Api i Blazor bez błędów; testy Api 64/64 (w tym `PolecenieSluzboweControllerTests`: lista, brak tokenu 503, błąd atacamaDMS 502, PDF, brak PDF 404); testy Blazor 29/29. Nie sprawdzono w przeglądarce ani na żywym atacamaDMS.
- **Luki:** miniatury to ikony PDF, nie podgląd strony. Gdy brak tokenu serwisowego, `Pdf` zwraca 404 zamiast 503 (klient zwraca `null` w obu przypadkach).

## 2026-11-06 - Głos Hutnika: import treści z pliku Word, przycisk „Nowa wiadomość”, szerokość wyboru PDF

- **Zgłoszenie:** wstawianie treści z plików Word do edytora nowej wiadomości (w tym obrazów osadzonych); przycisk „Nowy numer” na „Nowa wiadomość”; wybór PDF w jednej linii z większym polem.
- **Zmiana:** `Utils\WordHtmlConverter.cs` (konwersja .docx → HTML: akapity, nagłówki, pogrubienie, kursywa, podkreślenie, listy, tabele, obrazy jako data URI); okno `Pages\WiadomoscPubliczna\ImportWordOkno.razor` (Wczytaj/Anuluj, limit 20 MB, tylko .docx); przycisk „Importuj z Worda” nad edytorem w `GlosWysylkaOkno.razor`; `GlosZarzadzanie.razor` (tekst przycisku); pakiet `DocumentFormat.OpenXml` 3.5.1 w `indigoERP.Blazor.csproj`.
- **Decyzja techniczna:** konwersja po stronie serwera (OpenXML), bez skryptów z CDN. Przycisk nad edytorem, bo edytor Radzen nie daje prostego dodania własnego przycisku do paska narzędzi. Okno importu otwierane jako zwykłe okno (nie slide), żeby nie zagnieżdżać dwóch okien bocznych. Hiperłącza zostają jako tekst; obrazy EMF/WMF pomijane (przeglądarki ich nie pokazują).
- **Status wdrożenia:** zmiana w kodzie, niewdrożona.
- **Weryfikacja:** testy Blazor 29/29 (w tym nowe `WordHtmlConverterTests`: pogrubienie i znaki specjalne, nagłówek, tabela, obraz osadzony). Plik „Komunikat spotkanie kobiet GK webinar 2026 11 12 Błońska Marta_Spółki GK.docx” przekonwertowany na HTML (`…GK.html` w tym samym katalogu, UTF-8 BOM). Nie sprawdzono w przeglądarce.

## 2026-11-04 - Worker wiadomości: wybór kanału (Graph/SMTP) po domenie nadawcy

- **Zgłoszenie:** epicaHR ma wysyłać e-mail z `epicaHR@huta-czestochowa.pl` przez Microsoft Graph, a dotychczasowa wysyłka z `@hutaczestochowa.com.pl` zostaje na SMTP.
- **Diagnoza:** `RoutingWiadomoscMailSender` kierował całą wysyłkę na SMTP (komentarz: powrót do Graph odłożony), a odczyt odpowiedzi o niedostarczeniu przez Graph próbował czytać skrzynki wszystkich nadawców spoza `@hutaczestochowa.com.pl`, także zewnętrznych (np. `@interbalt.pl`, `@tkmrp.pl`), które nie istnieją w naszym M365.
- **Zmiana:** `indigoERP.Worker/Services/IWiadomoscMailSender.cs` — `RoutingWiadomoscMailSender(smtp, graph, MailRoutingOptions)`: nadawca z domeny `Mail:Graph:Domena` (domyślnie `huta-czestochowa.pl`) wysyła przez Graph, wszystko inne (nasza domena SMTP, domeny zewnętrzne, brak nadawcy, subdomeny) przez SMTP; opcjonalna lista `Mail:Graph:Nadawcy` zawęża Graph do wymienionych nadawców. Gdy Graph nie jest skonfigurowany (`MicrosoftGraph_*`), wysyłka wraca na SMTP. Nowy interfejs `IKanalNadawcy.UzywaGraph`; `WiadomoscKolejkaService` czyta zwrotki (NDR) tylko dla nadawców wysyłających przez Graph. `Program.cs` rejestruje routing; `appsettings.json` ma sekcję `Mail:Graph` z `Wlaczony: false`.
- **Decyzja techniczna:** wyłącznik `Mail:Graph:Wlaczony` jest domyślnie `false`, więc samo wdrożenie Workera niczego nie zmienia. Po włączeniu przez Graph pójdą WSZYSCY nadawcy z domeny `huta-czestochowa.pl`, także ci, którzy dziś wysyłają przez SMTP (w ostatnich 180 dniach m.in. `andrzej.latocha@`, `itsupport@`, `indigoERP@huta-czestochowa.pl`) — aplikacja Azure musi mieć `Mail.Send` dla tych skrzynek albo trzeba zawęzić `Mail:Graph:Nadawcy`. Zalecane ograniczenie aplikacji Azure politykami Exchange do wybranych skrzynek.
- **Status wdrożenia:** tylko kod, niewdrożone.
- **Weryfikacja:** `indigoERP.Worker.Tests` 46/46 (w tym nowe scenariusze routingu: wyłączony, nadawca M365, domena SMTP, domeny zewnętrzne, subdomena, brak nadawcy, brak konfiguracji Graph, lista nadawców, `IsConfigured`). Przebudowa bez błędów.
## 2026-11-04 - Słownik dic.TypPlik: lookup po aplikacji i grupie

- **Zgłoszenie:** tabela `dic.TypPlik` ma nową strukturę z polem `tpkTypPlikGrupaID` (FK do `dic.TypPlikGrupa`); potrzebny lookup filtrowany po aplikacji i grupie.
- **Zmiana:** `Domain\Entities\Slownik\TypPlik.cs` — pole `int? tpkTypPlikGrupaID`. `Api\Controllers\Slownik\TypPlikController.cs` — nowa akcja `SelectLookupByGrupa/{tpkAplikacjaID}/{tpkTypPlikGrupaID}?tpkStatus=` zwraca wiersze z `tpkAplikacjaID = par lub 0` i `tpkTypPlikGrupaID = par lub 0`; istniejąca `SelectLookupByAplikacja` bez zmian.
- **Zmiana (2):** `Domain\Enumerators\Slownik\TypPlikEnumerator.cs` — pola `tpkTypPlikGrupaID` i `tpkDomyslny`, zgodnie z widokiem `enum.TypPlik` (`tpkNazwa AS tpkOpis`). Akcja czyta `enum.TypPlik` przez `TypPlikEnumerator`.
- **Zmiana (3):** pole `bool tpkObowiazkowy` (`BIT NOT NULL`) w `TypPlik` (encja) i w `TypPlikEnumerator` (widok `enum.TypPlik`); tabela i widok już je zawierają.
- **Decyzja techniczna:** wiersze z `NULL` w `tpkTypPlikGrupaID` nie są zwracane (ani dla grupy, ani dla aplikacji, `NULL` ≠ 0). Parametr `tpkStatus` zgodnie z konwencją (`xxxStatus` ostatni, `*` = wszystkie), jak w pozostałych akcjach.
- **Status wdrożenia:** zmiana w kodzie, niewdrożona.
- **Weryfikacja:** build Api.Tests bez błędów; testy Api 59/59 (w tym nowe `TypPlikControllerTests`: dopasowanie aplikacji/grupy z wartością 0 i filtr statusu).

## 2026-11-04 - Logowanie do plikow dziennych i obsluga bledow

- **Zgloszenie:** ujednolicenie logow aplikacji do plikow `{aplikacja}-{rrrr-MM-dd}.log` w `C:\inetpub\logs\{aplikacja}.Api` / `C:\inetpub\logs\{aplikacja}` / `...\{aplikacja}.Worker` (wzorzec namibPMS).
- **Zmiana:** projekty: Api, Blazor, Worker (Logging:FilePath zamiast katalogu logs w aplikacji). `DailyFileLogger` (Domain\Logging lub lokalnie w projekcie bez Domain), `AddDailyFile` i zapis bledu startu (`UnhandledException`) w `Program.cs`, `Logging:FilePath` w `appsettings.json` (produkcja) i `Logs` w `appsettings.Development.json`, `Microsoft.EntityFrameworkCore` i `System.Net.Http.HttpClient` na `Warning`; w API `UseApiErrorHandling` (ProblemDetails z `traceId`), w Blazorze `ApiErrorHandler` dla kazdego `HttpClient` (`ConfigureHttpClientDefaults`); `Logs/` w `.gitignore`.
- **Weryfikacja:** przebudowa projektow (0 bledow); uruchomienie i sprawdzenie pliku logu NIE wykonane (luka - do sprawdzenia po wdrozeniu).

## 2026-11-03 - Głos Hutnika: wysyłka SMTP, DocumentPath, okno wysyłki i nagłówek

- **Zgłoszenie:** wysyłka numeru GH nie mogła ruszyć (załącznik 1098 niewidoczny dla workera, Graph zamiast serwera pocztowego), okno wysyłki nie zamykało się po zapisie i pokazywało pustą stronę podczas ładowania, nagłówek Blazora odbiegał od wzorca.
- **Diagnoza:** worker i API czytały załączniki z różnych katalogów (`c:\_documents` na JAPET i `C:\indigoERP_documents` na DEIMOS); okno zamykało się przez `forceLoad` po 4 s; nagłówek używał `logo_white.svg` w strukturze bez `container-fluid`.
- **Zmiana:** Worker wysyła przez SMTP (`RoutingWiadomoscMailSender` używa tylko `SmtpWiadomoscMailSender`), katalog załączników z `DocumentPath` (`Program.cs`); API czyta `DocumentPath` jako `FileStorage:Path`; okno wysyłki odświeża listy bez przeładowania, pokazuje wskaźnik ładowania, przycisk potwierdzenia „WYŚLIJ”; nagłówek: `container-fluid w-100`, `header-logo` 38 px, `header-actions`, przycisk użytkownika `btn-outline-light`, pozycja menu „O systemie”.
- **Decyzja techniczna:** `DocumentPath` bez prefiksu, bo to zmienna wspólna dla API, Blazora i Workera (ustawiona na DEIMOS i JAPET jako `c:\_documents`). Logo zostaje `logo_white.svg`: `logo.svg` ma tekst `#575756` i byłby niewidoczny na tle nagłówka. Odczyt NDR przez Graph pozostał bez zmian; wysyłka SMTP nie daje odpowiedzi NDR na skrzynki M365.
- **Status wdrożenia:** API `3.26.1103.11411` wdrożone na DEIMOS (21:50 okno offline); Blazor `3.26.1103.11411` wdrożony na DEIMOS (22:37 okno offline); Worker wdrożony na JAPET (Z:, kopia `_backup_20261103_pre_docpath`), usługa uruchomiona. Pliki z `C:\indigoERP_documents` na DEIMOS przeniesione do `c:\_documents` (WENUS, 354 pliki w `CORE0900`). Publikacje wykonano z drzewa roboczego `developer`, nie z `master`; ten commit to ich zapis w repo.
- **Weryfikacja:** build Release bez błędów; testy Api 57/57, Blazor 24/24, Services 10/10, Worker 34/34; worker po restarcie wysłał 100 adresatów wiadomości 1098 (20:57:55 wg logu); hash DLL na serwerach zgodny z publikacją; API i Blazor odpowiadają (Blazor 401 bez Kerberos, API 200). Wyglądu nagłówka i okna wysyłki nie sprawdzono w przeglądarce.

## 2026-11-03 - Strona O programie z nowościami z indigoERP

- **Zgłoszenie:** aplikacja ma mieć stronę O programie z informacjami o programie, grafiką i tabelą nowości (od najnowszej); tytuł nowości otwiera okno z pełnym opisem w HTML.
- **Zmiana:** katalog `OProgramie` (komponenty `OProgramieStrona` i `OProgramieNowosci`, klient `OProgramieNowosciClient`, `OProgramieHtml`), strona `About.razor` (`/about`) oparta na wspólnym komponencie; rejestracja `AddOProgramie` w `Program.cs`; pozycja menu O programie tam, gdzie istnieje menu. Nowości: `GET Nowosc/SelectByAplikacja/{AppID}?nowStatus=2` z API indigoERP (`appSettings:IndigoApiUrl`, produkcja: zmienna `indigoERP_Api_Url`).
- **Decyzja techniczna:** komponent niezależny od biblioteki UI (HTML/CSS, bez Radzen i DevExpress), opis nowości czyszczony z skryptów, atrybutów zdarzeń i adresów javascript:.
- **Status wdrożenia:** zmiana w kodzie, niewdrożona.
- **Weryfikacja:** przebudowa projektu Blazor (0 błędów), testy bUnit klasy OProgramieTests: 11/11 zaliczonych; podgląd w przeglądarce (namibPMS).

## 2026-11-03 - Ujednolicenie nagłówka i stopki (logo 38 px, ikony z images/footer)

- **Zgłoszenie:** logo w nagłówku ma mieć wysokość 38 px, a ikony stopki (telefon, e-mail) mają pochodzić z katalogu `images/footer`.
- **Zmiana:** wysokość logo w nagłówku ustawiona na 38 px (atrybut `height` lub CSS `.header-logo`); ikony stopki `ico_phone.png` i `ico_email.png` z `wwwroot/images/footer` (dodane tam, gdzie brakowało).
- **Status wdrożenia:** zmiana w kodzie, niewdrożona.
- **Weryfikacja:** przebudowa projektu Blazor (0 błędów).

## 2026-11-03 - Dane wrażliwe przeniesione z appsettings.json do appsettings.Development.json

- **Zgłoszenie:** dane wrażliwe (klucze, hasła, connection stringi z loginem SQL) nie mogą być w `appsettings.json`; lokalne wartości mają być w `appsettings.Development.json` ignorowanym przez git.
- **Zmiana:** wartości przeniesione do `appsettings.Development.json` (istniejące wartości w tym pliku zachowane), w `appsettings.json` wyzerowane (connection stringi: usunięty login i hasło, `Trusted_Connection=True`); dodany wzorzec `appsettings.Development.json.example` (bez wartości tajnych); `**/appsettings.Development.json` w `.gitignore`; plik Development wycofany z indeksu git (`git rm --cached`, zostaje na dysku). Klucze: indigoERP.Api: Logging:LogLevel:Default; indigoERP.Api: Logging:LogLevel:Microsoft.AspNetCore; indigoERP.Api: FileStorage:Path; indigoERP.Api: MicrosoftGraph:TenantId; indigoERP.Api: MicrosoftGraph:ClientId; indigoERP.Api: MicrosoftGraph:ClientSecret; indigoERP.Api: connectionStrings:indigoERPConnection; indigoERP.Api: Jwt:Key; indigoERP.Api: InternalSso:Key; indigoERP.Api: appSettings:MonitoringSqlPassword; indigoERP.Api: appSettings:ApiKey; indigoERP.Blazor: InternalSso:Key; indigoERP.WebUI: appSettings:ApiKey.
- **Decyzja techniczna:** produkcyjne wartości mają pochodzić ze zmiennych systemowych serwera; klucze bez nadpisania w kodzie wymagają dopisania odczytu zmiennej przed publikacją. Historia git nadal zawiera dawne wartości – zalecana rotacja kluczy i haseł.
- **Status wdrożenia:** zmiana w kodzie, niewdrożona; przed publikacją ustawić zmienne systemowe.
- **Weryfikacja:** poprawność JSON wszystkich plików appsettings; ponowny skan nie znajduje wartości wrażliwych w `appsettings.json`.

## 2026-11-01 - indigoERP.Agent: podpis PDF z wyborem miejsca (szkielet)

- **Zgłoszenie:** nowa aplikacja w autostarcie na koncie użytkownika do podpisywania PDF kwalifikowanym podpisem z wizualnym wzorem (np. PIT-11 dla pracownika). Miejsce podpisu wybierane w oknie podglądu, ustawialne w konfiguracji.
- **Zmiana:** nowy projekt `indigoERP.Agent` (WinForms, net10.0-windows), dodany do `indigoERP.sln`. `Signing\PdfSignatureService.cs` (podpis PKCS#7 certyfikatem po numerze seryjnym, grafika wzoru, DevExpress.Pdf), `Forms\PdfPlacementForm.cs` (podgląd PDF, kliknięcie ustawia lewy dolny róg), `Signing\SignatureSettings.cs` (sekcja `Signing` w `appsettings.json`: `ShowPlacementWindow`, domyślna strona i prostokąt).
- **Wywołanie:** `indigoERP.Agent.exe <wejście.pdf> <wyjście.pdf> <numer seryjny> <obraz podpisu>`. Bez okna, gdy `ShowPlacementWindow` = false.
- **Decyzja techniczna:** logika podpisu skopiowana z `atacamaDMS.Agent` (ta sama biblioteka DevExpress i ten sam sposób wyboru certyfikatu). Podpis XML PIT-11 nie wchodzi tutaj, zostaje w `epicaHR.Agent`.
- **Hasło do PDF:** pole w oknie podglądu (maskowane). Puste = PDF bez zabezpieczenia, podpis elektroniczny bez szyfrowania. Podane hasło otwiera plik `PdfDocumentSigner(stream, hasło)`. Hasło nie jest zapisywane w pliku ani w logu. Gdy `ShowPlacementWindow` = false, hasło nie jest pytane (podpis bez hasła). Ustalenie: zapis wyjścia z zachowaniem szyfrowania nie jest jeszcze sprawdzone na prawdziwym PDF.
- **Otwarte:** integracja z bazą epica (`usp_SelectPodatekPITPodpisz` / `usp_UpdatePodatekPITPodpisz`, `pod_plik_pit`) jeszcze nie zrobiona.
- **Status wdrożenia:** nie wdrożone, brak publikacji.
- **Weryfikacja:** `dotnet build indigoERP.Agent` — 0 błędów, 0 ostrzeżeń. Nie uruchamiano okna ani podpisu na prawdziwym PDF i certyfikacie. Nie sprawdzono, czy `PdfDocumentPosition.PageNumber` jest numerowane od 1 i czy `Point` jest w przestrzeni PDF (lewy dolny róg). Brak testów jednostkowych `SignatureSettings` (do dodania z projektem testowym).
## 2026-10-31 - Głos Hutnika: wybór wzorca w oknie „Nowy numer” (wymagany)

- **Zgłoszenie:** wiadomość z formularza ma być opakowana w wybrany szablon z listy dla aplikacji 900, bez domyślnej wartości.
- **Zmiana (API):** WiadomoscPracownikController.cs — `InsertRobocza` i `InsertWysylka` zwracają `400` bez `wiaWzorzecID`, a zapisują wybrany wzorzec. Stała `WzorzecGlosu = 9` zostaje tylko jako wartość przy `WyslijRobocza` dla starych szkiców.
- **Zmiana (Blazor):** GlosWysylkaOkno.razor — combo wzorców (`WiadomoscWzorzec/SelectLookup?msgAplikacjaID=900&msgStatus=2`), puste na starcie, bez wyboru nie da się zapisać szkicu. IWiadomoscPracownikService/WiadomoscPracownikService.cs — `SelectWzorce`, `wiaWzorzecID` w formularzu. Modele WiadomoscRoboczaModel/WiadomoscWysylkaZakresModel — pole `wiaWzorzecID`.
- **Testy:** Api 57/57, Blazor 13/13, Services 10/10.
- **Do ustalenia:** domyślny szablon 9 („Huta Częstochowa - powiadomienie”) vs 10 („Głos Hutnika”) — fallback w `WyslijRobocza` nadal 9.
- **Status wdrożenia:** kod niezacommitowany, API i Blazor nie wdrożone.

## 2026-10-31 - API: wiadomości z formularza Głosu Hutnika dostają wzorzec 9

- **Zgłoszenie:** wiadomość 1095 doszła bez opakowania w szablon (`wiaWzorzecID` był pusty), a numery z wcześniejszych wysyłek szły z wzorcem.
- **Zmiana (API):** WiadomoscPracownikController.cs — stała `WzorzecGlosu = 9`; ustawiana przy `InsertRobocza`, `InsertWysylka`, a w `WyslijRobocza` uzupełniana dla szkiców bez wzorca.
- **Zmiana (testy):** WiadomoscWysylkaTests.cs — asercja wzorca w teście zapisu szkicu. Testy Api 55/55.
- **Status wdrożenia:** kod niezacommitowany, API nie wdrożone. Wiadomość 1095 już wysłana bez wzorca, nie wysyłano ponownie.
- **Do sprawdzenia przed wdrożeniem:** czy wzorzec 9 jest aktywny (`msgStatus = '2'`) w `dbo.WiadomoscWzorzec`.

## 2026-10-30 - Worker: wysyłka 1095 (załącznik na WENUS, nadawca tymczasowo itsupport)

- **Zgłoszenie:** wiadomość 1095 („Głos Hutnika 29/2026”) nie wychodziła: worker nie czytał załącznika, a potem Graph odrzucał nadawcę `indigoERP@`, bo skrzynka nie istniała w M365.
- **Diagnoza:** (1) załącznik leżał na udziale `\\WENUS\Documents$`, na który wskazuje dowiązanie `c:\_documents` na JAPET; konto `JAPET\svc-indigoWorker` (lokalne) nie miało dostępu na WENUS. Naprawione przez konto na WENUS z tym samym hasłem. (2) Graph: `ErrorInvalidUser` dla `indigoERP@` (skrzynka nieutworzona).
- **Zmiana (Worker):** SciezkaDiagnostyka.cs — przy braku pliku komunikat zawiera konto procesu, stan katalogów po drodze i wynik odczytu (wdrożone 20:39). Testy Worker 20/20.
- **Zmiana (dane, tymczasowo):** 1095 — `wiaEmail` = `itsupport@huta-czestochowa.pl`; 936 adresatów z `ErrorInvalidUser` przywrócone do statusu `2`. Dla 299 pozostałych adresatów nic nie zmieniano.
- **Decyzja:** wysyłka 1095 kończy się z `itsupport@`. Kod (NadawcaGlosu) zostaje `indigoERP@huta-czestochowa.pl`, skrzynka została utworzona w M365; przed kolejnymi wysyłkami trzeba potwierdzić uprawnienie Mail.Send dla aplikacji workera.
- **Status wdrożenia:** worker uruchomiony na JAPET 21:03; log bez błędów od startu.

## 2026-10-30 - Worker: wiadomości z formularza bez wzorca nie były wysyłane (1095)

- **Zgłoszenie:** wiadomość 1095 „Głos Hutnika 29/2026” (utworzona w oknie „Nowy numer”, 1 załącznik) od 18:59 stoi w statusie `2`, bez wysyłki.
- **Diagnoza:** `WiadomoscKolejkaService.ClaimNextAsync` pobierał tylko wiadomości z `wiaWzorzecID IS NOT NULL`. API (`WiadomoscPracownikController` InsertRobocza/WyslijRobocza) nie ustawia wzorca, więc wiadomości z formularza nigdy nie były podejmowane. Wiadomości 1093/1094 wysłały się dzięki ręcznemu wznowieniu z wzorcem.
- **Zmiana (Worker):** WiadomoscKolejkaService.cs — claim obejmuje też wiadomości bez wzorca; bez wzorca treść idzie z formularza (`WiadomoscTrescFactory.Prepare(wiadomosc)`), z wzorcem jak dotąd. WiadomoscTrescFactory.cs — nowa metoda bez wzorca.
- **Zmiana (testy):** indigoERP.Worker.Tests/WiadomoscTrescFactoryTests.cs — 2 testy dla treści bez wzorca. Testy Worker 18/18, build Worker 0 błędów.
- **Doraźnie:** _SQL Queries/indigoERP20261030_wznow_1095.sql — ustawia wzorzec 1095 (wymaga ID wzorca w `@wzorzecID`). Nie uruchomiony; uruchamia użytkownik.
- **Status wdrożenia:** kod niezacommitowany, worker na Z: nie wdrożony (wymaga zatrzymania usługi indigoERP.Worker i podmiany plików).

## 2026-10-30 - Strona główna: wysokość kafelków i ikony hutaForms/Głos

- **Zgłoszenie:** ostatni kafelek (OneDrive) był ucięty na ~70% wysokości, a ikony hutaForms i Głosu były mniejsze niż w pozostałych kafelkach.
- **Zmiana (Blazor):** Pages/Home.razor.css — wiersze `repeat(6, minmax(0, 1fr)) minmax(0, 0.5fr)` (bez minimum wysokości, panel nie przewija się), ikony FontAwesome hutaForms i Głosu `2.6rem` z `line-height: 1`.
- **Diagnoza (miniatura/pobieranie 404):** nie błąd kodu. Lokalnie nie ma katalogu `indigoERP.Api\wwwroot\_documents` (ani `C:\indigoERP_documents`), więc `MiniaturaPubliczna` i `OpenPubliczny` zwracają 404 zgodnie z założeniem. Do sprawdzenia na serwerze z plikami.
- **Status wdrożenia:** niezacommitowane.

## 2026-10-30 - Strona główna: kafelek Głos Hutnika; wysyłka: potwierdzenie w oknie, komunikat, odświeżenie

- **Zgłoszenie:** na stronie głównej w lewym panelu (pod hutaForms) ma być Głos Hutnika, kafelki mają się zmieścić na wysokość ekranu. Na /wiadomosc: usunięty opis „Brak aktualnych ogłoszeń”, toast „Wysłano” w jednej linii, wskaźnik podczas wysyłki, po wysłaniu zamknięcie slide, komunikat i odświeżenie. „Anuluj” nie zamykał okna slide.
- **Zmiana (Blazor):** Pages/Home.razor — kafelek Głos Hutnika (`/gloshutnika`). Pages/Home.razor.css — 6 wierszy kafelków `minmax(72px, 1fr)`, ikona 52 px, padding 8 px. Pages/WiadomoscPubliczna.cshtml — usunięty komunikat „Brak aktualnych ogłoszeń”. Pages/WiadomoscPubliczna/GlosWysylkaOkno.razor — potwierdzenie wysyłki w oknie slide (zamiast Radzen Confirm), `RadzenProgressBar` i `IsBusy` podczas wysyłki, najpierw `Close(true)`, potem toast. Pages/WiadomoscPubliczna/GlosZarzadzanie.razor — odświeżenie strony po 4 s od zamknięcia slide. wwwroot/css/glos-hutnika.css — toast `width: min(760px, 92vw)`, `white-space: nowrap`.
- **Decyzja techniczna:** Radzen `DialogService.Close` zamyka ostatni dialog na liście. Drugie okno (Confirm) mogło zostać na liście i „Anuluj”/zamknięcie po wysłaniu trafiały w nie zamiast w slide. Potwierdzenie jest więc w tym samym oknie. Hipoteza nie została potwierdzona w przeglądarce.
- **Weryfikacja:** build Blazor 0 błędów; testy Blazor 13/13. Przeglądarka nie otworzyła localhost:8900 (`navigate` odrzucone), więc wyglądu i zachowania slide nie sprawdzono.
- **Status wdrożenia:** niezacommitowane.

## 2026-10-30 - Wiadomości (/wiadomosc): panel z rokiem, stronicowanie, status 1, cutoff 1.10.2026

- **Zgłoszenie:** na /wiadomosc (zarządzanie) combo roku przed „+ NOWY NUMER”, lista z paginacją w całości na ekranie, najnowsze pierwsze, status „1” z ikoną „oczekujący na wysłanie”, ukrycie rekordów sprzed 1.10.2026. W oknie wysyłki grid o 2 wiersze więcej.
- **Zmiana (Blazor):** Pages/WiadomoscPubliczna/WiadomosciPanel.razor (nowy) — combo roku (domyślnie bieżący), `GlosZarzadzanie` obok, lista; filtr `DataOd = 2026-10-01`. ListaWiadomosci.razor — parametr `Lista`, PageSize 10, `max-height: calc(100vh - 16rem)`, status „1” → 2.png. Pages/WiadomoscPubliczna.cshtml — zamiast osobnych komponentów panel. Pages/WiadomoscPubliczna/GlosWysylkaOkno.razor — PageSize 21 → 23.
- **Weryfikacja:** build Blazor 0 błędów; testy Blazor 13/13. Wyglądu nie sprawdzono w przeglądarce.
- **Status wdrożenia:** niezacommitowane.

## 2026-10-30 - Głos Hutnika: menu, strona /gloshutnika z wyborem roku i lista wiadomości

- **Zgłoszenie:** w menu zaraz po „Główna” ma być „Głos Hutnika” prowadzące do /gloshutnika z miniaturami numerów (siatka 10 kolumn, combo roku, najnowsze pierwsze, klik pobiera PDF). Na /wiadomosc: nagłówek „Ogłoszenia” ma być „Wiadomości” i lista wysłanych wiadomości ze statusem dla Gazety/Administratora.
- **Zmiana (Blazor):** Pages/GlosHutnika.cshtml(.cs) — nowa strona, `GlosHutnikaModel`, miniatury i pobieranie przez handlery `WiadomoscPubliczna` (Miniatura, Download). wwwroot/css/glos-hutnika.css — siatka `.glos-gh-siatka` (10 / 5 / 2 kolumny). Shared/MainHeader.razor — pozycja menu „Głos Hutnika” (target _blank jak „Wiadomości”). Pages/WiadomoscPubliczna.cshtml — nagłówek „Wiadomości”, komponent `ListaWiadomosci` (tylko dla ról). Pages/WiadomoscPubliczna/ListaWiadomosci.razor — RadzenDataGrid z WiadomoscPracownik/SelectPodsumowanie, ikony statusu `images/wiadomosc/status/{status}.png`.
- **Zmiana (Api/Domain):** WiadomoscPodsumowanieDto — pola wiaTemat, wiaData, wiaStatus (wypełniane w WiadomoscPracownikController.SelectPodsumowanie).
- **Weryfikacja:** build Blazor 0 błędów; testy Blazor 13/13 (stub z SelectPodsumowanie). Build Api: kompilacja bez błędów CS, ale kopia DLL zablokowana przez działające VS i indigoERP.Api.exe (MSB3027/MSB3021). Wyglądu w przeglądarce nie sprawdzono (strona wymaga logowania domenowego dla części funkcji).
- **Status wdrożenia:** niezacommitowane; do decyzji użytkownika.

## 2026-10-30 - Głos Hutnika: okno nowego numeru i wysyłki jako okno modalne (wersja 3.26.1030.11403)

- **Zgłoszenie:** okno boczne „Nowy numer” / „Wyślij” na /gloshutnika było wyższe niż widok: tytuł, temat i edytor wypadały poza ekran, a przyciski nie były dostępne (zapis roboczego zablokowany, „Anuluj” bez reakcji).
- **Zmiana (Blazor):** Pages/GlosHutnika/GlosZarzadzanie.razor - OpenAsync z DialogOptions (wyśrodkowane, szerokość min(1000px, 95vw), wysokość min(88vh, 900px)), zamiast OpenSideAsync. wwwroot/css/glos-hutnika.css - .glos-okno z przewijaniem treści.
- **Wersja:** 3.26.1030.11403 (Api i Blazor w appsettings). Na serwerze publikowany tylko Blazor.
- **Weryfikacja:** build Blazor 0 błędów; testy Blazor 13/13. Wyglądu nie sprawdzono w przeglądarce (strona wymaga logowania domenowego).
- **Status wdrożenia:** commit na developer, merge do master; publikacja Blazora do sprawdzenia po cyklu app_offline.
## 2026-10-30 - Wiadomość: podsumowanie adresatów (Wiadomosc/SelectPodsumowanie)

- **Zgłoszenie:** epicaHR musi pokazać liczby wysłanych i błędów na wiadomość w liście (jeden wiersz na wiadomość).
- **Zmiana (API):** `WiadomoscController.SelectPodsumowanie(wiaAplikacjaID)` (AllowAnonymous, jak `SelectHistoria`) zlicza `dbo.WiadomoscPracownik` po `wprWiadomoscID` dla aplikacji: `Razem`, `Wyslane` (status 4), `Bledy` (status 5), `Oczekujace` (status 2).
- **Zmiana (Domain):** `WiadomoscPodsumowanieDto`.
- **Weryfikacja:** build `indigoERP.Api` 0 błędów. Zapytanie nie jest pokryte testem na bazie – luka.
- **Status wdrożenia:** kod gotowy, niezacommitowany, niewdrożony na JAPET (wymagane przed wdrożeniem epicaHR).

## 2026-10-29 - Adres e-mail służbowy: indigoERP -> epicaHR (PracownikKontakt), token service-to-service

- **Zgłoszenie:** konto użytkownika nieobecne w Microsoft 365 ma czyścić adres e-mail (rekord użytkownika zostaje, usuwa go synchronizacja z epicaHR). Zmiana adresu w oknie różnic ma iść także do epicaHR: e-mail służbowy (typ kontaktu 9) w PracownikKontakt, z `Usuniety = true` zamiast kasowania. Pozostałe kontakty bez zmian.
- **Zmiana (indigoERP.Api):** `Services/UzytkownikEmailGraphRoznice.cs` - aktywny użytkownik z adresem, którego nie ma w M365, dostaje akcję `Usun` (wynik `Brak`). `Controllers/Administrator/UzytkownikController.cs` - przed zapisem w indigoERP wywołanie epicaHR (`UstawEmailSluzbowyAsync`); błąd epicaHR pomija zmianę w indigoERP i raportuje ją w komunikatach. `Services/EpicaHrServiceTokenProvider.cs` - token service-to-service (`Service/GetToken`, clientApp `indigoERP`, target `epicaHR`), cache do wygaśnięcia. `Services/EpicaHrPracownikEmailClient.cs` - `PUT api/PracownikEmail/UstawEmailSluzbowy/{pracownikID}`.
- **Zmiana (epicaHR.Api):** `Controllers/Kadry/PracownikEmailController.cs` - `UstawEmailSluzbowy`: pusty adres czyści (Usuniety = true) przypisania typu 9; adres aktualizuje istniejący kontakt w indigoERP albo tworzy nowy (`Kontakt/Insert`) i przypisanie. Autoryzacja: rola `HCZ-epicaHR-Kadry` (token service-to-service). `Domain/Entities/Kadry/PracownikKontakt.cs` - pole `Usuniety`.
- **Zmiana (authorizationSRV):** `appsettings.json` (dev) - klient `indigoERP` w `ServiceClients`.
- **Baza danych:**
  - `_SQL Queries/indigoERP_AplikacjaUslugaUprawnienie_indigoERP_epicaHR.sql` - nadanie indigoERP -> epicaHR, rola `HCZ-epicaHR-Kadry` (idempotentne).
  - `_SQL Queries/epicaHR_PracownikKontakt_utrIdPracownikKontakt.sql` - trigger `INSTEAD OF DELETE` zamieniający DELETE w `Usuniety = 1` (na bazie epicaHR).
- **Decyzja techniczna:** wybrano token service-to-service (droga A), bo wywołanie idzie w imieniu systemu, a nie zalogowanego użytkownika. Klucz klienta to zmienna systemowa `ServiceClient_indigoERP` (ta sama wartość na authorizationSRV i na indigoERP.Api). Zapis do epicaHR jest przed zapisem w indigoERP, żeby błąd nie rozjechał obu baz.
- **Kolejność wdrożenia (produkcja):**
  1. Ustawić zmienną systemową `ServiceClient_indigoERP` (Machine) na JAPET (authorizationSRV) i na DEIMOS (indigoERP.Api) - ta sama wartość.
  2. Dopiero potem dopisać `"indigoERP": ""` do `ServiceClients` w produkcyjnym `appsettings.json` authorizationSRV i opublikować go. Dopisanie klucza bez zmiennej blokuje start bramki (patrz wpis atacamaDMS 2026-09-07).
  3. Uruchomić skrypt nadania (indigoERP) i trigger (epicaHR).
  4. Publikacja epicaHR.Api (endpoint `PracownikEmail`) i indigoERP.Api.
- **Status wdrożenia:** tylko repozytorium. Nie zacommitowano, nie opublikowano, skrypty SQL nie uruchomione.
- **Weryfikacja:** build indigoERP.Api - 0 błędów; testy indigoERP.Api 52/52 (w tym 3 nowe w `UzytkownikEmailGraphTests`: czyszczenie przy braku konta w M365, wywołanie epicaHR, odrzucenie przez epicaHR; 4 nowe w `EpicaHrServiceTokenProviderTests`: nadanie klucza i cache tokenu, brak sekretu, odrzucenie przez bramkę, wymóg HTTPS). epicaHR.Api - build 0 błędów; `PracownikEmailControllerTests` 2/2 (czyszczenie tylko e-maila służbowego, dodanie nowego kontaktu).
- **Luki:**
  - Rola `HCZ-epicaHR-Kadry` to założenie wzięte z wdrożenia atacamaDMS; trzeba potwierdzić, że taka rola istnieje i jest właściwa dla zapisu adresów.
  - Zakładam, że `usrPracownikID` w indigoERP jest tym samym numerem co `pktPracownikID` w epicaHR (potwierdzone przez użytkownika).

## 2026-10-29 - Worker: odpowiedzi o niedostarczeniu oznaczają adresatów statusem 5

- **Zgłoszenie:** worker nie sprawdzał skrzynki nadawcy, więc adresy nieistniejące były oznaczane jako wysłane (status 4), choć nie doszły.
- **Zmiana (Worker):** `MicrosoftGraphMailService.GetUndeliveryNoticesAsync` czyta `users/{nadawca}/mailFolders/inbox/messages` od daty wysyłki i zwraca tylko powiadomienia (nadawca MAILER-DAEMON/postmaster albo temat „Undeliverable”, „Delivery Status Notification”, „Nie można dostarczyć”, „Nie dostarczono”).
- **Zmiana (Worker):** `NdrDopasowanie` (czyste funkcje) dopasowuje adresy adresatów w treści powiadomienia z granicami słów, bez rozróżniania wielkości liter.
- **Zmiana (Worker):** `WiadomoscKolejkaService.OznaczNiedostarczoneAsync` ustawia `wprStatus = '5'` i `wprBlad` dla dopasowanych adresatów wiadomości wysłanych w ostatnich 7 dniach. `WiadomoscWorker` wywołuje to co 15 minut.
- **Uprawnienia:** odczyt skrzynki wymaga uprawnienia aplikacji `Mail.Read` w Microsoft Entra (zgoda administratora) dla konta nadawcy.
- **Ryzyko:** treść powiadomienia może cytować oryginalną wiadomość z innymi adresami; wtedy dopasowanie oznaczy też adresatów, którzy dostali wiadomość. Do weryfikacji na pierwszym przebiegu.
- **Nadawca:** docelowo konto `epica.hr@huta-czestochowa.pl`; epicaHR musi zapisywać je w `wiaEmail` zamiast adresu zalogowanego użytkownika (zmiana w epicaHR, osobno).
- **Weryfikacja:** build `indigoERP.Worker` 0 błędów; `indigoERP.Worker.Tests` 16/16 (9 nowych dla `NdrDopasowanie`). Odczyt Graph i zapis do bazy nie są pokryte testami – luka.
- **Status wdrożenia:** kod gotowy, niezacommitowany, niewdrożony na JAPET.

## 2026-10-29 - Worker wysyła też do adresatów z WiadomoscPracownik

- **Zgłoszenie:** adresaci wpisani ręcznie w epicaHR („Własna lista adresów”) trafiają do `dbo.WiadomoscPracownik`, a worker czytał wyłącznie `dbo.WiadomoscUzytkownik`. Takie wiadomości nigdy nie były wysyłane (test wiadomości 1094 dostał odrzucenie „nie ma aktywnych odbiorców”).
- **Zmiana (Worker):** `WiadomoscKolejkaService` dołącza adresatów z `WiadomoscPracownik` (`wprStatus = '2'`, `wprEmail` niepusty), deduplikuje po adresie e-mail i po wysyłce ustawia `wprStatus` na `'4'`, a przy błędzie na `'5'` z treścią w `wprBlad` (obcięta do 4000 znaków).
- **Zmiana (Worker):** `WiadomoscWorkerContext` – `DbSet<WiadomoscPracownik>` i mapowanie `dbo.WiadomoscPracownik`.
- **Weryfikacja:** build `indigoERP.Worker` 0 błędów; `indigoERP.Worker.Tests` 7/7. Ścieżki EF (zapytania na bazie) nie są pokryte testami jednostkowymi – luka.
- **Status wdrożenia:** kod gotowy, niezacommitowany, niewdrożony na JAPET (`Z:\Services\indigoERP.Worker`).

## 2026-10-29 - Załączniki wiadomości w mechanizmie epicaHR (Plik + wdoPlikID), Worker czyta przez Plik

- **Zgłoszenie:** załączniki wysyłek (Głos Hutnika i pozostałe wiadomości) mają działać jak w epicaHR: plik w `{appFolder}\{usrFolder}\{yyyyMMddHHmmss}\`, rekord `dbo.Plik`, powiązanie z wiadomością. Wysyłkę wykonuje indigoERP.Worker.
- **Zmiana (Domain):** `Entities/Wspolne/WiadomoscDokument.cs` - nowe pole `wdoPlikID` (int?, powiązanie z `Plik`).
- **Zmiana (API):** `Services/WiadomoscZalacznikStorage.cs` - `ZapiszPlik(plik, appFolder, usrFolder, ct)`, katalog `{appFolder}\{usrFolder}\{czas}`, metoda `Segment` (bez separatorów). `Controllers/Wspolne/WiadomoscPracownikController.cs` - `ZapiszZalaczniki` zapisuje plik, dodaje rekord `Plik` (`plkAplikacjaID`, `plkFolder`, `plkPlik`, `plkLink`, `plkRozszerzenie=pdf`) i ustawia `wdoPlikID`. Folder: claim `appFolder` / `usrFolder`, a gdy brak - `CORE{AppID:D4}` i `usrID:D12` (wzorzec z CLAUDE.md).
- **Zmiana (Worker):** `Services/WiadomoscKolejkaService.cs` - `ResolveStorageAsync`: załącznik z `wdoPlikID` czyta ścieżkę z `Plik`; bez `wdoPlikID` (stare rekordy) zostaje `wdoKatalog`/`wdoPlik`. `Data/WiadomoscWorkerContext.cs` - `DbSet<Plik> Pliki`.
- **Baza danych:** `_SQL Queries/GlosHutnika_WiadomoscDokument_wdoPlikID.sql` - kolumna `wdoPlikID` i indeks `IX_WiadomoscDokument01`. Uruchamia użytkownik w SSMS na JAPET. Bez tego skryptu nowe zapisy załączników się nie udadzą.
- **Decyzja techniczna:** powiązanie trzymam w `WiadomoscDokument.wdoPlikID`, a nie w osobnej tabeli łączącej - ten sam wiersz już łączy wiadomość z plikiem, a mniej tabel do utrzymania. Stare załączniki nie są migrowane; Worker je obsługuje przez fallback.
- **Status wdrożenia:** tylko repozytorium. Nie zacommitowano, nie opublikowano, skrypt SQL nie uruchomiony.
- **Weryfikacja:** build Api, Worker, Blazor - 0 błędów. Testy: Api 45/45 (w tym nowy: plik w katalogu aplikacji/użytkownika, rekord Plik, wdoPlikID), Worker 7/7, Blazor 13/13.
- **Luka:** `ResolveStorageAsync` w Workerze nie ma testu jednostkowego (metoda prywatna, kontekst EF). Propozycja: wydzielić ją do osobnej klasy `IZalacznikResolver` z testem na InMemory.
- **Do sprawdzenia na serwerze:** claim `appFolder`/`usrFolder` w tokenie Bearer (jeśli go brak, działa fallback CORE0900); Worker musi mieć `FileStorage:Path` takie samo jak API oraz konto usługi z prawem odczytu do `C:\indigoERP_documents`.

## 2026-10-28 - Grupy i role indigoERP dla bibliotek atacamaDMS (CERES)

- **Zgłoszenie:** przygotować grupy i role w indigoERP dla bibliotek z transkrypcji obiegu dokumentów (Umowy, NDA, Zgody, Uchwały, Wnioski, Polecenia służbowe).
- **Baza danych / Zmiana:** `_SQL Queries/indigoERP_grupy_role_biblioteki_CERES_20261028.sql` (wersja JAPET: `indigoERP_grupy_role_biblioteki_JAPET_20261028.sql`, nie uruchomiona). W `dbo.Grupa`, `dbo.Rola` i `dbo.RolaCzlonkowie` dla `groAplikacjaID` / `srlAplikacjaID` = 210 (atacamaDMS): 18 grup `HCZ-atacamaDMS-<Poziom> biblioteki - <nazwa>` (poziomy Administrator, Obsługa, Przeglądanie), 18 ról o tych samych nazwach, 18 powiązań rola-grupa. Bez członków.
- **Decyzja techniczna:** skrypt jednorazowy (przy istniejącej nazwie zatrzymuje się bez zmian), nazwy z polskimi znakami przez `NCHAR()`, na złym serwerze przerywa przed zmianami.
- **Status wdrożenia:** CERES uruchomiony przez użytkownika z `@Zatwierdz = 1` (COMMIT), wynik kontrolny: 18 grup, 18 ról, 18 powiązań. Test z `@Zatwierdz = 0` nie był wykonany przed zapisem. JAPET do wykonania przez użytkownika.
- **Weryfikacja:** wynik kontrolny z SSMS (CERES, 2026-10-28 12:37). Członkostwa i `BibliotekaDostep` w atacamaDMS nie są częścią tego skryptu.

## 2026-10-24 - Głos Hutnika: kafelki numerów, okno wysyłki i miniatury PDF

- **Zgłoszenie:** strona `/gloshutnika` ma pokazywać numery jako kafelki z miniaturą pierwszej strony PDF i opisem, z pobieraniem PDF dla każdego. Przyciski zarządzania (nowy numer, wysyłka) tylko dla ról HCZ-indigoERP-Gazeta i HCZ-indigoERP-Administrator. Wysyłka ma być oknem bocznym, a nie osobną stroną. `/glos-historia` i `/glos-wysylka` zastępuje `/gloshutnika`.
- **Zmiana (API):** `WiadomoscDokumentController.MiniaturaPubliczna` (anonimowy, tylko załącznik PDF publicznej wiadomości) zwraca JPEG pierwszej strony. `Services/WiadomoscMiniaturaService.cs` renderuje stronę przez PDFtoImage 5.4.0 (PDFium, na serwerze IIS/Windows x64) i zapisuje obok pliku `*.miniatura.jpg` (cache; zapis przez plik tymczasowy). `WiadomoscPracownikController.InsertWysylka` ustawia `wiaPubliczna = true`, żeby nowy numer pojawiał się na stronie publicznej.
- **Zmiana (Blazor):** `Pages/GlosHutnika.cshtml` i `.cshtml.cs` - kafelki (siatka, miniatura, tytuł, opis `SkrocOpis`, „Czytaj”, „Pobierz PDF”); proxy `OnGetMiniaturaAsync`. Przyciski zarządzania to osadzony komponent `Pages/GlosHutnika/GlosZarzadzanie.razor` (renderowany tylko gdy `MozeZarzadzac`); okno boczne `Pages/GlosHutnika/GlosWysylkaOkno.razor` (formularz i wysyłka przeniesione z usuniętego `Wysylka.razor`). Usunięto `Pages/GlosWysylka/` (Wysylka.razor, Historia.razor) i linki w `Shared/MainHeader.razor`. `Program.cs` - `app.MapRazorPages()` (strona Razor nie była mapowana w repozytorium). Styl: `wwwroot/css/glos-hutnika.css`.
- **Decyzje:** strona zostaje anonimowa (jak wcześniej), a komponent zarządzania jest osadzany tylko dla zalogowanych z rolą; uprawnienia i tak egzekwuje API (`[Authorize]` na `InsertWysylka`). Miniatura generowana na żądanie, nie w bazie, więc bez zmian schematu. Ekran historii wysyłki (status per odbiorca) został usunięty z menu; dane nadal dostępne przez API `SelectWyslane`.
- **Testy:** dodane `WiadomoscMiniaturaServiceTests` (renderowanie mini-PDF do JPEG, odrzucenie pliku nie-PDF), `WiadomoscDokumentMiniaturaTests` (publiczna / prywatna wiadomość), `GlosHutnikaPageTests` (okno wysyłki, przycisk „Nowy numer”, `SkrocOpis`). Usunięto `GlosWysylkaPageTests`.
- **Status wdrożenia:** tylko repozytorium; nie zacommitowano i nie opublikowano. Do publikacji: pakiet PDFtoImage (natywny pdfium) trafi do katalogu Api; w `web.config` Api nadal brak `maxAllowedContentLength` (patrz wpis 2026-10-23).
- **Weryfikacja:** build Api i Blazor - 0 błędów. Testy: Api 40/40, Services 10/10, Blazor 12/12. Renderowanie PDF sprawdzone testem na prawdziwym pdfium. Wyglądu strony nie sprawdzono w przeglądarce (produkcja i lokalny SSO wymagają logowania domenowego).

## 2026-10-23 - Głos Hutnika: załączniki PDF w wysyłce

- **Zgłoszenie:** wysyłka „Głosu Hutnika” musi obsługiwać załączniki - plik PDF z gazetą.
- **Zmiana (API):** `WiadomoscPracownikController.InsertWysylka` przyjmuje `multipart/form-data`: pola wysyłki oraz pliki `pliki`. Przed zapisem sprawdzane są: liczba (najwyżej 5), rozszerzenie `.pdf`, nagłówek `%PDF` i rozmiar (30 MB na plik, 60 MB łącznie). Załączniki są zapisywane w katalogu `FileStorage:Path` w `GLOS\{usrID}\{yyyyMMddHHmmss}\` (nazwa z prefiksem GUID) i powiązane z wiadomością jako `WiadomoscDokument` (typ `pdf`, status `2`). Wiadomość, załączniki i odbiorcy powstają w jednej transakcji (dla bazy relacyjnej). Limity żądania: `RequestSizeLimit` i `RequestFormLimits` = 60 MB.
- **Zmiana (klasy):** `Api/Services/WiadomoscZalacznikStorage.cs` (walidacja i zapis, rejestracja w `Program.cs`); `Domain/Models/Wspolne/WiadomoscPlikWysylki.cs`.
- **Zmiana (Blazor):** `Pages/GlosWysylka/Wysylka.razor` - `InputFile` (wiele plików, `.pdf`), lista wybranych plików z usuwaniem, limity po stronie klienta; `IWiadomoscPracownikService.InsertWysylka(model, pliki)` wysyła multipart.
- **Uwaga:** konstruktor kontrolera bez DI (używany w testach) tworzy magazyn bez katalogu; próba zapisu pliku wtedy zgłasza wyjątek. W działaniu aplikacji magazyn dostaje katalog z konfiguracji przez DI.
- **Do sprawdzenia przed wdrożeniem:** (1) IIS na DEIMOS dla `indigoERP.Api` - `maxAllowedContentLength` musi być co najmniej 60 MB (plik `web.config` nie jest w repozytorium); (2) na produkcji `FileStorage:Path` nie jest ustawiony w `appsettings.json` - domyślnie `wwwroot\_documents` API, katalog musi być zapisywalny dla puli; (3) czy proces wysyłki e-mail (poza tym repozytorium) dołącza pliki z `WiadomoscDokument`.
- **Status wdrożenia:** tylko repozytorium; nie zacommitowano i nie opublikowano.
- **Weryfikacja:** build Api i Blazor - 0 błędów. Testy: Api 36/36 (w tym 3 nowe: plik nie-PDF, zły nagłówek, zapis PDF z powiązaniem), Services 10/10, Blazor 11/11. Połączenie z JAPET w tej sesji nie działało (SSPI), więc stanu bazy nie potwierdzono.
## 2026-10-23 - Głos Hutnika: ekrany wysyłki i historii (osobne menu, rola Gazeta/Administrator)

- **Zgłoszenie:** ekrany do wysyłki „Głosu Hutnika” do pracowników; osobne menu; odbiorcy tylko z wybranego zakresu; dostęp tylko dla ról HCZ-indigoERP-Gazeta i HCZ-indigoERP-Administrator.
- **Zmiana (API):** Controllers/Wspolne/WiadomoscPracownikController.cs - rola na poziomie klasy; nowe InsertWysylka (POST: wiadomość i odbiorcy w jednej transakcji; odbiorcy sprawdzani po stronie serwera względem zakresu; osoby spoza zakresu lub bez e-mail pomijane i zgłaszane w wyniku) oraz SelectWyslane?appID= (historia z mvw.vWiadomoscPracownik); SelectByWiadomoscZakres/{wzaID} korzysta z nowego źródła. Controllers/Wspolne/WiadomoscZakresController.cs - rola na poziomie klasy. Services/WiadomoscZakresPracownikSource.cs - odczyt odbiorców z widoku pp.vWiadomoscZakres* (wzorzec nazwy walidowany regexem).
- **Zmiana (Domain):** Models/Wspolne/WiadomoscWysylkaZakresModel.cs (żądanie i wynik wysyłki).
- **Zmiana (Services):** IWiadomoscPracownikService / WiadomoscPracownikService (zakresy, odbiorcy zakresu, wysyłka, historia); rejestracja w indigoERP.Blazor/Program.cs.
- **Zmiana (Blazor):** Pages/GlosWysylka/Wysylka.razor (/glos-wysylka: wybór zakresu, temat, treść HTML, lista odbiorców z zaznaczaniem, zaznacz/odznacz wszystko, potwierdzenie) i Pages/GlosWysylka/Historia.razor (/glos-historia); Shared/MainHeader.razor - osobna grupa „Głos – wysyłka / historia” widoczna tylko dla nowych ról.
- **Błąd naprawiony w trakcie:** przycisk „Zaznacz wszystko” odwoływał się do pracownicy.Count przy 
ull (wykryte testem renderowania).
- **Decyzje:** wysyłka tylko do zakresu (bez ręcznego dobierania); załączników na tym ekranie nie ma (stary przepływ /wiadomosc/ ich obsługuje); stare menu „Wiadomości” (rola HCZ-indigoERP-Wiadomość) bez zmian.
- **Wymagane przed użyciem na produkcji:** uruchomić _SQL Queries/indigoERP20261017170000.sql (kolumna wprBlad, widok mvw.vWiadomoscPracownik) oraz upewnić się, że w bazie istnieją widoki pp.vWiadomoscZakres* odpowiadające wartościom enum.WiadomoscZakres.wzaWidok (nie są tworzone tym skryptem).
- **Status wdrożenia:** tylko repozytorium; nie opublikowano. Commit i publikacja po akceptacji.
- **Weryfikacja:** dotnet build Api i Blazor - 0 błędów. Testy: Api 33/33 (w tym 5 nowych: zakres, pusta lista, nieznany zakres, brak e-maili, role), Services 10/10, Blazor 11/11 (w tym 2 nowe: strona wysyłki i historii). Połączenia z JAPET w tej sesji nie udało się sprawdzić (SSPI), więc stanu bazy nie potwierdzono.
## 2026-10-21 - Publikacja indigoERP.Blazor na DEIMOS (3.26.1021.11394)

- **Zmiana:** publikacja Release z cyklem pp_offline.htm przez Y:\inetpub\wwwroot\indigoERP (7 plików skopiowanych, kod wyjścia 3 = OK; bez ppsettings*.json, web.config, logs, _documents). Zawiera stopkę wyśrodkowaną z linkiem na nazwie firmy (jak w atacamaDMS) oraz wcześniejsze zmiany okna różnic e-mail i nagłówka. Wersja 3.26.1015.11388 + 6 dni = 11394 (repo i serwer, Blazor oraz Api).
- **Weryfikacja:** hash indigoERP.Blazor.dll zgodny z paczką; wwwroot\_documents na miejscu. Wyglądu stopki nie sprawdzono w przeglądarce (wymaga logowania domenowego).
- **Status wdrożenia:** wdrożone 2026-10-21.
## 2026-10-21 - Stopka zgodna z atacamaDMS: wyśrodkowanie i link na nazwie firmy

- **Zgłoszenie:** poprawić stopkę jak w atacamaDMS.
- **Diagnoza:** w atacamaDMS zawartość stopki jest wyśrodkowana (gridlayout-item z 	ext-align: center) i nazwa firmy jest linkiem do indigoERP; w indigoERP stopka była wyrównana do lewej, a link był tylko na logo.
- **Zmiana:** Shared/Footer.razor - „Huta Częstochowa” jako link do https://app.hcz.com.pl (logo bez zmian); Shared/Footer.razor.css - .footer { text-align: center; }.
  Wysokość stopki (60 px) bez zmian, bo strona główna ma calc(100vh - 116px) (56 px nagłówek + 60 px stopka).
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** dotnet build indigoERP.Blazor - 0 błędów; indigoERP.Blazor.Tests 9/9. Wyglądu nie sprawdzono w przeglądarce: strona na DEIMOS wymaga uwierzytelnienia domenowego, którego przeglądarka w sesji nie ma.
## 2026-10-18 - Osadzanie treści HTML wiadomości w szablonie workera

- **Zgłoszenie:** treść HTML rekordu `dbo.Wiadomosc` ma być osadzana w szablonie wskazanym przez `wiaWzorzecID` przed wysłaniem przez Worker.
- **Działanie:** Worker pobiera aktywny rekord `dbo.WiadomoscWzorzec` o `msgID = wiaWzorzecID`, zastępuje `{{message}}` treścią `wiaTresc`
  bez kodowania HTML oraz `{{subject}}` rozstrzygniętym tematem; Microsoft Graph wysyła wynik jako `contentType = HTML`.
- **Weryfikacja:** dodano regresyjny test pełnego szablonu HTML z osadzoną treścią HTML; nie wysyłano wiadomości ani nie zmieniano danych
  w bazie ani na koncie Microsoft 365.

## 2026-10-17 - Błąd dostarczenia odbiorcy-pracownika

- **Zgłoszenie:** dodać obsługę pola `WiadomoscPracownik.wprBlad` dla zwrotek poczty.
- **Zmiana:** encja, DTO, widok `mvw.vWiadomoscPracownik` i CRUD `WiadomoscPracownikController` przekazują opis błędu.
  Walidacja DTO ogranicza wartość do 4000 znaków.
- **Zmiany bazy danych:** dodano nieuruchomiony, idempotentny skrypt `_SQL Queries\indigoERP20261017170000.sql`, który dodaje
  `dbo.WiadomoscPracownik.wprBlad varchar(4000)` i aktualizuje widok `mvw.vWiadomoscPracownik`.
- **Weryfikacja:** test CRUD sprawdza zapis i aktualizację pola; test DTO sprawdza limit długości.

## 2026-10-17 - Powiadomienia użytkownika z widoku wiadomości

- **Zgłoszenie:** przełączyć nagłówek atacamaDMS na kontroler `WiadomoscUzytkownik`.
- **Zmiana:** `WiadomoscUzytkownikController` udostępnia autoryzowane Bearer endpointy
  `SelectCountByUzytkownik` i `SelectBrakOdczytByUzytkownik`, filtrowane po aplikacji, odbiorcy i `wuzOdczyt = 0`.
- **Weryfikacja:** dodano test kontraktu obu endpointów.

## 2026-10-17 - Odczyt odbiorców-użytkowników wiadomości z widoku

- **Zgłoszenie:** dodać `WiadomoscUzytkownik/Select` zwracający `mvw.vWiadomoscUzytkownik`.
- **Zmiana:** dodano `WiadomoscUzytkownikViewModel`, mapowanie `DbSet` oraz autoryzowaną metodę
  `GET api/WiadomoscUzytkownik/Select?wiaStatus=2`. Wynik zawiera wiadomość, jej nadawcę, odbiorcę i stan odczytania.
- **Zmiany bazy danych:** brak skryptu; widok został utworzony poza tym zadaniem.
- **Weryfikacja:** dodano test kontraktu endpointu.

## 2026-10-17 - Odczyt odbiorców-pracowników wiadomości z widoku

- **Zgłoszenie:** metoda `WiadomoscPracownik/Select` ma zwracać dane z `mvw.vWiadomoscPracownik`.
- **Zmiana:** dodano `WiadomoscPracownikViewModel`, mapowanie `DbSet` oraz odczyt aktywnych rekordów według `wprStatus`.
  Pozostałe metody kontrolera zachowują DTO i obsługują bezpośrednio `dbo.WiadomoscPracownik`.
- **Zmiany bazy danych:** brak skryptu; widok został utworzony poza tym zadaniem.
- **Weryfikacja:** dodano test kontraktu typu zwracanego przez `Select`.

## 2026-10-17 - Widok wiadomości z odbiorcą w tabeli Wiadomosc

- **Zgłoszenie:** dostosować API do zmienionego widoku `mvw.vWiadomosc`, który łączy `dbo.Wiadomosc` z `dbo.Uzytkownik`
  przez `wiaUzytkownikID`.
- **Zmiana:** `WiadomoscViewModel` odpowiada dokładnie kolumnom widoku; usunięto pola dawnego połączenia z
  `WiadomoscUzytkownik`. `SelectByUzytkownik`, `SelectCountByUzytkownik` i `Detail` filtrują po `wiaUzytkownikID`.
- **Decyzja:** usunięto endpointy filtrowania odczytanych i nieodczytanych wiadomości, ponieważ nowa struktura nie
  zawiera stanu odczytania. Licznik użytkownika obejmuje wszystkie jego wiadomości w danej aplikacji.
- **Zmiany bazy danych:** brak skryptu; widok został zmieniony poza tym zadaniem.
- **Weryfikacja:** `WiadomoscControllerTests` obejmuje kontrakt modelu widoku.

## 2026-10-14 - Nagłówek Główna z BaseUrl i link do indigoERP w stopce

- **Zgłoszenie:** w menu nagłówka Home → Główna z linkiem z appSettings:BaseUrl; w stopce logo i tekst Huta Częstochowa jako link do indigoERP.
- **Zmiana:** Header/NavMenu używają BaseUrl (appSettings:BaseUrl, awaryjnie ./); Footer używa IndigoUrl - w Development appSettings:IndigoApiUrl, na produkcji appSettings:IndigoUrl nadpisywane zmienną systemową indigoERP_Url (mapowanie w ExternalUrlConfiguration).
- **Weryfikacja:** przebudowa projektu Blazor, 0 błędów.

## 2026-10-07 - Animacja kropek (ThinkingDots) zamiast dotychczasowego loadera

- **Zgłoszenie:** ujednolicić animację ładowania we wszystkich aplikacjach (wzorzec: epicaHR).
- **Zmiana:** nowy komponent `ThinkingDots.razor` (+ `.razor.css`, parametr `Size`: `Small` / `Medium` / `Large` / `ExtraLarge`) obok `Load.razor` i `LoadSmall.razor`; `Load` pokazuje rozmiar `ExtraLarge`, `LoadSmall` rozmiar `Medium`. Strony używające `<Load />` / `<LoadSmall />` bez zmian. Dotychczasowe obrazki GIF / kółka zębate zostają w projekcie (nieużywane).
- **Status wdrożenia:** niewdrożone, niezacommitowane.
- **Weryfikacja:** patrz wpis w epicaHR (build 0 błędów); wygląd w przeglądarce nie był sprawdzany.

## 2026-10-15 - Nagłówek i stopka zgodne z atacamaDMS

- **Zgłoszenie:** nagłówek i stopka indigoERP nie spełniają standardu; wzorzec: atacamaDMS.
- **Różnice (atacamaDMS vs indigoERP):**
  - stopka: brak panelu środowiska (wersja API, serwer SQL, baza) - w atacamaDMS jest zaokrąglony panel `footer-serwer-panel`;
  - logo w nagłówku: atacamaDMS 40 px, indigoERP 44 px.
- **Zmiana:** `Shared/Footer.razor` - panel „Wersja API / Serwer / Baza” z `ISerwerService.GetInfo()` (endpoint `Serwer/GetInfo` już był);
  panel tylko gdy dane są dostępne (serwis zwraca `null` przy błędzie, więc strony bez zalogowania nie padają).
  `Shared/Footer.razor.css` - styl panelu jak w atacamaDMS, z `line-height: normal` (klasa `.footer` ma 60 px i rozpychała panel).
  `wwwroot/css/site.css` - `.app-header-logo` 40 px (także w `.navbar-brand > img`).
- **Nie zmieniano (różnice do decyzji):** przycisk użytkownika (indigoERP: `RadzenButton` w `LoginDisplay` + przełącznik motywu; atacama:
  przełącznik motywu i `LoginDisplay`), układ strony (atacama: `DxGridLayout` z wierszami nagłówka/stopki; indigoERP: zwykły układ).
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Testy:** `StaticPageTests` - test stopki dostał atrapę `ISerwerService`; dodane 2 testy: panel widoczny z danymi / ukryty bez danych.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; `indigoERP.Blazor.Tests` 9/9; wygląd nie sprawdzany w przeglądarce.

## 2026-10-15 - Okno różnic e-mail: szerokość 80% i krótszy tekst potwierdzenia

- **Zgłoszenie:** okno „Dane z M365” ma mieć 80% szerokości (żeby były widoczne wszystkie kolumny); z potwierdzenia usunąć zdanie
  „Zostanie wyświetlona lista różnic; zapis nastąpi dopiero po zatwierdzeniu.”.
- **Zmiana:** `Pages/Administrator/Uzytkownik/Index.razor` - `GetWideRelationSideDialogOptions(width)` (domyślnie `1120px`), okno
  różnic otwierane z `"80%"`; potwierdzenie: „Porównać adresy e-mail użytkowników z Microsoft 365?”.
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; `Blazor.Tests` bez regresji; wygląd nie sprawdzany w przeglądarce.

## 2026-10-15 - Publikacja indigoERP.Api i indigoERP.Blazor na DEIMOS (3.26.1015.11388)

- **Zmiana:** publikacja Release z cyklem `app_offline.htm` przez `Y:\inetpub\wwwroot` - najpierw Api (51 plików skopiowanych), potem
  Blazor (7 plików); kod wyjścia 3 = OK; bez `appsettings*.json`, `web.config`, `logs`, `_documents`. Zawiera okno różnic adresów
  e-mail (Dodaj/Popraw/Usuń) z zatwierdzaniem. Wersja `3.26.1014.11387` + 1 dzień = `3.26.1015.11388` (repo i serwer, Api oraz Blazor).
  Skrypty SQL 01 i 02 uruchomione wcześniej przez użytkownika (wynik potwierdzony: 0 nieaktywnych z adresem, reguła w procedurze).
- **Weryfikacja:** hash `indigoERP.Api.dll` i `indigoERP.Blazor.dll` zgodny z paczkami; `Home/SelectAplikacjaByUzytkownik` -> 200,
  `Uzytkownik/SelectDaneGraph` bez tokenu -> 401; `wwwroot\_documents` na miejscu. Połączenie z Graph (Entra ID, `User.Read.All`)
  potwierdzi dopiero pierwsze użycie przycisku „Dane z M365”.

## 2026-10-15 - Adresy e-mail z M365: różnice, akcje Dodaj/Popraw/Usuń, zatwierdzanie zaznaczonych

- **Zgłoszenie:** adresy w M365 przyjmujemy za prawidłowe; okno z różnicami ma pokazywać status (zielona = dodaj, niebieska = popraw,
  czerwona = usuń), checkboxy z „zaznacz / odznacz wszystko” oraz przyciski „Zatwierdź” i „Zamknij”. Konto ze statusem 1 nie ma
  mieć adresu e-mail. Przygotować skrypt.
- **Zmiana (kod):**
  - `Api/Services/UzytkownikEmailGraphRoznice.cs` - klasyfikator: `Usun` (status 1 i niepusty adres), `Dodaj` (pusty adres, jest
    w M365), `Popraw` (adres inny niż `mail` z M365; samego UPN nigdy nie używa do nadpisania); dopasowanie po loginie.
  - `UzytkownikController.SelectDaneGraph` - różnice dla wszystkich nieusuniętych użytkowników (+ nierozstrzygnięci bez adresu);
    nowa `UpdateDaneGraph` (`PUT api/Uzytkownik/UpdateDaneGraph`, rola Administrator): serwer ponownie wylicza różnice, stosuje tylko
    zgodne z żądaną akcją, adres zawsze z M365; zwraca liczniki dodano/poprawiono/usunięto/pominięto.
  - `Domain/Models/Administrator` - `UzytkownikEmailGraphModel` (+ `usrEmail`, `usrStatus`, `akcja`),
    `UzytkownikEmailGraphZatwierdzModel`, `...ZmianaModel`, `...WynikModel`.
  - `IUzytkownikService.UpdateDaneGraph`; `UzytkownikDaneGraph.razor` - kolorowe ikony, checkboxy, zaznacz/odznacz wszystko,
    „Zatwierdź (N)” z potwierdzeniem i podsumowaniem, „Zamknij”; `Index.razor` odświeża listę po zamknięciu okna.
- **Zmiana (SQL, do uruchomienia przez użytkownika na JAPET):** `01_Uzytkownik_usrEmail_status1_czyszczenie.sql` (kopia starych adresów
  w `tmp.UzytkownikEmailKopia_20261015`, 66 kont ze statusem 1 miało adres), `02_aspMergeUzytkownik_czyszczenie_email_przy_dezaktywacji.sql`
  (z żywej definicji; dodana linia `usrEmail = ''` przy dezaktywacji), `99_Uzytkownik_email_kontrola_stanu.sql` (tylko odczyt).
- **Decyzja techniczna:** zapis do epicaHR nie jest wykonywany - nie ustalono, które pole/tabela (patrz pytania); do czasu decyzji
  zmiany dotyczą wyłącznie `indigoERP.dbo.Uzytkownik`.
- **Testy:** `Api.Tests/UzytkownikEmailGraphTests.cs` (klasyfikator + kontroler, 8 testów), `Services.Tests/UzytkownikGraphServiceTests.cs`
  (3 testy), `Blazor.Tests/HomePageTests.cs` poprawiony (test oczekiwał usuniętego kafelka „Stary Intranet”) + test przycisków starych
  aplikacji. Wyniki: Api 17/17, Services 10/10, Blazor 7/7.
- **Luka testowa:** logika okna `UzytkownikDaneGraph.razor` (zaznaczanie, potwierdzenie, odświeżenie) nie ma testu komponentu - do
  dodania w `Blazor.Tests` (bUnit) z atrapą `IUzytkownikService`.
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji Api i Blazora oraz uruchomienia skryptów SQL 01 i 02.

## 2026-10-15 - Publikacja indigoERP.Blazor na DEIMOS (3.26.1014.11387, z aktualizacją pakietów)

- **Zmiana:** publikacja Release z cyklem `app_offline.htm` przez `Y:\inetpub\wwwroot\indigoERP` (135 plików skopiowanych z 960 -
  w tym pakiety NuGet zaktualizowane przez użytkownika, np. Aspire/HealthChecks/MessagePack w AppHost), kod wyjścia 3 = OK; bez
  `appsettings*.json`, `web.config`, `logs`, `_documents`. Zawiera też siatki administratora o jeden wiersz wyższe. Wersja
  `3.26.1014.11386` + 1 = `11387` (repo i serwer, Blazor oraz Api).
- **Weryfikacja:** hash `indigoERP.Blazor.dll` na serwerze zgodny z paczką, `wwwroot\_documents` na miejscu; w logu aplikacji
  po publikacji `Login/GetSso` wywoływane przez użytkowników. Uwaga: pakiety zaktualizowano tylko w kodzie - `indigoERP.Api` na
  DEIMOS nie był ponownie publikowany.

## 2026-10-15 - Administrator: siatki o jeden wiersz wyższe

- **Zgłoszenie:** na stronach w części administrator zwiększyć wysokość siatek o jeden wiersz.
- **Zmiana:** we wszystkich `Pages/Administrator/**/*.razor` `PageSize` zwiększony o 1: listy główne (Aplikacja, Grupa, Użytkownik,
  Serwer, Uprawnienia usługowe, Blokady logowania) 16 -> 17, okna relacji (grupy/role użytkownika, użytkownicy grupy/roli,
  dane z M365) 18 -> 19, Rola 6 -> 7.
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; wygląd nie sprawdzany w przeglądarce.

## 2026-10-14 - Publikacja indigoERP.Blazor na DEIMOS (3.26.1014.11386, przez dysk Y)

- **Zmiana:** publikacja Release z cyklem `app_offline.htm` przez `Y:\inetpub\wwwroot\indigoERP` (`Y:` = `\\deimos\c$`),
  7 plików skopiowanych, kod wyjścia 3 = OK; bez `appsettings*.json`, `web.config`, `logs`, `_documents`. Zawiera poprawkę combo
  aplikacji i filtra grup w oknie grup użytkownika. Wersja `3.26.1014.11385` + 1 = `11386` (repo i serwer, Blazor oraz Api).
- **Weryfikacja:** hash `indigoERP.Blazor.dll` na serwerze zgodny z paczką, `wwwroot\_documents` na miejscu; w logu aplikacji
  po publikacji prawdziwe żądania użytkowników: `Login/GetSso` 200 i `Home/SelectAplikacjaByUzytkownik` 200. Własny test strony
  zwrócił 401 (Negotiate), bo ta stacja nie ma teraz działającego uwierzytelnienia domenowego (SSPI/Kerberos) - nie wada aplikacji.

## 2026-10-14 - Grupy użytkownika: combo aplikacji tylko ze statusem 2, filtr aplikacji zawęża listę grup

- **Zgłoszenie:** w combo aplikacji (okno grup użytkownika) pojawiały się trzy rekordy bez opisu i podwójne wpisy; mają być
  tylko aplikacje ze statusem 2. Wybór aplikacji ma filtrować także combo „Wybierz grupę”.
- **Diagnoza:** `SelectLookup("*")` zwracało też aplikacje nieaktywne (status 1) i bez nazwy; zdublowane nazwy to aplikacje
  o tej samej nazwie i różnych `appID` (np. nieaktywne odpowiedniki).
- **Zmiana:** `Pages/Administrator/Uzytkownik/UzytkownikGrupa.razor` - `SelectLookup("2")`, pominięte puste nazwy; gdy nazwa w statusie 2
  nadal się powtarza, etykieta ma dopisane `(appID)`. Lista grup do wyboru jest materializowana przy każdej zmianie
  (`RefreshAvailableGroups`): tylko aktywne grupy (status 2), bez grup, w których użytkownik już jest, z filtrem `groAplikacjaID`.
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; działanie w przeglądarce nie sprawdzane (brak dostępu do JAPET
  w trakcie zmiany - błąd SSPI/Kerberos), więc nie potwierdzono na danych, ile aplikacji o statusie 2 ma wspólną nazwę.

## 2026-10-14 - Skrypt: użytkownicy do grup gobiMRP 2410 (Plan-Właściciel) i 2414 (Budżet-Właściciel)

- **Zgłoszenie:** dodać 12 wskazanych osób do grup 2410 i 2414 w bazie indigoERP.
- **Zmiana:** `_SQL Queries/GrupaCzlonkowie_gobiMRP_Wlasciciel_20261014.sql` - idempotentny `INSERT` do `dbo.GrupaCzlonkowie`
  (jak `GrupaController.InsertCzlonek`), w transakcji, z kontrolą przed i po. 11 osób dopasowanych jednoznacznie
  (m.in. „Mykhailo Malygin” = `mykhaylo.malygin`); „Marek Mednarski” = `marek.bednarski` (literówka, potwierdzone przez
  użytkownika) - razem 12 osób.
- **Status wdrożenia:** skrypt przygotowany, NIE uruchomiony (produkcję SQL uruchamia użytkownik).
- **Weryfikacja:** zapytanie tylko do odczytu na JAPET potwierdziło brak brakujących użytkowników i liczbę członkostw do dodania.

## 2026-10-14 - Stary cerber na JAPET: niezgodne wersje DotNetOpenAuth (w toku)

- **Zgłoszenie:** `https://intranet.hcz.com.pl/cerber/` - błąd `Could not load file or assembly 'DotNetOpenAuth.Core, Version=4.1.0.0'`
  (manifest nie zgadza się z referencją); „wstawić starszą wersję biblioteki”.
- **Diagnoza:** `Microsoft.AspNet.Membership.OpenAuth.dll` wymaga `DotNetOpenAuth.Core` 4.1.0.0, a w `cerber\bin` był komplet
  DotNetOpenAuth 4.3.0.0 (z 2013-11-25).
- **Zmiana (JAPET, `Z:\inetpub\wwwroot\cerber\bin`):** podmienione na 4.1.0.0 (`4.1.4.12333`, z `kolejowy\bin` i `kjkoszty\bin`):
  `DotNetOpenAuth.Core`, `.AspNet`, `.OAuth`, `.OAuth.Consumer`, `.OpenId`, `.OpenId.RelyingParty` oraz
  `Org.Mentalis.Security.Cryptography`. Poprzednie 4.3.0.0 w `cerber\_DotNetOpenAuth_4.3_20261014\`
  (plus `cerber\DotNetOpenAuth.Core.dll.4.3.0.0.20261014.bak`).
- **Status:** `/cerber/` nadal zwraca 500 (strona `Error/500.aspx` też 500, więc komunikatu nie widać; dziennik zdarzeń JAPET
  niedostępny zdalnie). Skan referencji w `bin` nie wykazał już niezgodności wersji DotNetOpenAuth; pozostałe niezgodne/brakujące
  to zależności opcjonalne (Telerik: AWSSDK, Azure; `System.Web.Optimization` -> WebGrease). Potrzebny tekst błędu aplikacji.
- **Cofnięcie:** skopiować pliki z `cerber\_DotNetOpenAuth_4.3_20261014\` z powrotem do `cerber\bin`.
- **Dalszy ciąg:** po odczycie błędu (`customErrors Off`, potem przywrócone `On`): `Compilation Error CS0433` - w `bin` są dwa
  komplety DevExpress (v24.1 i v24.2), a `cerber.dll` (2026-10-06) i strony `.aspx` odwołują się do v24.1.7.0, podczas gdy
  `Web.config` do v24.2.5.0. Próba przeniesienia plików v24.1 została cofnięta (`cerber.dll` ich wymaga). Decyzja użytkownika:
  stare strony sprawdzi sam, stary cerberTMS zostanie wkrótce usunięty - dalszych zmian w `cerber` nie wykonywano.
  Stan: `bin` ma 50 plików v24.1 i 50 plików v24.2 oraz DotNetOpenAuth 4.1.0.0; `Web.config` taki jak przed diagnozą (`customErrors On`).

## 2026-10-14 - Stary Intranet na JAPET: przekierowanie strony głównej intranet.hcz.com.pl + publikacja Gateway

- **Zgłoszenie:** tylko `https://intranet.hcz.com.pl/` ma przekierowywać na `https://app.hcz.com.pl/`; aplikacje w podkatalogach
  działają normalnie. Stare strony mogą być na JAPET albo WENUS.
- **Diagnoza:** DNS `intranet.hcz.com.pl` -> JAPET (10.0.40.101), gdzie „Default Web Site” (korzeń `C:\inetpub\wwwroot\intranet`)
  ma podaplikacje `/atacama`, `/cerber`, `/certificate`, `/kolejowy`, `/warehouse`, `/kendo`, `/bespin`, `/mojave` itd.;
  WENUS ma analogiczny układ (`/hydra`, `/magazyn`, `/bhp`, `/epica`, `/kjkoszty`).
- **Zmiana:** `Z:\inetpub\wwwroot\intranet\Default.aspx` (JAPET; kopia `Default.aspx.20261014.bak`) - na początku `Content2` blok
  `<% if (Request.Url.Host == "intranet.hcz.com.pl") Response.Redirect("https://app.hcz.com.pl/", true); %>`; tak samo na WENUS
  (poprzedni wpis). `Web.config` bez zmian (na WENUS przywrócony). Przekierowanie dotyczy tylko `/` i `/Default.aspx`.
- **Weryfikacja:** `https://intranet.hcz.com.pl/` i `/Default.aspx` -> 302 `https://app.hcz.com.pl/`; `/atacama/`, `/certificate/`,
  `/kolejowy/`, `/bespin/`, `/mojave/`, `/warehouse/Default.aspx` -> 200; WENUS `/hydra/` -> 200. Uwaga: `/cerber/` (JAPET) i
  `/kjkoszty/` (WENUS) zwracają 500, `/kendo/` przekierowuje na własną stronę błędu 500 - to nie są strony główne, których dotykałem;
  wymagają osobnej analizy.
- **Publikacja authorizationSRV.Gateway na DEIMOS:** `dll/exe/pdb/deps.json` wgrane z cyklem `app_offline.htm` (bez `appsettings.json`,
  `web.config`, `dotnet-tools.json`); trasy `authorizationSRV-route` i `indigoERP-route` zachowane; `https://app.hcz.com.pl/health` -> 401
  (jak wcześniej, przed autoryzacją), `/indigoERP/Api/...` -> 401 z indigoERP.Api.

## 2026-10-14 - Stary Intranet na WENUS: przekierowanie tylko dla hosta intranet.hcz.com.pl (korekta)

- **Zgłoszenie:** po dodaniu `httpRedirect` do `Web.config` linki do starych aplikacji wracały na `app.hcz.com.pl`;
  przekierowanie ma dotyczyć wyłącznie adresu `intranet.hcz.com.pl`.
- **Diagnoza:** katalog `W:\inetpub\wwwroot\intranet` to korzeń „Default Web Site” na WENUS, a `/hydra`, `/magazyn`, `/bhp`,
  `/epica`, `/kjkoszty` to jego podaplikacje - dziedziczyły `httpRedirect` z `Web.config`; `httpRedirect` nie ma warunku na host,
  a moduł URL Rewrite nie jest zainstalowany.
- **Zmiana:** `Web.config` przywrócony z `Web.config.20261014.bak` (bez `httpRedirect`); w `Default.aspx` (kopia
  `Default.aspx.20261014.bak`) na początku `Content2` dodany blok `<% if (Request.Url.Host == "intranet.hcz.com.pl") Response.Redirect("https://app.hcz.com.pl/", true); %>` -
  przekierowuje tylko stronę główną i tylko dla tego hosta.
- **Weryfikacja:** żądania do `10.0.45.10`: Host `intranet.hcz.com.pl` `/` -> 302 `https://app.hcz.com.pl/`;
  Host `wenus.hcz.com.pl` `/`, `/hydra/`, `/magazyn/`, `/bhp/Default.aspx` oraz Host `intranet.hcz.com.pl` `/hydra/` -> 200.
- **Uwaga:** DNS `intranet.hcz.com.pl` wskazuje na JAPET (10.0.40.101), więc ten warunek działa na WENUS tylko dla ruchu z tym
  nagłówkiem Host; aplikacje `/atacama`, `/cerber`, `/certificate` itd. leżą na JAPET i nie są objęte.
- **Cofnięcie:** przywrócić `Default.aspx.20261014.bak` jako `Default.aspx`.

## 2026-10-14 - Publikacja indigoERP.Api i indigoERP.Blazor na DEIMOS (3.26.1014.11385)

- **Zmiana:** publikacja Release z cyklem `app_offline.htm` - najpierw Api (`c:\inetpub\wwwroot\indigoERP.Api`, 8 plików),
  potem Blazor (`c:\inetpub\wwwroot\indigoERP`, 7 plików); robocopy bez `appsettings*.json`, `web.config`, `logs`, `_documents`,
  kod wyjścia 3 = OK. Zawiera właściwe linki przycisków starych aplikacji. Wersja `3.26.1014.11384` + 1 = `11385`
  (repo i serwer, Api oraz Blazor).
- **Weryfikacja:** `https://app.hcz.com.pl/indigoERP/` -> 200; `Home/SelectAplikacjaByUzytkownik` -> 200; `Uzytkownik/SelectDaneGraph`
  bez tokenu -> 401; hash `indigoERP.Api.dll` i `indigoERP.Blazor.dll` na serwerze zgodny z paczkami; `wwwroot\_documents` na miejscu.

## 2026-10-14 - Przyciski starych aplikacji: właściwe linki

- **Zgłoszenie:** wszystkie przyciski starych aplikacji na stronie głównej prowadziły na stronę główną Intranetu.
- **Zmiana:** `Pages/Home.razor` - tablica `legacyApps` z adresami z `Default.aspx` starego Intranetu
  (`D:\Work\Huta\VisualStudio\intranet\intranet\Default.aspx`; code-behind nie nadpisuje linków): atacamaDMS `/atacama/Default.aspx`,
  hydraRCP `wenus/hydra/`, atestyHCZ `/certificate/`, magazynBHP `wenus/magazyn/`, cerberTMS `/cerber/`,
  warehouseBI `/warehouse/Default.aspx`, kosztyKJ `/kjkoszty/`, transportRAP `/kolejowy/`, bhpAPP `wenus/bhp/Default.aspx`,
  bespinEWA `/bespin/`.
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; działania linków nie sprawdzano.

## 2026-10-14 - Publikacja indigoERP.Blazor na DEIMOS (3.26.1014.11384)

- **Zmiana:** publikacja Release z cyklem `app_offline.htm` do `c:\inetpub\wwwroot\indigoERP` (7 plików skopiowanych,
  kod wyjścia 3 = OK; bez `appsettings*.json`, `web.config`, `logs`, `_documents`). Zawiera: przyciski starych aplikacji
  na dole strony głównej w stylu Bootstrap `btn-warning`. Wersja `3.26.1014.11383` + 1 = `11384` (repo i serwer, Blazor oraz Api).
- **Weryfikacja:** `https://app.hcz.com.pl/indigoERP/` -> 200, hash `indigoERP.Blazor.dll` zgodny z paczką,
  `wwwroot\_documents` na serwerze na miejscu.

## 2026-10-14 - Skrypt: użytkownicy do grup gobiMRP 2409 (Plan) i 2353 (Budżet)

- **Zgłoszenie:** dodać 35 wskazanych osób do grup 2409 i 2353 w bazie indigoERP.
- **Zmiana:** `_SQL Queries/GrupaCzlonkowie_gobiMRP_Plan_Budzet_20261014.sql` - idempotentny `INSERT` do `dbo.GrupaCzlonkowie`
  (jak `GrupaController.InsertCzlonek`: grupa nadrzędna, `uwgGrupaID NULL`, status `2`), loginy rozwiązane po `usrImieNazwisko`
  (34 z 35 jednoznacznie; „Alicja Maćkowiak” = „Alicja Maćkowska-Zapała” (`alicja.mackowska`) - potwierdzone przez
  użytkownika, dodana do listy; razem 35 osób).
- **Status wdrożenia:** skrypt przygotowany, NIE uruchomiony (produkcję SQL uruchamia użytkownik); w transakcji, z kontrolą przed i po.
- **Weryfikacja:** zapytanie tylko do odczytu na JAPET potwierdziło brak brakujących użytkowników i liczbę członkostw do dodania.

## 2026-10-14 - Strona główna: przyciski starych aplikacji w stylu Bootstrap warning

- **Zgłoszenie:** przyciski starych aplikacji mają wyglądać jak przycisk `warning` z Bootstrapa.
- **Zmiana:** `Home.razor` - klasy `btn btn-warning legacy-button`; `Home.razor.css` - usunięte własne kolory, obramowanie,
  cień i hover (daje je Bootstrap), zostały tylko style układu (ikona po lewej, pełna szerokość kolumny).
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; wygląd nie sprawdzany w przeglądarce.

## 2026-10-14 - Strona główna: przyciski starych aplikacji przy dolnej krawędzi

- **Zgłoszenie:** przyciski starych aplikacji mają być na dole, tuż nad stopką.
- **Zmiana:** `Home.razor.css` - `.application-grid` `flex: 1 1 0` (zajmuje wolne miejsce i przewija się wewnętrznie),
  `.legacy-apps` `margin-top: auto` - sekcja przyklejona do dołu obszaru treści, nad stopką.
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; wygląd nie sprawdzany w przeglądarce.

## 2026-10-14 - Strona główna: przywrócone przyciski starych aplikacji, usunięty kafelek „Stary Intranet”

- **Zgłoszenie:** „usuń przycisk dostęp do starych aplikacji” dotyczyło kafelka „Stary Intranet” w lewym panelu, a nie
  nowej sekcji przycisków - sekcję wycofano omyłkowo (patrz wcześniejszy wpis), użytkownik jej nie widział na produkcji.
- **Zmiana:** `Pages/Home.razor` i `Home.razor.css` - przywrócona sekcja `legacy-apps` (10 przycisków, 5 kolumn x 2 rzędy);
  usunięty kafelek `home-shortcut--intranet` wraz ze stylami; siatka lewego panelu przeliczona na 6 skrótów
  (`grid-template-rows: repeat(5, ...)`, kolumny `repeat(6, ...)` w widoku wąskim).
- **Uwaga:** adresy przycisków nadal wskazują `https://intranet.hcz.com.pl/` - właściwe do uzupełnienia w `legacyApps`.
- **Status wdrożenia:** opublikowane na DEIMOS jako `3.26.1014.11383` (cykl `app_offline.htm`, 7 plików skopiowanych,
  kod wyjścia 3 = OK; wersja podbita w repo i na serwerze, Blazor oraz Api).
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; `https://app.hcz.com.pl/indigoERP/` -> 200, hash
  `indigoERP.Blazor.dll` na serwerze zgodny z paczką; wygląd nie sprawdzany w przeglądarce.

## 2026-10-14 - Stary Intranet na WENUS: przekierowanie na https://app.hcz.com.pl/

- **Zgłoszenie:** w `w:\inetpub\wwwroot\intranet\` (WENUS, `\\wenus\c$`) dodać przekierowanie na `https://app.hcz.com.pl/`.
- **Zmiana:** `Web.config` aplikacji (to aplikacja ASP.NET WebForms, brak `appsettings.json`) - w `<system.webServer>`
  dodane `<httpRedirect enabled="true" destination="https://app.hcz.com.pl/" exactDestination="true" httpResponseStatus="Found" />`
  (302 - łatwy do cofnięcia; po potwierdzeniu można zmienić na `Permanent`). Kopia: `Web.config.20261014.bak` w tym samym katalogu.
- **Weryfikacja:** żądanie bezpośrednio do `10.0.45.10` (Host `wenus.hcz.com.pl` i `intranet.hcz.com.pl`, ścieżki `/`,
  `/intranet/`, `/intranet/Default.aspx`) -> 302 `Location: https://app.hcz.com.pl`. Uwaga: DNS `intranet.hcz.com.pl` wskazuje
  na JAPET (10.0.40.101), nie na WENUS - to przekierowanie go nie dotyczy.
- **Cofnięcie:** przywrócić `Web.config.20261014.bak` jako `Web.config`.

## 2026-10-14 - Publikacja indigoERP.Api na DEIMOS (3.26.1014.11382)

- **Zmiana:** publikacja Release z cyklem `app_offline.htm` do `c:\inetpub\wwwroot\indigoERP.Api` (10 plików skopiowanych,
  kod wyjścia 3 = OK; bez `appsettings*.json`, `web.config`, `logs`). Zawiera `Uzytkownik/SelectDaneGraph` i `MicrosoftGraphUserService`.
  Wersja `3.26.1014.11381` + 1 = `11382` (repo i serwer, Api oraz Blazor).
- **Weryfikacja:** `Home/SelectAplikacjaByUzytkownik` -> 200; `Uzytkownik/SelectDaneGraph` bez tokenu -> 401 (akcja istnieje).
  Zmienne systemowe `MicrosoftGraph_TenantId/ClientId/ClientSecret` są ustawione na DEIMOS; samo połączenie z Graph
  (uprawnienie `User.Read.All`, zgoda administratora) nieprzetestowane - zależy od przycisku „Dane z M365”.

## 2026-10-14 - Publikacja indigoERP.Blazor na DEIMOS (3.26.1014.11381)

- **Zmiana:** publikacja Release z cyklem `app_offline.htm` do `c:\inetpub\wwwroot\indigoERP` (robocopy bez `appsettings*.json`,
  `web.config`, `logs`, `_documents`; 11 plików skopiowanych, kod wyjścia 3 = OK). Zawiera: niższe kafelki aplikacji,
  przycisk „Dane z M365” na stronie użytkowników. Wersja `3.26.1014.11380` + 1 (ten sam dzień) = `11381`
  (repo i serwer, Blazor oraz Api).
- **Uwaga:** akcja `Uzytkownik/SelectDaneGraph` jest tylko w kodzie Api - `indigoERP.Api` na DEIMOS nie był publikowany,
  więc przycisk „Dane z M365” zwróci błąd (404) do czasu publikacji Api i ustawienia zmiennych `MicrosoftGraph_*`.
- **Weryfikacja:** `https://app.hcz.com.pl/indigoERP/` -> 200, `wwwroot\_documents` na serwerze na miejscu.

## 2026-10-14 - Strona główna: przyciski do starych aplikacji Intranetu

- **Zgłoszenie:** pod kafelkami aplikacji dodać przyciski dostępu do starych aplikacji ze strony intranet.hcz.com.pl
  (atacamaDMS, hydraRCP, atestyHCZ, magazynBHP, cerberTMS, warehouseBI, kosztyKJ, transportRAP, bhpAPP, bespinEWA),
  w pięciu kolumnach i dwóch rzędach, z ikoną po lewej (FontAwesome), na pełną szerokość kolumny.
- **Zmiana:** `indigoERP.Blazor/Pages/Home.razor` - sekcja `legacy-apps` (tablica `legacyApps` w `@code`);
  `Home.razor.css` - style `legacy-grid` / `legacy-button` (5 kolumn, 3 poniżej 992 px, 2 poniżej 700 px).
- **Uwaga:** adresy wszystkich przycisków wskazywały tymczasowo stronę główną `https://intranet.hcz.com.pl/`.
- **Status wdrożenia:** tylko repozytorium. **Wycofane tego samego dnia na życzenie** - sekcja `legacy-apps`, tablica
  `legacyApps` i style `legacy-*` usunięte z `Home.razor` / `Home.razor.css` (kafelek „Stary Intranet” w lewym panelu został).
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; wygląd nie sprawdzany w przeglądarce.

## 2026-10-14 - Strona główna: niższe kafelki aplikacji

- **Zgłoszenie:** zmniejszyć wysokość paneli z aplikacjami na stronie głównej o ok. 40%.
- **Zmiana:** `indigoERP.Blazor/Pages/Home.razor.css` - `.application-grid` `grid-auto-rows` 172px -> 104px (-40%),
  `.home-loader` `min-height` tak samo.
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; wygląd nie sprawdzany w przeglądarce (trzy linie opisu
  mieszczą się w nowej wysokości wg obliczeń: ikona 62px + padding 28px = 90px < 104px).

## 2026-10-14 - Microsoft Graph: pobieranie brakujących adresów e-mail użytkowników (tylko odczyt)

- **Zgłoszenie:** uzupełnianie brakujących adresów e-mail użytkowników indigoERP danymi z Microsoft 365; na razie tylko
  pobieranie (bez zapisu do bazy i bez UI).
- **Zmiana:** `indigoERP.Api/Services/MicrosoftGraphUserService.cs` (client credentials, `GET /v1.0/users` z paginacją);
  `UzytkownikController.SelectDaneGraph` (`GET api/Uzytkownik/SelectDaneGraph`, rola
  `HCZ-indigoERP-Administrator`, Bearer) - dla użytkowników z pustym `usrEmail` zwraca dopasowany adres z Graph;
  `Domain/Models/Administrator/UzytkownikEmailGraphModel.cs`. UI: przycisk „Dane z M365” na `/administrator/uzytkownik`
  (`Index.razor`) z potwierdzeniem; wynik w panelu bocznym `UzytkownikDaneGraph.razor` (tylko podgląd);
  `IUzytkownikService.SelectDaneGraph`. Konfiguracja: zmienne systemowe `MicrosoftGraph_TenantId`,
  `MicrosoftGraph_ClientId`, `MicrosoftGraph_ClientSecret` -> `MicrosoftGraph:*` (mapowanie w `Program.cs`, jak `indigoERP_*`);
  lokalnie sekcja `MicrosoftGraph` w `appsettings.Development.json` (puste wartości do uzupełnienia).
- **Decyzja techniczna:** `HttpClient` zamiast pakietu `Microsoft.Graph` (brak nowych zależności). Dopasowanie po loginie:
  `usrNazwaAD`/`usrNazwa` (bez domeny, małe litery) do `onPremisesSamAccountName`, a w drugiej kolejności do części loginowej UPN;
  wiele trafień = „Niejednoznaczne”, adres = `mail`, a gdy pusty - UPN. Wymagane uprawnienie aplikacyjne `User.Read.All`
  ze zgodą administratora. Opcja z usługą w authorizationSRV zwracającą dane połączenia - nie realizowana.
- **Status wdrożenia:** kod gotowy; wymaga publikacji indigoERP.Api i ustawienia trzech zmiennych na DEIMOS.
- **Weryfikacja:** `dotnet build indigoERP.Api` - 0 błędów; połączenie z Graph nieprzetestowane (brak danych dostępowych).

## 2026-10-14 - Przywrócony kafelek „Stary Intranet” i publikacja Blazora (3.26.1014.11380)

- **Zgłoszenie:** kafelek „Stary Intranet” ma jednak zostać na stronie głównej; opublikować na DEIMOS.
- **Zmiana:** `Pages/Home.razor` i `Home.razor.css` - kafelek i jego style przywrócone w pierwotnej postaci (pierwszy w lewym panelu).
  Publikacja Blazora z cyklem `app_offline.htm` (7 plików skopiowanych, kod wyjścia 3 = OK; zawiera też zmiany nagłówka
  administratora i linku logo Huty). Wersja `3.26.1014.11379` + 1 (ten sam dzień) = `11380` - repo i serwer (Blazor oraz Api).
- **Weryfikacja:** `https://app.hcz.com.pl/indigoERP/` -> 200; `wwwroot\_documents` na serwerze na miejscu; wygląd
  w przeglądarce nie sprawdzany.

## 2026-10-14 - Nagłówek administratora: „Główna” i link logo Huty

- **Zgłoszenie:** przycisk „Home” w nagłówku stron administratora linkował do `https://app.hcz.com.pl/home` (nieistniejąca
  trasa; strona główna to `/`); ma się nazywać „Główna” i prowadzić do `https://app.hcz.com.pl/administrator`.
  Logo Huty ma linkować do `https://app.hcz.com.pl`.
- **Zmiana:** `Shared/AdministratorHeader.razor` - `Home` -> `Główna`, `href="/home"` -> `href="administrator"` (względny,
  rozwiązywany przez `<base href="~/">`, więc działa niezależnie od PathBase); `Shared/Footer.razor` - logo Huty
  (`Logo16.png`) opakowane linkiem `https://app.hcz.com.pl`.
- **Uwaga:** `Shared/MainHeader.razor` ma ten sam błędny `href="/home"` (nie zmieniany - poza zakresem prośby).
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów.

## 2026-10-14 - Publikacja indigoERP.Blazor na DEIMOS (3.26.1014.11379)

- **Zmiana:** publikacja Release z cyklem `app_offline.htm` do `c:\inetpub\wwwroot\indigoERP` (robocopy bez `appsettings*.json`,
  `web.config`, `logs`, `_documents`; 326 plików skopiowanych, kod wyjścia 3 = OK). Zawiera: usunięty kafelek „Stary Intranet”
  i filtr aplikacji w oknie grup użytkownika. Wersja `3.26.1012.11377` + 2 dni = build `11379`; pole `Version` podmienione w repo
  i na serwerze w Blazorze oraz w `appsettings.json` Api (zgodnie z regułą identycznej wersji; kod Api bez zmian).
- **Weryfikacja:** `https://app.hcz.com.pl/indigoERP/` -> 200, katalog `wwwroot\_documents` na serwerze na miejscu.

## 2026-10-14 - Grupy użytkownika: filtr po aplikacji i tylko grupy bez członkostwa

- **Zgłoszenie:** na oknie grup użytkownika (Administrator -> Użytkownicy) dodać za przyciskiem „Zamknij” combo z filtrem
  aplikacji, a w combo „Wybierz grupę” pokazywać tylko grupy, w których użytkownik jeszcze nie jest.
- **Zmiana:** `indigoERP.Blazor/Pages/Administrator/Uzytkownik/UzytkownikGrupa.razor` - combo „Wszystkie aplikacje”
  (`IAplikacjaService.SelectLookup`), filtruje zarówno listę grup użytkownika (po `appID`), jak i listę grup do wyboru
  (po `groAplikacjaID`); lista do wyboru pomija grupy już przypisane (także dziedziczone).
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; działanie w przeglądarce nie sprawdzane.

## 2026-10-14 - Strona główna: usunięty kafelek "Stary Intranet"

- **Zgłoszenie:** usunąć z lewego panelu skrótów kafelek dostępu do starej wersji portalu.
- **Zmiana:** `indigoERP.Blazor/Pages/Home.razor` - usunięty kafelek `home-shortcut--intranet` (Stary Intranet,
  `https://intranet.hcz.com.pl`); `Home.razor.css` - usunięte nieużywane style tego kafelka.
- **Status wdrożenia:** tylko repozytorium; wymaga publikacji indigoERP.Blazor.
- **Weryfikacja:** `dotnet build indigoERP.Blazor` - 0 błędów; wygląd nie sprawdzany w przeglądarce.

## 2026-10-12 - Publikacja indigoERP.Api i indigoERP.Blazor na DEIMOS (3.26.1012.11377)

- **Zmiana:** publikacja Release z cyklem `app_offline.htm` do `c:\inetpub\wwwroot\indigoERP.Api` i `\indigoERP`
  (robocopy bez `appsettings*.json`, `web.config`, `logs`, `_documents`; Api 10 plików, Blazor 594 pliki; kod wyjścia 3 = OK).
  Na serwerze podmieniono tylko `appSettings:Version`. Wersja w repo podbita w Api i Blazor
  (`3.26.1005.11370` + 7 dni = build `11377`). Zawiera m.in. filtr aplikacji na stronie głównej wg ról użytkownika
  (widok `mvw.vUzytkownikAplikacjaDostep` uruchomiony na JAPET przez użytkownika).
- **Weryfikacja:** `Home/SelectAplikacjaByUzytkownik` zwraca 2 aplikacje dla usrID 3102 i 10 dla usrID 3105;
  `DetailByEmail` przez Gateway bez klucza -> 401. Strony głównej Blazora w przeglądarce nie sprawdzano.

## 2026-10-06 - Strona główna: aplikacje tylko z dostępem użytkownika

- **Zgłoszenie:** lista aplikacji na stronie głównej była filtrowana tylko po `appWyswietl`; ma pokazywać wyłącznie aplikacje,
  w których użytkownik ma rolę (dowolną).
- **Baza danych / Zmiana:** nowy widok `mvw.vUzytkownikAplikacjaDostep` (`_SQL Queries/Aplikacja_Dostep_View.sql`) - pary
  (usrID, appID) z `app.vUzytkownikRola`, czyli ta sama definicja "ma rolę w aplikacji" co w procedurze logowania.
  Kod: `UzytkownikAplikacjaDostepViewModel` + DbSet, akcja `Home/SelectAplikacjaByUzytkownik?usrID=&appStatus=`,
  `IHomeService/HomeService.SelectAplikacjaByUzytkownik`, `Home.razor` pobiera `usrID` z claimu.
- **Decyzja techniczna:** dostęp wyliczany z bazy, a nie z listy ról w claimach - procedura logowania zwraca role tylko dla
  jednej aplikacji (`srlNazwa` = `nazwaAplikacji|rola|...`), więc nie da się z niej ocenić pozostałych aplikacji.
  Aplikacje bez żadnej roli nie będą widoczne dla nikogo.
- **Status wdrożenia:** kod gotowy; wymaga uruchomienia skryptu SQL (CERES, JAPET) i publikacji Api i Blazora.
- **Weryfikacja:** `dotnet build` Api i Blazor - 0 błędów; działanie nieprzetestowane (widok jeszcze nie istnieje w bazie).

## 2026-10-05 - Publikacja indigoERP.Api na DEIMOS (3.26.1005.11370)

- **Zmiana:** `dotnet publish -c Release` i kopia do `c:\inetpub\wwwroot\indigoERP.Api` na DEIMOS z cyklem `app_offline.htm`
  (robocopy bez `appsettings*.json`, `web.config`, `logs`; 21 plików skopiowanych, kod wyjścia 3 = bez błędów).
  W `appsettings.json` na serwerze podmieniono tylko pole `appSettings:Version`; w repo wersja podbita w Api i Blazor
  (`3.26.909.11344` + 26 dni = build `11370`, release `1005`). Blazor nie był publikowany.
- **Weryfikacja:** `GET /Api/Uzytkownik/DetailByEmail` bez klucza na `https://app.hcz.com.pl:9900` -> 401 (aplikacja działa,
  akcja istnieje). Wywołania z kluczem nie testowano.
- **Do zrobienia:** trasa w authorizationSRV.Gateway na DEIMOS (punkt 3) - niewykonana; skrypt `Uzytkownik_NazwaAD.sql`
  musi być uruchomiony na bazie używanej przez DEIMOS.

## 2026-10-05 - Usunięcie projektu indigoERP.Gateway

- **Zgłoszenie:** `indigoERP.Gateway` (YARP, jedna trasa `/Api/Podpis/PreviewByEmail` z rate limiterem) nie jest używany -
  ruch zewnętrzny obsługuje authorizationSRV.
- **Zmiana:** usunięty folder `indigoERP.Gateway`, wpis w `indigoERP.sln`, `ProjectReference` w `indigoERP.AppHost.csproj`
  oraz `AddProject<Projects.indigoERP_Gateway>` i `WithReference(indigoGateway)` w `indigoERP.AppHost/AppHost.cs`.
- **Decyzja techniczna:** pliki dodatku Outlook (`wwwroot/addin/podpis`) Gateway tylko linkował z `indigoERP.Blazor` -
  źródła zostają w Blazor, nic nie przepadło. Pliki Gateway były w git, więc usunięcie jest odwracalne.
- **Status wdrożenia:** zmiana tylko w repozytorium; jeśli Gateway był kiedyś opublikowany w IIS, osobny site/katalog
  na serwerze trzeba wyłączyć ręcznie.
- **Weryfikacja:** `dotnet build indigoERP.AppHost` - 0 błędów; w `indigoERP.sln` brak odwołań do Gateway.

## 2026-10-05 - UzytkownikController.DetailByEmail (klucz API)

- **Zgłoszenie:** potrzebny endpoint zwracający użytkownika na podstawie adresu e-mail; dotąd lookup po `usrEmail`
  istniał tylko wewnątrz `PodpisController.PreviewByEmail` (zwraca HTML podpisu, nie użytkownika).
- **Zmiana:** `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs` - nowa akcja
  `GET api/Uzytkownik/DetailByEmail?usrEmail=` (`[AllowAnonymous]`, bez Bearer), zwraca `UzytkownikViewModel` z `mvw.vUzytkownikLista`
  (jeden wiersz na użytkownika); 401 przy braku/złym nagłówku `X-Api-Key`, 400 dla pustego adresu, 404 gdy brak.
  Do konstruktora kontrolera dodano `IConfiguration`.
- **Decyzja techniczna:** widok `vUzytkownikLista` zamiast `vUzytkownik`, bo ten drugi ma wiersz per aplikacja
  i nie zawiera `usrNazwaAD`/`usrBlokada`/`usrStatus`. Klucz API jak w `Podpis/PreviewByEmail`
  (`appSettings:ApiKey`, nagłówek `X-Api-Key`) - endpoint zwraca dane osobowe (imię, nazwisko, telefon, stanowisko).
  Klucz to bariera przed scrapowaniem, nie prawdziwy sekret, jeśli klient przechowuje go po stronie przeglądarki.
  Na produkcji klucz pochodzi ze zmiennej systemowej `indigoERP_ApiKey` - mapowanie na `appSettings:ApiKey` już istnieje
  w `indigoERP.Api/Program.cs` (`MapMonitoringEnvVar`), nadpisuje wartość z appsettings.json, więc kontroler nie wymagał zmian.
- **Status wdrożenia:** kod gotowy, wymaga publikacji indigoERP.Api; bez zmian SQL. Zmienna `indigoERP_ApiKey` musi być
  ustawiona na serwerze (używa jej już PreviewByEmail).
- **Weryfikacja:** `dotnet build indigoERP.Api` - 0 błędów.

## 2026-09-23

### Home - rozmiar ikon skrótów

- `indigoERP.Blazor/Pages/Home.razor.css` - ikony fontowe Starego Intranetu i hutaForms mają `2.6rem`,
  zgodnie z wizualnym rozmiarem obrazu ikony Poczty (`46px`).
- Weryfikacja: reguła dotyczy wyłącznie dwóch wskazanych kafelków i nie zmienia ich wymiarów ani układu.

### Administrator / Użytkownik - przywrócenie działania

#### Zgłoszenie

Strona `Administrator/Użytkownik` przekierowywała do strony błędu.

#### Przyczyna i zmiana

- `UzytkownikListaViewModel` i `UzytkownikController.Select` odczytują `usrNazwaAD`, ale produkcyjny widok
  `mvw.vUzytkownikLista` nie zwracał tej kolumny.
- Dodano skrypt `_SQL Queries/indigoERP20260923143000.sql`, który wykonuje `CREATE OR ALTER VIEW` i zwraca
  `dbo.Uzytkownik.usrNazwaAD` jako `usrNazwaAD`.
- Skrypt zapisano jako UTF-8 BOM i wykonano na produkcyjnej bazie `indigoERP` na DEIMOS.

#### Weryfikacja

- Produkcyjny `GET https://app.hcz.com.pl:9900/Api/Uzytkownik/Select?usrStatus=2` zwraca `200`.
- Produkcyjny widok ma kolumnę `usrNazwaAD`.
- Po odświeżeniu `https://app.hcz.com.pl/administrator/uzytkownik` grid jest widoczny i renderuje dane.

### Zgłoszenie

Stopka `indigoERP` ma odpowiadać stopce używanej w `atacamaDMS`.

### Zmienione obszary

- `indigoERP.Blazor/Shared/Footer.razor` - zastosowano ten sam układ copyright, telefonu i poczty IT.
- `indigoERP.Blazor/Shared/Footer.razor.css` - przywrócono styl odnośnika e-mail zgodny z `atacamaDMS`.
- `indigoERP.Blazor/wwwroot/css/site.css` - stopka używa koloru, wielkości i grubości tekstu z wzorca `atacamaDMS`.

### Decyzje techniczne

- Wykorzystano istniejące pliki `images/footer/ico_phone.png` i `images/footer/ico_email.png`; ich zawartość jest identyczna
  z ikonami używanymi przez `atacamaDMS`.
- Usunięto dodatkowy panel wersji API, serwera SQL i bazy, ponieważ nie występuje w działającej stopce `atacamaDMS`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `dotnet build .\indigoERP.Blazor\indigoERP.Blazor.csproj --configuration Release --no-restore -v:q`
  z odrębnym katalogiem wynikowym - poprawny, 21 istniejących ostrzeżeń nullable, 0 błędów.
- `git diff --check` dla zmienionych plików - poprawny.

### Korekta zgodności z atacamaDMS

- Usunięto panel `Wersja API / Serwer / Baza`, ponieważ nie jest widoczny w działającej stopce `atacamaDMS`.
- `dotnet build .\indigoERP.Blazor\indigoERP.Blazor.csproj --configuration Release --no-restore -v:q`
  z odrębnym katalogiem wynikowym - poprawny, 168 istniejących ostrzeżeń, 0 błędów.
- Poprawkę opublikowano na DEIMOS; odświeżona strona produkcyjna pokazuje wyłącznie copyright, telefon i e-mail IT.

### Publikacja DEIMOS

- Opublikowano wyłącznie `indigoERP.Blazor` na `\\DEIMOS\c$\inetpub\wwwroot\indigoERP` z commita `e38a4a6`.
- Zachowano produkcyjne `appsettings.json`, `web.config`, katalogi `logs` i `wwwroot\_documents`.
- Tymczasowy plik `app_offline.htm` został usunięty po kopii.
- Hashe `indigoERP.Blazor.dll` i `wwwroot\indigoERP.Blazor.styles.css` są zgodne z paczką; strona główna oraz stylesheet
  produkcyjny zwracają `200`.
- Końcowa paczka zawiera także wcześniejsze zmiany `Pages/Home.razor` i `Pages/Home.razor.css`: panel skrótów oraz układ
  kafli aplikacji. Nie włączono żadnych pozostałych lokalnych zmian projektu.

## 2026-09-23

### Publikacja DEIMOS

Opublikowano frontend `indigoERP.Blazor` na DEIMOS po dodaniu skrótów starego Intranetu i hutaForms oraz zmianie układu
kafli aplikacji.

### Zakres wdrożenia

- Docelowy katalog: `\\DEIMOS\c$\inetpub\wwwroot\indigoERP`.
- Opublikowano wyłącznie `indigoERP.Blazor`; API nie było częścią tego wdrożenia.
- Zachowano produkcyjne `appsettings.json`, `web.config`, katalog `logs` oraz `wwwroot\_documents`.
- Usunięto z pakietu 12 pomocniczych plików w katalogu `bin` przed kopią, dlatego nie powstał dodatkowy katalog `bin` na DEIMOS.
- Tymczasowy plik `app_offline.htm` został usunięty po skopiowaniu plików.

### Weryfikacja

- `dotnet publish indigoERP.Blazor` - poprawny, 0 błędów i istniejące ostrzeżenia nullable.
- Skróty SHA-256 wdrożonych `indigoERP.Blazor.dll` i `wwwroot\indigoERP.Blazor.styles.css` są zgodne z pakietem.
- `https://app.hcz.com.pl/` oraz `https://app.hcz.com.pl/indigoERP.Blazor.styles.css` zwracają `200`.
- Odpowiedź produkcyjnego stylesheetu zawiera reguły `home-shortcut--huta-forms` i poziomego układu kafla aplikacji.

## 2026-09-23

### Zgłoszenie

Kafle aplikacji mają wyświetlać kolorową ikonę po lewej oraz tytuł i opis po prawej. W lewym panelu mają pojawić się
na początku skróty do starego Intranetu i hutaForms.

### Zmienione obszary

- `indigoERP.Blazor/Pages/Home.razor` - dodano skróty do `https://intranet.hcz.com.pl` i hutaForms oraz klasy wariantów ikon.
- `indigoERP.Blazor/Pages/Home.razor.css` - przebudowano kafle aplikacji do układu poziomego i wprowadzono kolory ikon.

### Decyzje techniczne

- Ikony aplikacji otrzymują cyklicznie cztery kontrastowe kolory, niezależnie od jasnego lub ciemnego motywu.
- Panel lewy przewiduje sześć pełnych skrótów oraz skrót formularza zgłoszenia o połowie wysokości.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `DailyFileLoggerProviderTests` potwierdza utworzenie i zawartość dziennego pliku w katalogu tymczasowym.
- `dotnet test indigoERP.sln --no-restore`: 43/43 testów pomyślnie.

## 2026-10-17 - Odczyt zakresów wiadomości

- Keyless `WiadomoscZakresEnumerator` jest mapowany do widoku `enum.WiadomoscZakres`, a endpoint Bearer ma trasę `GET api/WiadomoscZakres/Select?appID={id}`.
- Endpoint zwraca wyłącznie aktywne rekordy (`wzaStatus = '2'`) przypisane do podanej aplikacji lub wspólne (`wzaAplikacjaID = 0`).
- Model odczytowy zawiera `wzaNazwa` bezpośrednio po `wzaAplikacjaID`, zgodnie z aktualną strukturą tabeli.
- `GET api/WiadomoscPracownik/SelectByWiadomoscZakres/{wzaID}` odczytuje odbiorców z widoku `app.vWiadomoscZakres...` wskazanego przez aktywny zakres i zwraca `prcID`, dane pracownika oraz kontakt.
- Nazwa widoku jest pobierana wyłącznie z zakresu w bazie i walidowana względem dozwolonego wzorca; nie jest przyjmowana od klienta jako fragment SQL.
- `rowguid` nie jest zwracany przez model odczytowy. DDL nie zmieniano, ponieważ tabela i widok zostały utworzone poza tym zadaniem.
- `dotnet test indigoERP.Api.Tests\indigoERP.Api.Tests.csproj --no-restore`: 22/22 testów pomyślnie; test kontraktu potwierdza parametr `appID`.

## 2026-10-16

### Zgłoszenie

Dodanie API dla tabeli `dbo.WiadomoscPracownik`.

### Zmienione obszary

- `indigoERP.Domain/Entities/Wspolne/WiadomoscPracownik.cs`
- `indigoERP.Domain/Dtos/Wspolne/WiadomoscPracownikDto.cs`
- `indigoERP.Api/Data/indigoERPContext.cs`
- `indigoERP.Api/Controllers/Wspolne/WiadomoscPracownikController.cs`
- `indigoERP.Api.Tests/WiadomoscPracownikControllerTests.cs`

### Decyzje techniczne

- Kontroler ma uwierzytelnienie Bearer i obsługuje `Select`, `SelectByWiadomosc`, `Get`, `Insert`, `Update` oraz `Delete`.
- Lista jest filtrowana po `wprStatus`, a lista odbiorców wiadomości po `wprWiadomoscID` i `wprStatus`.
- DTO waliduje format i maksymalną długość adresu e-mail oraz długość statusu. `rowguid` nie jest mapowany do encji API.

### Zmiany bazy danych

Brak. Tabela `dbo.WiadomoscPracownik` została wcześniej utworzona przez użytkownika.

### Weryfikacja

- `dotnet test indigoERP.Api.Tests\indigoERP.Api.Tests.csproj --no-restore`: 21/21 testów pomyślnie.

### Weryfikacja

- `dotnet build .\indigoERP.Blazor\indigoERP.Blazor.csproj --configuration Release --no-restore -v:q`
  z odrębnym katalogiem wynikowym - poprawny, 21 istniejących ostrzeżeń, 0 błędów.
- `git diff --check` dla zmienionych plików - poprawny.

## 2026-09-22

### Publikacja DEIMOS

Opublikowano frontend `indigoERP.Blazor` na DEIMOS po zmianach układu strony głównej i stopki.

### Zakres wdrożenia

- Docelowy katalog: `\\DEIMOS\c$\inetpub\wwwroot\indigoERP`.
- Opublikowano wyłącznie `indigoERP.Blazor`; API nie było częścią tego wdrożenia.
- Zachowano produkcyjne `appsettings.json`, `web.config`, katalog `logs` oraz `wwwroot\_documents`.
- Tymczasowy plik `app_offline.htm` został usunięty po skopiowaniu plików.
- Usunięto dodatkowy katalog `bin` utworzony omyłkowo przez pakiet publikacyjny; zawierał wyłącznie 8 plików pomocniczych
  wygenerowanych podczas tej publikacji.

### Weryfikacja

- `dotnet publish indigoERP.Blazor` - poprawny, 0 błędów i istniejące ostrzeżenia nullable.
- Skróty SHA-256 wdrożonych `indigoERP.Blazor.dll` i `wwwroot\indigoERP.Blazor.styles.css` są zgodne z pakietem.
- `https://app.hcz.com.pl/` z poświadczeniami Windows zwraca `200`.
- `https://app.hcz.com.pl/indigoERP.Blazor.styles.css` zwraca `200` i 23636 bajtów.
- Produkcyjny `appsettings.json` nadal wskazuje `https://app.hcz.com.pl:9900/Api/`, a `web.config` nadal zapisuje logi stdout.

## 2026-09-21

### Zgłoszenie

Stałe skróty `IT Support`, `Poczta`, `Kalendarz`, `OneDrive` i formularz wiadomości do IT mają znaleźć się jeden pod drugim
w lewym panelu. Dynamiczna lista aplikacji ma być wyświetlana w głównej części strony w równych kaflach.

### Zmienione obszary

- `indigoERP.Blazor/Pages/Home.razor` - przeniesiono stałe skróty do panelu lewego, a dynamiczne aplikacje do części głównej.
- `indigoERP.Blazor/Pages/Home.razor.css` - ustawiono lewy panel o szerokości 350px oraz równą siatkę aplikacji po cztery kafle w wierszu.

### Decyzje techniczne

- Kafle aplikacji mają stałą wysokość 172px i jednakową szerokość w obrębie wiersza.
- Układ automatycznie przechodzi na trzy, dwa lub jeden kafel w wierszu wraz ze zmniejszaniem szerokości ekranu.
- Skrót formularza zgłoszenia ma połowę wysokości pozostałych skrótów i zawiera wyłącznie ikonę oraz nazwę.
- Pola ikon mają grafitowe tło w obu motywach, aby białe ikony aplikacji były czytelne także w trybie jasnym.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `dotnet build .\indigoERP.Blazor\indigoERP.Blazor.csproj --configuration Release --no-restore -v:q`
  z odrębnym katalogiem wynikowym - poprawny, 21 istniejących ostrzeżeń, 0 błędów.
- `git diff --check` dla zmienionych plików - poprawny.
- Podgląd `https://localhost:8900/` nie był dostępny w czasie weryfikacji (`ERR_CONNECTION_REFUSED`).

### Zgłoszenie

Informacja o wersji API, serwerze SQL i bazie danych w stopce ma być wyśrodkowana.

### Zmienione obszary

- `indigoERP.Blazor/Shared/Footer.razor` - rozdzielono informację copyright i panel informacji serwera.
- `indigoERP.Blazor/Shared/Footer.razor.css` - zastosowano trójkolumnowy układ stopki, który utrzymuje panel serwera na środku.

### Decyzje techniczne

- Prawa, pusta kolumna równoważy kolumnę copyright, dlatego panel API/SQL pozostaje na środku całej stopki.
- Dla szerokości do 900px stopka układa zawartość pionowo, bez ryzyka nakładania tekstu.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `dotnet build .\indigoERP.Blazor\indigoERP.Blazor.csproj --configuration Release --no-restore -v:q` - zatrzymany przez blokadę
  `indigoERP.Domain.dll` i `indigoERP.Service.dll` przez uruchomione `devenv.exe` oraz `indigoERP.Blazor.exe`; bez błędu składni stopki.
- `git diff --check` dla zmienionych plików - poprawny.

### Zgłoszenie

Zmiana strony głównej: boczna lista aplikacji ma być kafelkami w kolejnych wierszach, komunikat o starszych wersjach przez INTRANET
ma zostać usunięty, a link do wiadomości dla Działu IT ma być zastąpiony ikoną poczty.

### Zmienione obszary

- `indigoERP.Blazor/Pages/Home.razor` - usunięto panel o starszych wersjach aplikacji i zastąpiono komunikat wsparcia ikoną poczty.
- `indigoERP.Blazor/Pages/Home.razor.css` - przebudowano boczną listę aplikacji na kafelki, zmniejszono kafle centralne i dopasowano responsywność.

### Decyzje techniczne

- Kafle aplikacji w panelu bocznym działają na tej samej dynamicznej liście `SelectAplikacja("2")`, zmienił się tylko sposób prezentacji.
- Kafle `IT Support`, `Poczta`, `Kalendarz` i `OneDrive` pozostają dostępne, ale mają kompaktowy układ.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `dotnet build .\indigoERP.Blazor\indigoERP.Blazor.csproj --configuration Release --no-restore -v:q` - poprawny, 6 ostrzeżeń, 0 błędów.

## 2026-09-09

### Zgłoszenie

Po przenosinach na nowy serwer IIS (DEIMOS) logowanie przez Windows Authentication kończyło się timeoutem
`HttpClient.Timeout` (100s) w `ClaimsTransformation.TransformAsync` → `LoginService.GetSso`, mimo poprawnego
uwierzytelnienia Windows/Kerberos.

### Diagnoza

Wykluczono kolejno: SPN/Kerberos dla witryny (naprawione wcześniej, App Pool na `HCZ\iis.account`), łączność sieciową
DEIMOS→JAPET (test SQL Auth przez SSMS zadziałał), błędny `appSettings:BaseApiUrl` w Blazor (był
`http://deimos.hcz.com.pl:9900/Api/` zamiast `https://...`, poprawione), oraz samą procedurę `uspGetUzytkownikSso`
(uruchomiona wprost w SSMS na `JAPET` zwróciła dane natychmiast). Dodany endpoint diagnostyczny `Ping` (bez SQL)
odpowiedział w ~4s zamiast wisieć 100s, co potwierdza że warstwa Blazor→API/hosting działa. Pozostaje najbardziej
prawdopodobna przyczyna: zatkana pula połączeń `SqlClient` w procesie API po wielu nieudanych/timeoutujących próbach
podczas testów — do potwierdzenia po recyclu App Poola API.

### Zmienione obszary

- `indigoERP.Api/Controllers/Autoryzacja/LoginController.cs` - dodano `GET api/Login/Ping`, endpoint diagnostyczny
  bez dostępu do `_context`/SQL, do izolowania warstwy sieciowej od warstwy bazy danych.
- `indigoERP.Blazor/appsettings.json` - usunięto nieużywany klucz `appSettings:ApiKey` (nie był odczytywany nigdzie
  w kodzie C# Blazor/Service; po stronie API analogiczny klucz jest już poprawnie pobierany ze zmiennej środowiskowej
  `indigoERP_ApiKey`, patrz `indigoERP.Api/Program.cs`).

### Decyzje techniczne

- `Ping` umieszczono w istniejącym `LoginController` (już ma `[AllowAnonymous]`), zamiast tworzyć osobny kontroler,
  żeby nie dodawać nowej powierzchni do utrzymania dla tymczasowej diagnostyki.

### Status wdrożenia

W trakcie. Po recyclu App Poola zawieszenie 100s zniknęło, ale ujawniło kolejną warstwę problemu: `Jwt:Key`,
`appSettings:ApiKey`, `InternalSso:Key` mają być pobierane wyłącznie ze zmiennych środowiskowych `indigoERP_JwtKey`,
`indigoERP_ApiKey`, `InternalSso` (Machine scope) — patrz `indigoERP.Api/Program.cs` i `indigoERP.Blazor/Program.cs`.
Na DEIMOS zmienne były początkowo ustawione jako **User scope** (widoczne tylko dla konta, którym je ustawiono, nie
dla `HCZ\iis.account` na którym działa App Pool), przez co API nigdy ich nie widziało — stąd `ArgumentNullException`
w `Encoding.GetBytes(_configuration["jwt:Key"])` przy starcie uwierzytelniania JWT. Przeniesiono je do Machine scope,
ale ani `iisreset`, ani wcześniejszy reboot (wykonany zanim zmienne były poprawnie ustawione) tego nie naprawiły —
`services.exe` cache'uje blok środowiska Machine przy starcie systemu, nie przy restarcie pojedynczej usługi, więc
potrzebny jest pełny reboot serwera **po** ustawieniu zmiennych na Machine scope. Zaplanowany na jutro rano.

**Obejście tymczasowe do czasu rebootu:** wartości `Jwt:Key`, `appSettings:ApiKey` (w `indigoERP.Api/appsettings.json`)
i `InternalSso:Key` (w obu `appsettings.json`, ta sama wartość) wpisane wprost do opublikowanych plików na DEIMOS
(nie w repo źródłowym — to sekrety). Do usunięcia z powrotem po potwierdzeniu, że zmienne środowiskowe działają
po reboocie.

### Weryfikacja

- `Invoke-WebRequest` na `https://deimos.hcz.com.pl:9900/Api/Login/Ping` z DEIMOS: ~4,1s (zamiast 100s timeout).
- `EXEC uspGetUzytkownikSso @usrNazwa='HCZ\andrzej.latocha', @sesKomputer='WebBrowser', @appID=900` w SSMS na `JAPET`:
  wynik natychmiastowy, poprawne dane użytkownika.
## 2026-08-13

### Zgłoszenie

Zwiększenie szerokości okna slide `Pokaż role` na stronie Administrator/Użytkownik tak jak w funkcji `Pokaż grupy`.

### Zmienione obszary

- `indigoERP.Blazor/Pages/Administrator/Uzytkownik/Index.razor` - `Pokaż role` używa szerokich opcji okna slide.

### Decyzje techniczne

- Zastosowano istniejący wariant `GetWideRelationSideDialogOptions()`, żeby szerokość była identyczna jak dla `Pokaż grupy`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `dotnet build .\indigoERP.Blazor\indigoERP.Blazor.csproj --configuration Release --no-restore -v:q` - poprawny, 11 ostrzeżeń, 0 błędów.
## 2026-08-13

### Zgłoszenie

Dodanie obsługi słownika `dic.JednostkaMiara` w API: standardowy `Select` z parametrem `jmiStatus`, `Insert`, `Update`, `Delete`
oraz `SelectLookup` z widoku `enum.JednostkaMiara`.

### Zmienione obszary

- `indigoERP.Domain/Entities/Slownik/JednostkaMiara.cs` - nowa encja tabeli `dic.JednostkaMiara`, bez dziedziczenia po `AuditableEntity`.
- `indigoERP.Domain/Dtos/Slownik/JednostkaMiaraDto.cs` - nowy DTO do edycji z walidacją pól `jmiKOD`, `jmiOpis` i `jmiStatus`.
- `indigoERP.Api/Data/indigoERPContext.cs` - dodany `DbSet<JednostkaMiara>`.
- `indigoERP.Api/Controllers/Slownik/JednostkaMiaraController.cs` - dodane `Select`, `Get`, `Insert`, `Update`, `Delete` i uporządkowany `SelectLookup`.

### Decyzje techniczne

- `SelectLookup` nadal zwraca `JednostkaMiaraEnumerator` z widoku `enum.JednostkaMiara`.
- Operacje edycyjne używają `JednostkaMiaraDto`; walidacja jest tylko w DTO.
- `Delete` usuwa rekord z tabeli, ponieważ encja nie ma pól audytowych ani `Usuniety`.

### Zmiany bazy danych

Brak. Wykorzystano istniejącą tabelę `dic.JednostkaMiara` oraz widok `enum.JednostkaMiara`.

### Weryfikacja

- `dotnet build .\indigoERP.Api\indigoERP.Api.csproj --configuration Release --no-restore -v:q` - poprawny, 625 ostrzeżeń, 0 błędów.

## 2026-08-02

### Zgłoszenie

Rozszerzenie monitoringu (temat z 2026-08-01) o czwarty typ serwera "API" — lista instancji API do monitorowania obok WIN/SQL/ORA, sprawdzana przez HTTP GET zamiast ping/TCP. Dodatkowo: ikony typów na dashboardzie, zmienne środowiskowe indigoERP_JwtKey/indigoERP_ApiKey (pojedyncze podkreślenie, nie __).

### Zmienione obszary

- `indigoERP.Api/Services/ServerCheckService.cs` — case `"API"` w `CheckAsync`, nowa metoda `CheckHttpAsync` (HTTP GET, `IsAlive = response.IsSuccessStatusCode`, pole `srvIP` przechowuje pełny URL nie samo IP), wstrzyknięty `IHttpClientFactory`.
- `indigoERP.Api/Program.cs` — `services.AddHttpClient()`; rozszerzone mapowanie zmiennych środowiskowych o `indigoERP_JwtKey` → `Jwt:Key` i `indigoERP_ApiKey` → `appSettings:ApiKey` (ta sama mechanika co `indigoERP_MonitoringSqlLogin`/`Password` — pojedyncze podkreślenie, ręczne mapowanie bo wbudowany dostawca ASP.NET Core wymaga `__`); drugi log startowy potwierdzający czy `Jwt:Key`/`appSettings:ApiKey` są ustawione (bez ujawniania wartości).
- `indigoERP.Blazor/Pages/Administrator/Monitor/Index.razor` (+ `.razor.css`) — ikony per typ serwera (`desktop_windows`/`storage`/`database`/`cloud`) w kolorowych plakietkach, karta API też dostała ikonę `cloud`.
- `indigoERP.Blazor/Pages/Administrator/Serwer/Index.razor`, `Update.razor` — etykieta pola `srvIP` zmieniona na "IP / host / URL" + podpowiedź co wpisać dla każdego typu.
- `_SQL Queries/Serwer_Monitoring_TypApi.sql` — nowy wiersz `dic.SerwerTyp` (`API`/`Web API`).

### Decyzje techniczne

- Dla typu `API` pole `srvIP` przechowuje pełny URL do sprawdzenia (np. adres endpointu health/`GetInfo`), nie samo IP — `srvPort` jest wtedy nieużywany (port jest już częścią URL). Etykiety w UI to wyjaśniają.
- Check HTTP uznaje serwer za żywy tylko przy `IsSuccessStatusCode` (2xx/3xx) — inaczej niż ping/TCP-probe, które sprawdzają tylko dostępność transportową. Dla API liczy się realna odpowiedź aplikacji, więc 4xx/5xx = "down" z komunikatem `HTTP {kod} {powód}`.
- `ad.vUzytkownik`/ADSI — **zbadane, nie wdrożone**: żaden z projektów (indigoERP/atacamaDMS/cerberTMS) ani firmowe wzorce nie mają precedensu integracji z AD przez ADSI/LDAP; istnieją tylko puste kolumny `usrNazwaAD`/`usrGuidAD`/`usrSynchronizacjaAD` (rzeczywista synchronizacja idzie z `epicaHR`). Czeka na dane od użytkownika (domena/DC, base DN, konto serwisowe) zanim powstanie DDL.

### Zmiany bazy danych

Do wykonania przez użytkownika: `_SQL Queries/Serwer_Monitoring_TypApi.sql` (dodaje `('API','Web API','2')` do `dic.SerwerTyp`) — **nie uruchomiony z tej sesji**.

### Weryfikacja

- `dotnet build indigoERP.sln --nologo -v quiet` — kompilacja poprawna, 0 błędów.
- Bez dostępu do żywego środowiska z tej sesji — weryfikacja UI/HTTP-check po stronie użytkownika.

## 2026-08-01

### Zgłoszenie

Monitoring działania serwerów (Windows/SQL/Oracle) i API na stronie Administrator, z pętlą sprawdzającą co 5 minut, CRUD-em rejestru serwerów, drill-down do JOB-ów SQL Agenta oraz osobnym panelem informacyjnym w stopce (wersja API/serwer/baza).

### Zmienione obszary

- `indigoERP.Domain/Entities/Slownik/SerwerTyp.cs`, `Entities/Administrator/Serwer.cs`, `Entities/Administrator/SerwerLog.cs`
- `indigoERP.Domain/Enumerators/Slownik/SerwerTypEnumerator.cs`
- `indigoERP.Domain/Dtos/Administrator/SerwerDto.cs`
- `indigoERP.Domain/ViewModels/Administracja/SerwerViewModel.cs`, `MonitorSerwerStatusViewModel.cs`, `SerwerInfoViewModel.cs`, `SqlAgentJobViewModel.cs`
- `indigoERP.Api/Data/indigoERPContext.cs` (nowe `DbSet`, mapowanie `Serwer`/`SerwerLog`)
- `indigoERP.Api/Services/IServerCheckService.cs`, `ServerCheckService.cs`, `ServerMonitoringHostedService.cs`
- `indigoERP.Api/Controllers/Slownik/SerwerTypController.cs`
- `indigoERP.Api/Controllers/Administrator/SerwerController.cs` (CRUD, `GetInfo`, `SelectStatus`, `Check`, `SelectJoby`)
- `indigoERP.Api/Program.cs`, `appsettings.json` (`MonitoringIntervalMinutes`)
- `indigoERP.Services/Interfaces` i `Services` — `Administrator/ISerwerService.cs`/`SerwerService.cs`, `Slownik/ISerwerTypService.cs`/`SerwerTypService.cs`
- `indigoERP.Blazor/Program.cs` (rejestracja serwisów)
- `indigoERP.Blazor/Pages/Administrator/Serwer/Index.razor`, `Update.razor` (CRUD rejestru serwerów; komponent edycji nazwany `Update`, nie `Edit`)
- `indigoERP.Blazor/Pages/Administrator/Monitor/Index.razor` (+ `.razor.css`), `Zadanie.razor` (dashboard + drill-down JOB-ów, route `/administrator/monitor/zadanie/{srvID}`)
- `indigoERP.Blazor/Pages/Administrator/Index.razor` (kafelek "Serwery")
- `indigoERP.Blazor/Shared/Footer.razor` (panel informacyjny API/SQL/baza)
- `indigoERP.Blazor/Pages/Administrator/Rola/RolaGrupa.razor` (było `RolaGrupy.razor`), `RolaUzytkownik.razor` (było `RolaUzytkownicy.razor`)
- `indigoERP.Blazor/Pages/Administrator/Grupa/GrupaRola.razor` (było `GrupaRole.razor`), `GrupaUzytkownika.razor` (było `GrupaUzytkownicy.razor`)
- `indigoERP.Blazor/Pages/Administrator/Uzytkownik/UzytkownikGrupa.razor` (było `UzytkownikGrupy.razor`), `UzytkownikRola.razor` (było `UzytkownikRole.razor`)
- `indigoERP.Api/Controllers/Administrator/RolaController.cs` (`SelectUzytkownikByRola`), `UzytkownikController.cs` (`SelectRolaByUzytkownik`) — przebudowane z ręcznego LINQ Join+Concat+GroupBy na jedno zapytanie SQL (`FromSqlInterpolated`) oparte na `mvw.vUzytkownikRolaLista`
- `_SQL Queries/Serwer_Monitoring.sql`, `_SQL Queries/Serwer_Monitoring_Login.sql`

### Decyzje techniczne

- `SerwerLog` (historia sprawdzeń) celowo **bez** `AuditableEntity` — log systemowy, append-only, nigdy edytowany/usuwany przez użytkownika.
- Ostatni status per serwer czytany z widoku `mvw.vSerwerStatus` (`OUTER APPLY ... TOP(1)`), nie przez LINQ `GroupBy().First()` — EF Core nie gwarantuje tłumaczenia tego wzorca na SQL.
- WIN sprawdzany ICMP ping, SQL/Oracle przez TCP connect-probe (port `srvPort` albo domyślny 1433/1433/1521) — bez przechowywania haseł.
- Pętla sprawdzająca to `BackgroundService` w `indigoERP.Api` (nie w Blazor) — działa niezależnie od otwartych sesji przeglądarki.
- `SerwerController.GetInfo()` wzorowany na `atacamaDMS.Api/Controllers/_Administrator/SerwerController.Get()` — ta sama nazwa kontrolera co CRUD rejestru serwerów, ale osobna akcja/ViewModel (`SerwerInfoViewModel`) żeby uniknąć konfliktu znaczeń; dostępny dla każdego zalogowanego (`[Authorize(AuthenticationSchemes="Bearer")]` bez `Roles`), bo stopka jest widoczna dla wszystkich.
- `SelectJoby` łączy się z `msdb` wybranego serwera SQL przez **logowanie SQL** (jeden wspólny login/hasło w `appSettings:MonitoringSqlLogin`/`MonitoringSqlPassword`), nie Windows Integrated Security — Integrated Security nie działa przez VPN. Login trzeba założyć na każdym monitorowanym serwerze SQL skryptem `_SQL Queries/Serwer_Monitoring_Login.sql` (rola `SQLAgentReaderRole` w `msdb`, żeby widzieć wszystkie JOB-y niezależnie od właściciela).
- Filtr JOB-ów ograniczony do nazw zaczynających się od `ujb` i `MaintenancePlan`, prezentowanych w dwóch kolumnach 50%/50%.
- Strona Monitor jest wyłącznie podglądem statusów (karty z kolorem: zielony/czerwony/szary dla nieświeżych danych) — edycja danych serwera przeniesiona do osobnej strony `/administrator/serwer` (przycisk "Zarządzaj serwerami").
- Strony relacji Rola/Grupa/Uzytkownik przemianowane na formy liczby pojedynczej (np. `RolaUzytkownicy` → `RolaUzytkownik`) — bez zmian funkcjonalnych, tylko nazwy plików/komponentów i miejsca `OpenSideAsync<...>` w odpowiednich `Index.razor`.
- `RolaController.SelectUzytkownikByRola` i `UzytkownikController.SelectRolaByUzytkownik` przebudowane analogicznie do wcześniejszej migracji `UzytkownikController.SelectByRola` (2026-07-29): jedno zapytanie `FromSqlInterpolated` z CTE `UNION ALL` (widok `mvw.vUzytkownikRolaLista` dla członkostwa dziedziczonego + `dbo.RolaCzlonkowie` dla bezpośredniego) i `ROW_NUMBER()` do deduplikacji z tym samym priorytetem co poprzedni kod C# (wiersz z widoku wygrywa nad bezpośrednim).
- `SerwerController.SelectJoby` wykrywa wersję docelowego serwera SQL (`SERVERPROPERTY('ProductMajorVersion')`) i dobiera zapytanie: `OUTER APPLY`/`TOP (n)` dla SQL Server 2005+ (w tym 2008), osobny wariant `LEFT JOIN` + skorelowany `MAX` (bez APPLY/TOP(n)/ROW_NUMBER) dla SQL Server 2000. Skrypt loginu ma analogiczny podział: `_SQL Queries/Serwer_Monitoring_Login.sql` (2005+, `sp_addrolemember` zamiast `ALTER ROLE ... ADD MEMBER` bo ten drugi wymaga SQL Server 2012+) i `_SQL Queries/Serwer_Monitoring_Login_SQL2000.sql` (stare `sp_addlogin`/`sp_grantdbaccess`, rola `db_datareader` zamiast nieistniejącej w 2000 `SQLAgentReaderRole`).
- Pole `Serwer.srvTypKOD` przemianowane na `srvSerwerTypKOD` (pełna konwencja `xxxNazwaTabeliKOD` dla FK do `dic.SerwerTyp`, zamiast skróconej wersji) — zmiana wykonana najpierw na bazie przez użytkownika, dociągnięta do C#/Blazor/DDL w repo.

### Zmiany bazy danych

Wykonano `_SQL Queries/Serwer_Monitoring.sql` na `JAPET/indigoERP.Developer`: `dic.SerwerTyp` + `enum.SerwerTyp` (słownik WIN/SQL/ORA, zasiane), `dbo.Serwer` + `mvw.vSerwerLista`, `dbo.SerwerLog` + `IX_SerwerLog01`, `mvw.vSerwerStatus`.

### Weryfikacja

- `dotnet build indigoERP.sln --nologo -v quiet` (i pełny `-t:Rebuild`) — kompilacja poprawna, 0 błędów, po każdym etapie zmian (dodanie funkcji, przełączenie na login SQL, zmiany nazw, refaktor zapytań mvw).
- Skrypt DDL uruchomiony przez użytkownika na `JAPET/indigoERP.Developer` przed kompilacją API (Database-First).
- `_SQL Queries/Serwer_Monitoring_Login.sql` przygotowany, ale **nie uruchomiony** z tej sesji — login/hasło i wykonanie na monitorowanych serwerach SQL po stronie użytkownika.
- Manualna weryfikacja przez Swagger/UI (dodanie serwera, podgląd statusu, JOB-y) do wykonania przez użytkownika — brak dostępu do żywego środowiska z tej sesji.

## 2026-07-29

### Zgłoszenie

Sprawdzenie funkcji `Grupa.SelectUzytkownikByGrupa` po publikacji anonimowych endpointów.

### Zmienione obszary

- `indigoERP.Services/Services/Administrator/GrupaService.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Kontroler `Grupa.SelectUzytkownikByGrupa` pozostaje `[AllowAnonymous]`.
- Wywołanie produkcyjne `Api/Grupa/SelectUzytkownikByGrupa/-1/1?usrStatus=2` zwróciło `200 OK` bez tokena.
- W serwisie Blazor funkcja `SelectUzytkownikByGrupa` używa zwykłego klienta `baseApiClient`, bez pobierania tokena z `AuthenticationStateProvider`.
- Funkcje zapisujące i usuwające członków grupy nadal używają klienta z tokenem i wymagają roli administratora.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wywołano produkcyjnie `GET http://japet:9900/Api/Grupa/SelectUzytkownikByGrupa/-1/1?usrStatus=*` - `200 OK`.
- Wywołano produkcyjnie `GET http://japet:9900/Api/Grupa/SelectUzytkownikByGrupa/-1/1?usrStatus=2` - `200 OK`.
- Wywołano produkcyjnie `GET http://japet:9900/Api/Grupa/SelectUzytkownikByGrupa/900/1?usrStatus=2` - `200 OK`.
## 2026-07-29

### Zgłoszenie

Anonimowe wywołanie `Uzytkownik/SelectByRola` zwracało błąd 500 przy pobieraniu listy użytkowników z roli.

### Zmienione obszary

- `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Pole `srlID` pozostawiono jako `string?` w modelach autoryzacji, zgodnie z typem zwracanym przez widok bazy.
- LoginController czyta srlID przez GetString, a BuildAdministrationUserQuery pobiera srlID bez dodatkowego rzutowania.
- `SelectByRola` przebudowano na jedno parametryzowane zapytanie SQL, bez pośredniego składania list ID i ponownego joinowania przez LINQ.
- Zapytanie korzysta z `mvw.vUzytkownikRolaLista`, `dbo.RolaCzlonkowie`, `dbo.Rola`, `dbo.Uzytkownik` oraz `mvw.vUzytkownik`.
- Dodano wartości domyślne dla pól wymaganych przez starsze klienty, gdy dane z widoku `mvw.vUzytkownik` nie zwrócą wiersza.
- Nie zmieniano modeli po stronie `atacamaDMS`, bo nadmiarowe pola JSON są ignorowane przez deserializację.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build .\indigoERP.Api\indigoERP.Api.csproj --configuration Release --no-restore -v:minimal` - kompilacja poprawna,
  416 istniejących ostrzeżeń, 0 błędów.
- Uruchomiono lokalnie świeży kod API na bazie `indigoERP` i wywołano
  `GET /Api/Uzytkownik/SelectByRola/HCZ-atacamaDMS-Zapotrzebowania-właściciel?usrStatus=2` - zwrócono `200 OK` oraz JSON.
## 2026-07-29

### Zgłoszenie

W kontrolerach administracyjnych `Uzytkownik`, `Rola` i `Grupa` wszystkie funkcje `Select*` mają być dostępne anonimowo.

### Zmienione obszary

- `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs`
- `indigoERP.Api/Controllers/Administrator/RolaController.cs`
- `indigoERP.Api/Controllers/Administrator/GrupaController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Akcje `Select*` w kontrolerach `Uzytkownik`, `Rola` i `Grupa` oznaczono `[AllowAnonymous]`.
- Akcje `Get`, `GetCzlonek`, `Detail` oraz funkcje zapisujące i usuwające pozostawiono z dotychczasową autoryzacją.
- Zapisy i usuwanie nadal wymagają roli `HCZ-indigoERP-Administrator`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build .\indigoERP.Api\indigoERP.Api.csproj --configuration Release --no-restore -v:minimal` - kompilacja poprawna,
  416 istniejących ostrzeżeń, 0 błędów.

## 2026-07-29

### Zgłoszenie

`atacamaDMS` przy wejściu na `Zapotrzebowanie/Detail` zgłaszał błąd pobrania listy użytkowników z indigoERP:
`Response status code does not indicate success: 404 (Not Found)`.

### Zmienione obszary

- `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Dodano brakujący endpoint `GET api/Uzytkownik/SelectByRola/{rola}?usrStatus=...`, używany przez `atacamaDMS`.
- Endpoint filtruje użytkowników po nazwie roli, uwzględnia role dziedziczone z grup oraz bezpośrednich członków roli.
- Autoryzację administracyjną przeniesiono z poziomu klasy na metody administracyjne, żeby nowy endpoint dopuścił także rolę
  `HCZ-atacamaDMS-Zapotrzebowania-administrator` bez otwierania pozostałych funkcji administracji użytkownikami.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build .\indigoERP.Api\indigoERP.Api.csproj -c Debug -p:UseAppHost=false -p:OutDir=D:\Temp\CodexBuild\indigoERP-SelectByRola\out\` -
  kompilacja poprawna, 0 błędów; pozostały istniejące ostrzeżenia nullable/XML.

## 2026-07-29

### Zgłoszenie

W kontrolerach administracyjnych `Uzytkownik`, `Rola` i `Grupa` funkcje pobierające dane mają wymagać autoryzacji, ale bez wymagania
roli administratora; funkcje zapisujące mają pozostać dostępne tylko dla roli administratora.

### Zmienione obszary

- `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs`
- `indigoERP.Api/Controllers/Administrator/RolaController.cs`
- `indigoERP.Api/Controllers/Administrator/GrupaController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Akcje `HttpGet` w kontrolerach `Uzytkownik`, `Rola` i `Grupa` wymagają poprawnego tokena Bearer bez sprawdzania roli.
- Akcje `HttpPost`, `HttpPut` i `HttpDelete` w tych kontrolerach nadal wymagają roli `HCZ-indigoERP-Administrator`.
- Użyto istniejącej roli administratora indigoERP stosowanej w projekcie: `HCZ-indigoERP-Administrator`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build .\indigoERP.Api\indigoERP.Api.csproj --configuration Release --no-restore -v:minimal` - kompilacja poprawna,
  416 istniejących ostrzeżeń, 0 błędów.

## 2026-07-29

### Zgłoszenie

`Rola.SelectUzytkownikByRola` ma wymagać autoryzacji, ale bez ograniczenia do roli `HCZ-indigoERP-Administrator`.

### Zmienione obszary

- `indigoERP.Api/Controllers/Administrator/RolaController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Usunięto rolę administratora z poziomu całego `RolaController`.
- Pozostałe akcje kontrolera nadal wymagają roli `HCZ-indigoERP-Administrator`.
- `SelectUzytkownikByRola` wymaga tylko poprawnego tokena Bearer.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build indigoERP.Api.csproj --configuration Release --no-restore -v:minimal`.

## 2026-07-28

### Zgłoszenie

Błąd dostępu do `AuthenticationStateProvider` przy wywołaniu administracyjnego endpointu `Grupa.SelectUzytkownikByGrupa`.

### Zmienione obszary

- `indigoERP.Blazor/Program.cs`
- `indigoERP.Blazor/Shared/Uzytkownik.razor`
- `indigoERP.Services/Extensions/HttpClientExtensions.cs`
- `indigoERP.Services/Services/Administrator/*.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- W Blazorze dodano `UseAuthentication()` i `UseAuthorization()` do pipeline przed mapowaniem huba i fallbacku.
- Pobieranie tokena Bearer z `AuthenticationStateProvider` przeniesiono na asynchroniczny helper bez blokującego `.Result`.
- Brak claima `accessToken` nie powoduje już wyjątku w helperze HTTP.
- Komponent `Shared/Uzytkownik.razor` odczytuje claimy bezpiecznie, bez `.FindFirst(...).Value`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build indigoERP.Blazor.csproj --configuration Release --no-restore -v:minimal`.

## 2026-07-25

### Zgłoszenie

Po publikacji produkcyjnej `indigoERP.Blazor` przestał działać, a log API pokazał błąd rzutowania wyniku `uspGetUzytkownik`
między `Guid` i `string`.

### Zmienione obszary

- `indigoERP.Api/Controllers/Autoryzacja/LoginController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Odczyt wyniku `uspGetUzytkownik` w `LoginController` przeniesiono z EF `FromSql` na `DbDataReader`.
- Mapowanie loginu obsługuje teraz `Guid`, tekst i `NULL` dla pól `usrGuid` i `sesGuid`, bez zmiany procedury używanej przez inne aplikacje.
- Kontrakt HTTP API pozostaje bez zmian dla Blazora i pozostałych systemów.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Sprawdzono logi produkcyjne Blazora i API.
- Wykonano `dotnet build indigoERP.Api.csproj --configuration Release --no-restore -v:minimal`.

## 2026-07-22

### Zgłoszenie

`mvw.vUzytkownik` i `UzytkownikViewModel` mają nowe pole `prcKod`, ale `uspGetUzytkownik` pozostaje bez zmian, bo korzystają z niej też inne systemy.

### Zmienione obszary

- `indigoERP.Domain/ViewModels/Autoryzacja/UzytkownikViewModel.cs`
- `indigoERP.Domain/ViewModels/Autoryzacja/UzytkownikLoginViewModel.cs`
- `indigoERP.Api/Data/indigoERPContext.cs`
- `indigoERP.Api/Controllers/Autoryzacja/LoginController.cs`
- `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- `UzytkownikViewModel` pozostaje mapowany do `mvw.vUzytkownik` i zawiera `prcKod`.
- Dodano `UzytkownikLoginViewModel` jako osobny `[Keyless]` model wyniku `uspGetUzytkownik` bez pola `prcKod`.
- `LoginController` mapuje wynik procedury z `UzytkownikLoginViewModel` na `UzytkownikViewModel` przed generowaniem tokena.
- Administracyjne zapytanie listy użytkowników zwraca `prcKod` w SQL używanym przez `FromSqlRaw`.

### Zmiany bazy danych

Brak zmian w kodzie bazy danych.

### Weryfikacja

- Sprawdzono, że `mvw.vUzytkownik` zwraca kolumnę `prcKod`, a `uspGetUzytkownik` jej nie zwraca.
- Wykonano `dotnet build indigoERP.Api.csproj --no-restore -v:minimal`.
## 2026-07-22

### Zgłoszenie

`UzytkownikViewModel` ma mieć mapowanie do `mvw.vUzytkownik`, a `Detail` ma pobierać rekord z widoku po kluczu `usrID + appID`.

### Zmienione obszary

- `indigoERP.Domain/ViewModels/Autoryzacja/UzytkownikViewModel.cs`
- `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Do `UzytkownikViewModel` dodano `[Table("vUzytkownik", Schema = "mvw")]`.
- Klucz modelu ustawiono jako `[PrimaryKey(nameof(usrID), nameof(appID))]`.
- Pola zwracane przez `uspGetUzytkownik`, ale nieistniejące w `mvw.vUzytkownik`, pozostają `[NotMapped]`.
- `Detail` pobiera rekord bezpośrednio z `_context.UzytkownikViewModel` po `appID` i `usrID`.
- `Select` nadal używa deduplikacji listy użytkowników, bo widok może zwracać więcej niż jeden rekord na użytkownika.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Sprawdzono występowanie duplikatów `usrID + appID` w `mvw.vUzytkownik` w bazie `indigoERP.Developer` na serwerze `JAPET`.
- Sprawdzono liczbę rekordów dla testowego `appID = 900` i `usrID = 3102`.
- Wykonano `dotnet build indigoERP.Api.csproj --no-restore -v:minimal`.
## 2026-07-22

### Zgłoszenie

Do endpointu `Uzytkownik.Detail` dodać parametr `appID`, aby szczegóły użytkownika były pobierane jednoznacznie dla aplikacji.

### Zmienione obszary

- `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Zmieniono trasę `Detail` na `GET api/Uzytkownik/Detail/{appID}/{usrID}`.
- `Detail` przekazuje `appID` do zapytania budującego `UzytkownikViewModel`.
- `BuildAdministrationUserQuery` filtruje `mvw.vUzytkownik` po `appID` przed deduplikacją po `usrID`.
- `Select` nadal korzysta z tego samego zapytania bez parametru `appID`, aby pobierać listę użytkowników administracji.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano SQL smoke test dla `appID = 900` i `usrID = 3102` w bazie `indigoERP.Developer` na serwerze `JAPET`.
- Wykonano `dotnet build indigoERP.Api.csproj --no-restore -v:minimal`.
## 2026-07-22

### Zgłoszenie

Usunąć klasę `UzytkownikAdministracjaViewModel` z części administracyjnej i użyć `UzytkownikViewModel`. Na stronie administracyjnej
wyświetlającej użytkowników dodać pola `Wydział` i `Stanowisko` z tego modelu.

### Zmienione obszary

- `indigoERP.Domain/ViewModels/Autoryzacja/UzytkownikViewModel.cs`
- `indigoERP.Domain/ViewModels/Administracja/UzytkownikAdministracjaViewModel.cs`
- `indigoERP.Api/Data/indigoERPContext.cs`
- `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs`
- `indigoERP.Services/Interfaces/Administrator/IUzytkownikService.cs`
- `indigoERP.Services/Services/Administrator/UzytkownikService.cs`
- `indigoERP.Blazor/Pages/Administrator/Uzytkownik/Index.razor`
- `indigoERP.Blazor/Pages/Administrator/Uzytkownik/Edit.razor`
- `indigoERP.Blazor/Pages/Administrator/Grupa/GrupaUzytkownicy.razor`
- `indigoERP.Blazor/Pages/Administrator/Rola/RolaUzytkownicy.razor`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Usunięto osobną klasę `UzytkownikAdministracjaViewModel`; administracja, serwis i kontroler używają teraz `UzytkownikViewModel`.
- `UzytkownikViewModel` pozostaje `[Keyless]`, zgodnie z użyciem w logowaniu przez `uspGetUzytkownik`.
- Pola administracyjne, których nie zwraca procedura logowania, oznaczono jako `[NotMapped]`.
- `UzytkownikController.Select` i `Detail` budują model z `dbo.Uzytkownik` oraz danych organizacyjnych z `mvw.vUzytkownik`.
- Grid użytkowników w administracji pokazuje dodatkowe kolumny `Wydział` (`wdzNazwa`) i `Stanowisko` (`stnNazwa`).
- Popup edycji aktualizuje `wdzNazwa` i `stnNazwa` po wyborze pracownika, aby odświeżony rekord miał pełny model.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Sprawdzono kolumny `mvw.vUzytkownik` w bazie `indigoERP.Developer` na serwerze `JAPET`.
- Sprawdzono nagłówki wyniku `dbo.uspGetUzytkownik` dla błędnego użytkownika testowego.
- Wykonano `dotnet build indigoERP.Api.csproj --no-restore -v:minimal`.
- Wykonano `dotnet build indigoERP.Service.csproj --no-restore -v:minimal -p:BuildProjectReferences=false`.
- Wykonano `dotnet build indigoERP.Blazor.csproj --no-restore -v:minimal -p:BuildProjectReferences=false`.
- Kompilacje zakończyły się bez błędów; pozostały istniejące ostrzeżenia projektu.

## 2026-07-22

### Zgłoszenie

Uruchomienie aplikacji zgłaszało `AggregateException` z komunikatem, że wymagana jest nowsza wersja pakietu `Aspire.Hosting.AppHost`, co najmniej `13.4.6`.

### Zmienione obszary

- `indigoERP.AppHost/indigoERP.AppHost.csproj`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Podniesiono SDK projektu AppHost z `Aspire.AppHost.Sdk/13.2.4` do `Aspire.AppHost.Sdk/13.4.6`.
- Wersja SDK jest teraz zgodna z używanym pakietem `Aspire.Hosting.Redis` w wersji `13.4.6`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet restore indigoERP.AppHost.csproj`.
- Wykonano `dotnet build indigoERP.AppHost.csproj --no-restore -v:minimal` do `.codex-build/apphost-aspire-13-4-6`.
- Kompilacja zakończyła się bez błędów; pozostały istniejące ostrzeżenia projektu.

## 2026-08-07

### Zgłoszenie

Dodanie dokumentu pracownika przez `Plik/Insert` zwracało pustą odpowiedź HTTP `500`.

### Zmienione obszary

- `indigoERP.Api/Controllers/Wspolne/PlikController.cs`
- `indigoERP.Api/Services/ApplicationFileLogger.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- `DbUpdateException` podczas zapisu pliku jest logowany do `Logs/application-errors-YYYY-MM.log` obok uruchomionego API.
- Endpoint zwraca `ProblemDetails` z komunikatem, zamiast pozostawić klientowi pustą odpowiedź `500`.

### Zmiany bazy danych

Brak.

## 2026-07-22

### Zgłoszenie

Do kontrolera `Uzytkownik` dodać funkcję `Detail`, gdzie parametrem jest `usrID` z trasy routingu, zwracającą klasę `UzytkownikViewModel`.

### Zmienione obszary

- `indigoERP.Api/Controllers/Administrator/UzytkownikController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Dodano endpoint `GET api/Uzytkownik/Detail/{usrID}`.
- Akcja pobiera rekord z `UzytkownikViewModel` po polu `usrID`.
- Gdy użytkownik nie istnieje, endpoint zwraca `404 NotFound`.
- Akcja `Detail` ma dostęp anonimowy przez `[AllowAnonymous]`, bez zdejmowania autoryzacji z pozostałych funkcji kontrolera.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build indigoERP.Api.csproj --no-restore -v:minimal` do `.codex-build/uzytkownik-detail-api`.
- Kompilacja zakończyła się bez błędów; pozostały istniejące ostrzeżenia projektu.

## 2026-07-21

### Zgłoszenie

Strona `Support` ma wysyłać wiadomość przez istniejące konto i serwer SMTP, ale jako nadawcę ma ustawiać adres email zalogowanego użytkownika.
Odbiorcą ma być `itsupport@hutaczestochowa.com.pl`, a treść maila ma być budowana z szablonu `wwwroot/support.html` przez podmianę `{{message}}`.

### Zmienione obszary

- `indigoERP.Blazor/Pages/Support.razor`
- `indigoERP.Blazor/wwwroot/support.html`
- `indigoERP.Domain/Entities/SupportMessage.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- `Support.razor` czyta szablon z `WebRootPath/support.html`, zamiast stałej ścieżki `d:\mail.html`.
- Odbiorca wiadomości jest wymuszony na `itsupport@hutaczestochowa.com.pl`.
- Nadawca wiadomości jest pobierany z claimu `usrEmail` zalogowanego użytkownika i ustawiany jako `MailMessage.From`.
- Do wysłania pozostawiono istniejący serwer `poczta.hcz.com.pl:25` oraz konto SMTP `support`.
- Treść wpisana przez użytkownika jest kodowana do HTML i podstawiana w miejsce `{{message}}`.
- Szablon `support.html` uporządkowano i zapisano z poprawnymi polskimi znakami oraz aktualnym adresem IT Support.
- Pole treści wiadomości na stronie `Support` powiększono do stałej wysokości `180px`.
- Na czas wysyłania wiadomości przycisk `Wyślij` zmienia ikonę na oczekiwanie, tekst na `Wysyłanie...` i blokuje ponowne kliknięcie.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Sprawdzono, że strona nie korzysta już ze ścieżki `d:\mail.html`.
- Sprawdzono zapis zmienionych plików jako UTF-8 BOM.
- Wykonano `dotnet build indigoERP.Blazor.csproj --no-restore -v:minimal`.
- Kompilacja zakończyła się bez błędów; pozostały istniejące ostrzeżenia projektu.
- Po powiększeniu pola wiadomości wykonano build do `.codex-build/support-textarea`, ponieważ zwykły `bin` był zablokowany przez uruchomioną aplikację.

## 2026-07-15

### Zgłoszenie

Po wdrożeniu nowej wersji na produkcji strona główna ładowała się do widoku błędu.
Log wskazywał `System.UriFormatException` przy tworzeniu `baseApiClient` w `Program.cs`.

### Zmienione obszary

- `indigoERP.Blazor/Program.cs`
- `indigoERP.Blazor/Pages/Home.razor.css`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Dodano funkcję odczytu i walidacji adresów API z konfiguracji.
- `BaseApiUrl` jest teraz normalizowany i walidowany przed utworzeniem `HttpClient`.
- Jeśli `BaseApiUrl` jest niepoprawny, `baseApiClient` używa poprawnego `IndigoApiUrl` jako wartości zapasowej.
- Dla adresów bez schematu dopisywany jest domyślnie `https://`.
- Jeśli nie da się zbudować poprawnego adresu HTTP/HTTPS, aplikacja zwraca czytelny błąd konfiguracji zamiast surowego `UriFormatException`.
- Uporządkowano położenie reguły CSS ikony kafla `IT Support`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build indigoERP.Blazor.csproj --no-restore`.
- Kompilacja zakończyła się bez błędów.
- Pozostały istniejące ostrzeżenia nullable w projekcie, niezwiązane z tą zmianą.
- Sprawdzono zapis zmienionych plików jako UTF-8 BOM.

## 2026-07-15

### Zgłoszenie

Część użytkowników po wdrożeniu nadal widziała stronę błędu na Home, mimo poprawienia adresów URL w konfiguracji produkcyjnej.

### Zmienione obszary

- `indigoERP.Api/Controllers/Administrator/AplikacjaController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Logi produkcyjne Blazora pokazały aktualny błąd `403 Forbidden` przy wywołaniu `Api/Aplikacja/Select` z Home.
- Przyczyną było objęcie całego `AplikacjaController` rolą `HCZ-indigoERP-Administrator`.
- Home nie ma mieć ograniczeń roli, a do startu potrzebuje publicznego odczytu listy aplikacji.
- Dodano `[AllowAnonymous]` wyłącznie do akcji `AplikacjaController.Select`.
- Pozostałe akcje kontrolera, w tym edycja, dodawanie i usuwanie, nadal dziedziczą autoryzację administratora z klasy kontrolera.

### Zmiany bazy danych

Brak zmian w kodzie aplikacji. Procedura `dbo.uspGetUzytkownik` została zweryfikowana po poprawce duplikatu kolumn `usrTelefon`/`usrKlientID`
w wyniku błędnego logowania.

### Weryfikacja

- Wykonano test produkcyjnego Home: odpowiedź HTTP `200`.
- Wykonano test produkcyjnego API dla błędnego logowania: odpowiedź bez wyjątku `usrTelefon`, rekord techniczny `usrID=-1`.
- W aktualnym logu Blazora potwierdzono błąd `403 Forbidden` na `Api/Aplikacja/Select`.
- Wykonano `dotnet build indigoERP.Api.csproj --no-restore`.
- Kompilacja zakończyła się bez błędów; pozostały istniejące ostrzeżenia projektu.

## 2026-07-15

### Zgłoszenie

Home ma mieć publiczną listę aplikacji tylko dla lewego panelu.
Administracyjne zarządzanie aplikacjami ma pozostać dostępne wyłącznie dla roli `HCZ-indigoERP-Administrator`.

### Zmienione obszary

- `indigoERP.Api/Controllers/Administrator/AplikacjaController.cs`
- `indigoERP.Api/Controllers/HomeController.cs`
- `indigoERP.Services/Interfaces/IHomeService.cs`
- `indigoERP.Services/Services/HomeService.cs`
- `indigoERP.Blazor/Pages/Home.razor`
- `indigoERP.Blazor/Program.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Wycofano publiczny dostęp z administracyjnej akcji `AplikacjaController.Select`.
- Dodano osobny publiczny endpoint `api/Home/SelectAplikacja` do pobierania aplikacji widocznych na stronie głównej.
- Endpoint Home filtruje tylko aktywne aplikacje o `appWyswietl = true`.
- Dodano `IHomeService` i `HomeService`, aby `Home.razor` nie korzystał z administracyjnego `IAplikacjaService`.
- Administracyjny grid aplikacji nadal korzysta z `AplikacjaController.Select` i wymaga roli `HCZ-indigoERP-Administrator`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Sprawdzono, że `Home.razor` używa `IHomeService.SelectAplikacja`.
- Sprawdzono, że `[AllowAnonymous]` jest tylko na `HomeController`, a nie na `AplikacjaController.Select`.
- Wykonano `dotnet build indigoERP.Api.csproj --no-restore`.
- Wykonano `dotnet build indigoERP.Blazor.csproj --no-restore`.
- Obie kompilacje zakończyły się bez błędów; pozostały istniejące ostrzeżenia projektu.

## 2026-07-15

### Zgłoszenie

Po wdrożeniu nowej wersji na produkcji strona główna zgłaszała błąd `404 Not Found` podczas wywołania `LoginService.Get` do endpointu `Login/Get`.

### Zmienione obszary

- `\\intranet.hcz.com.pl\c$\inetpub\wwwroot\indigoERP.Api\web.config`
- `\\intranet.hcz.com.pl\c$\inetpub\wwwroot\indigoERP.Api\appsettings.json`
- `\\intranet.hcz.com.pl\c$\inetpub\wwwroot\indigoERP.Api\logs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- W katalogu produkcyjnym `indigoERP.Api` brakowało `web.config`, więc IIS nie uruchamiał aplikacji ASP.NET Core jako API.
- Brakowało również `appsettings.json`, więc konfiguracja produkcyjna API nie była kompletna.
- Odtworzono `web.config` dla `AspNetCoreModuleV2` z uruchamianiem `dotnet .\indigoERP.Api.dll`.
- Odtworzono produkcyjne `appsettings.json` z połączeniem do `JAPET/indigoERP`, ustawieniami JWT i adresami `intranet.hcz.com.pl:9900`.
- Utworzono katalog `logs` dla logów stdout API.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `Api/Login/Get` na produkcji zwraca `200`.
- `Api/Home/SelectAplikacja` na produkcji zwraca `200`.
- Administracyjny `Api/Aplikacja/Select` bez tokena zwraca `401`, czyli pozostaje zabezpieczony.
- Strona główna `https://intranet.hcz.com.pl:8900/` zwraca `200`.
## 2026-07-25

### Zgłoszenie

Na produkcji `Api/Login/Get` zgłaszał `InvalidCastException: Unable to cast object of type 'System.String' to type 'System.Guid'`
podczas odczytu wyniku `uspGetUzytkownik`.

### Zmienione obszary

- `indigoERP.Domain/ViewModels/Autoryzacja/UzytkownikLoginViewModel.cs`
- `indigoERP.Api/Controllers/Autoryzacja/LoginController.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Nie zmieniano procedury `uspGetUzytkownik`, bo korzystają z niej różne systemy do autentykacji.
- W loginowym modelu pośrednim `UzytkownikLoginViewModel` pola `usrGuid` i `sesGuid` ustawiono jako `string?`, zgodnie z produkcyjnym wynikiem procedury.
- Przy mapowaniu do `UzytkownikViewModel` dodano bezpieczną konwersję `Guid.TryParse`.
- Wynik procedury jest materializowany przez `ToList()`, żeby nie iterować wyniku SQL wielokrotnie.
- Wywołanie procedury przełączono z `FromSqlRaw` na parametryzowane `FromSqlInterpolated` i dodano walidację liczbowego `appID`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build indigoERP.Api.csproj --no-restore -v:minimal`.
- Kompilacja zakończyła się bez błędów; pozostały istniejące ostrzeżenia projektu.

## 2026-10-13

### Zgłoszenie

Dodanie automatycznych testów API, serwisów i Blazora z aktualnymi wersjami bibliotek.

### Zmienione obszary

- `indigoERP.Api.Tests`
- `indigoERP.Services.Tests`
- `indigoERP.Blazor.Tests`
- `indigoERP.sln`
- `indigoERP.Api/Controllers/Autoryzacja/UzytkownikAplikacjaController.cs`
- `indigoERP.Api/Controllers/Slownik/MarkaPojazdController.cs`

### Decyzje techniczne

- Dodano trzy projekty testowe .NET 10 z `xunit` 2.9.3, `Microsoft.NET.Test.Sdk` 18.10.1,
  `coverlet.collector` 10.1.0, EF Core InMemory 10.0.12, Moq 4.21.0 i bUnit 2.11.3.
- API korzysta z izolowanego dostawcy EF Core InMemory i nie łączy się z żadną bazą SQL.
- Serwisy są testowane przez atrapę `HttpMessageHandler` i `IHttpClientFactory`, bez żądań sieciowych.
- Testy bUnit sprawdzają strony Home, Administracja, Support i Footer oraz kontrakt tras komponentów.
- Dodano kontrakty refleksyjne dla wszystkich kontrolerów i serwisów, aby pilnować tras, metod HTTP i interfejsów.
- Ujednolicono trasy `UzytkownikAplikacjaController` i `MarkaPojazdController` do `api/[controller]/[action]`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet restore indigoERP.sln --force-evaluate`.
- Wykonano `dotnet test indigoERP.sln --no-build --no-restore`.
- Wynik: API 9/9, serwisy 7/7, Blazor 6/6; łącznie 22 testy bez błędów i pominięć.
- Kompilacja zależnych projektów zakończyła się bez błędów; w istniejącym kodzie nadal występują wcześniejsze ostrzeżenia.

## 2026-10-16

### Zgłoszenie

Dodanie usługi `indigoERP.Worker`, która wysyła wiadomości zapisane w tabelach `dbo.Wiadomosc`,
`dbo.WiadomoscUzytkownik`, `dbo.WiadomoscDokument`, `dbo.WiadomoscWzorzec` i `dbo.WiadomoscWysylka` przez Microsoft Graph.

### Zmienione obszary

- `indigoERP.Worker`
- `indigoERP.Worker.Tests`
- `indigoERP.Domain/Entities/Wspolne/Wiadomosc.cs`
- `indigoERP.Domain/Dtos/Wspolne/WiadomoscDto.cs`
- `indigoERP.AppHost`
- `indigoERP.sln`

### Decyzje techniczne

- Worker pobiera wyłącznie zmienne środowiskowe `MicrosoftGraph_TenantId`, `MicrosoftGraph_ClientId` i
  `MicrosoftGraph_ClientSecret`; wartości nie są zapisywane w plikach konfiguracji ani logach.
- Połączenie SQL jest pobierane z `ConnectionStrings__indigoERPConnection`, a katalog załączników z
  `indigoERP_FileStorage_Path` albo `FileStorage__Path`.
- Do kolejki kwalifikują się rekordy z `wiaStatus = '2'` albo odzyskane rekordy `wiaStatus = '3'`, wyłącznie gdy
  mają `wiaWzorzecID`. Chroni to istniejące wiadomości portalu przed przypadkową wysyłką e-mail.
- Statusy wiadomości: `1` anulowana, `2` nowa, `3` rozpoczęta wysyłka, `4` zakończona wysyłka.
- Treść wzorca obsługuje znaczniki `{{message}}` i `{{subject}}`. Dla każdego odbiorcy Worker tworzy osobny wpis
  w `WiadomoscWysylka`; wpis `2` oznacza powodzenie, a `1` niepowodzenie. Ponowienie nie wysyła drugi raz na adres,
  dla którego istnieje już wpis powodzenia.
- Załączniki korzystają z aktualnego magazynu `FileStorage`; małe pliki są przesyłane bezpośrednio, a duże przez
  sesję uploadu Microsoft Graph.

### Zmiany bazy danych

- Kolumna `dbo.Wiadomosc.wiaWzorzecID` została dodana wcześniej w bazie przez użytkownika i została odwzorowana
  w encji oraz DTO. Worker nie wykonuje automatycznych migracji bazy.

### Weryfikacja

- Wykonano `dotnet test indigoERP.Worker.Tests\indigoERP.Worker.Tests.csproj`.
- Wykonano `dotnet build indigoERP.sln --configuration Debug --no-restore -v:minimal`: 0 błędów; 614 istniejących ostrzeżeń.
- Wykonano `dotnet test indigoERP.sln --no-restore`: 42/42 testów pomyślnie.

## 2026-10-16

### Zgłoszenie

Przygotowanie instalacji opublikowanego `indigoERP.Worker` jako usługi Windows.

### Zmienione obszary

- `indigoERP.Worker/appsettings.json`
- `indigoERP.Worker/Install-Worker.ps1`
- `indigoERP.Worker/indigoERP.Worker.csproj`

### Decyzje techniczne

- Worker pobiera połączenie SQL z `ConnectionStrings:indigoERPConnection`: `JAPET`, baza `indigo`, uwierzytelnianie Windows.
- Skrypt sprawdza obecność systemowych zmiennych `MicrosoftGraph_TenantId`, `MicrosoftGraph_ClientId` i
  `MicrosoftGraph_ClientSecret`, lecz nigdy nie zapisuje ani nie wyświetla ich wartości.
- `Install-Worker.ps1` jest publikowany razem z Workerem, instaluje lub aktualizuje usługę, ustawia automatyczny start,
  trzy próby ponownego uruchomienia oraz uprawnienie modyfikacji katalogu `logs` dla konta usługi.
- Hasło jest przekazywane bezpośrednio do `New-Service` tylko przy pierwszej instalacji. Aktualizacja istniejącej usługi
  zachowuje wcześniej ustawione konto logowania.
- Sprawdzenie wymaganych zmiennych środowiskowych zawsze zwraca tablicę, także gdy wszystkie wartości są dostępne.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Składnia `Install-Worker.ps1` i `appsettings.json` została sprawdzona lokalnie.
- `dotnet build indigoERP.Worker\indigoERP.Worker.csproj --configuration Release --no-restore -v:minimal`: 0 błędów, 0 ostrzeżeń.
- Testy obejmują składanie treści z wzorca, wywołania Microsoft Graph przez atrapę `HttpMessageHandler` i mapowanie
  `wiaWzorzecID` w `WiadomoscController`, bez połączenia z SQL Server ani Microsoft 365.

## 2026-10-16

### Zgłoszenie

Zapisywanie logów Worker do katalogu `logs`.

### Zmienione obszary

- `indigoERP.Worker/Program.cs`
- `indigoERP.Worker/Services/DailyFileLoggerProvider.cs`
- `indigoERP.Worker/appsettings.json`
- `indigoERP.Worker.Tests/DailyFileLoggerProviderTests.cs`

### Decyzje techniczne

- Worker zapisuje dzienny plik `logs\indigoERP.Worker-yyyyMMdd.log` względem katalogu wdrożenia.
- Logi obejmują wszystkie wpisy `ILogger` od poziomu `Information`, w tym wyjątki z pełnym stosem.
- Pliki starsze niż 30 dni są usuwane; okres i minimalny poziom można zmienić w `FileLogging`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `DailyFileLoggerProviderTests` potwierdza utworzenie i zawartość dziennego pliku w katalogu tymczasowym.
- `dotnet test indigoERP.sln --no-restore`: 43/43 testów pomyślnie.
## 2026-11-09 - Pierwszeństwo pracowników Huty w synchronizacji kont

- Zgłoszenie: aktywny angaż status 2 oraz pracownicy APT T; kolizje pomijać, P ma pierwszeństwo.
  `_SQL Queries/indigoERP.Api20261109130100.sql` dla JAPET.indigoERP rozszerza widoki i procedurę,
  po uprzednim wdrożeniu `epicaHR.Api20261109130000.sql` w JAPET.epicaHR. Skrypty niewdrożone, uruchamia użytkownik.
- Przy zderzeniu proponowanych nazw P/T wygrywa P, a T jest pomijany. Identyczne nazwy osób tego samego rodzaju
  lub istniejące konto są nadal pomijane; nie przepinamy kont. Zachowano blokowanie po zwolnieniu.
- Weryfikacja: 10/10 syntetycznych przypadków priorytetu w SELECT, test projektowanej procedury na tabelach
  tymczasowych `_Documentation/TestSynchronizacjaKontPriorytet_temp.sql`: 195 dodanych zablokowanych kont,
  19 kolizji, zero zmian przy drugim uruchomieniu, kontrola kont/grup/IDENTITY produkcji bez zmian.
  PARSEONLY obu definicji poprawny; build indigoERP: 0 błędów i 0 ostrzeżeń, epicaHR: 0 błędów i 1 ostrzeżenie
  ASPIRE010. Brak wdrożenia, testu triggera, SSO i faktycznych uprawnień konta usługi.

## 2026-11-09 - Przygotowanie pracowników T w synchronizacji kont

- Na prośbę użytkownika przygotowano w epicaHR skrypt `_SQL Queries/epicaHR.Api20261109120000.sql` dla JAPET.epicaHR,
  który rozszerza istniejący widok o pracowników tymczasowych T; skrypt niewdrożony, uruchamia użytkownik.
  Procedura indigoERP nie wymaga zmiany. Stare joby i dane produkcyjne nie zostały zmienione.
- Test `_Documentation/TestSynchronizacjaKontTypT_temp.sql`: projektowana definicja widoku i bieżąca procedura
  z podstawionymi tabelami tymczasowymi, 195 dodanych zablokowanych kont, 19 kolizji, 0 blokad/odblokowań,
  0 zmian przy drugim przebiegu, kontrola produkcji bez zmian. 19 kolizji obejmuje 14 T i 5 wcześniejszych P.
  Build indigoERP: 0 błędów, 821 ostrzeżeń; build epicaHR: 0 błędów, 760 ostrzeżeń.
- Sprawdzono 209 aktywnych T: obecny widok epicaHR wykorzystywany przez SSO nie zwraca żadnego z nich;
  `uspGetUzytkownikSso` może więc zwrócić `usrPracownikID=0`. To ograniczenie wymaga osobnej analizy przed
  potwierdzeniem pełnego logowania. Bez testu triggera lub SSO na produkcji.

## 2026-11-09 - Uruchomienie synchronizacji kont epicaHR

- Zgłoszenie: uruchomić synchronizację, administrator uzupełni konta AD. JAPET.indigoERP: wykonano istniejącą
  `dbo.aspSynchronizujKontaEpicaHR @Wykonaj=1` w zewnętrznej transakcji z walidacją wyniku, po podglądzie
  i wykonaniu kopii statusów. Nie zmieniano procedur, schematu, bazy źródłowej ani zadań SQL Agent.
- Zatwierdzono: 65 nowych zablokowanych kont bez hasła/AD, 11 blokad, 2 odblokowania, 65 wpisów do grupy 0.
  Powtórny podgląd: 5 kolizji i 7 pominiętych odblokowań, zero kolejnych dodawań/blokad/odblokowań.
  Liczba kont 1530 → 1595; stary job pozostał wyłączony. Dostępu SSO nie testowano.
- Materiały operacyjne: `_Documentation/SynchronizacjaKont/WykonanieSynchronizacji_20261109.sql`, kopia statusów
  `KopiaStatusow_20261109_przed.csv` i lista `NoweKontaDoUzupelnieniaAD_20261109.csv`; lista zawiera dane osobowe
  i nie powinna być zatwierdzana w repozytorium. Brak nowego skryptu migracji: nie zmieniano obiektów SQL.

## 2026-11-09 - Izolowany test synchronizacji kont epicaHR

- Zgłoszenie: przetestować wdrożoną synchronizację. `_Documentation/TestSynchronizacjaKont_temp.sql` uruchamia
  aktualną definicję procedury z podmianą źródła/celu na lokalne tabele tymczasowe oraz osobnym zasobem applock.
  Nie zmieniano obiektów bazy ani produkcyjnych kont; unika to również zużycia produkcyjnych wartości IDENTITY.
- Wynik: podgląd i zapis w kopii 65 DODAJ, 11 BLOKUJ, 2 ODBLOKUJ, 5 POMIN_KOLIZJA, 2 POMIN_ODBLOKOWANIE;
  65 nowych jest zablokowanych bez AD/hasła, zweryfikowano 11 blokad i 2 odblokowania. Powtórny zapis 0 zmian;
  pominięte odblokowania wzrosły do 7 przez 5 nowo dodanych kont osób ponownie zatrudnionych.
- Kontrola przed/po: liczba kont, suma kontrolna wybranych pól, liczba członkostw i IDENTITY bez zmian.
  Trigger INSERT i SSO nie zostały wykonane w teście; trigger zbadano tylko przez definicję i stan włączenia.
  Nie zbadano wszystkich pól ani wykonania z rzeczywistym kontem usługi. Brak zmian SQL do wdrożenia.

## 2026-11-06 - Analiza wdrożonej synchronizacji kont epicaHR

- Potwierdzono na JAPET widoki epicaHR.sha i indigoERP.rdb/mvw oraz procedurę `dbo.aspSynchronizujKontaEpicaHR`.
  Wykonano tylko podgląd `@Wykonaj=0`: 65 DODAJ, 11 BLOKUJ, 2 ODBLOKUJ, 5 POMIN_KOLIZJA i 2 POMIN_ODBLOKOWANIE.
  Kolizje: 2 istniejące konta bez ID pracownika, 3 już powiązane; 5/5 zgodnych imion i nazwisk po normalizacji,
  co nie potwierdza tożsamości. Pominięte odblokowania: data zmiany konta
  w dniu angażu lub po nim. Dane osobowe ani konta nie były zmieniane; tryb zapisu nie był testowany.
- Widoczny stary job `ujbIndigoErpMergeUzytkownik` jest wyłączony; nowego joba nie odnaleziono. Trigger INSERT
  użytkownika jest aktywny i dodaje grupę 0, jedyną aktywną oznaczoną jako wszyscy. Ryzyko: brak informacji
  o przyczynie blokady, 2 kandydatów do odblokowania wymaga indywidualnego sprawdzenia.
- Widok epicaHR jest zgodny ze skryptem. Definicja procedury różni się od pliku formatowaniem i blokami BEGIN/END;
  istotne warunki i operacje przejrzano, ale pełna równoważność nie została formalnie dowiedziona.

## 2026-11-05 - Projekt synchronizacji kont pracowników z epicaHR

- Przygotowano `_SQL Queries/indigoERP.Api20261105110100.sql` dla JAPET.indigoERP: warstwa rdb/mvw i szkic procedury
  `dbo.aspSynchronizujKontaEpicaHR`; najpierw wymagany skrypt epicaHR `_SQL Queries/epicaHR.Api20261105110000.sql`.
  Skrypty nie są wdrożone. Domyślny tryb to podgląd; zapis wymaga jawnego `@Wykonaj=1` i nie był testowany.
- Szkic nie korzysta z huta ani inout, nie zmienia `aspMergeUzytkownik`, danych, zadań ani harmonogramu.
  Nowe konta są blokowane do weryfikacji i nie dostają nazwy AD ani hasła. Użytkownik potwierdził ręczne uzupełnianie
  AD i automatyczne odblokowanie po ponownym zatrudnieniu. Reguła wymaga starszej modyfikacji konta, statusu 1,
  blokady i gotowych danych logowania; brak informacji o przyczynie blokady ogranicza jej pewność. E-mail pozostaje.
- Weryfikacja tylko do odczytu: statusy 2/5/9 i definicja aktywnego widoku; PARSEONLY obu skryptów, TOP 0
  sześć przypadków klasyfikacji i sześć przypadków kolizji/idempotencji. W podglądzie 65 kandydatów do dodania,
  5 kolizji i 11 kont do blokady; liczby są chwilowe i nie stanowią wykonania. Build: 0 błędów i 662 ostrzeżenia.
  Osobno 2 kandydatów do odblokowania, 7/7 scenariuszy reguły odblokowania w SELECT. Ponowny przyrostowy build:
  0 błędów i 0 ostrzeżeń; brak testu utworzenia obiektów i uruchomienia procedury.



## 2026-11-10 — synchronizacja statusów P i T oraz dane APT

- Przygotowano `_SQL Queries/indigoERP.Api20261110113000.sql` dla JAPET.indigoERP: aktywny angaż ustawia `usrBlokada=0`, `usrStatus=2`, brak aktywnego `usrBlokada=1`, `usrStatus=1`; dotyczy typów P i T. Nowe konta aktywne bez AD, kolizje pomijane z priorytetem P. Aktywność również znosi ręczne blokady.
- Przygotowano `_SQL Queries/indigoERP.Api20261110113200.sql` dla JAPET.indigoERP, po wdrożeniu skryptu epicaHR: `rdb.vPracownikLista` obejmuje bieżący angaż T; prezentuje rzeczywisty wydział i nazwę stanowiska z dopiskiem ` (APT)`, również dla SSO. Identyfikatory pozostają rzeczywiste.
- Status wdrożenia: oba skrypty przygotowane, niewdrożone; synchronizacji nie uruchomiono. Weryfikacja na tymczasowej kopii danych: podgląd bez zapisu, statusy P/T, nowe konta, powtórzenie bez zmian i brak zmian tabeli produkcyjnej. W źródle 195/195 aktywnych kont APT miało wydział i stanowisko.

- Build rozwiązań: indigoERP 0 błędów, 821 ostrzeżeń; epicaHR 0 błędów, 1115 ostrzeżeń. Zapytanie odczytowe potwierdziło sufiks stanowiska dla 209/209 bieżących angaży T. Pełnego zestawu testów aplikacji ani rzeczywistego przepływu SSO nie uruchomiono.
