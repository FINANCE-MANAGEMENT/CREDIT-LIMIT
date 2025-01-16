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
    public class UtilitiesARCreditLimitAccess : IUtilitiesARCreditLimitAccess
    {
        private readonly ICommonDB _commonDB = null;
        private readonly string DealerNet_Next_Connection = "DealerNetNextConnection";

        public UtilitiesARCreditLimitAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        public async Task<List<Branch>> ReadBranch()
        {
            List<Branch> lstBranch = new List<Branch>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[1];
                arrParams[0] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[0].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.CreditLimit.READ_BRANCH, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Branch _branch = new Branch();
                    _branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    //_branch.BranchAddress = SafeTypeHandling.ConvertToString(row["BRANCH_ADDRESS"]);
                    lstBranch.Add(_branch);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstBranch;
        }

        /// <summary>
        /// Save Notes Master
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task<APIResponse> NoteSave(Notes notes)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[7];
                arrParams[0] = new OracleParameter("P_ID", OracleDbType.Int64);
                if (notes.Id == 0 || string.IsNullOrEmpty(Convert.ToString(notes.Id)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = notes.Id;
                }
                arrParams[1] = new OracleParameter("P_NOTES", OracleDbType.Varchar2, 100);
                arrParams[1].Value = notes.Note;
                arrParams[2] = new OracleParameter("P_ORDER_NO", OracleDbType.Int32);
                arrParams[2].Value = notes.OrderNo;
                arrParams[3] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 100);
                arrParams[3].Value = notes.Status;
                arrParams[4] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[4].Value = notes.CreatedBy;
                arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;
                arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.SAVE_UPDATE_NOTES_MASTER, arrParams, DealerNet_Next_Connection);

                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[5].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[6].Value)
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
        /// <summary>
        /// Read Notes Master
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task<List<Notes>> ReadNotes(Notes notes)
        {
            List<Notes> lstNotes = new List<Notes>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_ID", OracleDbType.Varchar2, 100);
                if (notes.Id == 0 || string.IsNullOrEmpty(Convert.ToString(notes.Id)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = notes.Id;
                }
                arrParams[1] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 100);
                arrParams[1].Value = notes.Status;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.READ_NOTES_MASTER, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    Notes note = new Notes();
                    note.Id = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    note.Note = SafeTypeHandling.ConvertToString(row["NOTE"]);
                    note.OrderNo = SafeTypeHandling.ConvertStringToInt32(row["ORDER_NO"]);
                    note.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    note.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    note.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPDATED_BY"]);
                    note.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY_NAME"]);
                    note.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
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
        /// Read All Master Updated
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> MastersUpdatedRead()
        {
            DataTable dtData = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[1];
                arrParams[0] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[0].Direction = ParameterDirection.Output;
                dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.ALL_MASTERS_UPDATED_LIST_READ, arrParams, DealerNet_Next_Connection);
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return dtData;
        }

        /// <summary>
        /// Read Upload Logs for All
        /// </summary>
        /// <param name="ProcessType"></param>
        /// <returns></returns>
        public async Task<List<ARCreditLimitSalesDOM>> UploadFilesLogRead(string ProcessType)
        {
            List<ARCreditLimitSalesDOM> lstLog = new List<ARCreditLimitSalesDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_PROCESS_TYPE", OracleDbType.Varchar2, 100);
                arrParams[0].Value = ProcessType;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.UPLOAD_FILES_LOG_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitSalesDOM log = new ARCreditLimitSalesDOM();
                    log.Id = SafeTypeHandling.ConvertStringToInt64(row["ID"]);
                    log.ProcessType = SafeTypeHandling.ConvertToString(row["PROCESS_TYPE"]);
                    log.FileNamePath = SafeTypeHandling.ConvertToString(row["FILE_NAME_PATH"]);
                    log.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    log.Remarks = SafeTypeHandling.ConvertToString(row["REMARKS"]);
                    log.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPDATED_BY"]);
                    log.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY_NAME"]);
                    log.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    lstLog.Add(log);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstLog;
        }

        /// <summary>
        /// Sales data Upload
        /// </summary>
        /// <param name="sales"></param>
        /// <returns></returns>
        public async Task<APIResponse> SalesUpload(List<ARCreditLimitSalesDOM> lstSales)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (var sale in lstSales)
            {
                try
                {
                    arrParams = new OracleParameter[8];
                    arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = sale.HeaderCode;
                    arrParams[1] = new OracleParameter("P_MONTH_YEAR", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = sale.MonthYear;
                    arrParams[2] = new OracleParameter("P_SALE_AMT", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = sale.SaleAmount;
                    arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[3].Value = sale.CreatedBy;
                    arrParams[4] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[4].Value = sale.RowNumber;
                    arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[5].Direction = ParameterDirection.Output;
                    arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[6].Direction = ParameterDirection.Output;
                    arrParams[7] = new OracleParameter("P_FILE_NAME_PATH", OracleDbType.Varchar2, 500);
                    arrParams[7].Value = sale.FileNamePath;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.SALES_UPLOAD_TEMP_INSERT, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[5].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[6].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", sale.RowNumber, ": ", ex.ToString()) });
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

            Int32 createdBy = (Int32)lstSales[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.SALES_UPLOAD_TEMP_VALIDATE, arrParams, DealerNet_Next_Connection);
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
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.SALES_UPLOAD_FINAL, arrParams, DealerNet_Next_Connection);
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
        /// <summary>
        /// Sales Data Upload Read
        /// </summary>
        /// <param name="sale"></param>
        /// <returns></returns>
        public async Task<List<ARCreditLimitSalesDOM>> ReadSalesData(ARCreditLimitSalesDOM sale)
        {
            List<ARCreditLimitSalesDOM> lstSales = new List<ARCreditLimitSalesDOM>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(sale.HeaderCode))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = sale.HeaderCode;
                }
                arrParams[1] = new OracleParameter("P_MONTH_YEAR", OracleDbType.Varchar2, 1000);
                if (string.IsNullOrEmpty(sale.MonthYear))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = sale.MonthYear;
                }
                arrParams[2] = new OracleParameter("P_DATA_SOURCE", OracleDbType.Varchar2, 100);
                arrParams[2].Value = sale.DataSource;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.SALES_UPLOAD_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitSalesDOM _sale = new ARCreditLimitSalesDOM();
                    _sale.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    _sale.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    _sale.MonthYear = SafeTypeHandling.ConvertToString(row["MONTH_YEAR"]);
                    _sale.SaleAmount = SafeTypeHandling.ConvertStringToDecimal(row["SALE_AMOUNT"]);
                    _sale.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _sale.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY_NAME"]);
                    lstSales.Add(_sale);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstSales;
        }


        /// <summary>
        /// Insurance data Upload
        /// </summary>
        /// <param name="insurances"></param>
        /// <returns></returns>
        public async Task<APIResponse> InsuranceUpload(List<ARCreditLimitInsurance> lstInsurance)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (var insurance in lstInsurance)
            {
                try
                {
                    arrParams = new OracleParameter[10];
                    arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = insurance.HeaderCode;
                    arrParams[1] = new OracleParameter("P_INSURANCE_CODE", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = insurance.InsuranceCode;
                    arrParams[2] = new OracleParameter("P_INSURNACE_NAME", OracleDbType.Varchar2, 500);
                    arrParams[2].Value = insurance.InsuranceName;
                    arrParams[3] = new OracleParameter("P_CURRENT_LIMIT", OracleDbType.Decimal);
                    arrParams[3].Value = insurance.CurrentCreditLimitAmount;
                    arrParams[4] = new OracleParameter("P_CURRENT_LIMIT_END_DATE", OracleDbType.Date);
                    arrParams[4].Value = insurance.CurrentCreditLimitEndDate;
                    arrParams[5] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[5].Value = insurance.CreatedBy;
                    arrParams[6] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[6].Value = insurance.RowNumber;
                    arrParams[7] = new OracleParameter("P_FILE_NAME_PATH", OracleDbType.Varchar2, 500);
                    arrParams[7].Value = insurance.FileNamePath;
                    arrParams[8] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[8].Direction = ParameterDirection.Output;
                    arrParams[9] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[9].Direction = ParameterDirection.Output;

                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.INSURNACE_UPLOAD_TEMP_INSERT, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[8].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[9].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", insurance.RowNumber, ": ", ex.ToString()) });
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

            Int32 createdBy = (Int32)lstInsurance[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.INSURNACE_UPLOAD_TEMP_VALIDATE, arrParams, DealerNet_Next_Connection);
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
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.INSURNACE_UPLOAD_FINAL, arrParams, DealerNet_Next_Connection);
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
        /// <summary>
        /// Insurance Data Upload Read
        /// </summary>
        /// <param name="insurance"></param>
        /// <returns></returns>
        public async Task<List<ARCreditLimitInsurance>> ReadInsuranceData(ARCreditLimitInsurance insurance)
        {
            List<ARCreditLimitInsurance> lstInsurance = new List<ARCreditLimitInsurance>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(insurance.HeaderCode))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = insurance.HeaderCode;
                }
                arrParams[1] = new OracleParameter("P_DATA_SOURCE", OracleDbType.Varchar2, 100);
                arrParams[1].Value = insurance.DataSource;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.INSURNACE_UPLOAD_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitInsurance _insurance = new ARCreditLimitInsurance();
                    _insurance.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    _insurance.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    _insurance.InsuranceCode = SafeTypeHandling.ConvertToString(row["INSURANCE_CODE"]);
                    _insurance.InsuranceName = SafeTypeHandling.ConvertToString(row["INSURANCE_NAME"]);
                    _insurance.CurrentCreditLimitAmount = SafeTypeHandling.ConvertStringToDecimal(row["CURRENT_LIMIT"]);
                    _insurance.CurrentCreditLimitEndDate = SafeTypeHandling.ConvertToDateTime(row["LIMIT_END_DATE"]);
                    _insurance.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _insurance.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY_NAME"]);
                    lstInsurance.Add(_insurance);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstInsurance;
        }


        /// <summary>
        /// OD data Upload
        /// </summary>
        /// <param name="lstOD"></param>
        /// <returns></returns>
        public async Task<APIResponse> ODUpload(List<ARCreditLimitOD> lstOD)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (var od in lstOD)
            {
                try
                {
                    arrParams = new OracleParameter[8];
                    arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = od.HeaderCode;
                    arrParams[1] = new OracleParameter("P_MONTH_YEAR", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = od.MonthYear;
                    arrParams[2] = new OracleParameter("P_OD_AMOUNT", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = od.ODAmount;
                    arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[3].Value = od.CreatedBy;
                    arrParams[4] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[4].Value = od.RowNumber;
                    arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[5].Direction = ParameterDirection.Output;
                    arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[6].Direction = ParameterDirection.Output;
                    arrParams[7] = new OracleParameter("P_FILE_NAME_PATH", OracleDbType.Varchar2, 500);
                    arrParams[7].Value = od.FileNamePath;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.OD_UPLOAD_TEMP_INSERT, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[5].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[6].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", od.RowNumber, ": ", ex.ToString()) });
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

            Int32 createdBy = (Int32)lstOD[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.OD_UPLOAD_TEMP_VALIDATE, arrParams, DealerNet_Next_Connection);
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
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.OD_UPLOAD_FINAL, arrParams, DealerNet_Next_Connection);
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
        /// <summary>
        /// OD Data Upload Read
        /// </summary>
        /// <param name="od"></param>
        /// <returns></returns>
        public async Task<List<ARCreditLimitOD>> ReadODData(ARCreditLimitOD od)
        {
            List<ARCreditLimitOD> lstOD = new List<ARCreditLimitOD>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(od.HeaderCode))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = od.HeaderCode;
                }
                arrParams[1] = new OracleParameter("P_MONTH_YEAR", OracleDbType.Varchar2, 1000);
                if (string.IsNullOrEmpty(od.MonthYear))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = od.MonthYear;
                }
                arrParams[2] = new OracleParameter("P_DATA_SOURCE", OracleDbType.Varchar2, 100);
                arrParams[2].Value = od.DataSource;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.OD_UPLOAD_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitOD _od = new ARCreditLimitOD();
                    _od.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    _od.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    _od.MonthYear = SafeTypeHandling.ConvertToString(row["MONTH_YEAR"]);
                    _od.ODAmount = SafeTypeHandling.ConvertStringToDecimal(row["OD_AMOUNT"]);
                    _od.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _od.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY_NAME"]);
                    lstOD.Add(_od);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstOD;
        }


        /// <summary>
        /// Collection data Upload
        /// </summary>
        /// <param name="lstCollection"></param>
        /// <returns></returns>
        public async Task<APIResponse> CollectionUpload(List<ARCreditLimitCollection> lstCollection)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (var collection in lstCollection)
            {
                try
                {
                    arrParams = new OracleParameter[8];
                    arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = collection.HeaderCode;
                    arrParams[1] = new OracleParameter("P_MONTH_YEAR", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = collection.MonthYear;
                    arrParams[2] = new OracleParameter("P_COLLECTION_AMOUNT", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = collection.CollectionAmount;
                    arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[3].Value = collection.CreatedBy;
                    arrParams[4] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[4].Value = collection.RowNumber;
                    arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[5].Direction = ParameterDirection.Output;
                    arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[6].Direction = ParameterDirection.Output;
                    arrParams[7] = new OracleParameter("P_FILE_NAME_PATH", OracleDbType.Varchar2, 500);
                    arrParams[7].Value = collection.FileNamePath;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.COLLECTION_UPLOAD_TEMP_INSERT, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[5].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[6].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", collection.RowNumber, ": ", ex.ToString()) });
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

            Int32 createdBy = (Int32)lstCollection[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.COLLECTION_UPLOAD_TEMP_VALIDATE, arrParams, DealerNet_Next_Connection);
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
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.COLLECTION_UPLOAD_FINAL, arrParams, DealerNet_Next_Connection);
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
        /// <summary>
        /// Collection Data Upload Read
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<List<ARCreditLimitCollection>> ReadCollectionData(ARCreditLimitCollection collection)
        {
            List<ARCreditLimitCollection> lstCollection = new List<ARCreditLimitCollection>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(collection.HeaderCode))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = collection.HeaderCode;
                }
                arrParams[1] = new OracleParameter("P_MONTH_YEAR", OracleDbType.Varchar2, 1000);
                if (string.IsNullOrEmpty(collection.MonthYear))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = collection.MonthYear;
                }
                arrParams[2] = new OracleParameter("P_DATA_SOURCE", OracleDbType.Varchar2, 100);
                arrParams[2].Value = collection.DataSource;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.COLLECTION_UPLOAD_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitCollection _collection = new ARCreditLimitCollection();
                    _collection.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    _collection.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    _collection.MonthYear = SafeTypeHandling.ConvertToString(row["MONTH_YEAR"]);
                    _collection.CollectionAmount = SafeTypeHandling.ConvertStringToDecimal(row["COLLECTION_AMOUNT"]);
                    _collection.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _collection.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY_NAME"]);
                    lstCollection.Add(_collection);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstCollection;
        }


        /// <summary>
        /// FY Status Master Save
        /// </summary>
        /// <param name="fYStatus"></param>
        /// <returns></returns>
        public async Task<APIResponse> FYStatusSave(ARCreditLimitFYStatus fYStatus)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[8];
                arrParams[0] = new OracleParameter("P_ID", OracleDbType.Int32);
                if (fYStatus.Id == 0 || string.IsNullOrEmpty(Convert.ToString(fYStatus.Id)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = fYStatus.Id;
                }
                arrParams[1] = new OracleParameter("P_FY_STATUS", OracleDbType.Varchar2, 100);
                arrParams[1].Value = fYStatus.FYStatusId;
                arrParams[2] = new OracleParameter("P_MANDATORY", OracleDbType.Varchar2, 100);
                arrParams[2].Value = fYStatus.MandatoryStatus;
                arrParams[3] = new OracleParameter("P_UPLOAD", OracleDbType.Varchar2, 100);
                arrParams[3].Value = fYStatus.UploadStatus;
                arrParams[4] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 100);
                arrParams[4].Value = fYStatus.Status;
                arrParams[5] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[5].Value = fYStatus.CreatedBy;
                arrParams[6] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[6].Direction = ParameterDirection.Output;
                arrParams[7] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[7].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.FY_STATUS_MASTER_SAVE_UPDATE, arrParams, DealerNet_Next_Connection);

                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[6].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[7].Value)
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

        /// <summary>
        /// Read FY Status Master
        /// </summary>
        /// <param name="fYStatus"></param>
        /// <returns></returns>
        public async Task<List<ARCreditLimitFYStatus>> FYStatusRead(ARCreditLimitFYStatus fYStatus)
        {
            List<ARCreditLimitFYStatus> lstFYStatus = new List<ARCreditLimitFYStatus>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_ID", OracleDbType.Varchar2, 100);
                if (fYStatus.Id == 0 || string.IsNullOrEmpty(Convert.ToString(fYStatus.Id)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = fYStatus.Id;
                }
                arrParams[1] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 100);
                arrParams[1].Value = fYStatus.Status;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.FY_STATUS_MASTER_SAVE_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitFYStatus _fYStatus = new ARCreditLimitFYStatus();
                    _fYStatus.Id = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    _fYStatus.FYStatusId = SafeTypeHandling.ConvertStringToInt32(row["FINANCIAL_YEAR_STATUS_ID"]);
                    _fYStatus.FYStatus = SafeTypeHandling.ConvertToString(row["FINANCIAL_YEAR_STATUS"]);
                    _fYStatus.MandatoryStatus = SafeTypeHandling.ConvertToString(row["MANDATORY"]);
                    _fYStatus.UploadStatus = SafeTypeHandling.ConvertToString(row["UPLOAD"]);
                    _fYStatus.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    _fYStatus.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    _fYStatus.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPDATED_BY"]);
                    _fYStatus.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY_NAME"]);
                    _fYStatus.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    lstFYStatus.Add(_fYStatus);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstFYStatus;
        }

        
        /// <summary>
        /// Financial Year Attachment data Upload
        /// </summary>
        /// <param name="fyAttachements"></param>
        /// <returns></returns>
        public async Task<APIResponse> FinancialYearAttachmentUpload(List<ARCreditLimitFYStatus> lstFYAttachement)
        {
            APIResponse response = null;
            List<Errors> errors = new List<Errors>();
            OracleParameter[] arrParams;

            // Data save in Temprory table
            foreach (var fyItem in lstFYAttachement)
            {
                try
                {
                    arrParams = new OracleParameter[8];
                    arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                    arrParams[0].Value = fyItem.HeaderCode;
                    arrParams[1] = new OracleParameter("P_FIN_YEAR", OracleDbType.Varchar2, 100);
                    arrParams[1].Value = fyItem.FinancialYear;
                    arrParams[2] = new OracleParameter("P_FIN_YEAR_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[2].Value = fyItem.FYStatus;
                    arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                    arrParams[3].Value = fyItem.CreatedBy;
                    arrParams[4] = new OracleParameter("P_ROW_NUMBER", OracleDbType.Int32);
                    arrParams[4].Value = fyItem.RowNumber;
                    arrParams[5] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                    arrParams[5].Direction = ParameterDirection.Output;
                    arrParams[6] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                    arrParams[6].Direction = ParameterDirection.Output;
                    arrParams[7] = new OracleParameter("P_FILE_NAME_PATH", OracleDbType.Varchar2, 500);
                    arrParams[7].Value = fyItem.FileNamePath;
                    DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.FY_ATTACHMENT_UPLOAD_TEMP_INSERT, arrParams, DealerNet_Next_Connection);
                    // Check Response
                    if (arrParams[5].Value.ToString().Equals(Utilities.ERROR))
                    {
                        errors.Add(new Errors { Error = arrParams[6].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(new Errors { Error = string.Concat("Row No. ", fyItem.RowNumber, ": ", ex.ToString()) });
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

            Int32 createdBy = (Int32)lstFYAttachement[0].CreatedBy;
            // Validate Records
            try
            {
                arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[0].Value = createdBy;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.FY_ATTACHMENT_UPLOAD_TEMP_VALIDATE, arrParams, DealerNet_Next_Connection);
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
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.FY_ATTACHMENT_UPLOAD_FINAL, arrParams, DealerNet_Next_Connection);
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
        /// <summary>
        /// Financial Year Attachment Data Upload Read
        /// </summary>
        /// <param name="fyAttachement"></param>
        /// <returns></returns>
        public async Task<List<ARCreditLimitFYStatus>> FinancialYearAttachmentRead(ARCreditLimitFYStatus fyAttachement)
        {
            List<ARCreditLimitFYStatus> lstFYAttachment = new List<ARCreditLimitFYStatus>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_HEADER_CODE", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(fyAttachement.HeaderCode))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = fyAttachement.HeaderCode;
                }
                arrParams[1] = new OracleParameter("P_FIN_YEAR", OracleDbType.Varchar2, 500);
                if (string.IsNullOrEmpty(fyAttachement.FinancialYear))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = fyAttachement.FinancialYear;
                }
                arrParams[2] = new OracleParameter("P_DATA_SOURCE", OracleDbType.Varchar2, 100);
                arrParams[2].Value = fyAttachement.DataSource;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ARCreditLimitSystem.Utilities.FY_ATTACHMENT_UPLOAD_READ, arrParams, DealerNet_Next_Connection);
                foreach (DataRow row in dtData.Rows)
                {
                    ARCreditLimitFYStatus _fyAttachement = new ARCreditLimitFYStatus();
                    _fyAttachement.HeaderCode = SafeTypeHandling.ConvertToString(row["HEADER_CODE"]);
                    _fyAttachement.AccountName = SafeTypeHandling.ConvertToString(row["ACCOUNT_NAME"]);
                    _fyAttachement.FinancialYear = SafeTypeHandling.ConvertToString(row["FINANCIAL_YEAR"]);
                    _fyAttachement.FYStatus = SafeTypeHandling.ConvertToString(row["FINANCIAL_YEAR_STATUS"]);
                    _fyAttachement.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    _fyAttachement.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY_NAME"]);
                    lstFYAttachment.Add(_fyAttachement);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.ARCreditLimit, ex);
                throw;
            }
            return lstFYAttachment;
        }



    }
}
