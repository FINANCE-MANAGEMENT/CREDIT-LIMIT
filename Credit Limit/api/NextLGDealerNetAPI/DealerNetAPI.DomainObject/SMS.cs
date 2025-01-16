using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    [Serializable]
    public class SMS
    {
        public string MessageTemplateId { get; set; }
        public string Message { get; set; }
        public string OTPNo { get; set; }
    }
}
