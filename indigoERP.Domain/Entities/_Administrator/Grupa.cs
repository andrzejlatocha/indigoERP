using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Administrator
{
    public partial class Grupa
    {
        public Grupa()
        {
            AplikacjaDostep = new HashSet<AplikacjaDostep>();
            RolaCzlonkowie = new HashSet<RolaCzlonkowie>();
            UzytkownikGrupa = new HashSet<UzytkownikGrupa>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int groID { get; set; }
        public int? groAplikacjaID { get; set; }
        public string groNazwa { get; set; }
        public string groOpis { get; set; }
        public DateTime? groDataWaznosc { get; set; }
        public bool? groGrupaWszyscy { get; set; }
        public bool? groAdministrator { get; set; }
        public bool? groBlokada { get; set; }
        public bool? groEdycja { get; set; }
        public string groStatus { get; set; }

        public virtual Aplikacja groAplikacja { get; set; }
        public virtual ICollection<AplikacjaDostep> AplikacjaDostep { get; set; }
        public virtual ICollection<RolaCzlonkowie> RolaCzlonkowie { get; set; }
        public virtual ICollection<UzytkownikGrupa> UzytkownikGrupa { get; set; }
    }
}
