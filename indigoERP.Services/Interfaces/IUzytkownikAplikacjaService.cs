using indigoERP.Domain.ViewModels._Autoryzacja;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Service.Interfaces
{
    public interface IUzytkownikAplikacjaService
    {
        Task<IEnumerable<UzytkownikAplikacjaViewModel>> Select(int usrID);
    }
}
