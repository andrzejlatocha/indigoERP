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

- Wykonano `dotnet build .\indigoERP.Api\indigoERP.Api.csproj --configuration Release --no-restore -v:q` - kompilacja poprawna,
  596 istniejących ostrzeżeń, 0 błędów.
