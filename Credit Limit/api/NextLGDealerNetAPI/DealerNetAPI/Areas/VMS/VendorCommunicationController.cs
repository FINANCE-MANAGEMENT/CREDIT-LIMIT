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
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DealerNetAPI.Areas.VMS
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VendorCommunicationController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IVendorCommunicationBusinessLogic _vendorCommunicationBusinessLogic = null;
        private readonly IWebHostEnvironment _environment;

        public VendorCommunicationController(JwtSettings jwtSettings, IConfiguration configuration, IVendorCommunicationBusinessLogic vendorCommunicationBusinessLogic,
                                            IWebHostEnvironment environment)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _vendorCommunicationBusinessLogic = vendorCommunicationBusinessLogic;
            _environment = environment;
        }


        /// <summary>
        /// Vendor/ Supplier Communication Template Registration
        /// </summary>
        /// <param name="vendorCommunication"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorCommunicationTemplateRegistration([FromBody] VendorCommunication vendorCommunication)
        {
            try
            {
                if (string.IsNullOrEmpty(vendorCommunication.TemplateName) ||
                    string.IsNullOrEmpty(vendorCommunication.TemplateContent) || vendorCommunication == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                APIResponse result = await _vendorCommunicationBusinessLogic.VendorCommunicationTemplateRegistration(vendorCommunication);
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
        /// Read Communication Template
        /// </summary>
        /// <param name="vendorCommunication"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadVendorCommunicationTemplate([FromBody] VendorCommunication vendorCommunication)
        {
            try
            {
                var data = await _vendorCommunicationBusinessLogic.ReadVendorCommunicationTemplate(vendorCommunication);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// Read Communication Template Required Info
        /// </summary>
        /// <param name="vendorCommunication"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadCommunicationTemplateRequiredInfo([FromBody] VendorCommunication vendorCommunication)
        {
            try
            {
                var data = await _vendorCommunicationBusinessLogic.ReadCommunicationTemplateRequiredInfo(vendorCommunication);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// Communication Send to Vendor
        /// </summary>
        /// <param name="vendorCommunication"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CommunicationSendToVendor([FromBody] VendorCommunication vendorCommunication)
        {
            try
            {
                if (string.IsNullOrEmpty(vendorCommunication.TemplateId.ToString()) || vendorCommunication.Vendors == null ||
                    vendorCommunication.Vendors.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                APIResponse result = await _vendorCommunicationBusinessLogic.CommunicationSendToVendor(vendorCommunication);
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
        /// Vendor/ Supplier Upload Temprory for comminucation.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorUploadForCommunication()
        {
            try
            {
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());

                //Create the Directory.
                string path = Path.Combine(this._environment.WebRootPath, _configuration["VMS_UploadPath"]);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Fetch the File.
                IFormFile postedFile = Request.Form.Files[0];

                //Unique file name
                Guid uniqueGuid = Guid.NewGuid();

                //Fetch the File Name.
                string fileName = Request.Form["fileName"] + createdBy + uniqueGuid.ToString() + Path.GetExtension(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);

                //Save the File.
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Read data from Excel File
                DataTable dtVendors = Utilities.MapExcelToDictionary(filePath);
                if (dtVendors.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<Vendor> lstVendors = new List<Vendor>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name
                if (!dtVendors.Columns[0].ColumnName.Equals("Vendor Code"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[1].ColumnName.Equals("Vendor Name"))
                {
                    excelHeaderCheck++;
                }
                //if (!dtVendors.Columns[2].ColumnName.Equals("CTRL_AU"))
                //{
                //    excelHeaderCheck++;
                //}

                if (excelHeaderCheck > 0)
                {
                    errors.Add(new Errors { Error = Utilities.INVALID_EXCEL_FILE });
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE, data = errors }));
                }

                // Read data from datatable
                for (int i = 0; i < dtVendors.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string vendorCode = Convert.ToString(dtVendors.Rows[i]["Vendor Code"]).Trim();
                    string vendorName = Convert.ToString(dtVendors.Rows[i]["Vendor Name"]).Trim();
                    //string ctrl_au = Convert.ToString(dtVendors.Rows[i]["CTRL_AU"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(vendorCode))
                    {
                        _errorMsg += "Vendor Code can't be blank, ";
                    }
                    //if (string.IsNullOrEmpty(ctrl_au))
                    //{
                    //    _errorMsg += "CTRL_AU can't be blank, ";
                    //}

                    #endregion

                    if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    else
                    {
                        Vendor vendor = new Vendor();
                        vendor.VendorCode = vendorCode;
                        vendor.VendorName = vendorName;
                        //vendor.CTRL_AU = ctrl_au;
                        vendor.CreatedBy = createdBy;
                        vendor.RowNumber = (i + 2);
                        lstVendors.Add(vendor);
                    }
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _vendorCommunicationBusinessLogic.VendorUploadForCommunication(lstVendors);

                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }

        }

        /// <summary>
        /// Read All Templates, which is send to Vendor
        /// </summary>
        /// <param name="vendorCommunication"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadVendorCommunicationSendTemplate([FromBody] VendorCommunication vendorCommunication)
        {
            try
            {
                var data = await _vendorCommunicationBusinessLogic.ReadVendorCommunicationSendTemplate(vendorCommunication);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Vendor Communication Acceptance
        /// </summary>
        /// <param name="vendorCommunication"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorCommunicationAcceptance([FromBody] VendorCommunication vendorCommunication)
        {
            try
            {
                if (string.IsNullOrEmpty(vendorCommunication.TemplateId.ToString()) ||
                    string.IsNullOrEmpty(vendorCommunication.Vendors[0].VendorCode) ||
                   vendorCommunication.Vendors == null || vendorCommunication == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                APIResponse result = await _vendorCommunicationBusinessLogic.VendorCommunicationAcceptance(vendorCommunication);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


    }
}
