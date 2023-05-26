using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Slownik
{
	public partial class Godzina
	{
        [Key]
		public byte czaGodzina { get; set; }
		public string? czaStatus { get; set; }
	}
}