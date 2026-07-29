using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Dtos._Administrator
{
    public class GrupaCzlonkowieDto : IValidatableObject
    {
        public int uwgID { get; set; }
        public int? uwgGrupaNadrzednaID { get; set; }
        public int? uwgUzytkownikID { get; set; }
        public int? uwgGrupaID { get; set; }
        public bool? uwgDomyslna { get; set; }
        public string? uwgStatus { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (uwgGrupaNadrzednaID.GetValueOrDefault() <= 0)
            {
                yield return new ValidationResult("Grupa nadrzędna jest wymagana.", new[] { nameof(uwgGrupaNadrzednaID) });
            }

            var hasUser = uwgUzytkownikID.GetValueOrDefault() > 0;
            var hasGroup = uwgGrupaID.GetValueOrDefault() > 0;

            if (hasUser == hasGroup)
            {
                yield return new ValidationResult(
                    "Członek grupy musi wskazywać jednego użytkownika albo jedną grupę.",
                    new[] { nameof(uwgUzytkownikID), nameof(uwgGrupaID) });
            }
        }
    }
}
