using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Lookup : Base
    {

        public int Id { get; set; }
        public int LookupId { get; set; }
        public string LookupName { get; set; }
        public string LookupInfo { get; set; }
        public string LookupValue { get; set; }
        public string LookupDesc { get; set; }
        public string LookupDispSeq { get; set; }
        public string SystemName { get; set; }
        public bool isChecked { get; set; }
        public bool isUsed { get; set; }
        public string Value1 { get; set; }
    }
}
