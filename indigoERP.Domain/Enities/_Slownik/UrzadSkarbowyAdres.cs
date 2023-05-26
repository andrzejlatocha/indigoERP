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
    public class UrzadSkarbowyAdres
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int uadID { get; set; }
        public int uadUrzadSkarbowyID { get; set; }
        public int uadAdresID { get; set; }
        public string uadStatus { get; set; }
    }
}
