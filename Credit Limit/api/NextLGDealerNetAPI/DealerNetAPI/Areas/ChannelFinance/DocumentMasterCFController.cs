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
    public class DocumentMasterCFController : ControllerBase
    {
        //private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IDocumentBusinessLogic _documentBusinessLogic = null;
        public DocumentMasterCFController(IConfiguration configuration, IDocumentBusinessLogic documentBusinessLogic)
        {
            _configuration = configuration;
            _documentBusinessLogic = documentBusinessLogic;
        }

        [HttpPost]
        public async Task<IActionResult> SaveDocument([FromBody] Document document)
        {
            try
            {
                if (string.IsNullOrEmpty(document.DocumentName) || string.IsNullOrEmpty(document.Status))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _documentBusinessLogic.SaveDocument(document);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadDocument([FromBody] Document document)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized(new { message = "Token Expired." });

                return Ok(_documentBusinessLogic.ReadDocument(document));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

    }
}





