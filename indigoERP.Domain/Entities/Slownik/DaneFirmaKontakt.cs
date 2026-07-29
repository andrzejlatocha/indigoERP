using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using indigoERP.Domain.Common;
namespace indigoERP.Domain.Entities._Slownik
{
	public partial class DaneFirmaKontakt : AuditableEntity
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fkoID { get; set; }
		public int? fkoDaneFirmaID { get; set; }
		public int? fkoKontaktID { get; set; }
		public string? fkoStatus { get; set; }
	}
}