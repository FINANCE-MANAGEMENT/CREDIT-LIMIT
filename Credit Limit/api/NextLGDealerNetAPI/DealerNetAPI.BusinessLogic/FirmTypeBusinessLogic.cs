using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic
{
    public class FirmTypeBusinessLogic : IFirmTypeBusinessLogic
    {
        private readonly IFirmTypeAccess _firmtypeAccess = null;
        public FirmTypeBusinessLogic(IFirmTypeAccess firmtypeAccess)
        {
            _firmtypeAccess = firmtypeAccess;
        }

        public async Task<APIResponse> SaveFirmType(FirmType firmtype)
        {
            var data = await _firmtypeAccess.SaveFirmType(firmtype);
            return data;
        }

        public async Task<List<FirmType>> ReadFirmType(FirmType firmtype)
        {
            var data = await _firmtypeAccess.ReadFirmType(firmtype);
            return data;
        }

    }
}
