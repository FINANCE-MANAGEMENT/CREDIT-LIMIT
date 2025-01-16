using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.VMS
{
    public class VendorCommunicationBusinessLogic : IVendorCommunicationBusinessLogic
    {
        private readonly IVendorCommunicationAccess _vendorCommunicationAccess = null;

        public VendorCommunicationBusinessLogic(IVendorCommunicationAccess vendorCommunicationAccess)
        {
            _vendorCommunicationAccess = vendorCommunicationAccess;
        }

        public async Task<APIResponse> VendorCommunicationTemplateRegistration(VendorCommunication vendorCommunication)
        {
            var data = await _vendorCommunicationAccess.VendorCommunicationTemplateRegistration(vendorCommunication);
            return data;
        }

        public async Task<List<VendorCommunication>> ReadVendorCommunicationTemplate(VendorCommunication vendorCommunication)
        {
            var data = await _vendorCommunicationAccess.ReadVendorCommunicationTemplate(vendorCommunication);
            return data;
        }

        public async Task<List<Lookup>> ReadCommunicationTemplateRequiredInfo(VendorCommunication vendorCommunication)
        {
            var data = await _vendorCommunicationAccess.ReadCommunicationTemplateRequiredInfo(vendorCommunication);
            return data;
        }

        public async Task<APIResponse> CommunicationSendToVendor(VendorCommunication vendorCommunication)
        {
            var data = await _vendorCommunicationAccess.CommunicationSendToVendor(vendorCommunication);
            return data;
        }

        public async Task<APIResponse> VendorUploadForCommunication(List<Vendor> vendors)
        {
            var data = await _vendorCommunicationAccess.VendorUploadForCommunication(vendors);
            return data;
        }

        public async Task<List<VendorCommunication>> ReadVendorCommunicationSendTemplate(VendorCommunication vendorCommunication)
        {
            var data = await _vendorCommunicationAccess.ReadVendorCommunicationSendTemplate(vendorCommunication);
            return data;
        }

        public async Task<APIResponse> VendorCommunicationAcceptance(VendorCommunication vendorCommunication)
        {
            var data = await _vendorCommunicationAccess.VendorCommunicationAcceptance(vendorCommunication);
            return data;
        }

    }
}
