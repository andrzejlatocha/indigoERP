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
            return await BuildAdministrationUserQuery()
                .Where(w => w.usrStatus == usrStatus || usrStatus == "*")
                .ToListAsync();
        }

        [AllowAnonymous]
        [HttpGet("{rola}")]
        public async Task<ActionResult<IEnumerable<UzytkownikViewModel>>> SelectByRola(string rola, string usrStatus)
        {
            return await _context.UzytkownikViewModel
                .FromSqlInterpolated($"""
                    WITH userIds AS
                    (
                        SELECT DISTINCT
                            usrID
                        FROM mvw.vUzytkownikRolaLista
                        WHERE srlNazwa = {rola}
                            AND (usrStatus = {usrStatus} OR {usrStatus} = '*')
                        UNION
                        SELECT DISTINCT
                            c.cwrUzytkownikID AS usrID
                        FROM dbo.RolaCzlonkowie AS c
                        INNER JOIN dbo.Rola AS r ON c.cwrRolaID = r.srlID
                        INNER JOIN dbo.Uzytkownik AS u ON c.cwrUzytkownikID = u.usrID
                        WHERE c.cwrUzytkownikID IS NOT NULL
                            AND r.srlNazwa = {rola}
                            AND (c.cwrStatus = '2' OR c.cwrStatus IS NULL)
                            AND (r.srlStatus = '2' OR r.srlStatus IS NULL)
                            AND ISNULL(u.Usuniety, 0) = 0
                            AND (u.usrStatus = {usrStatus} OR {usrStatus} = '*')
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
            var inheritedRoles = await _context.UzytkownikRolaViewModel
                .Where(w => (w.appID == appID || appID == -1) && w.usrID == usrID && (w.srlStatus == srlStatus || srlStatus == "*"))
                .ToListAsync();

            var directRoles = await _context.RolaCzlonkowie
                .Where(w => w.cwrUzytkownikID == usrID && w.cwrRolaID != null && (w.cwrStatus == "2" || w.cwrStatus == null))
                .Join(_context.Uzytkownik,
                    c => c.cwrUzytkownikID,
                    u => u.usrID,
                    (c, u) => new { Czlonek = c, Uzytkownik = u })
                .Join(_context.Rola,
                    cu => cu.Czlonek.cwrRolaID,
                    r => r.srlID,
                    (cu, r) => new UzytkownikRolaViewModel
                    {
                        cwrID = cu.Czlonek.cwrID,
                        appID = r.srlAplikacjaID ?? 0,
                        usrID = usrID,
                        usrNazwa = cu.Uzytkownik.usrNazwa,
                        usrNazwaAD = null,
                        usrStatus = cu.Uzytkownik.usrStatus,
                        srlID = r.srlID,
                        srlNazwa = r.srlNazwa,
                        srlOpis = r.srlOpis,
                        srlStatus = r.srlStatus,
                        uwgPoziom = 0,
                        uwgDziedziczy = false
                    })
                .Where(w => (w.appID == appID || appID == -1) && (w.srlStatus == srlStatus || srlStatus == "*"))
                .ToListAsync();

            return inheritedRoles
                .Concat(directRoles)
                .GroupBy(g => new { g.cwrID, g.usrID, g.srlID })
                .Select(s => s.First())
                .ToList();
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

        private IQueryable<UzytkownikViewModel> BuildAdministrationUserQuery(int? appID = null)
        {
            var appIdParameter = appID.HasValue ? (object)appID.Value : DBNull.Value;

            var details = _context.UzytkownikViewModel
                .FromSqlRaw("""
                    WITH details AS
                    (
                        SELECT
                            usrID,
                            usrPracownikID,
                            prcKod,
                            usrNazwa,
                            usrImie,
                            usrNazwisko,
                            usrInicjal,
                            usrTelefon,
                            usrKomorka,
                            usrEmail,
                            CAST(NULL AS int) AS usrKlientID,
                            CAST(NULL AS int) AS usrDostawcaID,
                            CAST(NULL AS int) AS usrOdbiorcaID,
                            CAST(NULL AS int) AS usrSpedytorID,
                            appID,
                            appFolder,
                            usrFolder,
                            usrZdjecie,
                            wdzID,
                            wdzKod,
                            wdzSymbol,
                            wdzNazwa,
                            stnKod,
                            stnSymbol,
                            stnNazwa,
                            usrDataWaznoscHaslo,
                            srlID,
                            srlNazwa,
                            usrGuid,
                            usrToken,
                            CAST(NULL AS uniqueidentifier) AS sesGuid,
                            ROW_NUMBER() OVER (PARTITION BY usrID ORDER BY usrID) AS rowNumber
                        FROM mvw.vUzytkownik
                        WHERE ({0} IS NULL OR appID = {0})
                    )
                    SELECT
                        usrID,
                        usrPracownikID,
                        prcKod,
                        usrNazwa,
                        usrImie,
                        usrNazwisko,
                        usrInicjal,
                        usrTelefon,
                        usrKomorka,
                        usrEmail,
                        usrKlientID,
                        usrDostawcaID,
                        usrOdbiorcaID,
                        usrSpedytorID,
                        appID,
                        appFolder,
                        usrFolder,
                        usrZdjecie,
                        wdzID,
                        wdzKod,
                        wdzSymbol,
                        wdzNazwa,
                        stnKod,
                        stnSymbol,
                        stnNazwa,
                        usrDataWaznoscHaslo,
                        srlID,
                        srlNazwa,
                        usrGuid,
                        usrToken,
                        sesGuid
                    FROM details
                    WHERE rowNumber = 1
                    """, appIdParameter)
                .AsNoTracking();

            return from u in _context.Uzytkownik.AsNoTracking()
                   join d in details on u.usrID equals d.usrID into userDetails
                   from d in userDetails.DefaultIfEmpty()
                   where u.Usuniety != true
                   select new UzytkownikViewModel
                   {
                       usrID = u.usrID,
                       usrPracownikID = u.usrPracownikID ?? 0,
                       prcKod = d.prcKod,
                       usrNazwa = u.usrNazwa,
                       usrImie = u.usrImie,
                       usrNazwisko = u.usrNazwisko,
                       usrImieNazwisko = u.usrImieNazwisko,
                       usrInicjal = u.usrInicjal,
                       usrJezyk = u.usrJezyk,
                       usrTelefon = u.usrTelefon,
                       usrKomorka = d.usrKomorka,
                       usrEmail = u.usrEmail,
                       usrDataWaznoscHaslo = d.usrDataWaznoscHaslo ?? u.usrDataWaznoscHaslo ?? new DateTime(2099, 12, 31),
                       usrNowy = u.usrNowy,
                       usrBlokada = u.usrBlokada,
                       usrEdycja = u.usrEdycja,
                       usrStatus = u.usrStatus,
                       appID = (int?)d.appID ?? 0,
                       appFolder = d.appFolder,
                       usrFolder = d.usrFolder,
                       usrZdjecie = d.usrZdjecie,
                       wdzID = d.wdzID,
                       wdzKod = d.wdzKod,
                       wdzSymbol = d.wdzSymbol,
                       wdzNazwa = d.wdzNazwa,
                       stnKod = d.stnKod,
                       stnSymbol = d.stnSymbol,
                       stnNazwa = d.stnNazwa,
                       srlID = d.srlID,
                       srlNazwa = d.srlNazwa,
                       usrGuid = d.usrGuid,
                       usrToken = d.usrToken
                   };
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
