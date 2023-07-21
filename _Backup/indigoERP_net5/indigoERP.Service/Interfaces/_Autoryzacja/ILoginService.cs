using indigoERP.Domain.Models._Autoryzacja;
using indigoERP.Domain.ViewModels._Autoryzacja;
using System.Threading.Tasks;

namespace indigoERP.Service.Interfaces._Autoryzacja
{
    public interface ILoginService
    {
        Task<UzytkownikViewModel> Get(UzytkownikLoginModel uzytkownikLoginModel);
        Task<UzytkownikViewModel> Get(string usrNazwa, string usrHaslo);
    }
}
