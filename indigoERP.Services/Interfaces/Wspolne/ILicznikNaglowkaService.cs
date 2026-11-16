using indigoERP.Domain.ViewModels._Wspolne;

namespace indigoERP.Service.Interfaces._Wspolne
{
    public interface ILicznikNaglowkaService
    {
        Task<LicznikNaglowkaViewModel> Pobierz(int appID, int usrID);
    }
}
