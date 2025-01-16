using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Approval : Base
    {
        public string ApprovalSeq { get; set; }
        public string SchemeRefNo { get; set; }
        public int? ApproverUserId { get; set; }
        public string ApproverEmpCode { get; set; }
        public string ApproverName { get; set; }
        public string ApproverRoleName { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalComments { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string URL { get; set; }
        public string FixedApprover { get; set; }
    }
}
