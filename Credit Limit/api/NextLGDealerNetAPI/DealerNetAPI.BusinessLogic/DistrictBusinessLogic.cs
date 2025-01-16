using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic
{
    public class DistrictBusinessLogic : IDistrictBusinessLogic

    {
        private readonly IDistrictAccess _districtAccess = null;
        public DistrictBusinessLogic(IDistrictAccess districtAccess)
        {
            _districtAccess = districtAccess;
        }

        public async Task<APIResponse> SaveDistrict(District district)
        {
            var data = await _districtAccess.SaveDistrict(district);
            return data;
        }

        public async Task<List<District>> ReadDistrict(District district)
        {
            var data = await _districtAccess.ReadDistrict(district);
            return data;
        }
    }

    
}
