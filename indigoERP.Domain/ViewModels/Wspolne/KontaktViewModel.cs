using System.ComponentModel.DataAnnotations;

namespace indigoERP.Domain.ViewModels._Wspolne
{
    public class KontaktViewModel
    {
        [Key]
        public int kntID { get; set; }
        public int kntTypKontaktID { get; set; }
        public string? tkoIkona { get; set; }
        public string? tkoOpis { get; set; }
        public string? kntKontakt { get; set; }
        public string? kntUwaga { get; set; }
        public string? kntStatus { get; set; }
    }
}
