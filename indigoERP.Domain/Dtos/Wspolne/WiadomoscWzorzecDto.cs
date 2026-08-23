namespace indigoERP.Domain.Dtos._Wspolne
{
    public class WiadomoscWzorzecDto
    {
        public int msgID { get; set; }
        public int? msgAplikacjaID { get; set; }
        public byte? msgTyp { get; set; }
        public string? msgJezyk { get; set; }
        public string? msgTemat { get; set; }
        public string msgTresc { get; set; }
        public string msgStatus { get; set; }
    }
}
