using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject.ARCreditLimit
{
    public class ARCreditLimitOD : Base
    {
        public Int64? Id { get; set; }
        public string HeaderCode { get; set; }
        public string AccountName { get; set; }
        public string MonthYear { get; set; }
        public decimal? ODAmount { get; set; }

        public int? TotalOD { get; set; }
        public decimal? MaxOD { get; set; }
        public decimal? AverageOD { get; set; }

        public int? RowNumber { get; set; }
        public string FileNamePath { get; set; }
        public string ProcessType { get; set; }

        public string DataSource { get; set; }
    }
}
