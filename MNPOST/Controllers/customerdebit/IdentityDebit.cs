using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Controllers.customerdebit
{
    public class IdentityDebit
    {
       public string  MailerID {get;set;}
       public Nullable< decimal>  Amount{get;set;}
       public Nullable<decimal> Price { get; set; }
       public Nullable<decimal> PriceService { get; set; }
       public Nullable<double> Discount { get; set; }
       public Nullable<double> DiscountPercent { get; set; }
       public Nullable<double> VATPercent { get; set; }
       public Nullable<decimal> BfVATAmount { get; set; }
       public Nullable<decimal> VATAmount { get; set; }
       public Nullable<decimal> AfVATAmount { get; set; }
       public Nullable<DateTime> AcceptDate { get; set; }
       public int Quantity {get;set;}
       public double Weight {get;set;}
       public Nullable<decimal> COD { get; set; }
    }
}