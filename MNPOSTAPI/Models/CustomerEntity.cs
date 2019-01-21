using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MNPOSTCOMMON;

namespace MNPOSTAPI.Models
{
   
    public class AddCustomerRequest
    {
        public string email { get; set; }

        public string phone { get; set; }

        public string fullName { get; set; }

        public string clientUser { get; set; }
    }


    public class AddCustomerResult : ResultInfo
    {
        public string cusId { get; set; }
    }


    public class CustomerInfo
    {
        public string CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string DistrictID { get; set; }
        public string ProvinceID { get; set; }
        public string CountryID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WardID { get; set; }
        public string Deputy { get; set; }
    }

}