using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Enities._Wspolne
{
    public partial class NowoscUzytkownik
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int nusID { get; set; }
        public int? nusNowosciID { get; set; }
        public int? nusUzytkownikID { get; set; }
        public string nusStatus { get; set; }
    }
}
