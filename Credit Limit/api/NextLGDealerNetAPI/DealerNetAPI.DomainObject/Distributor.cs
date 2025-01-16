using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Distributor : Base
    {
        public Int64? Id { get; set; }
        public string SchemeRefNo { get; set; }
        public string Zone { get; set; }
        public string Region { get; set; }
        public string Branch { get; set; }
        public string CTRL_AU { get; set; }
        public string BillingCode { get; set; }
        public string HeaderCode { get; set; }
        public string AccountName { get; set; }
        public string MajorSalesChannel { get; set; }
        public string SalesChannel { get; set; }
        public string ChannelCode { get; set; }
        public string ChannelCodeName { get; set; }

    }
}
