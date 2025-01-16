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
    public class LookupAccess : ILookupAccess
    {
        private readonly ICommonDB _commonDB = null;

        public LookupAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }
        public async Task<APIResponse> SaveLookup(Lookup lookup)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[9];
                arrParams[0] = new OracleParameter("P_ID", OracleDbType.Varchar2);
                arrParams[0].Value = lookup.Id;
                arrParams[1] = new OracleParameter("P_LOOKUP_ID", OracleDbType.Varchar2);
                arrParams[1].Value = lookup.LookupId;
                arrParams[2] = new OracleParameter("P_LOOKUP_VALUE", OracleDbType.Varchar2);
                arrParams[2].Value = lookup.LookupValue;
                arrParams[3] = new OracleParameter("P_LOOKUP_DESC", OracleDbType.Varchar2);
                arrParams[3].Value = lookup.LookupDesc;
                arrParams[4] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[4].Value = lookup.Status;
                arrParams[5] = new OracleParameter("P_DISP_SEQ", OracleDbType.Varchar2);
                arrParams[5].Value = lookup.LookupDispSeq;
                arrParams[6] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                arrParams[6].Value = lookup.CreatedBy;
                arrParams[7] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 500);
                arrParams[7].Direction = ParameterDirection.Output;
                arrParams[8] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[8].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Lookup_Master.INSERT_UPDATE, arrParams);
                response = new APIResponse
                {
                    Status = arrParams[7].Value.ToString(),
                    StatusDesc = arrParams[8].Value.ToString()
                };
            }
            catch (Exception ex)
            {
                response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
                throw;
            }
            return response;
        }

        public async Task<List<Lookup>> ReadLookup(Lookup lookup)
        {
            List<Lookup> lstLookup = new List<Lookup>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_ID", OracleDbType.Varchar2);
                if (lookup.Id == 0 || string.IsNullOrEmpty(Convert.ToString(lookup.Id)))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = lookup.Id;
                }
                arrParams[1] = new OracleParameter("LOOKUP_ID", OracleDbType.Varchar2);
                if (lookup.LookupId == 0)
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = lookup.LookupId;
                }
                arrParams[2] = new OracleParameter("P_SYSTEM_NAME", OracleDbType.Varchar2);
                arrParams[2].Value = lookup.SystemName;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                arrParams[4] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(lookup.Status)))
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = lookup.Status;
                }
                arrParams[5] = new OracleParameter("P_LOOKUP_NAME", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(lookup.LookupName)))
                {
                    arrParams[5].Value = DBNull.Value;
                }
                else
                {
                    arrParams[5].Value = lookup.LookupName;
                }

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Lookup_Master.READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Lookup objLookup = new Lookup();
                    objLookup.Id = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    objLookup.LookupId = SafeTypeHandling.ConvertStringToInt32(row["LOOKUP_ID"]);
                    objLookup.LookupName = SafeTypeHandling.ConvertToString(row["LOOKUP_NAME"]);
                    objLookup.LookupInfo = SafeTypeHandling.ConvertToString(row["LOOKUP_INFO"]);
                    objLookup.LookupValue = SafeTypeHandling.ConvertToString(row["LOOKUP_VAL"]);
                    objLookup.LookupDesc = SafeTypeHandling.ConvertToString(row["LOOKUP_DESC"]);
                    objLookup.Status = SafeTypeHandling.ConvertToString(row["USE_FLAG"]);
                    objLookup.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    objLookup.LookupDispSeq = SafeTypeHandling.ConvertToString(row["DISP_SEQ"]);
                    objLookup.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    objLookup.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    objLookup.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    objLookup.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATE_DATE"]);
                    lstLookup.Add(objLookup);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstLookup;
        }

        public async Task<List<Lookup>> ReadLookupTypes(string SystemName)
        {
            List<Lookup> lstLookupTypes = new List<Lookup>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_SYSTEM_NAME", OracleDbType.Varchar2);
                arrParams[0].Value = SystemName;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DNET.Lookup_Master.LOOKUP_TYPES_READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Lookup objLookup = new Lookup();
                    objLookup.LookupId = SafeTypeHandling.ConvertStringToInt32(row["LOOKUP_ID"]);
                    objLookup.LookupValue = SafeTypeHandling.ConvertToString(row["LOOKUP_NAME"]);
                    objLookup.LookupDesc = SafeTypeHandling.ConvertToString(row["LOOKUP_INFO"]);
                    //objLookup.Status = SafeTypeHandling.ConvertToString(row["USE_FLAG"]);
                    //objLookup.StatusDesc = SafeTypeHandling.ConvertToString(row["DISP_SEQ"]);
                    objLookup.CreatedByName = SafeTypeHandling.ConvertToString(row["CREATED_BY"]);
                    objLookup.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    objLookup.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY"]);
                    objLookup.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATE_DATE"]);
                    lstLookupTypes.Add(objLookup);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstLookupTypes;
        }
    }
}











