using indigoERP.Api.Data;
using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.ViewModels._Administracja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace indigoERP.Api.Controllers._Administrator
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UzytkownikController : ControllerBase
    {
        private const string IndigoAdministratorRole = "HCZ-indigoERP-Administrator";
        private const string BearerScheme = "Bearer";

        private readonly indigoERPContext _context;

        public UzytkownikController(indigoERPContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UzytkownikViewModel>>> Select(string usrStatus)
        {
            return await _context.UzytkownikListaViewModel
                .Where(w => w.usrStatus == usrStatus || usrStatus == "*")
                .Select(w => new UzytkownikViewModel
                {
                    usrID = w.usrID,
                    usrPracownikID = w.usrPracownikID,
                    prcKod = w.prcKod,
                    usrNazwa = w.usrNazwa,
                    usrImie = w.usrImie,
                    usrNazwisko = w.usrNazwisko,
                    usrImieNazwisko = w.usrImieNazwisko,
                    usrInicjal = w.usrInicjal,
                    usrJezyk = w.usrJezyk,
                    usrTelefon = w.usrTelefon,
                    usrKomorka = w.usrKomorka,
                    usrEmail = w.usrEmail,
                    usrDataWaznoscHaslo = w.usrDataWaznoscHaslo,
                    usrNowy = w.usrNowy,
                    usrBlokada = w.usrBlokada,
                    usrEdycja = w.usrEdycja,
                    usrStatus = w.usrStatus,
                    appID = w.appID,
                    appFolder = w.appFolder,
                    usrFolder = w.usrFolder,
                    usrZdjecie = w.usrZdjecie,
                    wdzID = w.wdzID,
                    wdzKod = w.wdzKod,
                    wdzSymbol = w.wdzSymbol,
                    wdzNazwa = w.wdzNazwa,
                    stnKod = w.stnKod,
                    stnSymbol = w.stnSymbol,
                    stnNazwa = w.stnNazwa,
                    usrGuid = w.usrGuid,
                    usrZablokowanyLogowaniem = w.usrZablokowanyLogowaniem
                })
                .AsNoTracking()
                .ToListAsync();
        }

        // Odblokowuje logowanie zablokowane przez ban managera (authorizationSRV, app.LoginBlokada) —
        // ustawia lgbDataOdblokowania/lgbOdblokowanoUsrID na wszystkich aktualnie aktywnych blokadach
        // dla danej nazwy użytkownika. To NIE dotyczy dbo.Uzytkownik.usrBlokada (osobna, ręczna flaga).
        [Authorize(Roles = IndigoAdministratorRole, AuthenticationSchemes = BearerScheme)]
        [HttpPost]
        public async Task<IActionResult> OdblokujLogowanie(string usrNazwa)
        {
            var teraz = DateTime.Now;

            var aktywneBlokady = await _context.LoginBlokada
                .Where(b => b.lgbUsrNazwa == usrNazwa && b.lgbDataOdblokowania == null && b.lgbDataOd <= teraz && b.lgbDataDo >= teraz)
                .ToListAsync();

            if (aktywneBlokady.Count == 0)
            {
                return NotFound();
            }

            int? adminUsrID = int.TryParse(User.FindFirst("usrID")?.Value, out var parsed) ? parsed : null;

            foreach (var blokada in aktywneBlokady)
            {
                blokada.lgbDataOdblokowania = teraz;
                blokada.lgbOdblokowanoUsrID = adminUsrID;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{rola}")]
        public async Task<ActionResult<IEnumerable<UzytkownikViewModel>>> SelectByRola(string rola, string usrStatus)
        {
            // "userIds" korzysta z gotowego widoku mvw.vUzytkownikRolaEfektywna (bezpośrednie +
            // dziedziczone członkostwo, już zdeduplikowane) zamiast duplikować tu tę samą logikę UNION —
            // patrz RolaController.SelectUzytkownikByRola / UzytkownikController.SelectRolaByUzytkownik.
            return await _context.UzytkownikViewModel
                .FromSqlInterpolated($"""
                    WITH userIds AS
                    (
                        SELECT DISTINCT usrID
                        FROM mvw.vUzytkownikRolaEfektywna
                        WHERE srlNazwa = {rola}
                            AND (usrStatus = {usrStatus} OR {usrStatus} = '*')
                    )
                    SELECT
                        u.usrID,
                        ISNULL(u.usrPracownikID, ISNULL(d.usrPracownikID, 0)) AS usrPracownikID,
                        d.prcKod,
                        ISNULL(u.usrNazwa, d.usrNazwa) AS usrNazwa,
                        ISNULL(u.usrImie, d.usrImie) AS usrImie,
                        ISNULL(u.usrNazwisko, d.usrNazwisko) AS usrNazwisko,
                        ISNULL(u.usrInicjal, d.usrInicjal) AS usrInicjal,
                        ISNULL(u.usrTelefon, d.usrTelefon) AS usrTelefon,
                        ISNULL(d.usrKomorka, u.usrKomorka) AS usrKomorka,
                        ISNULL(u.usrEmail, d.usrEmail) AS usrEmail,
                        ISNULL(d.appID, 0) AS appID,
                        d.appFolder,
                        ISNULL(d.usrFolder, UPPER(REPLACE(CONVERT(varchar(36), u.rowguid), '-', ''))) AS usrFolder,
                        ISNULL(d.usrZdjecie, 'empty.png') AS usrZdjecie,
                        ISNULL(d.wdzID, 0) AS wdzID,
                        ISNULL(d.wdzKod, '00') AS wdzKod,
                        ISNULL(d.wdzSymbol, '') AS wdzSymbol,
                        ISNULL(d.wdzNazwa, '') AS wdzNazwa,
                        ISNULL(d.stnKod, '') AS stnKod,
                        ISNULL(d.stnSymbol, '') AS stnSymbol,
                        ISNULL(d.stnNazwa, 'Pracownik zewnętrzny') AS stnNazwa,
                        ISNULL(d.usrDataWaznoscHaslo, ISNULL(u.usrDataWaznoscHaslo, CONVERT(datetime, '20991231', 112))) AS usrDataWaznoscHaslo,
                        d.srlID,
                        d.srlNazwa,
                        ISNULL(u.rowguid, CONVERT(uniqueidentifier, '00000000-0000-0000-0000-000000000000')) AS usrGuid,
                        ISNULL(d.usrToken, '???') AS usrToken
                    FROM dbo.Uzytkownik AS u
                    INNER JOIN userIds AS ids ON u.usrID = ids.usrID
                    OUTER APPLY
                    (
                        SELECT TOP (1)
                            v.*
                        FROM mvw.vUzytkownik AS v
                        WHERE v.usrID = u.usrID
                        ORDER BY CASE WHEN v.srlNazwa = {rola} THEN 0 ELSE 1 END, v.appID
                    ) AS d
                    WHERE ISNULL(u.Usuniety, 0) = 0
                        AND (u.usrStatus = {usrStatus} OR {usrStatus} = '*')
                    """)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Pobiera szczegóły użytkownika z widoku autoryzacji.
        /// </summary>
        /// <param name="appID">Identyfikator aplikacji.</param>
        /// <param name="usrID">Identyfikator użytkownika.</param>
        /// <returns>Klasa UzytkownikViewModel.</returns>
        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpGet("{appID:int}/{usrID:int}")]
        public async Task<ActionResult<UzytkownikViewModel>> Detail(int appID, int usrID)
        {
            var uzytkownik = await _context.UzytkownikViewModel
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.appID == appID && w.usrID == usrID);

            if (uzytkownik == null)
            {
                return NotFound();
            }

            return uzytkownik;
        }

        [AllowAnonymous]
        [HttpGet("{usrID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikAplikacjaViewModel>>> SelectAplikacjaByUzytkownik(int usrID, string appStatus)
        {
            return await _context.UzytkownikAplikacjaViewModel.Where(w => w.usrID == usrID && (w.appStatus == appStatus || appStatus == "*")).ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikRolaViewModel>>> SelectRolaByUzytkownik(int appID, int usrID, string srlStatus)
        {
            // Widok mvw.vUzytkownikRolaEfektywna trzyma już całą logikę (bezpośrednie + dziedziczone
            // członkostwo, deduplikacja z priorytetem) — patrz analogiczne użycie w
            // RolaController.SelectUzytkownikByRola (ten sam widok, filtrowany z drugiej strony).
            return await _context.UzytkownikRolaEfektywnaViewModel
                .Where(w => w.usrID == usrID
                    && (w.appID == appID || appID == -1)
                    && (w.srlStatus == srlStatus || srlStatus == "*"))
                .Select(w => new UzytkownikRolaViewModel
                {
                    cwrID = w.cwrID,
                    appID = w.appID,
                    usrID = w.usrID,
                    usrNazwa = w.usrNazwa,
                    usrNazwaAD = w.usrNazwaAD,
                    usrStatus = w.usrStatus,
                    srlID = w.srlID,
                    srlNazwa = w.srlNazwa,
                    srlOpis = w.srlOpis,
                    srlStatus = w.srlStatus,
                    uwgPoziom = w.uwgPoziom,
                    uwgDziedziczy = w.uwgDziedziczy
                })
                .AsNoTracking()
                .ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{appID}/{usrID}")]
        public async Task<ActionResult<IEnumerable<UzytkownikGrupaViewModel>>> SelectGrupaByUzytkownik(int appID, int usrID, string groStatus)
        {
            return await _context.UzytkownikGrupaViewModel
                .Where(w => (w.appID == appID || appID == -1) && w.usrID == usrID && (w.groStatus == groStatus || groStatus == "*"))
                .ToListAsync();
        }

        [Authorize(AuthenticationSchemes = BearerScheme)]
        [HttpGet("{usrID}")]
        public async Task<ActionResult<UzytkownikDto>> Get(int usrID)
        {
            var uzytkownik = await _context.Uzytkownik.FindAsync(usrID);

            if (uzytkownik == null)
            {
                return NotFound();
            }

            return uzytkownik.Adapt<UzytkownikDto>();
        }

        [Authorize(Roles = IndigoAdministratorRole, AuthenticationSchemes = BearerScheme)]
        [HttpPut("{usrID}")]
        public async Task<IActionResult> Update(int usrID, UzytkownikDto uzytkownikDto)
        {
            if (usrID != uzytkownikDto.usrID)
            {
                return BadRequest();
            }

            var uzytkownik = await _context.Uzytkownik.FindAsync(usrID);

            if (uzytkownik == null)
            {
                return NotFound();
            }

            PrepareUserData(uzytkownikDto);
            uzytkownikDto.usrHaslo ??= uzytkownik.usrHaslo;
            uzytkownikDto.usrDataWaznoscHaslo ??= uzytkownik.usrDataWaznoscHaslo;
            uzytkownikDto.Adapt(uzytkownik);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UzytkownikExists(usrID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [Authorize(Roles = IndigoAdministratorRole, AuthenticationSchemes = BearerScheme)]
        [HttpPost]
        public async Task<ActionResult<UzytkownikDto>> Insert(UzytkownikDto uzytkownikDto)
        {
            PrepareUserData(uzytkownikDto);
            uzytkownikDto.usrHaslo ??= Array.Empty<byte>();
            uzytkownikDto.usrStatus ??= "2";
            uzytkownikDto.usrJezyk = string.IsNullOrWhiteSpace(uzytkownikDto.usrJezyk) ? "pl-PL" : uzytkownikDto.usrJezyk;
            uzytkownikDto.usrTelefon ??= string.Empty;
            uzytkownikDto.usrEmail ??= string.Empty;
            var uzytkownik = uzytkownikDto.Adapt<Uzytkownik>();

            _context.Uzytkownik.Add(uzytkownik);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { uzytkownik.usrID }, uzytkownik.Adapt<UzytkownikDto>());
        }

        [Authorize(Roles = IndigoAdministratorRole, AuthenticationSchemes = BearerScheme)]
        [HttpDelete("{usrID}")]
        public async Task<IActionResult> Delete(int usrID)
        {
            var uzytkownik = await _context.Uzytkownik.FindAsync(usrID);

            if (uzytkownik == null)
            {
                return NotFound();
            }

            uzytkownik.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UzytkownikExists(int usrID)
        {
            return _context.Uzytkownik.Any(e => e.usrID == usrID);
        }

        private static void PrepareUserData(UzytkownikDto uzytkownikDto)
        {
            uzytkownikDto.usrNazwa ??= string.Empty;
            uzytkownikDto.usrImie ??= string.Empty;
            uzytkownikDto.usrNazwisko ??= string.Empty;
            uzytkownikDto.usrJezyk = string.IsNullOrWhiteSpace(uzytkownikDto.usrJezyk) ? "pl-PL" : uzytkownikDto.usrJezyk;
            uzytkownikDto.usrTelefon ??= string.Empty;
            uzytkownikDto.usrEmail ??= string.Empty;
            uzytkownikDto.usrStatus ??= "2";

            uzytkownikDto.usrInicjal = GenerateInitials(uzytkownikDto.usrImie, uzytkownikDto.usrNazwisko);
        }

        private static string GenerateInitials(string? firstName, string? lastName)
        {
            var culture = CultureInfo.GetCultureInfo("pl-PL");

            var initials = new[] { firstName, lastName }
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Select(s => culture.TextInfo.ToUpper(s!.Trim()[0].ToString()));

            return string.Concat(initials).PadRight(2).Substring(0, 2).Trim();
        }
    }
}
