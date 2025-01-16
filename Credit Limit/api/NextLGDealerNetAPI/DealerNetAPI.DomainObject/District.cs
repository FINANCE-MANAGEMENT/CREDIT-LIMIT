using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class District : Base
    {
        public District()
        {
            State = new State();
        }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public State State { get; set; }
    }
}
