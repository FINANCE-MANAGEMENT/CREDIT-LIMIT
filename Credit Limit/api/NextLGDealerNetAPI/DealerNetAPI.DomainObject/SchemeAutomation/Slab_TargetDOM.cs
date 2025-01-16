using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject.SchemeAutomation
{
    public class Slab_TargetDOM : Base
    {
        public string SchemeRefNo { get; set; }
        public string BillingCode { get; set; }
        public string SlabNo { get; set; }
        public string SlabFrom { get; set; }
        public string SlabTo { get; set; }
        public string SlabScheme { get; set; }
        public string Target { get; set; }
        public string TargetType { get; set; }
        public string TargetBased { get; set; }
        public string RowNumber { get; set; }
    }
}
