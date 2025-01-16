using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using DealerNetAPI.ResourceAccess.Interface;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
using DealerNetAPI.DomainObject.SchemeAutomation;

namespace DealerNetAPI.ResourceAccess.SchemeDFI
{
    public class SchemeDFIAccess : ISchemeDFIAccess
    {
        private readonly ICommonDB _commonDB = null;
        private readonly string DealerNet_Next_Connection = "DealerNetNextConnection";

        public SchemeDFIAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        /// <summary>
        /// History Scheme details search
        /// </summary>
        /// <param name="SchemeRefNo"></param>
        /// <returns></returns>
        public async Task<APIResponse> ReadSchemeHistory(string SchemeRefNo)
        {
            APIResponse response = new APIResponse();
            try
            {
                SchemeRequest schemeDetail = null;
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = SchemeRefNo;
                arrParams[1] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[1].Direction = ParameterDirection.Output;
                arrParams[2] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[2].Direction = ParameterDirection.Output;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_HISTORY_READ, arrParams);

                foreach (DataRow row in dtData.Rows)
                {
                    schemeDetail = new SchemeRequest();
                    schemeDetail.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_NO"]);
                    schemeDetail.SchemeName = SafeTypeHandling.ConvertToString(row["SCHEME_NAME"]);
                    schemeDetail.SchemeType = SafeTypeHandling.ConvertToString(row["SCHEME_TYPE"]);
                    schemeDetail.FundSource = SafeTypeHandling.ConvertToString(row["FUND_SOURCE"]);
                    schemeDetail.SchemeFromDate = SafeTypeHandling.ConvertToDateTime(row["APPLY_DATE_FROM"]);
                    schemeDetail.SchemeToDate = SafeTypeHandling.ConvertToDateTime(row["APPLY_DATE_TO"]);
                    schemeDetail.ProcessType = SafeTypeHandling.ConvertToString(row["PROCESS_TYPE"]);
                }
                response = new APIResponse
                {
                    Status = arrParams[1].Value.ToString(),
                    StatusDesc = arrParams[2].Value.ToString(),
                    data = schemeDetail
                };
            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = Utilities.SOMETHING_WENT_WRONG
                };
            }
            return response;
        }

        /// <summary>
        /// Save/Update scheme
        /// </summary>
        /// <param name="schemeRequest"></param>
        /// <returns></returns>
        public async Task<APIResponse> SaveSchemeRequest(SchemeRequest schemeRequest)
        {
            int exceptionCounter = 0;
            string schemeRefNo = schemeRequest.SchemeRefNo;
            APIResponse response = null;
            try
            {
                // Scheme Primary Details Save/ Upate.
                OracleParameter[] arrParams = new OracleParameter[13];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                arrParams[0].Value = schemeRequest.SchemeRefNo;
                arrParams[1] = new OracleParameter("P_FUND_SOURCE", OracleDbType.Varchar2);
                arrParams[1].Value = schemeRequest.FundSource;
                arrParams[2] = new OracleParameter("P_SCHEME_TYPE", OracleDbType.Varchar2);
                arrParams[2].Value = schemeRequest.SchemeType;
                arrParams[3] = new OracleParameter("P_SCHEME_REASON_CODE", OracleDbType.Varchar2);
                arrParams[3].Value = schemeRequest.SchemeReasonCode;
                arrParams[4] = new OracleParameter("P_PAYOUT_TYPE", OracleDbType.Varchar2);
                arrParams[4].Value = schemeRequest.PayoutType;
                arrParams[5] = new OracleParameter("P_FROM_DATE", OracleDbType.Date);
                arrParams[5].Value = schemeRequest.SchemeFromDate;
                arrParams[6] = new OracleParameter("P_TO_DATE", OracleDbType.Date);
                arrParams[6].Value = schemeRequest.SchemeToDate;
                arrParams[7] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[7].Value = schemeRequest.CreatedBy;
                arrParams[8] = new OracleParameter("P_OUT_SCHEME_REF_NO", OracleDbType.Varchar2, 500);
                arrParams[8].Direction = ParameterDirection.Output;
                arrParams[9] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[9].Direction = ParameterDirection.Output;
                arrParams[10] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[10].Direction = ParameterDirection.Output;
                arrParams[11] = new OracleParameter("P_SCHEME_NAME", OracleDbType.Varchar2, 1000);
                arrParams[11].Value = schemeRequest.SchemeName;
                arrParams[12] = new OracleParameter("P_PROCESS_TYPE", OracleDbType.Varchar2, 100);
                arrParams[12].Value = schemeRequest.ProcessType;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.INSERT_UPDATE_REQUEST, arrParams);
                if (arrParams[9].Value.ToString() == Utilities.ERROR)
                {
                    response = new APIResponse
                    {
                        Status = arrParams[9].Value.ToString(),
                        StatusDesc = arrParams[10].Value.ToString(),
                        data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new
                        {
                            SchemeRefNo = arrParams[8].Value.ToString()
                        })),
                    };
                }
                else
                {
                    if (string.IsNullOrEmpty(schemeRequest.SchemeRefNo))
                    {
                        schemeRequest.SchemeRefNo = arrParams[8].Value.ToString();
                    }

                    #region Customer details Save/ Update ,, not in use

                    // Commented by Vikrant Kumar, 28-Feb-2024
                    //try
                    //{
                    //    OracleParameter[] arrParamsBillingCode = new OracleParameter[10];
                    //    arrParamsBillingCode[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                    //    arrParamsBillingCode[0].Value = schemeRequest.SchemeRefNo;
                    //    arrParamsBillingCode[1] = new OracleParameter("P_ZONE", OracleDbType.Varchar2, 500);
                    //    arrParamsBillingCode[1].Value = schemeRequest.Zone;
                    //    arrParamsBillingCode[2] = new OracleParameter("P_REGION", OracleDbType.Varchar2, 500);
                    //    arrParamsBillingCode[2].Value = schemeRequest.Region;
                    //    arrParamsBillingCode[3] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2, 500);
                    //    arrParamsBillingCode[3].Value = schemeRequest.Branch;
                    //    arrParamsBillingCode[4] = new OracleParameter("P_MAJOR_SALES_CHANNEL", OracleDbType.Varchar2, 500);
                    //    arrParamsBillingCode[4].Value = schemeRequest.MajorSalesChannel;
                    //    arrParamsBillingCode[5] = new OracleParameter("P_SALES_CHANNEL", OracleDbType.Varchar2, 500);
                    //    arrParamsBillingCode[5].Value = schemeRequest.SalesChannel;
                    //    arrParamsBillingCode[6] = new OracleParameter("P_CHANNEL", OracleDbType.Varchar2, 500);
                    //    arrParamsBillingCode[6].Value = schemeRequest.Channel;
                    //    arrParamsBillingCode[7] = new OracleParameter("P_BILLING_CODE", OracleDbType.Varchar2, 4000);
                    //    arrParamsBillingCode[7].Value = schemeRequest.BillingCode;
                    //    arrParamsBillingCode[8] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                    //    arrParamsBillingCode[8].Value = schemeRequest.CreatedBy;
                    //    arrParamsBillingCode[9] = new OracleParameter("P_OUTPUT", OracleDbType.Varchar2, 500);
                    //    arrParamsBillingCode[9].Direction = ParameterDirection.Output;

                    //    DataTable dtBillingCode = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.INSERT_UPDATE_REQUEST_CUSTOMER_DETAILS, arrParamsBillingCode);
                    //    string customerStatus = arrParamsBillingCode[9].Value.ToString();
                    //}
                    //catch (Exception)
                    //{
                    //    exceptionCounter++;
                    //    throw;
                    //}

                    #endregion

                    #region BillingCode or Channel details Save/Update

                    int billingCodeRowCount = 1;
                    foreach (var customer in schemeRequest.BillingCodeDetailList)
                    {
                        try
                        {
                            OracleParameter[] arrParamsBillingCode = new OracleParameter[12];
                            arrParamsBillingCode[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                            arrParamsBillingCode[0].Value = schemeRequest.SchemeRefNo;
                            arrParamsBillingCode[1] = new OracleParameter("P_ZONE", OracleDbType.Varchar2, 500);
                            arrParamsBillingCode[1].Value = customer.Zone;
                            arrParamsBillingCode[2] = new OracleParameter("P_REGION", OracleDbType.Varchar2, 500);
                            arrParamsBillingCode[2].Value = customer.Region;
                            arrParamsBillingCode[3] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2, 500);
                            arrParamsBillingCode[3].Value = customer.Branch;
                            arrParamsBillingCode[4] = new OracleParameter("P_MAJOR_SALES_CHANNEL", OracleDbType.Varchar2, 500);
                            arrParamsBillingCode[4].Value = customer.MajorSalesChannel;
                            arrParamsBillingCode[5] = new OracleParameter("P_SALES_CHANNEL", OracleDbType.Varchar2, 500);
                            arrParamsBillingCode[5].Value = customer.SalesChannel;
                            arrParamsBillingCode[6] = new OracleParameter("P_CHANNEL", OracleDbType.Varchar2, 500);
                            arrParamsBillingCode[6].Value = customer.ChannelCode;
                            arrParamsBillingCode[7] = new OracleParameter("P_BILLING_CODE", OracleDbType.Varchar2, 500);
                            arrParamsBillingCode[7].Value = customer.BillingCode;
                            arrParamsBillingCode[8] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                            arrParamsBillingCode[8].Value = schemeRequest.CreatedBy;
                            arrParamsBillingCode[9] = new OracleParameter("P_ROW_NO", OracleDbType.Varchar2, 250);
                            arrParamsBillingCode[9].Value = billingCodeRowCount;
                            arrParamsBillingCode[10] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 250);
                            arrParamsBillingCode[10].Direction = ParameterDirection.Output;
                            arrParamsBillingCode[11] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                            arrParamsBillingCode[11].Direction = ParameterDirection.Output;

                            DataTable dtProduct = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.INSERT_UPDATE_REQUEST_CUSTOMER_DETAILS, arrParamsBillingCode);
                            string customerStatus = arrParamsBillingCode[11].Value.ToString();
                            billingCodeRowCount++;
                        }
                        catch (Exception)
                        {
                            exceptionCounter++;
                            throw;
                        }
                    }


                    #endregion

                    #region Product details Save/Update

                    int prodRowCount = 1;
                    foreach (var product in schemeRequest.ProductDetailList)
                    {
                        try
                        {
                            OracleParameter[] arrParamsProduct = new OracleParameter[14];
                            arrParamsProduct[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 250);
                            arrParamsProduct[0].Value = schemeRequest.SchemeRefNo;
                            arrParamsProduct[1] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2, 250);
                            arrParamsProduct[1].Value = product.Product;
                            arrParamsProduct[2] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2, 250);
                            arrParamsProduct[2].Value = product.SubProduct;
                            arrParamsProduct[3] = new OracleParameter("P_PRODUCT_LEVEL3", OracleDbType.Varchar2, 250);
                            arrParamsProduct[3].Value = product.ProductLevel3;
                            arrParamsProduct[4] = new OracleParameter("P_MODEL_CATEGORY", OracleDbType.Varchar2, 250);
                            arrParamsProduct[4].Value = product.ModelCategory;
                            arrParamsProduct[5] = new OracleParameter("P_MODEL_SERIES", OracleDbType.Varchar2, 250);
                            arrParamsProduct[5].Value = product.ModelSeries;
                            arrParamsProduct[6] = new OracleParameter("P_MODEL_SUB_CATEGORY", OracleDbType.Varchar2, 250);
                            arrParamsProduct[6].Value = product.ModelSubCategory;
                            arrParamsProduct[7] = new OracleParameter("P_STAR_RATING", OracleDbType.Varchar2, 250);
                            arrParamsProduct[7].Value = product.StarRating;
                            arrParamsProduct[8] = new OracleParameter("P_MODEL_YEAR", OracleDbType.Varchar2, 250);
                            arrParamsProduct[8].Value = product.ModelYear;
                            arrParamsProduct[9] = new OracleParameter("P_MODEL_NO", OracleDbType.Varchar2, 250);
                            arrParamsProduct[9].Value = product.ModelNo;
                            arrParamsProduct[10] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 250);
                            arrParamsProduct[10].Value = schemeRequest.CreatedBy;
                            arrParamsProduct[11] = new OracleParameter("P_OUTPUT", OracleDbType.Varchar2, 500);
                            arrParamsProduct[11].Direction = ParameterDirection.Output;
                            arrParamsProduct[12] = new OracleParameter("P_PAYOUT_VALUE", OracleDbType.Varchar2, 250);
                            arrParamsProduct[12].Value = product.PayoutValue;
                            arrParamsProduct[13] = new OracleParameter("P_ROW_NO", OracleDbType.Varchar2, 250);
                            arrParamsProduct[13].Value = prodRowCount;

                            DataTable dtProduct = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.INSERT_UPDATE_REQUEST_PRODUCT_DETAILS, arrParamsProduct);
                            string productStatus = arrParamsProduct[11].Value.ToString();
                            prodRowCount++;
                        }
                        catch (Exception)
                        {
                            exceptionCounter++;
                            throw;
                        }
                    }


                    #endregion

                    string message = "";
                    if (exceptionCounter == 0)
                    {
                        if (!string.IsNullOrEmpty(schemeRequest.ProcessType))
                        {
                            message = "Scheme details submitted successfylly..";
                        }
                        else
                        {
                            message = string.IsNullOrEmpty(schemeRefNo) ? "Scheme approval request submitted successfully." : "Scheme approval request updated successfully.";
                        }
                    }
                    else
                    {
                        message = string.IsNullOrEmpty(schemeRefNo) ? "Scheme approval request partially submitted successfully." : "Scheme approval request partially updated successfully.";
                        message = message + " Kindly check scheme approval request details and modify.";
                    }

                    response = new APIResponse
                    {
                        Status = Utilities.SUCCESS,
                        StatusDesc = message,
                        data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new
                        {
                            SchemeRefNo = schemeRequest.SchemeRefNo
                        })),
                    };
                }
            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString() + exceptionCounter.ToString()
                };
            }
            return response;
        }

        /// <summary>
        /// Save/Update scheme details
        /// </summary>
        /// <param name="schemeRequest"></param>
        /// <returns></returns>
        public async Task<APIResponse> SaveSchemeRequestDetail(SchemeRequest schemeRequest)
        {
            APIResponse response = null;
            int approverInsertErrorCounter = 0;
            try
            {
                if (string.IsNullOrEmpty(schemeRequest.SchemeRefNo.Trim()))
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = "Scheme Ref. No. can't be blank."
                    };
                    return response;
                }

                if (schemeRequest.SchemeApproverList.Count == 0 && string.IsNullOrEmpty(schemeRequest.ProcessType))
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = "Some error occured, Approval List not found !!"
                    };
                    return response;
                }

                // Approval process work for Only New Requset Generated by Branch,,, If History Data Upload by Admin then not need to Approval Process.
                APIResponse apprResponse = null;
                if (string.IsNullOrEmpty(schemeRequest.ProcessType))
                {
                    // Previously all Approvers Removed if exists.
                    OracleParameter[] arrParamsApprRemovPrev = new OracleParameter[3];
                    arrParamsApprRemovPrev[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 250);
                    arrParamsApprRemovPrev[0].Value = schemeRequest.SchemeRefNo;
                    arrParamsApprRemovPrev[1] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParamsApprRemovPrev[1].Direction = ParameterDirection.Output;
                    arrParamsApprRemovPrev[2] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParamsApprRemovPrev[2].Direction = ParameterDirection.Output;
                    DataTable dtApprRemovDataPrev = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_APPROVERS_REMOVE, arrParamsApprRemovPrev);

                    // All Dynamically Approvers Insert
                    foreach (var approval in schemeRequest.SchemeApproverList)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(approval.SchemeRefNo))
                            {
                                apprResponse = new APIResponse
                                {
                                    Status = Utilities.ERROR,
                                    StatusDesc = "Scheme Ref. No. can't be blank for Approval List."
                                };
                                return apprResponse;
                            }

                            OracleParameter[] arrParamsAppr = new OracleParameter[8];
                            arrParamsAppr[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 250);
                            arrParamsAppr[0].Value = approval.SchemeRefNo;
                            arrParamsAppr[1] = new OracleParameter("P_APPROVER_USER_ID", OracleDbType.Varchar2, 100);
                            arrParamsAppr[1].Value = approval.ApproverUserId;
                            arrParamsAppr[2] = new OracleParameter("P_APPROVER_EMP_CODE", OracleDbType.Varchar2, 100);
                            arrParamsAppr[2].Value = approval.ApproverEmpCode;
                            arrParamsAppr[3] = new OracleParameter("P_APPROVER_SEQ", OracleDbType.Varchar2, 100);
                            arrParamsAppr[3].Value = approval.ApprovalSeq;
                            arrParamsAppr[4] = new OracleParameter("P_URL", OracleDbType.Varchar2, 500);
                            arrParamsAppr[4].Value = approval.URL;
                            arrParamsAppr[5] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                            arrParamsAppr[5].Value = approval.CreatedBy;
                            arrParamsAppr[6] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                            arrParamsAppr[6].Direction = ParameterDirection.Output;
                            arrParamsAppr[7] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                            arrParamsAppr[7].Direction = ParameterDirection.Output;

                            DataTable dtApprovalData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_APPROVAL_INSERT_UPDATE, arrParamsAppr);
                            apprResponse = new APIResponse
                            {
                                Status = arrParamsAppr[6].Value.ToString(),
                                StatusDesc = arrParamsAppr[7].Value.ToString()
                            };
                            if (apprResponse.Status == Utilities.ERROR)
                            {
                                approverInsertErrorCounter++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex.ToString());
                            approverInsertErrorCounter++;
                            apprResponse = new APIResponse
                            {
                                Status = Utilities.ERROR,
                                StatusDesc = ex.ToString()
                            };
                        }
                    } // end dynamic approval list

                    // Scheme Approvers Removed
                    if (approverInsertErrorCounter > 0)
                    {
                        OracleParameter[] arrParamsApprRemov = new OracleParameter[3];
                        arrParamsApprRemov[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 250);
                        arrParamsApprRemov[0].Value = schemeRequest.SchemeRefNo;
                        arrParamsApprRemov[1] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                        arrParamsApprRemov[1].Direction = ParameterDirection.Output;
                        arrParamsApprRemov[2] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                        arrParamsApprRemov[2].Direction = ParameterDirection.Output;
                        DataTable dtApprRemovData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_APPROVERS_REMOVE, arrParamsApprRemov);
                        apprResponse = new APIResponse
                        {
                            Status = Utilities.ERROR,
                            StatusDesc = "Some error occured, while submitting the Approval List."
                        };
                        return apprResponse;
                    }

                }

                // Scheme Slab List Insert
                foreach (var slab in schemeRequest.SchemeSlabList)
                {
                    OracleParameter[] arrParamsSlab = new OracleParameter[8];
                    arrParamsSlab[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 250);
                    arrParamsSlab[0].Value = schemeRequest.SchemeRefNo;
                    arrParamsSlab[1] = new OracleParameter("P_SLAB", OracleDbType.Varchar2, 250);
                    arrParamsSlab[1].Value = slab.SlabNo;
                    arrParamsSlab[2] = new OracleParameter("P_SLAB_FROM", OracleDbType.Varchar2, 250);
                    arrParamsSlab[2].Value = slab.SlabFrom;
                    arrParamsSlab[3] = new OracleParameter("P_SLAB_TO", OracleDbType.Varchar2, 250);
                    arrParamsSlab[3].Value = slab.SlabTo;
                    arrParamsSlab[4] = new OracleParameter("P_SLAB_PAYOUT", OracleDbType.Varchar2, 250);
                    arrParamsSlab[4].Value = slab.SlabScheme;
                    arrParamsSlab[5] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 250);
                    arrParamsSlab[5].Value = schemeRequest.CreatedBy;
                    arrParamsSlab[6] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParamsSlab[6].Direction = ParameterDirection.Output;
                    arrParamsSlab[7] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParamsSlab[7].Direction = ParameterDirection.Output;
                    DataTable dtApprRemovDataPrev = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_SLAB_INSERT_UPDATE, arrParamsSlab);
                }


                // Scheme Calculation details
                OracleParameter[] arrParams = new OracleParameter[19];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = schemeRequest.SchemeRefNo;
                arrParams[1] = new OracleParameter("P_SETTLEMENT_CRITERIA", OracleDbType.Varchar2, 500);
                arrParams[1].Value = schemeRequest.SettlementCriteria;
                arrParams[2] = new OracleParameter("P_ELIGIBILITY_CALCULATIONS", OracleDbType.Varchar2, 500);
                arrParams[2].Value = schemeRequest.EligibilityCalculation;
                arrParams[3] = new OracleParameter("P_PAYOUT_CALCULATION", OracleDbType.Varchar2, 500);
                arrParams[3].Value = schemeRequest.PayoutCalculation;
                arrParams[4] = new OracleParameter("P_GROWTH_TYPE", OracleDbType.Varchar2, 250);
                arrParams[4].Value = schemeRequest.GrowthType;
                arrParams[5] = new OracleParameter("P_GROWTH", OracleDbType.Varchar2, 250);
                arrParams[5].Value = schemeRequest.Growth;

                // Sellin Period
                arrParams[6] = new OracleParameter("P_SELLIN_PERIOD_FROM", OracleDbType.Date);
                if (schemeRequest.SchemeSellInFrom == DateTime.MinValue)
                {
                    arrParams[6].Value = DBNull.Value;
                }
                else
                {
                    arrParams[6].Value = schemeRequest.SchemeSellInFrom;
                }
                arrParams[7] = new OracleParameter("P_SELLIN_PERIOD_TO", OracleDbType.Date);
                if (schemeRequest.SchemeSellInTo == DateTime.MinValue)
                {
                    arrParams[7].Value = DBNull.Value;
                }
                else
                {
                    arrParams[7].Value = schemeRequest.SchemeSellInTo;
                }

                // Sellout Period
                arrParams[8] = new OracleParameter("P_SELLOUT_PERIOD_FROM", OracleDbType.Date);
                if (schemeRequest.SchemeSelloutFrom == DateTime.MinValue)
                {
                    arrParams[8].Value = DBNull.Value;
                }
                else
                {
                    arrParams[8].Value = schemeRequest.SchemeSelloutFrom;
                }
                arrParams[9] = new OracleParameter("P_SELLOUT_PERIOD_TO", OracleDbType.Date);
                if (schemeRequest.SchemeSelloutTo == DateTime.MinValue)
                {
                    arrParams[9].Value = DBNull.Value;
                }
                else
                {
                    arrParams[9].Value = schemeRequest.SchemeSelloutTo;
                }

                // Leftover Sellin Period
                arrParams[10] = new OracleParameter("P_LEFTOVER_SELLIN_PERIOD_FROM", OracleDbType.Date);
                if (schemeRequest.LeftoverSellInFrom == DateTime.MinValue)
                {
                    arrParams[10].Value = DBNull.Value;
                }
                else
                {
                    arrParams[10].Value = schemeRequest.LeftoverSellInFrom;
                }
                arrParams[11] = new OracleParameter("P_LEFTOVER_SELLIN_PERIOD_TO", OracleDbType.Date);
                if (schemeRequest.LeftoverSellInTo == DateTime.MinValue)
                {
                    arrParams[11].Value = DBNull.Value;
                }
                else
                {
                    arrParams[11].Value = schemeRequest.LeftoverSellInTo;
                }

                // Leftover Sellout Period
                arrParams[12] = new OracleParameter("P_LEFTOVER_SELLOUT_PERIOD_FROM", OracleDbType.Date);
                if (schemeRequest.LeftoverSelloutFrom == DateTime.MinValue)
                {
                    arrParams[12].Value = DBNull.Value;
                }
                else
                {
                    arrParams[12].Value = schemeRequest.LeftoverSelloutFrom;
                }
                arrParams[13] = new OracleParameter("P_LEFTOVER_SELLOUT_PERIOD_TO", OracleDbType.Date);
                if (schemeRequest.LeftoverSelloutTo == DateTime.MinValue)
                {
                    arrParams[13].Value = DBNull.Value;
                }
                else
                {
                    arrParams[13].Value = schemeRequest.LeftoverSelloutTo;
                }

                arrParams[14] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[14].Value = schemeRequest.CreatedBy;
                arrParams[15] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[15].Direction = ParameterDirection.Output;
                arrParams[16] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[16].Direction = ParameterDirection.Output;
                arrParams[17] = new OracleParameter("P_TNCIDS", OracleDbType.Varchar2, 500);
                arrParams[17].Value = schemeRequest.TnCIds;
                arrParams[18] = new OracleParameter("P_URL", OracleDbType.Varchar2, 500);
                arrParams[18].Value = schemeRequest.URL;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.INSERT_UPDATE_REQUEST_CALCULATION_DETAILS, arrParams);
                response = new APIResponse
                {
                    Status = arrParams[15].Value.ToString(),
                    StatusDesc = arrParams[16].Value.ToString(),
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new
                    {
                        SchemeRefNo = schemeRequest.SchemeRefNo
                    }))
                };

            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
            return response;
        }

        /// <summary>
        /// Scheme Period Change , After Finally Approved Scheme.
        /// </summary>
        /// <param name="schemeRequest"></param>
        /// <returns></returns>
        public async Task<APIResponse> SchemeUpdateAfterApproved(SchemeRequest schemeRequest)
        {
            APIResponse response = null;
            try
            {
                if (string.IsNullOrEmpty(schemeRequest.SchemeRefNo.Trim()))
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = "Scheme Ref. No. can't be blank."
                    };
                    return response;
                }

                OracleParameter[] arrParams = new OracleParameter[10];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = schemeRequest.SchemeRefNo;
                arrParams[1] = new OracleParameter("P_SCHEME_PERIOD_TO", OracleDbType.Date);
                arrParams[1].Value = schemeRequest.SchemeToDate;

                // Sellin Period
                arrParams[2] = new OracleParameter("P_SELLIN_PERIOD_TO", OracleDbType.Date);
                if (schemeRequest.SchemeSellInTo == DateTime.MinValue)
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = schemeRequest.SchemeSellInTo;
                }
                // Sellout Period
                arrParams[3] = new OracleParameter("P_SELLOUT_PERIOD_TO", OracleDbType.Date);
                if (schemeRequest.SchemeSelloutTo == DateTime.MinValue)
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = schemeRequest.SchemeSelloutTo;
                }
                // Leftover Sellin Period
                arrParams[4] = new OracleParameter("P_LEFTOVER_SELLIN_PERIOD_TO", OracleDbType.Date);
                if (schemeRequest.LeftoverSellInTo == DateTime.MinValue)
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = schemeRequest.LeftoverSellInTo;
                }
                // Leftover Sellout Period
                arrParams[5] = new OracleParameter("P_LEFTOVER_SELLOUT_PERIOD_TO", OracleDbType.Date);
                if (schemeRequest.LeftoverSelloutTo == DateTime.MinValue)
                {
                    arrParams[5].Value = DBNull.Value;
                }
                else
                {
                    arrParams[5].Value = schemeRequest.LeftoverSelloutTo;
                }
                arrParams[6] = new OracleParameter("P_REMARKS", OracleDbType.Varchar2, 500);
                arrParams[6].Value = schemeRequest.Remarks;
                arrParams[7] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[7].Value = schemeRequest.CreatedBy;
                arrParams[8] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[8].Direction = ParameterDirection.Output;
                arrParams[9] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[9].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_UPDATE_AFTER_APPROVED, arrParams);
                response = new APIResponse
                {
                    Status = arrParams[8].Value.ToString(),
                    StatusDesc = arrParams[9].Value.ToString(),
                };
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
            return response;
        }



        /// <summary>
        /// Update Approve/Reject Status
        /// </summary>
        /// <param name="approval"></param>
        /// <returns></returns>
        public async Task<APIResponse> UpdateApprovalStatus(Approval approval)
        {
            APIResponse response = null;
            try
            {
                if (string.IsNullOrEmpty(approval.SchemeRefNo))
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = "Scheme Ref. No. can't be blank."
                    };
                    return response;
                }

                // Scheme Approval Status Update
                OracleParameter[] arrParams = new OracleParameter[8];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 250);
                arrParams[0].Value = approval.SchemeRefNo;
                arrParams[1] = new OracleParameter("P_STATUS_ID", OracleDbType.Int32);
                arrParams[1].Value = approval.ApprovalStatus;
                arrParams[2] = new OracleParameter("P_REMARKS", OracleDbType.Varchar2, 500);
                arrParams[2].Value = approval.ApprovalComments;
                arrParams[3] = new OracleParameter("P_USERID", OracleDbType.Varchar2, 100);
                arrParams[3].Value = approval.CreatedBy;
                arrParams[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_EMP_CODE", OracleDbType.Varchar2);
                arrParams[6].Value = approval.ApproverEmpCode;
                arrParams[7] = new OracleParameter("P_URL", OracleDbType.Varchar2);
                arrParams[7].Value = approval.URL;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.UPDATE_APPROVAL_STATUS, arrParams);
                response = new APIResponse
                {
                    Status = arrParams[4].Value.ToString(),
                    StatusDesc = arrParams[5].Value.ToString(),
                    data = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new
                    {
                        SchemeRefNo = approval.SchemeRefNo
                    }))
                };

            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
            return response;
        }

        /// <summary>
        /// Read Scheme basic details.
        /// </summary>
        /// <param name="schemeRequest"></param>
        /// <returns></returns>
        public async Task<List<SchemeRequest>> ReadSchemeRequest(SchemeRequest schemeRequest)
        {
            List<SchemeRequest> lstScheme = new List<SchemeRequest>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[11];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                arrParams[0].Value = schemeRequest.SchemeRefNo;
                arrParams[1] = new OracleParameter("P_ZONE", OracleDbType.Varchar2);
                arrParams[1].Value = schemeRequest.Zone;
                arrParams[2] = new OracleParameter("P_REGION", OracleDbType.Varchar2);
                arrParams[2].Value = schemeRequest.Region;
                arrParams[3] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2);
                arrParams[3].Value = schemeRequest.Branch;
                arrParams[4] = new OracleParameter("P_SCHEME_TYPE", OracleDbType.Varchar2);
                arrParams[4].Value = schemeRequest.SchemeType;
                arrParams[5] = new OracleParameter("P_SCHEME_REASON_CODE", OracleDbType.Varchar2);
                arrParams[5].Value = schemeRequest.SchemeReasonCode;
                arrParams[6] = new OracleParameter("P_FUND_SOURCE", OracleDbType.Varchar2);
                arrParams[6].Value = schemeRequest.FundSource;
                arrParams[7] = new OracleParameter("P_FROM_DATE", OracleDbType.Date);
                if (schemeRequest.SchemeFromDate == DateTime.MinValue || schemeRequest.SchemeFromDate == null)
                {
                    arrParams[7].Value = DBNull.Value;
                }
                else
                {
                    arrParams[7].Value = schemeRequest.SchemeFromDate;
                }
                arrParams[8] = new OracleParameter("P_TO_DATE", OracleDbType.Date);
                if (schemeRequest.SchemeToDate == DateTime.MinValue || schemeRequest.SchemeToDate == null)
                {
                    arrParams[8].Value = DBNull.Value;
                }
                else
                {
                    arrParams[8].Value = schemeRequest.SchemeToDate;
                }
                arrParams[9] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[9].Value = schemeRequest.CreatedBy;
                arrParams[10] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[10].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.READ_SCHEME_REQUEST, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    lstScheme.Add(FillSchemeDetails(row));
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                throw;
            }
            return lstScheme;
        }

        public async Task<List<Distributor>> ReadSchemeCustomerDetail(string SchemeRefNo)
        {
            List<Distributor> lstDistributor = new List<Distributor>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                arrParams[0].Value = SchemeRefNo;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.READ_CUSTOMER_DETAILS_BY_SCHEME, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Distributor distributor = new Distributor();
                    distributor.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_REF_NO"]);
                    distributor.Zone = SafeTypeHandling.ConvertToString(row["ZONE"]);
                    distributor.Region = SafeTypeHandling.ConvertToString(row["REGION"]);
                    distributor.Branch = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    distributor.BillingCode = SafeTypeHandling.ConvertToString(row["BILLING_CODE"]);
                    distributor.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    distributor.MajorSalesChannel = SafeTypeHandling.ConvertToString(row["MAJOR_SALES_CHANNEL"]);
                    distributor.SalesChannel = SafeTypeHandling.ConvertToString(row["SALES_CHANNEL"]);
                    distributor.ChannelCode = SafeTypeHandling.ConvertToString(row["CHANNEL"]);
                    distributor.ChannelCodeName = SafeTypeHandling.ConvertToString(row["CHANNEL_NAME"]);
                    lstDistributor.Add(distributor);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstDistributor;
        }

        public async Task<List<Model>> ReadSchemeProductDetail(string SchemeRefNo)
        {
            List<Model> lstProduct = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                arrParams[0].Value = SchemeRefNo;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.READ_PRODUCT_DETAILS_BY_SCHEME, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model model = new Model();
                    model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    model.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
                    model.ModelSeries = SafeTypeHandling.ConvertToString(row["SERIES"]);
                    model.ModelSubCategory = SafeTypeHandling.ConvertToString(row["MODEL_SUB_CATEGORY"]);
                    model.StarRating = SafeTypeHandling.ConvertToString(row["STAR_RATING"]);
                    model.ModelYear = SafeTypeHandling.ConvertToString(row["YEAR"]);
                    model.ModelNo = SafeTypeHandling.ConvertToString(row["MODEL_NO"]);
                    model.PayoutValue = SafeTypeHandling.ConvertToString(row["PAYOUT_VALUE"]);
                    lstProduct.Add(model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstProduct;
        }

        public async Task<List<SchemeRequest>> ReadApprovalScheme(string UserId)
        {
            List<SchemeRequest> lstScheme = new List<SchemeRequest>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[0].Value = UserId;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.READ_PENDING_APPROVAL_SCHEME, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    lstScheme.Add(FillSchemeDetails(row));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstScheme;
        }

        public async Task<List<Approval>> ReadAllApproverByScheme(string SchemeRefNo)
        {
            List<Approval> lstApprover = new List<Approval>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                arrParams[0].Value = SchemeRefNo;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.READ_ALL_APPROVER_BY_SCHEME, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Approval approval = new Approval();
                    approval.ApprovalSeq = SafeTypeHandling.ConvertToString(row["APPROVAL_SEQ"]);
                    approval.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_REF_NO"]);
                    approval.ApproverEmpCode = SafeTypeHandling.ConvertToString(row["APPROVER_CODE"]);
                    approval.ApproverName = SafeTypeHandling.ConvertToString(row["APPROVER_NAME"]);
                    approval.ApproverRoleName = SafeTypeHandling.ConvertToString(row["APPROVER_ROLE_NAME"]);
                    approval.ApprovalStatus = SafeTypeHandling.ConvertToString(row["APPROVAL_STATUS"]);
                    approval.ApprovalComments = SafeTypeHandling.ConvertToString(row["APPROVAL_COMMENTS"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(row["APPROVAL_DATE"])))
                    {
                        approval.ApprovalDate = SafeTypeHandling.ConvertToDateTime(row["APPROVAL_DATE"]);
                    }
                    approval.FixedApprover = SafeTypeHandling.ConvertToString(row["FIXED_APPROVER"]);
                    lstApprover.Add(approval);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstApprover;
        }

        public async Task<List<Approval>> ReadApprovalHistByScheme(string SchemeRefNo)
        {
            List<Approval> lstApprover = new List<Approval>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                arrParams[0].Value = SchemeRefNo;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.READ_APPROVAL_HISTORY_BY_SCHEME, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Approval approval = new Approval();
                    approval.ApprovalSeq = SafeTypeHandling.ConvertToString(row["APPROVAL_SEQ"]);
                    approval.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_REF_NO"]);
                    approval.ApproverEmpCode = SafeTypeHandling.ConvertToString(row["APPROVER_CODE"]);
                    approval.ApproverName = SafeTypeHandling.ConvertToString(row["APPROVER_NAME"]);
                    approval.ApprovalStatus = SafeTypeHandling.ConvertToString(row["APPROVAL_STATUS"]);
                    approval.ApprovalComments = SafeTypeHandling.ConvertToString(row["APPROVAL_COMMENTS"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(row["APPROVAL_DATE"])))
                    {
                        approval.ApprovalDate = SafeTypeHandling.ConvertToDateTime(row["APPROVAL_DATE"]);
                    }
                    lstApprover.Add(approval);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstApprover;
        }

        public async Task<List<TermsConditions>> ReadTnCByScheme(string SchemeRefNo)
        {
            List<TermsConditions> lstTermsConditions = new List<TermsConditions>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                arrParams[0].Value = SchemeRefNo;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.READ_TERM_CONDITION_BY_SCHEME, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    TermsConditions tnc = new TermsConditions();
                    tnc.Id = SafeTypeHandling.ConvertStringToInt32(row["TNCID"]);
                    tnc.TermCondition = SafeTypeHandling.ConvertToString(row["TERMS_CONDITIONS"]);
                    lstTermsConditions.Add(tnc);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstTermsConditions;
        }

        public async Task<List<Slab_TargetDOM>> ReadSchemeSlab(string SchemeRefNo)
        {
            List<Slab_TargetDOM> lstSlab = new List<Slab_TargetDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                arrParams[0].Value = SchemeRefNo;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_SLAB_READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Slab_TargetDOM slab = new Slab_TargetDOM();
                    slab.SlabNo = SafeTypeHandling.ConvertToString(row["SLAB"]);
                    slab.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_REF_NO"]);
                    slab.SlabFrom = SafeTypeHandling.ConvertToString(row["SLAB_FROM"]);
                    slab.SlabTo = SafeTypeHandling.ConvertToString(row["SLAB_TO"]);
                    slab.SlabScheme = SafeTypeHandling.ConvertToString(row["SLAB_PAYOUT"]);
                    lstSlab.Add(slab);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstSlab;
        }

        private SchemeRequest FillSchemeDetails(DataRow row)
        {
            SchemeRequest _scheme = new SchemeRequest();
            _scheme.SchemeId = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
            _scheme.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_REF_NO"]);
            _scheme.SchemeName = SafeTypeHandling.ConvertToString(row["SCHEME_NAME"]);
            _scheme.FundSource = SafeTypeHandling.ConvertToString(row["FUND_SOURCE"]);
            _scheme.SchemeType = SafeTypeHandling.ConvertToString(row["SCHEME_TYPE"]);
            _scheme.SchemeReasonCode = SafeTypeHandling.ConvertToString(row["SCHEME_REASON_CODE"]);
            _scheme.PayoutType = SafeTypeHandling.ConvertToString(row["PAYOUT_TYPE"]);
            _scheme.SchemeFromDate = SafeTypeHandling.ConvertToDateTime(row["PERIOD_FROM"]);
            _scheme.SchemeToDate = SafeTypeHandling.ConvertToDateTime(row["PERIOD_TO"]);

            _scheme.Zone = SafeTypeHandling.ConvertToString(row["ZONE"]);
            _scheme.Region = SafeTypeHandling.ConvertToString(row["REGION"]);
            _scheme.Branch = SafeTypeHandling.ConvertToString(row["BRANCH"]);
            _scheme.MajorSalesChannel = SafeTypeHandling.ConvertToString(row["MajorSalesChannel"]);
            _scheme.SalesChannel = SafeTypeHandling.ConvertToString(row["SalesChannel"]);
            _scheme.Channel = SafeTypeHandling.ConvertToString(row["Channel"]);
            _scheme.BillingCode = SafeTypeHandling.ConvertToString(row["BillingCode"]);

            //_scheme.ProductDetail.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
            //_scheme.ProductDetail.SubProduct = SafeTypeHandling.ConvertToString(row["SUBPRODUCT"]);
            //_scheme.ProductDetail.ModelCategory = SafeTypeHandling.ConvertToString(row["MODELCATEGORY"]);
            //_scheme.ProductDetail.ModelSeries = SafeTypeHandling.ConvertToString(row["MODELSERIES"]);
            //_scheme.ProductDetail.ModelNo = SafeTypeHandling.ConvertToString(row["MODELNO"]);

            _scheme.ProductDetail.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
            _scheme.ProductDetail.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
            _scheme.ProductDetail.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
            _scheme.ProductDetail.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
            _scheme.ProductDetail.ModelSeries = SafeTypeHandling.ConvertToString(row["SERIES"]);
            _scheme.ProductDetail.ModelSubCategory = SafeTypeHandling.ConvertToString(row["MODEL_SUB_CATEGORY"]);
            _scheme.ProductDetail.StarRating = SafeTypeHandling.ConvertToString(row["STAR_RATING"]);
            _scheme.ProductDetail.ModelYear = SafeTypeHandling.ConvertToString(row["YEAR"]);
            _scheme.ProductDetail.ModelNo = SafeTypeHandling.ConvertToString(row["MODEL_NO"]);

            _scheme.SettlementCriteria = SafeTypeHandling.ConvertToString(row["SETTLEMENT_CRITERIA"]);
            _scheme.EligibilityCalculation = SafeTypeHandling.ConvertToString(row["ELIGIBILITY_CALCULATION"]);
            _scheme.PayoutCalculation = SafeTypeHandling.ConvertToString(row["PAYOUT_CALCULATION"]);
            _scheme.Growth = SafeTypeHandling.ConvertToString(row["GROWTH"]);
            _scheme.GrowthType = SafeTypeHandling.ConvertToString(row["GROWTH_TYPE"]);

            // Sellin & Sellout Periods
            if (!string.IsNullOrEmpty(Convert.ToString(row["SELL_IN_PERIOD_FROM"])))
            {
                _scheme.SchemeSellInFrom = SafeTypeHandling.ConvertToDateTime(row["SELL_IN_PERIOD_FROM"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(row["SELL_IN_PERIOD_TO"])))
            {
                _scheme.SchemeSellInTo = SafeTypeHandling.ConvertToDateTime(row["SELL_IN_PERIOD_TO"]);
            }

            if (!string.IsNullOrEmpty(Convert.ToString(row["SELL_OUT_PERIOD_FROM"])))
            {
                _scheme.SchemeSelloutFrom = SafeTypeHandling.ConvertToDateTime(row["SELL_OUT_PERIOD_FROM"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(row["SELL_OUT_PERIOD_TO"])))
            {
                _scheme.SchemeSelloutTo = SafeTypeHandling.ConvertToDateTime(row["SELL_OUT_PERIOD_TO"]);
            }

            // Leftover Sellin & Sellout Periods
            if (!string.IsNullOrEmpty(Convert.ToString(row["LEFTOVER_SELLIN_PERIOD_FROM"])))
            {
                _scheme.LeftoverSellInFrom = SafeTypeHandling.ConvertToDateTime(row["LEFTOVER_SELLIN_PERIOD_FROM"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(row["LEFTOVER_SELLIN_PERIOD_TO"])))
            {
                _scheme.LeftoverSellInTo = SafeTypeHandling.ConvertToDateTime(row["LEFTOVER_SELLIN_PERIOD_TO"]);
            }

            if (!string.IsNullOrEmpty(Convert.ToString(row["LEFTOVER_SELLOUT_PERIOD_FROM"])))
            {
                _scheme.LeftoverSelloutFrom = SafeTypeHandling.ConvertToDateTime(row["LEFTOVER_SELLOUT_PERIOD_FROM"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(row["LEFTOVER_SELLOUT_PERIOD_TO"])))
            {
                _scheme.LeftoverSelloutTo = SafeTypeHandling.ConvertToDateTime(row["LEFTOVER_SELLOUT_PERIOD_TO"]);
            }

            _scheme.StatusId = SafeTypeHandling.ConvertStringToInt32(row["STATUS"]);
            _scheme.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
            _scheme.CreatedBy = SafeTypeHandling.ConvertStringToInt32(row["CREATED_BY"]);
            _scheme.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY_NAME"]);
            _scheme.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
            _scheme.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPDATED_BY"]);
            _scheme.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY_NAME"]);
            _scheme.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
            _scheme.ApproverEmpCode = SafeTypeHandling.ConvertToString(row["APPROVER_EMP_CODE"]);
            _scheme.ProcessType = SafeTypeHandling.ConvertToString(row["PROCESS_TYPE"]);
            return _scheme;
        }


        #region Scheme Calculation

        /// <summary>
        /// Scheme Calculation Request
        /// </summary>
        /// <param name="schemeSettlement"></param>
        /// <returns></returns>
        public async Task<APIResponse> SchemeCalculationRequest(SchemeSettlementDOM schemeSettlement)
        {
            APIResponse response = null;
            try
            {
                if (string.IsNullOrEmpty(schemeSettlement.SchemeRefNo))
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = "Scheme Ref. No. can't be blank."
                    };
                    return response;
                }

                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = schemeSettlement.SchemeRefNo;
                arrParams[1] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[1].Value = schemeSettlement.CreatedBy;
                arrParams[2] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[2].Direction = ParameterDirection.Output;
                arrParams[3] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[3].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.SchemeSettlement.SCHEME_CALCULATION_REQUEST_SAVE, arrParams, DealerNet_Next_Connection);
                response = new APIResponse
                {
                    Status = arrParams[2].Value.ToString(),
                    StatusDesc = arrParams[3].Value.ToString()
                };
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex.ToString());
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
            return response;
        }

        /// <summary>
        /// Scheme Calculation Requests Read
        /// </summary>
        /// <param name="schemeSettlement"></param>
        /// <returns></returns>
        public async Task<List<SchemeSettlementDOM>> SchemeCalculationRequestRead(SchemeSettlementDOM schemeSettlement)
        {
            List<SchemeSettlementDOM> lstSchemeCalculationRequest = new List<SchemeSettlementDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = schemeSettlement.SchemeRefNo;
                arrParams[1] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[1].Value = schemeSettlement.CreatedBy;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.SchemeSettlement.SCHEME_CALCULATION_REQUEST_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    SchemeSettlementDOM _schemeSettlement = new SchemeSettlementDOM();
                    _schemeSettlement.Id = SafeTypeHandling.ConvertStringToInt64(row["ID"]);
                    _schemeSettlement.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_REF_NO"]);
                    _schemeSettlement.SchemeName = SafeTypeHandling.ConvertToString(row["SCHEME_NAME"]);
                    _schemeSettlement.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    _schemeSettlement.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    _schemeSettlement.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    _schemeSettlement.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _schemeSettlement.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    _schemeSettlement.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    lstSchemeCalculationRequest.Add(_schemeSettlement);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstSchemeCalculationRequest;
        }

        /// <summary>
        /// Scheme Calculation Read
        /// </summary>
        /// <param name="schemeSettlement"></param>
        /// <returns></returns>
        public async Task<List<SchemeSettlementDOM>> SchemeCalculationRead(SchemeSettlementDOM schemeSettlement)
        {
            List<SchemeSettlementDOM> lstSchemeCalculation = new List<SchemeSettlementDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = schemeSettlement.SchemeRefNo;
                arrParams[1] = new OracleParameter("P_REQUEST_ID", OracleDbType.Varchar2, 100);
                arrParams[1].Value = schemeSettlement.Id;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.SchemeSettlement.SCHEME_CALCULATION_READ, arrParams, DealerNet_Next_Connection);

                foreach (DataRow row in dtData.Rows)
                {
                    SchemeSettlementDOM _schemeSettlement = new SchemeSettlementDOM();
                    //_schemeSettlement.Id = SafeTypeHandling.ConvertStringToInt64(row["ID"]);
                    _schemeSettlement.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_REF_NO"]);
                    _schemeSettlement.SchemeName = SafeTypeHandling.ConvertToString(row["SCHEME_NAME"]);
                    _schemeSettlement.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    _schemeSettlement.BillingCode = SafeTypeHandling.ConvertToString(row["BILLING_CODE"]);
                    _schemeSettlement.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    _schemeSettlement.Channel = SafeTypeHandling.ConvertToString(row["CHANNEL"]);
                    _schemeSettlement.ChannelName = SafeTypeHandling.ConvertToString(row["CHANNEL_NAME"]);
                    _schemeSettlement.Branch = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    _schemeSettlement.ProductDetail.Product = SafeTypeHandling.ConvertToString(row["PRODUCT_CATEGORY"]);
                    _schemeSettlement.ProductDetail.ModelPrefix = SafeTypeHandling.ConvertToString(row["MODEL_PREFIX"]);
                    _schemeSettlement.ProductDetail.ModelNo = SafeTypeHandling.ConvertToString(row["MODEL_SUFFIX"]);
                    //Leftover
                    _schemeSettlement.LeftoverSellIn_Normal = SafeTypeHandling.ConvertToString(row["LEFTOVER_SELLIN_NORMAL"]);
                    _schemeSettlement.LeftoverSellIn_Display = SafeTypeHandling.ConvertToString(row["LEFTOVER_SELLIN_DISPLAY"]);
                    _schemeSettlement.LeftoverSellIn_Total = SafeTypeHandling.ConvertToString(row["LEFTOVER_SELLIN_TOTAL"]);
                    _schemeSettlement.Leftover_Sellout = SafeTypeHandling.ConvertToString(row["LEFTOVER_SELLOUT"]);
                    _schemeSettlement.Leftover_Leftover = SafeTypeHandling.ConvertToString(row["LEFTOVER"]);
                    //Scheme
                    _schemeSettlement.SchemeSellIn_Normal = SafeTypeHandling.ConvertToString(row["SCHEME_SELLIN_NORMAL"]);
                    _schemeSettlement.SchemeSellIn_Display = SafeTypeHandling.ConvertToString(row["SCHEME_SELLIN_DISPLAY"]);
                    _schemeSettlement.SchemeSellIn_Total = SafeTypeHandling.ConvertToString(row["SCHEME_SELLIN_TOTAL"]);
                    _schemeSettlement.Scheme_TotalStock = SafeTypeHandling.ConvertToString(row["SCHEME_TOTAL_STOCK"]);
                    _schemeSettlement.Scheme_Sellout = SafeTypeHandling.ConvertToString(row["SCHEME_SELLOUT"]);
                    _schemeSettlement.Scheme_Eligibility = SafeTypeHandling.ConvertToString(row["SCHEME_ELIGIBILITY"]);
                    _schemeSettlement.Scheme_Scheme = SafeTypeHandling.ConvertToString(row["SCHEME"]);
                    _schemeSettlement.Scheme_LastYearBase = SafeTypeHandling.ConvertToString(row["LY_BASE"]);
                    _schemeSettlement.Scheme_GrowthPercentage = SafeTypeHandling.ConvertToString(row["GROWTH_PERCENTAGE"]);
                    _schemeSettlement.Target = SafeTypeHandling.ConvertToString(row["TARGET"]);
                    _schemeSettlement.Scheme_Payout = SafeTypeHandling.ConvertToString(row["PAYOUT"]);
                    lstSchemeCalculation.Add(_schemeSettlement);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstSchemeCalculation;
        }

        /// <summary>
        /// GTM Scheme Raw data Read, which is used for calculation
        /// </summary>
        /// <param name="SchemeRefNo"></param>
        /// <returns></returns>
        public async Task<List<GTMSchemeDOM>> GTMSchemeDataRead(string SchemeRefNo)
        {
            List<GTMSchemeDOM> lstGTMScheme = new List<GTMSchemeDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 100);
                arrParams[0].Value = SchemeRefNo;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.SchemeSettlement.GTM_SCHEME_DATA_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    GTMSchemeDOM gtmScheme = new GTMSchemeDOM();
                    gtmScheme.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_REF_NO"]);
                    gtmScheme.SelloutDate = SafeTypeHandling.ConvertToDateTime(row["SELLOUT_DATE"]);
                    gtmScheme.MonthYear = SafeTypeHandling.ConvertToString(row["MONTH_YEAR"]);
                    gtmScheme.BillingCode = SafeTypeHandling.ConvertToString(row["BILLING_CODE"]);
                    gtmScheme.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    gtmScheme.ChannelCode = SafeTypeHandling.ConvertToString(row["PRICING_GROUP_CHANNEL"]);
                    gtmScheme.Region = SafeTypeHandling.ConvertToString(row["REGION"]);
                    gtmScheme.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH_NAME"]);
                    gtmScheme.ModelPrefix = SafeTypeHandling.ConvertToString(row["MODEL_PREFIX"]);
                    gtmScheme.ModelSuffix = SafeTypeHandling.ConvertToString(row["MODEL_NUMBER"]);
                    gtmScheme.ProductGrade = SafeTypeHandling.ConvertToString(row["PRODUCT_GRADE"]);
                    gtmScheme.ProductCategory = SafeTypeHandling.ConvertToString(row["PRODUCT_CATEGORY"]);
                    gtmScheme.ProductGroup = SafeTypeHandling.ConvertToString(row["PRODUCT_GROUP"]);
                    gtmScheme.BasicPricePerUnit = SafeTypeHandling.ConvertToString(row["BASIC_PRICE_PER_UNIT"]);
                    gtmScheme.OpeningStock = SafeTypeHandling.ConvertToString(row["OPENING_STOCK"]);
                    gtmScheme.SellIn = SafeTypeHandling.ConvertToString(row["SELL_IN"]);
                    gtmScheme.SellOut = SafeTypeHandling.ConvertToString(row["SELL_OUT"]);
                    gtmScheme.Closing = SafeTypeHandling.ConvertToString(row["CLOSING"]);
                    gtmScheme.BPOpeningStock = SafeTypeHandling.ConvertToString(row["BP_OPENING_STOCK"]);
                    gtmScheme.BPSellIn = SafeTypeHandling.ConvertToString(row["BP_SELL_IN"]);
                    gtmScheme.BPSellOut = SafeTypeHandling.ConvertToString(row["BP_SELL_OUT"]);
                    gtmScheme.BPClosing = SafeTypeHandling.ConvertToString(row["BP_CLOSING"]);
                    gtmScheme.SourceSystem = SafeTypeHandling.ConvertToString(row["SOURCE_SYSTEM"]);
                    lstGTMScheme.Add(gtmScheme);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex.ToString());
            }
            return lstGTMScheme;
        }


        #endregion

        public async Task<APIResponse> SerialNoApplicability(SerailNoApplicability serailNoApplicability)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 250);
                arrParams[0].Value = serailNoApplicability.HeaderCode;
                arrParams[1] = new OracleParameter("P_APPLICABILITY_STATUS", OracleDbType.Varchar2, 100);
                arrParams[1].Value = serailNoApplicability.Status.ToUpper();
                arrParams[2] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[2].Value = serailNoApplicability.CreatedBy;
                arrParams[3] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[3].Direction = ParameterDirection.Output;
                arrParams[4] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[4].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.SerialNoApplicability.SERIAL_NO_APPLICABILITY_SAVE_UPDATE, arrParams, DealerNet_Next_Connection);
                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[3].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[4].Value),
                };
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex.ToString());
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
            return response;
        }

        public async Task<List<SerailNoApplicability>> SerialNoApplicabilityRead(SerailNoApplicability serailNoApplicability)
        {
            List<SerailNoApplicability> lstSerailNoApplicability = new List<SerailNoApplicability>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[0].Value = serailNoApplicability.HeaderCode;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.SerialNoApplicability.SERIAL_NO_APPLICABILITY_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    SerailNoApplicability srnoApp = new SerailNoApplicability();
                    srnoApp.Id = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    srnoApp.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    srnoApp.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    srnoApp.Status = SafeTypeHandling.ConvertToString(row["SN_APPLY_STATUS"]);
                    srnoApp.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    srnoApp.CreatedBy = SafeTypeHandling.ConvertStringToInt32(row["CREATED_BY"]);
                    srnoApp.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY_NAME"]);
                    srnoApp.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    srnoApp.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPDATED_BY"]);
                    srnoApp.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY_NAME"]);
                    srnoApp.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    lstSerailNoApplicability.Add(srnoApp);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex.ToString());
            }
            return lstSerailNoApplicability;
        }

        /// <summary>
        /// Scheme Target Upload
        /// </summary>
        /// <param name="lstSchemeTarget"></param>
        /// <returns></returns>
        public async Task<APIResponse> SchemeTargetUpload(List<Slab_TargetDOM> lstSchemeTarget)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (var target in lstSchemeTarget)
            {
                try
                {
                    arrParams = new OracleParameter[9];
                    arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = target.SchemeRefNo;
                    arrParams[1] = new OracleParameter("P_BILLING_CODE", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = target.BillingCode;
                    arrParams[2] = new OracleParameter("P_TARGET", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = target.Target;
                    arrParams[3] = new OracleParameter("P_TARGET_TYPE", OracleDbType.Varchar2, 100);
                    if (string.IsNullOrEmpty(target.TargetType))
                    {
                        arrParams[3].Value = DBNull.Value;
                    }
                    else
                    {
                        arrParams[3].Value = target.TargetType;
                    }
                    arrParams[4] = new OracleParameter("P_TARGET_BASED", OracleDbType.Varchar2, 100);
                    arrParams[4].Value = target.TargetBased;
                    arrParams[5] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[5].Value = target.CreatedBy;
                    arrParams[6] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[6].Value = target.RowNumber;
                    arrParams[7] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[7].Direction = ParameterDirection.Output;
                    arrParams[8] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[8].Direction = ParameterDirection.Output;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_TARGET_INSERT_TEMP, arrParams);
                    // Check Response
                    if (arrParams[7].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[8].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", target.RowNumber, ": ", ex.ToString()) });
                    Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                }
            }
            // if Error Found while data inset into Temp table
            if (errors.Count > 0)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = Utilities.EXCEL_FILE_DATA_ISSUES,
                    data = errors
                };
                return response;
            }

            Int32 createdBy = (Int32)lstSchemeTarget[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_TARGET_TEMP_DATA_VALIDATE, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    errors.Add(new Errors { Error = SafeTypeHandling.ConvertToString(row["REMARKS"]) });
                }

                // if Error Found while validate Temp table data
                if (errors.Count > 0)
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = Utilities.EXCEL_DATA_VALIDATION_FAILED,
                        data = errors
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString(),
                };
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                return response;
            }

            // Finally records submit in Main Table
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_TARGET_INSERT_FINAL, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    errors.Add(new Errors { Error = SafeTypeHandling.ConvertToString(row["REMARKS"]) });
                }

                // if Error Found while insert invoice in Main table
                if (errors.Count > 0)
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = Utilities.SOME_ERROR_OCCURED_WHILE_SUBMIT_RECORD,
                        data = errors
                    };
                    return response;
                }
                else
                {
                    response = new APIResponse
                    {
                        Status = Utilities.SUCCESS,
                        StatusDesc = Utilities.RECORD_SBUMITTED_SUCCESSFULLY,
                        data = errors
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString(),
                };
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                return response;
            }

        }

        /// <summary>
        /// Read Scheme Target Registration
        /// </summary>
        /// <param name="SchemeRefNo"></param>
        /// <returns></returns>
        public async Task<List<Slab_TargetDOM>> ReadSchemeTarget(string SchemeRefNo)
        {
            List<Slab_TargetDOM> lstTarget = new List<Slab_TargetDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                arrParams[0].Value = SchemeRefNo;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.SCHEME_TARGET_READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Slab_TargetDOM slab = new Slab_TargetDOM();
                    slab.SchemeRefNo = SafeTypeHandling.ConvertToString(row["SCHEME_REF_NO"]);
                    slab.BillingCode = SafeTypeHandling.ConvertToString(row["BILLING_CODE"]);
                    slab.TargetType = SafeTypeHandling.ConvertToString(row["TARGET_TYPE"]);
                    slab.TargetBased = SafeTypeHandling.ConvertToString(row["TARGET_BASED"]);
                    slab.Target = SafeTypeHandling.ConvertToString(row["TARGET"]);
                    lstTarget.Add(slab);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstTarget;
        }

    }
}
