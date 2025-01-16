using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.DomainObject.ARCreditLimit;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.ARCreditLimit
{
    public class UtilitiesARCreditLimitBusinessLogic : IUtilitiesARCreditLimitBusinessLogic
    {
        private readonly IUtilitiesARCreditLimitAccess _utilitiesARCreditLimitAccess = null;

        public UtilitiesARCreditLimitBusinessLogic(IUtilitiesARCreditLimitAccess utilitiesARCreditLimitAccess)
        {
            _utilitiesARCreditLimitAccess = utilitiesARCreditLimitAccess;
        }

        public async Task<List<Branch>> ReadBranch()
        {
            var data = await _utilitiesARCreditLimitAccess.ReadBranch();
            return data;
        }

        public async Task<APIResponse> NoteSave(Notes notes)
        {
            var data = await _utilitiesARCreditLimitAccess.NoteSave(notes);
            return data;
        }

        public async Task<List<Notes>> ReadNotes(Notes notes)
        {
            var data = await _utilitiesARCreditLimitAccess.ReadNotes(notes);
            return data;
        }

        public async Task<DataTable> MastersUpdatedRead()
        {
            var data = await _utilitiesARCreditLimitAccess.MastersUpdatedRead();
            return data;
        }

        public async Task<List<ARCreditLimitSalesDOM>> UploadFilesLogRead(string ProcessType)
        {
            var data = await _utilitiesARCreditLimitAccess.UploadFilesLogRead(ProcessType);
            return data;
        }

        public async Task<APIResponse> SalesUpload(List<ARCreditLimitSalesDOM> sales)
        {
            var data = await _utilitiesARCreditLimitAccess.SalesUpload(sales);
            return data;
        }
        public async Task<List<ARCreditLimitSalesDOM>> ReadSalesData(ARCreditLimitSalesDOM sale)
        {
            var data = await _utilitiesARCreditLimitAccess.ReadSalesData(sale);
            return data;
        }

        public async Task<APIResponse> InsuranceUpload(List<ARCreditLimitInsurance> insurances)
        {
            var data = await _utilitiesARCreditLimitAccess.InsuranceUpload(insurances);
            return data;
        }
        public async Task<List<ARCreditLimitInsurance>> ReadInsuranceData(ARCreditLimitInsurance insurance)
        {
            var data = await _utilitiesARCreditLimitAccess.ReadInsuranceData(insurance);
            return data;
        }


        public async Task<APIResponse> ODUpload(List<ARCreditLimitOD> lstOD)
        {
            var data = await _utilitiesARCreditLimitAccess.ODUpload(lstOD);
            return data;
        }
        public async Task<List<ARCreditLimitOD>> ReadODData(ARCreditLimitOD od)
        {
            var data = await _utilitiesARCreditLimitAccess.ReadODData(od);
            return data;
        }

        public async Task<APIResponse> CollectionUpload(List<ARCreditLimitCollection> lstCollection)
        {
            var data = await _utilitiesARCreditLimitAccess.CollectionUpload(lstCollection);
            return data;
        }
        public async Task<List<ARCreditLimitCollection>> ReadCollectionData(ARCreditLimitCollection collection)
        {
            var data = await _utilitiesARCreditLimitAccess.ReadCollectionData(collection);
            return data;
        }


        public async Task<APIResponse> FYStatusSave(ARCreditLimitFYStatus fYStatus)
        {
            var data = await _utilitiesARCreditLimitAccess.FYStatusSave(fYStatus);
            return data;
        }

        public async Task<List<ARCreditLimitFYStatus>> FYStatusRead(ARCreditLimitFYStatus fYStatus)
        {
            var data = await _utilitiesARCreditLimitAccess.FYStatusRead(fYStatus);
            return data;
        }
        
        public async Task<APIResponse> FinancialYearAttachmentUpload(List<ARCreditLimitFYStatus> fyAttachements)
        {
            var data = await _utilitiesARCreditLimitAccess.FinancialYearAttachmentUpload(fyAttachements);
            return data;
        }

        public async Task<List<ARCreditLimitFYStatus>> FinancialYearAttachmentRead(ARCreditLimitFYStatus fyAttachement)
        {
            var data = await _utilitiesARCreditLimitAccess.FinancialYearAttachmentRead(fyAttachement);
            return data;
        }



    }
}
