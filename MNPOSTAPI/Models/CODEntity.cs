using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOSTAPI.Models
{
    public class CODEntity
    {
        public string PostOfficeID { get; set; }
        public string DocumentID { get; set; }
        public DateTime DocumentDate { get; set; }
        public string PaymentID { get; set; }
        public decimal Total { get; set; }
    }
    public class CODDetail
    {
        public string PostOfficeID { get; set; }
        public string DocumentID { get; set; }
        public DateTime DocumentDate { get; set; }
        public string PaymentID { get; set; }
        public string MailerID { get; set; }
        public string RecieverName { get; set; }
        public decimal COD { get; set; }
    }
    public class CODTotal
    {
        public decimal SapChuyen { get; set; }
        public decimal ChuaGiao { get; set; }
        public decimal DaGiao { get; set; }
        public decimal DaChuyen { get; set; }
    }

    public class CalPrice : ResultInfo
    {
        public decimal Price { get; set; }
    }

    public class CODInfoResult : ResultInfo
    {
        public List<CODEntity> codinfo { get; set; }
    }
    public class CODDetailInfoResult : ResultInfo
    {
        public List<CODDetail> coddetailinfo { get; set; }
    }
    public class CODTotalInfoResult : ResultInfo
    {
        public List<CODTotal> codtotalinfo { get; set; }
    }

    public class CalPriceResult : ResultInfo
    {
        public CalPrice calprice { get; set; }
    }
}