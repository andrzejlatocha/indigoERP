using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using indigoERP.Domain.Common;

namespace indigoERP.Domain.Entities._Wspolne
{
    /// <summary>Tabela dbo.Powiadomienie: powiadomienie dla użytkownika w danej aplikacji (zapis przez API).</summary>
    public class Powiadomienie : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int powID { get; set; }
        public int powAplikacjaID { get; set; }
        public int powUzytkownikID { get; set; }
        public string powKodRodzaj { get; set; } = string.Empty;
        public string powTytul { get; set; } = string.Empty;
        public string? powTresc { get; set; }
        public string? powLink { get; set; }
        public DateTime powDataPowiadomienia { get; set; }
        public bool powOdczyt { get; set; }
        public DateTime? powDataOdczytu { get; set; }
    }
}
