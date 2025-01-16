using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic
{
    public class TermsConditionsBusinessLogic : ITermsConditionsBusinessLogic
    {
        private readonly ITermsConditionsAccess _termsConditionsAccess = null;

        public TermsConditionsBusinessLogic(ITermsConditionsAccess termsConditionsAccess)
        {
            _termsConditionsAccess = termsConditionsAccess;
        }

        public async Task<List<SchemeTypes>> ReadSchemeTypes(string SystemName, Int32 TnCID)
        {
            var data = await _termsConditionsAccess.ReadSchemeTypes(SystemName,TnCID);
            return data;
        }

        public async Task<APIResponse> SaveTermsConditions(TermsConditions termsConditions)
        {
            var data = await _termsConditionsAccess.SaveTermsConditions(termsConditions);
            return data;
        }

        public async Task<List<TermsConditions>> ReadTermsConditions(TermsConditions termsConditions)
        {
            var data = await _termsConditionsAccess.ReadTermsConditions(termsConditions);
            return data;
        }
    }
}

