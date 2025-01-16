using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class TermsConditions : Base
    {
        public int Id { get; set; }
        public string TermCondition { get; set; }
        public string SchemeType { get; set; }
        public string SystemName { get; set; }
        public bool isChecked { get; set; }
        public string SchemeRefNo { get; set; }
    }
}
