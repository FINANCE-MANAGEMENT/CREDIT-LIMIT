using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface IVendorVMSAccess
    {
        Task<APIResponse> SaveUpdateVendor(Vendor vendor);

        Task<APIResponse> SaveBulkVendor(List<Vendor> vendor);

        Task<List<Vendor>> ReadVendor(Vendor vendor);

        Task<List<Branch>> ReadPICRequiredBranch();

        Task<List<Users>> ReadPIC_Members(Branch branch);

        Task<APIResponse> VendorProfileUpdateRequest(Vendor vendor);
        Task<List<Vendor>> ReadVendorProfileUpdateRequests(Vendor vendor);

        Task<APIResponse> VendorProfileUpdateRequestApproval(Vendor vendor);


    }
}
