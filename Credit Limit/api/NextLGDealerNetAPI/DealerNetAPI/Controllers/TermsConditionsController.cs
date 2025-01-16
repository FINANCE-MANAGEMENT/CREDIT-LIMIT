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
    public class TermsConditionsController : ControllerBase
    {
        //private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly ITermsConditionsBusinessLogic _termsConditionsBusinessLogic = null;
        public TermsConditionsController(IConfiguration configuration, ITermsConditionsBusinessLogic termsConditionsBusinessLogic)
        {
            _configuration = configuration;
            _termsConditionsBusinessLogic = termsConditionsBusinessLogic;
        }

        [HttpGet]
        public async Task<IActionResult> ReadSchemeTypes(string SystemName, Int32 Id)
        {
            try
            {
                var data = await _termsConditionsBusinessLogic.ReadSchemeTypes(SystemName, Id);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTermsConditions([FromBody] TermsConditions termsConditions)
        {
            try
            {
                if (string.IsNullOrEmpty(termsConditions.TermCondition) || string.IsNullOrEmpty(termsConditions.SchemeType) || string.IsNullOrEmpty(termsConditions.SystemName))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _termsConditionsBusinessLogic.SaveTermsConditions(termsConditions);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReadTermsConditions([FromBody] TermsConditions termsConditions)
        {
            try
            {
                var result = await _termsConditionsBusinessLogic.ReadTermsConditions(termsConditions);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


    }
}
