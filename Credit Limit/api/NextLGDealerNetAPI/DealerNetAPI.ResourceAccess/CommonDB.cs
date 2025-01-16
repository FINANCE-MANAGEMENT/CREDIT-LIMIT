using DealerNetAPI.ResourceAccess.Interface;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess
{
    public class CommonDB : ICommonDB
    {
        public readonly IConfiguration _configuration;
        public OracleCommand objCmd;
        public OracleConnection objConn;
        public OracleDataAdapter objDA;
        private readonly string DealerNet_Connection = "DealerNetConnection";

        public CommonDB(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void CloseConnection()
        {
            if (this.objConn.State == ConnectionState.Open)
            {
                this.objConn.Close();
            }
        }

        public void DisposeConnection()
        {
            if (this.objConn != null)
            {
                if (this.objConn.State == ConnectionState.Closed)
                {
                    this.objConn.Dispose();
                }
                this.objConn = null;
            }
        }

        public int ExecuteOleDb(string strOleDb, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            this.objCmd.CommandType = CommandType.Text;
            this.objCmd.CommandText = strOleDb;
            int num = this.objCmd.ExecuteNonQuery();
            this.CloseConnection();
            this.DisposeConnection();
            return num;
        }

        public int ExecuteStoredProc(string strProcName, OracleParameter[] arProcParams, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            this.objCmd.CommandType = CommandType.StoredProcedure;
            this.objCmd.CommandText = strProcName;
            this.objCmd.Parameters.Clear();
            foreach (OracleParameter parameter in arProcParams)
            {
                this.objCmd.Parameters.Add(parameter);
            }
            int num = this.objCmd.ExecuteNonQuery();
            this.CloseConnection();
            this.DisposeConnection();
            return num;
        }

        public OracleDataReader getDataReader(string strOleDb, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            return this.objCmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public DataSet GetDataSet(string strOleDb, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            OracleDataAdapter adapter = new OracleDataAdapter(strOleDb, this.objConn);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            this.CloseConnection();
            this.DisposeConnection();
            return dataSet;
        }

        public DataSet GetDataSet(string strOleDb, string tblname, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            OracleDataAdapter adapter = new OracleDataAdapter(strOleDb, this.objConn);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, tblname);
            this.CloseConnection();
            this.DisposeConnection();
            return dataSet;
        }

        public DataSet GetDataSetProc(string strSPName, OracleParameter[] arProcParams, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            DataSet dataSet = new DataSet();
            OracleParameter parameter = new OracleParameter();
            this.objCmd.CommandType = CommandType.StoredProcedure;
            this.objCmd.CommandText = strSPName;
            this.objCmd.Parameters.Clear();
            this.objDA.SelectCommand = this.objCmd;
            foreach (OracleParameter parameter2 in arProcParams)
            {
                parameter = parameter2;
                this.objCmd.Parameters.Add(parameter);
            }
            this.objDA.Fill(dataSet);
            this.CloseConnection();
            this.DisposeConnection();
            return dataSet;
        }

        public DataTable GetDataTable(string strOleDb, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            this.objDA = new OracleDataAdapter(strOleDb, this.objConn);
            DataTable dataTable = new DataTable();
            this.objDA.Fill(dataTable);
            this.CloseConnection();
            this.DisposeConnection();
            return dataTable;
        }

        public DataTable getDataTableStoredProc(string sp_name, OracleParameter[] arrParams, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            this.objCmd.CommandType = CommandType.StoredProcedure;
            this.objCmd.CommandText = sp_name;
            this.objCmd.Parameters.Clear();
            this.objDA.SelectCommand = this.objCmd;
            foreach (OracleParameter parameter in arrParams)
            {
                this.objCmd.Parameters.Add(parameter);
            }
            DataTable dataTable = new DataTable();
            this.objDA.Fill(dataTable);
            this.CloseConnection();
            this.DisposeConnection();
            return dataTable;
        }
        public async Task<DataTable> getDataTableStoredProcAsync(string sp_name, OracleParameter[] arrParams, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            this.objCmd.CommandType = CommandType.StoredProcedure;
            this.objCmd.CommandText = sp_name;
            this.objCmd.Parameters.Clear();
            this.objDA.SelectCommand = this.objCmd;
            foreach (OracleParameter parameter in arrParams)
            {
                this.objCmd.Parameters.Add(parameter);
            }
            DataTable dataTable = new DataTable();
            await Task.Run(() => this.objDA.Fill(dataTable));
            this.CloseConnection();
            this.DisposeConnection();
            return dataTable;
        }

        public bool IsExist(string strOleDb, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            this.objCmd.CommandType = CommandType.Text;
            this.objCmd.CommandText = strOleDb;
            int num = (int)this.objCmd.ExecuteScalar();
            this.CloseConnection();
            this.DisposeConnection();
            if (num == 0)
            {
                return false;
            }
            return true;
        }

        public bool IsExistStoredProc(string strSpName, OracleParameter[] arParams, string connectionString = null)
        {
            this.OpenConnection(connectionString);
            this.objCmd.CommandType = CommandType.StoredProcedure;
            this.objCmd.CommandText = strSpName;
            this.objCmd.Parameters.Clear();
            this.objDA.SelectCommand = this.objCmd;
            foreach (OracleParameter parameter in arParams)
            {
                this.objCmd.Parameters.Add(parameter);
            }
            string str = (string)this.objCmd.ExecuteScalar();
            this.CloseConnection();
            this.DisposeConnection();
            return !string.IsNullOrEmpty(str);
        }

        public void OpenConnection(string connectionString = null)
        {
            if (this.objConn == null)
            {
                if (string.IsNullOrEmpty(connectionString)) // By Default Connection
                {
                    this.objConn = new OracleConnection(_configuration.GetConnectionString(DealerNet_Connection));
                }
                else
                {
                    this.objConn = new OracleConnection(_configuration.GetConnectionString(connectionString));
                }
                if (this.objConn.State == ConnectionState.Closed)
                {
                    this.objConn.Open();
                }
                this.objCmd = new OracleCommand();
                this.objDA = new OracleDataAdapter();
                this.objCmd.Connection = this.objConn;
            }
        }

    }
}
