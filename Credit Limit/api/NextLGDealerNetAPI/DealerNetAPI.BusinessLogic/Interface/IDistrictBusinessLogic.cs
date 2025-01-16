using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IDistrictBusinessLogic
    {
        Task<APIResponse> SaveDistrict(District district);

        Task<List<District>> ReadDistrict(District district);
    }
}