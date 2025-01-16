using CoreHtmlToImage;
using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.Areas.VMS
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VendorConfirmationController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IVendorConfirmationBusinessLogic _vendorConfirmationBusinessLogic = null;
        private readonly IUtilitiesVMSBusinessLogic _utilitiesVMSBusinessLogic = null;
        private readonly IWebHostEnvironment _environment;

        public VendorConfirmationController(JwtSettings jwtSettings, IConfiguration configuration, IVendorConfirmationBusinessLogic vendorConfirmationBusinessLogic,
                                            IUtilitiesVMSBusinessLogic utilitiesVMSBusinessLogic, IWebHostEnvironment environment)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _vendorConfirmationBusinessLogic = vendorConfirmationBusinessLogic;
            _utilitiesVMSBusinessLogic = utilitiesVMSBusinessLogic;
            _environment = environment;
        }

        /// <summary>
        /// Vendor/ Supplier Invoices upload by Admin Only. First time Quarter wise
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorInvoiceUpload()
        {
            try
            {
                //var file = Request.Form.Files[0]; // working fine
                //var file = Request.Form["File"];
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string localIP = Request.Form["LocalIP"].ToString();
                string publicIP = Request.Form["PublicIP"].ToString();

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
                DataTable dtInvoices = Utilities.MapExcelToDictionary(filePath);
                if (dtInvoices.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<VendorInvoice> lstInvoices = new List<VendorInvoice>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name
                if (!dtInvoices.Columns[0].ColumnName.Equals("Confirmation Period"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInvoices.Columns[1].ColumnName.Equals("Vendor Code"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInvoices.Columns[2].ColumnName.Equals("Vendor Name"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInvoices.Columns[3].ColumnName.Equals("Branch"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInvoices.Columns[4].ColumnName.Equals("Invoice No"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInvoices.Columns[5].ColumnName.Equals("Invoice Date (mm/dd/yyyy)"))
                {
                    excelHeaderCheck++;
                }
                if (!dtInvoices.Columns[6].ColumnName.Equals("Closing Balance"))
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
                for (int i = 0; i < dtInvoices.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string confirmationPeriod = Convert.ToString(dtInvoices.Rows[i]["Confirmation Period"]).Trim();
                    string vendorCode = Convert.ToString(dtInvoices.Rows[i]["Vendor Code"]).Trim();
                    string vendorName = Convert.ToString(dtInvoices.Rows[i]["Vendor Name"]).Trim();
                    string branchcode = Convert.ToString(dtInvoices.Rows[i]["Branch"]).Trim();
                    string invoiceNo = Convert.ToString(dtInvoices.Rows[i]["Invoice No"]).Trim();
                    string invoiceDate = Convert.ToString(dtInvoices.Rows[i]["Invoice Date (mm/dd/yyyy)"]).Trim();
                    string closingBalance = Convert.ToString(dtInvoices.Rows[i]["Closing Balance"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(confirmationPeriod))
                    {
                        _errorMsg += "Confirmation Period can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(vendorCode))
                    {
                        _errorMsg += "Vendor Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(branchcode))
                    {
                        _errorMsg += "Branch can't be blank, ";
                    }
                    //if (string.IsNullOrEmpty(invoiceNo))
                    //{
                    //    _errorMsg += "Invoice No. can't be blank, ";
                    //}

                    if (!string.IsNullOrEmpty(invoiceDate))
                    {
                        try
                        {
                            invoiceDate = DateTime.FromOADate(Convert.ToDouble(invoiceDate)).ToString().Split(' ')[0];
                            if (!string.IsNullOrEmpty(invoiceDate.Trim()))
                            {
                                if (!Utilities.isValidDate(invoiceDate))
                                {
                                    _errorMsg += "Invoice Date is Invalid, ";
                                }
                                else
                                {
                                    DateTime datevalue = Convert.ToDateTime(invoiceDate);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            _errorMsg += "Invoice Date is Invalid, ";
                        }
                    }

                    if (string.IsNullOrEmpty(closingBalance))
                    {
                        _errorMsg += "Closing Balance can't be blank, ";
                    }
                    else
                    {
                        try
                        {
                            if (!Utilities.isValidDecimal(closingBalance))
                            {
                                _errorMsg += "Closing Balance is Invalid, ";
                            }
                        }
                        catch (Exception)
                        {
                            _errorMsg += "Closing Balance is Invalid, ";
                        }
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    else
                    {
                        VendorInvoice vendorInvoice = new VendorInvoice();
                        vendorInvoice.ConfirmationPeriod = confirmationPeriod;
                        vendorInvoice.Vendors.VendorCode = vendorCode;
                        vendorInvoice.Vendors.VendorName = vendorName;
                        vendorInvoice.BranchCode = branchcode;
                        vendorInvoice.InvoiceNo = invoiceNo;
                        if (!string.IsNullOrEmpty(invoiceDate))
                        {
                            vendorInvoice.InvoiceDate = Convert.ToDateTime(invoiceDate);
                        }
                        vendorInvoice.ClosingBalance = Convert.ToDecimal(closingBalance);
                        vendorInvoice.CreatedBy = createdBy;
                        vendorInvoice.RowNumber = (i + 2);
                        vendorInvoice.LocalIP = localIP;
                        vendorInvoice.PublicIP = publicIP;
                        lstInvoices.Add(vendorInvoice);
                    }
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _vendorConfirmationBusinessLogic.VendorInvoiceUpload(lstInvoices);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));

            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }

        }

        /// <summary>
        /// Vendor/ Supplier Invoice Verify & Approve by Admin , After First time Invoice upload by admin
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorInvoiceVerifyApproveByAdmin([FromBody] List<VendorInvoice> vendorInvoices, Int32 createdBy)
        {
            APIResponse apiResponse = null;
            List<Errors> errors = new List<Errors>();
            try
            {
                // Basic validation
                if (vendorInvoices.Count == 0 || vendorInvoices.Where(m => m.AdminApprovedStatus == true).ToList().Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Please select atleast one Vendor/ Supplier confirmation detail." }));
                }

                // Email Template data Read
                string VendorInvoiceDetails_Template = string.Empty;
                string VendorInvoiceTemplatePath = Path.Combine(this._environment.WebRootPath, "Templates", "VMS", "VendorInvoiceDetail_Template.html");
                try
                {
                    VendorInvoiceDetails_Template = System.IO.File.ReadAllText(VendorInvoiceTemplatePath);
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse
                        {
                            Status = Utilities.ERROR,
                            StatusDesc = "Vendor/ Supplier invoice detail Template not available. Kindly Co-ordinate with system administrator.",
                        }));
                }

                // Status Update in database for Admin Approved.
                List<VendorInvoice> lstFinalApprovedVendors = new List<VendorInvoice>();
                foreach (var vendor in vendorInvoices.Where(m => m.AdminApprovedStatus == true).ToList())
                {
                    var response = await _vendorConfirmationBusinessLogic.VendorInvoiceVerifyApprove(vendor, createdBy);
                    if (response.Status == Utilities.SUCCESS)
                    {
                        var vendorExist = lstFinalApprovedVendors.Where(m => m.Vendors.VendorCode == vendor.Vendors.VendorCode).FirstOrDefault();
                        if (vendorExist == null)
                        {
                            VendorInvoice vendorInvoice = new VendorInvoice();
                            vendorInvoice.ConfirmationPeriod = vendor.ConfirmationPeriod;
                            vendorInvoice.Vendors.VendorCode = vendor.Vendors.VendorCode;
                            vendorInvoice.Vendors.VendorName = vendor.Vendors.VendorName;
                            vendorInvoice.Vendors.EmailId = ((Vendor)response.data).EmailId;
                            vendorInvoice.Vendors.MobileNo = ((Vendor)response.data).MobileNo;
                            lstFinalApprovedVendors.Add(vendorInvoice);
                        }
                    }
                    else
                    {
                        errors.Add((Errors)response.data);
                    }
                }

                // check if any error occured, while updating Approved status in database
                if (errors.Count > 0)
                {
                    // Rollback all invoice approved by admin
                    List<VendorInvoice> lstApprovedVendorsRollback = new List<VendorInvoice>();
                    string confirmationPeriod = vendorInvoices.FirstOrDefault().ConfirmationPeriod;
                    var rollbackVendorsCode = vendorInvoices.Select(x => x.Vendors.VendorCode).Distinct();

                    foreach (var _vendorCode in rollbackVendorsCode)
                    {
                        VendorInvoice _vendorInvoice = new VendorInvoice();
                        _vendorInvoice.ConfirmationPeriod = confirmationPeriod;
                        _vendorInvoice.Vendors.VendorCode = _vendorCode;
                        lstApprovedVendorsRollback.Add(_vendorInvoice);
                    }

                    var rollbackResponse = await _vendorConfirmationBusinessLogic.VendorInvoiceApprovedRollback(lstApprovedVendorsRollback, createdBy);
                    if (rollbackResponse.Status == Utilities.ERROR)
                    {
                        foreach (var _errors in (List<Errors>)rollbackResponse.data)
                        {
                            errors.Add(_errors);
                        }
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    apiResponse = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = Utilities.SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD,
                        data = errors
                    }));
                }


                // All Approved Invoice details send to vendor/Supplier on Email & SMS.
                foreach (var vendor in lstFinalApprovedVendors)
                {
                    string emailBody = VendorInvoiceDetails_Template;

                    #region Email Body
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(vendor.ConfirmationPeriod.Substring(4)));
                    emailBody = emailBody.Replace("{{VENDOR_NAME}}", string.Concat(vendor.Vendors.VendorName, " (", vendor.Vendors.VendorCode, ")"));
                    emailBody = emailBody.Replace("{{CONFIRMATION_PERIOD_MONTH}}", string.Concat(monthName.Substring(0, 3), " ", vendor.ConfirmationPeriod.Substring(0, 4)));

                    StringBuilder sbInvoiceDetail = new StringBuilder();
                    List<VendorInvoice> lstInvoiceDetails = await _vendorConfirmationBusinessLogic.ReadVendorInvoice(vendor);
                    decimal totalClosingAmt = 0;
                    foreach (var invDetail in lstInvoiceDetails)
                    {
                        totalClosingAmt += invDetail.ClosingBalance;
                        sbInvoiceDetail.Append("<tr><td>" + invDetail.BranchCode + "</td><td>" + invDetail.InvoiceNo + "</td><td>" +
                                            (invDetail.InvoiceDate == null ? "" : Convert.ToDateTime(invDetail.InvoiceDate).ToString("dd-MMM-yyyy")) +
                                            "</td><td>" + invDetail.ClosingBalance.ToString("#,##0.00") + "</td></tr>");
                    }
                    sbInvoiceDetail.Append("<tr><td colspan='3' style='text-align:center;font-weight:bold'>Total Closing Balance</td><td style='font-weight:bold'>" +
                                            totalClosingAmt.ToString("#,##0.00") + "</td></tr>");
                    emailBody = emailBody.Replace("{{INVOICE_DETAIL}}", sbInvoiceDetail.ToString());
                    #endregion

                    #region Invoice details Convert into Image

                    var converter = new HtmlConverter();
                    var imgBytes = converter.FromHtmlString(emailBody.ToString());
                    Guid guid = Guid.NewGuid();
                    string imageFilePath = Path.Combine(this._environment.WebRootPath, "Templates", "VMS", vendor.Vendors.VendorCode + "_" + guid.ToString() + ".png");
                    //System.IO.File.WriteAllBytes(imageFilePath, imgBytes);
                    //emailBody = "<img src='" + imageFilePath + "' />";
                    #endregion

                    #region Email send to Vendor/ Supplier with Invoice details
                    if (!string.IsNullOrEmpty(vendor.Vendors.EmailId))
                    {
                        try
                        {
                            EMail email = new EMail();
                            email.To = vendor.Vendors.EmailId;
                            //email.CC = "";
                            email.MailSubject = string.Concat("AP Balance Confirmation for the period ending- ", vendor.ConfirmationPeriod);
                            email.MailBody = emailBody;
                            Utilities.SendingEmail(email, _configuration);

                            vendor.EmailSend = "Y";
                            var emailResp = await _vendorConfirmationBusinessLogic.LGInvoiceEmailSendToVendorStatusUpdate(vendor, createdBy);
                        }
                        catch (Exception ex)
                        {
                            Utilities.CreateLogFile(Utilities.PROJECTS.VMS, vendor.Vendors.VendorCode + "," + vendor.ConfirmationPeriod + "#" + ex);
                            vendor.EmailSend = "N";
                            var emailResp = await _vendorConfirmationBusinessLogic.LGInvoiceEmailSendToVendorStatusUpdate(vendor, createdBy);
                        }
                    }
                    #endregion

                    #region SMS Send to Vendor/ Supplier

                    if (!string.IsNullOrEmpty(vendor.Vendors.MobileNo))
                    {
                        try
                        {
                            string SMSTemplateName = "VMS_PARTNER_COMMUNICATION";

                            // SMS Template & message read
                            SmsData smsDLT = _utilitiesVMSBusinessLogic.SMS_DLT_DetailRead(SMSTemplateName);
                            if (smsDLT != null)
                            {
                                OTP otp = new OTP();
                                otp.MobileNo = vendor.Vendors.MobileNo;
                                otp.CreatedBy = createdBy;
                                otp.ProcessName = "VENDOR_CONFIRMATION_BY_LG";

                                var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32(vendor.ConfirmationPeriod.Substring(0, 4)), Convert.ToInt32(vendor.ConfirmationPeriod.Substring(4)));
                                string quarterEndDate = string.Concat(lastDayOfMonth, "-", monthName.Substring(0, 3), "-", vendor.ConfirmationPeriod.Substring(0, 4)); // dd-MMM-yyyy
                                string message = smsDLT.MESSAGE.Replace("{var1}{var2}", "Confirmation for Qtr ended on " + quarterEndDate + " initiated by LG"); // SMS body change
                                var smsResp = await _utilitiesVMSBusinessLogic.SMSSend(smsDLT, otp, SMSTemplateName, otp.MobileNo, message);
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.CreateLogFile(Utilities.PROJECTS.VMS, vendor.Vendors.VendorCode + "," + vendor.ConfirmationPeriod + "SMS Sending Issues. " + "#" + ex);
                        }
                    }

                    #endregion

                }

                apiResponse = new APIResponse
                {
                    Status = Utilities.SUCCESS,
                    StatusDesc = "Vendor/ Supplier Invoice Approved Successfully...",
                };

                return StatusCode((apiResponse.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), apiResponse));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// Read Vendor/ Supplier Invoice
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadVendorInvoice([FromBody] VendorInvoice vendorInvoice)
        {
            try
            {
                var data = await _vendorConfirmationBusinessLogic.ReadVendorInvoice(vendorInvoice);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Read Vendor/ Supplier Invoice Summary
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadVendorInvoiceSummary([FromBody] VendorInvoice vendorInvoice)
        {
            try
            {
                var data = await _vendorConfirmationBusinessLogic.ReadVendorInvoiceSummary(vendorInvoice);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }






        /// <summary>
        /// Vendor/ Supplier Claim Amounts summary submit
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorClaimsAmount([FromBody] VendorInvoice vendorClaims)
        {
            List<Errors> errors = new List<Errors>();
            try
            {
                if (string.IsNullOrEmpty(vendorClaims.ConfirmationPeriod) || string.IsNullOrEmpty(vendorClaims.Vendors.VendorCode) || vendorClaims == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _vendorConfirmationBusinessLogic.VendorClaimsAmount(vendorClaims);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Read Vendor/ Supplier Claims Amount
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadVendorClaimsAmount([FromBody] VendorInvoice vendorInvoice)
        {
            try
            {
                var data = await _vendorConfirmationBusinessLogic.ReadVendorClaimsAmount(vendorInvoice);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }



        /// <summary>
        /// Vendor/ Supplier Invoice Entry By Vendor/ Supplier => Vendor confirmation Invoice
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorInvoiceAddedByVendor([FromBody] List<VendorInvoice> vendorInvoices)
        {
            try
            {
                if (vendorInvoices.Count == 0 || vendorInvoices == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Please enter atleast one invoice detail." }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _vendorConfirmationBusinessLogic.VendorInvoiceAddedByVendor(vendorInvoices);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Read Vendor/ Supplier Invoice Details which is Upload/Added by Vendor
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadVendorInvoiceAddedByVendor([FromBody] VendorInvoice vendorInvoice)
        {
            try
            {
                var data = await _vendorConfirmationBusinessLogic.ReadVendorInvoiceAddedByVendor(vendorInvoice);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// Vendor/ Supplier Invoice "Bulk Upload by Excel" By Vendor/ Supplier => Vendor confirmation Invoice
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorInvoiceUploadByVendor()
        {
            try
            {
                string confirmationPeriod = Request.Form["ConfirmationPeriod"].ToString();
                string vendorCode = Request.Form["VendorCode"].ToString();
                string myFileName = Request.Form["fileName"].ToString();
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string localIP = Request.Form["LocalIP"].ToString();
                string publicIP = Request.Form["PublicIP"].ToString();

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
                string fileName = myFileName + createdBy + uniqueGuid.ToString() + Path.GetExtension(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);

                //Save the File.
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Read data from Excel File
                DataTable dtVendorInvoice = Utilities.MapExcelToDictionary(filePath);
                if (dtVendorInvoice.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<VendorInvoice> lstInvoices = new List<VendorInvoice>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name

                //if (!dtVendorInvoice.Columns[0].ColumnName.Equals("Confirmation Period"))
                //{
                //    excelHeaderCheck++;
                //}
                //if (!dtVendorInvoice.Columns[1].ColumnName.Equals("Vendor Code"))
                //{
                //    excelHeaderCheck++;
                //}
                //if (!dtVendorInvoice.Columns[2].ColumnName.Equals("Vendor Name"))
                //{
                //    excelHeaderCheck++;
                //}
                if (!dtVendorInvoice.Columns[0].ColumnName.Equals("Branch"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[1].ColumnName.Equals("Invoice No"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[2].ColumnName.Equals("Invoice Date (mm/dd/yyyy)"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[3].ColumnName.Equals("Invoice Amount"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[4].ColumnName.Equals("Payment Received"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[5].ColumnName.Equals("Balance"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[6].ColumnName.Equals("LG PO No."))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[7].ColumnName.Equals("Remark"))
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
                for (int i = 0; i < dtVendorInvoice.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    //string confirmationPeriod = Convert.ToString(dtVendorInvoice.Rows[i]["Confirmation Period"]).Trim();
                    //string vendorCode = Convert.ToString(dtVendorInvoice.Rows[i]["Vendor Code"]).Trim();
                    //string vendorName = Convert.ToString(dtVendorInvoice.Rows[i]["Vendor Name"]).Trim();
                    string branchcode = Convert.ToString(dtVendorInvoice.Rows[i]["Branch"]).Trim();
                    string invoiceNo = Convert.ToString(dtVendorInvoice.Rows[i]["Invoice No"]).Trim();
                    string invoiceDate = Convert.ToString(dtVendorInvoice.Rows[i]["Invoice Date (mm/dd/yyyy)"]).Trim();
                    string invoiceAmount = Convert.ToString(dtVendorInvoice.Rows[i]["Invoice Amount"]).Trim();
                    string paymentReceivedAmount = Convert.ToString(dtVendorInvoice.Rows[i]["Payment Received"]).Trim();
                    string balance = Convert.ToString(dtVendorInvoice.Rows[i]["Balance"]).Trim();
                    string LG_PO_No = Convert.ToString(dtVendorInvoice.Rows[i]["LG PO No."]).Trim();
                    string vendorRemarks = Convert.ToString(dtVendorInvoice.Rows[i]["Remark"]).Trim();

                    #region Basic Validation check

                    //if (string.IsNullOrEmpty(confirmationPeriod))
                    //{
                    //    _errorMsg += "Confirmation Period can't be blank, ";
                    //}
                    //if (string.IsNullOrEmpty(vendorCode))
                    //{
                    //    _errorMsg += "Vendor Code can't be blank, ";
                    //}
                    if (string.IsNullOrEmpty(branchcode))
                    {
                        _errorMsg += "Branch can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(invoiceNo))
                    {
                        _errorMsg += "Invoice No. can't be blank, ";
                    }

                    if (string.IsNullOrEmpty(invoiceDate))
                    {
                        _errorMsg += "Invoice Date can't be blank, ";
                    }
                    else
                    {
                        try
                        {
                            invoiceDate = DateTime.FromOADate(Convert.ToDouble(invoiceDate)).ToString().Split(' ')[0];
                            if (!string.IsNullOrEmpty(invoiceDate.Trim()))
                            {
                                if (!Utilities.isValidDate(invoiceDate))
                                {
                                    _errorMsg += "Invoice Date is Invalid, ";
                                }
                                else
                                {
                                    DateTime datevalue = Convert.ToDateTime(invoiceDate);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            _errorMsg += "Invoice Date is Invalid, ";
                        }
                    }

                    if (string.IsNullOrEmpty(invoiceAmount))
                    {
                        _errorMsg += "Invoice Amount can't be blank, ";
                    }
                    else
                    {
                        try
                        {
                            if (!Utilities.isValidDecimal(invoiceAmount))
                            {
                                _errorMsg += "Invoice Amount is Invalid, ";
                            }
                        }
                        catch (Exception)
                        {
                            _errorMsg += "Invoice Amount is Invalid, ";
                        }
                    }

                    if (string.IsNullOrEmpty(paymentReceivedAmount))
                    {
                        _errorMsg += "Payment Received Amount can't be blank, ";
                    }
                    else
                    {
                        try
                        {
                            if (!Utilities.isValidDecimal(paymentReceivedAmount))
                            {
                                _errorMsg += "Payment Received Amount is Invalid, ";
                            }
                        }
                        catch (Exception)
                        {
                            _errorMsg += "Payment Received Amount is Invalid, ";
                        }
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    else
                    {
                        VendorInvoice vendorInvoice = new VendorInvoice();
                        vendorInvoice.ConfirmationPeriod = confirmationPeriod;
                        vendorInvoice.Vendors.VendorCode = vendorCode;
                        //vendorInvoice.Vendors.VendorName = vendorName;
                        vendorInvoice.BranchCode = branchcode;
                        vendorInvoice.InvoiceNo = invoiceNo;
                        vendorInvoice.InvoiceDate = Convert.ToDateTime(invoiceDate);
                        vendorInvoice.InvoiceAmount = Convert.ToDecimal(invoiceAmount);
                        vendorInvoice.ReceivedPaymentAmount = Convert.ToDecimal(paymentReceivedAmount);
                        vendorInvoice.Balance = vendorInvoice.InvoiceAmount - vendorInvoice.ReceivedPaymentAmount;
                        vendorInvoice.LG_PO_No = LG_PO_No;
                        vendorInvoice.VendorRemarks = vendorRemarks;
                        vendorInvoice.CreatedBy = createdBy;
                        vendorInvoice.RowNumber = (i + 1);
                        vendorInvoice.LocalIP = localIP;
                        vendorInvoice.PublicIP = publicIP;
                        lstInvoices.Add(vendorInvoice);
                    }
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = new APIResponse();
                result.Status = Utilities.SUCCESS;
                result.StatusDesc = "Excel Data Read Successfully...";
                result.data = await Task.Run(() => lstInvoices);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));

            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }

        }




        /// <summary>
        /// Read Vendor/ Supplier Confirmation Claims Summary (Only for BAM)
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadVendorConfirmationClaims([FromBody] VendorInvoice vendorInvoice)
        {
            try
            {
                var data = await _vendorConfirmationBusinessLogic.ReadVendorConfirmationClaims(vendorInvoice);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }





        /// <summary>
        /// BAM Claim Reply => "Bulk Upload by Excel" 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> BAMClaimReplyUpload()
        {
            try
            {
                string confirmationPeriod = Request.Form["ConfirmationPeriod"].ToString();
                string vendorCode = Request.Form["VendorCode"].ToString();
                string myFileName = Request.Form["fileName"].ToString();
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string localIP = Request.Form["LocalIP"].ToString();
                string publicIP = Request.Form["PublicIP"].ToString();

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
                string fileName = myFileName + createdBy + uniqueGuid.ToString() + Path.GetExtension(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);

                //Save the File.
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Read data from Excel File
                DataTable dtVendorInvoice = Utilities.MapExcelToDictionary(filePath);
                if (dtVendorInvoice.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<VendorInvoice> lstInvoices = new List<VendorInvoice>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name

                if (!dtVendorInvoice.Columns[0].ColumnName.Equals("Confirmation Period"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[1].ColumnName.Equals("Vendor Code"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[2].ColumnName.Equals("Vendor Name"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[3].ColumnName.Equals("Branch"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[4].ColumnName.Equals("Invoice No"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[5].ColumnName.Equals("Invoice Date (mm/dd/yyyy)"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[6].ColumnName.Equals("Invoice Amount"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[7].ColumnName.Equals("LG PO No."))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[8].ColumnName.Equals("Payment Received"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[9].ColumnName.Equals("Balance"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[10].ColumnName.Equals("Vendor Remark"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[11].ColumnName.Equals("Acceptance(Y/N)"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[12].ColumnName.Equals("Payment Date (mm/dd/yyyy)"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[13].ColumnName.Equals("Payment Amount"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[14].ColumnName.Equals("Balance After Payment Amount"))
                {
                    excelHeaderCheck++;
                }
                if (!dtVendorInvoice.Columns[15].ColumnName.Equals("BAM Remark"))
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
                for (int i = 0; i < dtVendorInvoice.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    //string confirmationPeriod = Convert.ToString(dtVendorInvoice.Rows[i]["Confirmation Period"]).Trim();
                    //string vendorCode = Convert.ToString(dtVendorInvoice.Rows[i]["Vendor Code"]).Trim();
                    //string vendorName = Convert.ToString(dtVendorInvoice.Rows[i]["Vendor Name"]).Trim();
                    string branchcode = Convert.ToString(dtVendorInvoice.Rows[i]["Branch"]).Trim();
                    string invoiceNo = Convert.ToString(dtVendorInvoice.Rows[i]["Invoice No"]).Trim();
                    string invoiceDate = Convert.ToString(dtVendorInvoice.Rows[i]["Invoice Date (mm/dd/yyyy)"]).Trim();
                    string balance = Convert.ToString(dtVendorInvoice.Rows[i]["Balance"]).Trim();
                    string acceptance = Convert.ToString(dtVendorInvoice.Rows[i]["Acceptance(Y/N)"]).Trim();
                    string paymentDate = Convert.ToString(dtVendorInvoice.Rows[i]["Payment Date (mm/dd/yyyy)"]).Trim();
                    string paymentAmount = Convert.ToString(dtVendorInvoice.Rows[i]["Payment Amount"]).Trim();
                    string balanceAfterPaymentAmount = Convert.ToString(dtVendorInvoice.Rows[i]["Balance After Payment Amount"]).Trim();
                    string bamRemark = Convert.ToString(dtVendorInvoice.Rows[i]["BAM Remark"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(confirmationPeriod))
                    {
                        _errorMsg += "Confirmation Period can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(vendorCode))
                    {
                        _errorMsg += "Vendor Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(branchcode))
                    {
                        _errorMsg += "Branch can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(invoiceNo))
                    {
                        _errorMsg += "Invoice No. can't be blank, ";
                    }

                    if (string.IsNullOrEmpty(invoiceDate))
                    {
                        _errorMsg += "Invoice Date can't be blank, ";
                    }
                    else
                    {
                        try
                        {
                            invoiceDate = DateTime.FromOADate(Convert.ToDouble(invoiceDate)).ToString().Split(' ')[0];
                            if (!string.IsNullOrEmpty(invoiceDate.Trim()))
                            {
                                if (!Utilities.isValidDate(invoiceDate))
                                {
                                    _errorMsg += "Invoice Date is Invalid, ";
                                }
                                else
                                {
                                    DateTime datevalue = Convert.ToDateTime(invoiceDate);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            _errorMsg += "Invoice Date is Invalid, ";
                        }
                    }

                    if (string.IsNullOrEmpty(balance))
                    {
                        _errorMsg += "Balance can't be blank, ";
                    }
                    else
                    {
                        try
                        {
                            if (!Utilities.isValidDecimal(balance))
                            {
                                _errorMsg += "Balance Amount is Invalid, ";
                            }
                        }
                        catch (Exception)
                        {
                            _errorMsg += "Balance Amount is Invalid, ";
                        }
                    }

                    if (string.IsNullOrEmpty(acceptance))
                    {
                        _errorMsg += "Acceptation can't be blank, ";
                    }
                    else
                    {
                        acceptance = acceptance.ToUpper();
                        if (acceptance == "Y" || acceptance == "N")
                        {

                        }
                        else
                        {
                            _errorMsg += "Acceptation is Invalid (Y/N), ";
                        }
                    }

                    if (acceptance == "Y")
                    {
                        if (string.IsNullOrEmpty(paymentDate))
                        {
                            _errorMsg += "Payment Date can't be blank, ";
                        }
                        else
                        {
                            try
                            {
                                paymentDate = DateTime.FromOADate(Convert.ToDouble(paymentDate)).ToString().Split(' ')[0];
                                if (!string.IsNullOrEmpty(paymentDate.Trim()))
                                {
                                    if (!Utilities.isValidDate(paymentDate))
                                    {
                                        _errorMsg += "Payment Date is Invalid, ";
                                    }
                                    else
                                    {
                                        DateTime datevalue = Convert.ToDateTime(paymentDate);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                _errorMsg += "Payment Date is Invalid, ";
                            }
                        }

                        if (string.IsNullOrEmpty(paymentAmount))
                        {
                            _errorMsg += "Payment Amount can't be blank, ";
                        }
                        else
                        {
                            try
                            {
                                if (!Utilities.isValidDecimal(paymentAmount))
                                {
                                    _errorMsg += "Payment Amount is Invalid, ";
                                }
                            }
                            catch (Exception)
                            {
                                _errorMsg += "Payment Amount is Invalid, ";
                            }
                        }
                    }


                    #endregion

                    if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    else
                    {
                        VendorInvoice vendorInvoice = new VendorInvoice();
                        vendorInvoice.ConfirmationPeriod = confirmationPeriod;
                        vendorInvoice.Vendors.VendorCode = vendorCode;
                        //vendorInvoice.Vendors.VendorName = vendorName;
                        vendorInvoice.BranchCode = branchcode;
                        vendorInvoice.InvoiceNo = invoiceNo;
                        vendorInvoice.InvoiceDate = Convert.ToDateTime(invoiceDate);
                        vendorInvoice.Balance = Convert.ToDecimal(balance);
                        vendorInvoice.BAMAcceptance = acceptance;
                        if (acceptance == "Y")
                        {
                            vendorInvoice.PaymentDate = Convert.ToDateTime(paymentDate);
                            vendorInvoice.PaymentAmount = Convert.ToDecimal(paymentAmount);
                            vendorInvoice.AfterPaymentBalance = vendorInvoice.Balance - vendorInvoice.PaymentAmount;
                        }
                        vendorInvoice.BAMRemarks = bamRemark;
                        vendorInvoice.CreatedBy = createdBy;
                        vendorInvoice.RowNumber = (i + 1);
                        vendorInvoice.LocalIP = localIP;
                        vendorInvoice.PublicIP = publicIP;
                        lstInvoices.Add(vendorInvoice);
                    }
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = new APIResponse();
                result.Status = Utilities.SUCCESS;
                result.StatusDesc = "Excel Data Read Successfully...";
                result.data = await Task.Run(() => lstInvoices);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));

            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                     new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }

        }


        /// <summary>
        /// Vendor/ Supplier Invoice Entry By Vendor => Reply by BAM
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorInvoiceClaimReplied([FromBody] List<VendorInvoice> vendorInvoices)
        {
            List<Errors> errors = new List<Errors>();
            try
            {
                if (vendorInvoices.Count == 0 || vendorInvoices == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Please check invoice & reply detail." }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _vendorConfirmationBusinessLogic.VendorInvoiceClaimReplied(vendorInvoices);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }



        /// <summary>
        /// Vendor/ Supplier Acceptance by Vendor
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> VendorInvoiceAcceptance([FromBody] VendorInvoice vendorAcceptance)
        {
            try
            {
                if (string.IsNullOrEmpty(vendorAcceptance.ConfirmationPeriod) ||
                    string.IsNullOrEmpty(vendorAcceptance.Vendors.VendorCode) || vendorAcceptance == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _vendorConfirmationBusinessLogic.VendorInvoiceAcceptance(vendorAcceptance);
                return StatusCode((result.Status == Utilities.SUCCESS ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest),
                    Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }



        /// <summary>
        /// Read Vendor/ Supplier Confirmation Tracker Logs
        /// </summary>
        /// <param name="vendorInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReadVendorConfirmationTracker([FromBody] VendorInvoice vendorInvoice)
        {
            try
            {
                var data = await _vendorConfirmationBusinessLogic.ReadVendorConfirmationTracker(vendorInvoice);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.VMS, ex);
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }



        //[HttpPost]
        //[RequestSizeLimit(long.MaxValue)]
        //public async Task<ActionResult<StudentFormSubmissionResult>> SubmitForm(int id, [FromForm] StudentForm form)
        //{
        //    //_logger.LogInformation($"Validating the form#{form.FormId} for Student ID={id}");

        //    //if (form.Courses == null || form.Courses.Length == 0)
        //    //{
        //    //    return BadRequest("Please enter at least one course.");
        //    //}

        //    //if (form.StudentFile == null || form.StudentFile.Length < 1)
        //    //{
        //    //    return BadRequest("The uploaded file is empty.");
        //    //}

        //    //if (Path.GetExtension(form.StudentFile.FileName) != ".pdf")
        //    //{
        //    //    return BadRequest($"The uploaded file {form.StudentFile.Name} is not a PDF file.");
        //    //}

        //    //var filePath = Path.Combine(@"App_Data", $"{DateTime.Now:yyyyMMddHHmmss}.pdf");
        //    //new FileInfo(filePath).Directory?.Create();
        //    //await using (var stream = new FileStream(filePath, FileMode.Create))
        //    //{
        //    //    _logger.LogInformation($"Saving file [{form.StudentFile.FileName}]");
        //    //    await form.StudentFile.CopyToAsync(stream);
        //    //    _logger.LogInformation($"\t The uploaded file is saved as [{filePath}].");
        //    //}

        //    //var result = new StudentFormSubmissionResult { FormId = form.FormId, StudentId = id, FileSize = form.StudentFile.Length };
        //    //return CreatedAtAction(nameof(ViewForm), new { id, form.FormId }, result);

        //    return Ok();

        //}

    }
}
