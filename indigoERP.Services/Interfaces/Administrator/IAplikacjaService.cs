using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Enumerators._Administrator;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IAplikacjaService
    {
        Task<IEnumerable<AplikacjaViewModel>> Select(string appStatus);
        Task<IEnumerable<AplikacjaEnumerator>> SelectLookup(string appStatus);
        Task<AplikacjaDto> Get(int appID);
        Task<ResponseMessage> Insert(AplikacjaDto aplikacja);
        Task<ResponseMessage> Update(int appID, AplikacjaDto aplikacja);
        Task<ResponseMessage> Delete(int appID);
    }
}
