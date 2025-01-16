using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.Models;
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

namespace DealerNetAPI.Areas.SchemeDFI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ModelMasterDFIController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IUserBusinessLogic _userBusinessLogic = null;
        private readonly IUtilitiesDFIBusinessLogic _utilitiesDFIBusinessLogic = null;

        public ModelMasterDFIController(JwtSettings jwtSettings, IConfiguration configuration, IUserBusinessLogic userBusinessLogic, 
            IUtilitiesDFIBusinessLogic utilitiesDFIBusinessLogic)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _userBusinessLogic = userBusinessLogic;
            _utilitiesDFIBusinessLogic = utilitiesDFIBusinessLogic;
        }

        //[HttpGet]
        //public async Task<IActionResult> ReadRoles(string SystemName)
        //{
        //    try
        //    {
        //        // Check mandatory fields.
        //        if (string.IsNullOrEmpty(SystemName))
        //            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
        //                new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));

        //        var data = await _userBusinessLogic.ReadRoles(SystemName);
        //        return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
        //            new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
        //    }
        //}

        

    }
}
