using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Notes : Base
    {
        public int? Id { get; set; }
        public string Note { get; set; }
        public int? OrderNo { get; set; }
    }
}
