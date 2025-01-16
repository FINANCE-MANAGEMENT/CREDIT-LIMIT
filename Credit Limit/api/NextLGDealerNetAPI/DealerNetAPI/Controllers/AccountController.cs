using DealerNetAPI.BusinessLogic;
using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.Helper;
using DealerNetAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DealerNetAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IUserBusinessLogic _userBusinessLogic = null;
        public AccountController(JwtSettings jwtSettings, IConfiguration configuration, IUserBusinessLogic userBusinessLogic)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _userBusinessLogic = userBusinessLogic;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> ReadRoles()
        {
            try
            {
                var data = await _userBusinessLogic.ReadRoles();
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.NextDNET, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// User Login with UserName & Password and Generate Token
        /// </summary>
        /// <param name="userLogins"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UserLogin([FromBody] UserLogins userLogins)
        {
            try
            {
                if (userLogins == null)
                    return BadRequest();
                // Check mandatory fields.
                if (string.IsNullOrEmpty(userLogins.LoginId) || string.IsNullOrEmpty(userLogins.Password))
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Please enter correct user Credential!!" }));

                UserTokens Token = new UserTokens();
                Users users = _userBusinessLogic.UserLogin(userLogins);
                if (!string.IsNullOrEmpty(users.LoginID))
                {
                    Token = JwtHelpers.GenTokenkey(new UserTokens()
                    {

                        EmailId = users.EmailId,
                        MobileNo = users.MobileNo,
                        GuidId = Guid.NewGuid(),
                        UserId = users.UserId,
                        Role = users.Role,
                    }, _jwtSettings, _configuration, users.LoginID);

                    Token.LoginId = users.LoginID;
                    Token.RefreshToken = CreateRefreshToken();
                    Token.Role = users.Role;
                    Token.LoginName = users.LoginName;
                    Token.Zone = users.Zone;
                    Token.ChangePwdStatus = users.ChangePwdStatus;
                    return StatusCode(StatusCodes.Status200OK, new APIResponse { Status = Utilities.SUCCESS, data = Token });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.AUTHENTICATION_FAILED }));
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.AUTHENTICATION_FAILED }));
            }
        }

        /// <summary>
        /// User Forgot Password
        /// </summary>
        /// <param name="userLogins"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UserForgotPassword([FromBody] UserLogins userLogins)
        {
            try
            {
                if (userLogins == null)
                    return BadRequest();

                APIResponse result = await _userBusinessLogic.UserForgotPassword(userLogins);
                if (result.Status == Utilities.SUCCESS)
                {
                    return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest), result));
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.SOMETHING_WENT_WRONG }));
            }
        }

        /// <summary>
        /// SSO User authenticate via DealerNet Portal (param1 & param2) and Generate Token  ,SSO login concept
        /// </summary>
        /// <param name="userLogins"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Authenticate([FromBody] UserLogins userLogins)
        {
            try
            {
                if (userLogins == null)
                    return BadRequest();
                // Check mandatory fields.
                if (string.IsNullOrEmpty(userLogins.LoginId))
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));

                UserTokens Token = new UserTokens();
                //userLogins.IPAddress = GetIPAddress();
                Users users = _userBusinessLogic.ValidateSSO(userLogins);
                if (!string.IsNullOrEmpty(users.LoginID))
                {
                    Token = JwtHelpers.GenTokenkey(new UserTokens()
                    {

                        EmailId = users.EmailId,
                        MobileNo = users.MobileNo,
                        GuidId = Guid.NewGuid(),
                        UserId = users.UserId,
                        Role = users.Role,
                    }, _jwtSettings, _configuration, users.LoginID);

                    Token.LoginId = users.LoginID;
                    Token.RefreshToken = CreateRefreshToken();
                    Token.Role = users.Role;
                    Token.LoginName = users.LoginName;
                    Token.Zone = users.Zone;
                    return StatusCode(StatusCodes.Status200OK, new APIResponse { Status = Utilities.SUCCESS, data = Token });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.SERVER_AUTHENTICATION_FAILED }));
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.NextDNET, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.AUTHENTICATION_FAILED }));
            }
        }

        /// <summary>
        /// EP authenticate User and Generate Token
        /// </summary>
        /// <param name="userLogins"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EPAuthenticate([FromBody] UserLogins userLogins)
        {
            try
            {
                if (userLogins == null)
                    return BadRequest();
                // Check mandatory fields.
                if (string.IsNullOrEmpty(userLogins.LoginId))
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));

                UserTokens Token = new UserTokens();
                Users users = _userBusinessLogic.ReadUserDetailByUser(userLogins);
                if (!string.IsNullOrEmpty(users.LoginID))
                {
                    Token = JwtHelpers.GenTokenkey(new UserTokens()
                    {
                        EmailId = users.EmailId,
                        MobileNo = users.MobileNo,
                        GuidId = Guid.NewGuid(),
                        UserId = users.UserId,
                        Role = users.Role,
                    }, _jwtSettings, _configuration, users.LoginID);

                    return StatusCode(StatusCodes.Status200OK, new APIResponse { Status = Utilities.SUCCESS, data = Token });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.SERVER_AUTHENTICATION_FAILED }));
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.AUTHENTICATION_FAILED }));
            }
        }

        /// <summary>
        /// Refresh Token API
        /// </summary>
        /// <param name="userTokens"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RefreshToken([FromBody] UserTokens userTokens)
        {
            try
            {
                if (userTokens == null)
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                            new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_CLIENT_REQUEST }));
                string accessToken = userTokens.Token;
                string refreshToken = userTokens.RefreshToken;
                var principal = JwtHelpers.GetPrincipleFromExpiredToken(_jwtSettings, accessToken);
                var username = principal.Identity.Name;
                DateTime tokenExpiredTime;
                try
                {
                    tokenExpiredTime = Convert.ToDateTime(principal.Claims.Where(m => m.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration").FirstOrDefault().Value);
                }
                catch (Exception)
                {
                    throw;
                }

                // Check token details of user from database.
                var user = new UserTokens();
                user.RefreshToken = refreshToken; // this line only for testing, after testing remove.
                
                //if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                if (user is null || user.RefreshToken != refreshToken || tokenExpiredTime.Date != DateTime.Now.Date)
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                            new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.SESSION_EXPIRED }));

                UserTokens finalToken = new UserTokens();
                finalToken = JwtHelpers.GenTokenkey(new UserTokens()
                {
                    EmailId = userTokens.EmailId,
                    MobileNo = userTokens.MobileNo,
                    GuidId = Guid.NewGuid(),
                    UserId = userTokens.UserId,
                    Role = userTokens.Role,
                }, _jwtSettings, _configuration, username);
                finalToken.RefreshToken = CreateRefreshToken();
                return StatusCode(StatusCodes.Status200OK, new APIResponse { Status = Utilities.SUCCESS, data = finalToken });
            }
            catch (Exception ex)
            {
                string[] errarr = ex.ToString().Split("'");
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = errarr[0] }));
            }

        }

        /// <summary>
        /// Create Refresh Token unique
        /// </summary>
        /// <returns></returns>
        private string CreateRefreshToken()
        {
            byte[] tokenBytes = new byte[32];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            //Check "refreshToken" is assigned to any user in DB
            var tokenInUser = false;
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> ReadUsersByRoleId(int roleId)
        {
            try
            {
                // Check mandatory fields.
                if (roleId == 0)
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));

                var data = await _userBusinessLogic.ReadUsersByRoleId(roleId);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Get User IP
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserIP()
        {
            try
            {
                return StatusCode(StatusCodes.Status200OK, new APIResponse
                {
                    Status = Utilities.SUCCESS,
                    data = new
                    {
                        LocalIP = GetIPAddress(),
                        //PublicIP = Utilities.GetPublicIPAddress()
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new APIResponse
                {
                    Status = Utilities.ERROR,
                    data = new { message = "Some error occured, while fetching the client IP Address." }
                });
            }
        }

        private string GetIPAddress()
        {
            string IpAddress = Response.HttpContext.Connection.RemoteIpAddress.ToString();

            //127.0.0.1    localhost
            //::1          localhost
            if (IpAddress == "::1")
            {
                IpAddress = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[1].ToString();
            }
            return IpAddress;
        }



    }
}
