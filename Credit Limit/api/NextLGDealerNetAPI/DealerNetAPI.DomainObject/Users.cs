using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    [Serializable]
    public class Users : Base
    {
        public Users()
        {
            Role = new Role();
            Zone = new Zone();
            Branch = new Branch();
        }
        public int UserId { get; set; }

        public string LoginID { get; set; }
        public string LoginName { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MobileNo { get; set; }

        public string EmailId { get; set; }

        public Role Role { get; set; }
        public Zone Zone { get; set; }

        public Boolean IsActive { get; set; }

        public string IsHeaderActive { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public int StateID { get; set; }
        public bool ChangePwdStatus { get; set; }

        public DateTime ChangePwdDate { get; set; }

        public List<Tasks> TaskList { get; set; }

        public string ClientIP { get; set; }

        public Branch Branch { get; set; }

        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }


    }
}
