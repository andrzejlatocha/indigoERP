namespace indigoERP.Domain.Dtos._Administrator
{
    public class UzytkownikDto
    {
        public int usrID { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrImie { get; set; }
        public string? usrNazwisko { get; set; }
        public string? usrImieNazwisko { get; set; }
        public string? usrInicjal { get; set; }
        public int? usrKlientID { get; set; }
        public int? usrPracownikID { get; set; }
        public string? usrJezyk { get; set; }
        public string? usrTelefon { get; set; }
        public string? usrEmail { get; set; }
        public byte[]? usrHaslo { get; set; }
        public DateTime? usrDataWaznoscHaslo { get; set; }
        public bool? usrNowy { get; set; }
        public bool? usrBlokada { get; set; }
        public bool? usrEdycja { get; set; }
        public string? usrStatus { get; set; }
    }
}
