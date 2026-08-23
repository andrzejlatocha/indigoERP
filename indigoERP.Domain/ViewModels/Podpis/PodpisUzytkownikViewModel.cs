using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace indigoERP.Domain.ViewModels._Podpis
{
    [Table("vPodpisUzytkownik", Schema = "mvw")]
    [Keyless]
    public class PodpisUzytkownikViewModel
    {
        public int pduID { get; set; }
        public int pduPodpisID { get; set; }
        public string pdpNazwa { get; set; }
        public int pduUzytkownikID { get; set; }
        public string? usrImieNazwisko { get; set; }
        public string pduStatus { get; set; }
    }
}
