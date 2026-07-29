using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    [Table("vZadanieLista", Schema = "mvw")]
    public class ZadanieViewModel
    {
        [Key]
        public int tskID { get; set; }
        public int tskZadanieID { get; set; }
        public int tskAplikacjaID { get; set; }
        public string? tskSerwer { get; set; }
        public string? tskBaza { get; set; }
        public string? tskTablica { get; set; }
        public int? tskIndeksRekordu { get; set; }
        public int? tskKategoriaZadanieID { get; set; }
        public string? ktzIkona { get; set; }
        public string? ktzOpis { get; set; }
        public int? tskUzytkownikID { get; set; }
        public string? usrNazwa { get; set; }
        public string? usrImieNazwisko { get; set; }
        public string? tskUzytkownikLST { get; set; }
        public bool tskRodzic { get; set; }
        public int tskKierunekZadanieID { get; set; }
        public string kizIkona { get; set; }
        public string kizOpis { get; set; }
        public int tskFlagaZadanieID { get; set; }
        public string fzaIkona { get; set; }
        public string fzaOpis { get; set; }
        public int tskPriorytetZadanieID { get; set; }
        public string przIkona { get; set; }
        public string przOpis { get; set; }
        public DateTime tskData { get; set; }
        public DateTime tskDataRozpoczecie { get; set; }
        public DateTime tskDataZakonczenie { get; set; }
        public bool tskCalodzienny { get; set; }
        public string tskTemat { get; set; }
        public string tskTresc { get; set; }
        public int tskEtykieta { get; set; }
        public int? tskLokalizacjaID { get; set; }
        public string? lokOpis { get; set; }
        public int? tskZasobyID { get; set; }
        public string? tskZasobyLST { get; set; }
        public bool tskPubliczny { get; set; }
        public byte tskPostep { get; set; }
        public string tskStatus { get; set; }
        public int tusID { get; set; }
        public int tusUzytkownikID { get; set; }
        public bool tusAkceptacja { get; set; }
        public string tusStatus { get; set; }
    }
}
