using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOST.Models;
using MNPOSTCOMMON;
using System.Globalization;
using System.Data.SqlClient;
using MNPOST.Report;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using MNPOST.Report.customer;
namespace MNPOST.Controllers.customerdebit
{
    public class CustomerDebitController : BaseController
    {

        public ActionResult Show()
        {

            ViewBag.CustomerGroup = db.BS_CustomerGroups.Select(p => new
            {
                code = p.CustomerGroupCode,
                name = p.CustomerGroupName
            }).ToList();
            return View();
        }


        public ActionResult ReportDebit()
        {
            var data = db.CUS_DEBIT_NOTPAY().ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult ShowReport(string docid)
        {
            // lam như phân lấy dữ liệu thư viện
            Dictionary<string, string> textValues = new Dictionary<string, string>();
            var listmaster = db.AC_CustomerDebitVoucher.Where(p => p.DocumentID == docid).FirstOrDefault();

            textValues.Add("txtDocumentID", docid);
            textValues.Add("txtDebtMonth", DateTime.Parse(listmaster.DebtMonth.ToString()).ToString("MM/yyyy"));
            textValues.Add("txtGhiChu", listmaster.Description);
            textValues.Add("txtCustomerID", listmaster.CustomerGroupID);
            textValues.Add("txttotal", string.Format("{0:n0}", listmaster.ToTalAmount)); //tong tien

            //txttenkh
            var query =
                   (from a in db.AC_CustomerDebitVoucher
                    join b in db.BS_CustomerGroups on a.CustomerGroupID equals b.CustomerGroupCode
                    where a.DocumentID == docid
                    select b.CustomerGroupName).FirstOrDefault();
            textValues.Add("txttenkh", query);
            //
            decimal totalprice = (from a in db.AC_CustomerDebitVoucherDetail
                                  where a.DocumentID == docid
                                  select a.Price).Sum();
            textValues.Add("txttongcuoc", string.Format("{0:n0}", totalprice));

            decimal discount = (from a in db.AC_CustomerDebitVoucherDetail
                                where a.DocumentID == docid
                                select a.Discount).Sum();


            decimal vat = (from a in db.AC_CustomerDebitVoucherDetail
                           where a.DocumentID == docid
                           select a.VATamount).Sum();
            textValues.Add("txtgiamtru", string.Format("{0:n0}", discount));
            textValues.Add("txtvat", string.Format("{0:n0}", vat));

            Dictionary<string, Dictionary<string, string>> values = new Dictionary<string, Dictionary<string, string>>();
            values.Add("Section2", textValues); // 

            //lay data
            var results = (from mm in db.MM_Mailers
                           join d in db.AC_CustomerDebitVoucherDetail
                           on mm.MailerID equals d.MailerID
                           join p in db.BS_Provinces
                           on mm.RecieverProvinceID equals p.ProvinceID
                           where d.DocumentID == docid
                           select new
                           {
                               SoPhieu = mm.MailerID,
                               NgayGui = mm.AcceptDate,
                               NoiDen = p.ProvinceCode,
                               DichVu = mm.MailerTypeID,
                               SoLuong = mm.Quantity,
                               TrongLuong = mm.Weight,
                               CuocPhi = d.Price,
                               PhuPhi = d.PriceService,
                               ThanhTien = d.BfVATamount
                           }).ToList();



            Stream stream = REPORTUTILS.GetReportStream(ReportPath.RptAC_CustomerDebitDetails, results);

            return File(stream, "application/pdf");
        }

        [HttpPost]
        public ActionResult GetDebit(int month, int year, string cusId)
        {

            var data = db.CUS_DEBIT_GETDocuments(month, year).ToList();

            if(!String.IsNullOrEmpty(cusId))
            {
                data = data.Where(p => p.CustomerGroupID.Contains(cusId)).ToList();
            }

            // var data;
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = data
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetDetails(string documentId)
        {
            var results = (from mm in db.MM_Mailers
                           join d in db.AC_CustomerDebitVoucherDetail
                           on mm.MailerID equals d.MailerID
                           join p in db.BS_Provinces
                           on mm.RecieverProvinceID equals p.ProvinceID
                           where d.DocumentID == documentId
                           select new
                           {
                               MailerID = mm.MailerID,
                               AcceptDate = mm.AcceptDate,
                               ReciveprovinceID = p.ProvinceCode,
                               MerchandiseID = mm.MerchandiseID,
                               MailerTypeID = mm.MailerTypeID,
                               Quantity = mm.Quantity,
                               Weight = mm.Weight,
                               Price = d.Price,
                               PriceService = mm.PriceService,
                               Discount = mm.Discount,
                               Amount = mm.Amount,
                               COD = mm.COD
                           }).ToList();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public string getMaxid()
        {
            //lay ra gia tri maxid
            var maxid = db.AC_CustomerDebitVoucher.Where(p => p.PostOfficeID == "BCQ3").OrderByDescending(x => x.DocumentID).FirstOrDefault();
            string maxndg = string.Empty;
            if (maxid != null)
            {
                maxndg = string.Format("BCQ3" + "{0}", (Convert.ToUInt32(maxid.DocumentID.Substring(6)) + 1).ToString("D6"));
            }
            else
            {
                maxndg = "BCQ3" + "000001";
            }
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = maxndg
            };
            return maxndg;
        }


        [HttpPost]
        public ActionResult GetNotDebit(string CusId, string DethTime)
        {

            try
            {

                var denNgay = DateTime.ParseExact(DethTime, "dd/MM/yyyy", null).ToString("yyyy-MM-dd");

                var findCus = db.BS_CustomerGroups.Where(p => p.CustomerGroupCode == CusId).FirstOrDefault();

                if (findCus == null)
                    throw new Exception("Sai thông tin");

                var listMailer = db.get_mailerfordebit(findCus.CustomerGroupID, denNgay).ToList();

                return Json(new { error = 0, data = listMailer }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { error = 1 }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult delete(string documentid)
        {
            if (String.IsNullOrEmpty(documentid))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.AC_CustomerDebitVoucher.Where(p => p.DocumentID == documentid).FirstOrDefault();

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);
            var listup = db.AC_CustomerDebitVoucherDetail.Where(p => p.DocumentID == documentid).Select(p => p.MailerID).ToList();
            foreach (var item in listup)
            {
                var mailerid = db.MM_Mailers.Where(p => p.MailerID == item).FirstOrDefault();
                mailerid.DiscountPercent = 0;
                mailerid.Discount = 0;
                mailerid.IsPayment = 0;
                
                db.Entry(mailerid).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            db.AC_CustomerDebitVoucherDetail.RemoveRange(db.AC_CustomerDebitVoucherDetail.Where(p => p.DocumentID == documentid));
            db.Entry(check).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ThanhToan(string documentid)
        {

            try
            {
                var check = db.AC_CustomerDebitVoucher.Where(p => p.DocumentID == documentid).FirstOrDefault();

                if (check == null)
                    return Json(new ResultInfo() { error = 0, msg = "" }, JsonRequestBehavior.AllowGet);

                check.StatusID = 1; // da thanh toan

                db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var allDetail = db.AC_CustomerDebitVoucherDetail.Where(p => p.DocumentID == documentid).ToList();

                foreach(var item in allDetail)
                {
                    var mailer = db.MM_Mailers.Find(item.MailerID);

                    if(mailer != null)
                    {
                        mailer.IsPayment = 1;
                        db.Entry(mailer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
               
            }
            catch
            {
                return Json(new { error = 1, msg = "Lỗi xác nhận" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = 0}, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Create(string CusId, string DethTime, string Notes, List<string> ListMailers)
        {
            

            try
            {

                var denNgay = DateTime.ParseExact(DethTime, "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                var _tungay = new SqlParameter("@FromDate", "1900.01.01");
                var _denngay = new SqlParameter("@ToDate", denNgay);
                var _post = new SqlParameter("@PostOfficeID", EmployeeInfo.currentPost);
                var _payment = new SqlParameter("@PaymentMethodID", "NGTT");
                var _customerid = new SqlParameter("@CustomerID", CusId);
                var _groupbyrep = new SqlParameter("@GroupByRep", false);
                var _bydebtdate = new SqlParameter("@ByDebtDate", true);
                var data = db.Database.SqlQuery<ReturnValue>("AC_CustomerDebitVoucher_procUpdateDiscountMailer @FromDate,@ToDate,@PostOfficeID,@PaymentMethodID,@CustomerID,@GroupByRep,@ByDebtDate", _tungay, _denngay, _post, _payment, _customerid, _groupbyrep, _bydebtdate).ToList();

                if (ListMailers.Count() == 0)
                    throw new Exception("Chọn mailer");


                var ngayChot = DateTime.ParseExact(DethTime, "dd/MM/yyyy", null);
                var findCus = db.BS_CustomerGroups.Where(p => p.CustomerGroupCode == CusId).FirstOrDefault();

                var allCusChild = db.BS_Customers.Where(p => p.CustomerGroupID == findCus.CustomerGroupID).Select(p => p.CustomerCode).ToList();

                if (findCus == null)
                    throw new Exception("Sai thông tin");
                double totalamount = 0;
                double totalcod = 0;
                string maxid = getMaxid();

                AC_CustomerDebitVoucher mt = new AC_CustomerDebitVoucher();
                mt.DocumentID = maxid;
                mt.DocumentDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                mt.PostOfficeID = EmployeeInfo.currentPost;
                mt.CustomerGroupID = findCus.CustomerGroupCode;
                mt.StatusID = 2;
                mt.ToTalAmount = Math.Round(totalamount, 0);
                mt.CODTotal = totalcod;
                mt.DebtMonth = ngayChot;
                mt.Description = Notes;
                mt.CDay = ngayChot.Day;
                mt.CMonth = ngayChot.Month;
                mt.CYear = ngayChot.Year;

                db.AC_CustomerDebitVoucher.Add(mt);
                db.SaveChanges();

                foreach (var item in ListMailers)
                {
                    decimal? returncost = 0;
                    var findMailer = db.MM_Mailers.Find(item);

                    if (findMailer == null)
                        continue;

                    if (!allCusChild.Contains(findMailer.SenderID))
                        continue;

                    if (findMailer.IsReturn == true)
                    {
                        var checkext = db.MM_MailerServices.Where(p => p.MailerID == findMailer.MailerID).FirstOrDefault();
                        if (checkext == null)
                        {
                            returncost = findMailer.Price / 2; //nếu có chuyển hoàn.
                                                          //thêm vào bảng dịch vụ cộng thêm
                            MM_MailerServices ms = new MM_MailerServices();
                            ms.MailerID = findMailer.MailerID;
                            
                            ms.ServiceID = "CH";
                            ms.SellingPrice = decimal.Parse(returncost.ToString());
                            ms.PriceDefault = 0;
                            ms.IsPercentage = true;
                            ms.BfVATAmount = decimal.Parse((double.Parse(returncost.ToString()) * 0.9).ToString());
                            ms.VATAmount = decimal.Parse(returncost.ToString()) - decimal.Parse((double.Parse(returncost.ToString()) * 0.9).ToString());
                            ms.AfVATAmount = decimal.Parse(returncost.ToString());
                            ms.LastEditDate = DateTime.Now;
                            ms.CreationDate = DateTime.Now;
                            db.MM_MailerServices.Add(ms);
                            //update thu khac vào bảng mm_mailers
                        }

                    }

                    findMailer.PriceService = findMailer.PriceService + decimal.Parse(returncost.ToString());
                    findMailer.IsPayment = 2;
                    db.Entry(findMailer).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    string tttt = findMailer.MailerID;
                    AC_CustomerDebitVoucherDetail ct = new AC_CustomerDebitVoucherDetail();
                    ct.DocumentID = maxid;
                    ct.MailerID = findMailer.MailerID;
                    ct.Amount = double.Parse((findMailer.Amount ?? 0).ToString());
                    ct.Price = decimal.Parse((double.Parse((findMailer.Price ?? 0).ToString()) / 1.1).ToString());
                    ct.PriceService = decimal.Parse((findMailer.PriceService ?? 0).ToString()) + decimal.Parse(returncost.ToString());
                    ct.DiscountPercent = decimal.Parse((findMailer.DiscountPercent ?? 0).ToString());
                    ct.Discount = decimal.Parse((findMailer.Discount ?? 0).ToString());
                    ct.VATpercent = decimal.Parse((findMailer.VATPercent ?? 0).ToString());
                    // ct.BfVATamount = decimal.Parse((item1.BfVATAmount ?? 0).ToString());
                    ct.BfVATamount = decimal.Round(decimal.Parse((findMailer.BefVATAmount ?? 0).ToString()));
                    ct.VATamount = decimal.Round(decimal.Parse((findMailer.VATAmount ?? 0).ToString()));
                    ct.AcceptDate = DateTime.Parse(findMailer.AcceptDate.ToString());
                    ct.Quantity = int.Parse(findMailer.Quantity.ToString());
                    ct.Weight = decimal.Parse(findMailer.Weight.ToString());
                    db.AC_CustomerDebitVoucherDetail.Add(ct);
                    totalamount += double.Parse((findMailer.Amount ?? 0).ToString());
                    totalcod += double.Parse((findMailer.COD ?? 0).ToString());
                    db.SaveChanges();

                }

                mt.ToTalAmount = Math.Round(totalamount, 0);
                mt.CODTotal = totalcod;

                db.Entry(mt).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                return Json(new { error = 0}, JsonRequestBehavior.AllowGet);

            } catch (Exception e)
            {
                return Json(new { error = 1, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}