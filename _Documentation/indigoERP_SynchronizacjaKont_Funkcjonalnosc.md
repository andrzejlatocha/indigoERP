# Synchronizacja kont pracowników epicaHR

## Status 2026-11-05

Aktualizacja 2026-11-06: widoki i procedura zostały wdrożone na JAPET, co sprawdzono w metadanych.
Wywołanie `@Wykonaj=0` wykonało podgląd bez zapisu kont. Tryb zapisu wymaga jawnego `@Wykonaj=1`;
9.11.2026 przetestowano go wyłącznie na kopii w tymczasowych tabelach sesji SQL; na produkcyjnej tabeli nie został wywołany.

## Zakres i źródło

Źródło `epicaHR.sha.vPracownikKontoSynchronizacja` utworzono osobnym skryptem epicaHR. Warstwa docelowa
`indigoERP.rdb.vPracownikKontoSynchronizacja` i `indigoERP.mvw.vPracownikKontoSynchronizacja` służy lokalnej
procedurze `dbo.aspSynchronizujKontaEpicaHR`. Dotyczy osób o typie umowy P; nie wywołuje procedur baz huta ani
inout oraz nie modyfikuje istniejącej procedury `dbo.aspMergeUzytkownik` ani zadań SQL Agent.

Aktualizacja 2026-11-09: przygotowano rozszerzenie źródła o rodzaj T (pracownicy tymczasowi APT) w skrypcie
`epicaHR/_SQL Queries/epicaHR.Api20261109120000.sql`. Jest niewdrożone; do czasu jego wykonania produkcyjna
procedura obejmuje P. Reguły kont, blokowania i odblokowania pozostają takie same.

Nowsza decyzja: zamiast skryptu 120000 przygotowano sekwencję `epicaHR.Api20261109130000.sql` w epicaHR,
nastepnie `indigoERP.Api20261109130100.sql` w indigoERP. Obie zmiany nie są wdrożone. Rozszerzają źródło o T
oraz rodzaj osoby, a procedurę o priorytet P dla kolizji nowych nazw. Dwie osoby P o tej samej nazwie lub dwie T
są pomijane. Jeśli kolidujące konto już istnieje, jest zachowane, a nowa osoba pomijana niezależnie od typu.
Dodanie i odblokowanie wymaga obecnego angażu statusu 2; zwolnienie nadal podlega osobnej regule blokady statusu 5.
Skrypty należy uruchomić kolejno bez wywoływania synchronizacji pomiędzy nimi, po czym sprawdzić podgląd @Wykonaj=0.

Podgląd projektowanego źródła na 9.11.2026: 195 nowych kont T, 14 kolizji T oraz 5 wcześniejszych kolizji P;
brak blokad i odblokowań. Próba bieżącej procedury na tymczasowej kopii danych i projektowanym źródle:
195 nowych kont w stanie zablokowanym, drugi przebieg bez zmian. Trigger produkcyjny i SSO nie były uruchamiane.
Test priorytetu P: 10/10 syntetycznych przypadków SELECT; `_Documentation/TestSynchronizacjaKontPriorytet_temp.sql`
uruchomił projektowaną procedurę z projektowanym źródłem na tymczasowej kopii: 195 dodanych, 19 kolizji,
drugi przebieg bez zmian, kontrola podstawowych danych produkcyjnych bez zmian. Testy nie tworzyły kont produkcyjnych.

Zależność do wyjaśnienia przed uznaniem obsługi SSO za pełną: wdrożony `epicaHR.sha.vPracownikLista`, z którego
korzysta `indigoERP.rdb.vPracownikLista` i `uspGetUzytkownikSso`, nie udostępnia T. Odczyt wykazał 209/209
aktywnych T bez dopasowania w tym widoku, co może powodować `usrPracownikID=0` w tokenie mimo istniejącego konta.
Nie zmieniano szeroko używanego widoku ani procedury logowania w ramach tej zmiany; wymagają osobnej analizy zależności.

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

## Test izolowany 2026-11-09

