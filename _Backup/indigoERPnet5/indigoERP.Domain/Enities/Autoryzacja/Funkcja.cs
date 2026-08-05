using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Autoryzacja
{
	public partial class Funkcja
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int funID { get; set; }
		public int? funFunkcjaID { get; set; }
		public byte? funKolejnosc { get; set; }
		public string? funNazwa { get; set; }
		public string? funTekst { get; set; }
		public string? funTyp { get; set; }
		public string? funIkona { get; set; }
		public string? funFunkcja { get; set; }
		public string? funStatus { get; set; }
	}
}