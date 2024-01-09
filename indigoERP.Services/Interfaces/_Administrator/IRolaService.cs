using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Models._Wspolne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IRolaService
    {
        Task<IEnumerable<Rola>> Select(string srlStatus);
        Task<Rola> Get(int srlID);
        Task<ResponseMessage> Insert(Rola rola);
        Task<ResponseMessage> Update(int srlID, Rola rola);
        Task<ResponseMessage> Delete(int srlID);
    }
}
