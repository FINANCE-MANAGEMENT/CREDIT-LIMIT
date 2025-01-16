using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface ILookupAccess
    {
        Task<APIResponse> SaveLookup(Lookup lookup);

        Task<List<Lookup>> ReadLookup(Lookup lookup);

        Task<List<Lookup>> ReadLookupTypes(string SystemName);
    }
}
