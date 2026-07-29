using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Dtos._Administrator
{
    public class AplikacjaDto
    {
        [Range(101, int.MaxValue, ErrorMessage = "Symbol aplikacji musi być większy od 100")]
        public int appID { get; set; }
        public string? appNazwa { get; set; }
        public string? appWersja { get; set; }
        public string? appOpis { get; set; }
        public string? appLink { get; set; }
        public string? appIkona { get; set; }
        public string? appFolder { get; set; }
        public bool? appWyswietl { get; set; }
        public string? appStatus { get; set; }
    }
}
