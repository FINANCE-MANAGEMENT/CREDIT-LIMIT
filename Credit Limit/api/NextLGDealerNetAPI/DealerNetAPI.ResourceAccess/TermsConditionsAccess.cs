using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess
{
    public class TermsConditionsAccess : ITermsConditionsAccess
    {
        private readonly ICommonDB _commonDB = null;

        public TermsConditionsAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        public async Task<List<SchemeTypes>> ReadSchemeTypes(string SystemName, Int32 TnCID)
        {
            List<SchemeTypes> lstSchemeTypes = new List<SchemeTypes>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_SYSTEM_NAME", OracleDbType.Varchar2);
                arrParams[0].Value = SystemName;
                arrParams[1] = new OracleParameter("P_ID", OracleDbType.Varchar2);
                arrParams[1].Value = TnCID;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Terms_Conditions_Master.READ_SCHEME_TYPES, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    SchemeTypes schemeTypes = new SchemeTypes();
                    schemeTypes.SchemeType = SafeTypeHandling.ConvertToString(row["SCHEME_TYPE"]);
                    schemeTypes.isChecked = SafeTypeHandling.ConvertStringToBoolean(row["CHECKED_STATUS"]);
                    lstSchemeTypes.Add(schemeTypes);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstSchemeTypes;
        }

        public async Task<APIResponse> SaveTermsConditions(TermsConditions termsConditions)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[8];
                arrParams[0] = new OracleParameter("P_ID", OracleDbType.Varchar2);
                arrParams[0].Value = termsConditions.Id;
                arrParams[1] = new OracleParameter("P_TERMS_AND_CONDITIONS", OracleDbType.Varchar2);
                arrParams[1].Value = termsConditions.TermCondition;
                arrParams[2] = new OracleParameter("P_SCHEME_TYPE", OracleDbType.Varchar2);
                arrParams[2].Value = termsConditions.SchemeType;
                arrParams[3] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[3].Value = termsConditions.Status;
                arrParams[4] = new OracleParameter("P_SYSTEM_NAME", OracleDbType.Varchar2);
                arrParams[4].Value = termsConditions.SystemName;
                arrParams[5] = new OracleParameter("P_USERID", OracleDbType.Varchar2);
                arrParams[5].Value = termsConditions.CreatedBy;
                arrParams[6] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[6].Direction = ParameterDirection.Output;
                arrParams[7] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[7].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Terms_Conditions_Master.INSERT_UPDATE, arrParams);
                apiResponse.Status = arrParams[6].Value.ToString();
                apiResponse.StatusDesc = arrParams[7].Value.ToString();
            }
            catch (Exception ex)
            {
                apiResponse.Status = Utilities.ERROR;
                apiResponse.StatusDesc = ex.ToString();
                throw;
            }
            return apiResponse;
        }
        public async Task<List<TermsConditions>> ReadTermsConditions(TermsConditions termsConditions)
        {
            List<TermsConditions> lstTermsConditions = new List<TermsConditions>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_SYSTEM_NAME", OracleDbType.Varchar2);
                arrParams[0].Value = termsConditions.SystemName;
                arrParams[1] = new OracleParameter("P_ID", OracleDbType.Varchar2);
                if (termsConditions.Id == 0 || string.IsNullOrEmpty(Convert.ToString(termsConditions.Id)))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = termsConditions.Id;
                }
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                arrParams[3] = new OracleParameter("P_SCHEME_TYPE", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(termsConditions.SchemeType)))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = termsConditions.SchemeType;
                }
                arrParams[4] = new OracleParameter("P_SCHEME_REF_NO", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(termsConditions.SchemeRefNo)))
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = termsConditions.SchemeRefNo;
                }

                arrParams[5] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(termsConditions.Status)))
                {
                    arrParams[5].Value = DBNull.Value;
                }
                else
                {
                    arrParams[5].Value = termsConditions.Status;
                }

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Terms_Conditions_Master.READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    TermsConditions tnc = new TermsConditions();
                    tnc.Id = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    tnc.TermCondition = SafeTypeHandling.ConvertToString(row["TERMS_AND_CONDITIONS"]);
                    tnc.SchemeType = SafeTypeHandling.ConvertToString(row["SCHEME_TYPE"]);
                    tnc.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    tnc.SystemName = SafeTypeHandling.ConvertToString(row["SYSTEM_NAME"]);
                    tnc.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    tnc.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    tnc.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    tnc.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    tnc.isChecked = SafeTypeHandling.ConvertStringToBoolean(row["IS_USED"]);
                    lstTermsConditions.Add(tnc);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstTermsConditions;
        }

    }
}
