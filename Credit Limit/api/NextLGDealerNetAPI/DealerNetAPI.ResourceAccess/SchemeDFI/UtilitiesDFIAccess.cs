using DealerNetAPI.Common;
using DealerNetAPI.DomainObject;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using DealerNetAPI.ResourceAccess.Interface;
using System.Threading.Tasks;

namespace DealerNetAPI.ResourceAccess.SchemeDFI
{
    public class UtilitiesDFIAccess : IUtilitiesDFIAccess
    {
        private readonly ICommonDB _commonDB = null;
        private readonly string DealerNet_Next_Connection = "DealerNetNextConnection";

        public UtilitiesDFIAccess(ICommonDB commonDB)
        {
            _commonDB = commonDB;
        }

        public async Task<List<Zone>> ReadZone(string createdBy)
        {
            List<Zone> lstZone = new List<Zone>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[0].Direction = ParameterDirection.Output;
                arrParams[1] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(createdBy))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = createdBy;
                }

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_ZONE, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Zone zone = new Zone();
                    zone.ZoneCode = SafeTypeHandling.ConvertToString(row["ZONE"]);
                    //zone.isSelected = SafeTypeHandling.ConvertStringToBoolean(row["IS_SELECTED"]);
                    lstZone.Add(zone);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstZone;
        }

        public async Task<List<Region>> ReadRegion(Branch branch)
        {
            List<Region> lstRegion = new List<Region>();
            try
            {
                //OracleParameter[] arrParams = new[] {
                //    new OracleParameter("P_ZONE", OracleDbType.Varchar2, branch.ZoneCode, ParameterDirection.Input),
                //    new OracleParameter("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output),
                //};

                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_ZONE", OracleDbType.Varchar2);
                arrParams[0].Value = branch.ZoneCode;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                arrParams[2] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(branch.CreatedBy)))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = branch.CreatedBy;
                }

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_REGION, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Region region = new Region();
                    region.RegionCode = SafeTypeHandling.ConvertToString(row["REGION"]);
                    lstRegion.Add(region);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstRegion;
        }

        public async Task<List<Branch>> ReadBranch(Branch branch)
        {
            List<Branch> lstBranch = new List<Branch>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_ZONE", OracleDbType.Varchar2);
                arrParams[0].Value = branch.ZoneCode;
                arrParams[1] = new OracleParameter("P_REGION", OracleDbType.Varchar2);
                arrParams[1].Value = branch.RegionCode;
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;
                arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Convert.ToString(branch.CreatedBy)))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = branch.CreatedBy;
                }

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_BRANCH, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Branch _branch = new Branch();
                    _branch.ZoneCode = SafeTypeHandling.ConvertToString(row["ZONE"]);
                    _branch.RegionCode = SafeTypeHandling.ConvertToString(row["REGION"]);
                    _branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    lstBranch.Add(_branch);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstBranch;
        }

        public async Task<List<Lookup>> ReadSchemeType()
        {
            List<Lookup> lstSchemeType = new List<Lookup>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[1];
                arrParams[0] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[0].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_SCHEME_TYPE, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Lookup lookup = new Lookup();
                    lookup.LookupName = SafeTypeHandling.ConvertToString(row["SCHEME_TYPE"]);
                    lstSchemeType.Add(lookup);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstSchemeType;
        }

        public async Task<List<Lookup>> ReadSchemeReasonCode(string SchemeType, string FundSource, string RoleName)
        {
            List<Lookup> lstSchemeSubType = new List<Lookup>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_SCHEME_TYPE", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(SchemeType) || SchemeType == null)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = SchemeType;
                }
                arrParams[1] = new OracleParameter("P_FUND_SOURCE", OracleDbType.Varchar2, 100);
                if (string.IsNullOrEmpty(FundSource) || FundSource == null)
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = FundSource;
                }
                arrParams[2] = new OracleParameter("P_ROLE_NAME", OracleDbType.Varchar2, 100);
                arrParams[2].Value = RoleName;
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_SCHEME_REASON_CODE, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Lookup lookup = new Lookup();
                    lookup.LookupName = SafeTypeHandling.ConvertToString(row["NATURE"]);
                    lookup.LookupValue = SafeTypeHandling.ConvertToString(row["REASON_CD"]);
                    lstSchemeSubType.Add(lookup);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstSchemeSubType;
        }

        public async Task<List<ChannelDOM>> ReadMajorSalesChannel()
        {
            List<ChannelDOM> lstMajorSalesChannel = new List<ChannelDOM>();
            try
            {
                OracleParameter[] arrParams = new[] {
                    new OracleParameter("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output),
                };
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_MAJOR_SALES_CHANNEL, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    ChannelDOM channel = new ChannelDOM();
                    channel.MajorSalesChannel = SafeTypeHandling.ConvertToString(row["MAJOR_SALES_CHANNEL"]);
                    lstMajorSalesChannel.Add(channel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstMajorSalesChannel;
        }

        public async Task<List<ChannelDOM>> ReadSalesChannel(string MajorSalesChannel)
        {
            List<ChannelDOM> lstSalesChannel = new List<ChannelDOM>();
            try
            {
                OracleParameter[] arrParams = new[] {
                    new OracleParameter("P_MAJOR_SALES_CHANNEL", OracleDbType.Varchar2, MajorSalesChannel, ParameterDirection.Input),
                    new OracleParameter("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output),
                };
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_SALES_CHANNEL, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    ChannelDOM channel = new ChannelDOM();
                    channel.MajorSalesChannel = SafeTypeHandling.ConvertToString(row["MAJOR_SALES_CHANNEL"]);
                    channel.SalesChannel = SafeTypeHandling.ConvertToString(row["SALES_CHANNEL"]);
                    lstSalesChannel.Add(channel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstSalesChannel;
        }

        public async Task<List<ChannelDOM>> ReadChannel(string MajorSalesChannel, string SalesChannel)
        {
            List<ChannelDOM> lstChannel = new List<ChannelDOM>();
            try
            {
                OracleParameter[] arrParams = new[] {
                    new OracleParameter("P_MAJOR_SALES_CHANNEL", OracleDbType.Varchar2, MajorSalesChannel, ParameterDirection.Input),
                    new OracleParameter("P_SALES_CHANNEL", OracleDbType.Varchar2, SalesChannel, ParameterDirection.Input),
                    new OracleParameter("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output),
                };
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_CHANNEL_CODE, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    ChannelDOM channel = new ChannelDOM();
                    channel.MajorSalesChannel = SafeTypeHandling.ConvertToString(row["MAJOR_SALES_CHANNEL"]);
                    channel.SalesChannel = SafeTypeHandling.ConvertToString(row["SALES_CHANNEL"]);
                    channel.ChannelCodeName = SafeTypeHandling.ConvertToString(row["SALES_CHANNEL_NAME"]);
                    channel.ChannelCode = SafeTypeHandling.ConvertToString(row["SALES_CHANNEL_CD"]);
                    lstChannel.Add(channel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstChannel;
        }

        public async Task<List<Distributor>> ReadBillingCode(Distributor distributor)
        {
            List<Distributor> lstBillingCodes = new List<Distributor>();
            try
            {
                OracleParameter[] arrParams = new[] {
                    new OracleParameter("P_ZONE", OracleDbType.Varchar2, distributor.Zone, ParameterDirection.Input),
                    new OracleParameter("P_REGION", OracleDbType.Varchar2, distributor.Region, ParameterDirection.Input),
                    new OracleParameter("P_BRANCH", OracleDbType.Varchar2, distributor.Branch, ParameterDirection.Input),
                    new OracleParameter("P_MAJOR_SALES_CHANNEL", OracleDbType.Varchar2, distributor.MajorSalesChannel, ParameterDirection.Input),
                    new OracleParameter("P_SALES_CHANNEL", OracleDbType.Varchar2, distributor.SalesChannel, ParameterDirection.Input),
                    new OracleParameter("P_CHANNEL", OracleDbType.Varchar2, distributor.ChannelCode, ParameterDirection.Input),
                    new OracleParameter("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output),
                };

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_BILLING_CODE, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Distributor dist = new Distributor();
                    dist.Zone = SafeTypeHandling.ConvertToString(row["ZONE"]);
                    dist.Region = SafeTypeHandling.ConvertToString(row["REGION"]);
                    dist.Branch = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    dist.BillingCode = SafeTypeHandling.ConvertToString(row["LOGIN_ID"]);
                    dist.AccountName = SafeTypeHandling.ConvertToString(row["LOGIN_NAME"]);
                    dist.MajorSalesChannel = SafeTypeHandling.ConvertToString(row["MAJOR_SALES_CHANNEL"]);
                    dist.SalesChannel = SafeTypeHandling.ConvertToString(row["SALES_CHANNEL"]);
                    dist.ChannelCode = SafeTypeHandling.ConvertToString(row["PRICING_GROUP_CODE"]);
                    dist.ChannelCodeName = SafeTypeHandling.ConvertToString(row["SALES_CHANNEL_NAME"]);
                    lstBillingCodes.Add(dist);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstBillingCodes;
        }

        public async Task<List<Model>> ReadProduct()
        {
            List<Model> lstProduct = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new[] {
                    new OracleParameter("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output),
                };
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.READ_PRODUCT_CATEGORY, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    lstProduct.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstProduct;
        }
        public async Task<List<Model>> ReadSubProduct(string Product)
        {
            List<Model> lstSubProduct = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(Product))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = Product;
                }
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.READ_SUB_PRODUCT_CATEGORY, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    lstSubProduct.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstSubProduct;
        }
        public async Task<List<Model>> ReadProductLevel3(Model model)
        {
            List<Model> lstProductLevel3 = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[3];
                arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.Product))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = model.Product;
                }
                arrParams[1] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.SubProduct))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = model.SubProduct;
                }
                arrParams[2] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[2].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.READ_MODEL_PRODUCT_LEVEL3, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    _model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    lstProductLevel3.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstProductLevel3;
        }
        public async Task<List<Model>> ReadModelCategory(Model model)
        {
            List<Model> lstModelCategory = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[4];
                arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.Product))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = model.Product;
                }
                arrParams[1] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.SubProduct))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = model.SubProduct;
                }
                arrParams[2] = new OracleParameter("P_PRODUCT_LEVEL3", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ProductLevel3))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = model.ProductLevel3;
                }
                arrParams[3] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[3].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.READ_MODEL_CATEGORY, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    _model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    _model.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
                    lstModelCategory.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstModelCategory;
        }
        public async Task<List<Model>> ReadModelSeries(Model model)
        {
            List<Model> lstModelSeries = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[5];
                arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.Product))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = model.Product;
                }
                arrParams[1] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.SubProduct))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = model.SubProduct;
                }
                arrParams[2] = new OracleParameter("P_PRODUCT_LEVEL3", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ProductLevel3))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = model.ProductLevel3;
                }
                arrParams[3] = new OracleParameter("P_MODEL_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelCategory))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = model.ModelCategory;
                }
                arrParams[4] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[4].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.READ_MODEL_SERIES, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    _model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    _model.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
                    _model.ModelSeries = SafeTypeHandling.ConvertToString(row["MODEL_SERIES"]);
                    lstModelSeries.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstModelSeries;
        }

        public async Task<List<Model>> ReadModelSubCategory(Model model)
        {
            List<Model> lstModelSubCategory = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.Product))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = model.Product;
                }
                arrParams[1] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.SubProduct))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = model.SubProduct;
                }
                arrParams[2] = new OracleParameter("P_PRODUCT_LEVEL3", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ProductLevel3))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = model.ProductLevel3;
                }
                arrParams[3] = new OracleParameter("P_MODEL_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelCategory))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = model.ModelCategory;
                }
                arrParams[4] = new OracleParameter("P_SERIES", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelSeries))
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = model.ModelSeries;
                }
                arrParams[5] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[5].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.READ_MODEL_SUB_CATEGORY, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    _model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    _model.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
                    _model.ModelSeries = SafeTypeHandling.ConvertToString(row["MODEL_SERIES"]);
                    _model.ModelSubCategory = SafeTypeHandling.ConvertToString(row["MODEL_SUB_CATEGORY"]);
                    lstModelSubCategory.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstModelSubCategory;
        }

        public async Task<List<Model>> ReadModelStarRating(Model model)
        {
            List<Model> lstModelStarRating = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[7];
                arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.Product))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = model.Product;
                }
                arrParams[1] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.SubProduct))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = model.SubProduct;
                }
                arrParams[2] = new OracleParameter("P_PRODUCT_LEVEL3", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ProductLevel3))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = model.ProductLevel3;
                }
                arrParams[3] = new OracleParameter("P_MODEL_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelCategory))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = model.ModelCategory;
                }
                arrParams[4] = new OracleParameter("P_SERIES", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelSeries))
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = model.ModelSeries;
                }
                arrParams[5] = new OracleParameter("P_MODEL_SUB_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelSubCategory))
                {
                    arrParams[5].Value = DBNull.Value;
                }
                else
                {
                    arrParams[5].Value = model.ModelSubCategory;
                }
                arrParams[6] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[6].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.READ_MODEL_STAR_RATING, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    _model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    _model.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
                    _model.ModelSeries = SafeTypeHandling.ConvertToString(row["MODEL_SERIES"]);
                    _model.ModelSubCategory = SafeTypeHandling.ConvertToString(row["MODEL_SUB_CATEGORY"]);
                    _model.StarRating = SafeTypeHandling.ConvertToString(row["STAR_RATING"]);
                    lstModelStarRating.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstModelStarRating;
        }
        public async Task<List<Model>> ReadModelYear(Model model)
        {
            List<Model> lstModelYear = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[8];
                arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.Product))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = model.Product;
                }
                arrParams[1] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.SubProduct))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = model.SubProduct;
                }
                arrParams[2] = new OracleParameter("P_PRODUCT_LEVEL3", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ProductLevel3))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = model.ProductLevel3;
                }
                arrParams[3] = new OracleParameter("P_MODEL_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelCategory))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = model.ModelCategory;
                }
                arrParams[4] = new OracleParameter("P_SERIES", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelSeries))
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = model.ModelSeries;
                }
                arrParams[5] = new OracleParameter("P_MODEL_SUB_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelSubCategory))
                {
                    arrParams[5].Value = DBNull.Value;
                }
                else
                {
                    arrParams[5].Value = model.ModelSubCategory;
                }
                arrParams[6] = new OracleParameter("P_STAR_RATING", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.StarRating))
                {
                    arrParams[6].Value = DBNull.Value;
                }
                else
                {
                    arrParams[6].Value = model.StarRating;
                }
                arrParams[7] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[7].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.READ_MODEL_YEARS, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    _model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    _model.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
                    _model.ModelSeries = SafeTypeHandling.ConvertToString(row["MODEL_SERIES"]);
                    _model.ModelSubCategory = SafeTypeHandling.ConvertToString(row["MODEL_SUB_CATEGORY"]);
                    _model.StarRating = SafeTypeHandling.ConvertToString(row["STAR_RATING"]);
                    _model.ModelYear = SafeTypeHandling.ConvertToString(row["YEAR"]);
                    lstModelYear.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstModelYear;
        }

        public async Task<List<Model>> ReadModel(Model model)
        {
            List<Model> lstModel = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[9];
                arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.Product))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = model.Product;
                }
                arrParams[1] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.SubProduct))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = model.SubProduct;
                }
                arrParams[2] = new OracleParameter("P_PRODUCT_LEVEL3", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ProductLevel3))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = model.ProductLevel3;
                }
                arrParams[3] = new OracleParameter("P_MODEL_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelCategory))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = model.ModelCategory;
                }
                arrParams[4] = new OracleParameter("P_SERIES", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelSeries))
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = model.ModelSeries;
                }
                arrParams[5] = new OracleParameter("P_MODEL_SUB_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelSubCategory))
                {
                    arrParams[5].Value = DBNull.Value;
                }
                else
                {
                    arrParams[5].Value = model.ModelSubCategory;
                }
                arrParams[6] = new OracleParameter("P_STAR_RATING", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.StarRating))
                {
                    arrParams[6].Value = DBNull.Value;
                }
                else
                {
                    arrParams[6].Value = model.StarRating;
                }
                arrParams[7] = new OracleParameter("P_MODEL_YEAR", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelYear))
                {
                    arrParams[7].Value = DBNull.Value;
                }
                else
                {
                    arrParams[7].Value = model.ModelYear;
                }
                arrParams[8] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[8].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.READ_MODELS, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    _model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    _model.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
                    _model.ModelSeries = SafeTypeHandling.ConvertToString(row["MODEL_SERIES"]);
                    _model.ModelSubCategory = SafeTypeHandling.ConvertToString(row["MODEL_SUB_CATEGORY"]);
                    _model.StarRating = SafeTypeHandling.ConvertToString(row["STAR_RATING"]);
                    _model.ModelYear = SafeTypeHandling.ConvertToString(row["YEAR"]);
                    _model.ModelNo = SafeTypeHandling.ConvertToString(row["MODEL_CODE"]);
                    _model.ModelDesc = SafeTypeHandling.ConvertToString(row["MODEL_DESC"]);
                    lstModel.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstModel;
        }

        public async Task<List<Model>> ReadFilterProduct(Model model)
        {
            List<Model> lstModel = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[11];
                arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.Product))
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = model.Product;
                }
                arrParams[1] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.SubProduct))
                {
                    arrParams[1].Value = DBNull.Value;
                }
                else
                {
                    arrParams[1].Value = model.SubProduct;
                }
                arrParams[2] = new OracleParameter("P_PRODUCT_LEVEL3", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ProductLevel3))
                {
                    arrParams[2].Value = DBNull.Value;
                }
                else
                {
                    arrParams[2].Value = model.ProductLevel3;
                }
                arrParams[3] = new OracleParameter("P_MODEL_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelCategory))
                {
                    arrParams[3].Value = DBNull.Value;
                }
                else
                {
                    arrParams[3].Value = model.ModelCategory;
                }
                arrParams[4] = new OracleParameter("P_SERIES", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelSeries))
                {
                    arrParams[4].Value = DBNull.Value;
                }
                else
                {
                    arrParams[4].Value = model.ModelSeries;
                }
                arrParams[5] = new OracleParameter("P_MODEL_SUB_CATEGORY", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelSubCategory))
                {
                    arrParams[5].Value = DBNull.Value;
                }
                else
                {
                    arrParams[5].Value = model.ModelSubCategory;
                }
                arrParams[6] = new OracleParameter("P_STAR_RATING", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.StarRating))
                {
                    arrParams[6].Value = DBNull.Value;
                }
                else
                {
                    arrParams[6].Value = model.StarRating;
                }
                arrParams[7] = new OracleParameter("P_MODEL_YEAR", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelYear))
                {
                    arrParams[7].Value = DBNull.Value;
                }
                else
                {
                    arrParams[7].Value = model.ModelYear;
                }
                arrParams[8] = new OracleParameter("P_MODEL_NO", OracleDbType.Varchar2);
                if (string.IsNullOrEmpty(model.ModelNo))
                {
                    arrParams[8].Value = DBNull.Value;
                }
                else
                {
                    arrParams[8].Value = model.ModelNo;
                }
                arrParams[9] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[9].Direction = ParameterDirection.Output;
                arrParams[10] = new OracleParameter("P_SCHEME_TYPE", OracleDbType.Varchar2);
                arrParams[10].Value = model.SchemeType;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.DFISchemeRequest.READ_FILTER_PRODUCTS, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    if (row.Table.Columns.IndexOf("PRODUCT") >= 0)
                    {
                        _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    }
                    if (row.Table.Columns.IndexOf("SUB_PRODUCT") >= 0)
                    {
                        _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    }
                    if (row.Table.Columns.IndexOf("PRODUCT_LEVEL3") >= 0)
                    {
                        _model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    }
                    if (row.Table.Columns.IndexOf("MODEL_CATEGORY") >= 0)
                    {
                        _model.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
                    }
                    if (row.Table.Columns.IndexOf("MODEL_SERIES") >= 0)
                    {
                        _model.ModelSeries = SafeTypeHandling.ConvertToString(row["MODEL_SERIES"]);
                    }
                    if (row.Table.Columns.IndexOf("MODEL_SUB_CATEGORY") >= 0)
                    {
                        _model.ModelSubCategory = SafeTypeHandling.ConvertToString(row["MODEL_SUB_CATEGORY"]);
                    }
                    if (row.Table.Columns.IndexOf("STAR_RATING") >= 0)
                    {
                        _model.StarRating = SafeTypeHandling.ConvertToString(row["STAR_RATING"]);
                    }
                    if (row.Table.Columns.IndexOf("YEAR") >= 0)
                    {
                        _model.ModelYear = SafeTypeHandling.ConvertToString(row["YEAR"]);
                    }
                    if (row.Table.Columns.IndexOf("MODEL_CODE") >= 0)
                    {
                        _model.ModelNo = SafeTypeHandling.ConvertToString(row["MODEL_CODE"]);
                    }
                    if (row.Table.Columns.IndexOf("MODEL_DESC") >= 0)
                    {
                        _model.ModelDesc = SafeTypeHandling.ConvertToString(row["MODEL_DESC"]);
                    }
                    lstModel.Add(_model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return lstModel;
        }

        public async Task<APIResponse> UpdateModels(List<Model> lstModel)
        {
            int counter = 0;
            APIResponse response = null;
            foreach (var model in lstModel)
            {
                try
                {
                    OracleParameter[] arrParams = new OracleParameter[13];
                    arrParams[0] = new OracleParameter("P_PRODUCT", OracleDbType.Varchar2, 250);
                    arrParams[0].Value = model.Product;
                    arrParams[1] = new OracleParameter("P_SUB_PRODUCT", OracleDbType.Varchar2, 250);
                    arrParams[1].Value = model.SubProduct;
                    arrParams[2] = new OracleParameter("P_MODEL_CATEGORY", OracleDbType.Varchar2, 250);
                    arrParams[2].Value = model.ModelCategory;
                    arrParams[3] = new OracleParameter("P_MODEL_SUB_CATEGORY", OracleDbType.Varchar2, 250);
                    arrParams[3].Value = model.ModelSubCategory;
                    arrParams[4] = new OracleParameter("P_MODEL_SERIES", OracleDbType.Varchar2, 250);
                    arrParams[4].Value = model.ModelSeries;
                    arrParams[5] = new OracleParameter("P_STAR_RATING", OracleDbType.Varchar2, 250);
                    arrParams[5].Value = model.StarRating;
                    arrParams[6] = new OracleParameter("P_YEAR", OracleDbType.Varchar2, 250);
                    arrParams[6].Value = model.ModelYear;
                    arrParams[7] = new OracleParameter("P_MODEL_CODE", OracleDbType.Varchar2, 250);
                    arrParams[7].Value = model.ModelNo;
                    arrParams[8] = new OracleParameter("P_MODEL_DESC", OracleDbType.Varchar2, 250);
                    arrParams[8].Value = model.ModelDesc;
                    arrParams[9] = new OracleParameter("P_PRODUCT_LEVEL3", OracleDbType.Varchar2, 250);
                    arrParams[9].Value = model.ProductLevel3;
                    arrParams[10] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 250);
                    arrParams[10].Value = model.Status;
                    arrParams[11] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 250);
                    arrParams[11].Value = model.CreatedBy;
                    arrParams[12] = new OracleParameter("P_OUTPUT", OracleDbType.Varchar2, 500);
                    arrParams[12].Direction = ParameterDirection.Output;

                    DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.ModelMaster.UPDATE_MODELS, arrParams);
                    counter++;
                }
                catch (Exception ex)
                {
                    response = new APIResponse
                    {
                        Status = Utilities.ERROR,
                        StatusDesc = ex.ToString()
                    };
                }
            }

            if (counter == lstModel.Count)
            {
                response = new APIResponse
                {
                    Status = Utilities.SUCCESS,
                    StatusDesc = "Record submitted successfully..."
                };
            }
            else
            {
                response = new APIResponse
                {
                    Status = Utilities.SUCCESS,
                    StatusDesc = "Record partially submitted successfully..."
                };
            }

            return response;
        }

        public List<Model> ReadAllModel()
        {
            List<Model> lstModel = new List<Model>();
            try
            {
                OracleParameter[] arrParams = new[] {
                    new OracleParameter("P_OUT", OracleDbType.RefCursor, ParameterDirection.Output),
                };
                DataTable dtData = _commonDB.getDataTableStoredProc(DatabaseConstants.DFIScheme.ModelMaster.READ_ALL_MODELS, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Model _model = new Model();
                    _model.Product = SafeTypeHandling.ConvertToString(row["PRODUCT"]);
                    _model.SubProduct = SafeTypeHandling.ConvertToString(row["SUB_PRODUCT"]);
                    _model.ModelCategory = SafeTypeHandling.ConvertToString(row["MODEL_CATEGORY"]);
                    _model.ModelSubCategory = SafeTypeHandling.ConvertToString(row["MODEL_SUB_CATEGORY"]);
                    _model.ModelSeries = SafeTypeHandling.ConvertToString(row["MODEL_SERIES"]);
                    _model.StarRating = SafeTypeHandling.ConvertToString(row["STAR_RATING"]);
                    _model.ModelYear = SafeTypeHandling.ConvertToString(row["YEAR"]);
                    _model.ModelPrefix = SafeTypeHandling.ConvertToString(row["MODEL_PREFIX"]);
                    _model.ModelNo = SafeTypeHandling.ConvertToString(row["MODEL_CODE"]);
                    _model.ModelDesc = SafeTypeHandling.ConvertToString(row["MODEL_DESC"]);
                    _model.ProductLevel3 = SafeTypeHandling.ConvertToString(row["PRODUCT_LEVEL3"]);
                    _model.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    lstModel.Add(_model);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                throw;
            }
            return lstModel;
        }


        public async Task<APIResponse> UserAU_Mapping(Users users)
        {
            APIResponse response = null;
            try
            {
                OracleParameter[] arrParams = new OracleParameter[6];
                arrParams[0] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, 100);
                arrParams[0].Value = users.UserId;
                arrParams[1] = new OracleParameter("P_BRANCH", OracleDbType.Varchar2, 500);
                arrParams[1].Value = users.Branch.BranchCode;
                arrParams[2] = new OracleParameter("P_STATUS", OracleDbType.Varchar2, 100);
                arrParams[2].Value = users.Status;
                arrParams[3] = new OracleParameter("P_CREATED_BY", OracleDbType.Varchar2, 100);
                arrParams[3].Value = users.CreatedBy;
                arrParams[4] = new OracleParameter("P_OUT_STATUS", OracleDbType.Varchar2, 100);
                arrParams[4].Direction = ParameterDirection.Output;
                arrParams[5] = new OracleParameter("P_OUT_STATUS_DESC", OracleDbType.Varchar2, 500);
                arrParams[5].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.SCHEME_USER_AU_MAPPING_UPDATE, arrParams);

                response = new APIResponse
                {
                    Status = SafeTypeHandling.ConvertToString(arrParams[4].Value),
                    StatusDesc = SafeTypeHandling.ConvertToString(arrParams[5].Value)
                };
                return response;
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                return response = new APIResponse
                {
                    Status = Utilities.ERROR,
                    StatusDesc = ex.ToString()
                };
            }
        }

        public async Task<List<Users>> ReadUserAU_Mapping(Users users)
        {
            List<Users> lstAUMapping = new List<Users>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_USER_ID", OracleDbType.Varchar2, 100);
                if (users.UserId == 0)
                {
                    arrParams[0].Value = DBNull.Value;
                }
                else
                {
                    arrParams[0].Value = users.UserId;
                }
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.SCHEME_USER_AU_MAPPING_READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Users _user = new Users();
                    _user.UserId = SafeTypeHandling.ConvertStringToInt32(row["USER_ID"]);
                    _user.LoginID = SafeTypeHandling.ConvertToString(row["LOGIN_ID"]);
                    _user.LoginName = SafeTypeHandling.ConvertToString(row["LOGIN_NAME"]);
                    _user.Branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);

                    _user.Role.RoleId = SafeTypeHandling.ConvertStringToInt32(row["USER_ROLE_ID"]);
                    _user.Role.RoleName = SafeTypeHandling.ConvertToString(row["USER_ROLE_NAME"]);

                    _user.Status = SafeTypeHandling.ConvertToString(row["STATUS"]);
                    _user.StatusDesc = SafeTypeHandling.ConvertToString(row["STATUS_DESC"]);
                    _user.LastUpdatedBy = SafeTypeHandling.ConvertStringToInt32(row["LAST_UPDATED_BY"]);
                    _user.LastUpdatedByName = SafeTypeHandling.ConvertToString(row["LAST_UPDATED_BY_NAME"]);
                    _user.LastUpdatedDate = SafeTypeHandling.ConvertToDateTime(row["LAST_UPDATED_DATE"]);
                    lstAUMapping.Add(_user);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                throw;
            }
            return lstAUMapping;
        }

        public async Task<List<Users>> ReadUsersForAUMappingByRoleId(int roleId)
        {
            List<Users> lstUsers = new List<Users>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_ROLEID", OracleDbType.Varchar2);
                arrParams[0].Value = roleId;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;

                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.READ_USERS_BY_ROLE, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Users users = new Users();
                    users.UserId = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    users.LoginID = SafeTypeHandling.ConvertToString(row["LOGIN_ID"]);
                    users.LoginName = SafeTypeHandling.ConvertToString(row["LOGIN_NAME"]);
                    users.Zone.Region.Branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    users.Zone.Region.RegionCode = SafeTypeHandling.ConvertToString(row["REGION"]);
                    users.Role.RoleId = SafeTypeHandling.ConvertStringToInt32(row["USER_ROLE_ID"]);
                    users.Role.RoleName = SafeTypeHandling.ConvertToString(row["USER_ROLE_NAME"]);
                    lstUsers.Add(users);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                throw;
            }
            return lstUsers;
        }

        public async Task<List<Users>> ReadApprovers(string approverName)
        {
            List<Users> lstUsers = new List<Users>();
            try
            {
                OracleParameter[] arrParams = new OracleParameter[2];
                arrParams[0] = new OracleParameter("P_APPROVER_NAME", OracleDbType.Varchar2);
                arrParams[0].Value = approverName;
                arrParams[1] = new OracleParameter("P_OUT", OracleDbType.RefCursor);
                arrParams[1].Direction = ParameterDirection.Output;
                DataTable dtData = await _commonDB.getDataTableStoredProcAsync(DatabaseConstants.DFIScheme.Utilities.APPROVERS_READ, arrParams);
                foreach (DataRow row in dtData.Rows)
                {
                    Users users = new Users();
                    users.UserId = SafeTypeHandling.ConvertStringToInt32(row["ID"]);
                    users.LoginID = SafeTypeHandling.ConvertToString(row["LOGIN_ID"]);
                    users.LoginName = SafeTypeHandling.ConvertToString(row["LOGIN_NAME"]);
                    users.Zone.Region.Branch.BranchCode = SafeTypeHandling.ConvertToString(row["BRANCH"]);
                    users.Zone.Region.RegionCode = SafeTypeHandling.ConvertToString(row["REGION"]);
                    users.Role.RoleId = SafeTypeHandling.ConvertStringToInt32(row["USER_ROLE_ID"]);
                    users.Role.RoleName = SafeTypeHandling.ConvertToString(row["USER_ROLE_NAME"]);
                    lstUsers.Add(users);
                }
            }
            catch (Exception ex)
            {
                Utilities.CreateLogFile(Utilities.PROJECTS.SchemeAII_DFI, ex);
                throw;
            }
            return lstUsers;
        }




    }
}
