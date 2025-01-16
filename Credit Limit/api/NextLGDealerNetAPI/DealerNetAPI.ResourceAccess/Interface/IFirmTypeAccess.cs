using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface IFirmTypeAccess
    {

        Task<APIResponse> SaveFirmType(FirmType firmtype);

        Task<List<FirmType>> ReadFirmType(FirmType firmtype);
    }
}
