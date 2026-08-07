namespace indigoERP.Domain.Dtos._Wspolne
{
    public class WiadomoscUzytkownikDto
    {
        public int wuzID { get; set; }
        public int? wuzWiadomoscID { get; set; }
        public int? wuzUzytkownikID { get; set; }
        public bool? wuzAkceptacja { get; set; }
        public bool? wuzOdczyt { get; set; }
        public string? wuzStatus { get; set; }
    }
}
