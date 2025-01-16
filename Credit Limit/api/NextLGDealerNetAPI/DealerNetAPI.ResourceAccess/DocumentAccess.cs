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
    public class DocumentAccess : IDocumentAccess
    {
        private readonly ICommonDB _commonDB = null;

        public DocumentAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }
        public async Task<APIResponse> SaveDocument(Document document)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_DOCUMENT_ID", OracleDbType.Varchar2);
                arrParams[0].Value = document.DocumentId;
                arrParams[1] = new OracleParameter("P_DOCUMENT_NAME", OracleDbType.Varchar2);
                arrParams[1].Value = document.DocumentName;
                arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[2].Value = document.Status;
                arrParams[3] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2);
                arrParams[3].Value = document.CreatedBy;
                arrParams[4] = new OracleParameter("P_OUT", OracleDbType.Varchar2, 500);
                arrParams[4].Direction = ParameterDirection.Output;

                DataTable dtInvData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.Document_Master.INSERT, arrParams);
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
        public async Task<List<Document>> ReadDocument(Document document)
        {
            List<Document> lstDocument = new List<Document>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_STATUS", OracleDbType.Varchar2);
                arrParams[0].Value = document.Status;
                arrParams[1] = new OracleParameter("P_DOCUMENT_ID", OracleDbType.Varchar2);
                arrParams[1].Value = document.DocumentId;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;

                DataTable dtData =await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.ChannelFinance.Document_Master.READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Document objDocument = new Document();

                    objDocument.DocumentId = SafeTypeHandling.ConvertStringToInt32(row["DOCUMENT_ID"]);;
                    objDocument.DocumentName = SafeTypeHandling.ConvertToString(row["DOCUMENT_NAME"]);
                    objDocument.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    objDocument.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    objDocument.CreatedBy = SafeTypeHandling.ConvertStringToInt32(row["CREATED_BY"]);
                    objDocument.CreatedDate = SafeTypeHandling.ConvertToDateTime(row["CREATION_DATE"]);
                    objDocument.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPATED_BY"]);
                    objDocument.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATE_DATE"]);
                    lstDocument.Add(objDocument);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstDocument;
        }
    }

}



        