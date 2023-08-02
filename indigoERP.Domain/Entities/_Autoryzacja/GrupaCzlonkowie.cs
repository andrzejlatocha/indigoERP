using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.Entities._Autoryzacja
{
	public partial class GrupaCzlonkowie
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int uwgID { get; set; }
		public int? uwgGrupaNadrzednaID { get; set; }
		public int? uwgUzytkownikID { get; set; }
		public int? uwgGrupaID { get; set; }
		public bool? uwgDomyslna { get; set; }
		public string? uwgStatus { get; set; }
	}
}