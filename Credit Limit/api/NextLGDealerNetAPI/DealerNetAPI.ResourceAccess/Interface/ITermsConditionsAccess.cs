using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface ITermsConditionsAccess
    {
        Task<List<SchemeTypes>> ReadSchemeTypes(string SystemName, Int32 TnCID);
        Task<APIResponse> SaveTermsConditions(TermsConditions termsConditions);
        Task<List<TermsConditions>> ReadTermsConditions(TermsConditions termsConditions);
    }
}
