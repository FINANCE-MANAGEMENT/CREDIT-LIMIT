using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class SmsJsonData
    {
        public string keyword { get; set; }
        public string timeStamp { get; set; }
        public List<SmsData> dataSet { get; set; }

    }
}
