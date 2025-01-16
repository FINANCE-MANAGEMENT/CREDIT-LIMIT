using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.DomainObject.ARCreditLimit;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.ARCreditLimit
{
    public class ARCreditLimitBusinessLogic : IARCreditLimitBusinessLogic
    {
        private readonly IARCreditLimitAccess _arCreditLimitAccess = null;

        public ARCreditLimitBusinessLogic(IARCreditLimitAccess arCreditLimitAccess)
        {
            _arCreditLimitAccess = arCreditLimitAccess;
        }


        public async Task<ARCreditLimitDOM> ReadHeaderCodeDetail(Distributor distributor)
        {
            var data = await _arCreditLimitAccess.ReadHeaderCodeDetail(distributor);
            return data;
        }

        public async Task<APIResponse> CreditLimitRequestSave(ARCreditLimitDOM creditLimitDOM)
        {
            var data = await _arCreditLimitAccess.CreditLimitRequestSave(creditLimitDOM);
            return data;
        }

        public async Task<APIResponse> CreditLimitRequestBulk(List<ARCreditLimitDOM> creditLimits)
        {
            var data = await _arCreditLimitAccess.CreditLimitRequestBulk(creditLimits);
            return data;
        }


        public async Task<List<ARCreditLimitDOM>> ReadAllRequest(ARCreditLimitDOM creditLimitDOM)
        {
            var data = await _arCreditLimitAccess.ReadAllRequest(creditLimitDOM);
            return data;
        }


        public async Task<APIResponse> RequestApproval(List<ARCreditLimitDOM> creditLimitRequests)
        {
            var data = await _arCreditLimitAccess.RequestApproval(creditLimitRequests);
            return data;
        }

        public void ReadEmailForCLRequest(Int64 ReqId, string HeaderCode, out string EmailTo, out string EmailCC)
        {
            _arCreditLimitAccess.ReadEmailForCLRequest(ReqId, HeaderCode, out EmailTo, out EmailCC);
        }

        public async Task<APIResponse> EmailSendLog(Int64 ReqId, string HeaderCode, string EmailTo, string EmailCC)
        {
            var data = await _arCreditLimitAccess.EmailSendLog(ReqId, HeaderCode, EmailTo, EmailCC);
            return data;
        }

    }
}
