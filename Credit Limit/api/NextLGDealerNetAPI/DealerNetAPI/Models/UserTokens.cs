using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealerNetAPI.Models
{
    public class UserTokens
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public TimeSpan Validaty { get; set; }
        public Int32 UserId { get; set; }
        public string LoginId { get; set; }
        public string LoginName { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public Guid GuidId { get; set; }
        public DateTime ExpiredTime { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public Role Role { get; set; }
        public Zone Zone { get; set; }
        public bool ChangePwdStatus { get; set; }

    }
}
