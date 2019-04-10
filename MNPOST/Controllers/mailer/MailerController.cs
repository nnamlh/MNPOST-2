using MNPOSTCOMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOST.Models;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace MNPOST.Controllers.mailer
{
    public class MailerController : BaseController
    {
        protected MNHistory HandleHistory = new MNHistory();


        [HttpGet]
        public ActionResult ShowMailer()
        {
            ViewBag.PostOffices = EmployeeInfo.postOffices;
            var customers = db.BS_Customers.Select(p => new
            {
                name = p.CustomerName,
                code = p.CustomerCode,
                address = p.Address,
                phone = p.Phone
            }).ToList();

            // dịch vu

            ViewBag.MailerTypes = db.BS_ServiceTypes.Select(p => new CommonData()
            {
                code = p.ServiceID,
                name = p.ServiceName
            }).ToList();

            // hinh thuc thanh toan
            ViewBag.Payments = db.CDatas.Where(p => p.CType == "MAILERPAY").Select(p => new CommonData() { code = p.Code, name = p.Name }).ToList();


            // danh sach phu phi
            ViewBag.Services = db.BS_Services.Select(p => new ItemPriceCommon()
            {
                code = p.ServiceID,
                name = p.ServiceName,
                price = p.Price,
                choose = false

            }).ToList(); ;

            // tinh thanh
            ViewBag.Provinces = GetProvinceDatas("", "province");

            ViewBag.AllCustomer = customers;

            return View();
        }

        [HttpGet]
        public ActionResult SearchMailer()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetMailers(int? page, string search, string fromDate, string toDate, int status, string postId, string customerId)
        {
            int pageSize = 50;

            int pageNumber = (page ?? 1);

            DateTime paserFromDate = DateTime.Now;
            DateTime paserToDate = DateTime.Now;

            try
            {
                paserFromDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                paserToDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
            }
            catch
            {
                paserFromDate = DateTime.Now;
                paserToDate = DateTime.Now;
            }

            var data = db.MAILER_GETALL(paserFromDate.ToString("yyyy-MM-dd"), paserToDate.ToString("yyyy-MM-dd"), "%" + postId + "%", "%" + search + "%").Where(p => p.SenderID.Contains(customerId)).ToList();

            if (status != -1)
            {
                data = data.Where(p => p.CurrentStatusID == status).ToList();
            }

            ResultInfo result = new ResultWithPaging()
            {
                error = 0,
                msg = "",
                page = pageNumber,
                pageSize = pageSize,
                toltalSize = data.Count(),
                data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            };


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelReturn(string mailerId)
        {
            try
            {
                if (String.IsNullOrEmpty(mailerId))
                    throw new Exception("Sai đơn");

                var findMailer = db.MM_Mailers.Where(p => p.MailerID == mailerId).FirstOrDefault();

                if(findMailer == null)
                    throw new Exception("Sai đơn");

                if (findMailer.CurrentStatusID == 4 || findMailer.CurrentStatusID == 11)
                    throw new Exception("Đơn đã hoàn rồi");

                findMailer.IsReturn = false;
                findMailer.LastUpdateDate = DateTime.Now;

                db.Entry(findMailer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new ResultInfo()
                {
                    error = 0,
                    msg = ""
                }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = e.Message
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public ActionResult GetMailerService(string mailerId)
        {
            var data = db.MM_MailerServices.Where(p => p.MailerID == mailerId).Select(p => new ItemPriceCommon()
            {
                choose = true,
                code = p.ServiceID,
                price = p.SellingPrice

            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateMailers(MailerIdentity mailer)
        {

            var checkExist = db.MM_Mailers.Where(p => p.MailerID == mailer.MailerID).FirstOrDefault();

            if (checkExist == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai mã"

                }, JsonRequestBehavior.AllowGet);
            }

            checkExist.SenderID = mailer.SenderID;
            checkExist.SenderName = mailer.SenderName;
            checkExist.SenderPhone = mailer.SenderPhone;
            checkExist.SenderAddress = mailer.SenderAddress;
            checkExist.SenderProvinceID = mailer.SenderProvinceID;
            checkExist.SenderDistrictID = mailer.SenderDistrictID;
            checkExist.SenderWardID = mailer.SenderWardID;
            checkExist.RecieverName = mailer.RecieverName;
            checkExist.RecieverPhone = mailer.RecieverPhone;
            checkExist.RecieverAddress = mailer.RecieverAddress;
            checkExist.RecieverProvinceID = mailer.RecieverProvinceID;
            checkExist.RecieverDistrictID = mailer.RecieverDistrictID;
            checkExist.RecieverWardID = mailer.RecieverWardID;

            checkExist.Weight = mailer.Weight;
            checkExist.LengthSize = mailer.LengthSize;
            checkExist.HeightSize = mailer.HeightSize;
            checkExist.WidthSize = mailer.WidthSize;

            checkExist.COD = mailer.COD;
            checkExist.Price = mailer.PriceDefault;
            checkExist.PriceDefault = mailer.PriceDefault;
            checkExist.PriceService = mailer.PriceService;
            checkExist.Quantity = mailer.Quantity;
            checkExist.Amount = mailer.Amount;

            checkExist.MailerTypeID = mailer.MailerTypeID;
            checkExist.MerchandiseID = mailer.MerchandiseID;
            checkExist.MerchandiseValue = mailer.MerchandiseValue;
            checkExist.PaymentMethodID = mailer.PaymentMethodID;

            checkExist.MailerDescription = mailer.MailerDescription;
            checkExist.Notes = mailer.Notes;
            checkExist.LastUpdateDate = DateTime.Now;

            db.Entry(checkExist).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            var allServices = db.MM_MailerServices.Where(p => p.MailerID == checkExist.MailerID).ToList();
            foreach (var item in allServices)
            {
                db.MM_MailerServices.Remove(item);
            }
            db.SaveChanges();

            // save service
            if (mailer.Services != null)
            {
                decimal? totalPriceService = 0;
                foreach (var service in mailer.Services)
                {
                    var checkService = db.BS_Services.Where(p => p.ServiceID == service.code && p.IsActive == true).FirstOrDefault();
                    if (checkService != null)
                    {
                        var servicePrice = service.price;
                      
                        if (service.percent == true)
                        {
                            servicePrice = (servicePrice * checkExist.Price) / 100;
                        }
                        totalPriceService = totalPriceService + servicePrice;
                        var mailerService = new MM_MailerServices()
                        {
                            MailerID = mailer.MailerID,
                            CreationDate = DateTime.Now,
                            SellingPrice = (decimal)servicePrice,
                            PriceDefault = (decimal)checkService.Price,
                            ServiceID = service.code
                        };
                        db.MM_MailerServices.Add(mailerService);
                    }
                }

                db.SaveChanges();

                checkExist.PriceService = totalPriceService;
                checkExist.Amount = checkExist.Price + checkExist.PriceCoD + checkExist.PriceService;

                db.Entry(checkExist).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult FindMailer(string mailerId)
        {
            var mailer = db.MAILER_GETINFO_BYID(mailerId).FirstOrDefault();

            if (mailer == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Không tìm thấy"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = db.MAILER_GETTRACKING(mailerId).ToList();
                var images = db.MailerImages.Where(p => p.MailerID == mailerId).Select(p => new
                {
                    url = p.PathImage
                }).ToList();
                return Json(new ResultInfo()
                {
                    error = 0,
                    data = new
                    {
                        mailer = mailer,
                        tracks = data,
                        images = images
                    }

                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetTracking(string mailerId)
        {
            var data = db.MAILER_GETTRACKING_BY_MAILERID(mailerId).ToList();

            return Json(new ResultInfo()
            {
                data = data,
                error = 0
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CalBillPrice(float weight = 0, string customerId = "", string provinceId = "", string serviceTypeId = "", string postId = "", float cod = 0, float merchandiseValue = 0, string districtId = "")
        {

            var findCus = db.BS_Customers.Where(p => p.CustomerCode == customerId).FirstOrDefault();
            decimal? price = 0;
            if (findCus != null)
            {
                if(cod > 0)
                {
                    var findDitrict = db.BS_Districts.Where(p => p.DistrictID == districtId).FirstOrDefault();
                    int? vsvx = findDitrict == null ? 1 : (findDitrict.VSVS == true ? 1 : 0);

                    price = db.CalPriceCOD(weight, customerId, provinceId, "CD", postId, DateTime.Now.ToString("yyyy-MM-dd"), vsvx, serviceTypeId == "ST" ? "CODTK" : "CODN").FirstOrDefault();
                } else
                {
                    price = db.CalPrice(weight, findCus.CustomerID, provinceId, serviceTypeId, postId, DateTime.Now.ToString("yyyy-MM-dd")).FirstOrDefault();
                }
            }

            return Json(new { price = price, codPrice = 0 }, JsonRequestBehavior.AllowGet);
        }

        protected bool CheckPostOffice(string postId)
        {
            var check = db.BS_PostOffices.Find(postId);

            return check == null ? false : true;
        }

        [HttpGet]
        public ActionResult GetAddressTemp(string phone)
        {
            var data = db.AddressTemps.Where(p => p.Phone.Contains(phone)).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}