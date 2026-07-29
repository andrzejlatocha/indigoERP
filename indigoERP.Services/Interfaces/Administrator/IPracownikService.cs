using indigoERP.Domain.Enumerators._Administrator;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IPracownikService
    {
        Task<IEnumerable<PracownikEnumerator>> SelectLookup();
    }
}
