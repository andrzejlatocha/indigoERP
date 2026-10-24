using indigoERP.Api.Controllers._Wspolne;
using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Api.Tests;

public sealed class WiadomoscPracownikControllerTests
{
    [Fact]
    public async Task Insert_select_update_and_delete_manage_message_employee()
    {
        await using var context = ApiTestContextFactory.Create();
        var controller = new WiadomoscPracownikController(context);
        var dto = new WiadomoscPracownikDto
        {
            wprWiadomoscID = 15,
            wprPracownikID = 3106,
            wprEmail = "pracownik@hcz.com.pl",
            wprBlad = "Nie można dostarczyć wiadomości.",
            wprStatus = "2"
        };

        var insertResult = await controller.Insert(dto);
        var created = Assert.IsType<CreatedAtActionResult>(insertResult.Result);
        var inserted = Assert.IsType<WiadomoscPracownikDto>(created.Value);

        var selectResult = await controller.SelectByWiadomosc(15, "2");
        var selected = Assert.IsAssignableFrom<IEnumerable<WiadomoscPracownikDto>>(selectResult.Value);
        Assert.Single(selected);

        inserted.wprEmail = "nowy.adres@hcz.com.pl";
        inserted.wprBlad = "Skrzynka odbiorcy nie istnieje.";
        var updateResult = await controller.Update(inserted.wprID, inserted);
        Assert.IsType<NoContentResult>(updateResult);
        Assert.Equal("nowy.adres@hcz.com.pl", (await context.WiadomoscPracownik.FindAsync(inserted.wprID))!.wprEmail);
        Assert.Equal("Skrzynka odbiorcy nie istnieje.", (await context.WiadomoscPracownik.FindAsync(inserted.wprID))!.wprBlad);

        var deleteResult = await controller.Delete(inserted.wprID);
        Assert.IsType<NoContentResult>(deleteResult);
        Assert.Null(await context.WiadomoscPracownik.FindAsync(inserted.wprID));
    }

    [Fact]
    public async Task Update_returns_bad_request_when_identifier_does_not_match_dto()
    {
        await using var context = ApiTestContextFactory.Create();
        var controller = new WiadomoscPracownikController(context);

        var result = await controller.Update(8, new WiadomoscPracownikDto { wprID = 9 });

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void Dto_rejects_invalid_email_and_status_longer_than_one_character()
    {
        var dto = new WiadomoscPracownikDto { wprEmail = "nie-poprawny", wprBlad = new string('x', 4001), wprStatus = "22" };
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, validateAllProperties: true);

        Assert.False(isValid);
        Assert.Equal(3, validationResults.Count);
    }

    [Fact]
    public void Select_returns_message_employee_view_model()
    {
        var method = typeof(WiadomoscPracownikController).GetMethod(nameof(WiadomoscPracownikController.Select));

        Assert.Equal(
            typeof(Task<ActionResult<IEnumerable<indigoERP.Domain.ViewModels._Wspolne.WiadomoscPracownikViewModel>>>),
            method!.ReturnType);
    }

    [Fact]
    public async Task SelectByWiadomoscZakres_returns_not_found_when_scope_is_not_active_or_does_not_exist()
    {
        await using var context = ApiTestContextFactory.Create();
        var controller = new WiadomoscPracownikController(context);

        var result = await controller.SelectByWiadomoscZakres(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void SelectByWiadomoscZakres_returns_range_employee_view_model()
    {
        var method = typeof(WiadomoscPracownikController).GetMethod(nameof(WiadomoscPracownikController.SelectByWiadomoscZakres));

        Assert.Equal(typeof(Task<ActionResult<IEnumerable<WiadomoscZakresPracownikViewModel>>>), method!.ReturnType);
        Assert.Equal("wzaID", Assert.Single(method.GetParameters()).Name);
    }
}
