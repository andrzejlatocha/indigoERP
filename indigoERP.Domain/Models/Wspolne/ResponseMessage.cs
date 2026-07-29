using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indigoERP.Domain.Models._Wspolne
{
    public class ResponseMessage
    {
        public ResponseMessage(int code, string? message, int? id)
        {
            Code = code;
            Message = message;
            Id = id;
            Date = DateTime.Now;
        }

        public int Code { get; set; }
        public string? Message { get; set; }
        public int? Id { get; set; }
        public DateTime Date { get; set; }
    }
}
