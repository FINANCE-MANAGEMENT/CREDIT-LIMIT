using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface IDistrictAccess
    {
        Task<APIResponse> SaveDistrict(District district);

        Task<List<District>> ReadDistrict(District district);

    }
}
