# Synchronizacja kont pracowników epicaHR

## Status 2026-11-05

Aktualizacja 2026-11-06: widoki i procedura zostały wdrożone na JAPET, co sprawdzono w metadanych.
Wywołanie `@Wykonaj=0` wykonało podgląd bez zapisu kont. Tryb zapisu wymaga jawnego `@Wykonaj=1`;
nie został wywołany ani sprawdzony podczas analizy.

## Zakres i źródło

Źródło `epicaHR.sha.vPracownikKontoSynchronizacja` utworzono osobnym skryptem epicaHR. Warstwa docelowa
`indigoERP.rdb.vPracownikKontoSynchronizacja` i `indigoERP.mvw.vPracownikKontoSynchronizacja` służy lokalnej
procedurze `dbo.aspSynchronizujKontaEpicaHR`. Dotyczy osób o typie umowy P; nie wywołuje procedur baz huta ani
inout oraz nie modyfikuje istniejącej procedury `dbo.aspMergeUzytkownik` ani zadań SQL Agent.

- Aktywność: nieusunięty angaż o statusie 2, rozpoczęty i bez minionej daty zakończenia.
- Zwolnienie: istnieje nieusunięty angaż o statusie 5 i dacie zakończenia przed bieżącym dniem; nie istnieje
  inny angaż o statusie aktywnym 2 lub oczekującym 9. Stan niejednoznaczny nie powoduje blokady.
- Identyfikator dopasowania to `prcID = usrPracownikID`. Konta o innych typach, bez powiązania oraz nieobecność
  w źródle nie powodują blokady. Nie zakłada się, że brak zdjęcia lub stanowiska oznacza zwolnienie.
- Podgląd pokazuje dodanie, blokadę, brak danych i kolizje nazw. Zapis dodaje konto z nazwą `imię.nazwisko`,
  bez hasła i nazwy AD, z `usrStatus='1'`, `usrBlokada=1` i `usrSynchronizacjaAD=1`; rzeczywisty login AD,
  tożsamość i role wymagają weryfikacji przed aktywacją. Nazwę AD uzupełnia administrator.
- Przy ponownym zatrudnieniu procedura odblokowuje konto oznaczone do synchronizacji, wcześniej zablokowane
  (`usrStatus=1`, `usrBlokada=1`), gdy aktualny angaż rozpoczął się po dacie wcześniejszego zwolnienia, a ostatnia
  zmiana konta jest sprzed dnia nowego angażu. Konto musi mieć nazwę AD lub hasło; nowe nieprzygotowane konto
  nie jest automatycznie aktywowane. Przypadki niespełniające warunków są wykazywane jako `POMIN_ODBLOKOWANIE`.
  Samego powodu blokady dane konta nie przechowują: warunek daty nie wyklucza wcześniejszej blokady ręcznej.
  Zmiana konta w dniu rozpoczęcia angażu jest pomijana do weryfikacji. Procedura nie czyści e-maila.

## Weryfikacja i zależności

Na JAPET 2026-11-05 wyłącznie przez SELECT: 65 kandydatów do dodania, 5 kolizji i 11 kont do blokady według
projektowanej reguły. Dane zmieniają się; należy wykonać bieżący podgląd przed jakimkolwiek zapisem.
Osobny podgląd reguły ponownego zatrudnienia wykazał 2 kandydatów do odblokowania, przed weryfikacją osób.
PARSEONLY nie jest testem kompilacji ani wykonania procedury; sześć syntetycznych przypadków zapytania
klasyfikującego zakończyło się wynikiem zgodnym z założeniami. Na 2026-11-06 podgląd wdrożonej procedury:
65 DODAJ, 11 BLOKUJ, 2 ODBLOKUJ, 5 POMIN_KOLIZJA i 2 POMIN_ODBLOKOWANIE.
Każda z 5 kolizji dotyczy istniejącego konta: dwa bez identyfikatora pracownika, trzy przypisane do innego.
We wszystkich pięciu przypadkach imię i nazwisko są zgodne po normalizacji; nie potwierdza to tożsamości ani
poprawności istniejącego powiązania. Nie łączyć ani nie przepinać kont bez indywidualnej weryfikacji.
Oba pominięte odblokowania wynikają z daty modyfikacji konta w dniu nowego angażu albo później.
Źródło zawiera 1221 aktywnych, bez duplikatów ID. Poprawność decyzji dla poszczególnych osób pozostaje do weryfikacji.

Należy sprawdzić kolejność wdrożenia, uprawnienia między bazami, wpływ triggera `utrAiUzytkownik` oraz nadawanie
grup i działanie authorizationSRV. Trigger `utrAiUzytkownik` jest włączony i dopisuje grupę 0; grupa 0 jest jedyną
aktywną grupą oznaczoną jako „wszyscy”. Pełne uprawnienia nowych użytkowników nie były weryfikowane. Stary job
`ujbIndigoErpMergeUzytkownik` ma `enabled=0`; nie znaleziono widocznego joba dla nowej procedury.
Ręczne lub równoległe uruchomienie starej procedury może nadpisać decyzje nowej. Przed odblokowaniem sprawdzić
przyczynę blokady: DataModyfikacja nie jest dowodem, że konto zablokowano tylko w związku ze zwolnieniem.
