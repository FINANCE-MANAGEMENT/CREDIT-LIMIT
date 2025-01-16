using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class VendorCommunication : Base
    {
        public VendorCommunication()
        {
            RequiredInfo = new List<Lookup>();
            Vendors = new List<Vendor>();
        }
        public int? TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateContent { get; set; }
        public List<Lookup> RequiredInfo { get; set; }
        public List<Vendor> Vendors { get; set; }
        public string TemplateSendStatus { get; set; }

        public DateTime? CommunicationSendDate { get; set; }
        public string CommunicationAcceptance { get; set; }
        public DateTime? CommunicationAcceptanceDate { get; set; }
    }
}
