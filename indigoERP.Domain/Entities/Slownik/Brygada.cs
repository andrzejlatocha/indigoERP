using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.Entities._Slownik
{
	[Table("Brygada", Schema = "dic")]	
	public partial class Brygada
	{
        [Key]
        public string brgBrygada { get; set; }
        public string brgOpis { get; set; }
        public string? brgStatus { get; set; }
	}
}