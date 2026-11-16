using indigoERP.Domain.ViewModels._Wspolne;

namespace indigoERP.Service.Interfaces._Wspolne
{
    public interface IPolecenieSluzboweService
    {
        Task<IEnumerable<PrzepisWewnetrznyViewModel>> SelectPubliczne(string? szukaj, int? katalogID);
        Task<IEnumerable<KatalogWyborViewModel>> SelectKatalogi();
        Task<byte[]?> Pdf(int pliID);
    }
}
