using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IVendorCommunicationBusinessLogic
    {
        Task<APIResponse> VendorCommunicationTemplateRegistration(VendorCommunication vendorCommunication);

        Task<List<VendorCommunication>> ReadVendorCommunicationTemplate(VendorCommunication vendorCommunication);

        Task<List<Lookup>> ReadCommunicationTemplateRequiredInfo(VendorCommunication vendorCommunication);

        Task<APIResponse> CommunicationSendToVendor(VendorCommunication vendorCommunication);

        Task<APIResponse> VendorUploadForCommunication(List<Vendor> vendors);

        Task<List<VendorCommunication>> ReadVendorCommunicationSendTemplate(VendorCommunication vendorCommunication);

        Task<APIResponse> VendorCommunicationAcceptance(VendorCommunication vendorCommunication);

    }
}
