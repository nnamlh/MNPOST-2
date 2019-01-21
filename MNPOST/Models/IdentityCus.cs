using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Models
{
    public class CustomerInfo
    {
        public string CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerGroupID { get; set; }
        public string CustomerGroupName { get; set; }
        public string CreateDate { get; set; }
        public string Address { get; set; }
        public string DistrictID { get; set; }
        public string ProvinceID { get; set; }
        public string CountryID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string PostOfficeID { get; set; }
        public string WardID { get; set; }
        public string UserLogin { get; set; }
        public string CustomerGroupCode { get; set; }
        public string Deputy { get; set; }
    }
}