using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface IStateAccess
    {
        Task<APIResponse> SaveState(State state);

        Task<List<State>> ReadState(State state);
    }
}
