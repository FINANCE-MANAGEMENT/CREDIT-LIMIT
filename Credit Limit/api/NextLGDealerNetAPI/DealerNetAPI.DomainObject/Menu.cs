using System;
using System.Collections.Generic;
using System.Text;

namespace DealerNetAPI.DomainObject
{
    public class Menu : Base
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string SubMenuName { get; set; }
        public string MenuDesc { get; set; }
        public int MenuParentId { get; set; }
        public string MenuParentName { get; set; }
        public string MenuLevel { get; set; }
        public string MenuURL { get; set; }
        public int DisplaySeq { get; set; }

        public string Target { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public string TaskStatus { get; set; }
        public int TaskMapId { get; set; }
        public bool isChecked { get; set; }
        public string ApplicationName { get; set; }
        public string LoginID { get; set; }
        public int SubMenuCount { get; set; }

        public string RouteLinkVisible { get; set; }

    }
}
