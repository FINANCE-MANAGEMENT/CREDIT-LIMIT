using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Bank: Base
    {
        public int? BankId { get; set; }
        public string BankName { get; set; }
        public string emailTo { get; set; }
        public string emailCc { get; set; }
        public string BankCode { get; set; }
    }
}



