using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Role : Base
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleDesc { get; set; }

        public bool isChecked { get; set; }

    }
}
