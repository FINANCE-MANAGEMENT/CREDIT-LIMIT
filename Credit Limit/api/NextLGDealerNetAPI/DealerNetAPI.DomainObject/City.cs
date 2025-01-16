using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class City : Base
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string CityCode { get; set; }
    }
}
