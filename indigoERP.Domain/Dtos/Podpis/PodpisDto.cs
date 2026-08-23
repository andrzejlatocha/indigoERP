namespace indigoERP.Domain.Dtos._Podpis
{
    public class PodpisDto
    {
        public int pdpID { get; set; }
        public string pdpNazwa { get; set; }
        public string? pdpOpis { get; set; }
        public string pdpTresc { get; set; }
        public bool pdpDomyslny { get; set; }
        public string pdpStatus { get; set; }
    }
}
