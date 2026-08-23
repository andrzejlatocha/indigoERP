using indigoERP.Domain.Dtos._Podpis;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Podpis;

namespace indigoERP.Service.Interfaces._Podpis
{
    public interface IPodpisGrupaService
    {
        Task<IEnumerable<PodpisGrupaViewModel>> SelectByPodpis(int pdpID, string pdgStatus);
        Task<PodpisGrupaDto> Get(int pdgID);
        Task<ResponseMessage> Insert(PodpisGrupaDto pozycja);
        Task<ResponseMessage> Update(int pdgID, PodpisGrupaDto pozycja);
        Task<ResponseMessage> Delete(int pdgID);
    }
}
