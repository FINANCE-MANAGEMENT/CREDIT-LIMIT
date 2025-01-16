using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic
{
    public class LookupBusinessLogic : ILookupBusinessLogic
    {
        private readonly ILookupAccess _lookupAccess = null;
        public LookupBusinessLogic(ILookupAccess lookupAccess)
        {
            _lookupAccess = lookupAccess;
        }

        public async Task<APIResponse> SaveLookup(Lookup lookup)
        {
            var data = await _lookupAccess.SaveLookup(lookup);
            return data;
        }

        public async Task<List<Lookup>> ReadLookup(Lookup lookup)
        {
            var data = await _lookupAccess.ReadLookup(lookup);
            return data;
        }

        public async Task<List<Lookup>> ReadLookupTypes(string SystemName)
        {
            var data = await _lookupAccess.ReadLookupTypes(SystemName);
            return data;
        }
    }
}




