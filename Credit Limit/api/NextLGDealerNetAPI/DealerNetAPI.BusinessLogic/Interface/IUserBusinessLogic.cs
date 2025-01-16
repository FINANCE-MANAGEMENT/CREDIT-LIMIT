using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IUserBusinessLogic
    {
        Task<List<Role>> ReadRoles();
        Users UserLogin(UserLogins userLogins);
        Task<APIResponse> UserForgotPassword(UserLogins userLogins);
        Users ValidateSSO(UserLogins userLogins);
        Users ReadUserDetailByUser(UserLogins userLogins);
        List<Users> ReadUsers();

        Task<List<Users>> ReadUsersByRoleId(int roleId);
        
        
    }
}
