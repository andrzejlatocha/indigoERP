using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Slownik
{
    public partial class Kraj
    {
        public Kraj()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int kraID { get; set; }
        public string kraKod { get; set; }
        public string kraNazwa { get; set; }
        public string kraStatus { get; set; }
    }
}
