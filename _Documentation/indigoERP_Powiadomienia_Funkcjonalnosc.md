# Powiadomienia SignalR

Uwierzytelnieni użytkownicy subskrybują zmiany liczników dla aplikacji. API monitoruje własne dane wiadomości, zadań i zdarzeń, również tworzone przez Worker. SignalR wysyła tylko sygnał, a klient odczytuje dane przez API.

Nieprzeczytane powiadomienia użytkownika są dostępne przez SelectNieprzeczytane; API porównuje usrID z tokenem. Monitor wysyła również sygnał zmiany ich liczby.
