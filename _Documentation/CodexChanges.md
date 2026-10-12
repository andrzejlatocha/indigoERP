# Codex Changes

## 2026-10-07 - Animacja kropek (ThinkingDots) zamiast dotychczasowego loadera

- **Zgłoszenie:** ujednolicić animację ładowania we wszystkich aplikacjach (wzorzec: epicaHR).
- **Zmiana:** nowy komponent `ThinkingDots.razor` (+ `.razor.css`, parametr `Size`: `Small` / `Medium` / `Large` / `ExtraLarge`) obok `Load.razor` i `LoadSmall.razor`; `Load` pokazuje rozmiar `ExtraLarge`, `LoadSmall` rozmiar `Medium`. Strony używające `<Load />` / `<LoadSmall />` bez zmian. Dotychczasowe obrazki GIF / kółka zębate zostają w projekcie (nieużywane).
- **Status wdrożenia:** niewdrożone, niezacommitowane.
- **Weryfikacja:** patrz wpis w epicaHR (build 0 błędów); wygląd w przeglądarce nie był sprawdzany.

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
