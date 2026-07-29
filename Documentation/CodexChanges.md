# Codex Changes
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
