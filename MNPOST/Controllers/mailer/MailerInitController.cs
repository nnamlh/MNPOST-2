using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MNPOST.Models;
using System.Web.Mvc;
using MNPOSTCOMMON;
using System.IO;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace MNPOST.Controllers.mailer
{
    public class MailerInitController : MailerController
    {



        [HttpGet]
        public ActionResult Init()
        {
            // dịch vu

            ViewBag.MailerTypes = db.BS_ServiceTypes.Where(p => p.IsActive == true).Select(p => new CommonData()
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
                choose = false,
                percent = p.IsPercent

            }).ToList(); ;




            // tinh thanh
            ViewBag.Provinces = GetProvinceDatas("", "province");
            // buu cuc
            ViewBag.PostOffices = EmployeeInfo.postOffices;



            return View();
        }

        [HttpGet]
        public ActionResult GetCustomer(string postId)
        {
            var data = db.BS_Customers.Where(p => p.IsActive == true && p.PostOfficeID == postId).Select(item => new
            {
                code = item.CustomerCode,
                name = item.CustomerName,
                phone = item.Phone,
                provinceId = item.ProvinceID,
                address = item.Address,
                districtId = item.DistrictID,
                wardId = item.WardID
            }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region
        [HttpPost]
        public ActionResult InsertByExcel(HttpPostedFileBase files, string senderID, string senderAddress, string senderName, string senderPhone, string senderProvince, string senderDistrict, string postId)
        {
            MailerHandleCommon mailerHandle = new MailerHandleCommon(db);
            //  List<MailerIdentity> mailers = new List<MailerIdentity>();
            var result = new ResultInfo()
            {
                error = 0,
                msg = "Đã tải"
            };
            string path = "";
            try
            {
                // var findVSVX = db.BS_Services.Where(p => p.ServiceID == "VSVX").FirstOrDefault();
                var allService = db.BS_Services.Select(p => new ItemPriceCommon()
                {
                    code = p.ServiceID,
                    name = p.ServiceName,
                    price = p.Price,
                    choose = false,
                    percent = p.IsPercent

                }).ToList();
                // check sender
                var checkSender = db.BS_Customers.Where(p => p.CustomerCode == senderID).FirstOrDefault();

                if (checkSender == null)
                    throw new Exception("Sai thông tin người gửi");

                var checkSendProvince = db.BS_Provinces.Find(senderProvince);

                if (checkSendProvince == null)
                    throw new Exception("Sai thông tin tỉnh thành");

                var checkSendDistrict = db.BS_Districts.Find(senderDistrict);
                if (checkSendDistrict == null)
                    throw new Exception("Sai thông tin quận huyện ");

                if (files == null || files.ContentLength <= 0)
                    throw new Exception("Thiếu file Excel");

                string extension = System.IO.Path.GetExtension(files.FileName);

                if (extension.Equals(".xlsx") || extension.Equals(".xls"))
                {
                    string fileSave = "mailersupload" + DateTime.Now.ToString("ddMMyyyyhhmmss") + extension;
                    path = Server.MapPath("~/Temps/" + fileSave);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    files.SaveAs(path);
                    FileInfo newFile = new FileInfo(path);
                    var package = new ExcelPackage(newFile);

                    ExcelWorksheet sheet = package.Workbook.Worksheets[1];

                    int totalRows = sheet.Dimension.End.Row;
                    int totalCols = sheet.Dimension.End.Column;

                    // 
                    int mailerCodeIdx = -1;
                    int receiverIdx = -1;
                    int receiPhoneIdx = -1;
                    int receiAddressIdx = -1;
                    int receiProvinceIdx = -1;
                    int receiDistrictIdx = -1;
                    int mailerTypeIdx = -1;
                    int payTypeIdx = -1;
                    int codIdx = -1;
                    int merchandiseIdx = -1;
                    int weigthIdx = -1;
                    int quantityIdx = -1;
                    int notesIdx = -1;
                    int desIdx = -1;
                    int vsvxIdx = -1;

                    // lay index col tren excel
                    for (int i = 0; i < totalCols; i++)
                    {
                        var colValue = Convert.ToString(sheet.Cells[1, i + 1].Value).Trim();

                        Regex regex = new Regex(@"\((.*?)\)");
                        Match match = regex.Match(colValue);

                        if (match.Success)
                        {
                            string key = match.Groups[1].Value;

                            switch (key)
                            {
                                case "1":
                                    mailerCodeIdx = i + 1;
                                    break;
                                case "2":
                                    receiverIdx = i + 1;
                                    break;
                                case "3":
                                    receiPhoneIdx = i + 1;
                                    break;
                                case "4":
                                    receiAddressIdx = i + 1;
                                    break;
                                case "5":
                                    receiProvinceIdx = i + 1;
                                    break;
                                case "6":
                                    receiDistrictIdx = i + 1;
                                    break;
                                case "8":
                                    mailerTypeIdx = i + 1;
                                    break;
                                case "9":
                                    payTypeIdx = i + 1;
                                    break;
                                case "10":
                                    codIdx = i + 1;
                                    break;
                                case "11":
                                    merchandiseIdx = i + 1;
                                    break;
                                case "12":
                                    weigthIdx = i + 1;
                                    break;
                                case "13":
                                    quantityIdx = i + 1;
                                    break;
                                case "17":
                                    notesIdx = i + 1;
                                    break;
                                case "18":
                                    desIdx = i + 1;
                                    break;
                                case "14":
                                    vsvxIdx = i + 1;
                                    break;
                            }

                        }
                    }

                    // check cac gia tri can
                    if (receiverIdx == -1 || receiAddressIdx == -1 || receiPhoneIdx == -1 || receiProvinceIdx == -1 || weigthIdx == -1)
                        throw new Exception("Thiếu các cột cần thiết");

                    for (int i = 2; i <= totalRows; i++)
                    {
                        string mailerId = mailerCodeIdx == -1 ? mailerHandle.GeneralMailerCode(postId) : Convert.ToString(sheet.Cells[i, mailerCodeIdx].Value);
                        if (String.IsNullOrEmpty(mailerId))
                            mailerId = mailerHandle.GeneralMailerCode(postId);
                        //
                        string receiverPhone = Convert.ToString(sheet.Cells[i, receiPhoneIdx].Value);
                        if (String.IsNullOrEmpty(receiverPhone))
                            throw new Exception("Dòng " + (i) + " cột " + receiPhoneIdx + " : thiếu thông tin");

                        //
                        string receiver = Convert.ToString(sheet.Cells[i, receiverIdx].Value);
                        if (String.IsNullOrEmpty(receiver))
                            throw new Exception("Dòng " + (i) + " cột " + receiverIdx + " : thiếu thông tin");
                        //
                        string receiverAddress = Convert.ToString(sheet.Cells[i, receiAddressIdx].Value);
                        if (String.IsNullOrEmpty(receiverAddress))
                            throw new Exception("Dòng " + (i) + " cột " + receiAddressIdx + " : thiếu thông tin");
                        // 
                        string receiverProvince = receiProvinceIdx == -1 ? "" : Convert.ToString(sheet.Cells[i, receiProvinceIdx].Value);
                        var checkProvince = db.BS_Provinces.Where(p => p.ProvinceCode == receiverProvince).FirstOrDefault();


                        //
                        string receiverDistrict = receiDistrictIdx == -1 ? "" : Convert.ToString(sheet.Cells[i, receiDistrictIdx].Value);
                        var receiverDistrictSplit = receiverDistrict.Split('-');
                        var checkDistrict = db.BS_Districts.Find(receiverDistrictSplit[0]);

                        string mailerType = Convert.ToString(sheet.Cells[i, mailerTypeIdx].Value);
                        var checkMailerType = db.BS_ServiceTypes.Find(mailerType);

                        //
                        var mailerPay = payTypeIdx == -1 ? "NGTT" : Convert.ToString(sheet.Cells[i, payTypeIdx].Value);
                        if (payTypeIdx != -1)
                        {
                            var checkMailerPay = db.CDatas.Where(p => p.Code == mailerPay && p.CType == "MAILERPAY").FirstOrDefault();
                            mailerPay = checkMailerPay == null ? "NGTT" : checkMailerPay.Code;
                        }

                        // COD
                        var codValue = sheet.Cells[i, codIdx].Value;
                        decimal cod = 0;
                        if (codValue != null)
                        {
                            var isCodeNumber = codIdx == -1 ? false : Regex.IsMatch(codValue.ToString(), @"^\d+$");
                            cod = isCodeNumber ? Convert.ToDecimal(codValue) : 0;
                        }

                        // hang hoa
                        var merchandisType = Convert.ToString(sheet.Cells[i, merchandiseIdx].Value);
                        var checkMerchandisType = db.CDatas.Where(p => p.Code == merchandisType && p.CType == "GOODTYPE").FirstOrDefault();
                        if (checkMerchandisType == null)
                            throw new Exception("Dòng " + (i) + " cột " + merchandiseIdx + " : sai thông tin");

                        // trong luong
                        var weightValue = sheet.Cells[i, weigthIdx].Value;
                        double weight = 0;
                        if (weightValue == null)
                            throw new Exception("Dòng " + (i) + " cột " + weigthIdx + " : sai thông tin");
                        else
                        {
                            var isWeightNumber = Regex.IsMatch(weightValue.ToString(), @"^\d+$");
                            weight = isWeightNumber ? Convert.ToDouble(sheet.Cells[i, weigthIdx].Value) : 0;
                        }

                        // so luong
                        var quantityValue = sheet.Cells[i, quantityIdx].Value;
                        var isQuantityNumber = quantityIdx == -1 ? false : Regex.IsMatch(quantityValue == null ? "0" : quantityValue.ToString(), @"^\d+$");
                        var quantity = isQuantityNumber ? Convert.ToInt32(quantityValue) : 0;
                        //
                        string notes = notesIdx == -1 ? "" : Convert.ToString(sheet.Cells[i, notesIdx].Value);

                        //
                        string describe = desIdx == -1 ? "" : Convert.ToString(sheet.Cells[i, desIdx].Value);
                        string vsvs = vsvxIdx == -1 ? "N" : Convert.ToString(sheet.Cells[i, vsvxIdx].Value);
                        decimal? price = 0;

                        if (cod > 0)
                        {

                            price = db.CalPriceCOD(weight, senderID, checkProvince.ProvinceID, "CD", postId, DateTime.Now.ToString("yyyy-MM-dd"), vsvs == "N" ? 0 : 1, checkMailerType.ServiceID == "ST" ? "CODTK" : "CODN").FirstOrDefault();
                        }
                        else
                        {
                            price = db.CalPrice(weight, senderID, checkProvince.ProvinceID, checkMailerType.ServiceID, postId, DateTime.Now.ToString("yyyy-MM-dd")).FirstOrDefault();
                        }

                        var codPrice = 0;

                        decimal? priceService = 0;


                        // theem
                        var mailerIns = new MM_Mailers()
                        {
                            MailerID = mailerId,
                            AcceptTime = DateTime.Now,
                            AcceptDate = DateTime.Now,
                            COD = cod,
                            CreationDate = DateTime.Now,
                            CurrentStatusID = 0,
                            HeightSize = 0,
                            Weight = weight,
                            LengthSize = 0,
                            WidthSize = 0,
                            Quantity = quantity,
                            PostOfficeAcceptID = postId,
                            CurrentPostOfficeID = postId,
                            EmployeeAcceptID = EmployeeInfo.employeeId,
                            MailerDescription = describe,
                            MailerTypeID = checkMailerType != null ? checkMailerType.ServiceID : "",
                            MerchandiseValue = cod,
                            MerchandiseID = merchandisType,
                            PriceDefault = price,
                            Price = price,
                            PriceService = priceService,
                            Amount = price + codPrice + priceService,
                            PriceCoD = codPrice,
                            Notes = notes,
                            PaymentMethodID = mailerPay,
                            RecieverAddress = receiverAddress,
                            RecieverName = receiver,
                            RecieverPhone = receiverPhone,
                            RecieverDistrictID = checkDistrict != null ? checkDistrict.DistrictID : "",
                            RecieverWardID = "",
                            RecieverProvinceID = checkProvince != null ? checkProvince.ProvinceID : "",
                            SenderID = senderID,
                            SenderAddress = senderAddress,
                            SenderDistrictID = senderDistrict,
                            SenderName = senderName,
                            SenderPhone = senderPhone,
                            SenderProvinceID = senderProvince,
                            SenderWardID = "",
                            PaidCoD = 0,
                            CreateType = 0,
                            VATPercent = 10,
                            IsReturn = false,
                            IsPayment = 0,
                            IsPostAccept = false
                        };

                        try
                        {
                            // 
                            db.MM_Mailers.Add(mailerIns);
                            db.SaveChanges();

                            if (vsvs == "Y" && cod == 0)
                            {
                                // services.Where(p => p.code == "VSVX").FirstOrDefault().choose = true;
                                var serviceVSVX = allService.Where(p => p.code == "VSVX").FirstOrDefault();

                                if (serviceVSVX.percent == true)
                                {
                                    priceService = (price * serviceVSVX.price) / 100;
                                }
                                else
                                {
                                    priceService = serviceVSVX.price;
                                }

                                var mailerService = new MM_MailerServices()
                                {
                                    MailerID = mailerId,
                                    CreationDate = DateTime.Now,
                                    SellingPrice = (decimal)priceService,
                                    PriceDefault = (decimal)serviceVSVX.price,
                                    ServiceID = serviceVSVX.code
                                };
                                db.MM_MailerServices.Add(mailerService);

                                db.SaveChanges();

                                mailerIns.PriceService = priceService;
                                mailerIns.Amount = mailerIns.Price + mailerIns.PriceCoD + mailerIns.PriceService;

                                db.Entry(mailerIns).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }

                            // luu tracking
                            HandleHistory.AddTracking(0, mailerId, postId, "Nhận thông tin đơn hàng");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }
                    // xoa file temp
                    package.Dispose();
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                }

                //    result.data = mailers;
            }
            catch (Exception e)
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                result.error = 1;
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion



        [HttpPost]
        public ActionResult InsertMailers(List<MailerIdentity> mailers, string postId)
        {

            if (mailers == null)
                return Json(new { error = 1, msg = "Hoàn thành" }, JsonRequestBehavior.AllowGet);

            if (mailers.Count() > 300)
                return Json(new { error = 1, msg = "Để đảm bảo hệ thống chỉ update 300/1 lần" }, JsonRequestBehavior.AllowGet);

            var checkPost = db.BS_PostOffices.Find(postId);

            if (checkPost == null)
                return Json(new { error = 1, msg = "chọn bưu cục" }, JsonRequestBehavior.AllowGet);

            List<MailerIdentity> insertFail = new List<MailerIdentity>();

            foreach (var item in mailers)
            {
                // checkMailer
                if (String.IsNullOrEmpty(item.MailerID))
                {
                    insertFail.Add(item);
                    continue;
                }

                var checkExist = db.MM_Mailers.Where(p => p.MailerID == item.MailerID).FirstOrDefault();

                if (checkExist != null)
                {
                    insertFail.Add(item);
                    continue;
                }

                var checkSender = db.BS_Customers.Where(p => p.CustomerCode == item.SenderID).FirstOrDefault();
                if (checkSender == null)
                {
                    insertFail.Add(item);
                    continue;
                }

                decimal? price = 0;
                if (item.COD > 0)
                {
                    var findDitrict = db.BS_Districts.Where(p => p.DistrictID == item.RecieverDistrictID).FirstOrDefault();
                    int? vsvx = findDitrict == null ? 1 : (findDitrict.VSVS == true ? 1 : 0);
                    price = db.CalPriceCOD(item.Weight, checkSender.CustomerID, item.RecieverProvinceID, "CD", postId, DateTime.Now.ToString("yyyy-MM-dd"), vsvx, item.MailerTypeID == "ST" ? "CODTK" : "CODN").FirstOrDefault();
                }
                else
                {
                    price = db.CalPrice(item.Weight, checkSender.CustomerID, item.RecieverProvinceID, item.MailerTypeID, postId, DateTime.Now.ToString("yyyy-MM-dd")).FirstOrDefault();
                }

                var codPrice = 0;
                // theem
                var mailerIns = new MM_Mailers()
                {
                    MailerID = item.MailerID,
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
                    PostOfficeAcceptID = postId,
                    CurrentPostOfficeID = postId,
                    EmployeeAcceptID = EmployeeInfo.employeeId,
                    MailerDescription = item.MailerDescription,
                    MailerTypeID = item.MailerTypeID,
                    MerchandiseValue = item.MerchandiseValue,
                    MerchandiseID = item.MerchandiseID,
                    PriceDefault = price,
                    Price = price,
                    PriceService = item.PriceService,
                    Amount = price + codPrice + item.PriceService,
                    PriceCoD = codPrice,
                    Notes = item.Notes,
                    PaymentMethodID = item.PaymentMethodID,
                    RecieverAddress = item.RecieverAddress,
                    RecieverName = item.RecieverName,
                    RecieverPhone = item.RecieverPhone,
                    RecieverDistrictID = item.RecieverDistrictID,
                    RecieverWardID = item.RecieverWardID,
                    RecieverProvinceID = item.RecieverProvinceID,
                    SenderID = item.SenderID,
                    SenderAddress = item.SenderAddress,
                    SenderDistrictID = item.SenderDistrictID,
                    SenderName = item.SenderName,
                    SenderPhone = item.SenderPhone,
                    SenderProvinceID = item.SenderProvinceID,
                    SenderWardID = item.SenderWardID,
                    PaidCoD = 0,
                    CreateType = 0,
                    VATPercent = 10,
                    IsReturn = false,
                    IsPayment = 0,
                    IsPostAccept = false
                };

                try
                {
                    // 
                    db.MM_Mailers.Add(mailerIns);
                    db.SaveChanges();

                    // add addressTemp
                    var findAddressTemp = db.AddressTemps.Where(p => p.Phone == mailerIns.RecieverPhone).FirstOrDefault();

                    if (findAddressTemp != null)
                    {
                        findAddressTemp.AddressInfo = mailerIns.RecieverAddress;
                        findAddressTemp.DistrictId = mailerIns.RecieverDistrictID;
                        findAddressTemp.ProvinceId = mailerIns.RecieverProvinceID;
                        findAddressTemp.WardId = mailerIns.RecieverWardID;
                        findAddressTemp.Name = mailerIns.RecieverName;

                        db.Entry(findAddressTemp).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var insAddressInfo = new AddressTemp()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = mailerIns.RecieverName,
                            Phone = mailerIns.RecieverPhone,
                            AddressInfo = mailerIns.RecieverAddress,
                            DistrictId = mailerIns.RecieverDistrictID,
                            ProvinceId = mailerIns.RecieverProvinceID,
                            WardId = mailerIns.RecieverWardID
                        };

                        db.AddressTemps.Add(insAddressInfo);
                        db.SaveChanges();
                    }

                    // save service
                    if (item.Services != null)
                    {

                        decimal? totalPriceService = 0;
                        foreach (var service in item.Services)
                        {
                            var checkService = db.BS_Services.Where(p => p.ServiceID == service.code && p.IsActive == true).FirstOrDefault();
                            if (checkService != null && service.choose == true)
                            {
                                var servicePrice = service.price;

                                if (service.percent == true)
                                {
                                    servicePrice = (servicePrice * mailerIns.Price) / 100;
                                }
                                totalPriceService = totalPriceService + servicePrice;
                                var mailerService = new MM_MailerServices()
                                {
                                    MailerID = item.MailerID,
                                    CreationDate = DateTime.Now,
                                    SellingPrice = (decimal)servicePrice,
                                    PriceDefault = (decimal)checkService.Price,
                                    ServiceID = service.code
                                };
                                db.MM_MailerServices.Add(mailerService);
                            }
                        }

                        db.SaveChanges();

                        mailerIns.PriceService = totalPriceService;
                        mailerIns.Amount = mailerIns.Price + mailerIns.PriceCoD + mailerIns.PriceService;

                        db.Entry(mailerIns).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }


                    // luu tracking
                    HandleHistory.AddTracking(0, item.MailerID, postId, "Nhận thông tin đơn hàng");
                }
                catch
                {
                    insertFail.Add(item);
                    continue;
                }
            }


            return Json(new { error = 0, data = insertFail }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetProvinces(string parentId, string type)
        {
            return Json(GetProvinceDatas(parentId, type), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDistrictSendAndRecei(string provinceSend, string provinceRecie)
        {
            var listSend = GetProvinceDatas(provinceSend, "district");
            var listRecei = GetProvinceDatas(provinceRecie, "district");
            return Json(new
            {
                send = listSend,
                recei = listRecei

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDistrictAndWard(string provinceId, string districtId)
        {
            return Json(new { districts = GetProvinceDatas(provinceId, "district"), wards = GetProvinceDatas(districtId, "ward") }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GeneralCode(string postId)
        {
            MailerHandleCommon mailerHandle = new MailerHandleCommon(db);
            var code = mailerHandle.GeneralMailerCode(postId);

            return Json(new { error = 0, code = code }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProvinceFromAddress(string province, string district, string ward)
        {
            var findProvince = db.BS_Provinces.Where(p => p.ProvinceName == province).FirstOrDefault();

            if (findProvince == null)
                return Json(new { provinceId = "", districtId = "", wardId = "" }, JsonRequestBehavior.AllowGet);


            var findDistrict = db.BS_Districts.Where(p => p.ProvinceID == findProvince.ProvinceID && p.DistrictName == district).FirstOrDefault();

            if (findDistrict == null)
                return Json(new { provinceId = findProvince.ProvinceID, districtId = "", wardId = "" }, JsonRequestBehavior.AllowGet);


            var findWard = db.BS_Wards.Where(p => p.DistrictID == findDistrict.DistrictID && p.WardName == ward).FirstOrDefault();

            if (findWard == null)
                return Json(new { provinceId = findProvince.ProvinceID, districtId = findDistrict.DistrictID, wardId = "" }, JsonRequestBehavior.AllowGet);


            return Json(new { provinceId = findProvince.ProvinceID, districtId = findDistrict.DistrictID, wardId = findWard.WardID }, JsonRequestBehavior.AllowGet);
        }



    }
}