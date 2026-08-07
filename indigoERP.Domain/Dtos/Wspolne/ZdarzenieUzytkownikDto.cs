namespace indigoERP.Domain.Dtos._Wspolne
{
    public class ZdarzenieUzytkownikDto
    {
        public int zurID { get; set; }
        public int? zurZdarzenieID { get; set; }
        public int? zurUzytkownikID { get; set; }
        public bool? zurOdczyt { get; set; }
        public string? zurStatus { get; set; }
    }
}
