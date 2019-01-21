using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Models
{
    public class IdentityDiscountPolicy
    {
        public string PostOfficeID { get; set; }
        public string DiscountID { get; set; }
        public string Description { get; set; }
        public int StatusID { get; set; }
        public string PermanentCal { get; set; }
        public string AllService { get; set; }
        public string AllCustomer { get; set; }
        public decimal LimitValue { get; set; }
        public int? DiscountPercent { get; set; }
        public int? AllMethod { get; set; }
    }
    public class IdentityComissionPolicy
    {
        public string PostOfficeID { get; set; }
        public string ComissionID { get; set; }
        public string Description { get; set; }
        public int StatusID { get; set; }
        public string PermanentCal { get; set; }
        public string AllService { get; set; }
        public string AllCustomer { get; set; }
        public decimal LimitValue { get; set; }
        public int? ComissionPercent { get; set; }
        public int? AllMethod { get; set; }
    }
    public class HTCoDinh
    {
        public decimal GiaTriDat { get; set; }
        public int? TiLe { get; set; }
    }
    public class HTDinhMuc
    {
        public decimal BatDau { get; set; }
        public decimal KetThuc { get; set; }
        public int? TiLe { get; set; }
    }
}