using MNPOST.Models;
using MNPOSTCOMMON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Script.Serialization;

namespace MNPOST.Utils
{
    public class SendViettelHandle
    {
        MNPOSTEntities db;
        BS_Partners partner;
        MNHistory HandleHistory;
        public SendViettelHandle(MNPOSTEntities db, BS_Partners partner, MNHistory HandleHistory)
        {
            this.db = db;
            this.partner = partner;
            this.HandleHistory = HandleHistory;
        }

       


        public int sendCalPrice(CalPriceVietle info)
        {
            var token = CheckTokenExpired(partner);

            string url = @"https://api.viettelpost.vn/api/tmdt/getPrice";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Token"] = token;

            string json = new JavaScriptSerializer().Serialize(info);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var paser = new JavaScriptSerializer().Deserialize<List<CalPriceViettleResult>>(result);

                var price = paser.Where(p => p.SERVICE_CODE == "ALL").FirstOrDefault();

                if (price != null)
                {
                    try
                    {
                        return Convert.ToInt32(price.PRICE);
                    }
                    catch
                    {

                    }
                }

            }

            return 0;

        }
        public void CancelOrder(string orderNumber)
        {
            var token = CheckTokenExpired(partner);

            string url = @"https://api.viettelpost.vn/api/tmdt/UpdateOrder";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Token"] = token;

            try
            {
                var info = new UpdateOrderViettel()
                {
                    DATE = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    TYPE = 4,
                    ORDER_NUMBER = orderNumber,
                    NOTE = "HỦY ĐƠN HÀNG"
                };

                string json = new JavaScriptSerializer().Serialize(info);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var res = Json.Decode(result);

                    if (res.status != null)
                    {
                        
                    }

                }
            }
            catch
            {

            }


        }
       public void SendViettelPost(string documentId, MyAddressInfo infoAddress)
        {
            var token = CheckTokenExpired(partner);

            string url = @"https://api.viettelpost.vn/api/tmdt/InsertOrder";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Token"] = token;

            var details = db.MAILER_PARTNER_GETDETAIL(documentId).ToList();

            foreach (var item in details)
            {
                try
                {
                    ViettelOrderSend info = new ViettelOrderSend()
                    {
                        ORDER_NUMBER = item.MailerID,
                        GROUPADDRESS_ID = 0,
                        DELIVERY_DATE = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm:ss"),
                        CUS_ID = Convert.ToInt32(partner.UserID),
                        SENDER_FULLNAME = infoAddress.name,
                        SENDER_ADDRESS = infoAddress.address,
                        SENDER_EMAIL = infoAddress.email,
                        SENDER_PHONE = infoAddress.phone,
                        SENDER_WARD = Convert.ToInt32(infoAddress.ward),
                        SENDER_DISTRICT = Convert.ToInt32(infoAddress.district),
                        SENDER_PROVINCE = Convert.ToInt32(infoAddress.province),
                        SENDER_LATITUDE = 0,
                        SENDER_LONGITUDE = 0,
                        RECEIVER_FULLNAME = item.RecieverName,
                        RECEIVER_ADDRESS = item.RecieverAddress,
                        RECEIVER_PHONE = item.RecieverPhone,
                        RECEIVER_DISTRICT = Convert.ToInt32(item.RecieverDistrictID),
                        RECEIVER_PROVINCE = Convert.ToInt32(item.RecieverProvinceID),
                        RECEIVER_WARD = Convert.ToInt32(item.RecieverWardID),
                        PRODUCT_QUANTITY = Convert.ToInt32(item.Quantity),
                        RECEIVER_EMAIL = "",
                        RECEIVER_LATITUDE = 0,
                        RECEIVER_LONGITUDE = 0,
                        PRODUCT_NAME = "",
                        PRODUCT_HEIGHT = 0,
                        PRODUCT_LENGTH = 0,
                        PRODUCT_WEIGHT = 0,
                        PRODUCT_WIDTH = 0,
                        ORDER_NOTE = item.Notes,
                        PRODUCT_DESCRIPTION = item.MailerDescription,
                        MONEY_COLLECTION = Convert.ToString(Convert.ToInt32(item.COD)),
                        ORDER_SERVICE = "VCN",
                        PRODUCT_TYPE = "HH",
                        PRODUCT_PRICE = Convert.ToInt32(item.MerchandiseValue),
                        ORDER_PAYMENT = 3
                    };

                    var findServiceMap = db.PARTNER_MAP_INFO("VIETTEL", "SERVICE").ToList();
                    info.ORDER_SERVICE = findServiceMap.Where(p => p.MNPostData == item.MailerTypeID).Select(p => p.PartnerData).FirstOrDefault();

                    var priceSend = sendCalPrice(new CalPriceVietle()
                    {
                        SENDER_PROVINCE = info.SENDER_PROVINCE,
                        SENDER_DISTRICT = info.SENDER_DISTRICT,
                        RECEIVER_PROVINCE = info.RECEIVER_PROVINCE,
                        RECEIVER_DISTRICT = info.RECEIVER_DISTRICT,
                        PRODUCT_TYPE = "HH",
                        ORDER_SERVICE = info.ORDER_SERVICE,
                        PRODUCT_WEIGHT = info.PRODUCT_WEIGHT,
                        PRODUCT_PRICE = info.PRODUCT_PRICE,
                        MONEY_COLLECTION = info.MONEY_COLLECTION,
                    });

                    info.MONEY_TOTAL = priceSend;

                    string json = new JavaScriptSerializer().Serialize(info);

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    var httpResponse = (HttpWebResponse)request.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var res = Json.Decode(result);

                        if (res.status != null)
                        {
                            var checkDetail = db.MM_MailerPartnerDetail.Where(p => p.DocumentID == documentId && p.MailerID == item.MailerID).FirstOrDefault();

                            var mailer = db.MM_Mailers.Find(item.MailerID);

                            checkDetail.StatusID = 1;
                            checkDetail.OrderReference = res.data.ORDER_NUMBER;
                            checkDetail.OrderCosst = info.MONEY_TOTAL;

                            db.Entry(checkDetail).State = System.Data.Entity.EntityState.Modified;

                            mailer.ThirdpartyDocID = res.data.ORDER_NUMBER;
                            mailer.ThirdpartyCode = partner.PartnerCode;
                            mailer.ThirdpartyID = partner.PartnerID;
                            mailer.CurrentStatusID = 9;
                            mailer.ThirdpartyCost = info.MONEY_TOTAL;

                            db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            // tracking
                           // HandleHistory.AddTracking(3, mailer.MailerID, res.data.ORDER_NUMBER, mailer.CurrentPostOfficeID, "Chuyển qua đối tác " + partner.ParterName + ", vận đơn đối tác : " + res.data.ORDER_NUMBER);
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }


            var document = db.MM_MailerPartner.Find(documentId);
            document.StatusID = 1;
            db.Entry(document).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }


        public string CheckTokenExpired(BS_Partners partners)
        {
            if (partners.LastModife == null)
                return SendLogin(partners.UserLogin, partners.PassLogin, "42422");

            var days = DateTime.Now.Subtract(partners.LastModife.Value).TotalDays;

            if (days >= partners.Expired)
                return SendLogin(partners.UserLogin, partners.PassLogin, "42422");

            return partners.TokenLogin;
        }

        public string SendLogin(string user, string pass, string capcha)
        {
            string url = @"https://api.viettelpost.vn/api/user/Login";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            string json = new JavaScriptSerializer().Serialize(new
            {
                USERNAME = user,
                PASSWORD = pass,
                CAPTCHA = capcha
            });

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var res = Json.Decode(result);

                if (res.USERNAME != null)
                {
                    partner.LastModife = DateTime.Now;
                    partner.TokenLogin = res.TokenKey;
                    partner.Expired = 1;

                    db.Entry(partner).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return res.TokenKey;
                }
                else
                {
                    return "";
                }
            }
        }

    }



    public class CalPriceVietle
    {

        public int SENDER_PROVINCE { get; set; }
        public int SENDER_DISTRICT { get; set; }
        public int RECEIVER_PROVINCE { get; set; }
        public int RECEIVER_DISTRICT { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string ORDER_SERVICE { get; set; }
        public string ORDER_SERVICE_ADD { get; set; }
        public int PRODUCT_WEIGHT { get; set; }
        public int PRODUCT_PRICE { get; set; }
        public string MONEY_COLLECTION { get; set; }
        public int PRODUCT_QUANTITY { get; set; }
        public int NATIONAL_TYPE { get; set; }

    }

    public class UpdateOrderViettel
    {
        public int TYPE { get; set; }
        public string ORDER_NUMBER { get; set; }
        public string NOTE { get; set; }
        public string DATE { get; set; }
    }
    public class CalPriceViettleResult
    {
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string PRICE { get; set; }
    }
    public class ViettelOrderSend
    {
        public string ORDER_NUMBER { get; set; }
        public int GROUPADDRESS_ID { get; set; }
        public int CUS_ID { get; set; }
        public string DELIVERY_DATE { get; set; }
        public string SENDER_FULLNAME { get; set; }
        public string SENDER_ADDRESS { get; set; }
        public string SENDER_PHONE { get; set; }
        public string SENDER_EMAIL { get; set; }
        public int SENDER_WARD { get; set; }
        public int SENDER_DISTRICT { get; set; }
        public int SENDER_PROVINCE { get; set; }
        public int SENDER_LATITUDE { get; set; }
        public int SENDER_LONGITUDE { get; set; }
        public string RECEIVER_FULLNAME { get; set; }
        public string RECEIVER_ADDRESS { get; set; }
        public string RECEIVER_PHONE { get; set; }
        public string RECEIVER_EMAIL { get; set; }
        public int RECEIVER_WARD { get; set; }
        public int RECEIVER_DISTRICT { get; set; }
        public int RECEIVER_PROVINCE { get; set; }
        public int RECEIVER_LATITUDE { get; set; }
        public int RECEIVER_LONGITUDE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string PRODUCT_DESCRIPTION { get; set; }
        public int PRODUCT_QUANTITY { get; set; }
        public int PRODUCT_PRICE { get; set; }
        public int PRODUCT_WEIGHT { get; set; }
        public int PRODUCT_LENGTH { get; set; }
        public int PRODUCT_WIDTH { get; set; }
        public int PRODUCT_HEIGHT { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public int ORDER_PAYMENT { get; set; }
        public string ORDER_SERVICE { get; set; }
        public string ORDER_SERVICE_ADD { get; set; }
        public string ORDER_VOUCHER { get; set; }
        public string ORDER_NOTE { get; set; }
        public string MONEY_COLLECTION { get; set; }
        public int MONEY_TOTALFEE { get; set; }
        public int MONEY_FEECOD { get; set; }
        public int MONEY_FEEVAS { get; set; }
        public int MONEY_FEEINSURRANCE { get; set; }
        public int MONEY_FEE { get; set; }
        public int MONEY_FEEOTHER { get; set; }
        public int MONEY_TOTALVAT { get; set; }
        public int MONEY_TOTAL { get; set; }
        public List<ViettelOrderItemSend> LIST_ITEM { get; set; }
    }

    public class ViettelOrderItemSend
    {
        public string PRODUCT_NAME { get; set; }
        public int PRODUCT_PRICE { get; set; }
        public int PRODUCT_WEIGHT { get; set; }
        public int PRODUCT_QUANTITY { get; set; }
    }

}