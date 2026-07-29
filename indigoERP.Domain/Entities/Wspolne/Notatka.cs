using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using indigoERP.Domain.Common;
namespace indigoERP.Domain.Entities._Wspolne
{
	public partial class Notatka : AuditableEntity
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int notID { get; set; }
        public int? notAplikacjaID { get; set; }
        public int? notUzytkownikID { get; set; }
		public byte? notTypNotatkaID { get; set; }
		public string? notTemat { get; set; }
		public DateTime? notData { get; set; }
		public string? notKategoriaNotatkaLST { get; set; }
		public string? notStatus { get; set; }
	}
}