using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.Areas.VMS
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UtilitiesVMSController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IUtilitiesVMSBusinessLogic _utilitiesVMSBusinessLogic = null;
        private readonly IWebHostEnvironment _environment;

        public UtilitiesVMSController(JwtSettings jwtSettings, IConfiguration configuration, IUtilitiesVMSBusinessLogic utilitiesVMSBusinessLogic,
            IWebHostEnvironment environment)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _utilitiesVMSBusinessLogic = utilitiesVMSBusinessLogic;
            _environment = environment;
        }

        /// <summary>
        /// Read Confirmation Periods for VMS.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReadConfirmationPeriod([FromQuery] string ClosingStatus, string VendorCode)
        {
            try
            {
                var data = await _utilitiesVMSBusinessLogic.ReadConfirmationPeriod(ClosingStatus, VendorCode);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
                

        /// <summary>
        /// Branch Master Read for VMS system.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReadBranch()
        {
            try
            {
                var data = await _utilitiesVMSBusinessLogic.ReadBranch();
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Generate OTP
        /// </summary>
        /// <param name="OTP"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GenerateOTP([FromBody] OTP otp)
        {
            try
            {
                if (string.IsNullOrEmpty(otp.CreatedBy.ToString()))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Invalid Vendor." }));
                }
                else if (string.IsNullOrEmpty(otp.VendorCode))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Invalid Vendor." }));
                }

                APIResponse result = await _utilitiesVMSBusinessLogic.GenerateOTP(otp);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Validate OTP
        /// </summary>
        /// <param name="OTP"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ValidateOTP([FromBody] OTP otp)
        {
            try
            {
                if (string.IsNullOrEmpty(otp.CreatedBy.ToString()) || string.IsNullOrEmpty(otp.OTPNo))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Vendor or OTP No. can't be blank." }));
                }

                APIResponse result = await _utilitiesVMSBusinessLogic.ValidateOTP(otp);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// Update OTP Status after applied
        /// </summary>
        /// <param name="OTP"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateOTP([FromBody] OTP otp)
        {
            try
            {
                if (string.IsNullOrEmpty(otp.CreatedBy.ToString()) || string.IsNullOrEmpty(otp.OTPNo))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Vendor or OTP No. can't be blank." }));
                }

                APIResponse result = await _utilitiesVMSBusinessLogic.UpdateOTP(otp);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// User AU Mappings
        /// </summary>
        /// <param name="OTP"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UserAU_Mapping([FromBody] Users users)
        {
            try
            {
                if (users.UserId == 0 && string.IsNullOrEmpty(users.Zone.Region.Branch.BranchCode))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                APIResponse result = await _utilitiesVMSBusinessLogic.UserAU_Mapping(users);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// Read user AU Mapping.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadUserAU_Mapping([FromBody] Users users)
        {
            try
            {
                var data = await _utilitiesVMSBusinessLogic.ReadUserAU_Mapping(users);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadUsersForAUMappingByRoleId(int roleId)
        {
            try
            {
                // Check mandatory fields.
                if (roleId == 0)
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));

                var data = await _utilitiesVMSBusinessLogic.ReadUsersForAUMappingByRoleId(roleId);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// Change Password of Vendors/ Supplier
        /// </summary>
        /// <param name="_users"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] Users userPswd)
        {
            try
            {
                if (userPswd == null)
                    return BadRequest();
                // Check mandatory fields.
                if (userPswd.UserId == 0 || string.IsNullOrEmpty(userPswd.UserId.ToString()) ||
                    string.IsNullOrEmpty(userPswd.Password) || string.IsNullOrEmpty(userPswd.NewPassword))
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Please check details." }));

                APIResponse result = await _utilitiesVMSBusinessLogic.ChangePassword(userPswd);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Send SMS
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SendSMS()
        {
            string BASE_URL = "https://qyvewr.api-in.infobip.com";
            string API_KEY = "26a5e164282c39649870cc8878a44fd8-a5cafc9f-615f-4c75-a312-c316e199e7f1";

            string customerMobileNo = "918882603537";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BASE_URL);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("App", API_KEY);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Guid uniqueId = Guid.NewGuid();
            InfobipSMS infobipSMS = new InfobipSMS();
            Messages messages = new Messages();
            messages.From = "LGEIVM";
            Destinations destinations = new Destinations();
            destinations.To = customerMobileNo;
            destinations.MessageId = uniqueId.ToString();
            messages.Destination.Add(destinations);
            IndiaDlt indiaDlt = new IndiaDlt();
            indiaDlt.ContentTemplateId = "1107170486118810066"; // Template ID which is registered and assign unique SMS ID.
            indiaDlt.PrincipalEntityId = "1101588720000028697"; // This is common ID for a Orgination of any SMS send, Only Template ID need to change.
            messages.Regional.indiaDlt = indiaDlt;
            messages.Text = "Dear Partner, Thank you for association with LG. {#var#}{#var#} Please login on VMS Portal www.lgvms.com with your credential for further details.";
            infobipSMS.Messages.Add(messages);
            string smsbody = Newtonsoft.Json.JsonConvert.SerializeObject(infobipSMS);

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/sms/2/text/advanced");
            httpRequest.Content = new StringContent(smsbody, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            return Ok(responseContent);
        }


    }
}
