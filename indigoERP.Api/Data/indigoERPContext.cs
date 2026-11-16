using epicaHR.Domain.Entities._Slownik;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Entities._Autoryzacja;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Domain.Common;
using indigoERP.Domain.Entities._Podpis;
using indigoERP.Domain.Enumerators._Administrator;
using indigoERP.Domain.Enumerators._Podpis;
using indigoERP.Domain.Enumerators._Slownik;
using indigoERP.Domain.Enumerators._Wspolne;
using indigoERP.Domain.ViewModels;
using indigoERP.Domain.ViewModels._Administracja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Domain.ViewModels._Podpis;
using indigoERP.Domain.ViewModels._Slownik;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Claims;

namespace indigoERP.Api.Data
{
    /// <summary>
    /// DBContext dla klas aplikacji
    /// </summary>
    public partial class indigoERPContext : DbContext
    {
        public indigoERPContext()
        {
        }

        private readonly IHttpContextAccessor? _httpContextAccessor;

        public indigoERPContext(DbContextOptions<indigoERPContext> options) : base(options)
        {
        }

        public indigoERPContext(DbContextOptions<indigoERPContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //Entities
        public virtual DbSet<Adres> Adres { get; set; }
        public virtual DbSet<Aplikacja> Aplikacja { get; set; }
        public virtual DbSet<AplikacjaDostep> AplikacjaDostep { get; set; }
        public virtual DbSet<AplikacjaUslugaUprawnienie> AplikacjaUslugaUprawnienie { get; set; }
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<BankAdres> BankAdres { get; set; }
        public virtual DbSet<Brygada> Brygada { get; set; }
        public virtual DbSet<DaneFirma> DaneFirma { get; set; }
        public virtual DbSet<DaneFirmaAdres> DaneFirmaAdres { get; set; }
        public virtual DbSet<DaneFirmaKontakt> DaneFirmaKontakt { get; set; }
        public virtual DbSet<Forum> Forum { get; set; }
        public virtual DbSet<ForumUzytkownik> ForumUzytkownik { get; set; }
        public virtual DbSet<Funkcja> Funkcja { get; set; }
        public virtual DbSet<Gmina> Gmina { get; set; }
        public virtual DbSet<Godzina> Godzina { get; set; }
        public virtual DbSet<Grupa> Grupa { get; set; }
        public virtual DbSet<GrupaCzlonkowie> GrupaCzlonkowie { get; set; }
        public virtual DbSet<JednostkaMiara> JednostkaMiara { get; set; }
        public virtual DbSet<Kraj> Kraj { get; set; }
        public virtual DbSet<Kontakt> Kontakt { get; set; }
        public virtual DbSet<Notatka> Notatka { get; set; }
        public virtual DbSet<MarkaPojazd> MarkaPojazd { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<Miejscowosc> Miejscowosc { get; set; }
        public virtual DbSet<Modul> Modul { get; set; }
        public virtual DbSet<ModulDostep> ModulDostep { get; set; }
        public virtual DbSet<Narodowosc> Narodowosc { get; set; }
        public virtual DbSet<Nowosc> Nowosc { get; set; }
        public virtual DbSet<NowoscUzytkownik> NowoscUzytkownik { get; set; }
        public virtual DbSet<Rola> Rola { get; set; }
        public virtual DbSet<LoginBlokada> LoginBlokada { get; set; }
        public virtual DbSet<Obywatelstwo> Obywatelstwo { get; set; }
        public virtual DbSet<Plec> Plec { get; set; }
        public virtual DbSet<Plik> Plik { get; set; }
        public virtual DbSet<Poczta> Poczta { get; set; }
        public virtual DbSet<Podpis> Podpis { get; set; }
        public virtual DbSet<PodpisUzytkownik> PodpisUzytkownik { get; set; }
        public virtual DbSet<PodpisGrupa> PodpisGrupa { get; set; }
        public virtual DbSet<Powiat> Powiat { get; set; }
        public virtual DbSet<RodzajDokumentTozsamosc> RodzajDokumentTozsamosc { get; set; }
        public virtual DbSet<RolaCzlonkowie> RolaCzlonkowie { get; set; }
        public virtual DbSet<RolaMenu> RolaMenu { get; set; }
        public virtual DbSet<Serwer> Serwer { get; set; }
        public virtual DbSet<SerwerLog> SerwerLog { get; set; }
        public virtual DbSet<SerwerTyp> SerwerTyp { get; set; }
        public virtual DbSet<TypKontakt> TypKontakt { get; set; }
        public virtual DbSet<TypPlik> TypPlik { get; set; }
        public virtual DbSet<TypPlikGrupa> TypPlikGrupa { get; set; }
        public virtual DbSet<UrzadSkarbowy> UrzadSkarbowy { get; set; }
        public virtual DbSet<UrzadSkarbowyAdres> UrzadSkarbowyAdres { get; set; }
        public virtual DbSet<UrzadSkarbowyKontakt> UrzadSkarbowyKontakt { get; set; }
        public virtual DbSet<Uzytkownik> Uzytkownik { get; set; }
        public virtual DbSet<UzytkownikGrupa> UzytkownikGrupa { get; set; }
        public virtual DbSet<Wiadomosc> Wiadomosc { get; set; }
        public virtual DbSet<WiadomoscPracownik> WiadomoscPracownik { get; set; }
        public virtual DbSet<WiadomoscUzytkownik> WiadomoscUzytkownik { get; set; }
        public virtual DbSet<WiadomoscWzorzec> WiadomoscWzorzec { get; set; }
        public virtual DbSet<WiadomoscDokument> WiadomoscDokument { get; set; }
        public virtual DbSet<WiadomoscWysylka> WiadomoscWysylka { get; set; }
        public virtual DbSet<Wojewodztwo> Wojewodztwo { get; set; }
        public virtual DbSet<Zadanie> Zadanie { get; set; }
        public virtual DbSet<ZadanieUzytkownik> ZadanieUzytkownik { get; set; }
        public virtual DbSet<Zdarzenie> Zdarzenie { get; set; }
        public virtual DbSet<ZdarzenieUzytkownik> ZdarzenieUzytkownik { get; set; }

        // Enumerators
        public virtual DbSet<AplikacjaEnumerator> AplikacjaEnumerator { get; set; }
        public virtual DbSet<BankEnumerator> BankEnumerator { get; set; }
        public virtual DbSet<GminaEnumerator> GminaEnumerator { get; set; }
        public virtual DbSet<GodzinaEnumerator> GodzinaEnumerator { get; set; }
        public virtual DbSet<JednostkaMiaraEnumerator> JednostkaMiaraEnumerator { get; set; }
        public virtual DbSet<KrajEnumerator> KrajEnumerator { get; set; }
        public virtual DbSet<MarkaPojazdEnumerator> MarkaPojazdEnumerator { get; set; }
        public virtual DbSet<MiejscowoscEnumerator> MiejscowoscEnumerator { get; set; }
        public virtual DbSet<NarodowoscEnumerator> NarodowoscEnumerator { get; set; }
        public virtual DbSet<ObywatelstwoEnumerator> ObywatelstwoEnumerator { get; set; }
        public virtual DbSet<PlecEnumerator> PlecEnumerator { get; set; }
        public virtual DbSet<PocztaEnumerator> PocztaEnumerator { get; set; }
        public virtual DbSet<PodpisEnumerator> PodpisEnumerator { get; set; }
        public virtual DbSet<WiadomoscWzorzecEnumerator> WiadomoscWzorzecEnumerator { get; set; }
        public virtual DbSet<WiadomoscZakresEnumerator> WiadomoscZakresEnumerator { get; set; }
        public virtual DbSet<PowiatEnumerator> PowiatEnumerator { get; set; }
        public virtual DbSet<PracownikEnumerator> PracownikEnumerator { get; set; }
        public virtual DbSet<RodzajDokumentTozsamoscEnumerator> RodzajDokumentTozsamoscEnumerator { get; set; }
        public virtual DbSet<SerwerTypEnumerator> SerwerTypEnumerator { get; set; }
        public virtual DbSet<TypNowoscEnumerator> TypNowoscEnumerator { get; set; }
        public virtual DbSet<TypKontaktEnumerator> TypKontaktEnumerator { get; set; }
        public virtual DbSet<TypPlikEnumerator> TypPlikEnumerator { get; set; }
        public virtual DbSet<UrzadSkarbowyEnumerator> UrzadSkarbowyEnumerator { get; set; }
        public virtual DbSet<WalutaEnumerator> WalutaEnumerator { get; set; }
        public virtual DbSet<WojewodztwoEnumerator> WojewodztwoEnumerator { get; set; }

        //Models


        //ViewModels
        public virtual DbSet<AplikacjaViewModel> AplikacjaViewModel { get; set; }
        public virtual DbSet<AplikacjaUzytkownikViewModel> AplikacjaUzytkownikViewModel { get; set; }
        public virtual DbSet<AplikacjaUslugaUprawnienieViewModel> AplikacjaUslugaUprawnienieViewModel { get; set; }
        public virtual DbSet<DaneFirmaAdresViewModel> DaneFirmaAdresViewModel { get; set; }
        public virtual DbSet<DaneFirmaKontaktViewModel> DaneFirmaKontaktViewModel { get; set; }
        public virtual DbSet<GrupaViewModel> GrupaViewModel { get; set; }
        public virtual DbSet<GminaViewModel> GminaViewModel { get; set; }
        public virtual DbSet<GrupaAplikacjaViewModel> GrupaAplikacjaViewModel { get; set; }
        public virtual DbSet<GrupaRolaViewModel> GrupaRolaViewModel { get; set; }
        public virtual DbSet<KrajViewModel> KrajViewModel { get; set; }
        public virtual DbSet<NotatkaViewModel> NotatkaViewModel { get; set; }
        public virtual DbSet<NowoscViewModel> NowoscViewModel { get; set; }
        public virtual DbSet<NowoscUzytkownikViewModel> NowoscUzytkownikViewModel { get; set; }
        public virtual DbSet<MarkaPojazdViewModel> MarkaPojazdViewModel { get; set; }
        public virtual DbSet<MiejscowoscViewModel> MiejscowoscViewModel { get; set; }
        public virtual DbSet<PocztaViewModel> PocztaViewModel { get; set; }
        public virtual DbSet<PodpisViewModel> PodpisViewModel { get; set; }
        public virtual DbSet<PodpisUzytkownikViewModel> PodpisUzytkownikViewModel { get; set; }
        public virtual DbSet<PodpisGrupaViewModel> PodpisGrupaViewModel { get; set; }
        public virtual DbSet<PowiatViewModel> PowiatViewModel { get; set; }
        public virtual DbSet<RolaViewModel> RolaViewModel { get; set; }
        public virtual DbSet<RolaAplikacjaViewModel> RolaAplikacjaViewModel { get; set; }
        public virtual DbSet<SerwerViewModel> SerwerViewModel { get; set; }
        public virtual DbSet<MonitorSerwerStatusViewModel> MonitorSerwerStatusViewModel { get; set; }
        public virtual DbSet<UzytkownikAplikacjaViewModel> UzytkownikAplikacjaViewModel { get; set; }
        public virtual DbSet<UzytkownikAplikacjaDostepViewModel> UzytkownikAplikacjaDostepViewModel { get; set; }
        public virtual DbSet<UzytkownikGrupaViewModel> UzytkownikGrupaViewModel { get; set; }
        public virtual DbSet<UzytkownikListaViewModel> UzytkownikListaViewModel { get; set; }
        public virtual DbSet<UzytkownikLoginViewModel> UzytkownikLoginViewModel { get; set; }
        public virtual DbSet<UzytkownikRolaViewModel> UzytkownikRolaViewModel { get; set; }
        public virtual DbSet<UzytkownikRolaEfektywnaViewModel> UzytkownikRolaEfektywnaViewModel { get; set; }
        public virtual DbSet<UzytkownikViewModel> UzytkownikViewModel { get; set; }
        public virtual DbSet<WiadomoscViewModel> WiadomoscViewModel { get; set; }
        public virtual DbSet<WiadomoscPracownikViewModel> WiadomoscPracownikViewModel { get; set; }
        public virtual DbSet<WiadomoscZakresPracownikViewModel> WiadomoscZakresPracownikViewModel { get; set; }
        public virtual DbSet<WiadomoscUzytkownikViewModel> WiadomoscUzytkownikViewModel { get; set; }
        public virtual DbSet<PowiadomienieViewModel> PowiadomienieViewModel { get; set; }
        public virtual DbSet<WiadomoscWzorzecViewModel> WiadomoscWzorzecViewModel { get; set; }
        public virtual DbSet<WiadomoscHistoriaViewModel> WiadomoscHistoriaViewModel { get; set; }
        public virtual DbSet<WiadomoscPublicznaViewModel> WiadomoscPublicznaViewModel { get; set; }
        public virtual DbSet<WojewodztwoViewModel> WojewodztwoViewModel { get; set; }
        public virtual DbSet<WalutaViewModel> WalutaViewModel { get; set; }
        public virtual DbSet<ZadanieViewModel> ZadanieViewModel { get; set; }
        public virtual DbSet<ZdarzenieViewModel> ZdarzenieViewModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=indigoERPConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Polish_CI_AS");
            modelBuilder.Entity<Aplikacja>().ToTable("Aplikacja", "dbo");
            modelBuilder.Entity<AplikacjaUslugaUprawnienie>().ToTable("AplikacjaUslugaUprawnienie", "app");
            modelBuilder.Entity<Grupa>().ToTable("Grupa", "dbo");
            modelBuilder.Entity<GrupaCzlonkowie>().ToTable("GrupaCzlonkowie", "dbo");
            modelBuilder.Entity<Rola>().ToTable("Rola", "dbo");
            modelBuilder.Entity<RolaCzlonkowie>().ToTable("RolaCzlonkowie", "dbo");
            modelBuilder.Entity<Serwer>().ToTable("Serwer", "dbo");
            modelBuilder.Entity<SerwerLog>().ToTable("SerwerLog", "dbo");
            var uzytkownik = modelBuilder.Entity<Uzytkownik>();
            uzytkownik.ToTable("Uzytkownik", "dbo");

            var usrImieNazwisko = uzytkownik.Property(p => p.usrImieNazwisko).ValueGeneratedOnAddOrUpdate();
            usrImieNazwisko.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            usrImieNazwisko.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override int SaveChanges()
        {
            ApplyAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditFields()
        {
            var now = DateTime.Now;
            var currentUserId = GetCurrentUserId();

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.DataUtworzenie ??= now;
                    entry.Entity.DataModyfikacja ??= now;
                    entry.Entity.UtworzenieID ??= currentUserId;
                    entry.Entity.ModyfikacjaID ??= currentUserId;
                    entry.Entity.Usuniety ??= false;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.DataModyfikacja = now;
                    entry.Entity.ModyfikacjaID = currentUserId ?? entry.Entity.ModyfikacjaID;
                    entry.Property(p => p.DataUtworzenie).IsModified = false;
                    entry.Property(p => p.UtworzenieID).IsModified = false;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.DataModyfikacja = now;
                    entry.Entity.ModyfikacjaID = currentUserId ?? entry.Entity.ModyfikacjaID;
                    entry.Entity.Usuniety = true;
                    entry.Property(p => p.DataUtworzenie).IsModified = false;
                    entry.Property(p => p.UtworzenieID).IsModified = false;
                }
            }
        }

        private int? GetCurrentUserId()
        {
            var value = _httpContextAccessor?.HttpContext?.User?.FindFirst("usrID")?.Value
                ?? _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(value, out var userId) ? userId : null;
        }
    }
}
