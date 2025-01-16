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
    public class BankMasterCFController : ControllerBase
    {
        //private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IBankBusinessLogic _bankBusinessLogic = null;
        public BankMasterCFController(IConfiguration configuration, IBankBusinessLogic bankBusinessLogic)
        {
            _configuration = configuration;
            _bankBusinessLogic = bankBusinessLogic;
        }

        [HttpPost]
        public async Task<IActionResult> SaveBank([FromBody] Bank bank)
        {
            try
            {
                if (string.IsNullOrEmpty(bank.BankCode) || string.IsNullOrEmpty(bank.BankName) || string.IsNullOrEmpty(bank.Status))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _bankBusinessLogic.SaveBank(bank);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadBank([FromBody] Bank bank)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized(new { message = "Token Expired." });

                var data = await _bankBusinessLogic.ReadBank(bank);
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
