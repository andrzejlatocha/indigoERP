using indigoERP.Domain.ViewModels._Wspolne;

namespace indigoERP.Service.Interfaces._Wspolne
{
    /// <summary>Nieprzeczytane pozycje zalogowanego użytkownika do podglądu (okno boczne i strony osadzane).</summary>
    public interface IPodgladService
    {
        Task<IEnumerable<WiadomoscUzytkownikViewModel>> WiadomosciNieprzeczytane(int appID, int usrID);
        Task<IEnumerable<ZadanieViewModel>> ZadaniaNieprzeczytane(int appID, int usrID);
        Task<IEnumerable<ZdarzenieViewModel>> ZdarzeniaNieprzeczytane(int appID, int usrID);
        Task<IEnumerable<PowiadomienieViewModel>> PowiadomieniaNieprzeczytane(int appID, int usrID);
    }
}
