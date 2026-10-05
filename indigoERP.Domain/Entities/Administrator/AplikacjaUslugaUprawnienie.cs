using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Administrator
{
    // Nadanie dostępu service-to-service: aplikacja usgAplikacjaZrodloID wolno wywoływać
    // aplikację usgAplikacjaCelID z rolą usgKodRola, bez udziału zalogowanego użytkownika
    // (authorizationSRV.Api, Service/GetToken). Tabela app.AplikacjaUslugaUprawnienie —
    // poza dbo/enum/mvw, więc bez AuditableEntity (tabela nie ma kolumn audytowych).
    public partial class AplikacjaUslugaUprawnienie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int usgID { get; set; }

        public int usgAplikacjaZrodloID { get; set; }
        public int usgAplikacjaCelID { get; set; }
        public string usgKodRola { get; set; }
        public bool usgAktywny { get; set; }
        public DateTime usgDataOd { get; set; }
        public DateTime? usgDataDo { get; set; }
        public string usgOpis { get; set; }
    }
}
