using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Slownik
{
	public partial class DaneFirma
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int stfID { get; set; }
		public string? stfNip { get; set; }
		public string? stfRegon { get; set; }
		public string? stfNazwaKrotka { get; set; }
		public string? stfNazwa { get; set; }
		public string? stfNazwaPlatnik { get; set; }
		public byte[]? stfLogo { get; set; }
		public string? stfStatus { get; set; }
	}
}