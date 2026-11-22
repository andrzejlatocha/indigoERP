# ToDo

## Do zrobienia

- [ ] **Funkcje z przeglądu starej atamy do dodania w indigoERP** (decyzja 2026-11-09)
  - forum (dyskusje),
  - kalendarz,
  - zadania (lista zadań z terminami),
  - ogłoszenia (komunikaty dla firmy),
  - kontakty (książka adresowa),
  - komentarze do treści.
  - Zakres i kolejność do ustalenia; przeniesienie z atacamaDMS (stara atama) jest pomijane w tej aplikacji.

- [ ] **Wspólny interfejs wiadomości dla indigoERP, kaizenITSM i epicaHR** (2026-11-19)
  - Docelowo wariant 1 (decyzja 2026-11-20): biblioteka komponentów Razor `indigoERP.Wiadomosci` z kontraktem `IWiadomosciService`; host dostarcza własną implementację i token. kaizenITSM zamiast `IndigoWiadomoscApiClient` używa komponentów.
  - Wariant 2: interfejs tylko w indigoERP.Blazor, pozostałe aplikacje linkują lub osadzają `/wiadomosc` (sprawdzić SSO w iframe).
  - Wariant 3: zostawić jak jest.
  - Ograniczenie: kaizenITSM i epicaHR są w osobnych repozytoriach; przy wariancie 1 potrzebny kanał pakietów NuGet albo submoduł.
  - Otwarte: czy strona Wiadomosc w epicaHR.Portal (rozmowa z HR) ma korzystać z tego samego UI.

- [ ] **Pusta strona po kliknięciu kafelka aplikacji po ~30 min bezczynności** (DEIMOS, zgłoszenie 2026-11-20)
  - Objaw: otwarta karta indigoERP; po kliknięciu kafelka adres się zmienia, strona się nie ładuje (pusta). Zgłoszone ok. 11:00.
  - Ustalenia z logów IIS (W3SVC1): kafelek otwiera inną aplikację Blazor; żądanie `/<app>/_blazor` dostaje 401 z podkodem 2, potem `/<app>/Error404`. Wzorzec u kilku klientów 10:40–11:13 (m.in. atacamaDMS, menageKSeF, kendoCMMS).
  - `applicationHost.config`: „Default Web Site” ma anonimowe logowanie wyłączone i Windows włączone; nadpisanie anonimowe jest tylko dla cerberTMS i kaizenITSM.
  - Ponowne połączenie SignalR w indigoERP: `UserNameChangedRejected` (nowe połączenie bez tożsamości Windows).
  - Token JWT (60 min) wykluczony jako główna przyczyna: brak 401 z API, każde wejście dostaje nowy token.
  - Do zrobienia: podać IP klienta i nazwę kafelka z 11:00; sprawdzić ustawienia IIS dla ścieżek aplikacji; test na CERES przed jakąkolwiek zmianą na DEIMOS; rozważyć logowanie w `ClaimsTransformation` i obsługę wyjątku `GetSso` (CLAUDE.md: błąd gateway nie może przerywać potoku).
  - Dodatkowo w logu z 2026-11-20 (do sprawdzenia osobno): 503 „Brak tokenu do atacamaDMS.Api” z PrzepisyWewnetrzne; 500 z AplikacjaUslugaUprawnienie/Select (10:45, 10:56); NullReferenceException w SignalR OnDisconnectedAsync (10:51); `GetSso` wywoływany ok. 2670 razy dziennie (brak cache).

## Zrobione
