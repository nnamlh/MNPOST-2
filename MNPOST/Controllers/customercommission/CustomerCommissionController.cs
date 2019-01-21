using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOST.Models;
using MNPOSTCOMMON;
using System.Globalization;
using System.Data.SqlClient;
namespace MNPOST.Controllers.customercommission
{
    public class CustomerCommissionController : BaseController
    {
        // GET: CustomerCommission
        public ActionResult Show()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetCom(string tungay, string denngay, string post)
        {

            string tngay = tungay.Substring(0, 2);
            string tthang = tungay.Substring(3, 2);
            string tnam = tungay.Substring(6, 4);

            string dngay = denngay.Substring(0, 2);
            string dthang = denngay.Substring(3, 2);
            string dnam = denngay.Substring(6, 4);

            string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

            //format sever M/d/yyyy
            DateTime dateFrom;
            DateTime dateTo;

            if (sysFormat == "M/d/yyyy")
            {
                dateFrom = DateTime.Parse(tthang + '/' + tngay + '/' + tnam);
                dateTo = DateTime.Parse(dthang + '/' + dngay + '/' + dnam);
            }
            else
            {
                dateFrom = DateTime.Parse(tungay);
                dateTo = DateTime.Parse(denngay);
            }
            var data = (from cdv in db.AC_CommissionOffer
                        join cg in db.BS_CustomerGroups
                        on cdv.CustomerID equals cg.CustomerGroupCode
                        where cdv.PostOfficeID == post && cdv.DocumentDate >= dateFrom && cdv.DocumentDate <= dateTo

                        select new
                        {
                            DocumentID = cdv.DocumentID,
                            DocumentDate = cdv.DocumentDate,
                            CustomerGroupID = cdv.CustomerID,
                            CustomerName = cg.CustomerGroupName,
                            TotalCommissionAmount = cdv.TotalCommissionAmount

                        }).ToList();
            // var data;
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = data
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public string getMaxid()
        {
            //lay ra gia tri maxid
            var maxid = db.AC_CommissionOffer.Where(p => p.PostOfficeID == "BCQ3").OrderByDescending(x => x.DocumentID).FirstOrDefault();
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
        public ActionResult create(string thangchot)
        {
            string maxid = string.Empty;
            string tthang = thangchot.Substring(0, 2);
            string tnam = thangchot.Substring(3, 4);


            var post = new SqlParameter("@Post", "BCQ3");
            var debtmonth = new SqlParameter("@DebtMonth", tthang);
            var debtyear = new SqlParameter("@DebtYear", tnam);
            var listcus = db.Database.SqlQuery<CusCom>("get_customerforcommission @Post,@DebtMonth,@DebtYear", post, debtmonth, debtyear).ToList();
            foreach (var item in listcus)
            {
                    var debtmonth0 = new SqlParameter("@DebtMonth", tthang);
                    var debtyear0 = new SqlParameter("@DebtYear", tnam);
                    var post0 = new SqlParameter("@Post", "BCQ3");
                    var cus0 = new SqlParameter("@CustID", item.CustomerGroupCode);
                    var listrt = db.Database.SqlQuery<ReturnValue>("AC_CommissionOffer_procUpdateComissionMailer @DebtMonth,@DebtYear,@Post,@CustID", debtmonth0, debtyear0, post0, cus0).ToList();

                decimal? total = 0;
                decimal? comamount = 0;
                maxid = getMaxid();
                //lay ra max id
                //insert bang chi tiet
                var post1 = new SqlParameter("@Post", "BCQ3");
                var debtmonth1 = new SqlParameter("@DebtMonth", tthang);
                var debtyear1 = new SqlParameter("@DebtYear", tnam);
                var customerid = new SqlParameter("@CustomerGroupID", item.CustomerGroupCode);
                var listcg = db.Database.SqlQuery<MailerCom>("get_commissiondetaildata @Post,@DebtMonth,@DebtYear,@CustomerGroupID", post1, debtmonth1, debtyear1, customerid).ToList();

                foreach (var item1 in listcg)
                {
                    AC_CommissionOfferDetail dt = new AC_CommissionOfferDetail();
                    dt.DocumentID = maxid;
                    dt.MailerID = item1.MailerID;
                    dt.Amount = double.Parse(item1.Amount.ToString());
                    dt.CommissionAmount = double.Parse(item1.CommissionAmt.ToString());
                    dt.ComPercent = double.Parse(item1.CommissionPercent.ToString());
                    dt.CreationDate = DateTime.Now;
                    db.AC_CommissionOfferDetail.Add(dt);
                    total += item1.Amount;
                    comamount += item1.CommissionAmt;
                }

                //insert bang master
                AC_CommissionOffer ms = new AC_CommissionOffer();
                ms.DocumentID = maxid;
                ms.DocumentDate = DateTime.Now.Date;
                ms.PostOfficeID = "BCQ3";
                ms.CommMonth = DateTime.Parse(thangchot);
                ms.CustomerID = item.CustomerGroupCode;
                ms.TotalAmount = double.Parse(total.ToString());
                ms.TotalAmt4Commission = 0;
                ms.TotalCommissionAmount = double.Parse(comamount.ToString());
                ms.VATPercent = 10;
                ms.VATAmount = double.Parse(comamount.ToString()) * 0.1;
                db.AC_CommissionOffer.Add(ms);


            }
            db.SaveChanges();
            return Json(new ResultInfo() { error = 0, msg = "", data = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult getDetail(string documentid)
        {
            var results = (from mm in db.MM_Mailers
                           join d in db.AC_CommissionOfferDetail
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
                               CommissionAmount = mm.CommissionAmt,
                               CommissionPercent = mm.CommissionPercent
                           }).ToList();
            //var chiTiet = db.AC_CustomerDebitVoucherDetail.Where(p => p.DocumentID == documentid).ToList();
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult delete(string documentid)
        {
            if (String.IsNullOrEmpty(documentid))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.AC_CommissionOffer.Where(p => p.DocumentID == documentid).FirstOrDefault();

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);
            db.AC_CommissionOfferDetail.RemoveRange(db.AC_CommissionOfferDetail.Where(p => p.DocumentID == documentid));

            db.Entry(check).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);
        }
    }
}