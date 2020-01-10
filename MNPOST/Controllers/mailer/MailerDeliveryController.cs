using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.mailer
{
    public class MailerDeliveryController : MailerController
    {
        // GET: MailerDelivery
        [HttpGet]
        public ActionResult Show()
        {
            ViewBag.PostOffices = EmployeeInfo.postOffices;
            List<AddressCommom> allProvince = GetProvinceDatas("", "province");
            ViewBag.Provinces = allProvince;
            ViewBag.ReturnReasons = db.BS_ReturnReasons.Select(p => new { name = p.ReasonName, code = p.ReasonID }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult GetReportEmployeeDelivery(string postId, List<string> employees)
        {
            var allDatas = db.MAILER_DELIVERY__EMPLOYEE(postId).ToList();

            if (employees == null)
                employees = new List<string>();

            if (employees.Count() > 0)
            {
                allDatas = allDatas.Where(p => employees.Contains(p.EmployeeID)).ToList();
            }

            return Json(new ResultInfo()
            {
                error = 0,
                data = allDatas
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetDataHandle(string postId)
        {
            var data = GetEmployeeByPost(postId);

            var licensePlates = new List<CommonData>();

            return Json(new { employees = data, licensePlates = licensePlates }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddMailer(string mailerId, string documentId)
        {
            var mailer = db.MM_Mailers.Find(mailerId);
            if (mailer == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);
            }

            if (mailer.CurrentStatusID != 2 && mailer.CurrentStatusID != 6 && mailer.CurrentStatusID != 5)
            {

                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Mã hàng không thể phân phát"
                }, JsonRequestBehavior.AllowGet);
            }



            if (mailer.CurrentStatusID == 6 || mailer.CurrentStatusID == 5)
            {
                if (mailer.IsPostAccept == false)
                {
                    return Json(new ResultInfo()
                    {
                        error = 1,
                        msg = "Mã hàng không thể phân phát vì bưu cục chưa xác nhận nhận hàng"
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            var delivery = db.MM_MailerDelivery.Find(documentId);

            if (delivery == null || (delivery.DocumentDate.Value.ToString("ddMMyyyy") != DateTime.Now.ToString("ddMMyyyy")))
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Không thể phân phát"
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // dang phat
                var check = db.MM_MailerDeliveryDetail.Where(p => p.MailerID == mailerId && p.DocumentID == documentId && p.DeliveryStatus == 3).FirstOrDefault();

                if (check != null)
                    throw new Exception("Không thể phân phát");

                var insData = new MM_MailerDeliveryDetail()
                {
                    Id = Guid.NewGuid().ToString(),
                    DocumentID = documentId,
                    MailerID = mailerId,
                    CreationDate = DateTime.Now,
                    DeliveryStatus = 3
                };

                db.MM_MailerDeliveryDetail.Add(insData);

                db.SaveChanges();

                mailer.CurrentStatusID = 3;
                mailer.IsPostAccept = false;
                mailer.LastUpdateDate = DateTime.Now;
                db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // luu tracking
                var employee = db.BS_Employees.Where(p => p.EmployeeID == delivery.EmployeeID).FirstOrDefault();
                HandleHistory.AddTracking(3, mailerId, mailer.CurrentPostOfficeID, "Nhân viên " + employee.EmployeeName + "(" + employee.Phone + ") , đang đi phát hàng");

                var data = db.MAILERDELIVERY_GETMAILER_BY_ID(insData.Id).FirstOrDefault();
                MailerHandle.SendNotifi("Phát hàng", "Có 1 đơn phát hàng mới: " + mailer.MailerID, employee.UserLogin);
                return Json(new ResultInfo()
                {
                    error = 0,
                    msg = "",
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Lỗi cập nhật"
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult AddListMailer(List<string> mailers, string documentId)
        {
            var delivery = db.MM_MailerDelivery.Find(documentId);
            var employee = db.BS_Employees.Where(p => p.EmployeeID == delivery.EmployeeID).FirstOrDefault();
            if (delivery == null || (delivery.DocumentDate.Value.ToString("ddMMyyyy") != DateTime.Now.ToString("ddMMyyyy")))
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Không thể phân phát"
                }, JsonRequestBehavior.AllowGet);
            }
            foreach (var item in mailers)
            {
                var mailer = db.MM_Mailers.Find(item);
                if (mailer == null)
                    continue;

                if (mailer.CurrentStatusID != 2 && mailer.CurrentStatusID != 6 && mailer.CurrentStatusID != 5)
                    continue;

                if (mailer.CurrentStatusID == 6 || mailer.CurrentStatusID == 5)
                {
                    if (mailer.IsPostAccept == false)
                    {
                        return Json(new ResultInfo()
                        {
                            error = 1,
                            msg = "Mã hàng không thể phân phát vì bưu cục chưa xác nhận nhận hàng"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                // dang phat
                var check = db.MM_MailerDeliveryDetail.Where(p => p.MailerID == item && p.DocumentID == documentId && p.DeliveryStatus == 3).FirstOrDefault();

                if (check != null)
                    throw new Exception("Không thể phân phát");


                var insData = new MM_MailerDeliveryDetail()
                {
                    Id = Guid.NewGuid().ToString(),
                    DocumentID = documentId,
                    MailerID = item,
                    CreationDate = DateTime.Now,
                    DeliveryStatus = 3,
                };

                db.MM_MailerDeliveryDetail.Add(insData);

                db.SaveChanges();

                mailer.CurrentStatusID = 3;
                mailer.LastUpdateDate = DateTime.Now;
                mailer.IsPostAccept = false;
                db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();


                HandleHistory.AddTracking(3, item, mailer.CurrentPostOfficeID, "Nhân viên " + employee.EmployeeName + "(" + employee.Phone + ") , đang đi phát hàng");

            }
            MailerHandle.SendNotifi("Phát hàng", "Có đơn phát hàng mới: " + mailers.ToString(), employee.UserLogin);
            return Json(new ResultInfo()
            {
                error = 0,
                msg = ""
            }, JsonRequestBehavior.AllowGet);
        }

        // huy mailer moi phan phat 
        // mailer trang thai la 3 - dang phat
        [HttpPost]
        public ActionResult DetroyMailerDelivery(string id)
        {
            var data = db.MM_MailerDeliveryDetail.Where(p=> p.Id == id).FirstOrDefault();

            if (data == null)
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);

            var mailer = db.MM_Mailers.Find(data.MailerID);

            if (mailer == null)
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                }, JsonRequestBehavior.AllowGet);


            if (mailer.CurrentStatusID != 3)
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Đơn đã được phát"
                }, JsonRequestBehavior.AllowGet);

            // remove in tracking
            var lastTracking = db.MM_Tracking.Where(p => p.MailerID == mailer.MailerID).OrderByDescending(p => p.CreateTime).FirstOrDefault();

            if (lastTracking != null)
            {
                db.MM_Tracking.Remove(lastTracking);
                db.SaveChanges();
            }
            lastTracking = db.MM_Tracking.Where(p => p.MailerID == mailer.MailerID).OrderByDescending(p => p.CreateTime).FirstOrDefault();

            if (lastTracking != null)
            {
                //
                db.MM_MailerDeliveryDetail.Remove(data);

                mailer.CurrentStatusID = lastTracking.StatusID;
                mailer.LastUpdateDate = DateTime.Now;
                mailer.IsPostAccept = true;
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
        public ActionResult GetDeliveryMailerDetail(string employeeId, string deliveryDate, string postId)
        {

            if (!CheckPostOffice(postId))
                return Json(new ResultInfo() { error = 0, msg = "Sai bưu cục" }, JsonRequestBehavior.AllowGet);
            DateTime date = DateTime.Now;
            try
            {
                date = DateTime.ParseExact(deliveryDate, "dd/M/yyyy", null);
            }
            catch
            {
                date = DateTime.Now;
            }
            var checkEmployyee = db.BS_Employees.Find(employeeId);
            if (checkEmployyee == null)
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai nhân viên"
                }, JsonRequestBehavior.AllowGet);

            var documentCod = postId + date.ToString("ddMMyyyy");
            //
            var findDocument = db.MM_MailerDelivery.Where(p => p.DocumentCode == documentCod && p.EmployeeID == checkEmployyee.EmployeeID).FirstOrDefault();

            if (findDocument == null)
            {
                if (DateTime.Now.ToString("dd/MM/yyyy") == deliveryDate)
                {
                    findDocument = new MM_MailerDelivery()
                    {
                        DocumentID = Guid.NewGuid().ToString(),
                        DocumentCode = documentCod,
                        CreateDate = date,
                        DocumentDate = date,
                        EmployeeID = employeeId,
                        LastEditDate = DateTime.Now,
                        Notes = "",
                        NumberPlate = "",
                        Quantity = 0,
                        StatusID = 0,
                        Weight = 0,
                        PostID = postId
                    };

                    db.MM_MailerDelivery.Add(findDocument);

                    db.SaveChanges();
                }
                else
                {
                    return Json(new ResultInfo()
                    {
                        error = 1,
                        msg = "Không có bảng kê nào cho ngày " + deliveryDate
                    }, JsonRequestBehavior.AllowGet);
                }

            }

            var document = db.MAILER_GET_DELIVERY_EMPLOYEE_BYDATE(date.ToString("yyyy-MM-dd"), documentCod, checkEmployyee.EmployeeID).FirstOrDefault();

            var data = db.MAILERDELIVERY_GETMAILER(document.DocumentID).ToList();

            return Json(new ResultInfo()
            {
                data = new
                {

                    document = document,
                    details = data
                },
                error = 0
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateDelivery(string documentId, string notes, string numberPlate)
        {
            var find = db.MM_MailerDelivery.Find(documentId);

            if(find != null)
            {
                find.LastEditDate = DateTime.Now;
                find.Notes = notes;
                find.NumberPlate = numberPlate;
                db.Entry(find).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }


        // UPDATE DELIVER 
        [HttpGet]
        public ActionResult GetDeliveryMailerDetailNotUpdate(string documentID)
        {
            var data = db.MAILERDELIVERY_GETMAILER(documentID).Where(p => p.DeliveryStatus == 3).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetMailerForReUpdate(string mailerID)
        {

            var findLastDelivery = db.MM_MailerDeliveryDetail.Where(p => p.MailerID == mailerID && p.DeliveryStatus == 3).FirstOrDefault();

            if (findLastDelivery == null)
            {
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Mã không thể cập nhật"
                }, JsonRequestBehavior.AllowGet);
            }

            // kiem tra đã chốt công nợ chưa


            // lấy thông tin

            var data = db.MAILERDELIVERY_GETMAILER_BY_ID(findLastDelivery.Id).FirstOrDefault();

            return Json(new ResultInfo()
            {
                error = 0,
                msg = "",
                data = data
            }, JsonRequestBehavior.AllowGet);

        }

        // lay tuyen tu dong
        [HttpGet]
        public ActionResult AutoGetRouteEmployees(string postId)
        {
            var employees = db.BS_Employees.Where(p => p.PostOfficeID == postId).ToList();
            List<EmployeeAutoRouteInfo> routeAutoes = new List<EmployeeAutoRouteInfo>();

            var countMailers = db.MM_Mailers.Where(p => p.CurrentPostOfficeID == postId && (p.CurrentStatusID == 2 || p.CurrentStatusID == 6 || p.CurrentStatusID == 5)).ToList();

            foreach (var item in employees)
            {
                var listMailer = db.ROUTE_GETMAILER_BYEMPLOYEEID(item.EmployeeID, postId).ToList();
                var data = new EmployeeAutoRouteInfo()
                {
                    EmployeeID = item.EmployeeID,
                    EmployeeName = item.EmployeeName,
                    Mailers = new List<MailerIdentity>()
                };

                foreach (var mailer in listMailer)
                {

                    if (mailer.IsDetail == true)
                    {
                        // check phuong
                        var wardCheck = db.BS_RouteDetails.Where(p => p.RouteID == mailer.RouteID && p.WardID == mailer.RecieverWardID).FirstOrDefault();

                        if (wardCheck != null)
                        {
                            data.Mailers.Add(new MailerIdentity()
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

                    }
                    else
                    {
                        data.Mailers.Add(new MailerIdentity()
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
                }

                routeAutoes.Add(data);
            }

            return Json(new { routes = routeAutoes, coutMailer = countMailers.Count() }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetAutoMailerFromEmployeeRoute(string postId, string employeeId)
        {
            var employee = db.BS_Employees.Find(employeeId);

            var countMailers = db.MM_Mailers.Where(p => p.CurrentPostOfficeID == postId && (p.CurrentStatusID == 2 || p.CurrentStatusID == 6 || p.CurrentStatusID == 5)).ToList();

            var listMailer = db.ROUTE_GETMAILER_BYEMPLOYEEID(employee.EmployeeID, postId).ToList();
            var data = new EmployeeAutoRouteInfo()
            {
                EmployeeID = employee.EmployeeID,
                EmployeeName = employee.EmployeeName,
                Mailers = new List<MailerIdentity>()
            };

            foreach (var mailer in listMailer)
            {
                if (mailer.IsDetail == true)
                {
                    // check phuong
                    var wardCheck = db.BS_RouteDetails.Where(p => p.RouteID == mailer.RouteID && p.WardID == mailer.RecieverWardID).FirstOrDefault();

                    if (wardCheck != null)
                    {
                        data.Mailers.Add(new MailerIdentity()
                        {
                            MailerID = mailer.MailerID,
                            COD = mailer.COD,
                            SenderName = mailer.SenderName,
                            SenderAddress = mailer.SenderAddress,
                            RecieverAddress = mailer.RecieverAddress,
                            RecieverProvinceID = mailer.RecieProvinceName,
                            RecieverDistrictID = mailer.ReceiDistrictName,
                            RecieverWardID = mailer.ReceiWardName,
                            RecieverPhone = mailer.RecieverPhone,
                            CurrentStatusID = mailer.CurrentStatusID,
                            MailerTypeID = mailer.MailerTypeID
                        });
                    }

                }
                else
                {
                    data.Mailers.Add(new MailerIdentity()
                    {
                        MailerID = mailer.MailerID,
                        COD = mailer.COD,
                        SenderName = mailer.SenderName,
                        SenderAddress = mailer.SenderAddress,
                        RecieverAddress = mailer.RecieverAddress,
                        RecieverProvinceID = mailer.RecieProvinceName,
                        RecieverDistrictID = mailer.ReceiDistrictName,
                        RecieverWardID = mailer.ReceiWardName,
                        RecieverPhone = mailer.RecieverPhone,
                        CurrentStatusID = mailer.CurrentStatusID,
                        MailerTypeID = mailer.MailerTypeID
                    });
                }
            }

            return Json(new { routes = data, countMailer = countMailers.Count() }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetMailerForEmployee(string postId, string province, string district)
        {
            var mailers = db.MM_Mailers.Where(p => p.CurrentPostOfficeID == postId && (p.CurrentStatusID == 2 || p.CurrentStatusID == 6 || p.CurrentStatusID == 5) && (p.RecieverProvinceID.Contains(province) || p.RecieverDistrictID.Contains(district))).ToList();
            var data = new List<MailerIdentity>();
            foreach (var item in mailers)
            {

                if (item.CurrentStatusID == 5 || item.CurrentStatusID == 6)
                {
                    if(item.IsPostAccept == false)
                        continue;
                }

                var mailer = db.MAILER_GETINFO_BYID(item.MailerID).FirstOrDefault();


                data.Add(new MailerIdentity()
                {
                    MailerID = mailer.MailerID,
                    COD = mailer.COD,
                    SenderName = mailer.SenderName,
                    SenderAddress = mailer.SenderAddress,
                    RecieverAddress = mailer.RecieverAddress,
                    RecieverProvinceID = mailer.RecieProvinceName,
                    RecieverDistrictID = mailer.ReceiDistrictName,
                    RecieverWardID = mailer.ReceiWardName,
                    RecieverPhone = mailer.RecieverPhone,
                    CurrentStatusID = mailer.CurrentStatusID,
                    MailerTypeID = mailer.MailerTypeID
                });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ShowMailerNotFinishOfDate(string employeeId)
        {
            var data = db.DELIVERY_GETMAILER_OFDATE_NOTFINISH(employeeId).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateFromRoutes(List<EmployeeAutoRouteInfo> routes, string postId)
        {
            DateTime date = DateTime.Now;

            foreach (var item in routes)
            {
                if (item.Mailers.Count() == 0)
                    continue;

                var documentCod = postId + date.ToString("ddMMyyyy");
                //
                var findDocument = db.MM_MailerDelivery.Where(p => p.DocumentCode == documentCod && p.EmployeeID == item.EmployeeID).FirstOrDefault();
                var findEmployee = db.BS_Employees.Find(item.EmployeeID);
                if (findDocument == null)
                {
                    findDocument = new MM_MailerDelivery()
                    {
                        DocumentID = Guid.NewGuid().ToString(),
                        DocumentCode = documentCod,
                        CreateDate = date,
                        DocumentDate = date,
                        EmployeeID = item.EmployeeID,
                        LastEditDate = DateTime.Now,
                        Notes = "",
                        NumberPlate = "",
                        Quantity = 0,
                        StatusID = 0,
                        Weight = 0,
                        PostID = postId
                    };

                    db.MM_MailerDelivery.Add(findDocument);

                    db.SaveChanges();
                }


                // add to detail
                foreach (var mailer in item.Mailers)
                {
                    var checkMailer = db.MM_Mailers.Where(p => p.MailerID == mailer.MailerID && (p.CurrentStatusID == 2 || p.CurrentStatusID == 6 || p.CurrentStatusID == 5)).FirstOrDefault();

                    if (checkMailer != null)
                    {

                        if (checkMailer.CurrentStatusID == 6 || checkMailer.CurrentStatusID == 5)
                        {
                            if (checkMailer.IsPostAccept == false)
                            {
                                continue;
                            }
                        }

                        // dang phat
                        var check = db.MM_MailerDeliveryDetail.Where(p => p.MailerID == mailer.MailerID && p.DocumentID == findDocument.DocumentID && p.DeliveryStatus == 3).FirstOrDefault();

                        if (check != null)
                            continue;

                        var insData = new MM_MailerDeliveryDetail()
                        {
                            Id = Guid.NewGuid().ToString(),
                            DocumentID = findDocument.DocumentID,
                            MailerID = checkMailer.MailerID,
                            CreationDate = DateTime.Now,
                            DeliveryStatus = 3,
                        };

                        db.MM_MailerDeliveryDetail.Add(insData);

                        checkMailer.CurrentStatusID = 3;
                        checkMailer.LastUpdateDate = DateTime.Now;
                        checkMailer.IsPostAccept = false;
                        db.Entry(checkMailer).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();

                        HandleHistory.AddTracking(3, checkMailer.MailerID, checkMailer.CurrentPostOfficeID, "Nhân viên " + findEmployee.EmployeeName + "(" + findEmployee.Phone + ") , đang đi phát hàng");
                    }
                }

                MailerHandle.SendNotifi("Phát hàng", "Có đơn phát mới phát", findEmployee.UserLogin);

            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        // update phat
        [HttpPost]
        public ActionResult ConfirmDeliveyMailer(MailerDeliveryConfirmInfo detail)
        {

            var findDetail = db.MM_MailerDeliveryDetail.Where(p=> p.Id == detail.DetailId).FirstOrDefault();

            if (findDetail != null)
            {
                var mailerInfo = db.MM_Mailers.Find(findDetail.MailerID);
                
                var findDocument = db.MM_MailerDelivery.Where(p => p.DocumentID == detail.DocumentID).FirstOrDefault();

                if (findDocument == null)
                {
                    return Json(new ResultInfo()
                    {
                        error = 1,
                        msg = "Sai thông tin"
                    }, JsonRequestBehavior.AllowGet);
                }

                findDetail.DeliveryStatus = detail.DeliveryStatus;
                mailerInfo.CurrentStatusID = detail.DeliveryStatus;

                var deliveryDate = DateTime.ParseExact(detail.DeliveryDate + " " + detail.DeliveryTime, "dd/M/yyyy HH:mm", null);

                if (deliveryDate == null)
                    deliveryDate = DateTime.Now;

                if (detail.DeliveryStatus == 5)
                {
       
                    findDetail.DeliveryTo = "";
                    findDetail.DeliveryNotes = detail.DeliveryNotes;
                    findDetail.ReturnReasonID = detail.ReturnReasonID;
                    findDetail.DeliveryDate = deliveryDate;


                    mailerInfo.DeliveryTo = "";
                    mailerInfo.DeliveryDate = deliveryDate;
                    mailerInfo.DeliveryNotes = detail.DeliveryNotes;
                    mailerInfo.IsReturn = true;

                    HandleHistory.AddTracking(5, detail.MailerID, mailerInfo.CurrentPostOfficeID, "Trả lại hàng, vì lý do " + detail.DeliveryNotes);
                }
                else if (detail.DeliveryStatus == 6)
                {
                    findDetail.DeliveryTo = "";
                    findDetail.DeliveryNotes = detail.DeliveryNotes;
                    findDetail.DeliveryDate = deliveryDate;

                    mailerInfo.DeliveryTo = "";
                    mailerInfo.DeliveryDate = deliveryDate;
                    mailerInfo.DeliveryNotes = detail.DeliveryNotes;

                    HandleHistory.AddTracking(6, detail.MailerID, mailerInfo.CurrentPostOfficeID, "Chưa phát được vì " + detail.DeliveryNotes);
                }
                else if (detail.DeliveryStatus == 4)
                {
                    findDetail.DeliveryTo = detail.DeliveryTo;
                    findDetail.ReturnReasonID = null;
                    findDetail.DeliveryNotes = "Đã phát";
                    findDetail.DeliveryDate = deliveryDate;

                    mailerInfo.DeliveryTo = detail.DeliveryTo;
                    mailerInfo.DeliveryDate = deliveryDate;
                    mailerInfo.DeliveryNotes = "Đã phát";
                    mailerInfo.CurrentStatusID = 4;
                    if (mailerInfo.IsReturn == true)
                    {
                        findDetail.DeliveryNotes = "Đã hoàn - Người gửi thanh toán cước";
                        mailerInfo.DeliveryNotes = "Đã hoàn - Người gửi thanh toán cước";

                        findDetail.DeliveryStatus = 11;
                        mailerInfo.CurrentStatusID = 11;
                        mailerInfo.PaymentMethodID = "NGTT";

                        HandleHistory.AddTracking(11, detail.MailerID, mailerInfo.CurrentPostOfficeID, "Ngày hoàn " + deliveryDate.ToString("dd/MM/yyyy") + " lúc " + deliveryDate.ToString("HH:mm") + ", người nhận: " + detail.DeliveryTo);

                    }
                    else
                    {
                        HandleHistory.AddTracking(4, detail.MailerID, mailerInfo.CurrentPostOfficeID, "Ngày phát " + deliveryDate.ToString("dd/MM/yyyy") + " lúc " + deliveryDate.ToString("HH:mm") + ", người nhận: " + detail.DeliveryTo);
                        // save nhung don co thu tien COD
                       if(mailerInfo.PaymentMethodID == "NNTT")
                        {
                            var saveCoDDebit = new EmpployeeDebitCOD()
                            {
                                Id = Guid.NewGuid().ToString(),
                                AccountantConfirm = 0,
                                COD = Convert.ToDouble(mailerInfo.COD) + Convert.ToDouble(mailerInfo.Amount),
                                Describe = "Thu: Cước + COD",
                                ConfirmDate = DateTime.Now,
                                CreateDate = DateTime.Now,
                                DocumentID = detail.DocumentID,
                                EmployeeID = findDocument.EmployeeID,
                                MailerID = mailerInfo.MailerID
                            };

                            db.EmpployeeDebitCODs.Add(saveCoDDebit);
                        }
                        else
                        {
                            if (mailerInfo.COD > 0)
                            {
                                var saveCoDDebit = new EmpployeeDebitCOD()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    AccountantConfirm = 0,
                                    Describe = "Thu: COD",
                                    COD = Convert.ToDouble(mailerInfo.COD),
                                    ConfirmDate = DateTime.Now,
                                    CreateDate = DateTime.Now,
                                    DocumentID = detail.DocumentID,
                                    EmployeeID = findDocument.EmployeeID,
                                    MailerID = mailerInfo.MailerID
                                };

                                db.EmpployeeDebitCODs.Add(saveCoDDebit);
                            }
                        }
                    }

                   
                }
                mailerInfo.IsPostAccept = false;
                db.Entry(mailerInfo).State = System.Data.Entity.EntityState.Modified;
                db.Entry(findDetail).State = System.Data.Entity.EntityState.Modified;
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
                msg = "Sai thông tin"
            }, JsonRequestBehavior.AllowGet);
        }



      

    }
}