using epicaHR.Domain.Entities._Slownik;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Entities._Slownik;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Domain.Enumerators._Slownik;
using indigoERP.Domain.ViewModels;
using indigoERP.Domain.ViewModels._Administracja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using indigoERP.Domain.ViewModels._Slownik;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.EntityFrameworkCore;

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

        public indigoERPContext(DbContextOptions<indigoERPContext> options) : base(options)
        {
        }

        //Entities
        public virtual DbSet<Adres> Adres { get; set; }
        public virtual DbSet<Aplikacja> Aplikacja { get; set; }
        public virtual DbSet<AplikacjaDostep> AplikacjaDostep { get; set; }
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
        public virtual DbSet<Obywatelstwo> Obywatelstwo { get; set; }
        public virtual DbSet<Plec> Plec { get; set; }
        public virtual DbSet<Plik> Plik { get; set; }
        public virtual DbSet<Poczta> Poczta { get; set; }
        public virtual DbSet<Powiat> Powiat { get; set; }
        public virtual DbSet<RodzajDokumentTozsamosc> RodzajDokumentTozsamosc { get; set; }
        public virtual DbSet<RolaCzlonkowie> RolaCzlonkowie { get; set; }
        public virtual DbSet<RolaMenu> RolaMenu { get; set; }
        public virtual DbSet<TypKontakt> TypKontakt { get; set; }
        public virtual DbSet<TypPlik> TypPlik { get; set; }
        public virtual DbSet<UrzadSkarbowy> UrzadSkarbowy { get; set; }
        public virtual DbSet<UrzadSkarbowyAdres> UrzadSkarbowyAdres { get; set; }
        public virtual DbSet<UrzadSkarbowyKontakt> UrzadSkarbowyKontakt { get; set; }
        public virtual DbSet<Uzytkownik> Uzytkownik { get; set; }
        public virtual DbSet<UzytkownikGrupa> UzytkownikGrupa { get; set; }
        public virtual DbSet<Wiadomosc> Wiadomosc { get; set; }
        public virtual DbSet<Wojewodztwo> Wojewodztwo { get; set; }
        public virtual DbSet<Zadanie> Zadanie { get; set; }
        public virtual DbSet<Zdarzenie> Zdarzenie { get; set; }

        // Enumerators
        public virtual DbSet<BankEnumerator> BankEnumerator { get; set; }
        public virtual DbSet<GminaEnumerator> GminaEnumerator { get; set; }
        public virtual DbSet<GodzinaEnumerator> GodzinaEnumerator { get; set; }
        public virtual DbSet<KrajEnumerator> KrajEnumerator { get; set; }
        public virtual DbSet<MarkaPojazdEnumerator> MarkaPojazdEnumerator { get; set; }
        public virtual DbSet<MiejscowoscEnumerator> MiejscowoscEnumerator { get; set; }
        public virtual DbSet<NarodowoscEnumerator> NarodowoscEnumerator { get; set; }
        public virtual DbSet<ObywatelstwoEnumerator> ObywatelstwoEnumerator { get; set; }
        public virtual DbSet<PlecEnumerator> PlecEnumerator { get; set; }
        public virtual DbSet<PocztaEnumerator> PocztaEnumerator { get; set; }
        public virtual DbSet<PowiatEnumerator> PowiatEnumerator { get; set; }
        public virtual DbSet<RodzajDokumentTozsamoscEnumerator> RodzajDokumentTozsamoscEnumerator { get; set; }
        public virtual DbSet<TypKontaktEnumerator> TypKontaktEnumerator { get; set; }
        public virtual DbSet<TypPlikEnumerator> TypPlikEnumerator { get; set; }
        public virtual DbSet<UrzadSkarbowyEnumerator> UrzadSkarbowyEnumerator { get; set; }
        public virtual DbSet<WojewodztwoEnumerator> WojewodztwoEnumerator { get; set; }

        //Models


        //ViewModels
        public virtual DbSet<AplikacjaViewModel> AplikacjaViewModel { get; set; }
        public virtual DbSet<AplikacjaUzytkownikViewModel> AplikacjaUzytkownikViewModel { get; set; }
        public virtual DbSet<DaneFirmaAdresViewModel> DaneFirmaAdresViewModel { get; set; }
        public virtual DbSet<DaneFirmaKontaktViewModel> DaneFirmaKontaktViewModel { get; set; }
        public virtual DbSet<GminaViewModel> GminaViewModel { get; set; }
        public virtual DbSet<GrupaAplikacjaViewModel> GrupaAplikacjaViewModel { get; set; }
        public virtual DbSet<GrupaRolaViewModel> GrupaRolaViewModel { get; set; }
        public virtual DbSet<KrajViewModel> KrajViewModel { get; set; }
        public virtual DbSet<NotatkaViewModel> NotatkaViewModel { get; set; }
        public virtual DbSet<MarkaPojazdViewModel> MarkaPojazdViewModel { get; set; }
        public virtual DbSet<MiejscowoscViewModel> MiejscowoscViewModel { get; set; }
        public virtual DbSet<PocztaViewModel> PocztaViewModel { get; set; }
        public virtual DbSet<PowiatViewModel> PowiatViewModel { get; set; }
        public virtual DbSet<RolaAplikacjaViewModel> RolaAplikacjaViewModel { get; set; }
        public virtual DbSet<UzytkownikAplikacjaViewModel> UzytkownikAplikacjaViewModel { get; set; }
        public virtual DbSet<UzytkownikGrupaViewModel> UzytkownikGrupaViewModel { get; set; }
        public virtual DbSet<UzytkownikRolaViewModel> UzytkownikRolaViewModel { get; set; }
        public virtual DbSet<UzytkownikViewModel> UzytkownikViewModel { get; set; }
        public virtual DbSet<WiadomoscViewModel> WiadomoscViewModel { get; set; }
        public virtual DbSet<WojewodztwoViewModel> WojewodztwoViewModel { get; set; }
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