`TestSynchronizacjaKont_temp.sql` kopiuje dane do tabel tymczasowych sesji, pobiera definicję aktualnej procedury
i podstawia wyłącznie tabele źródła i celu oraz nazwę blokady aplikacyjnej. Nie tworzy kont produkcyjnych.
Podgląd i pierwszy zapis w kopii: 65 DODAJ, 11 BLOKUJ, 2 ODBLOKUJ, 5 POMIN_KOLIZJA, 2 POMIN_ODBLOKOWANIE.
Sprawdzono 65 nowych zablokowanych kont bez AD/hasła, 11 faktycznych blokad i 2 odblokowania. Drugi zapis:
0 zmienionych kont, 5 kolizji i 7 pominiętych odblokowań; 5 dodatkowych pozycji to nowo utworzone konta osób
ponownie zatrudnionych, które celowo są zablokowane i nie mają danych logowania. Kontrola liczby kont, sumy
kontrolnej wybranych pól, liczby członkostw w grupach i bieżącego IDENTITY nie wykazała zmian w produkcji.

Test nie uruchamiał produkcyjnego triggera na tabeli tymczasowej ani nie potwierdza faktycznej autoryzacji SSO,
uprawnień konta serwisowego, pełnego stanu wszystkich pól czy działania wykonania na produkcyjnym celu.
Trigger `utrAiUzytkownik` sprawdzono przez odczyt definicji i stanu. Skrypt wyświetla identyfikatory i nazwy kont;
jego wynik należy traktować jako dane osobowe. Nie zapisano wyniku szczegółowego do pliku.

## Wykonanie produkcyjne 2026-11-09

Na wyraźne polecenie użytkownika wykonano `dbo.aspSynchronizujKontaEpicaHR @Wykonaj=1` na JAPET.indigoERP.
W zewnętrznej transakcji przed zatwierdzeniem sprawdzono zgodność kopii statusów i wyniku z podglądem:
65 nowych zablokowanych kont bez nazwy AD i hasła, 11 blokad, 2 odblokowania, 65 członkostw w grupie 0.
Nie zmieniono pozostałych statusów. Transakcja została zatwierdzona. Następny podgląd: 5 POMIN_KOLIZJA
i 7 POMIN_ODBLOKOWANIE; brak pozycji DODAJ, BLOKUJ i ODBLOKUJ. Łączna liczba kont wzrosła z 1530 do 1595.

W `_Documentation/SynchronizacjaKont/` zachowano kopię poprzednich statusów i listę 65 kont do uzupełnienia AD.
Lista zawiera dane osobowe i służy administratorowi; nie jest plikiem do zatwierdzania w repozytorium.
Nie weryfikowano przypisania nazw AD ani logowania SSO. Stary job pozostał wyłączony.


## Zmiana przygotowana 2026-11-10 — P i APT (niewdrożona)

> Poniższe zasady zastępują wcześniejsze opisy blokowania dopiero po zwolnieniu i warunkowego odblokowania po wdrożeniu skryptów z tej daty.

Dla pracowników własnych P i tymczasowych T aktywny, obowiązujący dziś angaż o statusie `2` oznacza `usrBlokada=0` i `usrStatus=2`. Przy braku takiego angażu konto jest blokowane i otrzymuje `usrStatus=1`. Nowe konta aktywne nie otrzymują nazwy AD ani hasła; administrator uzupełnia nazwę AD. Synchronizacja odblokowuje też konta zablokowane ręcznie. Konta niepowiązane oraz usunięte nie są aktualizowane; kolizje nazw są pomijane z pierwszeństwem pracowników P.

Na liście użytkowników i w SSO pracownicy APT mają wydział i stanowisko z aktualnego angażu; do nazwy stanowiska dopisuje się ` (APT)`. Źródłem jest `epicaHR.sha.vPracownikListaAPT`, udostępniane przez `indigoERP.rdb.vPracownikLista`. Skrypty są przygotowane, nie zostały wdrożone ani uruchomione.

## Stan i poprawka 2026-11-16

Na JAPET nadal działa starsza procedura, tworząca konta ze statusem 1 i blokadą. Przygotowano `_SQL Queries/indigoERP.Api20261116120000.sql` do wdrożenia opisanej wyżej reguły P/T; skrypt nie wykonuje synchronizacji. Po wdrożeniu należy sprawdzić podgląd `@Wykonaj=0` przed ewentualnym zapisem. Kolizje nazw z innymi kontami są nadal pomijane, ponieważ samą zgodnością imienia i nazwiska nie można bezpiecznie potwierdzić tożsamości.

Wdrożenie 2026-11-16: na polecenie użytkownika wykonano skrypt w JAPET / indigoERP i potwierdzono nową definicję. Podgląd wykazał 1 blokadę, 205 odblokowań i 16 pominiętych kolizji. Synchronizacji zapisującej nie uruchomiono; statusy kont nie zostały zmienione.
