using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Dtos._Administrator
{
    public class RolaCzlonkowieDto : IValidatableObject
    {
        public int cwrID { get; set; }
        public int? cwrUzytkownikID { get; set; }
        public int? cwrRolaID { get; set; }
        public int? cwrGrupaID { get; set; }
        public string? cwrStatus { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (cwrRolaID.GetValueOrDefault() <= 0)
            {
                yield return new ValidationResult("Rola jest wymagana.", new[] { nameof(cwrRolaID) });
            }

            var hasUser = cwrUzytkownikID.GetValueOrDefault() > 0;
            var hasGroup = cwrGrupaID.GetValueOrDefault() > 0;

            if (hasUser == hasGroup)
            {
                yield return new ValidationResult(
                    "Członek roli musi wskazywać jednego użytkownika albo jedną grupę.",
                    new[] { nameof(cwrUzytkownikID), nameof(cwrGrupaID) });
            }
        }
    }
}
