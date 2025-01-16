using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class TradePartner : Base
    {
        public int RequestId { get; set; }
        public string RequestNo { get; set; }
        public string FirmName { get; set; }
        public string HeaderCode { get; set; }
        public string BranchCode { get; set; }
        public string SalesChannel { get; set; }
        public int TotalRetailStore { get; set; }

        public string PAN_No { get; set; }
        public string GST_No { get; set; }

        public FirmType FirmType { get; set; }

        public string CommencementFirmYear { get; set; }
        public string AssociatedWithLGYear { get; set; }

        public FirmPartner FirmPartner { get; set; }

    }
}
