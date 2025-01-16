using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.DomainObject.ARCreditLimit;
using DealerNetAPI.ResourceAccess.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.ARCreditLimit
{
    public class ARCreditLimitAccess : IARCreditLimitAccess
    {
        private readonly ICommonDB _commonDB = null;
        private readonly IUtilitiesARCreditLimitAccess _utilitiesARCreditLimitAccess = null;
        private readonly string DealerNet_Next_Connection = "DealerNetNextConnection";

        public ARCreditLimitAccess(ICommonDB commonDB, IUtilitiesARCreditLimitAccess utilitiesARCreditLimitAccess)
        {
            _commonDB = commonDB;
            _utilitiesARCreditLimitAccess = utilitiesARCreditLimitAccess;
        }

        /// <summary>
        /// Read Header Code Details
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public async Task<ARCreditLimitDOM> ReadHeaderCodeDetail(Distributor distributor)
        {
            ARCreditLimitDOM customer = new ARCreditLimitDOM();
            try
            {
                // Header Code Details Read.
                OracleParameter[] arrParams = new OracleParameter[7];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int32);
                if (distributor.Id == null || distributor.Id == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = distributor.Id;
                }
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = distributor.HeaderCode;
                arrParams[2] = new OracleParameter("P_CTRL_AU", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(distributor.CTRL_AU))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = distributor.CTRL_AU;
                }
                arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[3].Value = distributor.CreatedBy;
                arrParams[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[6].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.HEADER_CODE_DETAIL_READ, arrParams, DealerNet_Next_Connection);
                customer.Status = SafeTypeHandling.ConvertToString(arrParams[4].Value);
                customer.StatusDesc = SafeTypeHandling.ConvertToString(arrParams[5].Value);

                foreach (DataRow row in dtData.Rows)
                {
                    customer.Distributor.Id = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    customer.Distributor.HeaderCode = SafeTypeHandling.ConvertToString(row["LOGIN_ID"]);
                    customer.Distributor.AccountName = SafeTypeHandling.ConvertToString(row["LOGIN_NAME"]);
                    customer.Distributor.CTRL_AU = SafeTypeHandling.ConvertToString(row["CTRL_AU"]);
                    customer.Insurance.InsuranceCode = SafeTypeHandling.ConvertToString(row["INSURANCE_CODE"]);
                    customer.Insurance.InsuranceName = SafeTypeHandling.ConvertToString(row["INSURANCE_COMPANY"]);
                    if (!string.IsNullOrEmpty(SafeTypeHandling.ConvertToString(row["CURRENT_CREDIT_LIMIT"])))
                    {
                        customer.Insurance.CurrentCreditLimitAmount = SafeTypeHandling.ConvertStringToDecimal(row["CURRENT_CREDIT_LIMIT"]);
                    }
                    if (!string.IsNullOrEmpty(SafeTypeHandling.ConvertToString(row["LIMIT_END_DATE"])))
                    {
                        customer.Insurance.CurrentCreditLimitEndDate = SafeTypeHandling.ConvertToDateTime(row["LIMIT_END_DATE"]);
                    }
                }

                if (customer.Status == Utilities.SUCCESS)
                {
                    customer.SalesHistory = ReadSalesHistory(distributor);
                    customer.Sales_PaymentTrend = ReadSales_PaymentTrend(distributor);
                    customer.OD_History = ReadOD_History(distributor);
                    customer.FutureSalesPlan = ReadFutureSalesPlan(distributor);
                    customer.Notes = await _utilitiesARCreditLimitAccess.ReadNotes(new Notes { Status = "Y" });
                    customer.FYAttachment = ReadFinancialYearAttachment(distributor);
                }
            }
            catch (Exception ex)
            {
                customer.Status = Utilities.ERROR;
                customer.StatusDesc = Utilities.SOMETHING_WENT_WRONG;
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
            }
            return customer;
        }

        /// <summary>
        /// Read Sales History of TOTAL, AVG, MAX, Max sale month data.
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public List<ARCreditLimitSalesDOM> ReadSalesHistory(Distributor distributor)
        {
            List<ARCreditLimitSalesDOM> lstSaleHistory = new List<ARCreditLimitSalesDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int32);
                if (distributor.Id == null || distributor.Id == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = distributor.Id;
                }
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = distributor.HeaderCode;
                arrParams[2] = new OracleParameter("P_CTRL_AU", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(distributor.CTRL_AU))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = distributor.CTRL_AU;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.ARCreditLimitSystem.CreditLimit.SALES_HISTORY_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitSalesDOM saleHistory = new ARCreditLimitSalesDOM();
                    saleHistory.DataSource = SafeTypeHandling.ConvertToString(row["SOURCE_TYPE"]);
                    saleHistory.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    saleHistory.SaleAmount = SafeTypeHandling.ConvertStringToDecimal(row["TOTAL_SALE_AMOUNT"]);
                    saleHistory.AvgSaleAmount = SafeTypeHandling.ConvertStringToDecimal(row["AVG_SALE_AMOUNT"]);
                    saleHistory.MaxSaleAmount = SafeTypeHandling.ConvertStringToDecimal(row["MAX_SALE_AMOUNT"]);
                    saleHistory.MaxSaleMonthYear = SafeTypeHandling.ConvertToString(row["MAX_SALE_MONTH"]);
                    lstSaleHistory.Add(saleHistory);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstSaleHistory;
        }

        /// <summary>
        /// Read Sales & Payment Trends
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public List<ARCreditLimitSalesDOM> ReadSales_PaymentTrend(Distributor distributor)
        {
            List<ARCreditLimitSalesDOM> lstSales_PaymentTrend = new List<ARCreditLimitSalesDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int32);
                if (distributor.Id == null || distributor.Id == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = distributor.Id;
                }
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = distributor.HeaderCode;
                arrParams[2] = new OracleParameter("P_CTRL_AU", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(distributor.CTRL_AU))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = distributor.CTRL_AU;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.ARCreditLimitSystem.CreditLimit.SALES_PAYMENT_TREND_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitSalesDOM saleHistory = new ARCreditLimitSalesDOM();
                    saleHistory.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    saleHistory.MonthYear = SafeTypeHandling.ConvertToString(row["MONTH_YEAR"]);
                    saleHistory.SaleAmount = SafeTypeHandling.ConvertStringToDecimal(row["SALE_AMOUNT"]);
                    saleHistory.CollectionAmount = SafeTypeHandling.ConvertStringToDecimal(row["COLLECTION_AMOUNT"]);
                    lstSales_PaymentTrend.Add(saleHistory);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstSales_PaymentTrend;
        }

        /// <summary>
        /// Read OD History Summary
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public ARCreditLimitOD ReadOD_History(Distributor distributor)
        {
            ARCreditLimitOD OD_History = new ARCreditLimitOD();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int32);
                if (distributor.Id == null || distributor.Id == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = distributor.Id;
                }
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = distributor.HeaderCode;
                arrParams[2] = new OracleParameter("P_CTRL_AU", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(distributor.CTRL_AU))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = distributor.CTRL_AU;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.ARCreditLimitSystem.CreditLimit.OD_HISTORY_SUMMARY_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    OD_History.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    OD_History.TotalOD = SafeTypeHandling.ConvertStringToInt32(row["TOTAL_OD"]);
                    OD_History.MaxOD = SafeTypeHandling.ConvertStringToDecimal(row["MAX_OD"]);
                    OD_History.AverageOD = SafeTypeHandling.ConvertStringToDecimal(row["AVG_OD"]);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return OD_History;
        }

        /// <summary>
        /// Read Future Sales Plan
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public List<ARCreditLimitSalesDOM> ReadFutureSalesPlan(Distributor distributor)
        {
            List<ARCreditLimitSalesDOM> lstFutureSalesPlan = new List<ARCreditLimitSalesDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int32);
                if (distributor.Id == null || distributor.Id == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = distributor.Id;
                }
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = distributor.HeaderCode;
                arrParams[2] = new OracleParameter("P_CTRL_AU", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(distributor.CTRL_AU))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = distributor.CTRL_AU;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.ARCreditLimitSystem.CreditLimit.FUTURE_SALES_PLAN_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitSalesDOM saleHistory = new ARCreditLimitSalesDOM();
                    saleHistory.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    saleHistory.MonthYear = SafeTypeHandling.ConvertToString(row["MONTH_YEAR"]);
                    saleHistory.SaleAmount = SafeTypeHandling.ConvertStringToDecimal(row["SALE_AMOUNT"]);
                    lstFutureSalesPlan.Add(saleHistory);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstFutureSalesPlan;
        }
        /// <summary>
        /// Read Financial Year Attachment
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public List<ARCreditLimitFYStatus> ReadFinancialYearAttachment(Distributor distributor)
        {
            List<ARCreditLimitFYStatus> lstFYAttachment = new List<ARCreditLimitFYStatus>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int64);
                if (distributor.Id == null || distributor.Id == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = distributor.Id;
                }
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = distributor.HeaderCode;
                arrParams[2] = new OracleParameter("P_CTRL_AU", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(distributor.CTRL_AU))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = distributor.CTRL_AU;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.ARCreditLimitSystem.CreditLimit.FY_ATTACHMENT_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitFYStatus fyAttachment = new ARCreditLimitFYStatus();
                    fyAttachment.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    fyAttachment.FYStatusId = SafeTypeHandling.ConvertStringToInt32(row["FINANCIAL_YEAR_STATUS"]);
                    fyAttachment.FYStatus = SafeTypeHandling.ConvertToString(row["FY_STATUS_NAME"]);
                    fyAttachment.FinancialYear = SafeTypeHandling.ConvertToString(row["FINANCIAL_YEAR"]);
                    fyAttachment.MandatoryStatus = SafeTypeHandling.ConvertToString(row["MANDATORY"]);
                    fyAttachment.UploadStatus = SafeTypeHandling.ConvertToString(row["UPLOAD"]);
                    fyAttachment.FileNamePath = SafeTypeHandling.ConvertToString(row["FILE_NAME_PATH"]);
                    lstFYAttachment.Add(fyAttachment);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstFYAttachment;
        }

        /// <summary>
        /// Read Nnotes of Requset
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public List<Notes> ReadNotes(Distributor distributor)
        {
            List<Notes> lstNotes = new List<Notes>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int32);
                arrParams[0].Value = distributor.Id;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.ARCreditLimitSystem.CreditLimit.NOTES_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Notes note = new Notes();
                    note.Id = SafeTypeHandling.ConvertStringToInt32(row["NOTE_ID"]);
                    note.Note = SafeTypeHandling.ConvertToString(row["NOTE"]);
                    note.OrderNo = SafeTypeHandling.ConvertStringToInt32(row["ORDER_NO"]);
                    lstNotes.Add(note);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstNotes;
        }


        /// <summary>
        /// Credit Limit Requset Submit
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        public async Task<APIResponse> CreditLimitRequestSave(ARCreditLimitDOM creditLimitDOM)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[14];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int64);
                if (creditLimitDOM.ReqId == null || creditLimitDOM.ReqId == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = creditLimitDOM.ReqId;
                }
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = creditLimitDOM.Distributor.HeaderCode;
                arrParams[2] = new OracleParameter("P_CTRL_AU", OracleDbType.Varchar2, 100);
                arrParams[2].Value = creditLimitDOM.Distributor.CTRL_AU;
                arrParams[3] = new OracleParameter("P_CURRENT_CL_AMOUNT", OracleDbType.Varchar2, 100);
                arrParams[3].Value = creditLimitDOM.Insurance.CurrentCreditLimitAmount;
                arrParams[4] = new OracleParameter("P_CURRENT_CL_END_DATE", OracleDbType.Varchar2, 100);
                if (creditLimitDOM.Insurance.CurrentCreditLimitEndDate == null)
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = Convert.ToDateTime(creditLimitDOM.Insurance.CurrentCreditLimitEndDate).ToString("dd-MMM-yyyy");
                }
                arrParams[5] = new OracleParameter("P_REQUEST_CL_AMOUNT", OracleDbType.Varchar2, 100);
                arrParams[5].Value = creditLimitDOM.Insurance.CreditLimitRequestAmount;
                arrParams[6] = new OracleParameter("P_INSURANCE", OracleDbType.Varchar2, 500);
                arrParams[6].Value = creditLimitDOM.Insurance.InsuranceName;
                arrParams[7] = new OracleParameter("P_INSURANCE_CODE", OracleDbType.Varchar2, 100);
                arrParams[7].Value = creditLimitDOM.Insurance.InsuranceCode;
                arrParams[8] = new OracleParameter("P_REMARKS", OracleDbType.Varchar2, 4000);
                arrParams[8].Value = creditLimitDOM.Remarks;
                arrParams[9] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[9].Value = creditLimitDOM.CreatedBy;
                arrParams[10] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[10].Direction = ParameterDirection.Output;
                arrParams[11] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[11].Direction = ParameterDirection.Output;
                arrParams[12] = new OracleParameter("P_OUT_REQ_ID", OracleDbType.Varchar2, 100);
                arrParams[12].Direction = ParameterDirection.Output;
                arrParams[13] = new OracleParameter("P_PROCESS_TYPE", OracleDbType.Varchar2, 100);
                arrParams[13].Value = "REQUEST";
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_REQUEST_SAVE, arrParams, DealerNet_Next_Connection);
                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[10].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[11].Value)
                };

                if (response.Status == Utilities.SUCCESS)
                {
                    Int64 CLReqId = SafeTypeHandling.ConvertStringToInt64(arrParams[12].Value);

                    // Future Sales Plan Save
                    foreach (var futureSalePlan in creditLimitDOM.FutureSalesPlan)
                    {
                        try
                        {
                            OracleParameter[] arrParamsFutureSales = new OracleParameter[7];
                            arrParamsFutureSales[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int64);
                            arrParamsFutureSales[0].Value = CLReqId;
                            arrParamsFutureSales[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[1].Value = creditLimitDOM.Distributor.HeaderCode;
                            arrParamsFutureSales[2] = new OracleParameter("P_MONTH_YEAR", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[2].Value = futureSalePlan.MonthYear;
                            arrParamsFutureSales[3] = new OracleParameter("P_SALE_AMOUNT", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[3].Value = futureSalePlan.SaleAmount;
                            arrParamsFutureSales[4] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[4].Value = creditLimitDOM.CreatedBy;
                            arrParamsFutureSales[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[5].Direction = ParameterDirection.Output;
                            arrParamsFutureSales[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                            arrParamsFutureSales[6].Direction = ParameterDirection.Output;
                            DataTable dtFutureSales = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.FUTURE_SALES_PLAN_SAVE, arrParamsFutureSales, DealerNet_Next_Connection);
                            APIResponse responseFSales = new APIResponse
                            {
                                Status = SafeTypeHandling.ConvertToString(arrParamsFutureSales[5].Value),
                                StatusDesc = SafeTypeHandling.ConvertToString(arrParamsFutureSales[6].Value)
                            };
                        }
                        catch (Exception ex)
                        {
                            Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                        }
                    }

                    //Financial Statements Save
                    foreach (var fystatement in creditLimitDOM.FYAttachment)
                    {
                        try
                        {
                            OracleParameter[] arrParamsFS = new OracleParameter[7];
                            arrParamsFS[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int64);
                            arrParamsFS[0].Value = CLReqId;
                            arrParamsFS[1] = new OracleParameter("P_FINANCIAL_YEAR", OracleDbType.Varchar2, 100);
                            arrParamsFS[1].Value = fystatement.FinancialYear;
                            arrParamsFS[2] = new OracleParameter("P_FY_STATUS", OracleDbType.Varchar2, 100);
                            arrParamsFS[2].Value = fystatement.FYStatusId;
                            arrParamsFS[3] = new OracleParameter("P_FILE_NAME_PATH", OracleDbType.Varchar2, 1000);
                            arrParamsFS[3].Value = fystatement.FileNamePath;
                            arrParamsFS[4] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                            arrParamsFS[4].Value = creditLimitDOM.CreatedBy;
                            arrParamsFS[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                            arrParamsFS[5].Direction = ParameterDirection.Output;
                            arrParamsFS[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                            arrParamsFS[6].Direction = ParameterDirection.Output;
                            DataTable dtFutureSales = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_REQUEST_FINANCIAL_STATEMENT_SAVE, arrParamsFS, DealerNet_Next_Connection);
                            APIResponse responseFStatement = new APIResponse
                            {
                                Status = SafeTypeHandling.ConvertToString(arrParamsFS[5].Value),
                                StatusDesc = SafeTypeHandling.ConvertToString(arrParamsFS[6].Value)
                            };
                        }
                        catch (Exception ex)
                        {
                            Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                        }
                    }

                    // Notes Save
                    foreach (var note in creditLimitDOM.Notes)
                    {
                        try
                        {
                            OracleParameter[] arrParamsNote = new OracleParameter[6];
                            arrParamsNote[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int64);
                            arrParamsNote[0].Value = CLReqId;
                            arrParamsNote[1] = new OracleParameter("P_NOTE_ID", OracleDbType.Varchar2, 100);
                            arrParamsNote[1].Value = note.Id;
                            arrParamsNote[2] = new OracleParameter("P_NOTE", OracleDbType.Varchar2, 1000);
                            arrParamsNote[2].Value = note.Note;
                            arrParamsNote[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                            arrParamsNote[3].Value = creditLimitDOM.CreatedBy;
                            arrParamsNote[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                            arrParamsNote[4].Direction = ParameterDirection.Output;
                            arrParamsNote[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                            arrParamsNote[5].Direction = ParameterDirection.Output;
                            DataTable dtFutureSales = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_REQUEST_NOTES_SAVE, arrParamsNote, DealerNet_Next_Connection);
                            APIResponse responseFSales = new APIResponse
                            {
                                Status = SafeTypeHandling.ConvertToString(arrParamsNote[4].Value),
                                StatusDesc = SafeTypeHandling.ConvertToString(arrParamsNote[5].Value)
                            };
                        }
                        catch (Exception ex)
                        {
                            Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                        }
                    }

                }
                return response;
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
        }

        /// <summary>
        /// Credit Limit Request Bulk Upload
        /// </summary>
        /// <param name="creditLimits"></param>
        /// <returns></returns>
        public async Task<APIResponse> CreditLimitRequestBulk(List<ARCreditLimitDOM> lstCreditLimits)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;
            try
            {
                int rowindex = 0;
                // Bulk Request Save in Temp Table
                foreach (var creditLimitRequest in lstCreditLimits)
                {
                    try
                    {
                        arrParams = new OracleParameter[8];
                        arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                        arrParams[0].Value = creditLimitRequest.Distributor.HeaderCode;
                        arrParams[1] = new OracleParameter("P_REQUEST_CL_AMOUNT", OracleDbType.Varchar2, 100);
                        arrParams[1].Value = creditLimitRequest.Insurance.CreditLimitRequestAmount;
                        arrParams[2] = new OracleParameter("P_REMARKS", OracleDbType.Varchar2, 4000);
                        arrParams[2].Value = creditLimitRequest.Remarks;
                        arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                        arrParams[3].Value = creditLimitRequest.CreatedBy;
                        arrParams[4] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Varchar2, 100);
                        arrParams[4].Value = creditLimitRequest.RowNumber;
                        arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                        arrParams[5].Direction = ParameterDirection.Output;
                        arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                        arrParams[6].Direction = ParameterDirection.Output;
                        arrParams[7] = new OracleParameter("P_OUT_TEMP_REQ_ID", OracleDbType.Varchar2, 100);
                        arrParams[7].Direction = ParameterDirection.Output;
                        DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_BULK_REQUEST_TEMP_SAVE, arrParams, DealerNet_Next_Connection);
                        string status = SafeTypeHandling.ConvertToString(arrParams[5].Value);
                        string statusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value);
                        string bulkTempReqId = SafeTypeHandling.ConvertToString(arrParams[7].Value);

                        // Check Response
                        if (status.Equals(Utilities.ERROR))
                        {
                            errors.Add(new Errors { Error = string.Concat("Row No. ", creditLimitRequest.RowNumber, ": ", statusDesc) });
                        }
                        else
                        {
                            lstCreditLimits[rowindex].ReqId = Convert.ToInt64(bulkTempReqId);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new Errors { Error = string.Concat("Row No. ", creditLimitRequest.RowNumber, ": ", ex.ToString()) });
                    }
                    rowindex++;
                }

                // if Error Found while data insert into Temp table
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

                Int32 createdBy = (Int32)lstCreditLimits[0].CreatedBy;
                // Validate Records
                try
                {
                    arrParams = new OracleParameter[2];
                    arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = createdBy;
                    arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                    arrParams[1].Direction = ParameterDirection.Output;
                    DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_BULK_REQUEST_TEMP_DATA_VALIDATE, arrParams, DealerNet_Next_Connection);
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
                            StatusDesc = Utilities.DATA_VALIDATION_FAILED,
                            data = errors
                        };
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = ex.ToString(),
                    };
                    return response;
                }

                // No error found, Future Sales Plan Details Insert in Temp Table.
                foreach (var creditLimitRequest in lstCreditLimits)
                {
                    // Future Sales Plan Save
                    foreach (var futureSalePlan in creditLimitRequest.FutureSalesPlan)
                    {
                        try
                        {
                            OracleParameter[] arrParamsFutureSales = new OracleParameter[8];
                            arrParamsFutureSales[0] = new OracleParameter("P_BULK_TEMP_REQ_ID", OracleDbType.Int64);
                            arrParamsFutureSales[0].Value = creditLimitRequest.ReqId; // Here is Temprory Id By Bulk Upload
                            arrParamsFutureSales[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[1].Value = creditLimitRequest.Distributor.HeaderCode;
                            arrParamsFutureSales[2] = new OracleParameter("P_MONTH_YEAR", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[2].Value = futureSalePlan.MonthYear;
                            arrParamsFutureSales[3] = new OracleParameter("P_SALE_AMOUNT", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[3].Value = futureSalePlan.SaleAmount;
                            arrParamsFutureSales[4] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[4].Value = creditLimitRequest.CreatedBy;
                            arrParamsFutureSales[5] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[5].Value = creditLimitRequest.RowNumber;
                            arrParamsFutureSales[6] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                            arrParamsFutureSales[6].Direction = ParameterDirection.Output;
                            arrParamsFutureSales[7] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                            arrParamsFutureSales[7].Direction = ParameterDirection.Output;
                            DataTable dtFutureSales = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_BULK_REQUEST_FUTURE_SALES_TEMP_SAVE, arrParamsFutureSales, DealerNet_Next_Connection);
                            string status = SafeTypeHandling.ConvertToString(arrParamsFutureSales[6].Value);
                            string statusDesc = SafeTypeHandling.ConvertToString(arrParamsFutureSales[7].Value);

                            // Check Response
                            if (status.Equals(Utilities.ERROR))
                            {
                                errors.Add(new Errors { Error = string.Concat("Row No. ", creditLimitRequest.RowNumber, ": ", statusDesc) });
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(new Errors { Error = string.Concat("Row No. ", creditLimitRequest.RowNumber, ": ", ex.ToString()) });
                        }
                    }
                }

                // if Error Found while data insert into Temp table, Future Sales Plan
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


                // Finally records submit in Main Table
                try
                {
                    arrParams = new OracleParameter[2];
                    arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = createdBy;
                    arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                    arrParams[1].Direction = ParameterDirection.Output;
                    DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_BULK_REQUEST_TEMP_DATA_FINAL_SUBMIT, arrParams, DealerNet_Next_Connection);
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
                    Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = ex.ToString(),
                    };
                    return response;
                }

            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
        }

        /// <summary>
        /// Read All Credit Limit Requests
        /// </summary>
        /// <param name="creditLimitDOM"></param>
        /// <returns></returns>
        public async Task<List<ARCreditLimitDOM>> ReadAllRequest(ARCreditLimitDOM creditLimitDOM)
        {
            List<ARCreditLimitDOM> lstAllRequest = new List<ARCreditLimitDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[9];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int32);
                if (creditLimitDOM.ReqId == null || creditLimitDOM.ReqId == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = creditLimitDOM.ReqId;
                }
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(creditLimitDOM.Distributor.HeaderCode))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = creditLimitDOM.Distributor.HeaderCode;
                }
                arrParams[2] = new OracleParameter("P_CTRL_AU", OracleDbType.Varchar2, 500);
                if (string.IsNullOrEmpty(creditLimitDOM.Distributor.CTRL_AU))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = creditLimitDOM.Distributor.CTRL_AU;
                }
                arrParams[3] = new OracleParameter("P_FROM_DATE", OracleDbType.Varchar2, 100);
                if (creditLimitDOM.FromDate == null)
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = Convert.ToDateTime(creditLimitDOM.FromDate).ToString("dd-MMM-yyyy");
                }
                arrParams[4] = new OracleParameter("P_TO_DATE", OracleDbType.Varchar2, 100);
                if (creditLimitDOM.ToDate == null)
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = Convert.ToDateTime(creditLimitDOM.ToDate).ToString("dd-MMM-yyyy");
                }
                arrParams[5] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 200);
                if (string.IsNullOrEmpty(creditLimitDOM.Status))
                {
                    arrParams[5].Value = DBNull.Value;
                }
                else
                {
                    arrParams[5].Value = creditLimitDOM.Status;
                }
                arrParams[6] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[6].Direction = ParameterDirection.Output;
                arrParams[7] = new OracleParameter("P_EMAIL_SENT", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(creditLimitDOM.EmailSent))
                {
                    arrParams[7].Value = DBNull.Value;
                }
                else
                {
                    arrParams[7].Value = creditLimitDOM.EmailSent;
                }
                arrParams[8] = new OracleParameter("P_PROCESS_TYPE", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(creditLimitDOM.ProcessType))
                {
                    arrParams[8].Value = DBNull.Value;
                }
                else
                {
                    arrParams[8].Value = creditLimitDOM.ProcessType;
                }
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_ALL_REQUEST_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitDOM clRequest = new ARCreditLimitDOM();
                    clRequest.ReqId = SafeTypeHandling.ConvertStringToInt32(row["REQ_ID"]);
                    clRequest.Distributor.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    clRequest.Distributor.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    clRequest.Distributor.CTRL_AU = SafeTypeHandling.ConvertToString(row["CTRL_AU"]);
                    clRequest.Insurance.InsuranceCode = SafeTypeHandling.ConvertToString(row["INSURANCE_CODE"]);
                    clRequest.Insurance.InsuranceName = SafeTypeHandling.ConvertToString(row["INSURANCE"]);
                    if (!string.IsNullOrEmpty(SafeTypeHandling.ConvertToString(row["CURRENT_CL_AMOUNT"])))
                    {
                        clRequest.Insurance.CurrentCreditLimitAmount = SafeTypeHandling.ConvertStringToDecimal(row["CURRENT_CL_AMOUNT"]);
                    }
                    if (!string.IsNullOrEmpty(SafeTypeHandling.ConvertToString(row["REQUEST_CL_AMOUNT"])))
                    {
                        clRequest.Insurance.CreditLimitRequestAmount = SafeTypeHandling.ConvertStringToDecimal(row["REQUEST_CL_AMOUNT"]);
                    }
                    if (!string.IsNullOrEmpty(SafeTypeHandling.ConvertToString(row["CURRENT_CL_END_DATE"])))
                    {
                        clRequest.Insurance.CurrentCreditLimitEndDate = SafeTypeHandling.ConvertToDateTime(row["CURRENT_CL_END_DATE"]);
                    }
                    if (!string.IsNullOrEmpty(SafeTypeHandling.ConvertToString(row["NEW_LIMIT_AMOUNT"])))
                    {
                        clRequest.Insurance.CreditLimitRequestAmountApproved = SafeTypeHandling.ConvertStringToDecimal(row["NEW_LIMIT_AMOUNT"]);
                    }
                    clRequest.ProcessType = SafeTypeHandling.ConvertToString(row["PROCESS_TYPE"]);
                    clRequest.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    clRequest.Remarks = SafeTypeHandling.ConvertToString(row["REMARKS"]);
                    clRequest.AdminRemarks = SafeTypeHandling.ConvertToString(row["APPROVAL_REMARK"]);
                    clRequest.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    clRequest.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY_NAME"]);
                    clRequest.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);

                    if (SafeTypeHandling.ConvertStringToInt64(creditLimitDOM.ReqId) > 0)
                    {
                        Distributor distributor = new Distributor { Id = creditLimitDOM.ReqId, HeaderCode = creditLimitDOM.Distributor.HeaderCode };
                        clRequest.SalesHistory = ReadSalesHistory(distributor);
                        clRequest.Sales_PaymentTrend = ReadSales_PaymentTrend(distributor);
                        clRequest.OD_History = ReadOD_History(distributor);
                        clRequest.FutureSalesPlan = ReadFutureSalesPlan(distributor);
                        clRequest.FYAttachment = ReadFinancialYearAttachment(distributor);
                        clRequest.Notes = ReadNotes(distributor);
                    }
                    lstAllRequest.Add(clRequest);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
            }
            return lstAllRequest;
        }


        /// <summary>
        /// All Credit Limit Requests Approval
        /// </summary>
        /// <param name="creditLimitRequests"></param>
        /// <returns></returns>
        public async Task<APIResponse> RequestApproval(List<ARCreditLimitDOM> lstCreditLimitRequests)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (var creditLimit in lstCreditLimitRequests)
            {
                try
                {
                    arrParams = new OracleParameter[10];
                    arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = creditLimit.ReqId;
                    arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = creditLimit.Distributor.HeaderCode;
                    arrParams[2] = new OracleParameter("P_CTRL_AU", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = creditLimit.Distributor.CTRL_AU;
                    arrParams[3] = new OracleParameter("P_NEW_LIMIT", OracleDbType.Decimal);
                    arrParams[3].Value = creditLimit.Insurance.CreditLimitRequestAmountApproved;
                    arrParams[4] = new OracleParameter("P_APPROVAL_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[4].Value = creditLimit.Status;
                    arrParams[5] = new OracleParameter("P_APPROVAL_REMARK", OracleDbType.Varchar2, 500);
                    arrParams[5].Value = creditLimit.AdminRemarks;
                    arrParams[6] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[6].Value = creditLimit.RowNumber;
                    arrParams[7] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[7].Value = creditLimit.CreatedBy;
                    arrParams[8] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[8].Direction = ParameterDirection.Output;
                    arrParams[9] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[9].Direction = ParameterDirection.Output;

                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_APPROVAL_UPLOAD_TEMP_INSERT, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[8].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[9].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", creditLimit.RowNumber, ": ", ex.ToString()) });
                    Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
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

            Int32 createdBy = (Int32)lstCreditLimitRequests[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_APPROVAL_UPLOAD_TEMP_VALIDATE, arrParams, DealerNet_Next_Connection);
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
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
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
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_APPROVAL_UPLOAD_FINAL, arrParams, DealerNet_Next_Connection);
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
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return response;
            }

        }


        public void ReadEmailForCLRequest(Int64 ReqId, string HeaderCode, out string EmailTo, out string EmailCC)
        {
            EmailTo = string.Empty;
            EmailCC = string.Empty;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int64);
                arrParams[0].Value = ReqId;
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = HeaderCode;
                arrParams[2] = new OracleParameter("P_EMAIL_TO", OracleDbType.Varchar2, 500);
                arrParams[2].Direction = ParameterDirection.Output;
                arrParams[3] = new OracleParameter("P_EMAIL_CC", OracleDbType.Varchar2, 500);
                arrParams[3].Direction = ParameterDirection.Output;
                arrParams[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;
                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_EMAIL_READ_FOR_REQUEST, arrParams, DealerNet_Next_Connection);
                if (SafeTypeHandling.ConvertToString(arrParams[4].Value) == Utilities.SUCCESS)
                {
                    EmailTo = SafeTypeHandling.ConvertToString(arrParams[2].Value);
                    EmailCC = SafeTypeHandling.ConvertToString(arrParams[3].Value);
                }
                else
                {
                    Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, SafeTypeHandling.ConvertToString(arrParams[5].Value));
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
            }
        }

        public async Task<APIResponse> EmailSendLog(Int64 ReqId, string HeaderCode, string EmailTo, string EmailCC)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_REQ_ID", OracleDbType.Int64);
                arrParams[0].Value = ReqId;
                arrParams[1] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = HeaderCode;
                arrParams[2] = new OracleParameter("P_EMAIL_TO", OracleDbType.Varchar2, 500);
                arrParams[2].Value = EmailTo;
                arrParams[3] = new OracleParameter("P_EMAIL_CC", OracleDbType.Varchar2, 500);
                arrParams[3].Value = EmailCC;
                arrParams[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.CREDIT_LIMIT_EMAIL_SENT_LOG_INSERT, arrParams, DealerNet_Next_Connection);
                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[4].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[5].Value)
                };
                return response;
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
        }


    }
}
