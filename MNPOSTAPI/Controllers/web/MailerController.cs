using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MNPOSTAPI.Models;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using MNPOSTCOMMON;

namespace MNPOSTAPI.Controllers.web
{
    public class MailerController : WebBaseController
    {

        [HttpPost]
        public ResultInfo AddMailer()
        {
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "Them moi thanh cong"
            };

            try
            {
                var requestContent = Request.Content.ReadAsStringAsync().Result;

                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<MailerIdentity>(requestContent);

                var findCus = db.BS_Customers.Where(p => p.CustomerCode == paser.SenderID).FirstOrDefault();

                if (findCus == null)
                    throw new Exception("Sai thông tin");

                if (String.IsNullOrEmpty(findCus.Address) || String.IsNullOrEmpty(findCus.ProvinceID) || String.IsNullOrEmpty(findCus.DistrictID) || String.IsNullOrEmpty(findCus.CustomerName))
                    throw new Exception("Cập nhật lại thông tin cá nhân");

                MailerHandleCommon mailerHandle = new MailerHandleCommon(db);
                var code = mailerHandle.GeneralMailerCode(findCus.PostOfficeID);
                var price = db.CalPrice(paser.Weight, findCus.CustomerID, paser.RecieverProvinceID, paser.MailerTypeID, findCus.PostOfficeID, DateTime.Now.ToString("yyyy-MM-dd")).FirstOrDefault();
                var codPrice = 0;


                // theem
                var mailerIns = new MM_Mailers()
                {
                    MailerID = code,
                    AcceptTime = DateTime.Now,
                    AcceptDate = DateTime.Now,
                    COD = paser.COD,
                    CreationDate = DateTime.Now,
                    CurrentStatusID = 0,
                    HeightSize = paser.HeightSize,
                    Weight = paser.Weight,
                    LengthSize = paser.LengthSize,
                    WidthSize = paser.WidthSize,
                    Quantity = paser.Quantity,
                    PostOfficeAcceptID = findCus.PostOfficeID,
                    CurrentPostOfficeID = findCus.PostOfficeID,
                    EmployeeAcceptID = "",
                    MailerDescription = paser.MailerDescription,
                    MailerTypeID = paser.MailerTypeID,
                    MerchandiseValue = paser.MerchandiseValue,
                    MerchandiseID = paser.MerchandiseID,
                    PriceDefault = price,
                    Price = price,
                    PriceService = paser.PriceService,
                    Amount = price + codPrice,
                    PriceCoD = codPrice,
                    Notes = paser.Notes,
                    PaymentMethodID = paser.PaymentMethodID,
                    RecieverAddress = paser.RecieverAddress,
                    RecieverName = paser.RecieverName,
                    RecieverPhone = paser.RecieverPhone,
                    RecieverDistrictID = paser.RecieverDistrictID,
                    RecieverWardID = paser.RecieverWardID,
                    RecieverProvinceID = paser.RecieverProvinceID,
                    SenderID = findCus.CustomerCode,
                    SenderAddress = findCus.Address,
                    SenderDistrictID = findCus.DistrictID,
                    SenderName = findCus.CustomerName,
                    SenderPhone = findCus.Phone,
                    SenderProvinceID = findCus.ProvinceID,
                    SenderWardID = findCus.WardID,
                    PaidCoD = 0,
                    CreateType = 1
                };

                // 
                db.MM_Mailers.Add(mailerIns);
                db.SaveChanges();

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }
            return result;
        }



        [HttpPost]
        public ResultInfo AddListMailer()
        {
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "Them moi thanh cong"
            };

