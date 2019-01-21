using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MNPOSTAPI.Models;
using MNPOSTCOMMON;

namespace MNPOSTAPI.Controllers.mobile.mailer
{
    public class MailerPresenter
    {

        MNPOSTEntities db = new MNPOSTEntities();

        MNHistory HandleHistory = new MNHistory();

        public ResponseInfo GetListDelivery(string employeeId)
        {
            var data = db.MAILER_DELIVERY_GETMAILER_EMPLOYEE(employeeId).ToList();

            return new ResponseInfo()
            {
                error = 0,
                data = data
            };
        }


        public ResultInfo GetListHistoryDelivery(string employeeId, string date)
        {
            try
            {

                var dateChoose = DateTime.ParseExact(date, "d/M/yyyy", null);

                if (dateChoose == null)
                    throw new Exception("Sai thông tin");

                var data = db.MAILER_DELIVERY_GETMAILER_EMPLOYEE_BYDATE(employeeId, dateChoose.ToString("yyyy-MM-dd")).ToList();

                return new ResponseInfo()
                {
                    error = 0,
                    data = data
                };


            }
            catch (Exception e)
            {
                return new ResultInfo()
                {
                    error = 1,
                    msg = e.Message
                };
            }


        }

        // get danh sach ly do
        public ResponseInfo GetListReturnReason()
        {
            var data = db.BS_ReturnReasons.Where(p => p.IsActive == true).Select(p => new { code = p.ReasonID, name = p.ReasonName }).ToList();

            return new ResponseInfo()
            {
                error = 0,
                data = data
            };
        }


        public ResponseInfo GetReportDelivert(string employeeId, string fDate, string tDate)
        {
            var reportDelivery = db.DELIVERY_GETREPORT_EMPLOYEE(employeeId, fDate, tDate).Select(p => new
            {
                code = p.DeliveryStatus,
                quantity = p.CountMailer
            }).ToList();

            var reportCOD = db.EMPLOYEE_DEBIT_REPORT_BY_EMPLOYEEID(employeeId, fDate, tDate).Select(p => new
            {
                code = p.AccountantConfirm,
                quantity = p.MoneySum
            }).ToList();

            return new ResponseInfo()
            {
                error = 0,
                data = new
                {
                    RDelivery = reportDelivery,
                    RCOD = reportCOD
                }
            };

        }


