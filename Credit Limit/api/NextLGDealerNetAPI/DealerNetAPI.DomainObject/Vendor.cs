using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Vendor : Base
    {
        public Vendor()
        {
            State = new State();
        }
        public Int64? Id { get; set; }
        public Int64? VendorId { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string CTRL_AU { get; set; }
        public string PIC_Code { get; set; }
        public string PIC_Name { get; set; }
        public string PIC_MobileNo { get; set; }
        public int RowNumber { get; set; }

        public string PAN_No { get; set; }
        public string GSTIN_No { get; set; }
        public string MSMERegNo { get; set; }
        public string EnterprisesType { get; set; }
        public string MajorActivity { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public State State { get; set; }
        public string PinCode { get; set; }
        public string BranchAddress { get; set; }
        public string ProfileDocPath { get; set; }
    }
}
