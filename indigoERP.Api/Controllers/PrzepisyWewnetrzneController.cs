using indigoERP.Api.Services;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace indigoERP.Api.Controllers
{
    // Przepisy wewnętrzne (biblioteka 13 w atacamaDMS) dla wszystkich zalogowanych użytkowników indigoERP.
    // Dane i PDF pochodzą z atacamaDMS.Api (token serwisowy); indigoERP.Api tylko przekazuje wynik do Blazora.
    // Trasa /api/PrzepisyWewnetrzne/... wynika z nazwy kontrolera (konwencja api/[controller]/[action]).
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PrzepisyWewnetrzneController(IAtacamaPolecenieClient atacama) : ControllerBase
    {
        // GET: api/PrzepisyWewnetrzne/SelectPubliczne?szukaj=
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrzepisWewnetrznyViewModel>>> SelectPubliczne(string? szukaj = null, int? katalogID = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var lista = await atacama.SelectPubliczneAsync(szukaj, katalogID, cancellationToken);

                if (lista == null)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, "Brak tokenu do atacamaDMS.Api.");
                }

                return Ok(lista);
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Nie udało się pobrać przepisów z atacamaDMS.Api.");
            }
        }

        // GET: api/PrzepisyWewnetrzne/SelectKatalogi - foldery pierwszego poziomu HUTY (combo).
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KatalogWyborViewModel>>> SelectKatalogi(CancellationToken cancellationToken = default)
        {
            try
            {
                var lista = await atacama.SelectKatalogiAsync(cancellationToken);

                if (lista == null)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, "Brak tokenu do atacamaDMS.Api.");
                }

                return Ok(lista);
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Nie udało się pobrać folderów z atacamaDMS.Api.");
            }
        }

        // GET: api/PolecenieSluzbowe/Pdf/5
        [HttpGet("{pliID}")]
        public async Task<IActionResult> Pdf(int pliID, CancellationToken cancellationToken = default)
        {
            try
            {
                var pdf = await atacama.PobierzPdfAsync(pliID, cancellationToken);

                if (pdf == null)
                {
                    return NotFound();
                }

                return File(pdf, "application/pdf", $"przepis_{pliID}.pdf");
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Nie udało się pobrać PDF z atacamaDMS.Api.");
            }
        }
    }
}
