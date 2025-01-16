using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class MessageResponse
    {
        public string transaction_id { get; set; }
        public string status_code { get; set; }
        public string status_desc { get; set; }
        public string Mobile_Number { get; set; }
        public string TransactionType { get; set; }
        public string result { get; set; }
        public string UniqueId { get; set; }
        public string MessageTemplateId { get; set; }
        public string Message { get; set; }
        public string MessageId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }

        public string DeviceType { get; set; }
    }
}
