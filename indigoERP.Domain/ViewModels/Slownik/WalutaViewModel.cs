using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace indigoERP.Domain.ViewModels._Slownik
{
    [Table("vWaluta", Schema = "mvw")]
    public class WalutaViewModel
    {
        [Key]
        public string walKOD { get; set; }
        public string walOpis { get; set; }
        public string walStatus { get; set; }
    }
}
