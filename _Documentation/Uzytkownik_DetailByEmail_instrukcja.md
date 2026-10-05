# Uzytkownik/DetailByEmail - instrukcja wywołania

Endpoint zwraca dane użytkownika indigoERP na podstawie adresu e-mail. Bez logowania (bez Bearer), zabezpieczony kluczem API.

## 1. Przygotowanie (jednorazowo)

1. Opublikować `indigoERP.Api` na DEIMOS (akcja jest nowa).
2. Na serwerze ustawić zmienną systemową `indigoERP_ApiKey` (ta sama co dla `Podpis/PreviewByEmail`) i zrestartować pulę aplikacji Api.
   Lokalnie (Development) klucz jest w `indigoERP.Api/appsettings.json` -> `appSettings:ApiKey`.
3. Ustalić adres Api:
   - lokalnie: `https://localhost:9900/` (profil `https` w `indigoERP.Api`),
   - produkcja z zewnątrz: `https://app.hcz.com.pl/indigoERP/` (przez authorizationSRV.Gateway, trasa `/indigoERP/Api/Uzytkownik/DetailByEmail`, limit 20 żądań/min na IP) - pełny adres: `https://app.hcz.com.pl/indigoERP/Api/Uzytkownik/DetailByEmail?usrEmail=...`,
   - produkcja w sieci: bezpośrednio `https://app.hcz.com.pl:9900/api/Uzytkownik/DetailByEmail` (indigoERP.Api na DEIMOS; instancja na JAPET obsługuje tylko stare aplikacje).

## 2. Wywołanie

```
GET <adres-api>/api/Uzytkownik/DetailByEmail?usrEmail=<adres e-mail>
Nagłówek: X-Api-Key: <wartość indigoERP_ApiKey>
```

Adres e-mail w query string zapisujemy z kodowaniem URL (`@` -> `%40`), narzędzia (curl `--data-urlencode`, Invoke-RestMethod) zrobią to same.

### curl

```bash
curl -G "https://app.hcz.com.pl/indigoERP/api/Uzytkownik/DetailByEmail" --data-urlencode "usrEmail=jan.kowalski@huta-czestochowa.pl" -H "X-Api-Key: <klucz>"
```

### PowerShell

```powershell
Invoke-RestMethod -Uri "https://app.hcz.com.pl/indigoERP/api/Uzytkownik/DetailByEmail?usrEmail=jan.kowalski%40huta-czestochowa.pl" -Headers @{ 'X-Api-Key' = '<klucz>' }
```

### C# (HttpClient)

```csharp
using var request = new HttpRequestMessage(HttpMethod.Get, $"api/Uzytkownik/DetailByEmail?usrEmail={Uri.EscapeDataString(email)}");
request.Headers.Add("X-Api-Key", apiKey);
var response = await http.SendAsync(request);
if (response.IsSuccessStatusCode)
{
    var uzytkownik = await response.Content.ReadFromJsonAsync<UzytkownikViewModel>();
}
```

## 3. Odpowiedź

| Kod | Znaczenie |
|-----|-----------|
| 200 | `UzytkownikViewModel` (JSON): `usrID`, `usrNazwa`, `usrNazwaAD`, `usrImie`, `usrNazwisko`, `usrImieNazwisko`, `usrEmail`, `usrTelefon`, `usrKomorka`, `usrBlokada`, `usrStatus`, `usrZablokowanyLogowaniem` i pozostałe pola widoku `mvw.vUzytkownikLista` |
| 401 | brak nagłówka `X-Api-Key`, zły klucz albo klucz nieustawiony na serwerze |
| 400 | pusty parametr `usrEmail` |
| 404 | brak użytkownika o takim adresie |

Gdy ten sam adres ma kilku użytkowników, zwracany jest ten z najmniejszym `usrID`.

## 4. Diagnostyka

- 401 mimo poprawnego klucza: sprawdzić w logu startowym Api linię `appSettings:ApiKey ustawiony=...` - zmienna `indigoERP_ApiKey`
  musi być widoczna dla procesu puli aplikacji (po zmianie zmiennej systemowej restart puli/IIS).
- 404 dla istniejącego użytkownika: adres w `mvw.vUzytkownikLista.usrEmail` musi się zgadzać (porównanie bez rozróżniania wielkości liter zależy od collation bazy).
- Endpoint zwraca dane osobowe - klucz traktować jak hasło, nie umieszczać go w kodzie po stronie przeglądarki.
