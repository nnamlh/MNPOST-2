using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
using MNPOST.Utils;

namespace MNPOST.Controllers.mailer
{
    public class MailerPartnerController : MailerController
    {
        public ActionResult GetTrackViettel(string track = "")
        {

            if(!String.IsNullOrEmpty(track))
            {

            }

            return View();
        }

        // GET: MailerPartner
        public ActionResult Show()
        {
            ViewBag.PostOffices = EmployeeInfo.postOffices;
            ViewBag.ToDate = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.FromDate = DateTime.Now.ToString("dd/MM/yyyy");
            var partner = db.BS_Partners.Select(p => new CommonData()
            {
                code = p.PartnerID,
                name = p.ParterName,

            }).ToList();

            ViewBag.Partners = partner;
            // tinh thanh
            ViewBag.Provinces = GetProvinceDatas("", "province");

            return View();
        }

        [HttpPost]
        public ActionResult GetMailerPartner(int? page, string fromDate, string toDate, string postId, string mailerId)
        {
            int pageSize = 50;

            int pageNumber = (page ?? 1);

            if (!CheckPostOffice(postId))
                return Json(new { error = 1, msg = "Không phải bưu cục" }, JsonRequestBehavior.AllowGet);

            if (String.IsNullOrEmpty(fromDate) || String.IsNullOrEmpty(toDate))
            {
                fromDate = DateTime.Now.ToString("dd/MM/yyyy");
                toDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

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

            if(String.IsNullOrEmpty(mailerId))
            {
                var data = db.MAILER_PARTNER_GETALL(paserFromDate.ToString("yyyy-MM-dd"), paserToDate.ToString("yyyy-MM-dd"), postId).ToList();

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
            } else
            {
                var data = db.MAILER_PARTNER_GETALL_ByMailerID(paserFromDate.ToString("yyyy-MM-dd"), paserToDate.ToString("yyyy-MM-dd"), postId, mailerId).ToList();

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

      
        }

        [HttpPost]
        public ActionResult CreateMailerPartner(string partnerId, string notes, string postId)
        {
            var insterInfo = new MM_MailerPartner()
            {
                CreateTime = DateTime.Now,
                DocumentCode = DateTime.Now.ToString("ddMMyyyyHHmmss"),
                DocumentID = Guid.NewGuid().ToString(),
                Notes = notes,
                PartnerID = partnerId,
                PostOfficeID = postId,
                StatusID = 0
            };

            db.MM_MailerPartner.Add(insterInfo);
            db.SaveChanges();

            return Json(new ResultInfo()
            {
                error = 0,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMailerByProvince(string postId, string province, string district)
        {
            var mailers = db.MM_Mailers.Where(p => p.CurrentPostOfficeID == postId && (p.CurrentStatusID == 2) && p.RecieverProvinceID.Contains(province) && p.RecieverDistrictID.Contains(district)).ToList();
            var data = new List<MailerIdentity>();
            foreach (var mailer in mailers)
            {
                data.Add(new MailerIdentity()
                {
                    MailerID = mailer.MailerID,
                    COD = mailer.COD,
                    SenderName = mailer.SenderName,
                    SenderAddress = mailer.SenderAddress,
                    RecieverAddress = mailer.RecieverAddress,
                    RecieverProvinceID = mailer.RecieverProvinceID,
                    RecieverDistrictID = mailer.RecieverDistrictID,
                    RecieverWardID = mailer.RecieverWardID,
                    RecieverPhone = mailer.RecieverPhone,
                    CurrentStatusID = mailer.CurrentStatusID,
                    MailerTypeID = mailer.MailerTypeID
                });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMailerPartnerDetail(string documentId)
        {
            var data = db.MAILER_PARTNER_GETDETAIL(documentId).ToList();

            return Json(new ResultInfo()
            {
                data = data,
                error = 0,
                msg = ""

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddMailer(string documentId, string mailerId)
        {
            var checkDocument = db.MM_MailerPartner.Find(documentId);

            if (checkDocument == null || checkDocument.StatusID == 1)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Không thể cập nhật"
                }, JsonRequestBehavior.AllowGet);
            }

            var mailer = db.MM_Mailers.Find(mailerId);

            if (mailer == null || mailer.CurrentStatusID != 2)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai mã hoặc chưa nhập kho"
                }, JsonRequestBehavior.AllowGet);
            }

            var checkDetail = db.MAILER_PARTNER_GETDETAIL_BY_MAILERID(documentId, mailer.MailerID).FirstOrDefault();

            if (checkDetail != null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Đã tồn tại"
                }, JsonRequestBehavior.AllowGet);
            }

            var detail = new MM_MailerPartnerDetail()
            {
                DocumentID = documentId,
                MailerID = mailer.MailerID,
                OrderCosst = 0,
                OrderReference = "",
                StatusID = 0
            };

            db.MM_MailerPartnerDetail.Add(detail);
            db.SaveChanges();

            return Json(new ResultInfo()
            {
                error = 0,
                msg = "",
                data = db.MAILER_PARTNER_GETDETAIL_BY_MAILERID(documentId, mailer.MailerID).FirstOrDefault()
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult AddMailers(string documentId, List<string> mailers)
        {
            var checkDocument = db.MM_MailerPartner.Find(documentId);

            if (checkDocument == null || checkDocument.StatusID == 1)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Không thể cập nhật"
                }, JsonRequestBehavior.AllowGet);
            }
            var partner = db.BS_Partners.Find(checkDocument.PartnerID);

            if (partner == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);
            }
            foreach (var item in mailers)
            {
                var mailer = db.MM_Mailers.Find(item);

                if (mailer == null || mailer.CurrentStatusID != 2)
                {
                    return Json(new ResultInfo()
                    {
                        error = 1,
                        msg = "Sai mã hoặc chưa nhập kho"
                    }, JsonRequestBehavior.AllowGet);
                }

                var checkDetail = db.MAILER_PARTNER_GETDETAIL_BY_MAILERID(documentId, mailer.MailerID).FirstOrDefault();

                if (checkDetail != null)
                {
                    return Json(new ResultInfo()
                    {
                        error = 1,
                        msg = "Đã tồn tại"
                    }, JsonRequestBehavior.AllowGet);
                }

                var detail = new MM_MailerPartnerDetail()
                {
                    DocumentID = documentId,
                    MailerID = mailer.MailerID,
                    OrderCosst = 0,
                    OrderReference = "",
                    StatusID = 0
                };

                db.MM_MailerPartnerDetail.Add(detail);
                db.SaveChanges();

                mailer.CurrentStatusID = 9;
                mailer.ThirdpartyCode = partner.PartnerCode;
                mailer.ThirdpartyID = partner.PartnerID;
                db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new ResultInfo()
            {
                error = 0,
                msg = ""
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UpdateDetails(List<MailerPartnerDetailUpdate> mailers, string documentId)
        {
            foreach (var item in mailers)
            {
                var find = db.MM_MailerPartnerDetail.Where(p => p.DocumentID == documentId && p.MailerID == item.MailerID).FirstOrDefault();

                if (find != null)
                {
                    find.OrderCosst = item.OrderCosst;
                    find.OrderReference = item.OrderReference;
                    db.Entry(find).State = System.Data.Entity.EntityState.Modified;
                }
            }

            db.SaveChanges();

            return Json(new ResultInfo()
            {
                error = 0,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteDetail(string documentId, string mailerId)
        {
            var find = db.MM_MailerPartnerDetail.Where(p => p.DocumentID == documentId && p.MailerID == mailerId).FirstOrDefault();
            var mailer = db.MM_Mailers.Find(mailerId);

            if (mailer == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Không thể xóa"
                }, JsonRequestBehavior.AllowGet);
            }


            mailer.CurrentStatusID = 2;
            mailer.ThirdpartyCode = "";
            mailer.ThirdpartyID = "";
            db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            if (find != null && find.StatusID == 0)
            {
                db.MM_MailerPartnerDetail.Remove(find);
                db.SaveChanges();

                return Json(new ResultInfo()
                {
                    error = 0,
                    msg = ""
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new ResultInfo()
            {
                error = 1,
                msg = "Không xóa được"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CancelSend(string documentId, string mailerId)
        {
            var checkDocument = db.MM_MailerPartner.Find(documentId);

            if (checkDocument == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);
            }

            var checkDetail = db.MM_MailerPartnerDetail.Where(p => p.DocumentID == documentId && p.MailerID == mailerId).FirstOrDefault();

            if (checkDetail == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Không thể hủy"
                }, JsonRequestBehavior.AllowGet);
            }

            var mailer = db.MM_Mailers.Find(mailerId);
            if (mailer == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Không thể hủy"
                }, JsonRequestBehavior.AllowGet);
            }


            var partner = db.BS_Partners.Find(checkDocument.PartnerID);

            if (partner == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);
            }

            //
            switch (partner.PartnerCode)
            {
                case "VIETTEL":
                    SendViettelHandle viettelHandle = new SendViettelHandle(db, partner, HandleHistory);
                    viettelHandle.CancelOrder(checkDetail.OrderReference);
                    break;
                default:
                    break;
            }

            checkDetail.StatusID = 2;// huy
            db.Entry(checkDetail).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            mailer.ThirdpartyDocID = "";
            mailer.ThirdpartyCode = "";
            mailer.ThirdpartyID = "";
            mailer.CurrentStatusID = 2;
            mailer.ThirdpartyCost = 0;

            db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new ResultInfo()
            {
                error = 0,
                msg = ""
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult SendPartner(string documentId, MyAddressInfo address, bool useAPI)
        {
            var checkDocument = db.MM_MailerPartner.Find(documentId);

            if (checkDocument == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);
            }

            var partner = db.BS_Partners.Find(checkDocument.PartnerID);

            if (partner == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);
            }

            //
            if (useAPI)
            {
                switch (partner.PartnerCode)
                {
                    case "VIETTEL":
                        SendViettelHandle viettelHandle = new SendViettelHandle(db, partner, HandleHistory);
                        viettelHandle.SendViettelPost(checkDocument.DocumentID, address);
                        break;
                    default:
                        var details = db.MAILER_PARTNER_GETDETAIL(documentId).ToList();
                        foreach (var item in details)
                        {
                            var checkDetail = db.MM_MailerPartnerDetail.Where(p => p.DocumentID == documentId && p.MailerID == item.MailerID).FirstOrDefault();

                            var mailer = db.MM_Mailers.Find(item.MailerID);

                            checkDetail.StatusID = 1;

                            db.Entry(checkDetail).State = System.Data.Entity.EntityState.Modified;

                            mailer.ThirdpartyDocID = checkDetail.OrderReference;
                            mailer.ThirdpartyCode = partner.PartnerCode;
                            mailer.ThirdpartyID = partner.PartnerID;
                            mailer.CurrentStatusID = 9;
                            mailer.ThirdpartyCost = Convert.ToDecimal(checkDetail.OrderCosst);

                            db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        break;
                }
            }
            else
            {
                var details = db.MAILER_PARTNER_GETDETAIL(documentId).ToList();
                foreach (var item in details)
                {
                    var checkDetail = db.MM_MailerPartnerDetail.Where(p => p.DocumentID == documentId && p.MailerID == item.MailerID).FirstOrDefault();

                    var mailer = db.MM_Mailers.Find(item.MailerID);

                    checkDetail.StatusID = 1;

                    db.Entry(checkDetail).State = System.Data.Entity.EntityState.Modified;

                    mailer.ThirdpartyDocID = checkDetail.OrderReference;
                    mailer.ThirdpartyCode = partner.PartnerCode;
                    mailer.ThirdpartyID = partner.PartnerID;
                    mailer.CurrentStatusID = 9;
                    mailer.ThirdpartyCost = Convert.ToDecimal(checkDetail.OrderCosst);

                    db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }


            return Json(new ResultInfo()
            {
                error = 0,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }
    }
}