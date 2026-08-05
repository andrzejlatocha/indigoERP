using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Slownik
{
	public partial class DaneFirmaAdres
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fadID { get; set; }
		public int? fadDaneFirmaID { get; set; }
		public int? fadAdresID { get; set; }
		public string? fadStatus { get; set; }
	}
}	