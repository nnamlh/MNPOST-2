using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MNPOSTCOMMON;
namespace MNPOSTAPI.Models
{
    public class MailerIdentity
    {

        /*
          'MailerID': '', 'SenderID': '', 'SenderName': '', 'SenderAddress': '', 'SenderWardID': '', 'SenderDistrictID': '',
                'SenderProvinceD:\work\Project\007MNPost\MNPOST\MNPOST\Models\IdentityMailer.csID': '', 'SenderPhone': '', 'RecieverName': ''
                , 'RecieverAddress': '', 'RecieverWardID': '', 'RecieverDistrictID': '', 'RecieverProvinceID': '',
                'RecieverPhone': '', 'Weight': 0.01, 'Quantity': 1, 'PaymentMethodID': 'NGTT', 'MailerTypeID': 'SN',
                'PriceService': 0, 'MerchandiseID': 'H', 'Services': [], 'MailerDescription': '', 'Notes': '', 'COD': 0, 'LengthSize': 0, 'WidthSize': 0, 'HeightSize': 0, 'PriceMain': 0, 'CODPrice': 0,
                'PriceDefault': 0
         */

        public string MailerID { get; set; }
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
        public double? Weight { get; set; }
        public int Quantity { get; set; }
        public string PaymentMethodID { get; set; }
        public string MailerTypeID { get; set; }
        public decimal? PriceService { get; set; }
        public string MerchandiseID { get; set; }
        public string MailerDescription { get; set; }
        public string Notes { get; set; }
        public decimal? COD { get; set; }
        public double? LengthSize { get; set; }
        public double? WidthSize { get; set; }
        public double? HeightSize { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PriceCoD { get; set; }
        public decimal? PriceDefault { get; set; }

        public decimal? MerchandiseValue { get; set; }

        public int? CurrentStatusID { get; set; }

    }

    public class MailerShowRequest
    {
        public int? page { get; set; }
        public string search { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int status { get; set; }
        public string customerId { get; set; }
    }
    public class CoDShowRequest
    {
        public int? page { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string customerId { get; set; }
    }

    public class  CancelMailerRequest
    {
        public string mailerId { get; set; }
        public string reason { get; set; }
    }
}