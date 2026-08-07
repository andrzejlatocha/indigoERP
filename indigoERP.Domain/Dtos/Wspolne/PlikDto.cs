namespace indigoERP.Domain.Dtos._Wspolne
{
    public class PlikDto
    {
        public PlikDto()
        {
            plkStatus = "2";
        }

        public int plkID { get; set; }
        public int? plkAplikacjaID { get; set; }
        public string? plkRozszerzenie { get; set; }
        public string? plkNazwa { get; set; }
        public string? plkNumer { get; set; }
        public string? plkNumerSystem { get; set; }
        public string? plkArchiwum { get; set; }
        public string? plkSzafa { get; set; }
        public string? plkPolka { get; set; }
        public string? plkTeczka { get; set; }
        public string? plkPozycja { get; set; }
        public string? plkPlik { get; set; }
        public string? plkLink { get; set; }
        public string? plkFolder { get; set; }
        public byte? plkWersja { get; set; }
        public string? plkOpis { get; set; }
        public string? plkStatus { get; set; }
    }
}
