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
