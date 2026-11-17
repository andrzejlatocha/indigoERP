using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Common
{
    /// <summary>Walidacja pola linku: pusty lub bezpieczny link (patrz <see cref="BezpieczneLinki"/>).</summary>
    public sealed class DozwolonyLinkAttribute : ValidationAttribute
    {
        public DozwolonyLinkAttribute()
            : base("Link musi być ścieżką aplikacji zaczynającą się od / albo adresem http lub https.")
        {
        }

        public override bool IsValid(object? value)
        {
            return value is null || (value is string tekst && BezpieczneLinki.Dozwolony(tekst));
        }
    }
}
