using DealerNetAPI.DomainObject;
using DealerNetAPI.DomainObject.ARCreditLimit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface IARCreditLimitAccess
    {
        Task<ARCreditLimitDOM> ReadHeaderCodeDetail(Distributor distributor);
        Task<APIResponse> CreditLimitRequestSave(ARCreditLimitDOM creditLimitDOM);
        Task<APIResponse> CreditLimitRequestBulk(List<ARCreditLimitDOM> creditLimits);

        Task<List<ARCreditLimitDOM>> ReadAllRequest(ARCreditLimitDOM creditLimitDOM);

        Task<APIResponse> RequestApproval(List<ARCreditLimitDOM> creditLimitRequests);

        void ReadEmailForCLRequest(Int64 ReqId, string HeaderCode, out string EmailTo, out string EmailCC);
        Task<APIResponse> EmailSendLog(Int64 ReqId, string HeaderCode, string EmailTo, string EmailCC);
       

    }
}