            try
            {
                var requestContent = Request.Content.ReadAsStringAsync().Result;

                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<List<MailerIdentity>>(requestContent);

                foreach (var item in paser)
                {
                    var findCus = db.BS_Customers.Where(p => p.CustomerCode == item.SenderID).FirstOrDefault();

                    if (findCus == null)
                        throw new Exception("Sai thông tin");

                    if (String.IsNullOrEmpty(findCus.Address) || String.IsNullOrEmpty(findCus.ProvinceID) || String.IsNullOrEmpty(findCus.DistrictID) || String.IsNullOrEmpty(findCus.CustomerName))
                        throw new Exception("Cập nhật lại thông tin cá nhân");

                    MailerHandleCommon mailerHandle = new MailerHandleCommon(db);
                    var code = mailerHandle.GeneralMailerCode(findCus.PostOfficeID);
                    var price = db.CalPrice(item.Weight, findCus.CustomerID, item.RecieverProvinceID, item.MailerTypeID, findCus.PostOfficeID, DateTime.Now.ToString("yyyy-MM-dd")).FirstOrDefault();
                    var codPrice = 0;


                    // theem
                    var mailerIns = new MM_Mailers()
                    {
                        MailerID = code,
                        AcceptTime = DateTime.Now,
                        AcceptDate = DateTime.Now,
                        COD = item.COD,
                        CreationDate = DateTime.Now,
                        CurrentStatusID = 0,
                        HeightSize = item.HeightSize,
                        Weight = item.Weight,
                        LengthSize = item.LengthSize,
                        WidthSize = item.WidthSize,
                        Quantity = item.Quantity,
                        PostOfficeAcceptID = findCus.PostOfficeID,
                        CurrentPostOfficeID = findCus.PostOfficeID,
                        EmployeeAcceptID = "",
                        MailerDescription = item.MailerDescription,
                        MailerTypeID = item.MailerTypeID,
                        MerchandiseValue = item.MerchandiseValue,
                        MerchandiseID = item.MerchandiseID,
                        PriceDefault = price,
                        Price = price,
                        PriceService = item.PriceService,
                        Amount = price + codPrice,
                        PriceCoD = codPrice,
                        Notes = item.Notes,
                        PaymentMethodID = item.PaymentMethodID,
                        RecieverAddress = item.RecieverAddress,
                        RecieverName = item.RecieverName,
                        RecieverPhone = item.RecieverPhone,
                        RecieverDistrictID = item.RecieverDistrictID,
                        RecieverWardID = item.RecieverWardID,
                        RecieverProvinceID = item.RecieverProvinceID,
                        SenderID = findCus.CustomerCode,
                        SenderAddress = findCus.Address,
                        SenderDistrictID = findCus.DistrictID,
                        SenderName = findCus.CustomerName,
                        SenderPhone = findCus.Phone,
                        SenderProvinceID = findCus.ProvinceID,
                        SenderWardID = findCus.WardID,
                        PaidCoD = 0,
                        CreateType = 1
                    };

                    // 
                    db.MM_Mailers.Add(mailerIns);
                    db.SaveChanges();
                }

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }
            return result;
        }

        [HttpPost]
        public ResultInfo GetMailers()
        {
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "Them moi thanh cong"
            };

            try
            {
                var requestContent = Request.Content.ReadAsStringAsync().Result;

                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<MailerShowRequest>(requestContent);

                int pageSize = 50;

                int pageNumber = (paser.page ?? 1);


                DateTime paserFromDate = DateTime.Now;
                DateTime paserToDate = DateTime.Now;

                try
                {
                    paserFromDate = DateTime.ParseExact(paser.fromDate, "dd/MM/yyyy", null);
                    paserToDate = DateTime.ParseExact(paser.toDate, "dd/MM/yyyy", null);
                }
                catch
                {
                    paserFromDate = DateTime.Now;
                    paserToDate = DateTime.Now;
                }

                var findCus = db.BS_Customers.Where(p => p.CustomerCode == paser.customerId).FirstOrDefault();
                if (findCus == null)
                    throw new Exception("sai thông tin");

                var data = db.MAILER_GETALL(paserFromDate.ToString("yyyy-MM-dd"), paserToDate.ToString("yyyy-MM-dd"), "%" + findCus.PostOfficeID + "%", "%" + paser.search + "%").Where(p => p.SenderID.Contains(paser.customerId)).ToList();

                if (paser.status != -1)
                {
                    data = data.Where(p => p.CurrentStatusID == paser.status).ToList();
                }

                result = new ResultWithPaging()
                {
                    error = 0,
                    msg = "",
                    page = pageNumber,
                    pageSize = pageSize,
                    toltalSize = data.Count(),
                    data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                };

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }
            return result;

        }

        [HttpGet]
        public ResultInfo FindMailer(string mailerId)
        {
            var mailer = db.MAILER_GETINFO_BYID(mailerId).FirstOrDefault();

            if (mailer == null)
            {
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Không tìm thấy"
                };
            }
            else
            {
                var data = db.MAILER_GETTRACKING(mailerId).ToList();
                var images = db.MailerImages.Where(p => p.MailerID == mailerId).Select(p => new
                {
                    url = p.PathImage,
                    time = p.CreateTime.Value.ToString("dd/MM/yyyy HH:mm")
                });
                return new ResponseInfo()
                {
                    error = 0,
                    data = new
                    {
                        mailer = mailer,
                        tracks = data
                    }

                };
            }
        }


        [HttpPost]
        public ResultInfo CancelMailers()
        {
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "Đã thực hiện"
            };
            try
            {
                var requestContent = Request.Content.ReadAsStringAsync().Result;

                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<CancelMailerRequest>(requestContent);
                var findMailer = db.MM_Mailers.Find(paser.mailerId);

                if (findMailer != null && findMailer.CurrentStatusID == 0)
                {
                    // moi tao moi dc huy
                    findMailer.CurrentStatusID = 10;
                    findMailer.LastUpdateDate = DateTime.Now;
                    findMailer.StatusNotes = paser.reason;
                    db.Entry(findMailer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }
            return result;

        }


        [HttpGet]
        public ResultInfo ReportCusCoD(string cusId)
        {
            var findCus = db.BS_Customers.Where(p => p.CustomerCode == cusId).FirstOrDefault();

            if (findCus == null)
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Không có chơi phá"
                };

            var findAllCus = db.BS_Customers.Where(p => p.CustomerGroupID == findCus.CustomerGroupID).Select(p => p.CustomerCode).ToList();

            var data = db.MM_Mailers.Where(p => findAllCus.Contains(p.SenderID) && p.PaidCoD != 2 && p.COD > 0 && p.CurrentStatusID == 4).ToList();

            var countMailer = data.Count();
            var sumCoD = data.Sum(p => p.COD);

            return new ResponseInfo()
            {
                error = 0,
                data = new
                {
                    countMailer = countMailer,
                    sumCoD = sumCoD
                }
            };
        }


        [HttpPost]
        public ResultInfo CoDBill()
        {
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "Them moi thanh cong"
            };

            try
            {
                var requestContent = Request.Content.ReadAsStringAsync().Result;

                var jsonserializer = new JavaScriptSerializer();
                var paser = jsonserializer.Deserialize<CoDShowRequest>(requestContent);

                int pageSize = 50;

                int pageNumber = (paser.page ?? 1);


                DateTime paserFromDate = DateTime.Now;
                DateTime paserToDate = DateTime.Now;

                try
                {
                    paserFromDate = DateTime.ParseExact(paser.fromDate, "dd/MM/yyyy", null);
                    paserToDate = DateTime.ParseExact(paser.toDate, "dd/MM/yyyy", null);
                }
                catch
                {
                    paserFromDate = DateTime.Now;
                    paserToDate = DateTime.Now;
                }

                var findCus = db.BS_Customers.Where(p => p.CustomerCode == paser.customerId).FirstOrDefault();
                if (findCus == null)
                    throw new Exception("sai thông tin");

                var findGroup = db.BS_CustomerGroups.Find(findCus.CustomerGroupID);

                if (findGroup == null)
                    throw new Exception("sai thông tin");

                var data = db.CUSTOMER_COD_DEBIT_GETDOCUMENTS(paserFromDate.ToString("yyyy-MM-dd"), paserToDate.ToString("yyyy-MM-dd"), "%" + findGroup.CustomerGroupCode + "%").ToList();

                result = new ResultWithPaging()
                {
                    error = 0,
                    msg = "",
                    page = pageNumber,
                    pageSize = pageSize,
                    toltalSize = data.Count(),
                    data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                };

            }
            catch (Exception e)
            {
                result.error = 1;
                result.msg = e.Message;
            }
            return result;
        }


        [HttpGet]
        public ResultInfo GetCoDBillDetail(string documentId)
        {
            var data = db.CUSTOMER_COD_DEBIT_GETDOCUMENT_DETAILS(documentId).ToList();

            return new ResponseInfo()
            {
                error = 0,
                data = data
            };
        }


        // cong no
        [HttpGet]
        public ResultInfo CustomerDebitNoPaid (string cusId)
        {
            // danh sách các thang chưa thanh tonas
            var findCus = db.BS_Customers.Where(p => p.CustomerCode == cusId).FirstOrDefault();

            if (findCus == null)
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Không có chơi phá"
                };

            var findGroup = db.BS_CustomerGroups.Find(findCus.CustomerGroupID);
            if (findGroup == null)
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Không có chơi phá"
                };

            var data = db.CUSTOMER_DEBIT_GET_NOPAID(findGroup.CustomerGroupCode).ToList();

            return new ResponseInfo()
            {
                data = data,
                error = 0,
                msg = ""
            };

        }

        [HttpGet]
        public ResultInfo CustomerDebitFindByMonth(string cusId, int month, int year)
        {
            // danh sách các thang chưa thanh tonas
            var findCus = db.BS_Customers.Where(p => p.CustomerCode == cusId).FirstOrDefault();

            if (findCus == null)
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Không có chơi phá"
                };

            var findGroup = db.BS_CustomerGroups.Find(findCus.CustomerGroupID);
            if (findGroup == null)
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Không có chơi phá"
                };

            var data = db.CUSTOMER_DEBIT_FIND_BYDebtMonth(findGroup.CustomerGroupCode, month, year).FirstOrDefault();

            return new ResponseInfo()
            {
                data = data,
                error = 0,
                msg = ""
            };

        }

        //lay ra danh sach chi tiet phieu gui dua vao documentid
        [HttpGet]
        public ResultInfo CustomerDebitDetail(string documentid)
        {
            var data = (from mm in db.MM_Mailers
                        join d in db.AC_CustomerDebitVoucherDetail
                        on mm.MailerID equals d.MailerID
                        join p in db.BS_Provinces
                        on mm.RecieverProvinceID equals p.ProvinceID
                        where d.DocumentID == documentid
                        select new
                        {
                            MailerID = mm.MailerID,
                            AcceptDate = mm.AcceptDate,
                            ReciveprovinceID = p.ProvinceCode,
                            MerchandiseID = mm.MerchandiseID,
                            MailerTypeID = mm.MailerTypeID,
                            Quantity = mm.Quantity,
                            Weight = mm.Weight,
                            Price = mm.Price,
                            PriceService = mm.PriceService,
                            Discount = mm.Discount,
                            Amount = mm.Amount,
                            COD = mm.COD
                        }).ToList();
            if (data == null)
            {
                return new ResultInfo()
                {
                    error = 1,
                    msg = "Không tìm th?y"
                };
            }

            return new ResponseInfo()
            {
                data = data,
                error = 0,
                msg = ""
            };
        }

    }
}
