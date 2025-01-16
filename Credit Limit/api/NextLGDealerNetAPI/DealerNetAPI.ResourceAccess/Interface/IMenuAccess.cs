using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface IMenuAccess
    {
        Task<APIResponse> SaveMenu(Menu menu);

        Task<List<Menu>> ReadMenu(Menu menu);

        Task<APIResponse> SaveTaskMapping(List<Menu> task);

        Task<List<Menu>> ReadTaskMapping(Menu menu);

        Task<List<Menu>> ReadUserTaskAssign(Menu menu);

        Task<APIResponse> MenuUtilization(Menu menu);

    }
}
