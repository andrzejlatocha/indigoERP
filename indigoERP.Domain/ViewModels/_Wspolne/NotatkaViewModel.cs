using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    [Table("vNotatka", Schema = "mvw")]
    public class NotatkaViewModel
    {
        [Key]
        public int notID { get; set; }
        public int? notAplikacjaID { get; set; }
        public int? notUzytkownikID { get; set; }       
        public string? usrNazwa { get; set; }
        public string? usrImie { get; set; }
        public string? usrNazwisko { get; set; }
        public string? usrImieNazwisko { get; set; }
        public string? usrTelefon { get; set; }
        public string? usrKomorka { get; set; }
        public string? usrEmail { get; set; }
        public byte? notTypNotatkaID { get; set; }
        public string? tnnIkona { get; set; }
        public string? tnnOpis { get; set; }
        public DateTime? notData { get; set; }
        public string? notKategoriaNotatkaLST { get; set; }
        public string? ktnIkona { get; set; }
        public string? ktnOpis { get; set; }
        public string? notStatus { get; set; }
    }
}
