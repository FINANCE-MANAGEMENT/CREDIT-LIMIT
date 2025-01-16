using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealerNetAPI.Areas.ChannelFinance
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StateMasterController : ControllerBase
    {
        //private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IStateBusinessLogic _stateBusinessLogic = null;
        public StateMasterController(IConfiguration configuration, IStateBusinessLogic stateBusinessLogic)
        {
            _configuration = configuration;
            _stateBusinessLogic = stateBusinessLogic;
        }

        [HttpPost]
        public async Task<IActionResult> SaveState([FromBody] State state)
        {
            try
            {
                if (string.IsNullOrEmpty(state.StateCode) || string.IsNullOrEmpty(state.StateName) || string.IsNullOrEmpty(state.Status))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _stateBusinessLogic.SaveState(state);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadState([FromBody] State state)
        {
            try
            {
                var data = await _stateBusinessLogic.ReadState(state);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
    }
}
