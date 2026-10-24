namespace indigoERP.Domain.Models._Administrator
{
    /// <summary>Jedna zaznaczona zmiana do zatwierdzenia (identyfikator użytkownika i oczekiwana akcja).</summary>
    public class UzytkownikEmailGraphZmianaModel
    {
        public int usrID { get; set; }
        public string akcja { get; set; } = string.Empty;
    }

    /// <summary>Żądanie zatwierdzenia zaznaczonych zmian adresów e-mail.</summary>
    public class UzytkownikEmailGraphZatwierdzModel
    {
        public List<UzytkownikEmailGraphZmianaModel> zmiany { get; set; } = new();
    }

    /// <summary>Wynik zatwierdzenia: ile adresów dodano, poprawiono, usunięto oraz ile pominięto.</summary>
    public class UzytkownikEmailGraphWynikModel
    {
        public int dodano { get; set; }
        public int poprawiono { get; set; }
        public int usunieto { get; set; }
        public int pominieto { get; set; }

        /// <summary>Powody pominięcia (np. dane zmieniły się od czasu podglądu).</summary>
        public List<string> komunikaty { get; set; } = new();
    }
}
