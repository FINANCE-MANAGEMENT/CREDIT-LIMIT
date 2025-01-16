using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.DomainObject.ARCreditLimit;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DealerNetAPI.Areas.ARCreditLimit
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UtilitiesARCreditLimitController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IWebHostEnvironment _environment;
        private readonly IUtilitiesARCreditLimitBusinessLogic _utilitiesARCreditLimitBusinessLogic = null;

        public UtilitiesARCreditLimitController(JwtSettings jwtSettings, IConfiguration configuration,
            IWebHostEnvironment environment, IUtilitiesARCreditLimitBusinessLogic utilitiesARCreditLimitBusinessLogic)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _environment = environment;
            _utilitiesARCreditLimitBusinessLogic = utilitiesARCreditLimitBusinessLogic;
        }

        /// <summary>
        /// Branch Master Read
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReadBranch()
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.ReadBranch();
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Note Master Save/Update
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> NoteSave([FromBody] Notes notes)
        {
            try
            {
                if (string.IsNullOrEmpty(notes.Note) || string.IsNullOrEmpty(notes.Status))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                APIResponse result = await _utilitiesARCreditLimitBusinessLogic.NoteSave(notes);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        /// <summary>
        /// Note Master Read
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadNotes([FromBody] Notes notes)
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.ReadNotes(notes);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// All Masters Upload Last Read
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> MastersUpdatedRead()
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.MastersUpdatedRead();
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Upload Logs File Read (ProcessType = SALES_UPLOAD , OD_UPLOAD)
        /// </summary>
        /// <param name="ProcessType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UploadFilesLogRead([FromQuery] string ProcessType)
        {
            try
            {
                if (string.IsNullOrEmpty(ProcessType))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                var data = await _utilitiesARCreditLimitBusinessLogic.UploadFilesLogRead(ProcessType);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        /// <summary>
        /// Download Uploads File
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> FileDownload([FromBody] ARCreditLimitSalesDOM sale)
        {
            try
            {
                var filePath = Path.Combine(this._environment.WebRootPath, sale.FileNamePath); // Path.Combine(Directory.GetCurrentDirectory(), fileUrl);
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
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
        }


        /// <summary>
        /// Sales data upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SalesUpload()
        {
            try
            {
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string fileFolder = string.Concat(_configuration["ARCreditLimit_UploadPath"], "Sales\\");

                //Create the Directory.
                string path = Path.Combine(this._environment.WebRootPath, fileFolder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Fetch the File.
                IFormFile postedFile = Request.Form.Files[0];

                //Unique file name
                Guid uniqueGuid = Guid.NewGuid();

                //Fetch the File Name.
                string fileName = string.Concat(Request.Form["fileName"], "_", createdBy, "_", uniqueGuid.ToString() + Path.GetExtension(postedFile.FileName));
                string filePath = Path.Combine(path, fileName);

                //Save the File.
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Read data from Excel File
                DataTable dtSales = Utilities.MapExcelToDictionary(filePath);
                if (dtSales.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<ARCreditLimitSalesDOM> lstSales = new List<ARCreditLimitSalesDOM>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name
                string[] columnNames = dtSales.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();


                if (!columnNames[0].Equals("Header Code"))
                {
                    excelHeaderCheck++;
                }
                if (!columnNames[1].Equals("Account Name"))
                {
                    excelHeaderCheck++;
                }

                for (int i = 2; i < columnNames.Length; i++)
                {
                    if (columnNames[i].Length != 8)
                    {
                        excelHeaderCheck++;
                    }
                }

                if (excelHeaderCheck > 0)
                {
                    errors.Add(new Errors { Error = Utilities.INVALID_EXCEL_FILE });
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE, data = errors }));
                }

                // Read data from datatable
                for (int i = 0; i < dtSales.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string headerCode = Convert.ToString(dtSales.Rows[i]["Header Code"]).Trim();
                    string accountName = Convert.ToString(dtSales.Rows[i]["Account Name"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(headerCode))
                    {
                        _errorMsg += "Header Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(accountName))
                    {
                        _errorMsg += "Account Name can't be blank, ";
                    }

                    if (string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        for (int d = 2; d < columnNames.Length; d++)
                        {
                            _errorMsg = string.Empty;
                            string monthYear = Convert.ToString(columnNames[d]).Trim();
                            string saleAmount = Convert.ToString(dtSales.Rows[i][d]).Trim();

                            if (string.IsNullOrEmpty(monthYear))
                            {
                                _errorMsg += "Month Year can't be blank, ";
                            }
                            else
                            {
                                if (!isValidMonth(monthYear))
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                                else if (!Utilities.isValidNumber(monthYear.Substring(4)))
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                                else if (monthYear.Substring(4).Length != 4)
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                                else if (!monthYear.Contains("-"))
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                            }

                            if (string.IsNullOrEmpty(saleAmount))
                            {
                                _errorMsg += "Sale Amount can't be blank, ";
                            }
                            else
                            {
                                if (!Utilities.isValidDecimal(saleAmount))
                                {
                                    _errorMsg += "Sale Amount is Invalid, ";
                                }
                            }

                            if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                            {
                                errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                            }
                            else
                            {
                                ARCreditLimitSalesDOM sale = new ARCreditLimitSalesDOM();
                                sale.HeaderCode = headerCode;
                                sale.AccountName = accountName;
                                sale.MonthYear = monthYear.ToUpper();
                                sale.SaleAmount = Convert.ToDecimal(saleAmount);
                                sale.CreatedBy = createdBy;
                                sale.RowNumber = (i + 2);
                                sale.FileNamePath = string.Concat(fileFolder, fileName);
                                lstSales.Add(sale);
                            }
                        }
                    }
                    else
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    #endregion
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _utilitiesARCreditLimitBusinessLogic.SalesUpload(lstSales);
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
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }
        }
        /// <summary>
        /// Sales data Upload read
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadSalesData([FromBody] ARCreditLimitSalesDOM sale)
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.ReadSalesData(sale);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// Insurance data upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> InsuranceUpload()
        {
            try
            {
                //var file = Request.Form.Files[0]; // working fine
                //var file = Request.Form["File"];
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());

                //Create the Directory.
                string path = Path.Combine(this._environment.WebRootPath, _configuration["ARCreditLimit_UploadPath"]);
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
                DataTable dtInsurace = Utilities.MapExcelToDictionary(filePath);
                if (dtInsurace.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<ARCreditLimitInsurance> lstInsurance = new List<ARCreditLimitInsurance>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;
                // Check Excel Columns Name
                if (!dtInsurace.Columns[0].ColumnName.Equals("Header Code"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInsurace.Columns[1].ColumnName.Equals("Account Name"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInsurace.Columns[2].ColumnName.Equals("Insurance"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInsurace.Columns[3].ColumnName.Equals("Insurance Code"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInsurace.Columns[4].ColumnName.Equals("Current Credit Limit"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInsurace.Columns[5].ColumnName.Equals("Limit End Date (mm/dd/yyyy)"))
                {
                    excelHeaderCheck++;
                }

                if (excelHeaderCheck > 0)
                {
                    errors.Add(new Errors { Error = Utilities.INVALID_EXCEL_FILE });
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE, data = errors }));
                }

                // Read data from datatable
                for (int i = 0; i < dtInsurace.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string headerCode = Convert.ToString(dtInsurace.Rows[i]["Header Code"]).Trim();
                    string accountName = Convert.ToString(dtInsurace.Rows[i]["Account Name"]).Trim();
                    string insuranceCode = Convert.ToString(dtInsurace.Rows[i]["Insurance Code"]).Trim();
                    string insurance = Convert.ToString(dtInsurace.Rows[i]["Insurance"]).Trim();
                    string currentCreditLimitAmount = Convert.ToString(dtInsurace.Rows[i]["Current Credit Limit"]).Trim();
                    string currentCreditLimitEndDate = Convert.ToString(dtInsurace.Rows[i]["Limit End Date (mm/dd/yyyy)"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(headerCode))
                    {
                        _errorMsg += "Header Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(accountName))
                    {
                        _errorMsg += "Account Name can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(insuranceCode))
                    {
                        _errorMsg += "Insurance Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(insurance))
                    {
                        _errorMsg += "Insurance can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(currentCreditLimitAmount))
                    {
                        _errorMsg += "Current Credit Limit Amount can't be blank, ";
                    }
                    else
                    {
                        if (!Utilities.isValidDecimal(currentCreditLimitAmount))
                        {
                            _errorMsg += "Current Credit Limit Amount is Invalid, ";
                        }
                    }
                    if (string.IsNullOrEmpty(currentCreditLimitEndDate))
                    {
                        _errorMsg += "Credit Limit End Date can't be blank, ";
                    }
                    else
                    {
                        try
                        {
                            currentCreditLimitEndDate = DateTime.FromOADate(Convert.ToDouble(currentCreditLimitEndDate)).ToString().Split(' ')[0];
                            if (!string.IsNullOrEmpty(currentCreditLimitEndDate.Trim()))
                            {
                                if (!Utilities.isValidDate(currentCreditLimitEndDate))
                                {
                                    _errorMsg += "Credit Limit End Date is Invalid, ";
                                }
                                else
                                {
                                    DateTime datevalue = Convert.ToDateTime(currentCreditLimitEndDate);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            _errorMsg += "Credit Limit End Date is Invalid, ";
                        }
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    else
                    {
                        ARCreditLimitInsurance _insurance = new ARCreditLimitInsurance();
                        _insurance.HeaderCode = headerCode;
                        _insurance.AccountName = accountName;
                        _insurance.InsuranceCode = insuranceCode;
                        _insurance.InsuranceName = insurance;
                        _insurance.CurrentCreditLimitAmount = Convert.ToDecimal(currentCreditLimitAmount);
                        if (!string.IsNullOrEmpty(currentCreditLimitEndDate))
                        {
                            _insurance.CurrentCreditLimitEndDate = Convert.ToDateTime(currentCreditLimitEndDate);
                        }
                        _insurance.CreatedBy = createdBy;
                        _insurance.RowNumber = (i + 2);
                        _insurance.FileNamePath = string.Concat(_configuration["ARCreditLimit_UploadPath"], fileName);
                        lstInsurance.Add(_insurance);
                    }
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _utilitiesARCreditLimitBusinessLogic.InsuranceUpload(lstInsurance);
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
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }
        }
        /// <summary>
        /// Insurance data Upload read
        /// </summary>
        /// <param name="insurance"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadInsuranceData([FromBody] ARCreditLimitInsurance insurance)
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.ReadInsuranceData(insurance);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// OD data upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ODUpload()
        {
            try
            {
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string fileFolder = string.Concat(_configuration["ARCreditLimit_UploadPath"], "OD\\");

                //Create the Directory.
                string path = Path.Combine(this._environment.WebRootPath, fileFolder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Fetch the File.
                IFormFile postedFile = Request.Form.Files[0];

                //Unique file name
                Guid uniqueGuid = Guid.NewGuid();

                //Fetch the File Name.
                string fileName = string.Concat(Request.Form["fileName"], "_", createdBy, "_", uniqueGuid.ToString() + Path.GetExtension(postedFile.FileName));
                string filePath = Path.Combine(path, fileName);

                //Save the File.
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Read data from Excel File
                DataTable dtOD = Utilities.MapExcelToDictionary(filePath);
                if (dtOD.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<ARCreditLimitOD> lstOD = new List<ARCreditLimitOD>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name
                string[] columnNames = dtOD.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();


                if (!columnNames[0].Equals("Header Code"))
                {
                    excelHeaderCheck++;
                }
                if (!columnNames[1].Equals("Account Name"))
                {
                    excelHeaderCheck++;
                }

                for (int i = 2; i < columnNames.Length; i++)
                {
                    if (columnNames[i].Length != 8)
                    {
                        excelHeaderCheck++;
                    }
                }

                if (excelHeaderCheck > 0)
                {
                    errors.Add(new Errors { Error = Utilities.INVALID_EXCEL_FILE });
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE, data = errors }));
                }

                // Read data from datatable
                for (int i = 0; i < dtOD.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string headerCode = Convert.ToString(dtOD.Rows[i]["Header Code"]).Trim();
                    string accountName = Convert.ToString(dtOD.Rows[i]["Account Name"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(headerCode))
                    {
                        _errorMsg += "Header Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(accountName))
                    {
                        _errorMsg += "Account Name can't be blank, ";
                    }

                    if (string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        for (int d = 2; d < columnNames.Length; d++)
                        {
                            _errorMsg = string.Empty;
                            string monthYear = Convert.ToString(columnNames[d]).Trim();
                            string odAmount = Convert.ToString(dtOD.Rows[i][d]).Trim();

                            if (string.IsNullOrEmpty(monthYear))
                            {
                                _errorMsg += "Month Year can't be blank, ";
                            }
                            else
                            {
                                if (!isValidMonth(monthYear))
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                                else if (!Utilities.isValidNumber(monthYear.Substring(4)))
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                                else if (monthYear.Substring(4).Length != 4)
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                                else if (!monthYear.Contains("-"))
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                            }

                            if (string.IsNullOrEmpty(odAmount))
                            {
                                _errorMsg += "OD Amount can't be blank, ";
                            }
                            else
                            {
                                if (!Utilities.isValidDecimal(odAmount))
                                {
                                    _errorMsg += "OD Amount is Invalid, ";
                                }
                            }

                            if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                            {
                                errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                            }
                            else
                            {
                                ARCreditLimitOD _OD = new ARCreditLimitOD();
                                _OD.HeaderCode = headerCode;
                                _OD.AccountName = accountName;
                                _OD.MonthYear = monthYear.ToUpper();
                                _OD.ODAmount = Convert.ToDecimal(odAmount);
                                _OD.CreatedBy = createdBy;
                                _OD.RowNumber = (i + 2);
                                _OD.FileNamePath = string.Concat(fileFolder, fileName);
                                lstOD.Add(_OD);
                            }
                        }
                    }
                    else
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    #endregion
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _utilitiesARCreditLimitBusinessLogic.ODUpload(lstOD);
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
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }
        }

        /// <summary>
        /// OD data Upload read
        /// </summary>
        /// <param name="od"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadODData([FromBody] ARCreditLimitOD od)
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.ReadODData(od);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// Collection data upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CollectionUpload()
        {
            try
            {
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string fileFolder = string.Concat(_configuration["ARCreditLimit_UploadPath"], "Collection\\");

                //Create the Directory.
                string path = Path.Combine(this._environment.WebRootPath, fileFolder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Fetch the File.
                IFormFile postedFile = Request.Form.Files[0];

                //Unique file name
                Guid uniqueGuid = Guid.NewGuid();

                //Fetch the File Name.
                string fileName = string.Concat(Request.Form["fileName"], "_", createdBy, "_", uniqueGuid.ToString() + Path.GetExtension(postedFile.FileName));
                string filePath = Path.Combine(path, fileName);

                //Save the File.
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Read data from Excel File
                DataTable dtCollection = Utilities.MapExcelToDictionary(filePath);
                if (dtCollection.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<ARCreditLimitCollection> lstCollection = new List<ARCreditLimitCollection>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name
                string[] columnNames = dtCollection.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();


                if (!columnNames[0].Equals("Header Code"))
                {
                    excelHeaderCheck++;
                }
                if (!columnNames[1].Equals("Account Name"))
                {
                    excelHeaderCheck++;
                }

                for (int i = 2; i < columnNames.Length; i++)
                {
                    if (columnNames[i].Length != 8)
                    {
                        excelHeaderCheck++;
                    }
                }

                if (excelHeaderCheck > 0)
                {
                    errors.Add(new Errors { Error = Utilities.INVALID_EXCEL_FILE });
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE, data = errors }));
                }

                // Read data from datatable
                for (int i = 0; i < dtCollection.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string headerCode = Convert.ToString(dtCollection.Rows[i]["Header Code"]).Trim();
                    string accountName = Convert.ToString(dtCollection.Rows[i]["Account Name"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(headerCode))
                    {
                        _errorMsg += "Header Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(accountName))
                    {
                        _errorMsg += "Account Name can't be blank, ";
                    }

                    if (string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        for (int d = 2; d < columnNames.Length; d++)
                        {
                            _errorMsg = string.Empty;
                            string monthYear = Convert.ToString(columnNames[d]).Trim();
                            string collectionAmount = Convert.ToString(dtCollection.Rows[i][d]).Trim();

                            if (string.IsNullOrEmpty(monthYear))
                            {
                                _errorMsg += "Month Year can't be blank, ";
                            }
                            else
                            {
                                if (!isValidMonth(monthYear))
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                                else if (!Utilities.isValidNumber(monthYear.Substring(4)))
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                                else if (monthYear.Substring(4).Length != 4)
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                                else if (!monthYear.Contains("-"))
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }
                            }

                            if (string.IsNullOrEmpty(collectionAmount))
                            {
                                _errorMsg += "Collection Amount can't be blank, ";
                            }
                            else
                            {
                                if (!Utilities.isValidDecimal(collectionAmount))
                                {
                                    _errorMsg += "Collection Amount is Invalid, ";
                                }
                            }

                            if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                            {
                                errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                            }
                            else
                            {
                                ARCreditLimitCollection _collection = new ARCreditLimitCollection();
                                _collection.HeaderCode = headerCode;
                                _collection.AccountName = accountName;
                                _collection.MonthYear = monthYear.ToUpper();
                                _collection.CollectionAmount = Convert.ToDecimal(collectionAmount);
                                _collection.CreatedBy = createdBy;
                                _collection.RowNumber = (i + 2);
                                _collection.FileNamePath = string.Concat(fileFolder, fileName);
                                lstCollection.Add(_collection);
                            }
                        }
                    }
                    else
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    #endregion
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _utilitiesARCreditLimitBusinessLogic.CollectionUpload(lstCollection);
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
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }
        }

        /// <summary>
        /// Collection data Upload read
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadCollectionData([FromBody] ARCreditLimitCollection collection)
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.ReadCollectionData(collection);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Save FY Status Master
        /// </summary>
        /// <param name="fYStatus"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FYStatusSave([FromBody] ARCreditLimitFYStatus fYStatus)
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.FYStatusSave(fYStatus);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        /// <summary>
        /// Read FY Status Master
        /// </summary>
        /// <param name="fYStatus"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FYStatusRead([FromBody] ARCreditLimitFYStatus fYStatus)
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.FYStatusRead(fYStatus);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Financial Year Attachment Upload against Header Code
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FinancialYearAttachmentUpload()
        {
            try
            {
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string fileFolder = string.Concat(_configuration["ARCreditLimit_UploadPath"], "FYAttachment\\");

                //Create the Directory.
                string path = Path.Combine(this._environment.WebRootPath, fileFolder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Fetch the File.
                IFormFile postedFile = Request.Form.Files[0];

                //Unique file name
                Guid uniqueGuid = Guid.NewGuid();

                //Fetch the File Name.
                string fileName = string.Concat(Request.Form["fileName"], "_", createdBy, "_", uniqueGuid.ToString() + Path.GetExtension(postedFile.FileName));
                string filePath = Path.Combine(path, fileName);

                //Save the File.
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Read data from Excel File
                DataTable dtFYAttachment = Utilities.MapExcelToDictionary(filePath);
                if (dtFYAttachment.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<ARCreditLimitFYStatus> lstFYAttachment = new List<ARCreditLimitFYStatus>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name
                string[] columnNames = dtFYAttachment.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                if (!columnNames[0].Equals("Header Code"))
                {
                    excelHeaderCheck++;
                }
                if (!columnNames[1].Equals("Account Name"))
                {
                    excelHeaderCheck++;
                }

                for (int i = 2; i < columnNames.Length; i++)
                {
                    if (columnNames[i].Length != 9)
                    {
                        excelHeaderCheck++;
                    }
                }

                if (excelHeaderCheck > 0)
                {
                    errors.Add(new Errors { Error = Utilities.INVALID_EXCEL_FILE });
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE, data = errors }));
                }

                // Read data from datatable
                for (int i = 0; i < dtFYAttachment.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string headerCode = Convert.ToString(dtFYAttachment.Rows[i]["Header Code"]).Trim();
                    string accountName = Convert.ToString(dtFYAttachment.Rows[i]["Account Name"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(headerCode))
                    {
                        _errorMsg += "Header Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(accountName))
                    {
                        _errorMsg += "Account Name can't be blank, ";
                    }

                    if (string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        _errorMsg = string.Empty;
                        for (int d = 2; d < columnNames.Length; d++)
                        {
                            string attachementYear = Convert.ToString(columnNames[d]).Trim();
                            string attachementStatus = Convert.ToString(dtFYAttachment.Rows[i][d]).Trim();

                            if (string.IsNullOrEmpty(attachementYear))
                            {
                                _errorMsg += "FY can't be blank, ";
                            }
                            else
                            {
                                if (!Utilities.isValidNumber(attachementYear.Substring(0, 4)) || !Utilities.isValidNumber(attachementYear.Substring(5)))
                                {
                                    _errorMsg += "Invalid FY, ";
                                }
                                else if (attachementYear.Substring(0, 4).Length != 4 || attachementYear.Substring(5).Length != 4)
                                {
                                    _errorMsg += "Invalid FY, ";
                                }
                                else if (!attachementYear.Contains("-"))
                                {
                                    _errorMsg += "Invalid FY, ";
                                }
                            }
                            if (string.IsNullOrEmpty(attachementStatus))
                            {
                                _errorMsg += "FY Status can't be blank, ";
                            }

                            if (string.IsNullOrEmpty(_errorMsg.Trim()))
                            {
                                ARCreditLimitFYStatus fyDOM = new ARCreditLimitFYStatus();
                                fyDOM.HeaderCode = headerCode;
                                fyDOM.AccountName = accountName;
                                fyDOM.FinancialYear = attachementYear;
                                fyDOM.FYStatus = attachementStatus;
                                fyDOM.CreatedBy = createdBy;
                                fyDOM.RowNumber = (i + 2);
                                fyDOM.FileNamePath = string.Concat(fileFolder, fileName);
                                lstFYAttachment.Add(fyDOM);
                            }
                        } // End Loop
                        if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                        {
                            errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                        }
                    }
                    else
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    #endregion
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _utilitiesARCreditLimitBusinessLogic.FinancialYearAttachmentUpload(lstFYAttachment);
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
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }
        }


        /// <summary>
        /// Read Financial Year Attachment Upload against Header Code
        /// </summary>
        /// <param name="fYStatus"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FinancialYearAttachmentRead([FromBody] ARCreditLimitFYStatus fYStatus)
        {
            try
            {
                var data = await _utilitiesARCreditLimitBusinessLogic.FinancialYearAttachmentRead(fYStatus);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        private bool isValidMonth(string month)
        {
            bool isValid = false;
            List<string> lstMonth = new List<string>() { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            if (!string.IsNullOrEmpty(month))
            {
                var chk = lstMonth.Find(m => m == month.Substring(0, 3).ToUpper());
                if (chk != null)
                {
                    isValid = true;
                }
            }
            return isValid;
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


    }
}
