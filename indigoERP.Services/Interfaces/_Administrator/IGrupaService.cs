using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Models._Wspolne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IGrupaService
    {
        Task<IEnumerable<Grupa>> Select(string groStatus);
        Task<Grupa> Get(int groID);
        Task<ResponseMessage> Insert(Grupa grupa);
        Task<ResponseMessage> Update(int groID, Grupa grupa);
        Task<ResponseMessage> Delete(int groID);
    }
}
