namespace indigoERP.Domain.Common
{
    /// <summary>
    /// Reguły harmonogramu wysyłki wiadomości publicznych: walidacja terminów przy zapisie (API)
    /// i wyliczanie kolejnego terminu cyklu (Worker). Jedna definicja dla obu miejsc.
    /// </summary>
    public static class HarmonogramTerminy
    {
        public const string Jednorazowo = "J";
        public const string Dziennie = "D";
        public const string Tygodniowo = "T";
        public const string Miesiecznie = "M";

        /// <summary>Zwraca komunikat o pierwszym błędzie terminów albo null, gdy harmonogram jest poprawny.</summary>
        public static string? Waliduj(string? kod, DateTime? start, DateTime? koniec, DateTime teraz)
        {
            if (kod is not (Jednorazowo or Dziennie or Tygodniowo or Miesiecznie))
            {
                return "Nieprawidłowy rodzaj harmonogramu (J, D, T albo M).";
            }

            if (start == null)
            {
                return "Podaj termin pierwszej wysyłki.";
            }

            if (start <= teraz)
            {
                return "Termin pierwszej wysyłki musi być w przyszłości.";
            }

            if (kod != Jednorazowo && koniec is DateTime dataKoncowa && dataKoncowa < start)
            {
                return "Data końcowa cyklu jest wcześniejsza niż pierwszy termin.";
            }

            return null;
        }

        /// <summary>Kolejny termin cyklu po podanym. Dla jednorazowej wysyłki (J) zwraca DateTime.MaxValue (brak kolejnego).</summary>
        public static DateTime NastepnyTermin(string kod, DateTime poprzedni)
        {
            return kod switch
            {
                Dziennie => poprzedni.AddDays(1),
                Tygodniowo => poprzedni.AddDays(7),
                Miesiecznie => poprzedni.AddMonths(1),
                _ => DateTime.MaxValue,
            };
        }
    }
}
