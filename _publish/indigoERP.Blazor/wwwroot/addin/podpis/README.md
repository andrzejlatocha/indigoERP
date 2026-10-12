# Dodatek Outlook "Podpisy indigoERP"

Etap 2 modułu Podpisy — automatyczne wstawianie podpisu w Outlooku, bez ręcznego kopiowania z `/podpis/podglad`.

## Zanim wdrożysz

1. **Podmień placeholdery** w `config.js` i `manifest.xml`:
   - `ADDIN_BASE_URL` — publiczny, **HTTPS** adres, pod którym te pliki będą dostępne (np. `https://indigoerp.huta-czestochowa.pl/addin/podpis/`). Uwaga: `http://japet:9900` (obecny `IndigoApiUrl` w dev) **nie zadziała** w produkcji — Office wymaga HTTPS dla zasobów dodatku (poza sideloadem lokalnym).
   - `ADDIN_API_KEY` — wartość **inna niż** produkcyjny `appSettings:ApiKey` z `appsettings.json` API. Ten klucz trafi do publicznie pobieralnego pliku JS (widoczny dla każdego w devtools), więc celowo nie wpisałem tu żadnej realnej wartości — wygeneruj osobny klucz tylko do tego celu i ustaw go po obu stronach (tu + w konfiguracji API, patrz niżej).
2. Wygeneruj prawdziwy **GUID** dla `<Id>` w `manifest.xml` (obecny to placeholder) — użyj `[guid]::NewGuid()` w PowerShell.
3. Zweryfikuj `manifest.xml` walidatorem Microsoftu przed wdrożeniem (`npx office-addin-manifest validate manifest.xml` albo https://config.office.com validator) — składnia event-based activation bywa zmieniana przez Microsoft między wersjami schematu, a nie miałem możliwości uruchomić żywego walidatora w tym środowisku.
4. Ikony w `icons/` to tymczasowe, wygenerowane programowo placeholdery (litera "P" na niebieskim tle) — podmień na docelowe grafiki firmowe przed produkcyjnym wdrożeniem.

## Wdrożenie

Microsoft 365 admin center → Settings → Integrated apps → Upload custom app → wskaż `manifest.xml` (dostępny pod publicznym URL-em, nie z dysku). Najpierw wdróż na grupę testową.

## Jak to działa

- `commands.js` — uruchamia się w tle przy starcie nowej wiadomości/odpowiedzi/przekazania (`LaunchEvent`), pobiera podpis z `GET Api/Podpis/PreviewByEmail?email=...` i wstawia go przez `Office.context.mailbox.item.body.setSignatureAsync` (nadpisuje wydzielony region podpisu, nie dokleja kolejnego przy odpowiedziach).
- `taskpane.html`/`taskpane.js` — przycisk "Wstaw / odśwież podpis" w task pane, ta sama logika wywoływana ręcznie (fallback, gdy auto-insert nie zadziała, albo do ręcznego odświeżenia po edycji szablonu).
