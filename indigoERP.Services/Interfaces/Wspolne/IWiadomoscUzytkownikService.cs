using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Models._Wspolne;

namespace indigoERP.Service.Interfaces._Wspolne
{
    public interface IWiadomoscUzytkownikService
    {
        Task<WiadomoscUzytkownikDto> Get(int wuzID);
        Task<ResponseMessage> Insert(WiadomoscUzytkownikDto wiadomoscUzytkownik);
        Task<ResponseMessage> Update(int wuzID, WiadomoscUzytkownikDto wiadomoscUzytkownik);
        Task<ResponseMessage> Delete(int wuzID);
    }
}
