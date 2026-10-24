# Codex Changes

## 2026-08-07

### Zgłoszenie

`PreparePlikData` w `PlikController` nie powinien ustawiać pól audytowych, bo `Plik` dziedziczy po `AuditableEntity`.

### Zmienione obszary

- `indigoERP.Api/Controllers/Wspolne/PlikController.cs`
- `indigoERP.Api/Data/indigoERPContext.cs`
- `indigoERP.Api/Program.cs`
- `indigoERP.Domain/Dtos/Wspolne/PlikDto.cs`
- `indigoERP.Domain/Entities/Wspolne/Plik.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- `PreparePlikData` zeruje tylko `plkID` przy dodawaniu.
- Domyślne `plkStatus = "2"` przeniesiono do konstruktorów `Plik` i `PlikDto`, żeby kontroler nie ustawiał statusu ręcznie.
- Pola `DataUtworzenie`, `DataModyfikacja`, `UtworzenieID`, `ModyfikacjaID` i `Usuniety` są ustawiane globalnie w `indigoERPContext`.
- `indigoERPContext` pobiera identyfikator użytkownika z claimu `usrID`, a gdy go nie ma, próbuje użyć `ClaimTypes.NameIdentifier`.
- W `Program.cs` dodano `AddHttpContextAccessor`, żeby kontekst miał dostęp do użytkownika aktualnego requestu.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Wykonano `dotnet build .\indigoERP.Api\indigoERP.Api.csproj --configuration Release --no-restore -v:q` - kompilacja poprawna,
  597 istniejących ostrzeżeń, 0 błędów.
## 2026-08-07

### Zgłoszenie

W debuggerze `indigoERP.Api` występował błąd zapisu encji w kontrolerze `Plik`.

### Zmienione obszary

- `indigoERP.Api/Controllers/Wspolne/PlikController.cs`
- `indigoERP.Domain/Dtos/Wspolne/PlikDto.cs`
- `Documentation/CodexChanges.md`

### Decyzje techniczne

- Dodano `PlikDto` i zmieniono `PlikController`, żeby operacje `Get`, `Insert` i `Update` używały DTO zamiast zapisu encji z requestu.
- `Update` pobiera istniejący rekord z bazy i mapuje DTO na encję śledzoną przez EF.
- `Insert` zeruje `plkID`, żeby nie próbować zapisać wartości do kolumny identity.
- Stare pola audytu `plkDataUtworzenie`, `plkUtworzenieUzytkownikID`, `plkDataModyfikacja` i `plkModyfikacjaUzytkownikID`
  zostały później usunięte z obsługi, bo `Plik` dziedziczy po `AuditableEntity`.
- Pusty `plkStatus` jest ustawiany na `2`.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `dotnet test indigoERP.sln --no-restore`: 43/43 testów pomyślnie.

## 2026-10-16

### Zgłoszenie

API dla tabeli `dbo.WiadomoscPracownik`.

### Zmienione obszary

- encja i DTO `WiadomoscPracownik`
- DbSet w `indigoERPContext`
- `WiadomoscPracownikController`
- testy CRUD kontrolera

### Decyzje techniczne

- Udostępniono uwierzytelnione Bearer CRUD oraz listowanie po statusie i po identyfikatorze wiadomości.
- Walidacja e-maila i długości pól pozostaje w DTO; `rowguid` nie jest mapowany.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Testy API: 21/21 pomyślnie.

## 2026-10-16

### Zgłoszenie

Instalacja `indigoERP.Worker` jako usługi Windows.

### Zmienione obszary

- `indigoERP.Worker/appsettings.json`
- `indigoERP.Worker/Install-Worker.ps1`
- `indigoERP.Worker/indigoERP.Worker.csproj`

### Decyzje techniczne

- Konfiguracja SQL używa `ConnectionStrings:indigoERPConnection` z uwierzytelnianiem Windows dla `JAPET` i bazy `indigo`.
- Skrypt instalacyjny wymaga administratora, kontroluje trzy wymagane zmienne Graph, konfiguruje usługę i katalog logów.
- Przy pierwszej instalacji konto usługi jest ustawiane przez `New-Service`; aktualizacja zachowuje obecne konto logowania.
- Kontrola zmiennych Graph obsługuje poprawnie również przypadek, gdy żadnej wartości nie brakuje.

### Zmiany bazy danych

Brak.

### Weryfikacja

- Składnia skryptu PowerShell i pliku JSON została zweryfikowana lokalnie.
- Projekt `indigoERP.Worker` kompiluje się w konfiguracji Release bez błędów i ostrzeżeń.

### Weryfikacja

- Wykonano `dotnet build .\indigoERP.Api\indigoERP.Api.csproj --configuration Release --no-restore -v:q` - kompilacja poprawna,
  596 istniejących ostrzeżeń, 0 błędów.

## 2026-10-16

### Zgłoszenie

Dodanie usługi `indigoERP.Worker` do wysyłania kolejki wiadomości przez Microsoft Graph.

### Zmienione obszary

- `indigoERP.Worker` i `indigoERP.Worker.Tests`
- model oraz DTO `Wiadomosc`
- `indigoERP.AppHost` i `indigoERP.sln`

### Decyzje techniczne

- Sekrety Graph są odczytywane wyłącznie ze zmiennych `MicrosoftGraph_TenantId`, `MicrosoftGraph_ClientId` i
  `MicrosoftGraph_ClientSecret`.
- Worker wysyła tylko wiadomości z `wiaWzorzecID`, przez co nie przejmuje dotychczasowych wiadomości portalu.
- Statusy `wiaStatus`: `1` anulowana, `2` nowa, `3` rozpoczęta, `4` zakończona.

### Zmiany bazy danych

- Dodaną przez użytkownika kolumnę `wiaWzorzecID` odwzorowano w modelu i DTO.

### Weryfikacja

- `dotnet build indigoERP.sln --configuration Debug --no-restore -v:minimal`: 0 błędów, 614 istniejących ostrzeżeń.
- `dotnet test indigoERP.sln --no-restore`: 42/42 pomyślnie.

## 2026-10-16

### Zgłoszenie

Zapisywanie logów Worker do katalogu `logs`.

### Zmienione obszary

- provider `DailyFileLoggerProvider`
- konfiguracja `FileLogging` Worker

### Decyzje techniczne

- Dzienny plik ma nazwę `logs\indigoERP.Worker-yyyyMMdd.log` i obejmuje wpisy od poziomu `Information`.
- Worker usuwa pliki logów starsze niż 30 dni.

### Zmiany bazy danych

Brak.

### Weryfikacja

- `dotnet test indigoERP.sln --no-restore`: 43/43 testów pomyślnie.
