using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Entities
{
    public class SupportMessage
    {
        public SupportMessage()
        {
            msgAdresEmailDo = "itsupport.czestochowa@libertysteelgroup.com"; 
        }

        public string msgImie { get; set; }
        public string msgNazwisko { get; set; }
        public string msgAdresEmailOd { get; set; }
        public string msgAdresEmailDo { get; set;}
        [Required(ErrorMessage = "Pole nie może być puste")]
        public string msgTemat { get; set; }
        [Required(ErrorMessage = "Pole nie może być puste")] 
        public string msgWiadomosc { get; set;}
        public string msgStatus { get; set; }
    }
}
