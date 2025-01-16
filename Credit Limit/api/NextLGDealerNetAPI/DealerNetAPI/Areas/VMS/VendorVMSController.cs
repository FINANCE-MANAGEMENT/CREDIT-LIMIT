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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DealerNetAPI.Areas.VMS
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VendorVMSController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IVendorVMSBusinessLogic _vendorVMSBusinessLogic = null;
        private readonly ILookupBusinessLogic _lookupBusinessLogic = null;
        private readonly IUtilitiesVMSBusinessLogic _utilitiesVMSBusinessLogic = null;
        private readonly IWebHostEnvironment _environment;

        public VendorVMSController(JwtSettings jwtSettings, IConfiguration configuration, IVendorVMSBusinessLogic vendorVMSBusinessLogic,
            IWebHostEnvironment environment, ILookupBusinessLogic lookupBusinessLogic, IUtilitiesVMSBusinessLogic utilitiesVMSBusinessLogic)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _vendorVMSBusinessLogic = vendorVMSBusinessLogic;
            _lookupBusinessLogic = lookupBusinessLogic;
            _utilitiesVMSBusinessLogic = utilitiesVMSBusinessLogic;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> SaveVendor([FromBody] Vendor vendor)
        {
            string errorMsg = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(vendor.VendorCode) || string.IsNullOrEmpty(vendor.VendorName) || string.IsNullOrEmpty(vendor.EmailId))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                else
                {
                    if (!string.IsNullOrEmpty(vendor.PAN_No))
                    {
                        string PAN_REGEX = "[A-Z]{5}[0-9]{4}[A-Z]{1}";
                        if (!Regex.IsMatch(vendor.PAN_No.ToUpper(), PAN_REGEX))
                        {
                            errorMsg = "Invalid PAN No, Enter only captial value.";
                        }
                    }
                    if (!string.IsNullOrEmpty(vendor.GSTIN_No))
                    {
                        string GSTIN_REGEX = "[0-9]{2}[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9A-Za-z]{1}[Z]{1}[0-9a-zA-Z]{1}";

                        if (!Regex.IsMatch(vendor.GSTIN_No.ToUpper(), GSTIN_REGEX))
                        {
                            errorMsg += "Invalid GSTIN. ";
                        }
                        if (!vendor.GSTIN_No.ToUpper().Contains(vendor.PAN_No.ToUpper()))
                        {
                            errorMsg += "Please enter correct GSTIN. ";
                        }
                    }
                    if (!string.IsNullOrEmpty(vendor.PinCode))
                    {
                        string PinCode_REGEX = "[1-9][0-9]{5}";
                        if (!Regex.IsMatch(vendor.PinCode, PinCode_REGEX))
                        {
                            errorMsg += "Invalid PinCode. ";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = errorMsg }));
                }

                APIResponse result = await _vendorVMSBusinessLogic.SaveUpdateVendor(vendor);

                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveBulkVendor()
        {

            try
            {
                //var file = Request.Form.Files[0]; // working fine
                //var file = Request.Form["File"];
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
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
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
                if (!dtVendors.Columns[2].ColumnName.Equals("Mobile No"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[3].ColumnName.Equals("Email Id"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[4].ColumnName.Equals("CTRL_AU"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[5].ColumnName.Equals("PIC"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[6].ColumnName.Equals("PAN"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[7].ColumnName.Equals("GSTIN"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[8].ColumnName.Equals("MSME Reg No."))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[9].ColumnName.Equals("Type of Enterprises"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[10].ColumnName.Equals("Major Activity"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[11].ColumnName.Equals("State"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[12].ColumnName.Equals("Address"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[13].ColumnName.Equals("City"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[14].ColumnName.Equals("PinCode"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendors.Columns[15].ColumnName.Equals("Status(Y/N)"))
                {
                    excelHeaderCheck++;
                }

                if (excelHeaderCheck > 0)
                {
                    errors.Add(new Errors { Error = Utilities.INVALID_EXCEL_FILE });
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE, data = errors }));
                }

                // All State Read
                List<Lookup> lstState = await _lookupBusinessLogic.ReadLookup(new Lookup
                {
                    SystemName = "VENDOR_MANAGEMENT_SYSTEM",
                    LookupName = "VMS_STATE_MASTER"
                });

                // Read data from datatable
                for (int i = 0; i < dtVendors.Rows.Count; i++)
                {
                    Lookup stateDetail = null;
                    string _errorMsg = string.Empty;
                    string vendorCode = Convert.ToString(dtVendors.Rows[i]["Vendor Code"]).Trim();
                    string vendorName = Convert.ToString(dtVendors.Rows[i]["Vendor Name"]).Trim();
                    string mobileNo = Convert.ToString(dtVendors.Rows[i]["Mobile No"]).Trim();
                    string emailId = Convert.ToString(dtVendors.Rows[i]["Email Id"]).Trim();
                    string branchcode = Convert.ToString(dtVendors.Rows[i]["CTRL_AU"]).Trim();
                    string pic = Convert.ToString(dtVendors.Rows[i]["PIC"]).Trim();

                    string PAN_No = Convert.ToString(dtVendors.Rows[i]["PAN"]).Trim().ToUpper();
                    string GSTIN_No = Convert.ToString(dtVendors.Rows[i]["GSTIN"]).Trim().ToUpper();
                    string MSMERegNo = Convert.ToString(dtVendors.Rows[i]["MSME Reg No."]).Trim().ToUpper();
                    string enterprises_Type = Convert.ToString(dtVendors.Rows[i]["Type of Enterprises"]).Trim();
                    string majorActivity = Convert.ToString(dtVendors.Rows[i]["Major Activity"]).Trim();
                    string state = Convert.ToString(dtVendors.Rows[i]["State"]).Trim();
                    string address = Convert.ToString(dtVendors.Rows[i]["Address"]).Trim();
                    string city = Convert.ToString(dtVendors.Rows[i]["City"]).Trim();
                    string pincode = Convert.ToString(dtVendors.Rows[i]["PinCode"]).Trim();

                    string status = Convert.ToString(dtVendors.Rows[i]["Status(Y/N)"]).Trim().ToUpper();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(vendorCode))
                    {
                        _errorMsg += "Vendor Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(vendorName))
                    {
                        _errorMsg += "Vendor Name can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(mobileNo))
                    {
                        _errorMsg += "Mobile No. can't be blank, ";
                    }
                    else
                    {
                        if (!Utilities.isValidMobileNo(mobileNo))
                        {
                            _errorMsg += "Mobile No. is Invalid, ";
                        }
                    }
                    if (string.IsNullOrEmpty(emailId))
                    {
                        _errorMsg += "EmailId can't be blank, ";
                    }
                    else
                    {
                        if (!Utilities.ValidateMailAddress(emailId))
                        {
                            _errorMsg += "EmailId is Invalid, ";
                        }
                    }
                    if (string.IsNullOrEmpty(branchcode))
                    {
                        _errorMsg += "CTRL_AU can't be blank, ";
                    }

                    if (!string.IsNullOrEmpty(PAN_No))
                    {
                        string PAN_REGEX = "[A-Z]{5}[0-9]{4}[A-Z]{1}";
                        if (!Regex.IsMatch(PAN_No, PAN_REGEX))
                        {
                            _errorMsg += "Invalid PAN No, Enter only captial value.";
                        }
                    }
                    if (!string.IsNullOrEmpty(GSTIN_No))
                    {
                        string GSTIN_REGEX = "[0-9]{2}[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9A-Za-z]{1}[Z]{1}[0-9a-zA-Z]{1}";

                        if (!Regex.IsMatch(GSTIN_No, GSTIN_REGEX))
                        {
                            _errorMsg += "Invalid GSTIN.";
                        }
                        if (!GSTIN_No.Contains(PAN_No))
                        {
                            _errorMsg += "Please enter correct GSTIN.";
                        }
                    }
                    if (!string.IsNullOrEmpty(pincode))
                    {
                        string PinCode_REGEX = "[1-9][0-9]{5}";
                        if (!Regex.IsMatch(pincode, PinCode_REGEX))
                        {
                            _errorMsg += "Invalid PinCode.";
                        }
                    }

                    if (string.IsNullOrEmpty(status))
                    {
                        _errorMsg += "Status can't be blank, ";
                    }
                    else
                    {
                        if (status == "Y" || status == "N")
                        {

                        }
                        else
                        {
                            _errorMsg += "Status is Invalid (Y/N), ";
                        }
                    }

                    if (!string.IsNullOrEmpty(state))
                    {
                        stateDetail = lstState.Where(m => m.LookupValue == state).FirstOrDefault();
                        if (stateDetail == null)
                        {
                            _errorMsg += "State is Invalid, ";
                        }
                    }

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
                        vendor.MobileNo = mobileNo;
                        vendor.EmailId = emailId;
                        vendor.CTRL_AU = branchcode;
                        vendor.PIC_Code = pic;
                        vendor.PAN_No = PAN_No;
                        vendor.GSTIN_No = GSTIN_No;
                        vendor.MSMERegNo = MSMERegNo;
                        vendor.EnterprisesType = enterprises_Type;
                        vendor.MajorActivity = majorActivity;
                        if (!string.IsNullOrEmpty(state) && stateDetail != null)
                        {
                            vendor.State.StateId = Convert.ToInt32(stateDetail.Id);
                        }
                        vendor.Address = address;
                        vendor.City = city;
                        vendor.PinCode = pincode;
                        vendor.Status = status;
                        vendor.CreatedBy = createdBy;
                        lstVendors.Add(vendor);
                    }
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _vendorVMSBusinessLogic.SaveBulkVendor(lstVendors);
                if (result.Status == Utilities.SUCCESS)
                {
                    return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest), result));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReadVendor([FromBody] Vendor vendor)
        {
            try
            {
                var data = await _vendorVMSBusinessLogic.ReadVendor(vendor);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Vendor Profile Update Request raise
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorProfileUpdateRequest([FromBody] Vendor vendor)
        {
            string errorMsg = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(vendor.VendorCode) || string.IsNullOrEmpty(vendor.VendorName) || string.IsNullOrEmpty(vendor.EmailId))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                else
                {
                    if (!string.IsNullOrEmpty(vendor.PAN_No.ToUpper()))
                    {
                        string PAN_REGEX = "[A-Z]{5}[0-9]{4}[A-Z]{1}";
                        if (!Regex.IsMatch(vendor.PAN_No.ToUpper(), PAN_REGEX))
                        {
                            errorMsg = "Invalid PAN No, Enter only captial value.";
                        }
                    }
                    if (!string.IsNullOrEmpty(vendor.GSTIN_No.ToUpper()))
                    {
                        string GSTIN_REGEX = "[0-9]{2}[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9A-Za-z]{1}[Z]{1}[0-9a-zA-Z]{1}";

                        if (!Regex.IsMatch(vendor.GSTIN_No.ToUpper(), GSTIN_REGEX))
                        {
                            errorMsg += "Invalid GSTIN. ";
                        }
                        if (!vendor.GSTIN_No.ToUpper().Contains(vendor.PAN_No.ToUpper()))
                        {
                            errorMsg += "Please enter correct GSTIN. ";
                        }
                    }
                    if (!string.IsNullOrEmpty(vendor.PinCode))
                    {
                        string PinCode_REGEX = "[1-9][0-9]{5}";
                        if (!Regex.IsMatch(vendor.PinCode, PinCode_REGEX))
                        {
                            errorMsg += "Invalid PinCode. ";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = errorMsg }));
                }


                APIResponse result = await _vendorVMSBusinessLogic.VendorProfileUpdateRequest(vendor);

                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        /// <summary>
        /// Profile modification request supporting documents
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult VendorProfileDocProof()
        {
            try
            {
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());

                //Create the Directory.
                string path = Path.Combine(this._environment.WebRootPath, _configuration["VMS_UploadPath"], "ProfileDoc");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Fetch the File.
                IFormFile postedFile = Request.Form.Files[0];

                //Unique file name
                //Guid uniqueGuid = Guid.NewGuid();

                //Fetch the File Name.
                string fileName = Request.Form["fileName"]; // + "_" + createdBy + uniqueGuid.ToString() + Path.GetExtension(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);

                //Save the File.
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), new APIResponse
                {
                    Status = Utilities.SUCCESS,
                    StatusDesc = Utilities.FILE_UPLOADED_SUCCESSFULLY,
                    data = JsonConvert.SerializeObject(new
                    {
                        FilePath = string.Concat(_configuration["VMS_UploadPath"], "ProfileDoc", "\\", fileName)
                    })
                }));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        /// <summary>
        /// Download Uploads File
        /// </summary>
        /// <returns></returns>
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> FileDownload([FromBody] Document document)
        {
            try
            {
                var filePath = Path.Combine(this._environment.WebRootPath, document.DocumentName);
                if (!System.IO.File.Exists(filePath)) return NotFound();
                var memory = new MemoryStream();
                await using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(filePath));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                throw;
            }
        }

        private string GetContentType(string path)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }


        /// <summary>
        /// Read All Request raised by vendor.
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadVendorProfileUpdateRequests([FromBody] Vendor vendor)
        {
            try
            {
                var data = await _vendorVMSBusinessLogic.ReadVendorProfileUpdateRequests(vendor);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Vendor Profile Request Approval by Admin
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorProfileUpdateRequestApproval([FromBody] Vendor vendor)
        {
            string errorMsg = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(vendor.Id.ToString()) || string.IsNullOrEmpty(vendor.VendorCode))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                APIResponse result = await _vendorVMSBusinessLogic.VendorProfileUpdateRequestApproval(vendor);
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
        /// Read all Branches for PIC required.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReadPICRequiredBranch()
        {
            try
            {
                var data = await _vendorVMSBusinessLogic.ReadPICRequiredBranch();
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Read PIC Member for VMS system.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadPIC_Members([FromBody] Branch branch)
        {
            try
            {
                var data = await _vendorVMSBusinessLogic.ReadPIC_Members(branch);
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
