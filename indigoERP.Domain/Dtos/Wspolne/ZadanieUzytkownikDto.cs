namespace indigoERP.Domain.Dtos._Wspolne
{
    public class ZadanieUzytkownikDto
    {
        public int tusID { get; set; }
        public int? tusZadanieID { get; set; }
        public int? tusUzytkownikID { get; set; }
        public bool? tusOdczyt { get; set; }
        public bool? tusAkceptacja { get; set; }
        public string? tusStatus { get; set; }
    }
}
