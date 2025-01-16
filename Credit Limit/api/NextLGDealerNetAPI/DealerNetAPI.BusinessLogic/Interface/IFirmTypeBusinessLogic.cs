using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IFirmTypeBusinessLogic
    {
        Task<APIResponse> SaveFirmType(FirmType firmtype);

        Task<List<FirmType>> ReadFirmType(FirmType firmtype);
    }
}
