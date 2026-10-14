using indigoERP.Domain.Dtos._Administrator;
using indigoERP.Domain.Models._Administrator;
using indigoERP.Domain.Models._Wspolne;
using indigoERP.Domain.ViewModels._Administracja;
using indigoERP.Domain.ViewModels._Autoryzacja;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IUzytkownikService
    {
        Task<IEnumerable<UzytkownikViewModel>> Select(string usrStatus);
        Task<IEnumerable<UzytkownikGrupaViewModel>> SelectGrupaByUzytkownik(int appID, int usrID, string groStatus);
        Task<IEnumerable<UzytkownikRolaViewModel>> SelectRolaByUzytkownik(int appID, int usrID, string srlStatus);
        Task<UzytkownikDto> Get(int usrID);
        Task<ResponseMessage> Insert(UzytkownikDto uzytkownik);
        Task<ResponseMessage> Update(int usrID, UzytkownikDto uzytkownik);
        Task<ResponseMessage> Delete(int usrID);
        Task<ResponseMessage> OdblokujLogowanie(string usrNazwa);
        Task<(ResponseMessage Response, IReadOnlyList<UzytkownikEmailGraphModel> Dane)> SelectDaneGraph();
    }
}
