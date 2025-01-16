using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class UserLogins
    {
        public string LoginId { get; set; }
        public string AuthToken { get; set; }
        public string IPAddress { get; set; }
        public string Password { get; set; }
    }
}
