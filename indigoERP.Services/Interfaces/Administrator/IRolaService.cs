using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Administracja;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IRolaService
    {
        Task<IEnumerable<RolaViewModel>> Select(string srlStatus);
        Task<IEnumerable<GrupaRolaViewModel>> SelectGrupaByRola(int appID, int srlID, string groStatus);
        Task<IEnumerable<UzytkownikRolaViewModel>> SelectUzytkownikByRola(int appID, int srlID, string usrStatus);
        Task<RolaDto> Get(int srlID);
        Task<ResponseMessage> Insert(RolaDto rola);
        Task<ResponseMessage> Update(int srlID, RolaDto rola);
        Task<ResponseMessage> Delete(int srlID);
        Task<ResponseMessage> InsertCzlonek(RolaCzlonkowieDto czlonek);
        Task<ResponseMessage> DeleteCzlonek(int cwrID);
    }
}
