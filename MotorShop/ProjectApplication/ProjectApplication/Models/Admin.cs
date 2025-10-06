using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public String AdminName { get; set; }
        public String AdminPassword { get; set; }
        public String AdminEmail { get; set; }
        public String AdminPhone { get; set; }
        public String AdminAddress { get; set; }
    }
}