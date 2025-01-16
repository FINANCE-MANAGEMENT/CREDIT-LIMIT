using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject.ARCreditLimit
{
    public class ARCreditLimitFYStatus : Base
    {
        public int? Id { get; set; }
        public int? FYStatusId { get; set; }
        public string FYStatus { get; set; }
        public string MandatoryStatus { get; set; }
        public string UploadStatus { get; set; }

        public string HeaderCode { get; set; }
        public string AccountName { get; set; }
        public string FinancialYear { get; set; }

        public int? RowNumber { get; set; }
        public string FileNamePath { get; set; }
        public string ProcessType { get; set; }

        public string DataSource { get; set; }


    }
}
