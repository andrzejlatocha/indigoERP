// Uruchamiane w tle przez Outlook przy LaunchEvent (nowa wiadomość / odpowiedź / przekazanie).
// Pobiera gotowy HTML podpisu z API indigoERP (po adresie email z profilu M365, bez logowania
// do indigoERP) i wstawia go przez setSignatureAsync — ta metoda nadpisuje wydzielony region
// podpisu w wiadomości zamiast doklejać kolejny, więc odpowiedzi/przekazania nie duplikują podpisu.

Office.onReady();

function onNewMessageComposeHandler(event) {
  fetchSignatureHtml()
    .then(function (html) {
      if (!html) {
        event.completed();
        return;
      }

      Office.context.mailbox.item.body.setSignatureAsync(
        html,
        { coercionType: Office.CoercionType.Html },
        function () {
          event.completed();
        }
      );
    })
    .catch(function () {
      // Brak podpisu/błąd sieci nie powinien blokować użytkownikowi wysłania wiadomości.
      event.completed();
    });
}

function fetchSignatureHtml() {
  var email = Office.context.mailbox.userProfile.emailAddress;
  var url = PODPIS_ADDIN_CONFIG.apiBaseUrl + "Podpis/PreviewByEmail?email=" + encodeURIComponent(email);

  return fetch(url, {
    headers: { "X-Api-Key": PODPIS_ADDIN_CONFIG.apiKey }
  }).then(function (response) {
    if (!response.ok) {
      return "";
    }

    return response.json();
  });
}

Office.actions.associate("onNewMessageComposeHandler", onNewMessageComposeHandler);
