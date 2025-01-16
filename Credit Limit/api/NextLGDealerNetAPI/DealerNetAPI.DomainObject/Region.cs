using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Region
    {
        public Region()
        {
            Branch = new Branch();
        }
        public string RegionCode { get; set; }
        public bool isSelected { get; set; }
        public Branch Branch { get; set; }
    }
}
