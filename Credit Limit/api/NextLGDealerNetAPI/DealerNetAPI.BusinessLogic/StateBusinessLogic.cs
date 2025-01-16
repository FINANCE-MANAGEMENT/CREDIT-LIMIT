using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic
{
    public class StateBusinessLogic : IStateBusinessLogic
    {
        private readonly IStateAccess _stateAccess = null;
        public StateBusinessLogic(IStateAccess stateAccess)
        {
            _stateAccess = stateAccess;
        }

        public async Task<APIResponse> SaveState(State state)
        {
            var data = await _stateAccess.SaveState(state);
            return data;
        }

        public async Task<List<State>> ReadState(State state)
        {
            var data = await _stateAccess.ReadState(state);
            return data;
        }

    }
}
