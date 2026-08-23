using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Enumerators._Wspolne;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;

namespace indigoERP.Service.Interfaces._Wspolne
{
    public interface IWiadomoscWzorzecService
    {
        Task<IEnumerable<WiadomoscWzorzecViewModel>> Select(int msgAplikacjaID, string msgStatus);
        Task<IEnumerable<WiadomoscWzorzecEnumerator>> SelectLookup(int msgAplikacjaID, string msgStatus);
        Task<WiadomoscWzorzecDto> Get(int msgID);
        Task<ResponseMessage> Insert(WiadomoscWzorzecDto wzorzec);
        Task<ResponseMessage> Update(int msgID, WiadomoscWzorzecDto wzorzec);
        Task<ResponseMessage> Delete(int msgID);
    }
}
