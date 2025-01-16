using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class ProductDetail :Base
    {
        public int ProductId { get; set; }
        public Int64 RequestId { get; set; }
        public string RecordType { get; set; }
        public decimal HE_Amount { get; set; }
        public decimal HA_Amount { get; set; }
        public decimal RAC_Amount { get; set; }
        public decimal Others_Amount { get; set; }
        public decimal Total_HE_HA_Others_Amount { get; set; }
    }
}
