// Ręczny odpowiednik commands.js — przycisk "Wstaw / odśwież podpis" w task pane.
// Fallback na wypadek gdy auto-insert (LaunchEvent) nie zadziała, oraz sposób na ręczne
// odświeżenie podpisu w trakcie pisania wiadomości (np. po zmianie szablonu w panelu Podpisy).

Office.onReady(function () {
  document.getElementById("insertButton").disabled = false;
  document.getElementById("insertButton").addEventListener("click", insertSignature);
});

function insertSignature() {
  setStatus("Wstawiam podpis…");

  var email = Office.context.mailbox.userProfile.emailAddress;
  var url = PODPIS_ADDIN_CONFIG.apiBaseUrl + "Podpis/PreviewByEmail?email=" + encodeURIComponent(email);

  fetch(url, { headers: { "X-Api-Key": PODPIS_ADDIN_CONFIG.apiKey } })
    .then(function (response) {
      if (!response.ok) {
        throw new Error("Błąd API: " + response.status);
      }

      return response.json();
    })
    .then(function (html) {
      if (!html) {
        setStatus("Brak przypisanego szablonu podpisu dla " + email + ".");
        return;
      }

      Office.context.mailbox.item.body.setSignatureAsync(
        html,
        { coercionType: Office.CoercionType.Html },
        function (asyncResult) {
          if (asyncResult.status === Office.AsyncResultStatus.Failed) {
            setStatus("Nie udało się wstawić podpisu: " + asyncResult.error.message);
            return;
          }

          setStatus("Podpis wstawiony.");
        }
      );
    })
    .catch(function (error) {
      setStatus("Błąd: " + error.message);
    });
}

function setStatus(message) {
  document.getElementById("status").textContent = message;
}
