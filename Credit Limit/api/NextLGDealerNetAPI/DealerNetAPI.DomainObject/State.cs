using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class State :Base
    {
        public int? StateId { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
    }
}
