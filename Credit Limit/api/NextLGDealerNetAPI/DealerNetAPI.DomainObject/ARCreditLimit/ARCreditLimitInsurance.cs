using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject.ARCreditLimit
{
    public class ARCreditLimitInsurance : Base
    {
        public Int64? Id { get; set; }
        public string HeaderCode { get; set; }
        public string AccountName { get; set; }
        public string InsuranceCode { get; set; }
        public string InsuranceName { get; set; }
        public decimal? CurrentCreditLimitAmount { get; set; }
        public DateTime? CurrentCreditLimitEndDate { get; set; }
        public decimal? CreditLimitRequestAmount { get; set; }
        public decimal? CreditLimitRequestAmountApproved { get; set; }

        public int? RowNumber { get; set; }
        public string FileNamePath { get; set; }
        public string ProcessType { get; set; }
        public string DataSource { get; set; }
    }
}
