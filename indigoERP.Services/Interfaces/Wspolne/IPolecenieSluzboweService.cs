using indigoERP.Domain.ViewModels._Wspolne;

namespace indigoERP.Service.Interfaces._Wspolne
{
    public interface IPolecenieSluzboweService
    {
        Task<IEnumerable<PrzepisWewnetrznyViewModel>> SelectPubliczne(string? szukaj);
        Task<byte[]?> Pdf(int pliID);
    }
}
