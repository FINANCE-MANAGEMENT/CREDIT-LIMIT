using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DealerNetAPI.BusinessLogic
{
    public class BaseBusinessLogic
    {
        public const string DealerNetConnection = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.20.188)(PORT=1523)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=IL11GT)));User Id=dnet09;Password=dnet09;";

        /// <summary>
        /// Handles the connection.
        /// </summary>
        /// <param name="dbConnection">The db connection.</param>
        public static void HandleConnection(DbConnection dbConnection)
        {
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }
        }
    }
}
