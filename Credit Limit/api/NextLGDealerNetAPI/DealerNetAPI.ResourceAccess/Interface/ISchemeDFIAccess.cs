using DealerNetAPI.DomainObject;
using DealerNetAPI.DomainObject.SchemeAutomation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface ISchemeDFIAccess
    {
        Task<APIResponse> ReadSchemeHistory(string SchemeRefNo);
        Task<APIResponse> SaveSchemeRequest(SchemeRequest schemeRequest);

        Task<APIResponse> SaveSchemeRequestDetail(SchemeRequest schemeRequest);
        Task<APIResponse> SchemeUpdateAfterApproved(SchemeRequest schemeRequest);

        Task<APIResponse> UpdateApprovalStatus(Approval approval);

        Task<List<SchemeRequest>> ReadSchemeRequest(SchemeRequest schemeRequest);

        Task<List<Distributor>> ReadSchemeCustomerDetail(string SchemeRefNo);

        Task<List<Model>> ReadSchemeProductDetail(string SchemeRefNo);

        Task<List<SchemeRequest>> ReadApprovalScheme(string UserId);

        Task<List<Approval>> ReadApprovalHistByScheme(string SchemeRefNo);

        Task<List<Approval>> ReadAllApproverByScheme(string SchemeRefNo);
        Task<List<TermsConditions>> ReadTnCByScheme(string SchemeRefNo);
        Task<List<Slab_TargetDOM>> ReadSchemeSlab(string SchemeRefNo);


        Task<APIResponse> SchemeCalculationRequest(SchemeSettlementDOM schemeSettlement);
        Task<List<SchemeSettlementDOM>> SchemeCalculationRequestRead(SchemeSettlementDOM schemeSettlement);
        Task<List<SchemeSettlementDOM>> SchemeCalculationRead(SchemeSettlementDOM schemeSettlement);
        Task<List<GTMSchemeDOM>> GTMSchemeDataRead(string SchemeRefNo);

        Task<APIResponse> SerialNoApplicability(SerailNoApplicability serailNoApplicability);
        Task<List<SerailNoApplicability>> SerialNoApplicabilityRead(SerailNoApplicability serailNoApplicability);

        Task<APIResponse> SchemeTargetUpload(List<Slab_TargetDOM> schemeTargets);
        Task<List<Slab_TargetDOM>> ReadSchemeTarget(string SchemeRefNo);
        


    }
}
