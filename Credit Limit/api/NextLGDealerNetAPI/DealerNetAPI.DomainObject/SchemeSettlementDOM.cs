using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class SchemeSettlementDOM : Base
    {
        public SchemeSettlementDOM()
        {
            ProductDetail = new Model();
        }
        public Int64? Id { get; set; }
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
        public string ChannelName { get; set; }
        public string HeaderCode { get; set; }
        public string BillingCode { get; set; }
        public string AccountName { get; set; }
        public Model ProductDetail { get; set; }


        public string LeftoverSellIn_Normal { get; set; }
        public string LeftoverSellIn_Display { get; set; }
        public string LeftoverSellIn_Total { get; set; }
        public string Leftover_Sellout { get; set; }
        public string Leftover_Leftover { get; set; }

        public string SchemeSellIn_Normal { get; set; }
        public string SchemeSellIn_Display { get; set; }
        public string SchemeSellIn_Total { get; set; }
        public string Scheme_TotalStock { get; set; }
        public string Scheme_Sellout { get; set; }

        public string Scheme_Eligibility { get; set; }
        public string Scheme_Scheme { get; set; }
        public string Scheme_LastYearBase { get; set; }
        public string Scheme_GrowthPercentage { get; set; }
        public string Target { get; set; }
        public string Scheme_Payout { get; set; }

    }
}
