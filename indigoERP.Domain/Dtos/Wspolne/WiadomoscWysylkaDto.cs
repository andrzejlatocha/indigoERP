using System;

namespace indigoERP.Domain.Dtos._Wspolne
{
    public class WiadomoscWysylkaDto
    {
        public int wwyID { get; set; }
        public int? wwyWiadomoscID { get; set; }
        public int? wwyPracownikID { get; set; }
        public DateTime? wwyData { get; set; }
        public string? wwyEmail { get; set; }
        public string? wwyZalacznik { get; set; }
        public string? wwyStatus { get; set; }
    }
}
