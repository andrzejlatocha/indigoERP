using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Administracja;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IGrupaService
    {
        Task<IEnumerable<GrupaViewModel>> Select(string groStatus);
        Task<IEnumerable<GrupaRolaViewModel>> SelectRolaByGrupa(int appID, int groID, string srlStatus);
        Task<IEnumerable<UzytkownikGrupaViewModel>> SelectUzytkownikByGrupa(int appID, int groID, string usrStatus);
        Task<GrupaDto> Get(int groID);
        Task<ResponseMessage> Insert(GrupaDto grupa);
        Task<ResponseMessage> Update(int groID, GrupaDto grupa);
        Task<ResponseMessage> Delete(int groID);
        Task<ResponseMessage> InsertCzlonek(GrupaCzlonkowieDto czlonek);
        Task<ResponseMessage> DeleteCzlonek(int uwgID);
        Task<ResponseMessage> DeleteCzlonkowieByUzytkownik(int usrID);
    }
}
