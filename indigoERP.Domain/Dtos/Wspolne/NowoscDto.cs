using System;

namespace indigoERP.Domain.Dtos._Wspolne
{
    public class NowoscDto
    {
        public int nowID { get; set; }
        public DateTime? nowData { get; set; }
        public int? nowAplikacjaID { get; set; }
        public string? nowURL { get; set; }
        public byte? nowWersja { get; set; }
        public string? nowTytul { get; set; }
        public string? nowOpis { get; set; }
        public string? nowStatus { get; set; }
        public string? nowTypNowoscKOD { get; set; }
    }
}
