namespace indigoERP.Domain.Dtos._Administrator
{
    public class RolaDto
    {
        public int srlID { get; set; }
        public int? srlAplikacjaID { get; set; }
        public bool? srlEdycja { get; set; }
        public string srlNazwa { get; set; }
        public string? srlOpis { get; set; }
        public bool? srlBlokada { get; set; }
        public string? srlStatus { get; set; }
    }
}
