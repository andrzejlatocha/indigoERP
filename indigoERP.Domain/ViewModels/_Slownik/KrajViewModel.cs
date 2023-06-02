using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Slownik
{
    public class KrajViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int kraID { get; set; }
        public string kraKod { get; set; }
        public string kraNazwa { get; set; }
        public string kraNazwaPelna { get; set; }
        public string kraStatus { get; set; }
    }
}
