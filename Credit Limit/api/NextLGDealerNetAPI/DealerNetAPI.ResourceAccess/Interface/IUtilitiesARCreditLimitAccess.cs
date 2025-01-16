using DealerNetAPI.DomainObject;
using DealerNetAPI.DomainObject.ARCreditLimit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.Interface
{
    public interface IUtilitiesARCreditLimitAccess
    {
        Task<List<Branch>> ReadBranch();
        Task<APIResponse> NoteSave(Notes notes);
        Task<List<Notes>> ReadNotes(Notes notes);

        Task<DataTable> MastersUpdatedRead();
        Task<List<ARCreditLimitSalesDOM>> UploadFilesLogRead(string ProcessType);

        Task<APIResponse> SalesUpload(List<ARCreditLimitSalesDOM> sales);
        Task<List<ARCreditLimitSalesDOM>> ReadSalesData(ARCreditLimitSalesDOM sale);

        Task<APIResponse> InsuranceUpload(List<ARCreditLimitInsurance> insurances);
        Task<List<ARCreditLimitInsurance>> ReadInsuranceData(ARCreditLimitInsurance insurance);

        Task<APIResponse> ODUpload(List<ARCreditLimitOD> lstOD);
        Task<List<ARCreditLimitOD>> ReadODData(ARCreditLimitOD od);

        Task<APIResponse> CollectionUpload(List<ARCreditLimitCollection> lstCollection);
        Task<List<ARCreditLimitCollection>> ReadCollectionData(ARCreditLimitCollection collection);

        Task<APIResponse> FYStatusSave(ARCreditLimitFYStatus fYStatus);
        Task<List<ARCreditLimitFYStatus>> FYStatusRead(ARCreditLimitFYStatus fYStatus);


        Task<APIResponse> FinancialYearAttachmentUpload(List<ARCreditLimitFYStatus> fyAttachements);
        Task<List<ARCreditLimitFYStatus>> FinancialYearAttachmentRead(ARCreditLimitFYStatus fyAttachement);


    }
}
