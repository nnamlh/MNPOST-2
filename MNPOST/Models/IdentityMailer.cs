using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOST.Models
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
        public int? Quantity { get; set; }
        public string PaymentMethodID { get; set; }
        public string MailerTypeID { get; set; }
        public decimal? PriceService { get; set; }
        public string MerchandiseID { get; set; }
        public List<ItemPriceCommon> Services { get; set; }
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

    public class MailerPartnerDetailUpdate
    {
        public string MailerID { get; set; }
        public string OrderReference { get; set; }

        public float OrderCosst { get; set; }
    }

    public class MailerDeliveryIdentity
    {
        public string DocumentID { get; set; }

        public string DocumentCode { get; set; }

        public string DocumentDate { get; set; }

        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public string Notes { get; set; }

        public int? Quantity { get; set; }

        public double? Weight { get; set; }

        public int? StatusID { get; set; }

        public string RouteID { get; set; }

        public string NumberPlate { get; set; }

        public string PostOfficeId { get; set; }
    }

    public class MailerDeliveryConfirmInfo
    {
        public string MailerID { get; set; }

        public string DeliveryTo { get; set; }

        public string DocumentID { get; set; }

        public int? ReturnReasonID { get; set; }

        public int? DeliveryStatus { get; set; }

        public string DeliveryNotes { get; set; }

        public string DeliveryDate { get; set; }

        public string DeliveryTime { get; set; }

        public string DetailId { get; set; }

    }


   // chia tuyen nhan vien
   public class EmployeeAutoRouteInfo
    {
        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public List<MailerIdentity> Mailers { get; set; }
    }
}
 
 