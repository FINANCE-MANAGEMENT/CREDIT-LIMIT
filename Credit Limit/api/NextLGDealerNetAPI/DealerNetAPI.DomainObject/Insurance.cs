using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Insurance
    {
        public int? Id { get; set; }
        public string InsuranceCode { get; set; }
        public string InsuranceCompany { get; set; }
        public decimal? CurrentLmitAmount { get; set; }
    }
}
