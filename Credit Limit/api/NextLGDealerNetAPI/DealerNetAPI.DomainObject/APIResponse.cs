using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class APIResponse
    {
        public string Status { get; set; }
        public string StatusDesc { get; set; }

        public object data { get; set; }

    }
}
