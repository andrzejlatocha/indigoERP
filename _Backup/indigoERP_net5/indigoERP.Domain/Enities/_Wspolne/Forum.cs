using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Enities._Wspolne
{
    public partial class Forum
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int frmID { get; set; }
        public int? frmForumID { get; set; }
        public int? frmUzytkownikID { get; set; }
        public DateTime? frmData { get; set; }
        public int? frmTypID { get; set; }
        public string? frmTresc { get; set; }
        public string? frmStatus { get; set; }
    }
}