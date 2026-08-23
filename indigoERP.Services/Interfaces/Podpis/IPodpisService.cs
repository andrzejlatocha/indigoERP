using indigoERP.Domain.Dtos._Podpis;
using indigoERP.Domain.Enumerators._Podpis;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Podpis;

namespace indigoERP.Service.Interfaces._Podpis
{
    public interface IPodpisService
    {
        Task<IEnumerable<PodpisViewModel>> Select(string pdpStatus);
        Task<IEnumerable<PodpisEnumerator>> SelectLookup(string pdpStatus);
        Task<PodpisDto> Get(int pdpID);
        Task<ResponseMessage> Insert(PodpisDto podpis);
        Task<ResponseMessage> Update(int pdpID, PodpisDto podpis);
        Task<ResponseMessage> Delete(int pdpID);
        Task<string> Preview(int usrID);
    }
}
