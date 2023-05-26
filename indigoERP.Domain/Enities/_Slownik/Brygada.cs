using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Slownik
{
	public partial class Brygada
	{
        [Key]
        public string brgBrygada { get; set; }
        public string brgOpis { get; set; }
        public string? brgStatus { get; set; }
	}
}