        // cap nhat mailer
        public ResultInfo UpdateDelivery(UpdateDeliveryReceive info, string user)
        {

            var result = new ResultInfo()
            {
                error = 0,
                msg = "success"
            };

            try
            {
                var checkUser = db.BS_Employees.Where(p => p.UserLogin == user).FirstOrDefault();

                if (checkUser == null)
                    throw new Exception("Sai thông tin");

                // check Document of employee
                var document = db.MM_MailerDelivery.Where(p => p.DocumentID == info.DocumentID && p.EmployeeID == checkUser.EmployeeID).FirstOrDefault();

                if (document == null)
                    throw new Exception("Đơn này không được phân cho bạn phát");

                // find detail
                var findDetail = db.MM_MailerDeliveryDetail.Where(p => p.DocumentID == info.DocumentID && p.MailerID == info.MailerID && p.DeliveryStatus == 3).FirstOrDefault();

                if (findDetail == null)
                    throw new Exception("Sai thông tin");


                DateTime deliverDate = DateTime.ParseExact(info.DeliveryDate, "dd/M/yyyy HH:mm", null);

                if (deliverDate == null)
                    deliverDate = DateTime.Now;

                //
                var mailerInfo = db.MM_Mailers.Find(findDetail.MailerID);

                if (mailerInfo == null)
                    throw new Exception("Vận đơn sai");

                findDetail.DeliveryStatus = info.StatusID;
                mailerInfo.CurrentStatusID = info.StatusID;

                if (info.StatusID == 5)
                {
                    //  var findReason = db.BS_ReturnReasons.Where(p => p.ReasonID == info.ReturnReasonID).FirstOrDefault();

                    findDetail.DeliveryTo = "";
                    findDetail.DeliveryNotes = info.Note;
                    findDetail.ReturnReasonID = info.ReturnReasonID;
                    findDetail.DeliveryDate = deliverDate;


                    mailerInfo.DeliveryTo = "";
                    mailerInfo.DeliveryDate = deliverDate;
                    mailerInfo.DeliveryNotes = info.Note;
                    mailerInfo.IsReturn = true;
                    HandleHistory.AddTracking(5, info.MailerID, mailerInfo.CurrentPostOfficeID, "Trả lại hàng, vì lý do " + info.Note);

                }
                else if (info.StatusID == 6)
                {
                    findDetail.DeliveryTo = "";
                    findDetail.DeliveryDate = deliverDate;
                    findDetail.DeliveryNotes = info.Note;

                    mailerInfo.DeliveryTo = "";
                    mailerInfo.DeliveryDate = deliverDate;
                    mailerInfo.DeliveryNotes = info.Note;

                    HandleHistory.AddTracking(6, info.MailerID, mailerInfo.CurrentPostOfficeID, "Chưa phát được vì " + info.Note);

                }
                else if (info.StatusID == 4)
                {
                    findDetail.DeliveryTo = info.Reciever;
                    findDetail.ReturnReasonID = null;
                    findDetail.DeliveryNotes = "Đã phát";
                    findDetail.DeliveryDate = deliverDate;

                    mailerInfo.DeliveryTo = info.Reciever;
                    mailerInfo.DeliveryDate = deliverDate;
                    mailerInfo.DeliveryNotes = "Đã phát";
                    mailerInfo.CurrentStatusID = 4;

                    if (mailerInfo.IsReturn == true)
                    {
                        findDetail.DeliveryNotes = "Đã hoàn";
                        mailerInfo.DeliveryNotes = "Đã hoàn";

                        findDetail.DeliveryStatus = 11;
                        mailerInfo.CurrentStatusID = 11;

                        HandleHistory.AddTracking(11, info.MailerID, mailerInfo.CurrentPostOfficeID, "Ngày hoàn " + deliverDate.ToString("dd/MM/yyyy") + " lúc " + deliverDate.ToString("HH:mm") + ", người nhận: " + info.Reciever);
                    }
                    else
                    {
                        HandleHistory.AddTracking(4, info.MailerID, mailerInfo.CurrentPostOfficeID, "Ngày phát " + deliverDate.ToString("dd/MM/yyyy") + " lúc " + deliverDate.ToString("HH:mm") + ", người nhận: " + info.Reciever);

                        if (mailerInfo.PaymentMethodID == "NNTT")
                        {
                            var saveCoDDebit = new EmpployeeDebitCOD()
                            {
                                Id = Guid.NewGuid().ToString(),
                                AccountantConfirm = 0,
                                COD = Convert.ToDouble(mailerInfo.COD) + Convert.ToDouble(mailerInfo.Amount),
                                Describe = "Thu: Cước + COD",
                                ConfirmDate = DateTime.Now,
                                CreateDate = DateTime.Now,
                                DocumentID = findDetail.DocumentID,
                                EmployeeID = document.EmployeeID,
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
                                    DocumentID = findDetail.DocumentID,
                                    EmployeeID = document.EmployeeID,
                                    MailerID = mailerInfo.MailerID
                                };

                                db.EmpployeeDebitCODs.Add(saveCoDDebit);
                            }
                        }
                    }


                }

                if (info.images != null)
                {
                    foreach (var image in info.images)
                    {
                        var saveImage = new MailerImage()
                        {
                            Id = Guid.NewGuid().ToString(),
                            CreateTime = DateTime.Now,
                            MailerID = info.MailerID,
                            PathImage = image,
                            UserSend = user
                        };

                        db.MailerImages.Add(saveImage);
                    }
                    db.SaveChanges();
                }

                db.Entry(mailerInfo).State = System.Data.Entity.EntityState.Modified;
                db.Entry(findDetail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                UpdateDeliveryStatus(document.DocumentID);

            }
            catch (Exception e)
            {
                result.msg = e.Message;
                result.error = 1;
            }

            return result;

        }

        public void UpdateDeliveryStatus(string documentId)
        {
            var find = db.MM_MailerDelivery.Find(documentId);

            var findDetail = db.MM_MailerDeliveryDetail.Where(p => p.DocumentID == documentId).ToList();

            var countSucess = 0;

            foreach (var item in findDetail)
            {
                if (item.DeliveryStatus == 4 || item.DeliveryStatus == 5)
                {
                    countSucess++;
                }
            }

            if (countSucess == findDetail.Count())
            {
                find.StatusID = 3;
            }
            else
            {
                find.StatusID = 2;
            }

            db.Entry(find).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }


        // lay hang
        public ResultInfo GetTakeMailer(string user)
        {
            var checkUser = db.BS_Employees.Where(p => p.UserLogin == user).FirstOrDefault();

            if (checkUser == null)
            {
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                };

            }
            var data = db.TAKEMAILER_GETLIST_BY_EMPLOYEE_NOTFINISH(checkUser.EmployeeID).ToList();

