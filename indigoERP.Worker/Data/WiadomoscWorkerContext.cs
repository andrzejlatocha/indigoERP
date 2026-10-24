using indigoERP.Domain.Entities._Wspolne;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Worker.Data;

public sealed class WiadomoscWorkerContext : DbContext
{
    public WiadomoscWorkerContext(DbContextOptions<WiadomoscWorkerContext> options)
        : base(options)
    {
    }

    public DbSet<Wiadomosc> Wiadomosci => Set<Wiadomosc>();
    public DbSet<WiadomoscUzytkownik> WiadomoscUzytkownicy => Set<WiadomoscUzytkownik>();
    public DbSet<WiadomoscWzorzec> WiadomoscWzorce => Set<WiadomoscWzorzec>();
    public DbSet<WiadomoscDokument> WiadomoscDokumenty => Set<WiadomoscDokument>();
    public DbSet<WiadomoscWysylka> WiadomoscWysylki => Set<WiadomoscWysylka>();
    public DbSet<WiadomoscOdbiorca> Uzytkownicy => Set<WiadomoscOdbiorca>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wiadomosc>().ToTable("Wiadomosc", "dbo");
        modelBuilder.Entity<WiadomoscUzytkownik>().ToTable("WiadomoscUzytkownik", "dbo");
        modelBuilder.Entity<WiadomoscWzorzec>().ToTable("WiadomoscWzorzec", "dbo");
        modelBuilder.Entity<WiadomoscDokument>().ToTable("WiadomoscDokument", "dbo");
        modelBuilder.Entity<WiadomoscWysylka>().ToTable("WiadomoscWysylka", "dbo");

        modelBuilder.Entity<WiadomoscOdbiorca>(entity =>
        {
            entity.ToTable("Uzytkownik", "dbo");
            entity.HasKey(item => item.usrID);
        });
    }
}

public sealed class WiadomoscOdbiorca
{
    public int usrID { get; set; }
    public int? usrPracownikID { get; set; }
    public string? usrEmail { get; set; }
    public string? usrStatus { get; set; }
    public bool? Usuniety { get; set; }
}
