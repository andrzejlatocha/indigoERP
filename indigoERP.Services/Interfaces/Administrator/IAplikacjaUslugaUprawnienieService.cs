using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Administracja;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IAplikacjaUslugaUprawnienieService
    {
        Task<IEnumerable<AplikacjaUslugaUprawnienieViewModel>> Select(string usgStatus);
        Task<AplikacjaUslugaUprawnienieDto> Get(int usgID);
        Task<ResponseMessage> Insert(AplikacjaUslugaUprawnienieDto usluga);
        Task<ResponseMessage> Update(int usgID, AplikacjaUslugaUprawnienieDto usluga);
        Task<ResponseMessage> Delete(int usgID);
    }
}
