
using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using DealerNetAPI.ResourceAccess.Interface;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess
{
    public class BankAccess : IBankAccess
    {

        private readonly ICommonDB _commonDB = null;

        public BankAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }
        public async Task<APIResponse> SaveBank(Bank bank)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_BANK_ID", OracleDbType.Varchar2);
                arrParams[0].Value = bank.BankId;
                arrParams[1] = new OracleParameter("P_BANK_NAME", OracleDbType.Varchar2);
                arrParams[1].Value = bank.BankName;
                arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[2].Value = bank.Status;
                arrParams[3] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2);
                arrParams[3].Value = bank.CreatedBy;
                arrParams[4] = new OracleParameter("P_OUT", OracleDbType.Varchar2, 500);
                arrParams[4].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.Bank_Master.INSERT, arrParams);
                apiResponse.StatusDesc = arrParams[4].Value.ToString();
            }
            catch (Exception ex)
            {
                apiResponse.Status = Utilities.ERROR;
                apiResponse.StatusDesc = ex.ToString();
                throw;
            }
            return apiResponse;
        }
        public async Task<List<Bank>> ReadBank(Bank bank)
        {
            List<Bank> lstBank = new List<Bank>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[0].Value = bank.Status;
                arrParams[2] = new OracleParameter("P_BANK_ID", OracleDbType.Varchar2);
                arrParams[2].Value = bank.BankId;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;

                DataTable dtData =await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.Bank_Master.READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Bank objBank = new Bank();
                    //objDistrict.State.StateId = SafeTypeHandling.ConvertStringToInt32(row["STATE_ID"]);
                    objBank.BankId = SafeTypeHandling.ConvertStringToInt32(row["BANK_ID"]);
                    //objDistrict.DistrictCode = SafeTypeHandling.ConvertToString(row["DISTRICT_CODE"]);
                    objBank.BankName = SafeTypeHandling.ConvertToString(row["BANK_NAME"]);
                    objBank.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    objBank.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    objBank.CreatedBy = SafeTypeHandling.ConvertStringToInt32(row["CREATED_BY"]);
                    objBank.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    objBank.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPATED_BY"]);
                    objBank.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATE_DATE"]);
                    lstBank.Add(objBank);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstBank;
        }
    }
}













