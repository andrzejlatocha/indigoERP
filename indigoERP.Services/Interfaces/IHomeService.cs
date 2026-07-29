using indigoERP.Domain.ViewModels;

namespace indigoERP.Service.Interfaces
{
    public interface IHomeService
    {
        Task<IEnumerable<AplikacjaViewModel>> SelectAplikacja(string appStatus);
    }
}
