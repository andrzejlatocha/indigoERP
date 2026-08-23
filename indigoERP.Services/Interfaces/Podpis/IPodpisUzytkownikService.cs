using indigoERP.Domain.Dtos._Podpis;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Podpis;

namespace indigoERP.Service.Interfaces._Podpis
{
    public interface IPodpisUzytkownikService
    {
        Task<IEnumerable<PodpisUzytkownikViewModel>> SelectByPodpis(int pdpID, string pduStatus);
        Task<PodpisUzytkownikDto> Get(int pduID);
        Task<ResponseMessage> Insert(PodpisUzytkownikDto pozycja);
        Task<ResponseMessage> Update(int pduID, PodpisUzytkownikDto pozycja);
        Task<ResponseMessage> Delete(int pduID);
    }
}
