using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    [Table("vWiadomoscWzorzec", Schema = "mvw")]
    [Keyless]
    public class WiadomoscWzorzecViewModel
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
