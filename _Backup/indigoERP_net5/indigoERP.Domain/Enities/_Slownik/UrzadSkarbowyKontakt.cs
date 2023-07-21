using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace indigoERP.Domain.Enities._Slownik
{
    public class UrzadSkarbowyKontakt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ukoID { get; set; }
        public int ukoUrzadSkarbowyID { get; set; }
        public int ukoKontaktID { get; set; }
        public string ukoStatus { get; set; }
    }
}
