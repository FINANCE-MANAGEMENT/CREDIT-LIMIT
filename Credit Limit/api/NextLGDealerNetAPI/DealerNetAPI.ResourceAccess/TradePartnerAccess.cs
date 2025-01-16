using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DealerNetAPI.ResourceAccess
{
    public class TradePartnerAccess : ITradePartnerAccess
    {
        private readonly ICommonDB _commonDB = null;

        public TradePartnerAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        /// <summary>
        /// Save Channel Finance of Basic Details.
        /// </summary>
        /// <param name="tradePartner"></param>
        /// <returns></returns>
        public string SaveChannelFinanceBasicDetail(TradePartner tradePartner)
        {
            string Status = string.Empty;
            try
            {
                //OracleParameter[] arrParams = new OracleParameter[5];
                //arrParams[0] = new OracleParameter("P_STATE_CODE", OracleDbType.Varchar2);
                //arrParams[0].Value = tradePartner.StateCode;

                //arrParams[1] = new OracleParameter("P_STATE_NAME", OracleDbType.Varchar2);
                //arrParams[1].Value = state.StateName;

                //arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                //arrParams[2].Value = state.Status;

                //arrParams[3] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2);
                //arrParams[3].Value = state.CreatedBy;

                //arrParams[4] = new OracleParameter("P_OUT", OracleDbType.Varchar2, 500);
                //arrParams[4].Direction = ParameterDirection.Output;

                //DataTable dtInvData = new DataTable();
                //dtInvData = _commonDB.getDataTableStoredProc(DatabaseConstants.ChannelFinance..ChannelFinance_State_Master.INSERT, arrParams);
                //Status = arrParams[4].Value.ToString();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Status;
        }



    }
}
