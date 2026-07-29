namespace indigoERP.Domain.Dtos._Administrator
{
    public class GrupaDto
    {
        public int groID { get; set; }
        public int? groAplikacjaID { get; set; }
        public string groNazwa { get; set; }
        public string? groOpis { get; set; }
        public DateTime? groDataWaznosc { get; set; }
        public bool? groGrupaWszyscy { get; set; }
        public bool? groAdministrator { get; set; }
        public bool? groBlokada { get; set; }
        public bool? groEdycja { get; set; }
        public string? groStatus { get; set; }
    }
}
