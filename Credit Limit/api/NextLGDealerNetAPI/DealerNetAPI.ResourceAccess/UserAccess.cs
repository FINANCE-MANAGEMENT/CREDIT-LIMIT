using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess
{
    public class UserAccess : IUserAccess
    {
        ICommonDB _commonDB = null;
        private readonly string DealerNet_Next_Connection = "DealerNetNextConnection";

        public UserAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        public async Task<List<Role>> ReadRoles()
        {
            //Utilities.CreateLogFile(Utilities.PROJECTS.NextDNET, "ReadRoles() Data Access Layer call...");
            List<Role> lstRoles = new List<Role>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[1];
                arrParams[0] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[0].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Roles.READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Role role = new Role();
                    role.RoleId = SafeTypeHandling.ConvertStringToInt32(row["ROLE_ID"]);
                    role.RoleName = SafeTypeHandling.ConvertToString(row["USER_ROLE_NAME"]);
                    role.RoleDesc = SafeTypeHandling.ConvertToString(row["USER_ROLE_DESC"]);
                    lstRoles.Add(role);
                }
                //Utilities.CreateLogFile(Utilities.PROJECTS.NextDNET, "ReadRoles() Data Access Layer Success Load data...");
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.NextDNET, ex);
                throw;
            }
            return lstRoles;
        }

        /// <summary>
        /// User login portal with UserName & Password
        /// </summary>
        /// <param name="userLogins"></param>
        /// <returns></returns>
        public Users UserLogin(UserLogins userLogins)
        {
            Users users = new Users();
            try
            {

                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_USERNAME", OracleDbType.Varchar2);
                arrParams[0].Value = userLogins.LoginId;
                arrParams[1] = new OracleParameter("P_PASSWORD", OracleDbType.Varchar2);
                arrParams[1].Value = Utilities.Encrypt_SHA512(userLogins.Password);
                arrParams[2] = new OracleParameter("P_IP_ADDRESS", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(userLogins.IPAddress))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = userLogins.IPAddress;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                arrParams[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;

                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.DNET.Users.USER_LOGIN, arrParams, DealerNet_Next_Connection);
                APIResponse response = new APIResponse
                {
                    Status = arrParams[4].Value.ToString(),
                    StatusDesc = arrParams[5].Value.ToString(),
                };

                if (response.Status == Utilities.SUCCESS)
                {
                    users.UserId = SafeTypeHandling.ConvertStringToInt32(dtData.Rows[0]["ID"]);
                    users.LoginID = SafeTypeHandling.ConvertToString(dtData.Rows[0]["LOGIN_ID"]);
                    users.LoginName = SafeTypeHandling.ConvertToString(dtData.Rows[0]["LOGIN_NAME"]);
                    users.ChangePwdStatus = SafeTypeHandling.ConvertStringToBoolean(dtData.Rows[0]["CHANGE_PASSWORD_STATUS"]);

                    users.Zone.ZoneCode = SafeTypeHandling.ConvertToString(dtData.Rows[0]["ZONE"]);
                    users.Zone.Region.RegionCode = SafeTypeHandling.ConvertToString(dtData.Rows[0]["REGION"]);
                    users.Zone.Region.Branch.BranchCode = SafeTypeHandling.ConvertToString(dtData.Rows[0]["BRANCH"]);
                    users.Zone.Region.Branch.CTRL_AU = SafeTypeHandling.ConvertToString(dtData.Rows[0]["CTRL_AU"]);
                    //users.Zone.Region.Branch.BranchAddress = SafeTypeHandling.ConvertToString(dtData.Rows[0]["BRANCH_ADDRESS"]);

                    string base64StringEmail = Convert.ToBase64String(Encoding.ASCII.GetBytes(SafeTypeHandling.ConvertToString(dtData.Rows[0]["LOGIN_EMAIL_ID"])));
                    users.EmailId = base64StringEmail;

                    users.MobileNo = SafeTypeHandling.ConvertToString(dtData.Rows[0]["CONTACT_NO1"]);

                    users.Role.RoleId = SafeTypeHandling.ConvertStringToInt32(dtData.Rows[0]["USER_ROLE_ID"]);
                    users.Role.RoleName = SafeTypeHandling.ConvertToString(dtData.Rows[0]["USER_ROLE_NAME"]);
                    //users.Role.RoleDesc = SafeTypeHandling.ConvertToString(dtData.Rows[0]["USER_ROLE_DESC"]);

                    //users.ClientIP = SafeTypeHandling.ConvertToString(dtData.Rows[0]["CLIENT_IP"]);

                    var base64EncodedBytes = Convert.FromBase64String(base64StringEmail);
                    //users.Status = Encoding.UTF8.GetString(base64EncodedBytes);
                }
                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// User Forgot Password
        /// </summary>
        /// <param name="userLogins"></param>
        /// <returns></returns>
        public async Task<APIResponse> UserForgotPassword(UserLogins userLogins)
        {
            APIResponse response;
            userLogins.Password = Guid.NewGuid().ToString().Substring(0, 8);

            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_USERNAME", OracleDbType.Varchar2, 500);
                if (string.IsNullOrEmpty(userLogins.LoginId))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = userLogins.LoginId;
                }
                arrParams[1] = new OracleParameter("P_ORIGINAL_PASSWORD", OracleDbType.Varchar2, 500);
                arrParams[1].Value = userLogins.Password;
                arrParams[2] = new OracleParameter("P_PASSWORD", OracleDbType.Varchar2, 500);
                arrParams[2].Value = Utilities.Encrypt_SHA512(userLogins.Password);
                arrParams[3] = new OracleParameter("P_IP_ADDRESS", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(userLogins.IPAddress))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = userLogins.IPAddress;
                }
                arrParams[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Users.USER_FORGOT_PASSWORD, arrParams, DealerNet_Next_Connection);
                response = new APIResponse()
                {
                    Status = arrParams[4].Value.ToString(),
                    StatusDesc = arrParams[5].Value.ToString(),
                };
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.NextDNET, ex);
                response = new APIResponse()
                {
                    Status = Utilities.ERROR,
                    StatusDesc = Utilities.SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD,
                };
            }
            return response;
        }

        /// <summary>
        /// User Login with SSO concept, via DealerNet Portal
        /// </summary>
        /// <param name="userLogins"></param>
        /// <returns></returns>
        public Users ValidateSSO(UserLogins userLogins)
        {
            Users users = new Users();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_USERNAME", OracleDbType.Varchar2);
                arrParams[0].Value = userLogins.LoginId;
                arrParams[1] = new OracleParameter("P_AUTH_TOKEN", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(userLogins.AuthToken))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = userLogins.AuthToken;
                }
                arrParams[2] = new OracleParameter("P_IP_ADDRESS", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(userLogins.IPAddress))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = userLogins.IPAddress;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;

                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.DNET.Users.SSO_VALIDATE, arrParams);
                users.UserId = SafeTypeHandling.ConvertStringToInt32(dtData.Rows[0]["ID"]);
                users.LoginID = SafeTypeHandling.ConvertToString(dtData.Rows[0]["LOGIN_ID"]);
                users.LoginName = SafeTypeHandling.ConvertToString(dtData.Rows[0]["LOGIN_NAME"]);

                users.Zone.ZoneCode = SafeTypeHandling.ConvertToString(dtData.Rows[0]["ZONE"]);
                users.Zone.Region.RegionCode = SafeTypeHandling.ConvertToString(dtData.Rows[0]["REGION"]);
                users.Zone.Region.Branch.BranchCode = SafeTypeHandling.ConvertToString(dtData.Rows[0]["BRANCH"]);
                users.Zone.Region.Branch.CTRL_AU = SafeTypeHandling.ConvertToString(dtData.Rows[0]["CTRL_AU"]);

                string base64StringEmail = Convert.ToBase64String(Encoding.UTF8.GetBytes(SafeTypeHandling.ConvertToString(dtData.Rows[0]["LOGIN_EMAIL_ID"])));
                users.EmailId = base64StringEmail;
                users.Role.RoleId = SafeTypeHandling.ConvertStringToInt32(dtData.Rows[0]["USER_ROLE_ID"]);
                users.Role.RoleName = SafeTypeHandling.ConvertToString(dtData.Rows[0]["USER_ROLE_NAME"]);
                //users.Role.RoleDesc = SafeTypeHandling.ConvertToString(dtData.Rows[0]["USER_ROLE_DESC"]);

                //users.ClientIP = SafeTypeHandling.ConvertToString(dtData.Rows[0]["CLIENT_IP"]);

                var base64EncodedBytes = Convert.FromBase64String(base64StringEmail);
                //users.Status = Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.NextDNET, ex);
            }
            return users;
        }

        public Users ReadUserDetailByUser(UserLogins userLogins)
        {
            Users users = new Users();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_USERNAME", OracleDbType.Varchar2);
                arrParams[0].Value = userLogins.LoginId;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.DNET.Users.READ_USER_DETAILS_BY_USER, arrParams);
                users.UserId = SafeTypeHandling.ConvertStringToInt32(dtData.Rows[0]["ID"]);
                users.LoginID = SafeTypeHandling.ConvertToString(dtData.Rows[0]["LOGIN_ID"]);
                users.LoginName = SafeTypeHandling.ConvertToString(dtData.Rows[0]["LOGIN_NAME"]);


                users.Zone.ZoneCode = SafeTypeHandling.ConvertToString(dtData.Rows[0]["ZONE"]);
                users.Zone.Region.RegionCode = SafeTypeHandling.ConvertToString(dtData.Rows[0]["REGION"]);
                users.Zone.Region.Branch.BranchCode = SafeTypeHandling.ConvertToString(dtData.Rows[0]["BRANCH"]);


                string base64StringEmail = Convert.ToBase64String(Encoding.UTF8.GetBytes(SafeTypeHandling.ConvertToString(dtData.Rows[0]["LOGIN_EMAIL_ID"])));
                users.EmailId = base64StringEmail;
                users.Role.RoleId = SafeTypeHandling.ConvertStringToInt32(dtData.Rows[0]["USER_ROLE_ID"]);
                users.Role.RoleName = SafeTypeHandling.ConvertToString(dtData.Rows[0]["USER_ROLE_NAME"]);
                //users.Role.RoleDesc = SafeTypeHandling.ConvertToString(dtData.Rows[0]["USER_ROLE_DESC"]);


                var base64EncodedBytes = Convert.FromBase64String(base64StringEmail);
                //users.Status = Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception ex)
            {
                throw;
            }
            return users;
        }

        public async Task<List<Users>> ReadUsersByRoleId(int roleId)
        {
            List<Users> lstUsers = new List<Users>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_ROLEID", OracleDbType.Varchar2);
                arrParams[0].Value = roleId;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Users.READ_USER_BY_ROLE, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Users users = new Users();
                    users.UserId = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    users.LoginID = SafeTypeHandling.ConvertToString(row["LOGIN_ID"]);
                    users.LoginName = SafeTypeHandling.ConvertToString(row["LOGIN_NAME"]);
                    users.Zone.Region.Branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    users.Zone.Region.RegionCode = SafeTypeHandling.ConvertToString(row["REGION"]);
                    users.Role.RoleId = SafeTypeHandling.ConvertStringToInt32(row["USER_ROLE_ID"]);
                    users.Role.RoleName = SafeTypeHandling.ConvertToString(row["USER_ROLE_NAME"]);
                    lstUsers.Add(users);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstUsers;
        }

    }
}
