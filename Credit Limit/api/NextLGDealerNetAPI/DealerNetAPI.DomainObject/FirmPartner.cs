using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class FirmPartner : Base
    {
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerOfficialEmail { get; set; }
        public string PartnerOfficialMobile { get; set; }
    }
}
