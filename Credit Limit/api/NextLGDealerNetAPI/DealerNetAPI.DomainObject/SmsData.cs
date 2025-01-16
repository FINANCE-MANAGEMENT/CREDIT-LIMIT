using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class SmsData
    {
        public string UNIQUE_ID { get; set; }
        public string MESSAGE { get; set; }
        public string OA { get; set; }
        public string MSISDN { get; set; }
        public string CHANNEL { get; set; }
        public string CAMPAIGN_NAME { get; set; }
        public string CIRCLE_NAME { get; set; }
        public string USER_NAME { get; set; }
        public string DLT_TM_ID { get; set; }
        public string DLT_CT_ID { get; set; }
        public string DLT_PE_ID { get; set; }
        public string LANG_ID { get; set; }

        public string SMS_REG_NAME { get; set; }
        public string SERVICE_URL { get; set; }

        public string HEADER_NAME { get; set; }
        public string TEMPLATE_ID { get; set; }
        public string API_BASE_URL { get; set; }
        public string API_KEY { get; set; }


    }
}
