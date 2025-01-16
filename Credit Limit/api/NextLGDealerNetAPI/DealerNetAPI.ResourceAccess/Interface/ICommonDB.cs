using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface ICommonDB
    {
        void CloseConnection();
        void DisposeConnection();
        int ExecuteOleDb(string strOleDb, string connectionString = null);
        int ExecuteStoredProc(string strProcName, OracleParameter[] arProcParams, string connectionString = null);
        OracleDataReader getDataReader(string strOleDb, string connectionString = null);
        DataSet GetDataSet(string strOleDb, string connectionString = null);
        DataSet GetDataSet(string strOleDb, string tblname, string connectionString = null);
        DataSet GetDataSetProc(string strSPName, OracleParameter[] arProcParams, string connectionString = null);
        DataTable GetDataTable(string strOleDb, string connectionString = null);
        DataTable getDataTableStoredProc(string sp_name, OracleParameter[] arrParams, string connectionString = null);
        Task<DataTable> getDataTableStoredProcAsync(string sp_name, OracleParameter[] arrParams, string connectionString = null);
        bool IsExist(string strOleDb, string connectionString = null);
        bool IsExistStoredProc(string strSpName, OracleParameter[] arParams, string connectionString = null);
        void OpenConnection(string connectionString = null);
    }
}