using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Tasks : Base
    {
        public Int32 MenuId { get; set; }
        public string MenuDisplayName { get; set; }
        public string MenuURL { get; set; }
        public Int32 MenuOrderId { get; set; }
        public Role Role { get; set; }
    }
}
