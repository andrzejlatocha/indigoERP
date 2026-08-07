using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
namespace indigoERP.Domain.Entities._Wspolne
{
    public class Zadanie : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tskID { get; set; }
        public int? tskZadanieID { get; set; }
        public int? tskAplikacjaID { get; set; }
        public string? tskSerwer { get; set; }
        public string? tskBaza { get; set; }
        public string? tskTablica { get; set; }
        public int? tskIndeksRekordu { get; set; }
        public int? tskKategoriaZadanieID { get; set; }
        public string? tskKategoriaZadanieLST { get; set; }
        public int? tskUzytkownikID { get; set; }
        public string? tskUzytkownikLST { get; set; }
        public bool? tskRodzic { get; set; }
        public int? tskKierunekZadanieID { get; set; }
        public int? tskFlagaZadanieID { get; set; }
        public int? tskPriorytetZadanieID { get; set; }
        public DateTime? tskData { get; set; }
        public DateTime? tskDataRozpoczecie { get; set; }
        public DateTime? tskDataZakonczenie { get; set; }
        public bool? tskCalodzienny { get; set; }
        public string? tskTemat { get; set; }
        public string? tskTresc { get; set; }
        public int? tskEtykieta { get; set; }
        public int? tskLokalizacjaID { get; set; }
        public int? tskZasobyID { get; set; }
        public string? tskZasobyLST { get; set; }
        public bool? tskPubliczny { get; set; }
        public byte? tskPostep { get; set; }
        public Guid? tskSesja { get; set; }
        public string? tskStatus { get; set; }
    }
}
