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
using System.Threading.Tasks;

namespace DealerNetAPI.Areas.SchemeDFI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UtilitiesDFIController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration = null;
        private readonly IUtilitiesDFIBusinessLogic _utilitiesDFIBusinessLogic = null;
        private readonly IWebHostEnvironment _environment;

        public UtilitiesDFIController(JwtSettings jwtSettings, IConfiguration configuration, IUtilitiesDFIBusinessLogic utilitiesDFIBusinessLogic,
            IWebHostEnvironment environment)
        {
            _jwtSettings = jwtSettings;
            _configuration = configuration;
            _utilitiesDFIBusinessLogic = utilitiesDFIBusinessLogic;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> ReadZone(string createdBy)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadZone(createdBy);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadRegion([FromBody] Branch branch)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadRegion(branch);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadBranch([FromBody] Branch branch)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadBranch(branch);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReadSchemeType()
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadSchemeType();
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReadSchemeReasonCode(string SchemeType,string FundSource, string RoleName)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadSchemeReasonCode(SchemeType, FundSource, RoleName);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReadMajorSalesChannel()
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadMajorSalesChannel();
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReadSalesChannel(string MajorSalesChannel)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadSalesChannel(MajorSalesChannel);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReadChannel(string MajorSalesChannel, string SalesChannel)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadChannel(MajorSalesChannel, SalesChannel);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadBillingCode([FromBody] Distributor distributor)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadBillingCode(distributor);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReadProduct()
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadProduct();
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReadSubProduct(string Product)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadSubProduct(Product);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadProductLevel3([FromBody] Model model)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadProductLevel3(model);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadModelCategory([FromBody] Model model)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadModelCategory(model);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadModelSeries([FromBody] Model model)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadModelSeries(model);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadModelSubCategory([FromBody] Model model)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadModelSubCategory(model);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadModelStarRating([FromBody] Model model)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadModelStarRating(model);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadModelYear([FromBody] Model model)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadModelYear(model);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadModel([FromBody] Model model)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadModel(model);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReadFilterProduct([FromBody] Model model)
        {
            try
            {
                var data = await _utilitiesDFIBusinessLogic.ReadFilterProduct(model);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateModels()
        {
            try
            {
                //var file = Request.Form.Files[0]; // working fine
                //var file = Request.Form["File"];
                Int32 createdBy = Convert.ToInt32(Request.Form["CreatedBy"].ToString());

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

                //File Extension check
                if (Path.GetExtension(postedFile.FileName).ToLower() != ".xlsx")
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EXTENSION_ALLOW }));
                }

                //Save the File.
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                // Read data from Excel File
                DataTable dtModels = Utilities.MapExcelToDictionary(filePath);
                if (dtModels.Rows.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_EMPTY }));
                }

                List<Model> lstModels = new List<Model>();
                List<Errors> errors = new List<Errors>();
                int excelHeaderCheck = 0;

                // Check Excel Columns Name
                if (!dtModels.Columns[0].ColumnName.Equals("Product"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[1].ColumnName.Equals("Sub Product"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[2].ColumnName.Equals("Product Level3"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[3].ColumnName.Equals("Model Category"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[4].ColumnName.Equals("Model Series"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[5].ColumnName.Equals("Model Sub Category"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[6].ColumnName.Equals("Star Rating"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[7].ColumnName.Equals("Model Year"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[8].ColumnName.Equals("ModelNo"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[9].ColumnName.Equals("Model Desc"))
                {
                    excelHeaderCheck++;
                }
                if (!dtModels.Columns[10].ColumnName.Equals("Status"))
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
                for (int i = 0; i < dtModels.Rows.Count; i++)
                {
                    string _errorMsg = string.Empty;
                    string product = Convert.ToString(dtModels.Rows[i]["Product"]).Trim();
                    string subProduct = Convert.ToString(dtModels.Rows[i]["Sub Product"]).Trim();
                    string productLevel3 = Convert.ToString(dtModels.Rows[i]["Product Level3"]).Trim();
                    string modelCategory = Convert.ToString(dtModels.Rows[i]["Model Category"]).Trim();
                    string modelSeries = Convert.ToString(dtModels.Rows[i]["Model Series"]).Trim();
                    string modelSubCategory = Convert.ToString(dtModels.Rows[i]["Model Sub Category"]).Trim();
                    string starRating = Convert.ToString(dtModels.Rows[i]["Star Rating"]).Trim();
                    string modelYear = Convert.ToString(dtModels.Rows[i]["Model Year"]).Trim();
                    string modelNo = Convert.ToString(dtModels.Rows[i]["ModelNo"]).Trim();
                    string modelDesc = Convert.ToString(dtModels.Rows[i]["Model Desc"]).Trim();
                    string status = Convert.ToString(dtModels.Rows[i]["Status"]).Trim();

                    #region Basic Validation check

                    if (string.IsNullOrEmpty(product))
                    {
                        _errorMsg += "Product can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(subProduct))
                    {
                        _errorMsg += "Sub Product can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(modelCategory))
                    {
                        _errorMsg += "Model Category can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(modelSubCategory))
                    {
                        _errorMsg += "Model Sub Category can't be blank, ";
                    }
                    if (string.IsNullOrEmpty(modelNo))
                    {
                        _errorMsg += "ModelNo can't be blank, ";
                    }
                    else
                    {
                        if (!modelNo.Contains('.'))
                        {
                            _errorMsg += "Model is Invalid, ";
                        }
                    }
                    if (string.IsNullOrEmpty(status))
                    {
                        _errorMsg += "Status can't be blank, ";
                    }
                    else
                    {
                        if (status.ToUpper() != "Y" && status.ToUpper() != "N")
                        {
                            _errorMsg += "Status is Invalid (Y/N), ";
                        }
                    }

                    #endregion

                    if (!string.IsNullOrEmpty(_errorMsg.Trim()))
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", (i + 2), ": ", _errorMsg) });
                    }
                    else
                    {
                        Model model = new Model();
                        model.Product = product;
                        model.SubProduct = subProduct;
                        model.ProductLevel3 = productLevel3;
                        model.ModelCategory = modelCategory;
                        model.ModelSeries = modelSeries;
                        model.ModelSubCategory = modelSubCategory;
                        model.StarRating = starRating;
                        model.ModelYear = modelYear;
                        model.ModelNo = modelNo;
                        model.ModelDesc = modelDesc;
                        model.Status = status;
                        model.CreatedBy = createdBy;
                        lstModels.Add(model);
                    }
                }

                if (errors.Count > 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES, data = errors }));
                }

                // Business Logic call for update in database.
                APIResponse result = await _utilitiesDFIBusinessLogic.UpdateModels(lstModels);
                return StatusCode(StatusCodes.Status200OK, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status200OK), result));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.INVALID_EXCEL_FILE }));
            }
        }

        [HttpGet]
        public IActionResult ReadAllModel()
        {
            try
            {
                var data = _utilitiesDFIBusinessLogic.ReadAllModel();
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
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
                if (users.UserId == 0 && string.IsNullOrEmpty(users.Branch.BranchCode))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));
                }

                APIResponse result = await _utilitiesDFIBusinessLogic.UserAU_Mapping(users);
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
                var data = await _utilitiesDFIBusinessLogic.ReadUserAU_Mapping(users);
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

                var data = await _utilitiesDFIBusinessLogic.ReadUsersForAUMappingByRoleId(roleId);
                return Ok(new APIResponse { Status = Utilities.SUCCESS, data = data });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                    new APIResponse { Status = Utilities.ERROR, StatusDesc = ex.ToString() }));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReadApprovers(string ApproverName)
        {
            try
            {
                // Check mandatory fields.
                if (string.IsNullOrEmpty(ApproverName))
                    return StatusCode(StatusCodes.Status400BadRequest, Utilities.GenerateResponse(Convert.ToString((int)StatusCodes.Status400BadRequest),
                        new APIResponse { Status = Utilities.ERROR, StatusDesc = Utilities.PARAMETER_MISSING }));

                var data = await _utilitiesDFIBusinessLogic.ReadApprovers(ApproverName);
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
