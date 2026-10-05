using System;

namespace indigoERP.Domain.Dtos._Administrator
{
    public class AplikacjaUslugaUprawnienieDto
    {
        public int usgID { get; set; }
        public int usgAplikacjaZrodloID { get; set; }
        public int usgAplikacjaCelID { get; set; }
        public string usgKodRola { get; set; }
        public bool usgAktywny { get; set; }
        public DateTime usgDataOd { get; set; }
        public DateTime? usgDataDo { get; set; }
        public string usgOpis { get; set; }
    }
}
