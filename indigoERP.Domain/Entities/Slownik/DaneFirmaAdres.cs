using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using indigoERP.Domain.Common;
namespace indigoERP.Domain.Entities._Slownik
{
	public partial class DaneFirmaAdres : AuditableEntity
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fadID { get; set; }
		public int? fadDaneFirmaID { get; set; }
		public int? fadAdresID { get; set; }
		public string? fadStatus { get; set; }
	}
}	