            return new ResponseInfo()
            {
                error = 0,
                msg = "",
                data = data
            };

        }

        public ResultInfo GetTakeMailerDetail(string documentID)
        {
            var data = db.TAKEMAILER_GETDETAILs(documentID).Where(p => p.CurrentStatusID == 7).ToList();

            return new ResponseInfo()
            {
                error = 0,
                msg = "",
                data = data
            };

        }

        public ResultInfo CancelTakeMailer(string user, UpdateTakeMailerReceive info)
        {
            var result = new ResultInfo()
            {
                error = 0,
                msg = "success"
            };
            var checkUser = db.BS_Employees.Where(p => p.UserLogin == user).FirstOrDefault();

            if (checkUser == null)
            {
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                };

            }
            try
            {
                var checkDocument = db.MM_TakeMailers.Find(info.documentId);

                if (checkDocument == null)
                    throw new Exception("Sai thông tin");


                var checkMailer = db.MM_Mailers.Find(info.mailers);

                var findDetail = db.MM_TakeDetails.Where(p => p.DocumentID == checkDocument.DocumentID && p.MailerID == info.mailers).FirstOrDefault();


                if (findDetail == null || checkMailer == null)
                    throw new Exception("Sai thông tin");

                findDetail.StatusID = 10;
                db.Entry(findDetail).State = System.Data.Entity.EntityState.Modified;

                //
                checkMailer.CurrentStatusID = 10;
                checkMailer.LastUpdateDate = DateTime.Now;
                db.Entry(checkMailer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                HandleHistory.AddTracking(10, info.mailers, checkMailer.CurrentPostOfficeID, "KHÁCH HÀNG YÊU CẦU HỦY KHI ĐI LẤY HÀNG");

                var checkCount = db.TAKEMAILER_GETDETAILs(checkDocument.DocumentID).Where(p => p.CurrentStatusID == 7).ToList();

                if (checkCount.Count() == 0)
                {
                    checkDocument.StatusID = 8;
                    db.Entry(checkDocument).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                result.msg = e.Message;
                result.error = 1;
            }

            return result;

        }

        public ResultInfo UpdateTakeMailer(string user, UpdateTakeMailerReceive info)
        {
            var result = new ResultInfo()
            {
                error = 0,
                msg = "success"
            };
            var checkUser = db.BS_Employees.Where(p => p.UserLogin == user).FirstOrDefault();

            if (checkUser == null)
            {
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Sai thông tin"
                };

            }
            try
            {
                var checkDocument = db.MM_TakeMailers.Find(info.documentId);

                if (checkDocument == null)
                    throw new Exception("Sai thông tin");

                var checkMailer = db.MM_Mailers.Find(info.mailers);

                var findDetail = db.MM_TakeDetails.Where(p => p.DocumentID == checkDocument.DocumentID && p.MailerID == info.mailers).FirstOrDefault();

                if (findDetail == null || checkMailer == null)
                    throw new Exception("Sai thông tin");

                findDetail.StatusID = 8;
                findDetail.TimeTake = DateTime.Now;
                db.Entry(findDetail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                checkMailer.CurrentStatusID = 8;
                checkMailer.LastUpdateDate = DateTime.Now;
                checkMailer.Weight = info.weight;


                var price = db.CalPrice(checkMailer.Weight, checkMailer.SenderID, checkMailer.RecieverProvinceID, checkMailer.MailerTypeID, checkMailer.PostOfficeAcceptID, DateTime.Now.ToString("yyyy-MM-dd")).FirstOrDefault();


                checkMailer.Price = price;
                checkMailer.PriceDefault = price;
                checkMailer.Amount = price + checkMailer.PriceCoD + checkMailer.PriceService;

                db.Entry(checkMailer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                HandleHistory.AddTracking(8, info.mailers, checkMailer.CurrentPostOfficeID, "Đã lấy hàng, đang giao về kho. Cập nhật trọng lượng: " + info.weight + " Gram");

                var checkCount = db.TAKEMAILER_GETDETAILs(checkDocument.DocumentID).Where(p => p.CurrentStatusID == 7).ToList();

                if (checkCount.Count() == 0)
                {
                    checkDocument.StatusID = 8;
                    db.Entry(checkDocument).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                result.msg = e.Message;
                result.error = 1;
            }

            return result;

        }
    }
}