using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace indigoERP.Domain.Entities._Autoryzacja
{
    public partial class Uzytkownik
    {
        public Uzytkownik()
        {
            AplikacjaDostep = new HashSet<AplikacjaDostep>();
            RolaCzlonkowie = new HashSet<RolaCzlonkowie>();
            UzytkownikGrupa = new HashSet<UzytkownikGrupa>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int usrID { get; set; }
        public string usrNazwa { get; set; }
        public string usrImie { get; set; }
        public string usrNazwisko { get; set; }
        public int? usrKlientID { get; set; }
        public int? usrPracownikID { get; set; }
        public string usrJezyk { get; set; }
        public string usrTelefon { get; set; }
        public string usrEmail { get; set; }
        public byte[] usrHaslo { get; set; }
        public DateTime? usrDataWaznoscHaslo { get; set; }
        public bool? usrNowy { get; set; }
        public bool? usrZmianaHasla { get; set; }
        public bool? usrBlokada { get; set; }
        public bool? usrEdycja { get; set; }
        public string usrStatus { get; set; }

        public virtual ICollection<AplikacjaDostep> AplikacjaDostep { get; set; }
        public virtual ICollection<RolaCzlonkowie> RolaCzlonkowie { get; set; }
        public virtual ICollection<UzytkownikGrupa> UzytkownikGrupa { get; set; }
    }
}
