namespace indigoERP.Domain.Models._Wspolne
{
    /// <summary>Plik PDF załączany do wysyłki (nazwa oryginalna i zawartość).</summary>
    public sealed record WiadomoscPlikWysylki(string Nazwa, byte[] Dane);
}