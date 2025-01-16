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
    public class StateAccess : IStateAccess
    {
        private readonly ICommonDB _commonDB = null;

        public StateAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        public async Task<APIResponse> SaveState(State state)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_STATE_CODE", OracleDbType.Varchar2);
                arrParams[0].Value = state.StateCode;
                arrParams[1] = new OracleParameter("P_STATE_NAME", OracleDbType.Varchar2);
                arrParams[1].Value = state.StateName;
                arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[2].Value = state.Status;
                arrParams[3] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2);
                arrParams[3].Value = state.CreatedBy;
                arrParams[4] = new OracleParameter("P_OUT", OracleDbType.Varchar2, 500);
                arrParams[4].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.State_Master.INSERT, arrParams);
                response = new APIResponse
                {
                    StatusDesc = arrParams[4].Value.ToString()
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

        public async Task<List<State>> ReadState(State state)
        {
            List<State> lstState = new List<State>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[0].Value = state.Status;
                arrParams[1] = new OracleParameter("P_STATE_ID", OracleDbType.Varchar2);
                arrParams[1].Value = state.StateId;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.State_Master.READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    State objState = new State();
                    objState.StateId = SafeTypeHandling.ConvertStringToInt32(row["STATE_ID"]);
                    objState.StateCode = SafeTypeHandling.ConvertToString(row["STATE_CODE"]);
                    objState.StateName = SafeTypeHandling.ConvertToString(row["STATE_NAME"]);
                    objState.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    objState.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    objState.CreatedBy = SafeTypeHandling.ConvertStringToInt32(row["CREATED_BY"]);
                    objState.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    objState.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPATED_BY"]);
                    objState.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATE_DATE"]);
                    lstState.Add(objState);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstState;
        }



    }
}
