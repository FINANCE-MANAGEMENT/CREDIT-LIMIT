using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Branch
    {
        public string ZoneCode { get; set; }
        public string RegionCode { get; set; }
        public string BranchCode { get; set; }
        public string CTRL_AU { get; set; }
        public bool isSelected { get; set; }
        public string BranchAddress { get; set; }
        public Int32? CreatedBy { get; set; }
    }
}
