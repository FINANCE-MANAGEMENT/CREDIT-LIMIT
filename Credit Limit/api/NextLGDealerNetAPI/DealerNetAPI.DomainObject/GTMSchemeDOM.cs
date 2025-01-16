using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class GTMSchemeDOM
    {
        public string SchemeRefNo { get; set; }
        public DateTime? SelloutDate { get; set; }
        public string MonthYear { get; set; }
        public string BillingCode { get; set; }
        public string AccountName { get; set; }
        public string ChannelCode { get; set; }
        public string Region { get; set; }
        public string BranchCode { get; set; }
        public string ModelPrefix { get; set; }
        public string ModelSuffix { get; set; }
        public string ProductGrade { get; set; }
        public string ProductCategory { get; set; }
        public string ProductGroup { get; set; }
        public string BasicPricePerUnit { get; set; }
        public string OpeningStock { get; set; }
        public string SellIn { get; set; }
        public string SellOut { get; set; }
        public string Closing { get; set; }
        public string BPOpeningStock { get; set; }
        public string BPSellIn { get; set; }
        public string BPSellOut { get; set; }
        public string BPClosing { get; set; }
        public string Remarks { get; set; }
        public string SourceSystem { get; set; }
    }
}
