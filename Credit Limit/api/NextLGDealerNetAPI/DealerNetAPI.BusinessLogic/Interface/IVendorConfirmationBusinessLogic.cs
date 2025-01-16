using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IVendorConfirmationBusinessLogic
    {
        Task<APIResponse> VendorInvoiceUpload(List<VendorInvoice> vendorInvoices);

        Task<List<VendorInvoice>> ReadVendorInvoice(VendorInvoice vendorInvoice);
        Task<List<VendorInvoice>> ReadVendorInvoiceSummary(VendorInvoice vendorInvoice);

        Task<APIResponse> VendorInvoiceVerifyApprove(VendorInvoice vendor, Int32 createdBy);
        Task<APIResponse> VendorInvoiceApprovedRollback(List<VendorInvoice> vendors, Int32 createdBy);
        Task<APIResponse> LGInvoiceEmailSendToVendorStatusUpdate(VendorInvoice vendor, Int32 createdBy);


        Task<APIResponse> VendorClaimsAmount(VendorInvoice vendorClaims);
        Task<VendorInvoice> ReadVendorClaimsAmount(VendorInvoice vendorInvoice);

        Task<APIResponse> VendorInvoiceAddedByVendor(List<VendorInvoice> vendorInvoices);
        Task<List<VendorInvoice>> ReadVendorInvoiceAddedByVendor(VendorInvoice vendorInvoice);

        Task<List<VendorInvoice>> ReadVendorConfirmationClaims(VendorInvoice vendorInvoice);

        Task<APIResponse> VendorInvoiceClaimReplied(List<VendorInvoice> vendorInvoices);


        Task<APIResponse> VendorInvoiceAcceptance(VendorInvoice vendorAcceptance);

        Task<List<VendorInvoice>> ReadVendorConfirmationTracker(VendorInvoice confirmationTracker);
    }
}
