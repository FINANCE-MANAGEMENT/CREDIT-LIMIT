using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.VMS
{
    public class VendorVMSBusinessLogic : IVendorVMSBusinessLogic
    {
        private readonly IVendorVMSAccess _vendorVMSAccess = null;
        public VendorVMSBusinessLogic(IVendorVMSAccess vendorVMSAccess)
        {
            _vendorVMSAccess = vendorVMSAccess;
        }

        public async Task<APIResponse> SaveUpdateVendor(Vendor vendor)
        {
            var data = await _vendorVMSAccess.SaveUpdateVendor(vendor);
            return data;
        }

        public async Task<APIResponse> SaveBulkVendor(List<Vendor> vendor)
        {
            var data = await _vendorVMSAccess.SaveBulkVendor(vendor);
            return data;
        }

        public async Task<List<Vendor>> ReadVendor(Vendor vendor)
        {
            var data = await _vendorVMSAccess.ReadVendor(vendor);
            return data;
        }

        public async Task<List<Branch>> ReadPICRequiredBranch()
        {
            var data = await _vendorVMSAccess.ReadPICRequiredBranch();
            return data;
        }

        public async Task<List<Users>> ReadPIC_Members(Branch branch)
        {
            var data = await _vendorVMSAccess.ReadPIC_Members(branch);
            return data;
        }


        public async Task<APIResponse> VendorProfileUpdateRequest(Vendor vendor)
        {
            var data = await _vendorVMSAccess.VendorProfileUpdateRequest(vendor);
            return data;
        }

        public async Task<List<Vendor>> ReadVendorProfileUpdateRequests(Vendor vendor)
        {
            var data = await _vendorVMSAccess.ReadVendorProfileUpdateRequests(vendor);
            return data;
        }


        public async Task<APIResponse> VendorProfileUpdateRequestApproval(Vendor vendor)
        {
            var data = await _vendorVMSAccess.VendorProfileUpdateRequestApproval(vendor);
            return data;
        }

    }
}
