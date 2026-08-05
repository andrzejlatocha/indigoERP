namespace indigoERP.Domain.Dtos._Administrator
{
    public class SerwerDto
    {
        public int srvID { get; set; }
        public string srvSerwerTypKOD { get; set; }
        public string srvNazwa { get; set; }
        public string srvAdres { get; set; }
        public int? srvPort { get; set; }
        public string? srvOpis { get; set; }
        public string? srvStatus { get; set; }
    }
}
