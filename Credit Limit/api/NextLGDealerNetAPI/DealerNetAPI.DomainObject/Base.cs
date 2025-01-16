using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Base
    {
        public Int32? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Int32? LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public string Remarks { get; set; }
        public string LocalIP { get; set; }
        public string PublicIP { get; set; }
        public string AdminRemarks { get; set; }

    }
}
