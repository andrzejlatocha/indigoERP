using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Models._Autoryzacja
{
    public class UzytkownikLoginModel
    {
        public UzytkownikLoginModel()
        {

        }

        public UzytkownikLoginModel(string usrNazwa, string usrHaslo)
        {
            this.usrNazwa = usrNazwa;
            this.usrHaslo = usrHaslo;
        }

        public string usrNazwa { get; set; }
        public string usrHaslo { get; set; }
    }
}
