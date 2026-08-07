using System;

namespace indigoERP.Domain.Dtos._Wspolne
{
    public class ZdarzenieDto
    {
        public int zdaID { get; set; }
        public int? zdaAplikacjaID { get; set; }
        public string? zdaSerwer { get; set; }
        public string? zdaBaza { get; set; }
        public string? zdaTablica { get; set; }
        public int? zdaIndeksRekordu { get; set; }
        public string? zdaTemat { get; set; }
        public string? zdaTresc { get; set; }
        public DateTime? zdaData { get; set; }
        public DateTime? zdaDataRozpoczecie { get; set; }
        public DateTime? zdaDataZakoczenie { get; set; }
        public Guid? zdaSesja { get; set; }
        public string? zdaStatus { get; set; }
    }
}
