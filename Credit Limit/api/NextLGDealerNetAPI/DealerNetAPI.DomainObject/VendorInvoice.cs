using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class VendorInvoice : Base
    {
        public VendorInvoice()
        {
            Vendors = new Vendor();
        }
        public Vendor Vendors { get; set; }
        public string ConfirmationPeriod { get; set; }
        public string BranchCode { get; set; }
        public Int64 InvoiceId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string LG_PO_No { get; set; }
        public decimal ReceivedPaymentAmount { get; set; }
        public decimal? Balance { get; set; }
        public decimal ClosingBalance { get; set; }

        public decimal? LGConfirmedAmount { get; set; }
        public decimal? VendorClaimAmount { get; set; }
        public decimal? TotalConfirmedAmount { get; set; }

        public Int32 RowNumber { get; set; }

        public bool AdminApprovedStatus { get; set; }

        public string PIC_Name { get; set; }
        public string PIC_MobileNo { get; set; }
        public string VendorRemarks { get; set; }

        public string ClaimReplyStatus { get; set; }
        public DateTime? ClaimReplyDate { get; set; }

        public string VendorAcceptance { get; set; }
        public DateTime? VendorAcceptanceDate { get; set; }
        public DateTime? VendorConfirmationDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal? AfterPaymentBalance { get; set; }

        public string ActivityType { get; set; }
        public DateTime? ActivityDate { get; set; }

        public string BAMAcceptance { get; set; }
        public string BAMClaimReplied { get; set; }
        public DateTime? BAMClaimRepliedDate { get; set; }
        public string BAMRemarks { get; set; }

        public string EmailSend { get; set; }
        public string SMSSend { get; set; }

    }

}
