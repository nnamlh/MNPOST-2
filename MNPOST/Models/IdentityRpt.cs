using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Models
{
    public class MailerRpt
    {
        public string MailerID { get; set; }
        public string PostOfficeAcceptID { get; set; }
        public string SenderID { get; set; }
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderWardID { get; set; }
        public string SenderDistrictID { get; set; }
        public string SenderProvinceID { get; set; }
        public string SenderPhone { get; set; }
        public string RecieverName { get; set; }
        public string RecieverAddress { get; set; }
        public string RecieverWardID { get; set; }
        public string RecieverDistrictID { get; set; }
        public string RecieverProvinceID { get; set; }
        public string RecieverPhone { get; set; }
        public string EmployeeAcceptID { get; set; }
        public string SenderProvinceName { get; set; }
        public string SenderDistrictName { get; set; }
        public string ReceiverProvinceName { get; set; }
        public string ReceiverDistrictName { get; set; }
        public string PostOfficeName { get; set; }
        //tai lieu
        public string TL { get; set; }
        //hang hoa
        public string HH { get; set; }
        //mau hang
        public string MH { get; set; }
        //nhanh
        public string N { get; set; }
        //duong bo
        public string DB { get; set; }
        //tiet kiem
        public string TK { get; set; }

        public string COD { get; set; }

        public string Weight { get; set; }

        public string Quantity { get; set; }

        public string Price { get; set; }

        public string ServicePrice { get; set; }

        public string Amount { get; set; }
    }
}