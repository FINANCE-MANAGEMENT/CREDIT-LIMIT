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
using PhantomJs.NetCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.Areas.ARCreditLimit
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ARCreditLimitController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IWebHostEnvironment _environment;
        private readonly IARCreditLimitBusinessLogic _arCreditLimitBusinessLogic = null;

        public ARCreditLimitController(JwtSettings jwtSettings, IConfiguration configuration,
            IWebHostEnvironment environment, IARCreditLimitBusinessLogic arCreditLimitBusinessLogic)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _environment = environment;
            _arCreditLimitBusinessLogic = arCreditLimitBusinessLogic;
        }

        /// <summary>
        /// Read Header Code Details
        /// </summary>
        /// <param name="HeaderCode"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadHeaderCodeDetail([FromBody] Distributor distributor)
        {
            try
            {
                if (string.IsNullOrEmpty(distributor.HeaderCode))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                var response = await _arCreditLimitBusinessLogic.ReadHeaderCodeDetail(distributor);
                return Ok(new APIResponse
                {
                    Status = response.Status,
                    StatusDesc = response.StatusDesc,
                    data = response
                });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Credit Limit Requset Submit
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreditLimitRequestSave([FromBody] ARCreditLimitDOM creditLimitDOM)
        {
            try
            {
                if(string.IsNullOrEmpty(creditLimitDOM.Distributor.HeaderCode))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                if(!Utilities.isValidDecimal(creditLimitDOM.Insurance.CreditLimitRequestAmount.Value.ToString()))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Credit Limit Request amount is Invalid." }));
                }

                APIResponse result = await _arCreditLimitBusinessLogic.CreditLimitRequestSave(creditLimitDOM);
                //if(result.Status == Utilities.SUCCESS)
                //{
                //    using (var client = new System.Net.Http.HttpClient())
                //    {
                //        client.BaseAddress = new Uri("http://localhost:54978/");
                //        client.DefaultRequestHeaders.Accept.Clear();
                //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //        var response = await client.GetAsync("api/ARCreditLimitScheduler/CreditLimitAutoEmailSent?status=REQUEST&RequestId=");
                //    }
                //}
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
        /// Financial Year Attachement Upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult FYAttachmentUpload()
        {
            try
            {
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string fileFolder = string.Concat(_configuration["ARCreditLimit_UploadPath"], "FinancialStatement\\");

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
                string fileName = string.Concat(Request.Form["HeaderCode"], "_", Request.Form["fileName"], "_", createdBy, "_", uniqueGuid.ToString(), Path.GetExtension(postedFile.FileName));
                string filePath = Path.Combine(path, fileName);

                //Save the File.
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK),
                    new APIResponse { Status = Utilities.SUCCESS, StatusDesc = "File Uploaded Successfully...", data = new { FilePath = string.Concat(fileFolder, fileName) } }));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = "File Uploading Issues." }));
            }
        }


        /// <summary>
        /// Read All Credit Limit Requests
        /// </summary>
        /// <param name="creditLimitDOM"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadAllRequest([FromBody] ARCreditLimitDOM creditLimitDOM)
        {
            try
            {
                var response = await _arCreditLimitBusinessLogic.ReadAllRequest(creditLimitDOM);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = response });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// All Credit Limit Requests Approval
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RequestApproval()
        {
            try
            {
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
                DataTable dtRequestApprove = Utilities.MapExcelToDictionary(filePath);
                if (dtRequestApprove.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<ARCreditLimitDOM> lstRequestApprove = new List<ARCreditLimitDOM>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;
                // Check Excel Columns Name
                if (!dtRequestApprove.Columns[0].ColumnName.Equals("Request Id"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[1].ColumnName.Equals("Header Code"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[2].ColumnName.Equals("Account Name"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[3].ColumnName.Equals("CTRL_AU"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[4].ColumnName.Equals("System Limit"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[5].ColumnName.Equals("Limit Request"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[6].ColumnName.Equals("Request Date (mm/dd/yyyy)"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[7].ColumnName.Equals("End Date (mm/dd/yyyy)"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[8].ColumnName.Equals("New Limit"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[9].ColumnName.Equals("Status"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[10].ColumnName.Equals("Branch Remark"))
                {
                    excelHeaderCheck++;
                }
                if (!dtRequestApprove.Columns[11].ColumnName.Equals("Approval Remark"))
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
                for (int i = 0; i < dtRequestApprove.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string requestId = Convert.ToString(dtRequestApprove.Rows[i]["Request Id"]).Trim();
                    string headerCode = Convert.ToString(dtRequestApprove.Rows[i]["Header Code"]).Trim();
                    string newApprovedLimitAmount = Convert.ToString(dtRequestApprove.Rows[i]["New Limit"]).Trim();
                    string status = Convert.ToString(dtRequestApprove.Rows[i]["Status"]).Trim();
                    string approvalRemark = Convert.ToString(dtRequestApprove.Rows[i]["Approval Remark"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(requestId))
                    {
                        _errorMsg += "Request Id can't be blank, ";
                    }
                    else
                    {
                        if (!Utilities.isValidNumber(requestId))
                        {
                            _errorMsg += "Request Id is Invalid, ";
                        }
                        else if (Convert.ToInt32(requestId) < 1)
                        {
                            _errorMsg += "Request Id is Invalid, ";
                        }
                    }
                    if (string.IsNullOrEmpty(headerCode))
                    {
                        _errorMsg += "Header Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(newApprovedLimitAmount))
                    {
                        _errorMsg += "New Credit Limit Amount can't be blank, ";
                    }
                    else
                    {
                        if (!Utilities.isValidDecimal(newApprovedLimitAmount))
                        {
                            _errorMsg += "New Credit Limit Amount is Invalid, ";
                        }
                    }
                    if (string.IsNullOrEmpty(status))
                    {
                        _errorMsg += "Status can't be blank, ";
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    else
                    {
                        ARCreditLimitDOM creditLimitDOM = new ARCreditLimitDOM();
                        creditLimitDOM.ReqId = Convert.ToInt32(requestId);
                        creditLimitDOM.Distributor.HeaderCode = headerCode;
                        creditLimitDOM.Insurance.CreditLimitRequestAmountApproved = Convert.ToDecimal(newApprovedLimitAmount);
                        creditLimitDOM.AdminRemarks = approvalRemark;
                        creditLimitDOM.Status = status;
                        creditLimitDOM.CreatedBy = createdBy;
                        creditLimitDOM.RowNumber = (i + 2);
                        creditLimitDOM.FileNamePath = string.Concat(_configuration["ARCreditLimit_UploadPath"], fileName);
                        lstRequestApprove.Add(creditLimitDOM);
                    }
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _arCreditLimitBusinessLogic.RequestApproval(lstRequestApprove);
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


        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> RequestDownload([FromBody] ARCreditLimitDOM creditLimitDOM)
        {
            try
            {
                // Email Template data Read
                string clRequestDetail_Template = string.Empty;
                string clRequestTemplatePath = Path.Combine(this._environment.WebRootPath, "Templates", "ARCreditLimit", "ARCreditLimitDetail_Template.html");
                try
                {
                    clRequestDetail_Template = System.IO.File.ReadAllText(clRequestTemplatePath);
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse
                        {
                            Status = Utilities.ERROR,
                            StatusDesc = "Credit Limit Request detail Template not available. Kindly Co-ordinate with system administrator.",
                        }));
                }

                string pathOfGeneratedPdf = string.Empty;
                string htmlBody = clRequestDetail_Template;

                var folderDirectory = Path.Combine(this._environment.WebRootPath, "Downloads", "ARCreditLimit");
                if (!Directory.Exists(folderDirectory))
                {
                    Directory.CreateDirectory(folderDirectory);
                }

                // Read Requset Id Details
                var clRequestDetail = await _arCreditLimitBusinessLogic.ReadAllRequest(creditLimitDOM);
                var clDetail = clRequestDetail[0];

                pathOfGeneratedPdf = Path.Combine(folderDirectory, clDetail.Distributor.HeaderCode + "_" + clDetail.ReqId + ".pdf");
                // Check File is Exit or Not
                FileInfo fi = new FileInfo(@pathOfGeneratedPdf);
                if (fi != null)
                {
                    if (clDetail.LastUpdatedDate < fi.CreationTime)
                    {
                        if (!System.IO.File.Exists(pathOfGeneratedPdf)) return NotFound();
                        var memory1 = new MemoryStream();
                        await using (var stream = new FileStream(pathOfGeneratedPdf, FileMode.Open))
                        {
                            await stream.CopyToAsync(memory1);
                        }
                        memory1.Position = 0;
                        return File(memory1, GetContentType(pathOfGeneratedPdf));
                    }
                }


                #region HTML Body

                //string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(vendor.ConfirmationPeriod.Substring(4)));
                htmlBody = htmlBody.Replace("{{HEADER_CODE}}", clDetail.Distributor.HeaderCode);
                htmlBody = htmlBody.Replace("{{CTRL_AU}}", clDetail.Distributor.CTRL_AU);
                htmlBody = htmlBody.Replace("{{ACCOUNT_NAME}}", clDetail.Distributor.AccountName);
                htmlBody = htmlBody.Replace("{{INSURANCE_CODE}}", clDetail.Insurance.InsuranceCode);
                htmlBody = htmlBody.Replace("{{INSURANCE_NAME}}", clDetail.Insurance.InsuranceName);
                htmlBody = htmlBody.Replace("{{CREATED_DATE}}", clDetail.CreatedDate.ToString());
                htmlBody = htmlBody.Replace("{{CREATED_BY_NAME}}", clDetail.CreatedByName);
                htmlBody = htmlBody.Replace("{{STATUS}}", clDetail.Status);
                htmlBody = htmlBody.Replace("{{ADMIN_REMARKS}}", clDetail.AdminRemarks);

                htmlBody = htmlBody.Replace("{{CURRENT_CREDIT_LIMIT_AMOUNT}}", Utilities.AmountFormatWithoutDecimal(clDetail.Insurance.CurrentCreditLimitAmount));
                htmlBody = htmlBody.Replace("{{CURRENT_CREDIT_LIMIT_ENDDATE}}", clDetail.Insurance.CurrentCreditLimitEndDate == null ? "" : Convert.ToDateTime(clDetail.Insurance.CurrentCreditLimitEndDate).ToString("dd-MMM-yyyy"));
                htmlBody = htmlBody.Replace("{{CREDIT_LIMIT_REQUEST_AMOUNT}}", Utilities.AmountFormatWithoutDecimal(clDetail.Insurance.CreditLimitRequestAmount));


                // Sales History
                StringBuilder sbSalesHistory = new StringBuilder();
                foreach (var salesHis in clDetail.SalesHistory)
                {
                    sbSalesHistory.Append("<tr>");
                    sbSalesHistory.Append("<th style='width: 140px!important;'>" + salesHis.DataSource + "</th>");
                    sbSalesHistory.Append("<td style='text-align: right;'>" + Utilities.AmountFormatWithoutDecimal(salesHis.SaleAmount) + "</td>");
                    sbSalesHistory.Append("<td style='text-align: right;'>" + Utilities.AmountFormatWithoutDecimal(salesHis.AvgSaleAmount) + "</td>");
                    sbSalesHistory.Append("<td style='text-align: right;'>" + Utilities.AmountFormatWithoutDecimal(salesHis.MaxSaleAmount) + "</td>");
                    sbSalesHistory.Append("<td style='text-align: center;'>" + salesHis.MaxSaleMonthYear + "</td>");
                    sbSalesHistory.Append("</tr>");
                }
                htmlBody = htmlBody.Replace("{{SALES_HISTORY}}", sbSalesHistory.ToString());

                // Last Six Month Sales and Payment Trend
                StringBuilder sbSalesPaymentTrend_MonthYear = new StringBuilder();
                StringBuilder sbSalesPaymentTrend_SaleAmount = new StringBuilder();
                StringBuilder sbSalesPaymentTrend_CollectionAmount = new StringBuilder();
                foreach (var salesPayment in clDetail.Sales_PaymentTrend)
                {
                    sbSalesPaymentTrend_MonthYear.Append("<th>" + salesPayment.MonthYear + "</th>");
                    sbSalesPaymentTrend_SaleAmount.Append("<td style='text-align: right;'>" + Utilities.AmountFormatWithoutDecimal(salesPayment.SaleAmount) + "</td>");
                    sbSalesPaymentTrend_CollectionAmount.Append("<td style='text-align: right;'>" + Utilities.AmountFormatWithoutDecimal(salesPayment.CollectionAmount) + "</td>");
                }
                htmlBody = htmlBody.Replace("{{SALES_PAYMENT_TREND_MONTHYEAR}}", sbSalesPaymentTrend_MonthYear.ToString());
                htmlBody = htmlBody.Replace("{{SALES_PAYMENT_TREND_SALE_AMOUNT}}", sbSalesPaymentTrend_SaleAmount.ToString());
                htmlBody = htmlBody.Replace("{{SALES_PAYMENT_TREND_COLLECTION_AMOUNT}}", sbSalesPaymentTrend_CollectionAmount.ToString());

                // OD History
                htmlBody = htmlBody.Replace("{{OD_HISTORY_TOTAL_OD}}", clDetail.OD_History.TotalOD.ToString());
                htmlBody = htmlBody.Replace("{{OD_HISTORY_MAX_OD}}", Utilities.AmountFormatWithoutDecimal(clDetail.OD_History.MaxOD));
                htmlBody = htmlBody.Replace("{{OD_HISTORY_AVERAGE_OD}}", Utilities.AmountFormatWithoutDecimal(clDetail.OD_History.AverageOD));

                // Futue Sales Plan
                StringBuilder sbFutureSalesPlan_MonthYear = new StringBuilder();
                StringBuilder sbFutureSalesPlan_SaleAmount = new StringBuilder();
                foreach (var salesPayment in clDetail.FutureSalesPlan)
                {
                    sbFutureSalesPlan_MonthYear.Append("<th>" + salesPayment.MonthYear + "</th>");
                    sbFutureSalesPlan_SaleAmount.Append("<td style='text-align: right;'>" + Utilities.AmountFormatWithoutDecimal(salesPayment.SaleAmount) + "</td>");
                }
                htmlBody = htmlBody.Replace("{{FUTURE_SALES_PLAN_MONTHYEAR}}", sbFutureSalesPlan_MonthYear.ToString());
                htmlBody = htmlBody.Replace("{{FUTURE_SALES_PLAN_SALE_AMOUNT}}", sbFutureSalesPlan_SaleAmount.ToString());


                //Remarks
                htmlBody = htmlBody.Replace("{{BRANCH_REMARKS}}", clDetail.Remarks);

                // Financial Statements
                StringBuilder sbFinancialStatemet = new StringBuilder();
                foreach (var fs in clDetail.FYAttachment)
                {
                    sbFinancialStatemet.Append("<tr><td>FY " + fs.FinancialYear + "</td>" + "<td>" +
                        (string.IsNullOrEmpty(fs.FileNamePath) ? "N/A" : "Available") + "</td></tr>");
                }
                htmlBody = htmlBody.Replace("{{FINANCIAL_STATEMENTS}}", sbFinancialStatemet.ToString());

                // Notes
                StringBuilder sbNotes = new StringBuilder();
                int noteCount = 1;
                foreach (var note in clDetail.Notes)
                {
                    sbNotes.Append("<tr><td>" + noteCount + "</td>" + "<td>" + note.Note + "</td></tr>");
                    noteCount++;
                }
                htmlBody = htmlBody.Replace("{{NOTES}}", sbNotes.ToString());



                #endregion
                pathOfGeneratedPdf = HtmlToPDF(htmlBody, folderDirectory, clDetail);

                var filePath = Path.Combine(pathOfGeneratedPdf);
                if (!System.IO.File.Exists(filePath)) return NotFound();
                var memory = new MemoryStream();
                await using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                // Delete File 
                if (filePath != null || filePath != string.Empty)
                {
                    if ((System.IO.File.Exists(filePath)))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                return File(memory, GetContentType(filePath));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
        }

        /// <summary>
        /// Bulk Credit Limit Request Generate
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreditLimitRequestBulk()
        {
            try
            {
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string fileFolder = string.Concat(_configuration["ARCreditLimit_UploadPath"], "CreditLimitRequestBulk\\");

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
                DataTable dtBulkRequest = Utilities.MapExcelToDictionary(filePath);
                if (dtBulkRequest.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<ARCreditLimitDOM> lstBulkRequest = new List<ARCreditLimitDOM>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name
                string[] columnNames = dtBulkRequest.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();


                if (!columnNames[0].Equals("Header Code"))
                {
                    excelHeaderCheck++;
                }
                if (!columnNames[1].Equals("Account Name"))
                {
                    excelHeaderCheck++;
                }
                //if (!columnNames[2].Equals("CTRL_AU"))
                //{
                //    excelHeaderCheck++;
                //}
                if (!columnNames[2].Equals("Credit Limit Request"))
                {
                    excelHeaderCheck++;
                }
                if (!columnNames[3].Equals("Remarks (Limit request/ buyer credibility)"))
                {
                    excelHeaderCheck++;
                }

                //for (int i = 4; i < columnNames.Length; i++)
                //{
                //    if (columnNames[i].Length != 8)
                //    {
                //        excelHeaderCheck++;
                //    }
                //}

                if (excelHeaderCheck > 0)
                {
                    errors.Add(new Errors { Error = Utilities.INVALID_EXCEL_FILE });
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE, data = errors }));
                }

                // Read data from datatable
                for (int i = 0; i < dtBulkRequest.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string headerCode = Convert.ToString(dtBulkRequest.Rows[i]["Header Code"]).Trim();
                    string accountName = Convert.ToString(dtBulkRequest.Rows[i]["Account Name"]).Trim();
                    //string ctrl_au = Convert.ToString(dtBulkRequest.Rows[i]["CTRL_AU"]).Trim();
                    string creditLimitRequestAmount = Convert.ToString(dtBulkRequest.Rows[i]["Credit Limit Request"]).Trim();
                    string requesterRemarks = Convert.ToString(dtBulkRequest.Rows[i]["Remarks (Limit request/ buyer credibility)"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(headerCode))
                    {
                        _errorMsg += "Header Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(accountName))
                    {
                        _errorMsg += "Account Name can't be blank, ";
                    }
                    //if (string.IsNullOrEmpty(ctrl_au))
                    //{
                    //    _errorMsg += "CTRL_AU can't be blank, ";
                    //}
                    if (string.IsNullOrEmpty(creditLimitRequestAmount))
                    {
                        _errorMsg += "Credit Limit Request Amount can't be blank, ";
                    }
                    else if (!Utilities.isValidDecimal(creditLimitRequestAmount))
                    {
                        _errorMsg += "Invalid Credit Limit Request Amount, ";
                    }
                    if (string.IsNullOrEmpty(requesterRemarks))
                    {
                        _errorMsg += "Remarks (Limit request/ buyer credibility) can't be blank, ";
                    }

                    if (string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        List<ARCreditLimitSalesDOM> lstFutureSalesPlan = new List<ARCreditLimitSalesDOM>();
                        for (int d = 4; d < columnNames.Length; d++)
                        {
                            _errorMsg = string.Empty;
                            string monthYear = Convert.ToString(columnNames[d]).Trim();
                            string futureSaleAmount = Convert.ToString(dtBulkRequest.Rows[i][d]).Trim();

                            if (string.IsNullOrEmpty(monthYear))
                            {
                                _errorMsg += "Month Year can't be blank, ";
                            }
                            else
                            {
                                try
                                {
                                    monthYear = monthYear.Substring(monthYear.IndexOf('(') + 1);
                                    monthYear = monthYear.Substring(0, monthYear.IndexOf(')'));
                                }
                                catch (Exception)
                                {
                                    _errorMsg += "Invalid Month Year, ";
                                }

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
                            if (string.IsNullOrEmpty(futureSaleAmount))
                            {
                                _errorMsg += "Future Sale Amount can't be blank, ";
                            }
                            else
                            {
                                if (!Utilities.isValidDecimal(futureSaleAmount))
                                {
                                    _errorMsg += "Future Sale Amount is Invalid, ";
                                }
                            }

                            if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                            {
                                errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                            }
                            else
                            {
                                ARCreditLimitSalesDOM futureSalesPlan = new ARCreditLimitSalesDOM();
                                futureSalesPlan.HeaderCode = headerCode;
                                futureSalesPlan.AccountName = accountName;
                                futureSalesPlan.MonthYear = monthYear.ToUpper();
                                futureSalesPlan.SaleAmount = Convert.ToDecimal(futureSaleAmount);
                                lstFutureSalesPlan.Add(futureSalesPlan);
                            }
                        }

                        if (errors.Count == 0)
                        {
                            ARCreditLimitDOM creditLimitRequestBulk = new ARCreditLimitDOM();
                            creditLimitRequestBulk.Distributor.HeaderCode = headerCode;
                            creditLimitRequestBulk.Distributor.AccountName = accountName;
                            //creditLimitRequestBulk.Distributor.CTRL_AU = ctrl_au;
                            creditLimitRequestBulk.Insurance.CreditLimitRequestAmount = Convert.ToDecimal(creditLimitRequestAmount);
                            creditLimitRequestBulk.Remarks = requesterRemarks;
                            creditLimitRequestBulk.CreatedBy = createdBy;
                            creditLimitRequestBulk.RowNumber = (i + 2);
                            creditLimitRequestBulk.FutureSalesPlan = lstFutureSalesPlan;
                            lstBulkRequest.Add(creditLimitRequestBulk);
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
                APIResponse result = await _arCreditLimitBusinessLogic.CreditLimitRequestBulk(lstBulkRequest);
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


        private string HtmlToPDF(string pdfContent, string folderDirectory, ARCreditLimitDOM creditLimitDOM)
        {
            var generator = new PdfGenerator();

            // Generate pdf from html and place in the current folder.
            string pathOfGeneratedPdf = generator.GeneratePdf(pdfContent, folderDirectory, creditLimitDOM.Distributor.HeaderCode + "_" + creditLimitDOM.ReqId);
            return pathOfGeneratedPdf;
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
