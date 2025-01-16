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
    public class FirmTypeAccess : IFirmTypeAccess
    {
        private readonly ICommonDB _commonDB = null;

        public FirmTypeAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }
        public async Task<APIResponse> SaveFirmType(FirmType firmtype)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_FIRMTYPE_ID", OracleDbType.Varchar2);
                arrParams[0].Value = firmtype.FirmTypeId;
                arrParams[1] = new OracleParameter("P_FIRMTYPE_NAME", OracleDbType.Varchar2);
                arrParams[1].Value = firmtype.FirmTypeName;
                arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[2].Value = firmtype.Status;
                arrParams[3] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2);
                arrParams[3].Value = firmtype.CreatedBy;
                arrParams[4] = new OracleParameter("P_OUT", OracleDbType.Varchar2, 500);
                arrParams[4].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.FirmType_Master.INSERT, arrParams);
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
        public async Task<List<FirmType>> ReadFirmType(FirmType firmtype)
        {
            List<FirmType> lstFirmType = new List<FirmType>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[0].Value = firmtype.Status;
               // arrParams[1] = new OracleParameter("P_STATE_ID", OracleDbType.Varchar2);
                //arrParams[1].Value = district.State.StateId;
                arrParams[1] = new OracleParameter("P_FIRMTYPE_ID", OracleDbType.Varchar2);
                arrParams[1].Value = firmtype.FirmTypeId;

                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.FirmType_Master.READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    FirmType objFirmType = new FirmType();
                    //objFirmType.State.StateId = SafeTypeHandling.ConvertStringToInt32(row["STATE_ID"]);

                    objFirmType.FirmTypeId = SafeTypeHandling.ConvertStringToInt32(row["FIRMTYPE_ID"]);
                    //objDistrict.DistrictCode = SafeTypeHandling.ConvertToString(row["DISTRICT_CODE"]);
                    objFirmType.FirmTypeName = SafeTypeHandling.ConvertToString(row["FIRMTYPE_NAME"]);
                    objFirmType.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    objFirmType.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    objFirmType.CreatedBy = SafeTypeHandling.ConvertStringToInt32(row["CREATED_BY"]);
                    objFirmType.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    objFirmType.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPATED_BY"]);
                    objFirmType.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATE_DATE"]);
                    lstFirmType.Add(objFirmType);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstFirmType;
        }
    }

    }
