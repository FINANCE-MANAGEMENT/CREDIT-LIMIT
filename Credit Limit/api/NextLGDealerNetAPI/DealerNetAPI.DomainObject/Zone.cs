using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Zone
    {
        public Zone()
        {
            Region = new Region();
        }
        public string ZoneCode { get; set; }
        public bool isSelected { get; set; }
        public Region Region { get; set; }
    }
}
