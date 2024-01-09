using indigoERP.Domain.Entities._Administrator;
using indigoERP.Domain.Models._Wspolne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Service.Interfaces._Administrator
{
    public interface IUzytkownikService
    {
        Task<IEnumerable<Uzytkownik>> Select(string usrStatus);
        Task<Uzytkownik> Get(int usrID);
        Task<ResponseMessage> Insert(Uzytkownik uzytkownik);
        Task<ResponseMessage> Update(int usrID, Uzytkownik uzytkownik);
        Task<ResponseMessage> Delete(int usrID);
    }
}
