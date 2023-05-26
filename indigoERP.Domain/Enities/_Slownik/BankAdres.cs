using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Slownik
{
	public partial class BankAdres
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int badID { get; set; }
		public int badBankID { get; set; }
		public int badAdresID { get; set; }
		public string? badStatus { get; set; }
	}
}