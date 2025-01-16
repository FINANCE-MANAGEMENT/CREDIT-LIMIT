using DealerNetAPI.DomainObject.SchemeAutomation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class SchemeRequest : Base
    {
        public SchemeRequest()
        {
            ProductDetail = new Model();
            ProductDetailList = new List<Model>();
            BillingCodeDetailList = new List<Distributor>();
            SchemeApproverList = new List<Approval>();
            SchemeSlabList = new List<Slab_TargetDOM>();
        }
        public Int64? Id { get; set; }
        public int SchemeId { get; set; }
        public string SchemeRefNo { get; set; }
        public string SchemeName { get; set; }
        public string FundSource { get; set; }
        public string SchemeType { get; set; }
        public string SchemeReasonCode { get; set; }
        public string PayoutType { get; set; }
        public DateTime? SchemeFromDate { get; set; }
        public DateTime? SchemeToDate { get; set; }

        public string Zone { get; set; }
        public string Region { get; set; }
        public string Branch { get; set; }
        public string MajorSalesChannel { get; set; }
        public string SalesChannel { get; set; }
        public string Channel { get; set; }
        public string HeaderCode { get; set; }
        public string BillingCode { get; set; }
        public string AccountName { get; set; }

        public Model ProductDetail { get; set; }

        public List<Distributor> BillingCodeDetailList { get; set; }
        public List<Model> ProductDetailList { get; set; }

        public DateTime? SchemeSellInFrom { get; set; }
        public DateTime? SchemeSellInTo { get; set; }
        public DateTime? SchemeSelloutFrom { get; set; }
        public DateTime? SchemeSelloutTo { get; set; }

        public DateTime? LeftoverSellInFrom { get; set; }
        public DateTime? LeftoverSellInTo { get; set; }
        public DateTime? LeftoverSelloutFrom { get; set; }
        public DateTime? LeftoverSelloutTo { get; set; }

        public string SettlementCriteria { get; set; }
        public string EligibilityCalculation { get; set; }
        public string PayoutCalculation { get; set; }
        public string GrowthType { get; set; }
        public string Growth { get; set; }
        public string TnCIds { get; set; }
        public string URL { get; set; }

        public string ApproverEmpCode { get; set; }

        public List<Approval> SchemeApproverList { get; set; }
        public string ProcessType { get; set; }

        public List<Slab_TargetDOM> SchemeSlabList { get; set; }

    }
}
