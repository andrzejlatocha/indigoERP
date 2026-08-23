using indigoERP.Domain.Models._Autoryzacja;
using indigoERP.Domain.ViewModels._Autoryzacja;

namespace indigoERP.Service.Interfaces._Autoryzacja
{
    public interface ILoginService
    {
        Task<UzytkownikViewModel> Get(UzytkownikLoginModel uzytkownikModel);
        Task<UzytkownikViewModel> GetSso(string usrNazwa, string usrKomputer, string appID);
    }
}
