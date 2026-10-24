using indigoERP.Api.Data;
using indigoERP.Api.Services;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Entities._Wspolne;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace indigoERP.Api.Controllers._Wspolne
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class WiadomoscDokumentController : ControllerBase
    {
        private readonly indigoERPContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IWiadomoscMiniaturaService _miniatura;

        public WiadomoscDokumentController(
            indigoERPContext context, IWebHostEnvironment environment, IConfiguration configuration, IWiadomoscMiniaturaService miniatura)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
            _miniatura = miniatura;
        }

        // Załączniki wiadomości publicznej (Głos Hutnika) — bez autoryzacji, ale tylko jeśli
        // rodzic faktycznie ma wiaPubliczna=1, żeby nie dało się w ten sposób wyciągnąć
        // załączników prywatnych wiadomości systemowych (np. resetów hasła innych appek).
        [AllowAnonymous]
        [HttpGet("{wiaID}")]
        public async Task<ActionResult<IEnumerable<WiadomoscDokumentDto>>> SelectByWiadomoscPubliczna(int wiaID)
        {
            var jestPubliczna = await _context.Wiadomosc.AnyAsync(w => w.wiaID == wiaID && w.wiaPubliczna == true && w.wiaStatus == "2");

            if (!jestPubliczna)
            {
                return NotFound();
            }

            return await SelectByWiadomosc(wiaID);
        }

        [HttpGet("{wiaID}")]
        public async Task<ActionResult<IEnumerable<WiadomoscDokumentDto>>> SelectByWiadomosc(int wiaID)
        {
            return await _context.WiadomoscDokument
                .Where(w => w.wdoWiadomoscID == wiaID && w.wdoStatus == "2")
                .OrderBy(w => w.wdoNazwa)
                .Select(w => new WiadomoscDokumentDto
                {
                    wdoID = w.wdoID,
                    wdoWiadomoscID = w.wdoWiadomoscID,
                    wdoTyp = w.wdoTyp,
                    wdoNazwa = w.wdoNazwa,
                    wdoPlik = w.wdoPlik,
                    wdoKatalog = w.wdoKatalog,
                    wdoLink = w.wdoLink,
                    wdoStatus = w.wdoStatus
                })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<WiadomoscDokumentDto>> Upload(IFormFile file, [FromForm] int wdoWiadomoscID)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nie wybrano żadnego pliku.");
            }

            var fileName = Path.GetFileName(file.FileName);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("Nie wybrano żadnego pliku.");
            }

            var wiadomosc = await _context.Wiadomosc.FindAsync(wdoWiadomoscID);

            if (wiadomosc == null)
            {
                return NotFound("Nie znaleziono wiadomości.");
            }

            var extension = Path.GetExtension(fileName).TrimStart('.').ToLower();

            var storageRoot = GetStorageRoot();
            var appFolder = CleanPathSegment(User.FindFirstValue("appFolder"));
            var usrFolder = CleanPathSegment(User.FindFirstValue("usrFolder"));
            var timeFolder = DateTime.Now.ToString("yyyyMMddHHmmss");

            if (string.IsNullOrWhiteSpace(appFolder))
            {
                appFolder = "WIA";
            }

            if (string.IsNullOrWhiteSpace(usrFolder))
            {
                usrFolder = GetUsrID().ToString("D12");
            }

            var relativeFolder = Path.Combine(appFolder, usrFolder, timeFolder);
            var targetFolder = Path.Combine(storageRoot, relativeFolder);

            Directory.CreateDirectory(targetFolder);

            var targetPath = Path.Combine(targetFolder, fileName);

            await using (var stream = new FileStream(targetPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var dokument = new WiadomoscDokument
            {
                wdoWiadomoscID = wdoWiadomoscID,
                wdoTyp = extension.Length > 4 ? extension.Substring(0, 4) : extension,
                wdoNazwa = fileName,
                wdoPlik = fileName,
                wdoKatalog = relativeFolder,
                wdoStatus = "2"
            };

            _context.WiadomoscDokument.Add(dokument);
            await _context.SaveChangesAsync();

            return CreatedAtAction("SelectByWiadomosc", new { wiaID = wdoWiadomoscID }, dokument.Adapt<WiadomoscDokumentDto>());
        }

        [AllowAnonymous]
        [HttpGet("{wdoID}")]
        public async Task<IActionResult> OpenPubliczny(int wdoID)
        {
            var model = await PobierzPubliczny(wdoID);

            if (model == null)
            {
                return NotFound();
            }

            return ServeFile(model);
        }

        // Miniatura pierwszej strony PDF do kafelka Głosu Hutnika — tylko dla załączników publicznych wiadomości.
        [AllowAnonymous]
        [HttpGet("{wdoID}")]
        public async Task<IActionResult> MiniaturaPubliczna(int wdoID)
        {
            var model = await PobierzPubliczny(wdoID);

            if (model == null || !string.Equals(model.wdoTyp, "pdf", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            var path = ResolveStoragePath(model);

            if (path == null || !System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var miniatura = _miniatura.PobierzMiniature(path);

            if (miniatura == null)
            {
                return NotFound();
            }

            return PhysicalFile(miniatura, "image/jpeg");
        }

        private async Task<WiadomoscDokument?> PobierzPubliczny(int wdoID)
        {
            var model = await _context.WiadomoscDokument.FindAsync(wdoID);

            if (model == null || model.wdoStatus != "2" || model.wdoWiadomoscID == null)
            {
                return null;
            }

            var jestPubliczna = await _context.Wiadomosc.AnyAsync(w => w.wiaID == model.wdoWiadomoscID && w.wiaPubliczna == true && w.wiaStatus == "2");

            return jestPubliczna ? model : null;
        }

        [HttpGet("{wdoID}")]
        public async Task<IActionResult> Open(int wdoID)
        {
            var model = await _context.WiadomoscDokument.FindAsync(wdoID);

            if (model == null || model.wdoStatus != "2")
            {
                return NotFound();
            }

            return ServeFile(model);
        }

        private IActionResult ServeFile(WiadomoscDokument model)
        {
            var path = ResolveStoragePath(model);

            if (path == null || !System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return PhysicalFile(path, contentType, model.wdoNazwa, enableRangeProcessing: true);
        }

        [HttpDelete("{wdoID}")]
        public async Task<IActionResult> Delete(int wdoID)
        {
            var model = await _context.WiadomoscDokument.FindAsync(wdoID);

            if (model == null)
            {
                return NotFound();
            }

            model.wdoStatus = "1";
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int GetUsrID()
        {
            var value = User.FindFirstValue("usrID");

            return int.TryParse(value, out var usrID) ? usrID : 0;
        }

        private string GetStorageRoot()
        {
            var storageRoot = _configuration["FileStorage:Path"];

            if (string.IsNullOrWhiteSpace(storageRoot))
            {
                storageRoot = Path.Combine(_environment.ContentRootPath, "wwwroot", "_documents");
            }

            return Path.GetFullPath(storageRoot);
        }

        private string? ResolveStoragePath(WiadomoscDokument model)
        {
            if (string.IsNullOrWhiteSpace(model.wdoKatalog) || string.IsNullOrWhiteSpace(model.wdoPlik))
            {
                return null;
            }

            var storageRoot = GetStorageRoot();
            var normalizedPath = Path.Combine(model.wdoKatalog, model.wdoPlik).Replace("/", Path.DirectorySeparatorChar.ToString());
            var fullPath = Path.GetFullPath(Path.Combine(storageRoot, normalizedPath));

            return fullPath.StartsWith(storageRoot, StringComparison.OrdinalIgnoreCase) ? fullPath : null;
        }

        private string CleanPathSegment(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            var segment = new string(value
                .Where(w => !invalidChars.Contains(w) && w != Path.DirectorySeparatorChar && w != Path.AltDirectorySeparatorChar)
                .ToArray());

            return segment.Trim();
        }
    }
}
