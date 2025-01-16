using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IStateBusinessLogic
    {
        Task<APIResponse> SaveState(State state);

        Task<List<State>> ReadState(State state);
    }
}
