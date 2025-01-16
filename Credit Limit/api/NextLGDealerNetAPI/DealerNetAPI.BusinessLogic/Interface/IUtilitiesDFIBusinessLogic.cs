using DealerNetAPI.DomainObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.Interface
{
    public interface IUtilitiesDFIBusinessLogic
    {
        Task<List<Zone>> ReadZone(string createdBy);
        Task<List<Region>> ReadRegion(Branch branch);
        Task<List<Branch>> ReadBranch(Branch branch);
        Task<List<Lookup>> ReadSchemeType();
        Task<List<Lookup>> ReadSchemeReasonCode(string SchemeType, string FundSource, string RoleName);

        Task<List<ChannelDOM>> ReadMajorSalesChannel();
        Task<List<ChannelDOM>> ReadSalesChannel(string MajorSalesChannel);
        Task<List<ChannelDOM>> ReadChannel(string MajorSalesChannel, string SalesChannel);
        Task<List<Distributor>> ReadBillingCode(Distributor distributor);

        Task<List<Model>> ReadProduct();
        Task<List<Model>> ReadSubProduct(string Product);
        Task<List<Model>> ReadProductLevel3(Model model);
        Task<List<Model>> ReadModelCategory(Model model);
        Task<List<Model>> ReadModelSeries(Model model);
        Task<List<Model>> ReadModelSubCategory(Model model);
        Task<List<Model>> ReadModelStarRating(Model model);
        Task<List<Model>> ReadModelYear(Model model);
        Task<List<Model>> ReadModel(Model model);

        Task<List<Model>> ReadFilterProduct(Model model);

        Task<APIResponse> UpdateModels(List<Model> lstModel);

        List<Model> ReadAllModel();

        Task<APIResponse> UserAU_Mapping(Users users);

        Task<List<Users>> ReadUserAU_Mapping(Users users);
        Task<List<Users>> ReadUsersForAUMappingByRoleId(int roleId);
        Task<List<Users>> ReadApprovers(string approverName);

        


    }
}
