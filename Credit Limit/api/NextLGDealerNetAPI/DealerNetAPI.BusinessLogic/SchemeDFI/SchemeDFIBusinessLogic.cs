using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.DomainObject.SchemeAutomation;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.SchemeDFI
{
    public class SchemeDFIBusinessLogic : ISchemeDFIBusinessLogic
    {
        private readonly ISchemeDFIAccess _schemeDFIAccess = null;

        public SchemeDFIBusinessLogic(ISchemeDFIAccess schemeDFIAccess)
        {
            _schemeDFIAccess = schemeDFIAccess;
        }


        public async Task<APIResponse> ReadSchemeHistory(string SchemeRefNo)
        {
            var data = await _schemeDFIAccess.ReadSchemeHistory(SchemeRefNo);
            return data;
        }


        public async Task<APIResponse> SaveSchemeRequest(SchemeRequest schemeRequest)
        {
            var data = await _schemeDFIAccess.SaveSchemeRequest(schemeRequest);
            return data;
        }

        public async Task<APIResponse> SaveSchemeRequestDetail(SchemeRequest schemeRequest)
        {
            var data = await _schemeDFIAccess.SaveSchemeRequestDetail(schemeRequest);
            return data;
        }
        public async Task<APIResponse> SchemeUpdateAfterApproved(SchemeRequest schemeRequest)
        {
            var data = await _schemeDFIAccess.SchemeUpdateAfterApproved(schemeRequest);
            return data;
        }


        public async Task<APIResponse> UpdateApprovalStatus(Approval approval)
        {
            var data = await _schemeDFIAccess.UpdateApprovalStatus(approval);
            return data;
        }

        public async Task<List<SchemeRequest>> ReadSchemeRequest(SchemeRequest schemeRequest)
        {
            var data = await _schemeDFIAccess.ReadSchemeRequest(schemeRequest);
            return data;
        }

        public async Task<List<Distributor>> ReadSchemeCustomerDetail(string SchemeRefNo)
        {
            var data = await _schemeDFIAccess.ReadSchemeCustomerDetail(SchemeRefNo);
            return data;
        }

        public async Task<List<Model>> ReadSchemeProductDetail(string SchemeRefNo)
        {
            var data = await _schemeDFIAccess.ReadSchemeProductDetail(SchemeRefNo);
            return data;
        }


        public async Task<List<SchemeRequest>> ReadApprovalScheme(string UserId)
        {
            var data = await _schemeDFIAccess.ReadApprovalScheme(UserId);
            return data;
        }




        public async Task<List<Approval>> ReadAllApproverByScheme(string SchemeRefNo)
        {
            var data = await _schemeDFIAccess.ReadAllApproverByScheme(SchemeRefNo);
            return data;
        }

        public async Task<List<Approval>> ReadApprovalHistByScheme(string SchemeRefNo)
        {
            var data = await _schemeDFIAccess.ReadApprovalHistByScheme(SchemeRefNo);
            return data;
        }

        public async Task<List<TermsConditions>> ReadTnCByScheme(string SchemeRefNo)
        {
            var data = await _schemeDFIAccess.ReadTnCByScheme(SchemeRefNo);
            return data;
        }

        public async Task<List<Slab_TargetDOM>> ReadSchemeSlab(string SchemeRefNo)
        {
            var data = await _schemeDFIAccess.ReadSchemeSlab(SchemeRefNo);
            return data;
        }


        public async Task<APIResponse> SchemeCalculationRequest(SchemeSettlementDOM schemeSettlement)
        {
            var data = await _schemeDFIAccess.SchemeCalculationRequest(schemeSettlement);
            return data;
        }
        public async Task<List<SchemeSettlementDOM>> SchemeCalculationRequestRead(SchemeSettlementDOM schemeSettlement)
        {
            var data = await _schemeDFIAccess.SchemeCalculationRequestRead(schemeSettlement);
            return data;
        }
        public async Task<List<SchemeSettlementDOM>> SchemeCalculationRead(SchemeSettlementDOM schemeSettlement)
        {
            var data = await _schemeDFIAccess.SchemeCalculationRead(schemeSettlement);
            return data;
        }

        public async Task<List<GTMSchemeDOM>> GTMSchemeDataRead(string SchemeRefNo)
        {
            var data = await _schemeDFIAccess.GTMSchemeDataRead(SchemeRefNo);
            return data;
        }

        public async Task<APIResponse> SerialNoApplicability(SerailNoApplicability serailNoApplicability)
        {
            var data = await _schemeDFIAccess.SerialNoApplicability(serailNoApplicability);
            return data;
        }

        public async Task<List<SerailNoApplicability>> SerialNoApplicabilityRead(SerailNoApplicability serailNoApplicability)
        {
            var data = await _schemeDFIAccess.SerialNoApplicabilityRead(serailNoApplicability);
            return data;
        }

        public async Task<APIResponse> SchemeTargetUpload(List<Slab_TargetDOM> lstSchemeTarget)
        {
            var data = await _schemeDFIAccess.SchemeTargetUpload(lstSchemeTarget);
            return data;
        }

        public async Task<List<Slab_TargetDOM>> ReadSchemeTarget(string SchemeRefNo)
        {
            var data = await _schemeDFIAccess.ReadSchemeTarget(SchemeRefNo);
            return data;
        }



    }
}
