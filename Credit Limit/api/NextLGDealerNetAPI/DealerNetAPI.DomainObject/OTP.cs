using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class OTP : Base
    {
        public string MobileNo { get; set; }
        public string OTPNo { get; set; }
        public string VendorCode { get; set; }
        public string DeviceType { get; set; }
        public string ProcessName { get; set; }
        public string SMSTemplateName { get; set; }
    }
}
