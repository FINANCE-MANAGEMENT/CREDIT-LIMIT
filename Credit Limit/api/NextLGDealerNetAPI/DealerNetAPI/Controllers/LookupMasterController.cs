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

namespace DealerNetAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LookupMasterController : ControllerBase
    {
        //private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly ILookupBusinessLogic _lookupBusinessLogic = null;
        public LookupMasterController(IConfiguration configuration, ILookupBusinessLogic lookupBusinessLogic)
        {
            _configuration = configuration;
            _lookupBusinessLogic = lookupBusinessLogic;
        }

        [HttpPost]
        public async Task<IActionResult> SaveLookup([FromBody] Lookup lookup)
        {
            try
            {
                if (lookup.LookupId == 0 || string.IsNullOrEmpty(lookup.LookupValue) || string.IsNullOrEmpty(lookup.Status))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _lookupBusinessLogic.SaveLookup(lookup);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReadLookup([FromBody] Lookup lookup)
        {
            try
            {
                var data = await _lookupBusinessLogic.ReadLookup(lookup);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadLookupTypes(string SystemName)
        {
            try
            {
                var data = await _lookupBusinessLogic.ReadLookupTypes(SystemName);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }



    }


}
















