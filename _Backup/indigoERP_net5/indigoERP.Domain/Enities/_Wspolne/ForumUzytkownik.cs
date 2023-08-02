using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Wspolne
{
    public partial class ForumUzytkownik
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fshID { get; set; }
        public int? fshForumID { get; set; }
        public int? fshUzytkownikID { get; set; }
        public bool? fshInformuj { get; set; }
        public bool? fshTylkoOdczyt { get; set; }
        public string? fshStatus { get; set; }
    }
}