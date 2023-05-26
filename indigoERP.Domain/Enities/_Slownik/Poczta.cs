using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace epicaHR.Domain.Entities._Slownik
{
    public class Poczta
    {
        public Poczta()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pczID { get; set; }
        public string? pczTyp { get; set; }
        public string? pczNazwa { get; set; }
        public int? pczGminaID { get; set; }
        public bool pczPlacowkaPocztowa { get; set; }
        public string? pczStatus { get; set; }
    }
}
