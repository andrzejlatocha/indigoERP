using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Wspolne
{
    [Table("WiadomoscWysylka", Schema = "dbo")]
    public partial class WiadomoscWysylka
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int wwyID { get; set; }
        public int? wwyWiadomoscID { get; set; }
        public int? wwyPracownikID { get; set; }
        public DateTime? wwyData { get; set; }
        public string wwyEmail { get; set; }
        public string wwyZalacznik { get; set; }
        public string wwyStatus { get; set; }
    }
}
