
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
    public class DistrictAccess : IDistrictAccess
    {
        private readonly ICommonDB _commonDB = null;

        public DistrictAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        public async Task<APIResponse> SaveDistrict(District district)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_STATE", OracleDbType.Varchar2);
                arrParams[0].Value = district.State;
                arrParams[1] = new OracleParameter("P_DISTRICT_ID", OracleDbType.Varchar2);
                arrParams[1].Value = district.DistrictId;
                arrParams[2] = new OracleParameter("P_DISTRICT_NAME", OracleDbType.Varchar2);
                arrParams[2].Value = district.DistrictName;
                arrParams[3] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[3].Value = district.Status;
                arrParams[4] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2);
                arrParams[4].Value = district.CreatedBy;
                arrParams[5] = new OracleParameter("P_OUT", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;

                DataTable dtInvData =await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.District_Master.INSERT, arrParams);
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
        public async Task<List<District>> ReadDistrict(District district)
        {
            List<District> lstDistrict = new List<District>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[0].Value = district.Status;
                arrParams[1] = new OracleParameter("P_STATE_ID", OracleDbType.Varchar2);
                arrParams[1].Value = district.State.StateId;
                arrParams[2] = new OracleParameter("P_DISTRICT_ID", OracleDbType.Varchar2);
                arrParams[2].Value = district.DistrictId;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.District_Master.READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    District objDistrict = new District();
                    objDistrict.State.StateId = SafeTypeHandling.ConvertStringToInt32(row["STATE_ID"]);
                    objDistrict.DistrictId = SafeTypeHandling.ConvertStringToInt32(row["DISTRICT_ID"]);
                    //objDistrict.DistrictCode = SafeTypeHandling.ConvertToString(row["DISTRICT_CODE"]);
                    objDistrict.DistrictName = SafeTypeHandling.ConvertToString(row["DISTRICT_NAME"]);
                    objDistrict.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    objDistrict.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    objDistrict.CreatedBy = SafeTypeHandling.ConvertStringToInt32(row["CREATED_BY"]);
                    objDistrict.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    objDistrict.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPATED_BY"]);
                    objDistrict.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATE_DATE"]);
                    lstDistrict.Add(objDistrict);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstDistrict;
        }
    }
}






