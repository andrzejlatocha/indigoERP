using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.ViewModels._Slownik
{
    [Table("vDaneFirmaKontakt", Schema = "app")]
    public class DaneFirmaKontaktViewModel
    {
        [Key]
        public int fkoID { get; set; }
        public int fkoDaneFirmaID { get; set; }
        public int kntID { get; set; }
        public string? kntKontakt { get; set; }
        public string? kntMaska { get; set; }
        public string? kntUwaga { get; set; }
        public string? tkoIkona { get; set; }
        public string? tkoOpis { get; set; }
    }
}
