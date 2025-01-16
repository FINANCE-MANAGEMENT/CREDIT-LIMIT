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
    public class FirmTypeMasterCFController : ControllerBase
    {
        //private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IFirmTypeBusinessLogic _firmtypeBusinessLogic = null;
        public FirmTypeMasterCFController(IConfiguration configuration, IFirmTypeBusinessLogic firmtypeBusinessLogic)
        {
            _configuration = configuration;
            _firmtypeBusinessLogic = firmtypeBusinessLogic;
        }

        [HttpPost]
        public async Task<IActionResult> SaveFirmType([FromBody] FirmType firmtype)
        {
            try
            {
                if (string.IsNullOrEmpty(firmtype.FirmTypeName) || string.IsNullOrEmpty(firmtype.Status))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _firmtypeBusinessLogic.SaveFirmType(firmtype);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadFirmType([FromBody] FirmType firmtype)
        {
            try
            {
                var result = await _firmtypeBusinessLogic.ReadFirmType(firmtype);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


    }
}



