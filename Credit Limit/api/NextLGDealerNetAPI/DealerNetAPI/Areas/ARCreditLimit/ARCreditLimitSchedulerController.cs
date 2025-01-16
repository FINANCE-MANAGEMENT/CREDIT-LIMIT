using DealerNetAPI.BusinessLogic.Interface;
using CommonDOM = DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mail;
using PhantomJs.NetCore;
using System.Net.Http.Headers;
using System.Net.Http;
using DealerNetAPI.DomainObject.ARCreditLimit;

namespace DealerNetAPI.Areas.ARCreditLimit
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ARCreditLimitSchedulerController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IWebHostEnvironment _environment;
        private readonly IARCreditLimitBusinessLogic _arCreditLimitBusinessLogic = null;

        public ARCreditLimitSchedulerController(JwtSettings jwtSettings, IConfiguration configuration,
            IWebHostEnvironment environment, IARCreditLimitBusinessLogic arCreditLimitBusinessLogic)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _environment = environment;
            _arCreditLimitBusinessLogic = arCreditLimitBusinessLogic;
        }

        /// <summary>
        /// Auto Email sent for AR Credit LImit of Status = REQUEST
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreditLimitAutoEmailREQUEST()
        {
            try
            {
                var result = await CreditLimitAutoEmailSent("REQUEST", null);
                return Ok(result);
            }
            catch (Exception ex)
            {
                CommonDOM.Utilities.CreateLogFile(CommonDOM.Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, CommonDOM.Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse
                    {
                        Status = CommonDOM.Utilities.ERROR,
                        StatusDesc = ex.ToString()
                    }));
            }
        }

        /// <summary>
        /// Auto Email sent for AR Credit LImit of Status = PENDING, CLOSED
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreditLimitAutoEmailCLOSED()
        {
            try
            {
                var result = await CreditLimitAutoEmailSent("CLOSED", null);
                return Ok(result);
            }
            catch (Exception ex)
            {
                CommonDOM.Utilities.CreateLogFile(CommonDOM.Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, CommonDOM.Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse
                    {
                        Status = CommonDOM.Utilities.ERROR,
                        StatusDesc = ex.ToString()
                    }));
            }
        }

        /// <summary>
        /// Auto Email sent for AR Credit Limit
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreditLimitAutoEmailSent(string status, long? RequestId)
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
                catch (Exception ex)
                {
                    CommonDOM.Utilities.CreateLogFile(CommonDOM.Utilities.PROJECTS.ARCreditLimit, ex.ToString());
                    return StatusCode(StatusCodes.Status400BadRequest, CommonDOM.Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse
                        {
                            Status = CommonDOM.Utilities.ERROR,
                            StatusDesc = "Credit Limit Request detail Template not available. Kindly Co-ordinate with system administrator.",
                        }));
                }

                string pathOfGeneratedPdf = string.Empty;
                string RequestTemplateData = clRequestDetail_Template;

                var folderDirectory = Path.Combine(this._environment.WebRootPath, "Downloads", "ARCreditLimit");
                if (!Directory.Exists(folderDirectory))
                {
                    Directory.CreateDirectory(folderDirectory);
                }

                // Read All Requsets, For Email Sending...
                var clAllRequest = await _arCreditLimitBusinessLogic.ReadAllRequest(new ARCreditLimitDOM()
                {
                    ReqId = RequestId,
                    EmailSent = "N",
                    Status = status
                });

                // Read Requset detail on Email
                foreach (var cl in clAllRequest)
                {
                    try
                    {
                        RequestTemplateData = clRequestDetail_Template;

                        ARCreditLimitDOM clRequest = new ARCreditLimitDOM();
                        clRequest.ReqId = cl.ReqId;
                        clRequest.Distributor.HeaderCode = cl.Distributor.HeaderCode;

                        var clRequestDetail = await _arCreditLimitBusinessLogic.ReadAllRequest(clRequest);
                        var clDetail = clRequestDetail[0];
                        if (clDetail != null)
                        {
                            string emailTo = string.Empty;
                            string emailCC = string.Empty;
                            _arCreditLimitBusinessLogic.ReadEmailForCLRequest(clDetail.ReqId.Value, clDetail.Distributor.HeaderCode, out emailTo, out emailCC);

                            if (!string.IsNullOrEmpty(emailTo))
                            {
                                #region Email HTML Body

                                RequestTemplateData = RequestTemplateData.Replace("{{HEADER_CODE}}", clDetail.Distributor.HeaderCode);
                                RequestTemplateData = RequestTemplateData.Replace("{{CTRL_AU}}", clDetail.Distributor.CTRL_AU);
                                RequestTemplateData = RequestTemplateData.Replace("{{ACCOUNT_NAME}}", clDetail.Distributor.AccountName);
                                RequestTemplateData = RequestTemplateData.Replace("{{INSURANCE_CODE}}", clDetail.Insurance.InsuranceCode);
                                RequestTemplateData = RequestTemplateData.Replace("{{INSURANCE_NAME}}", clDetail.Insurance.InsuranceName);
                                RequestTemplateData = RequestTemplateData.Replace("{{CREATED_DATE}}", clDetail.CreatedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss tt"));
                                RequestTemplateData = RequestTemplateData.Replace("{{CREATED_BY_NAME}}", clDetail.CreatedByName);
                                RequestTemplateData = RequestTemplateData.Replace("{{STATUS}}", clDetail.Status);
                                RequestTemplateData = RequestTemplateData.Replace("{{ADMIN_REMARKS}}", clDetail.AdminRemarks);
                                RequestTemplateData = RequestTemplateData.Replace("{{CURRENT_CREDIT_LIMIT_AMOUNT}}", CommonDOM.Utilities.AmountFormatWithoutDecimal(clDetail.Insurance.CurrentCreditLimitAmount));
                                RequestTemplateData = RequestTemplateData.Replace("{{CURRENT_CREDIT_LIMIT_ENDDATE}}", clDetail.Insurance.CurrentCreditLimitEndDate == null ? "" : Convert.ToDateTime(clDetail.Insurance.CurrentCreditLimitEndDate).ToString("dd-MMM-yyyy"));
                                RequestTemplateData = RequestTemplateData.Replace("{{CREDIT_LIMIT_REQUEST_AMOUNT}}", CommonDOM.Utilities.AmountFormatWithoutDecimal(clDetail.Insurance.CreditLimitRequestAmount));

                                // Sales History
                                StringBuilder sbSalesHistory = new StringBuilder();
                                foreach (var salesHis in clDetail.SalesHistory)
                                {
                                    sbSalesHistory.Append("<tr>");
                                    sbSalesHistory.Append("<th style='width: 140px!important;'>" + salesHis.DataSource + "</th>");
                                    sbSalesHistory.Append("<td style='text-align: right;'>" + CommonDOM.Utilities.AmountFormatWithoutDecimal(salesHis.SaleAmount) + "</td>");
                                    sbSalesHistory.Append("<td style='text-align: right;'>" + CommonDOM.Utilities.AmountFormatWithoutDecimal(salesHis.AvgSaleAmount) + "</td>");
                                    sbSalesHistory.Append("<td style='text-align: right;'>" + CommonDOM.Utilities.AmountFormatWithoutDecimal(salesHis.MaxSaleAmount) + "</td>");
                                    sbSalesHistory.Append("<td style='text-align: center;'>" + salesHis.MaxSaleMonthYear + "</td>");
                                    sbSalesHistory.Append("</tr>");
                                }
                                RequestTemplateData = RequestTemplateData.Replace("{{SALES_HISTORY}}", sbSalesHistory.ToString());

                                // Last Six Month Sales and Payment Trend
                                StringBuilder sbSalesPaymentTrend_MonthYear = new StringBuilder();
                                StringBuilder sbSalesPaymentTrend_SaleAmount = new StringBuilder();
                                StringBuilder sbSalesPaymentTrend_CollectionAmount = new StringBuilder();
                                foreach (var salesPayment in clDetail.Sales_PaymentTrend)
                                {
                                    sbSalesPaymentTrend_MonthYear.Append("<th>" + salesPayment.MonthYear + "</th>");
                                    sbSalesPaymentTrend_SaleAmount.Append("<td style='text-align: right;'>" + CommonDOM.Utilities.AmountFormatWithoutDecimal(salesPayment.SaleAmount) + "</td>");
                                    sbSalesPaymentTrend_CollectionAmount.Append("<td style='text-align: right;'>" + CommonDOM.Utilities.AmountFormatWithoutDecimal(salesPayment.CollectionAmount) + "</td>");
                                }
                                RequestTemplateData = RequestTemplateData.Replace("{{SALES_PAYMENT_TREND_MONTHYEAR}}", sbSalesPaymentTrend_MonthYear.ToString());
                                RequestTemplateData = RequestTemplateData.Replace("{{SALES_PAYMENT_TREND_SALE_AMOUNT}}", sbSalesPaymentTrend_SaleAmount.ToString());
                                RequestTemplateData = RequestTemplateData.Replace("{{SALES_PAYMENT_TREND_COLLECTION_AMOUNT}}", sbSalesPaymentTrend_CollectionAmount.ToString());

                                // OD History
                                RequestTemplateData = RequestTemplateData.Replace("{{OD_HISTORY_TOTAL_OD}}", clDetail.OD_History.TotalOD.ToString());
                                RequestTemplateData = RequestTemplateData.Replace("{{OD_HISTORY_MAX_OD}}", CommonDOM.Utilities.AmountFormatWithoutDecimal(clDetail.OD_History.MaxOD));
                                RequestTemplateData = RequestTemplateData.Replace("{{OD_HISTORY_AVERAGE_OD}}", CommonDOM.Utilities.AmountFormatWithoutDecimal(clDetail.OD_History.AverageOD));

                                // Futue Sales Plan
                                StringBuilder sbFutureSalesPlan_MonthYear = new StringBuilder();
                                StringBuilder sbFutureSalesPlan_SaleAmount = new StringBuilder();
                                foreach (var salesPayment in clDetail.FutureSalesPlan)
                                {
                                    sbFutureSalesPlan_MonthYear.Append("<th>" + salesPayment.MonthYear + "</th>");
                                    sbFutureSalesPlan_SaleAmount.Append("<td style='text-align: right;'>" + CommonDOM.Utilities.AmountFormatWithoutDecimal(salesPayment.SaleAmount) + "</td>");
                                }
                                RequestTemplateData = RequestTemplateData.Replace("{{FUTURE_SALES_PLAN_MONTHYEAR}}", sbFutureSalesPlan_MonthYear.ToString());
                                RequestTemplateData = RequestTemplateData.Replace("{{FUTURE_SALES_PLAN_SALE_AMOUNT}}", sbFutureSalesPlan_SaleAmount.ToString());

                                //Remarks
                                RequestTemplateData = RequestTemplateData.Replace("{{BRANCH_REMARKS}}", clDetail.Remarks);

                                // Financial Statements
                                StringBuilder sbFinancialStatemet = new StringBuilder();
                                foreach (var fs in clDetail.FYAttachment)
                                {
                                    sbFinancialStatemet.Append("<tr><td>FY " + fs.FinancialYear + "</td>" + "<td>" +
                                        (string.IsNullOrEmpty(fs.FileNamePath) ? "N/A" : "Available") + "</td></tr>");
                                }
                                RequestTemplateData = RequestTemplateData.Replace("{{FINANCIAL_STATEMENTS}}", sbFinancialStatemet.ToString());

                                // Notes
                                StringBuilder sbNotes = new StringBuilder();
                                int noteCount = 1;
                                foreach (var note in clDetail.Notes)
                                {
                                    sbNotes.Append("<tr><td>" + noteCount + "</td>" + "<td>" + note.Note + "</td></tr>");
                                    noteCount++;
                                }
                                RequestTemplateData = RequestTemplateData.Replace("{{NOTES}}", sbNotes.ToString());

                                #endregion

                                #region PDF Generate Logic

                                pathOfGeneratedPdf = HtmlToPDF(RequestTemplateData, folderDirectory, clDetail);

                                #endregion

                                #region Email send Credit Limit Request Details
                                if (!string.IsNullOrEmpty(emailTo) && !string.IsNullOrEmpty(pathOfGeneratedPdf))
                                {
                                    try
                                    {
                                        EMail email = new EMail();
                                        email.EmailSupport = "Trade.econnect@lge.com";
                                        email.To = emailTo;
                                        email.CC = emailCC;
                                        email.MailSubject = string.Concat(clDetail.Status, ": Insurance Credit Limit ", clDetail.Distributor.HeaderCode, " / ", clDetail.Distributor.AccountName);
                                        email.MailBody = RequestTemplateData.ToString();
                                        if (!string.IsNullOrEmpty(pathOfGeneratedPdf))
                                        {
                                            email.Attachments.Add(pathOfGeneratedPdf);
                                        }
                                        foreach (var fa in clDetail.FYAttachment.Where(m => !string.IsNullOrEmpty(m.FileNamePath)))
                                        {
                                            email.Attachments.Add(Path.Combine(this._environment.WebRootPath, fa.FileNamePath));
                                        }
                                        CommonDOM.Utilities.SendingEmail(email, _configuration);
                                    }
                                    catch (Exception ex)
                                    {
                                        CommonDOM.Utilities.CreateLogFile(CommonDOM.Utilities.PROJECTS.ARCreditLimit,
                                            clDetail.Distributor.HeaderCode + "_" + clDetail.ReqId + "#" + ex);
                                    }
                                }
                                #endregion

                                #region Email sent Log & Status Update
                                var data = await _arCreditLimitBusinessLogic.EmailSendLog(clDetail.ReqId.Value, clDetail.Distributor.HeaderCode, emailTo, emailCC);
                                #endregion
                            }
                            else
                            {
                                CommonDOM.Utilities.CreateLogFile(CommonDOM.Utilities.PROJECTS.ARCreditLimit, "Unable to send Auto Email of Credit Limit Requset Details. Beacuse Email details not found.# ReqId=" + clDetail.ReqId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonDOM.Utilities.CreateLogFile(CommonDOM.Utilities.PROJECTS.ARCreditLimit, ex);
                    }
                } // end Loop
                return StatusCode(StatusCodes.Status200OK, CommonDOM.Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), new APIResponse
                {
                    Status = CommonDOM.Utilities.SUCCESS,
                    StatusDesc = "Auto Email Sent Successfully..."
                }));
            }
            catch (Exception ex)
            {
                CommonDOM.Utilities.CreateLogFile(CommonDOM.Utilities.PROJECTS.ARCreditLimit, ex);
                return StatusCode(StatusCodes.Status400BadRequest, CommonDOM.Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = CommonDOM.Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        private string HtmlToPDF(string pdfContent, string folderDirectory, ARCreditLimitDOM creditLimitDOM)
        {
            var generator = new PdfGenerator();

            // Generate pdf from html and place in the current folder.
            string pathOfGeneratedPdf = generator.GeneratePdf(pdfContent, folderDirectory, creditLimitDOM.Distributor.HeaderCode + "_" + creditLimitDOM.ReqId + "_" + Path.GetRandomFileName());
            return pathOfGeneratedPdf;
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
