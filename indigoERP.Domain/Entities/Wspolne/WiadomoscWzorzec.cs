using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Wspolne
{
    [Table("WiadomoscWzorzec", Schema = "dbo")]
    public partial class WiadomoscWzorzec
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int msgID { get; set; }
        public int? msgAplikacjaID { get; set; }
        public byte? msgTyp { get; set; }
        public string msgJezyk { get; set; }
        public string msgTemat { get; set; }
        public string msgTresc { get; set; }
        public string msgStatus { get; set; }
    }
}
