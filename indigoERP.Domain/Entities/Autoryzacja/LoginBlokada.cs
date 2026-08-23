using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace indigoERP.Domain.Entities._Autoryzacja
{
    // Aktywna/historyczna blokada logowania (ban manager, authorizationSRV).
    // Tabela app.LoginBlokada na indigoERP — już wdrożona, zarządzana przez
    // authorizationSRV.Api. Blokada aktywna, gdy GETDATE() jest pomiędzy
    // lgbDataOd i lgbDataDo, a lgbDataOdblokowania jest NULL.
    [Table("LoginBlokada", Schema = "app")]
    public class LoginBlokada
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lgbID { get; set; }

        [Required]
        [MaxLength(50)]
        public string lgbUsrNazwa { get; set; } = string.Empty;

        public DateTime lgbDataOd { get; set; }

        public DateTime lgbDataDo { get; set; }

        [MaxLength(200)]
        public string? lgbPowod { get; set; }

        public int? lgbOdblokowanoUsrID { get; set; }

        public DateTime? lgbDataOdblokowania { get; set; }
    }
}
