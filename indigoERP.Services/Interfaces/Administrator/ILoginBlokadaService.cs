using indigoERP.Domain.Entities._Autoryzacja;
using indigoERP.Domain.Models._Wspolne;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface ILoginBlokadaService
    {
        Task<IEnumerable<LoginBlokada>> Select(string lgbStatus);
        Task<ResponseMessage> Odblokuj(int lgbID);
    }
}
