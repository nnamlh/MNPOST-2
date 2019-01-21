using MNPOSTAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOSTAPI.Controllers.mobile.user
{
    public class UserInfoResult : ResultInfo
    {
        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Function { get; set; }

        public string EmployeeCode { get; set; }

        public string PostOfficeID { get; set; }
    }
}