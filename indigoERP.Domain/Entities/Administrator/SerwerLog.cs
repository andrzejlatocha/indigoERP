using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Administrator
{
    public partial class SerwerLog
    {
        public SerwerLog()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int slgID { get; set; }
        public int slgSerwerID { get; set; }
        public DateTime slgDataSprawdzenie { get; set; }
        public bool slgDostepny { get; set; }
        public int? slgCzasOdpowiedzMs { get; set; }
        public string slgBlad { get; set; }
    }
}
