using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic
{
    public class DocumentBusinessLogic : IDocumentBusinessLogic
    {
        private readonly IDocumentAccess _documentAccess = null;
        public DocumentBusinessLogic(IDocumentAccess documentAccess)
        {
            _documentAccess = documentAccess;
        }

        public async Task<APIResponse> SaveDocument(Document document)
        {
            var data = await _documentAccess.SaveDocument(document);
            return data;
        }

        public async Task<List<Document>> ReadDocument(Document document)
        {
            var data = await _documentAccess.ReadDocument(document);
            return data;
        }
    }
}
