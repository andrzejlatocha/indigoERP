using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;
namespace indigoERP.Domain.Entities._Wspolne
{
    public class Wiadomosc : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int wiaID { get; set; }
        public int? wiaWiadomoscID { get; set; }
        public int? wiaAplikacjaID { get; set; }
        public int? wiaUzytkownikID { get; set; }
        public string? wiaEmail { get; set; }
        public string? wiaTemat { get; set; }
        public string? wiaTresc { get; set; }
        public DateTime? wiaData { get; set; }
        public string? wiaGrupa { get; set; }
        public Guid? wiaSesja { get; set; }
        public string? wiaStatus { get; set; }
    }
}
