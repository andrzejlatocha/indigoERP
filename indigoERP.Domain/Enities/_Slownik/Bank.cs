using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Slownik
{
	public partial class Bank
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lbaID { get; set; }
		public string? lbaBank { get; set; }
		public string? lbaNazwa { get; set; }
		public string? lbaStatus { get; set; }
	}
}