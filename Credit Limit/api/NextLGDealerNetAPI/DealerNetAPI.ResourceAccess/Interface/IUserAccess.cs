using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface IUserAccess
    {
        Task<List<Role>> ReadRoles();
        Users UserLogin(UserLogins userLogins);
        Task<APIResponse> UserForgotPassword(UserLogins userLogins);
        Users ValidateSSO(UserLogins userLogins);
        Users ReadUserDetailByUser(UserLogins userLogins);
        Task<List<Users>> ReadUsersByRoleId(int roleId);
        //Task<List<Users>> ReadUsersForAUMappingByRoleId(int roleId);

    }
}
