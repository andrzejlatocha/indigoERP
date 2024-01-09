using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Models._Wspolne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IAplikacjaService
    {
        Task<IEnumerable<Aplikacja>> Select(string appStatus);
        Task<Aplikacja> Get(int appID);
        Task<ResponseMessage> Insert(Aplikacja aplikacja);
        Task<ResponseMessage> Update(int appID, Aplikacja aplikacja);
        Task<ResponseMessage> Delete(int appID);
    }
}
