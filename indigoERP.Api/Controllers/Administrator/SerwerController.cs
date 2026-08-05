using indigoERP.Api.Data;
using indigoERP.Api.Services;
using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.ViewModels._Administracja;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Controllers._Administrator
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SerwerController : ControllerBase
    {
        private const string AdministratorRole = "HCZ-indigoERP-Administrator";
        private const string BearerScheme = "Bearer";

        // SQL Server 2005+ (w tym 2008, 2008 R2, 2012 ... 2022) — OUTER APPLY / TOP (n) są dostępne od 2005.
        private const string ModernJobsQuery = @"
            SELECT j.name AS JobName, j.enabled AS Enabled, h.run_status, h.run_date, h.run_time
            FROM msdb.dbo.sysjobs j
            OUTER APPLY (
                SELECT TOP (1) run_status, run_date, run_time
                FROM msdb.dbo.sysjobhistory
                WHERE job_id = j.job_id AND step_id = 0
                ORDER BY run_date DESC, run_time DESC
            ) h
            WHERE j.name LIKE 'ujb%' OR j.name LIKE 'MaintenancePlan%'
            ORDER BY j.name";

        // SQL Server 2000 (ProductMajorVersion <= 8) — brak APPLY, więc "ostatni wpis historii" liczony
        // przez LEFT JOIN + skorelowany podzapytanie MAX zamiast APPLY/TOP(1)/ROW_NUMBER (też niedostępne w 2000).
        private const string Sql2000JobsQuery = @"
            SELECT j.name AS JobName, j.enabled AS Enabled, h.run_status, h.run_date, h.run_time
            FROM msdb.dbo.sysjobs j
            LEFT JOIN msdb.dbo.sysjobhistory h
                ON h.job_id = j.job_id
                AND h.step_id = 0
                AND CONVERT(bigint, h.run_date) * 1000000 + h.run_time = (
                    SELECT MAX(CONVERT(bigint, h2.run_date) * 1000000 + h2.run_time)
                    FROM msdb.dbo.sysjobhistory h2
                    WHERE h2.job_id = j.job_id AND h2.step_id = 0
                )
            WHERE j.name LIKE 'ujb%' OR j.name LIKE 'MaintenancePlan%'
            ORDER BY j.name";

        private readonly indigoERPContext _context;
        private readonly IConfiguration _configuration;
        private readonly IServerCheckService _checkService;

        public SerwerController(indigoERPContext context, IConfiguration configuration, IServerCheckService checkService)
        {
            _context = context;
            _configuration = configuration;
            _checkService = checkService;
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<IEnumerable<SerwerViewModel>>> Select(string srvStatus)
        {
            return await _context.SerwerViewModel
                .Where(w => w.srvStatus == srvStatus || srvStatus == "*")
                .ToListAsync();
        }

        [HttpGet("{srvID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<SerwerDto>> Get(int srvID)
        {
            var serwer = await _context.Serwer.FindAsync(srvID);

            if (serwer == null)
            {
                return NotFound();
            }

            return serwer.Adapt<SerwerDto>();
        }

        [HttpPost]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<SerwerDto>> Insert(SerwerDto serwerDto)
        {
            serwerDto.srvStatus ??= "2";

            var serwer = serwerDto.Adapt<Serwer>();

            _context.Serwer.Add(serwer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { serwer.srvID }, serwer.Adapt<SerwerDto>());
        }

        [HttpPut("{srvID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Update(int srvID, SerwerDto serwerDto)
        {
            if (srvID != serwerDto.srvID)
            {
                return BadRequest();
            }

            var serwer = await _context.Serwer.FindAsync(srvID);

            if (serwer == null)
            {
                return NotFound();
            }

            serwerDto.Adapt(serwer);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SerwerExists(srvID))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{srvID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<IActionResult> Delete(int srvID)
        {
            var serwer = await _context.Serwer.FindAsync(srvID);

            if (serwer == null)
            {
                return NotFound();
            }

            serwer.Usuniety = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Informacja o środowisku (wersja API / serwer SQL / baza) — dostępna dla każdego zalogowanego użytkownika,
        // wykorzystywana przez panel w stopce.
        [HttpGet]
        [Authorize(AuthenticationSchemes = BearerScheme)]
        public ActionResult<SerwerInfoViewModel> GetInfo()
        {
            var connectionString = _configuration.GetConnectionString("indigoERPConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return Problem("Brak connection stringa indigoERPConnection.", statusCode: StatusCodes.Status500InternalServerError);
            }

            SqlConnectionStringBuilder builder;

            try
            {
                builder = new SqlConnectionStringBuilder(connectionString);
            }
            catch (ArgumentException)
            {
                return Problem("Connection string indigoERPConnection ma nieprawidłowy format.", statusCode: StatusCodes.Status500InternalServerError);
            }

            return Ok(new SerwerInfoViewModel
            {
                WersjaApi = _configuration["appSettings:Version"],
                SerwerSql = builder.DataSource,
                BazaDanych = builder.InitialCatalog
            });
        }

        [HttpGet]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<IEnumerable<MonitorSerwerStatusViewModel>>> SelectStatus()
        {
            return await _context.MonitorSerwerStatusViewModel.ToListAsync();
        }

        [HttpGet("{srvID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<MonitorSerwerStatusViewModel>> Check(int srvID)
        {
            var serwer = await _context.Serwer.FindAsync(srvID);

            if (serwer == null)
            {
                return NotFound();
            }

            var (isAlive, responseMs, error) = await _checkService.CheckAsync(serwer.srvSerwerTypKOD, serwer.srvAdres, serwer.srvPort, HttpContext.RequestAborted);

            _context.SerwerLog.Add(new SerwerLog
            {
                slgSerwerID = serwer.srvID,
                slgDataSprawdzenie = DateTime.Now,
                slgDostepny = isAlive,
                slgCzasOdpowiedzMs = responseMs,
                slgBlad = error
            });

            await _context.SaveChangesAsync();

            var status = await _context.MonitorSerwerStatusViewModel.FirstOrDefaultAsync(w => w.srvID == srvID);

            return status == null ? NotFound() : status;
        }

        // Lista JOB-ów SQL Agenta dla wybranego serwera SQL (msdb, logowanie SQL — Integrated Security
        // nie działa przez VPN, więc używamy wspólnego loginu appSettings:MonitoringSqlLogin/Password).
        [HttpGet("{srvID}")]
        [Authorize(Roles = AdministratorRole, AuthenticationSchemes = BearerScheme)]
        public async Task<ActionResult<IEnumerable<SqlAgentJobViewModel>>> SelectJoby(int srvID)
        {
            var serwer = await _context.Serwer.FindAsync(srvID);

            if (serwer == null)
            {
                return NotFound();
            }

            if (serwer.srvSerwerTypKOD != "SQL")
            {
                return BadRequest("Lista JOB-ów jest dostępna tylko dla serwerów typu SQL.");
            }

            var sqlLogin = _configuration["appSettings:MonitoringSqlLogin"];
            var sqlPassword = _configuration["appSettings:MonitoringSqlPassword"];

            if (string.IsNullOrWhiteSpace(sqlLogin) || string.IsNullOrWhiteSpace(sqlPassword))
            {
                return Problem(
                    "Brak konfiguracji appSettings:MonitoringSqlLogin / MonitoringSqlPassword.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            var dataSource = serwer.srvPort.HasValue ? $"{serwer.srvAdres},{serwer.srvPort}" : serwer.srvAdres;
            var connectionString = $"Data Source={dataSource};Initial Catalog=msdb;User Id={sqlLogin};Password={sqlPassword};TrustServerCertificate=True;Connect Timeout=5";

            var joby = new List<SqlAgentJobViewModel>();

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var majorVersion = await GetServerMajorVersionAsync(connection);
                var queryText = majorVersion <= 8 ? Sql2000JobsQuery : ModernJobsQuery;

                using var command = new SqlCommand(queryText, connection);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var runStatus = reader.IsDBNull(reader.GetOrdinal("run_status")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("run_status"));
                    var runDate = reader.IsDBNull(reader.GetOrdinal("run_date")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("run_date"));
                    var runTime = reader.IsDBNull(reader.GetOrdinal("run_time")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("run_time"));

                    joby.Add(new SqlAgentJobViewModel
                    {
                        JobName = reader.GetString(reader.GetOrdinal("JobName")),
                        Enabled = reader.GetBoolean(reader.GetOrdinal("Enabled")),
                        OstatniStatus = MapRunStatus(runStatus),
                        OstatnieUruchomienie = ParseRunDateTime(runDate, runTime)
                    });
                }
            }
            catch (Exception ex)
            {
                return Problem(
                    $"Nie udało się połączyć z serwerem SQL '{serwer.srvNazwa}' ({serwer.srvAdres}): {ex.Message}",
                    statusCode: StatusCodes.Status502BadGateway);
            }

            return joby;
        }

        private static async Task<int> GetServerMajorVersionAsync(SqlConnection connection)
        {
            using var command = new SqlCommand("SELECT CAST(SERVERPROPERTY('ProductMajorVersion') AS int)", connection);
            var result = await command.ExecuteScalarAsync();

            return result is int version ? version : int.MaxValue;
        }

        private static string MapRunStatus(int? runStatus) => runStatus switch
        {
            0 => "Błąd",
            1 => "Sukces",
            2 => "Ponawianie",
            3 => "Anulowano",
            4 => "W trakcie",
            _ => "Brak historii"
        };

        private static DateTime? ParseRunDateTime(int? runDate, int? runTime)
        {
            if (runDate is not > 0)
            {
                return null;
            }

            var timeValue = (runTime ?? 0).ToString().PadLeft(6, '0');
            var dateValue = runDate.Value.ToString();

            var hour = int.Parse(timeValue[..2]);
            var minute = int.Parse(timeValue.Substring(2, 2));
            var second = int.Parse(timeValue.Substring(4, 2));

            var year = int.Parse(dateValue[..4]);
            var month = int.Parse(dateValue.Substring(4, 2));
            var day = int.Parse(dateValue.Substring(6, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }

        private bool SerwerExists(int srvID)
        {
            return _context.Serwer.Any(e => e.srvID == srvID);
        }
    }
}
