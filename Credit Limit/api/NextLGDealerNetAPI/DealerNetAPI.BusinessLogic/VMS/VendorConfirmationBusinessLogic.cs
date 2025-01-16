using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.VMS
{
    public class VendorConfirmationBusinessLogic : IVendorConfirmationBusinessLogic
    {
        private readonly IVendorConfirmationAccess _vendorConfirmationAccess = null;
        public VendorConfirmationBusinessLogic(IVendorConfirmationAccess vendorConfirmationAccess)
        {
            _vendorConfirmationAccess = vendorConfirmationAccess;
        }

        public async Task<APIResponse> VendorInvoiceUpload(List<VendorInvoice> vendorInvoices)
        {
            var data = await _vendorConfirmationAccess.VendorInvoiceUpload(vendorInvoices);
            return data;
        }

        public async Task<List<VendorInvoice>> ReadVendorInvoice(VendorInvoice vendorInvoice)
        {
            var data = await _vendorConfirmationAccess.ReadVendorInvoice(vendorInvoice);
            return data;
        }


        public async Task<List<VendorInvoice>> ReadVendorInvoiceSummary(VendorInvoice vendorInvoice)
        {
            var data = await _vendorConfirmationAccess.ReadVendorInvoiceSummary(vendorInvoice);
            return data;
        }

        public async Task<APIResponse> VendorInvoiceVerifyApprove(VendorInvoice vendor, Int32 createdBy)
        {
            var data = await _vendorConfirmationAccess.VendorInvoiceVerifyApprove(vendor, createdBy);
            return data;
        }

        public async Task<APIResponse> VendorInvoiceApprovedRollback(List<VendorInvoice> vendors, Int32 createdBy)
        {
            var data = await _vendorConfirmationAccess.VendorInvoiceApprovedRollback(vendors, createdBy);
            return data;
        }

        public async Task<APIResponse> LGInvoiceEmailSendToVendorStatusUpdate(VendorInvoice vendor, Int32 createdBy)
        {
            var data = await _vendorConfirmationAccess.LGInvoiceEmailSendToVendorStatusUpdate(vendor, createdBy);
            return data;
        }





        public async Task<APIResponse> VendorClaimsAmount(VendorInvoice vendorClaims)
        {
            var data = await _vendorConfirmationAccess.VendorClaimsAmount(vendorClaims);
            return data;
        }


        public async Task<VendorInvoice> ReadVendorClaimsAmount(VendorInvoice vendorInvoice)
        {
            var data = await _vendorConfirmationAccess.ReadVendorClaimsAmount(vendorInvoice);
            return data;
        }

        public async Task<APIResponse> VendorInvoiceAddedByVendor(List<VendorInvoice> vendorInvoices)
        {
            var data = await _vendorConfirmationAccess.VendorInvoiceAddedByVendor(vendorInvoices);
            return data;
        }

        public async Task<List<VendorInvoice>> ReadVendorInvoiceAddedByVendor(VendorInvoice vendorInvoice)
        {
            var data = await _vendorConfirmationAccess.ReadVendorInvoiceAddedByVendor(vendorInvoice);
            return data;
        }



        public async Task<List<VendorInvoice>> ReadVendorConfirmationClaims(VendorInvoice vendorInvoice)
        {
            var data = await _vendorConfirmationAccess.ReadVendorConfirmationClaims(vendorInvoice);
            return data;
        }

        public async Task<APIResponse> VendorInvoiceClaimReplied(List<VendorInvoice> vendorInvoices)
        {
            var data = await _vendorConfirmationAccess.VendorInvoiceClaimReplied(vendorInvoices);
            return data;
        }


        
        public async Task<APIResponse> VendorInvoiceAcceptance(VendorInvoice vendorAcceptance)
        {
            var data = await _vendorConfirmationAccess.VendorInvoiceAcceptance(vendorAcceptance);
            return data;
        }


        
        public async Task<List<VendorInvoice>> ReadVendorConfirmationTracker(VendorInvoice confirmationTracker)
        {
            var data = await _vendorConfirmationAccess.ReadVendorConfirmationTracker(confirmationTracker);
            return data;
        }

    }
}
