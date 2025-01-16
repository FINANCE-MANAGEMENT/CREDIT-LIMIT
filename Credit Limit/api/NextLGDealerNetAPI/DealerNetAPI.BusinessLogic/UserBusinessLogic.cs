using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic
{
    public class UserBusinessLogic : IUserBusinessLogic
    {
        private readonly IUserAccess _userAccess = null;
        public UserBusinessLogic(IUserAccess userAccess)
        {
            _userAccess = userAccess;
        }

        public UserBusinessLogic(string ConnStr)
        {

        }

        public List<Users> ReadUsers()
        {
            List<Users> users = null;
            //UserAccess userAccess = new UserAccess(m_database);
            //users = userAccess.ReadUsers();
            return users;
        }

        public async Task<List<Role>> ReadRoles()
        {
            var data = await _userAccess.ReadRoles();
            return data;
        }

        public Users UserLogin(UserLogins userLogins)
        {
            var data = _userAccess.UserLogin(userLogins);
            return data;
        }

        public async Task<APIResponse> UserForgotPassword(UserLogins userLogins)
        {
            var data = await _userAccess.UserForgotPassword(userLogins);
            return data;
        }

        public Users ValidateSSO(UserLogins userLogins)
        {
            var data = _userAccess.ValidateSSO(userLogins);
            return data;
        }

        public Users ReadUserDetailByUser(UserLogins userLogins)
        {
            var data = _userAccess.ReadUserDetailByUser(userLogins);
            return data;
        }

        public async Task<List<Users>> ReadUsersByRoleId(int roleId)
        {
            var data = await _userAccess.ReadUsersByRoleId(roleId);
            return data;
        }

       






    }
}
