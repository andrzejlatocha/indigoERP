using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Slownik
{
	[Table("Godzina", Schema = "dic")]
	public partial class Godzina
	{
        [Key]
		public byte czaGodzina { get; set; }
		public string? czaStatus { get; set; }
	}
}