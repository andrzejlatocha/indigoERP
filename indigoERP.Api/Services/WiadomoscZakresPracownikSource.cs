using System.Text.RegularExpressions;
using indigoERP.Api.Data;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Services
{
    /// <summary>
    /// Odbiorcy zakresu wysyłki (widok app.vWiadomoscZakres*). Zwraca null, gdy zakres nie istnieje, nie jest aktywny
    /// lub nazwa widoku jest nieprawidłowa.
    /// </summary>
    public interface IWiadomoscZakresPracownikSource
    {
        Task<IReadOnlyList<WiadomoscZakresPracownikViewModel>?> Pobierz(int wzaID, CancellationToken cancellationToken);
    }

    public sealed class WiadomoscZakresPracownikSource : IWiadomoscZakresPracownikSource
    {
        private static readonly Regex WiadomoscZakresViewPattern = new(
            "^app\\.vWiadomoscZakres[0-9A-Za-z_]*$", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

        private readonly indigoERPContext _context;

        public WiadomoscZakresPracownikSource(indigoERPContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<WiadomoscZakresPracownikViewModel>?> Pobierz(int wzaID, CancellationToken cancellationToken)
        {
            var zakres = await _context.WiadomoscZakresEnumerator
                .AsNoTracking()
                .SingleOrDefaultAsync(w => w.wzaID == wzaID && w.wzaStatus == "2", cancellationToken);

            if (zakres?.wzaWidok is null || !TryGetViewName(zakres.wzaWidok, out var viewName))
            {
                return null;
            }

            // viewName is an identifier validated against the fixed app.vWiadomoscZakres pattern.
#pragma warning disable EF1002
            return await _context.WiadomoscZakresPracownikViewModel
                .FromSqlRaw($"SELECT prcID, prcKod, prcImie, prcNazwisko, kntID, kntKontakt FROM {viewName}")
                .AsNoTracking()
                .OrderBy(w => w.prcNazwisko)
                .ThenBy(w => w.prcImie)
                .ToListAsync(cancellationToken);
#pragma warning restore EF1002
        }

        private static bool TryGetViewName(string widok, out string viewName)
        {
            viewName = string.Empty;

            if (!WiadomoscZakresViewPattern.IsMatch(widok))
            {
                return false;
            }

            viewName = $"[app].[{widok[4..]}]";
            return true;
        }
    }
}