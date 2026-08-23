using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;

namespace indigoERP.Service.Interfaces._Wspolne
{
    public interface IWiadomoscService
    {
        Task<IEnumerable<WiadomoscViewModel>> Select(int wiaAplikacjaID, string wiaStatus);
        Task<IEnumerable<WiadomoscHistoriaViewModel>> SelectHistoria(int wiaAplikacjaID, int? wiaUzytkownikID, string wiaStatus);
        Task<IEnumerable<WiadomoscViewModel>> SelectByUzytkownik(int appID, int usrID, string wiaStatus);
        Task<int> SelectCountByUzytkownik(int appID, int usrID);
        Task<WiadomoscViewModel?> Detail(int wiaID, int usrID);
        Task<WiadomoscDto> Get(int wiaID);
        Task<ResponseMessage> Insert(WiadomoscDto wiadomosc);
        Task<ResponseMessage> Update(int wiaID, WiadomoscDto wiadomosc);
        Task<ResponseMessage> Delete(int wiaID);
    }
}
