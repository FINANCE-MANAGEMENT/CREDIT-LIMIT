using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.DomainObject.SchemeAutomation;
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
using System.Threading.Tasks;


namespace DealerNetAPI.Areas.SchemeDFI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SchemeDFIController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly ISchemeDFIBusinessLogic _schemeDFIBusinessLogic = null;
        private readonly IWebHostEnvironment _environment;

        public SchemeDFIController(JwtSettings jwtSettings, IConfiguration configuration, ISchemeDFIBusinessLogic schemeDFIBusinessLogic,
            IWebHostEnvironment environment)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _schemeDFIBusinessLogic = schemeDFIBusinessLogic;
            _environment = environment;
        }

        /// <summary>
        /// History Scheme details search
        /// </summary>
        /// <param name="SchemeRefNo"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReadSchemeHistory(string SchemeRefNo)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadSchemeHistory(SchemeRefNo);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        /// <summary>
        /// For Step -1 of details save.
        /// </summary>
        /// <param name="schemeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveSchemeRequest([FromBody] SchemeRequest schemeRequest)
        {
            try
            {
                if (schemeRequest.ProcessType == "HISTORY_UPDATE" && string.IsNullOrEmpty(schemeRequest.SchemeRefNo))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Scheme Ref. No. can't be blank." }));
                }
                if (string.IsNullOrEmpty(schemeRequest.FundSource) || string.IsNullOrEmpty(schemeRequest.SchemeType)
                    || string.IsNullOrEmpty(Convert.ToString(schemeRequest.SchemeFromDate))
                    || string.IsNullOrEmpty(schemeRequest.SchemeName))
                {

                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                else if (schemeRequest.SchemeFromDate == DateTime.MinValue)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _schemeDFIBusinessLogic.SaveSchemeRequest(schemeRequest);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// For Step -2 of details save.
        /// </summary>
        /// <param name="schemeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveSchemeRequestDetail([FromBody] SchemeRequest schemeRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(schemeRequest.FundSource) || string.IsNullOrEmpty(schemeRequest.SchemeType)
                    || string.IsNullOrEmpty(Convert.ToString(schemeRequest.SchemeFromDate))
                    || string.IsNullOrEmpty(schemeRequest.SchemeRefNo))
                {

                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                else if (schemeRequest.SchemeFromDate == DateTime.MinValue)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _schemeDFIBusinessLogic.SaveSchemeRequestDetail(schemeRequest);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Scheme Period Change , After Finally Approved Scheme.
        /// </summary>
        /// <param name="schemeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SchemeUpdateAfterApproved([FromBody] SchemeRequest schemeRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(schemeRequest.SchemeRefNo))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                else if (schemeRequest.SchemeToDate == DateTime.MinValue)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _schemeDFIBusinessLogic.SchemeUpdateAfterApproved(schemeRequest);
                if (result.Status == Utilities.ERROR)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest), result));
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateApprovalStatus([FromBody] Approval approval)
        {
            try
            {
                if (string.IsNullOrEmpty(approval.SchemeRefNo))
                {

                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _schemeDFIBusinessLogic.UpdateApprovalStatus(approval);

                // Excel data send to RPA.
                if (result.Status == Utilities.SUCCESS)
                {
                    //SchemeDetailSendToRPA(approval);
                }
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReadSchemeRequest([FromBody] SchemeRequest schemeRequest)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadSchemeRequest(schemeRequest);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Read scheme customer details like:- Zone | Region | Branch | Major Sales Channel | Sales Channel | Channel | Billing Code | Account Name 
        /// </summary>
        /// <param name="SchemeRefNo"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReadSchemeCustomerDetail(string SchemeRefNo)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadSchemeCustomerDetail(SchemeRefNo);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        // Read all product according to Scheme No. wise in Tabular Format.
        [HttpGet]
        public async Task<IActionResult> ReadSchemeProductDetail(string SchemeRefNo)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadSchemeProductDetail(SchemeRefNo);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadApprovalScheme(string UserId)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadApprovalScheme(UserId);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllApproverByScheme(string SchemeRefNo)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadAllApproverByScheme(SchemeRefNo);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadApprovalHistByScheme(string SchemeRefNo)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadApprovalHistByScheme(SchemeRefNo);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadTnCByScheme(string SchemeRefNo)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadTnCByScheme(SchemeRefNo);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Read Scheme Slabs
        /// </summary>
        /// <param name="SchemeRefNo"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReadSchemeSlab(string SchemeRefNo)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadSchemeSlab(SchemeRefNo);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        #region Scheme Calculation & Settlement

        /// <summary>
        /// Scheme Calculation Requset Submit
        /// </summary>
        /// <param name="schemeSettlement"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SchemeCalculationRequest([FromBody] SchemeSettlementDOM schemeSettlement)
        {
            try
            {
                if (string.IsNullOrEmpty(schemeSettlement.SchemeRefNo))
                {

                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                APIResponse result = await _schemeDFIBusinessLogic.SchemeCalculationRequest(schemeSettlement);

                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Scheme Calculation Requests Read
        /// </summary>
        /// <param name="schemeSettlement"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SchemeCalculationRequestRead([FromBody] SchemeSettlementDOM schemeSettlement)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.SchemeCalculationRequestRead(schemeSettlement);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Scheme Calculation data read
        /// </summary>
        /// <param name="schemeSettlement"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SchemeCalculationRead([FromBody] SchemeSettlementDOM schemeSettlement)
        {
            try
            {
                if (string.IsNullOrEmpty(schemeSettlement.SchemeRefNo))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                var data = await _schemeDFIBusinessLogic.SchemeCalculationRead(schemeSettlement);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        /// <summary>
        /// Scheme Calculation of Base data read (GTM Scheme Interface data)
        /// </summary>
        /// <param name="SchemeRefNo"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SchemeCalculationBaseDataRead(string SchemeRefNo)
        {
            try
            {
                if (string.IsNullOrEmpty(SchemeRefNo))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }
                var data = await _schemeDFIBusinessLogic.GTMSchemeDataRead(SchemeRefNo);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        #endregion

        #region Serial No. Applicability

        /// <summary>
        /// Serial No. applicability registration
        /// </summary>
        /// <param name="serailNoApplicability"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SerialNoApplicability([FromBody] SerailNoApplicability serailNoApplicability)
        {
            try
            {
                if (string.IsNullOrEmpty(serailNoApplicability.HeaderCode.Trim()) || string.IsNullOrEmpty(serailNoApplicability.Status.Trim()))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                if (serailNoApplicability.Status.ToUpper() != "Y" && serailNoApplicability.Status.ToUpper() != "N")
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Invalid Status." }));
                }

                APIResponse result = await _schemeDFIBusinessLogic.SerialNoApplicability(serailNoApplicability);

                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SerialNoApplicabilityRead([FromBody] SerailNoApplicability serailNoApplicability)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.SerialNoApplicabilityRead(serailNoApplicability);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }


        #endregion

        #region Scheme Target Registration

        /// <summary>
        /// Scheme Target Upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SchemeTargetUpload()
        {
            try
            {
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());
                string schemeRefNo = Convert.ToString(Request.Form["SchemeRefNo"]).Trim();
                //string schemeTargetType = Convert.ToString(Request.Form["TargetType"]).Trim();
                string schemeTargetBased = Convert.ToString(Request.Form["TargetBased"]).Trim();

                if (string.IsNullOrEmpty(schemeRefNo))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Scheme Ref. No. Missing." }));
                }
                //if (string.IsNullOrEmpty(schemeTargetType))
                //{
                //    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                //        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Pelase select Target Type." }));
                //}
                //else
                //{
                //    if (schemeTargetType != "AMT" && schemeTargetType != "QTY")
                //    {
                //        return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                //        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Target Type Invalid!!" }));
                //    }
                //}

                if (string.IsNullOrEmpty(schemeTargetBased))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Pelase select Target Based On." }));
                }
                else
                {
                    if (schemeTargetBased != "Sell-In" && schemeTargetBased != "Sell-Out")
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = "Target Based On Invalid!!" }));
                    }
                }

                //Create the Directory.
                string path = Path.Combine(this._environment.WebRootPath, _configuration["SchemeAutomation_UploadPath"]);
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
                DataTable dtTarget = Utilities.MapExcelToDictionary(filePath);
                if (dtTarget.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<Slab_TargetDOM> lstTarget = new List<Slab_TargetDOM>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;
                // Check Excel Columns Name
                if (!dtTarget.Columns[0].ColumnName.Equals("Billing Code"))
                {
                    excelHeaderCheck++;
                }
                if (!dtTarget.Columns[1].ColumnName.Equals("Target"))
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
                for (int i = 0; i < dtTarget.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string billingCode = Convert.ToString(dtTarget.Rows[i]["Billing Code"]).Trim();
                    string targetValue = Convert.ToString(dtTarget.Rows[i]["Target"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(billingCode))
                    {
                        _errorMsg += "Billing Code can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(targetValue))
                    {
                        _errorMsg += "Target can't be blank";
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    else
                    {
                        Slab_TargetDOM target = new Slab_TargetDOM();
                        target.SchemeRefNo = schemeRefNo;
                        target.BillingCode = billingCode;
                        //target.TargetType = schemeTargetType;
                        target.TargetBased = schemeTargetBased;
                        target.Target = targetValue;
                        target.CreatedBy = createdBy;
                        target.RowNumber = (i + 2).ToString();
                        lstTarget.Add(target);
                    }
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _schemeDFIBusinessLogic.SchemeTargetUpload(lstTarget);
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
        /// Scheme Target Read
        /// </summary>
        /// <param name="SchemeRefNo"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReadSchemeTarget(string SchemeRefNo)
        {
            try
            {
                var data = await _schemeDFIBusinessLogic.ReadSchemeTarget(SchemeRefNo);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        #endregion


        private void SchemeDetailSendToRPA(Approval approval)
        {
            const string SCHEME_TYPE_SellIn_DFI = "Sell-In(DFI)";
            const string SCHEME_TYPE_SellIn_AAI = "Sell-In(AAI)";

            string schemeType = "Sell-In(AAI)"; // Scheme Type 
            List<RPAExcel> lstRPAData = new List<RPAExcel>(); // Load from DB

            //Create the Directory.
            string path = Path.Combine(this._environment.WebRootPath, "Upload\\RPAExcel\\");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = path;
            if (System.IO.File.Exists(filepath))
            {
                //FileInfo fl = new FileInfo(filepath);

                System.IO.File.Delete(filepath);
            }




            using (DocumentFormat.OpenXml.Packaging.SpreadsheetDocument document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Create(filepath, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                DocumentFormat.OpenXml.Packaging.WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

                DocumentFormat.OpenXml.Packaging.WorksheetPart worksheetPart = workbookPart.AddNewPart<DocumentFormat.OpenXml.Packaging.WorksheetPart>();

                worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(new DocumentFormat.OpenXml.Spreadsheet.SheetData());
                DocumentFormat.OpenXml.Spreadsheet.Sheets sheets = workbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());

                DocumentFormat.OpenXml.Spreadsheet.Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                sheets.Append(sheet);

                var sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);

                // Add header
                UInt32 rowIdex = 0;
                var row = new DocumentFormat.OpenXml.Spreadsheet.Row { RowIndex = ++rowIdex };
                sheetData.AppendChild(row);
                var cellIdex = 0;

                List<string> RPAHeaders = new List<string>();

                #region RPA Excel Headers Columns name

                if (schemeType == SCHEME_TYPE_SellIn_DFI)
                {
                    #region Sell-In(DFI) excel header
                    RPAHeaders.Add("Sales PGM No");
                    RPAHeaders.Add("Sales PGM Name");
                    RPAHeaders.Add("Sales PGM Type");
                    RPAHeaders.Add("Apply Date(From)");
                    RPAHeaders.Add("Apply Date(To)");
                    RPAHeaders.Add("Original Start Date");
                    RPAHeaders.Add("Sales PGM Reason");
                    RPAHeaders.Add("Currency");
                    RPAHeaders.Add("Base/Cascade");
                    RPAHeaders.Add("Description");
                    RPAHeaders.Add("Customer Type");
                    RPAHeaders.Add("Customer Code");
                    RPAHeaders.Add("Customer Name");
                    RPAHeaders.Add("Division");
                    RPAHeaders.Add("Prod Level1");
                    RPAHeaders.Add("Prod Level2");
                    RPAHeaders.Add("Prod Level3");
                    RPAHeaders.Add("Prod Level4");
                    RPAHeaders.Add("PTO Model");
                    RPAHeaders.Add("PTO Component");
                    RPAHeaders.Add("Model Category");
                    RPAHeaders.Add("Model");
                    RPAHeaders.Add("CYON Model");
                    RPAHeaders.Add("Calculation Method");
                    RPAHeaders.Add("Percent");
                    RPAHeaders.Add("Amount");
                    RPAHeaders.Add("Minimum Volume Type");
                    RPAHeaders.Add("Minimum Qty");
                    RPAHeaders.Add("Minimum Amount");
                    RPAHeaders.Add("Reverse DFI Invoice");
                    RPAHeaders.Add("Registration Req Date");
                    RPAHeaders.Add("Sales PGM Class");
                    RPAHeaders.Add("Property Type");
                    RPAHeaders.Add("SPGM Tool Use Flag");
                    RPAHeaders.Add("BranchAU");
                    RPAHeaders.Add("Model Prefix");
                    RPAHeaders.Add("Bill To Exclude Flag");
                    RPAHeaders.Add("Model/Prefix Exclude Flag");
                    RPAHeaders.Add("Promotion Name");
                    RPAHeaders.Add("Budget AU");
                    RPAHeaders.Add("Project Code");
                    RPAHeaders.Add("Subsidiary Dff1");
                    RPAHeaders.Add("Subsidiary Dff2");
                    RPAHeaders.Add("Subsidiary Dff3");
                    RPAHeaders.Add("Subsidiary Dff4");
                    RPAHeaders.Add("Subsidiary Dff5");
                    RPAHeaders.Add("Subsidiary Dff6");
                    RPAHeaders.Add("Expected Qty");
                    RPAHeaders.Add("Expected Gross Sales");
                    RPAHeaders.Add("Requestor");
                    RPAHeaders.Add("Original Promotion No");
                    RPAHeaders.Add("KAM Code");
                    RPAHeaders.Add("Model Exclude");
                    RPAHeaders.Add("Subsidiary Dff7");
                    RPAHeaders.Add("Subsidiary Dff8");
                    RPAHeaders.Add("Subsidiary Dff9");
                    RPAHeaders.Add("Portal Promotion ID");
                    RPAHeaders.Add("Portal Promotion Desc");
                    RPAHeaders.Add("Promotion Start Date");
                    RPAHeaders.Add("Promotion End Date");
                    RPAHeaders.Add("Promotion Line Remark");
                    #endregion
                }
                else if (schemeType == SCHEME_TYPE_SellIn_AAI)
                {
                    #region Sell-In(AAI) excel header

                    RPAHeaders.Add("Sales PGM No");
                    RPAHeaders.Add("Sales PGM Name");
                    RPAHeaders.Add("Sales PGM Type");
                    RPAHeaders.Add("Apply Date(From)");
                    RPAHeaders.Add("Apply Date(To)");
                    RPAHeaders.Add("Sales PGM Reason");
                    RPAHeaders.Add("Including Tax");
                    RPAHeaders.Add("Base Price for Accrual");
                    RPAHeaders.Add("Expense To");
                    RPAHeaders.Add("Accounting Unit");
                    RPAHeaders.Add("Department");
                    RPAHeaders.Add("Currency");
                    RPAHeaders.Add("Autopay");
                    RPAHeaders.Add("Period for Autopay");
                    RPAHeaders.Add("Data Source Code");
                    RPAHeaders.Add("Estimate Sales PGM No");
                    RPAHeaders.Add("Description");
                    RPAHeaders.Add("Customer Type");
                    RPAHeaders.Add("Customer Code");
                    RPAHeaders.Add("Customer Name");
                    RPAHeaders.Add("Division");
                    RPAHeaders.Add("Prod Level 1");
                    RPAHeaders.Add("Prod Level 2");
                    RPAHeaders.Add("Prod Level 3");
                    RPAHeaders.Add("Prod Level 4");
                    RPAHeaders.Add("PTO Model");
                    RPAHeaders.Add("PTO Component");
                    RPAHeaders.Add("Model Category");
                    RPAHeaders.Add("Model");
                    RPAHeaders.Add("CYON Model");
                    RPAHeaders.Add("Calculation Method");
                    RPAHeaders.Add("Percent");
                    RPAHeaders.Add("Amount");
                    RPAHeaders.Add("Minimum Volume Type");
                    RPAHeaders.Add("Minimum Qty");
                    RPAHeaders.Add("Minimum Amount");
                    RPAHeaders.Add("Pre Sales Pgm No");
                    RPAHeaders.Add("Registration Request Date");
                    RPAHeaders.Add("NSR FLAG");
                    RPAHeaders.Add("Sales PGM Class");
                    RPAHeaders.Add("Sales Person");
                    RPAHeaders.Add("Property Type");
                    RPAHeaders.Add("SPGM Tool Use Flag");
                    RPAHeaders.Add("BranchAU");
                    RPAHeaders.Add("Model Prefix");
                    RPAHeaders.Add("Bill To Exclude Flag");
                    RPAHeaders.Add("Model/Prefix Exclude Flag");
                    RPAHeaders.Add("Claim Tax Applicable Flag");
                    RPAHeaders.Add("Sellout Actual Price Type");
                    RPAHeaders.Add("Model Segment");
                    RPAHeaders.Add("Promotion Name");
                    RPAHeaders.Add("Budget AU");
                    RPAHeaders.Add("Project Code");
                    RPAHeaders.Add("Subsidiary Dff1");
                    RPAHeaders.Add("Subsidiary Dff2");
                    RPAHeaders.Add("Subsidiary Dff3");
                    RPAHeaders.Add("Subsidiary Dff4");
                    RPAHeaders.Add("Subsidiary Dff5");
                    RPAHeaders.Add("Subsidiary Dff6");
                    RPAHeaders.Add("Expected Qty");
                    RPAHeaders.Add("Expected Gross Sales");
                    RPAHeaders.Add("Preprogram Flag");
                    RPAHeaders.Add("Requestor");
                    RPAHeaders.Add("Original Promotion No");
                    RPAHeaders.Add("KAM CODE");
                    RPAHeaders.Add("Model Exclude");
                    RPAHeaders.Add("Subsidiary Dff7");
                    RPAHeaders.Add("Subsidiary Dff8");
                    RPAHeaders.Add("Subsidiary Dff9");
                    RPAHeaders.Add("Portal Promotion ID");
                    RPAHeaders.Add("Portal Promotion Desc");
                    RPAHeaders.Add("Promotion Start Date");
                    RPAHeaders.Add("Promotion End Date");
                    RPAHeaders.Add("Promotion Line Remark");

                    #endregion
                }

                #endregion

                // Excel Header Write
                foreach (string header in RPAHeaders)
                {
                    row.AppendChild(CreateTextCell(ColumnLetter(cellIdex++), rowIdex, header ?? string.Empty));
                }

                // Add sheet data Rows/Columns
                foreach (RPAExcel rpaData in lstRPAData)
                {
                    cellIdex = 0;
                    row = new DocumentFormat.OpenXml.Spreadsheet.Row { RowIndex = ++rowIdex };
                    sheetData.AppendChild(row);

                    string[] rptColumnData = new string[] { };

                    if (schemeType == SCHEME_TYPE_SellIn_DFI)  // Sell-In(DFI)  data
                    {
                        rptColumnData = new string[] { rpaData.SalesPGM_No, rpaData.SalesPGM_Name, rpaData.SalesPGM_Type, rpaData.ApplyDateFrom, rpaData.ApplyDateTo,
                                                   rpaData.OriginalStartDate, rpaData.SalesPGM_Reason, rpaData.Currency, rpaData.BaseCascade, rpaData.Description,
                                                   rpaData.CustomerType, rpaData.CustomerCode, rpaData.CustomerName, rpaData.Division,
                                                   rpaData.ProdLevel1, rpaData.ProdLevel2, rpaData.ProdLevel3, rpaData.ProdLevel4, rpaData.PTO_Model, rpaData.PTO_Component,
                                                   rpaData.ModelCategory, rpaData.Model, rpaData.CYON_Model, rpaData.CalculationMethod, rpaData.Percent, rpaData.Amount,
                                                   rpaData.MinimumVolumeType, rpaData.MinimumQty, rpaData.MinimumAmount, rpaData.ReverseDFI_Invoice, rpaData.RegistrationReqDate,
                                                   rpaData.SalesPGM_Class, rpaData.PropertyType, rpaData.SPGM_ToolUseFlag, rpaData.BranchAU, rpaData.ModelPrefix,
                                                   rpaData.BillToExcludeFlag, rpaData.ModelPrefixExcludeFlag, rpaData.PromotionName, rpaData.BudgetAU, rpaData.ProjectCode,
                                                   rpaData.SubsidiaryDff1, rpaData.SubsidiaryDff2, rpaData.SubsidiaryDff3, rpaData.SubsidiaryDff4, rpaData.SubsidiaryDff5,
                                                   rpaData.SubsidiaryDff6, rpaData.ExpectedQty, rpaData.ExpectedGrossSales, rpaData.Requestor, rpaData.OriginalPromotion_No,
                                                   rpaData.KAMCode, rpaData.ModelExclude, rpaData.SubsidiaryDff7, rpaData.SubsidiaryDff8, rpaData.SubsidiaryDff9,
                                                   rpaData.PortalPromotionID, rpaData.PortalPromotionDesc, rpaData.PromotionStartDate, rpaData.PromotionEndDate,
                                                   rpaData.PromotionLineRemark
                                                };
                    }
                    else if (schemeType == SCHEME_TYPE_SellIn_AAI) // Sell-In(AAI) data
                    {
                        rptColumnData = new string[] { rpaData.SalesPGM_No, rpaData.SalesPGM_Name, rpaData.SalesPGM_Type, rpaData.ApplyDateFrom, rpaData.ApplyDateTo,
                                                   rpaData.SalesPGM_Reason, rpaData.IncludingTax, rpaData.BasePriceForAccrual, rpaData.ExpenseTo, rpaData.AccountingUnit,
                                                   rpaData.Department, rpaData.Currency, rpaData.AutoPay, rpaData.PeriodForAutoPay,rpaData.DataSourceCode,
                                                   rpaData.EstimateSalesPGM_No, rpaData.Description,
                                                   rpaData.CustomerType, rpaData.CustomerCode, rpaData.CustomerName, rpaData.Division,
                                                   rpaData.ProdLevel1, rpaData.ProdLevel2, rpaData.ProdLevel3, rpaData.ProdLevel4, rpaData.PTO_Model, rpaData.PTO_Component,
                                                   rpaData.ModelCategory, rpaData.Model, rpaData.CYON_Model, rpaData.CalculationMethod, rpaData.Percent, rpaData.Amount,
                                                   rpaData.MinimumVolumeType, rpaData.MinimumQty, rpaData.MinimumAmount, rpaData.PreSalesPGM_No, rpaData.RegistrationReqDate,
                                                   rpaData.NSR_Flag, rpaData.SalesPGM_Class, rpaData.SalesPerson, rpaData.PropertyType, rpaData.SPGM_ToolUseFlag,
                                                   rpaData.BranchAU, rpaData.ModelPrefix, rpaData.BillToExcludeFlag, rpaData.ModelPrefixExcludeFlag,
                                                   rpaData.ClaimTaxApplicableFlag, rpaData.SelloutActualPriceType, rpaData.ModelSegment,
                                                   rpaData.PromotionName, rpaData.BudgetAU, rpaData.ProjectCode,
                                                   rpaData.SubsidiaryDff1, rpaData.SubsidiaryDff2, rpaData.SubsidiaryDff3, rpaData.SubsidiaryDff4, rpaData.SubsidiaryDff5,
                                                   rpaData.SubsidiaryDff6, rpaData.ExpectedQty, rpaData.ExpectedGrossSales,
                                                   rpaData.Requestor, rpaData.OriginalPromotion_No,
                                                   rpaData.KAMCode, rpaData.ModelExclude, rpaData.SubsidiaryDff7, rpaData.SubsidiaryDff8, rpaData.SubsidiaryDff9,
                                                   rpaData.PortalPromotionID, rpaData.PortalPromotionDesc, rpaData.PromotionStartDate, rpaData.PromotionEndDate,
                                                   rpaData.PromotionLineRemark
                        };
                    }

                    // excel Cell data write
                    foreach (string callData in rptColumnData)
                    {
                        var cell = CreateTextCell(ColumnLetter(cellIdex++), rowIdex, callData ?? string.Empty);
                        row.AppendChild(cell);
                    }
                }
                workbookPart.Workbook.Save();
                //document.Close();
                document.Dispose();
            }
            if (!string.IsNullOrEmpty(filepath))
            {
                EMail email = new EMail();
                email.To = "vikrant.kumar@lge.com";
                email.MailSubject = "RPA Excel";
                email.MailBody = "my test data";
                Utilities.SendingEmail(email, _configuration);
            }
        }
        private string ColumnLetter(int intCol)
        {
            var intFirstLetter = ((intCol) / 676) + 64;
            var intSecondLetter = ((intCol % 676) / 26) + 64;
            var intThirdLetter = (intCol % 26) + 65;

            var firstLetter = (intFirstLetter > 64)
                ? (char)intFirstLetter : ' ';
            var secondLetter = (intSecondLetter > 64)
                ? (char)intSecondLetter : ' ';
            var thirdLetter = (char)intThirdLetter;

            return string.Concat(firstLetter, secondLetter,
                thirdLetter).Trim();
        }
        private DocumentFormat.OpenXml.Spreadsheet.Cell CreateTextCell(string header, UInt32 index, string text)
        {
            var cell = new DocumentFormat.OpenXml.Spreadsheet.Cell
            {
                DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.InlineString,
                CellReference = header + index
            };

            var istring = new DocumentFormat.OpenXml.Spreadsheet.InlineString();
            var t = new DocumentFormat.OpenXml.Spreadsheet.Text { Text = text };
            istring.AppendChild(t);
            cell.AppendChild(istring);
            return cell;
        }




    }
}
