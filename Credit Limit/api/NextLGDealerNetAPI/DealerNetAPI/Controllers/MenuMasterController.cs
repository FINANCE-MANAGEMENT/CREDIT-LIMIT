using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealerNetAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MenuMasterController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IMenuBusinessLogic _menuBusinessLogic = null;
        public MenuMasterController(IConfiguration configuration, IMenuBusinessLogic menuBusinessLogic, JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _menuBusinessLogic = menuBusinessLogic;
        }

        /// <summary>
        /// Save/ Update menu
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveMenu([FromBody] Menu menu)
        {
            try
            {
                if (string.IsNullOrEmpty(menu.MenuName) || string.IsNullOrEmpty(menu.MenuDesc))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _menuBusinessLogic.SaveMenu(menu);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Read menu
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadMenu([FromBody] Menu menu)
        {
            try
            {
                // Read Header details
                //var ddd = HttpContext.Request.Headers.TryGetValue("Authorization", out var headerValue);
                var result = await _menuBusinessLogic.ReadMenu(menu);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTaskMapping([FromBody] List<Menu> task)
        {
            try
            {
                if (task == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _menuBusinessLogic.SaveTaskMapping(task);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReadTaskMapping()
        {
            try
            {
                // Read Header details
                var isAuthorizationInHeader = HttpContext.Request.Headers.TryGetValue("Authorization", out var headerBearerTokenValue);

                var principal = Helper.JwtHelpers.GetPrincipleFromExpiredToken(_jwtSettings, headerBearerTokenValue.ToString().Replace("Bearer ", ""));

                var userIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                var claims = userIdentity.Claims;
                var roleClaimType = userIdentity.RoleClaimType;
                var roles = claims.Where(c => c.Type == roleClaimType).FirstOrDefault();

                string userId = claims.Where(c => c.Type == "Id").FirstOrDefault().Value; // User Id
                string username = principal.Identity.Name; // User Name
                string userRoleId = roles.Value; // User RoleId

                Menu _menu = new Menu();
                _menu.UserId = Convert.ToInt32(userId);
                _menu.RoleId = Convert.ToInt32(userRoleId);

                var result = await _menuBusinessLogic.ReadTaskMapping(_menu);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReadUserTaskAssign([FromBody] Menu menu)
        {
            try
            {
                // Read Header details
                //var ddd = HttpContext.Request.Headers.TryGetValue("Authorization", out var headerValue);
                var result = await _menuBusinessLogic.ReadUserTaskAssign(menu);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        [HttpPost]
        public async Task<IActionResult> MenuUtilization([FromBody] Menu menu)
        {
            try
            {
                if (string.IsNullOrEmpty(menu.MenuName) || string.IsNullOrEmpty(menu.MenuURL) || string.IsNullOrEmpty(menu.ApplicationName)
                    || string.IsNullOrEmpty(menu.LoginID))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _menuBusinessLogic.MenuUtilization(menu);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
              

    }
}
