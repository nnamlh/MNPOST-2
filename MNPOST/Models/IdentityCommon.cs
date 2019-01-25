using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Models
{
    public class IdentityCommon
    {
    }
    public class ReturnValue
    {
        public int returnvalue { get; set; }
    }
    public class ReturnDate
    {
        public DateTime Ngay { get; set; }
    }
    public class CusCom
    {
        public string CustomerGroupID { get; set; }
        public string CustomerGroupCode { get; set; }
    }
    public class MailerCom
    {
        public string MailerID { get; set; }
        public Nullable<decimal> CommissionAmt { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<double> CommissionPercent { get; set; }
    }
    public class ResultInfo
    {
        public int error { get; set; }

        public string msg { get; set; }

        public Object data { get; set; }

     
    }

    public class ResultWithPaging : ResultInfo
    {
        public int page { get; set; }

        public int toltalSize { get; set; }

        public int pageSize { get; set; }
    }

    //
    public class CustomerInfoResult
    {
        public string code { get; set; }
        public string name { get; set; }

        public string address { get; set; }

        public string phone { get; set; }

        public string provinceId { get; set; }

        public string districtId { get; set; }

        public string wardId { get; set; }
    }

    public class CommonData
    {
        public string code { get; set; }

        public string name { get; set; }
    }

    public class AddressCommom : CommonData
    {
        public bool?  vsvx { get; set; }
    }

    public class ItemPriceCommon : CommonData
    {
        public decimal? price { get; set; }

        public bool? choose { get; set; }

        public bool? percent { get; set; }
    }

    public class EmployeeInfoCommon
    {
        public string code { get; set; }

        public string name { get; set; }

        public string phone { get; set; }

        public string email { get; set; }
    }

    //
    public class UserInfo
    {
        public string user { get; set; }

        public string level { get; set; }

        public string groupId { get; set; }

        public List<string> postOffices { get; set; }

        public string employeeId { get; set; }

        public string fullName { get; set; }

        public string currentPost { get; set; }
    }

    public class MyAddressInfo
    {
        public string address { get; set; }
        public string phone { get; set; }

        public string email { get; set; }

        public string ward { get; set; }

        public string district { get; set; }

        public string province { get; set; }

        public string name { get; set; }
    }

   
}