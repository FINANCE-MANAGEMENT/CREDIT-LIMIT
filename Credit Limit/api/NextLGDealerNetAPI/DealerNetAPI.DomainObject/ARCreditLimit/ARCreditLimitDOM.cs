using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DealerNetAPI.DomainObject.ARCreditLimit
{
    public class ARCreditLimitDOM : Base
    {
        public ARCreditLimitDOM()
        {
            Distributor = new Distributor();
            Insurance = new ARCreditLimitInsurance();
            SalesHistory = new List<ARCreditLimitSalesDOM>();
            Sales_PaymentTrend = new List<ARCreditLimitSalesDOM>();
            OD_History = new ARCreditLimitOD();
            FutureSalesPlan = new List<ARCreditLimitSalesDOM>();
            Notes = new List<Notes>();
            FYAttachment = new List<ARCreditLimitFYStatus>();
        }
        public Int64? ReqId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public Distributor Distributor { get; set; }
        public ARCreditLimitInsurance Insurance { get; set; }

        public List<ARCreditLimitSalesDOM> SalesHistory { get; set; }
        public List<ARCreditLimitSalesDOM> Sales_PaymentTrend { get; set; }
        public ARCreditLimitOD OD_History { get; set; }
        public List<ARCreditLimitSalesDOM> FutureSalesPlan { get; set; }
        public List<Notes> Notes { get; set; }
        public List<ARCreditLimitFYStatus> FYAttachment { get; set; }

        public int? RowNumber { get; set; }
        public string FileNamePath { get; set; }
        public string EmailSent { get; set; }
        public string ProcessType { get; set; }

    }
}
