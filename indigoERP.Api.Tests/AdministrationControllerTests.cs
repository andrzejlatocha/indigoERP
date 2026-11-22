using System.ComponentModel.DataAnnotations;
using indigoERP.Api.Controllers._Administrator;
using indigoERP.Api.Controllers._Slownik;
using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Dtos._Slownik;
using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Entities._Slownik;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Tests;

public sealed class AdministrationControllerTests
{
    [Fact]
    public async Task Clear_user_groups_removes_only_memberships_of_the_selected_user()
    {
        await using var context = ApiTestContextFactory.Create();
        context.GrupaCzlonkowie.AddRange(
            new GrupaCzlonkowie { uwgUzytkownikID = 12, uwgGrupaNadrzednaID = 1 },
            new GrupaCzlonkowie { uwgUzytkownikID = 12, uwgGrupaNadrzednaID = 2 },
            new GrupaCzlonkowie { uwgUzytkownikID = 13, uwgGrupaNadrzednaID = 1 },
            new GrupaCzlonkowie { uwgGrupaID = 12, uwgGrupaNadrzednaID = 1 });
        await context.SaveChangesAsync();
        var controller = new GrupaController(context, ApiTestContextFactory.Configuration());
        Assert.IsType<NoContentResult>(await controller.DeleteCzlonkowieByUzytkownik(12));
        Assert.Equal(2, await context.GrupaCzlonkowie.CountAsync());
        Assert.IsType<BadRequestObjectResult>(await controller.DeleteCzlonkowieByUzytkownik(0));
        var authorization = typeof(GrupaController).GetMethod(nameof(GrupaController.DeleteCzlonkowieByUzytkownik))!
            .GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), false)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().Single();
        Assert.Equal("HCZ-indigoERP-Administrator", authorization.Roles);
    }

    [Fact]
    public async Task Aplikacja_insert_rejects_an_existing_identifier()
    {
        await using var context = ApiTestContextFactory.Create();
        context.Aplikacja.Add(new Aplikacja { appID = 132, appNazwa = "Indigo", appStatus = "2" });
        await context.SaveChangesAsync();
        var controller = new AplikacjaController(context);

        var result = await controller.Insert(new AplikacjaDto { appID = 132, appNazwa = "Duplikat", appStatus = "2" });

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Aplikacja_update_rejects_an_identifier_change()
    {
        await using var context = ApiTestContextFactory.Create();
        var controller = new AplikacjaController(context);

        var result = await controller.Update(132, new AplikacjaDto { appID = 133 });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Aplikacja_delete_marks_the_entity_as_deleted_and_preserves_audit_data()
    {
        await using var context = ApiTestContextFactory.Create(77);
        var aplikacja = new Aplikacja { appID = 132, appNazwa = "Indigo", appStatus = "2" };
        context.Aplikacja.Add(aplikacja);
        await context.SaveChangesAsync();
        var createdAt = aplikacja.DataUtworzenie;
        var controller = new AplikacjaController(context);

        var result = await controller.Delete(132);

        Assert.IsType<NoContentResult>(result);
        Assert.True(aplikacja.Usuniety);
        Assert.Equal(77, aplikacja.ModyfikacjaID);
        Assert.Equal(createdAt, aplikacja.DataUtworzenie);
    }

    [Fact]
    public async Task Grupa_insert_uses_the_configured_application_identifier()
    {
        await using var context = ApiTestContextFactory.Create();
        var controller = new GrupaController(context, ApiTestContextFactory.Configuration(132));

        var result = await controller.Insert(new GrupaDto { groNazwa = "Test", groStatus = "2" });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<GrupaDto>(created.Value);
        Assert.Equal(132, dto.groAplikacjaID);
    }

    [Fact]
    public async Task Grupa_member_insert_rejects_a_duplicate_active_membership()
    {
        await using var context = ApiTestContextFactory.Create();
        context.GrupaCzlonkowie.Add(new GrupaCzlonkowie
        {
            uwgGrupaNadrzednaID = 1,
            uwgUzytkownikID = 2,
            uwgStatus = "2"
        });
        await context.SaveChangesAsync();
        var controller = new GrupaController(context, ApiTestContextFactory.Configuration());

        var result = await controller.InsertCzlonek(new GrupaCzlonkowieDto
        {
            uwgGrupaNadrzednaID = 1,
            uwgUzytkownikID = 2,
            uwgStatus = "2"
        });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Jednostka_miary_insert_rejects_a_duplicate_code()
    {
        await using var context = ApiTestContextFactory.Create();
        context.JednostkaMiara.Add(new JednostkaMiara { jmiKOD = "kg", jmiOpis = "Kilogram", jmiStatus = "2" });
        await context.SaveChangesAsync();
        var controller = new JednostkaMiaraController(context);

        var result = await controller.Insert(new JednostkaMiaraDto { jmiKOD = "kg", jmiOpis = "Kilogram", jmiStatus = "2" });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public void Aplikacja_dto_requires_an_identifier_greater_than_100()
    {
        var dto = new AplikacjaDto { appID = 100 };
        var results = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);

        Assert.False(valid);
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(AplikacjaDto.appID)));
    }
}
