// Strona /wiadomosc (Głos Hutnika): usuwa z adresu parametry komunikatu po jego pokazaniu, żeby nie wracał przy odświeżeniu (F5).
window.glosHutnika = {
    usunKomunikat: function () {
        try {
            const adres = new URL(window.location.href);
            ["glosSeverity", "glosTytul", "glosTresc"].forEach(function (p) { adres.searchParams.delete(p); });
            window.history.replaceState(null, "", adres.toString());
        } catch (e) {
            // Brak obsługi adresu: komunikat może pojawić się ponownie po odświeżeniu, nic więcej się nie psuje.
        }
    }
};
