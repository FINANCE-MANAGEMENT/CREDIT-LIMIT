using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic
{
    public class MenuBusinessLogic : IMenuBusinessLogic
    {
        private readonly IMenuAccess _menuAccess = null;
        public MenuBusinessLogic(IMenuAccess menuAccess)
        {
            _menuAccess = menuAccess;
        }

        public async Task<APIResponse> SaveMenu(Menu menu)
        {
            var data = await _menuAccess.SaveMenu(menu);
            return data;
        }

        public async Task<List<Menu>> ReadMenu(Menu menu)
        {
            var data = await _menuAccess.ReadMenu(menu);
            return data;
        }
    

        public async Task<APIResponse> SaveTaskMapping(List<Menu> task)
        {
            var data = await _menuAccess.SaveTaskMapping(task);
            return data;
        }

        public async Task<List<Menu>> ReadTaskMapping(Menu menu)
        {
            var data = await _menuAccess.ReadTaskMapping(menu);
            return data;
        }

        public async Task<List<Menu>> ReadUserTaskAssign(Menu menu)
        {
            var data = await _menuAccess.ReadUserTaskAssign(menu);
            return data;
        }


        public async Task<APIResponse> MenuUtilization(Menu menu)
        {
            var data = await _menuAccess.MenuUtilization(menu);
            return data;
        }

    }
}
