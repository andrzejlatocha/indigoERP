using indigoERP.Api.Services;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace indigoERP.Api.Controllers
{
    // Polecenia służbowe dla wszystkich zalogowanych użytkowników indigoERP. Dane i PDF pochodzą z atacamaDMS.Api
    // (token serwisowy); indigoERP.Api tylko przekazuje wynik do Blazora.
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PolecenieSluzboweController(IAtacamaPolecenieClient atacama) : ControllerBase
    {
        // GET: api/PolecenieSluzbowe/SelectPubliczne?szukaj=
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PolecenieSluzboweViewModel>>> SelectPubliczne(string? szukaj = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var lista = await atacama.SelectPubliczneAsync(szukaj, cancellationToken);

                if (lista == null)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, "Brak tokenu do atacamaDMS.Api.");
                }

                return Ok(lista);
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Nie udało się pobrać poleceń z atacamaDMS.Api.");
            }
        }

        // GET: api/PolecenieSluzbowe/Pdf/5
        [HttpGet("{plsID}")]
        public async Task<IActionResult> Pdf(int plsID, CancellationToken cancellationToken = default)
        {
            try
            {
                var pdf = await atacama.PobierzPdfAsync(plsID, cancellationToken);

                if (pdf == null)
                {
                    return NotFound();
                }

                return File(pdf, "application/pdf", $"polecenie_{plsID}.pdf");
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Nie udało się pobrać PDF z atacamaDMS.Api.");
            }
        }
    }
}
