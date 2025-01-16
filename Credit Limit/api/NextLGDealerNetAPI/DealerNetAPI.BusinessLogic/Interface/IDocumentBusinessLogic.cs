using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IDocumentBusinessLogic
    {
        Task<APIResponse> SaveDocument(Document document);

        Task<List<Document>> ReadDocument(Document document);
    }
}
