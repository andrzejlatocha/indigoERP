using indigoERP.Domain.Enumerators._Slownik;

namespace indigoERP.Service.Interfaces._Slownik
{
    public interface ISerwerTypService
    {
        Task<IEnumerable<SerwerTypEnumerator>> SelectLookup(string styStatus);
    }
}
