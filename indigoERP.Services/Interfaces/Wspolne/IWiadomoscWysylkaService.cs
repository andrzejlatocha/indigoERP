using indigoERP.Domain.Dtos._Wspolne;
using indigoERP.Domain.Models._Wspolne;

namespace indigoERP.Service.Interfaces._Wspolne
{
    public interface IWiadomoscWysylkaService
    {
        Task<ResponseMessage> Insert(WiadomoscWysylkaDto wysylka);
    }
}
