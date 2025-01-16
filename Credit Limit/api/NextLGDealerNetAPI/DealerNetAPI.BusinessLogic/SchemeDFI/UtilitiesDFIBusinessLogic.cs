using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.DomainObject;
using DealerNetAPI.ResourceAccess.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DealerNetAPI.BusinessLogic.SchemeDFI
{
    public class UtilitiesDFIBusinessLogic : IUtilitiesDFIBusinessLogic
    {
        private readonly IUtilitiesDFIAccess _utilitiesDFIAccess = null;

        public UtilitiesDFIBusinessLogic(IUtilitiesDFIAccess utilitiesDFIAccess)
        {
            _utilitiesDFIAccess = utilitiesDFIAccess;
        }

        public async Task<List<Zone>> ReadZone(string createdBy)
        {
            var data = await _utilitiesDFIAccess.ReadZone(createdBy);
            return data;
        }
        public async Task<List<Region>> ReadRegion(Branch branch)
        {
            var data = await _utilitiesDFIAccess.ReadRegion(branch);
            return data;
        }
        public async Task<List<Branch>> ReadBranch(Branch branch)
        {
            var data = await _utilitiesDFIAccess.ReadBranch(branch);
            return data;
        }
        public async Task<List<Lookup>> ReadSchemeType()
        {
            var data = await _utilitiesDFIAccess.ReadSchemeType();
            return data;
        }
        public async Task<List<Lookup>> ReadSchemeReasonCode(string SchemeType, string FundSource, string RoleName)
        {
            var data = await _utilitiesDFIAccess.ReadSchemeReasonCode(SchemeType, FundSource, RoleName);
            return data;
        }

        public async Task<List<ChannelDOM>> ReadMajorSalesChannel()
        {
            var data = await _utilitiesDFIAccess.ReadMajorSalesChannel();
            return data;
        }
        public async Task<List<ChannelDOM>> ReadSalesChannel(string MajorSalesChannel)
        {
            var data = await _utilitiesDFIAccess.ReadSalesChannel(MajorSalesChannel);
            return data;
        }
        public async Task<List<ChannelDOM>> ReadChannel(string MajorSalesChannel, string SalesChannel)
        {
            var data = await _utilitiesDFIAccess.ReadChannel(MajorSalesChannel, SalesChannel);
            return data;
        }

        public async Task<List<Distributor>> ReadBillingCode(Distributor distributor)
        {
            var data = await _utilitiesDFIAccess.ReadBillingCode(distributor);
            return data;
        }
        public async Task<List<Model>> ReadProduct()
        {
            var data = await _utilitiesDFIAccess.ReadProduct();
            return data;
        }
        public async Task<List<Model>> ReadSubProduct(string Product)
        {
            var data = await _utilitiesDFIAccess.ReadSubProduct(Product);
            return data;
        }

        public async Task<List<Model>> ReadProductLevel3(Model model)
        {
            var data = await _utilitiesDFIAccess.ReadProductLevel3(model);
            return data;
        }

        public async Task<List<Model>> ReadModelCategory(Model model)
        {
            var data = await _utilitiesDFIAccess.ReadModelCategory(model);
            return data;
        }
        public async Task<List<Model>> ReadModelSeries(Model model)
        {
            var data = await _utilitiesDFIAccess.ReadModelSeries(model);
            return data;
        }
        public async Task<List<Model>> ReadModelSubCategory(Model model)
        {
            var data = await _utilitiesDFIAccess.ReadModelSubCategory(model);
            return data;
        }
        public async Task<List<Model>> ReadModelStarRating(Model model)
        {
            var data = await _utilitiesDFIAccess.ReadModelStarRating(model);
            return data;
        }
        public async Task<List<Model>> ReadModelYear(Model model)
        {
            var data = await _utilitiesDFIAccess.ReadModelYear(model);
            return data;
        }

        public async Task<List<Model>> ReadModel(Model model)
        {
            var data = await _utilitiesDFIAccess.ReadModel(model);
            return data;
        }
        public async Task<List<Model>> ReadFilterProduct(Model model)
        {
            var data = await _utilitiesDFIAccess.ReadFilterProduct(model);
            return data;
        }

        public async Task<APIResponse> UpdateModels(List<Model> lstModel)
        {
            var data = await _utilitiesDFIAccess.UpdateModels(lstModel);
            return data;
        }

        public List<Model> ReadAllModel()
        {
            var data = _utilitiesDFIAccess.ReadAllModel();
            return data;
        }

        public async Task<APIResponse> UserAU_Mapping(Users users)
        {
            var data = await _utilitiesDFIAccess.UserAU_Mapping(users);
            return data;
        }

        public async Task<List<Users>> ReadUserAU_Mapping(Users users)
        {
            var data = await _utilitiesDFIAccess.ReadUserAU_Mapping(users);
            return data;
        }


        public async Task<List<Users>> ReadUsersForAUMappingByRoleId(int roleId)
        {
            var data = await _utilitiesDFIAccess.ReadUsersForAUMappingByRoleId(roleId);
            return data;
        }

        public async Task<List<Users>> ReadApprovers(string approverName)
        {
            var data = await _utilitiesDFIAccess.ReadApprovers(approverName);
            return data;
        }



    }

}