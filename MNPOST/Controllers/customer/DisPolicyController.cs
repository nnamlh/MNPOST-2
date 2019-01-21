using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOST.Models;
using MNPOSTCOMMON;
using System.Globalization;
namespace MNPOST.Controllers.customer
{
    public class DisPolicyController : BaseController
    {
        // GET: DisPolicy
        public ActionResult Show()
        {
            ViewBag.allPost = db.BS_PostOffices.ToList();
            ViewBag.allService = db.BS_ServiceTypes.ToList();
            ViewBag.allCustomer = db.BS_CustomerGroups.ToList();
            ViewBag.allSV = db.BS_Services.ToList();
            return View();
        }
        [HttpGet]
        public ActionResult GetDiscountPolicy(string search = "")
        {
            var data = db.MM_DiscountPolicys.ToList();
            // var data;
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = data
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult CheckDV(string discountid)
        {
            var chiTiet = db.MM_DiscountPolicyService.Where(p => p.DiscountID == discountid).Select(p => new { ServiceID = p.ServiceID }).ToList();
            return Json(chiTiet, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult CheckKH(string discountid)
        {
            var chiTiet = db.MM_DiscountPolicyCustomer.Where(p => p.DiscountID == discountid).Select(p => new { CustomerID = p.CustomerID }).ToList();
            return Json(chiTiet, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult CheckPT(string discountid)
        {
            var chiTiet = db.MM_DiscountPolicyMethod.Where(p => p.DiscountID == discountid).Select(p => new { ServiceID = p.ServiceID }).ToList();
            return Json(chiTiet, JsonRequestBehavior.AllowGet);
        }
        [HttpGet] // khong su dung
        public ActionResult getDinhMuc(string discountid)
        {
            var chiTiet = db.MM_DiscountPolicyDinhMuc.Where(p => p.DiscountID == discountid).Select(p => new { BatDau = p.ValueBegin, KetThuc = p.ValueEnd, TiLe = p.DiscountPercent }).ToList();
            return Json(chiTiet, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetDiscountPolicyDinhMuc(string discountid)
        {
            var data = db.MM_DiscountPolicyDinhMuc.Where(p => p.DiscountID == discountid).Select(p =>
                new
                {
                    BatDau = p.ValueBegin,
                    KetThuc = p.ValueEnd,
                    TiLe = p.DiscountPercent
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
        [HttpGet]
        public ActionResult GetDiscountPolicyDichVu(string discountid)
        {
            var data = db.MM_DiscountPolicyService.Where(p => p.DiscountID == discountid).ToList();
            // var data;
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = data
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetDiscountPolicyKhachHang(string discountid)
        {
            var data = db.MM_DiscountPolicyCustomer.Where(p => p.DiscountID == discountid).ToList();
            // var data;
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = data
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult getMaxId(string postid)
        {
            var maxid = db.MM_DiscountPolicys.Where(p => p.PostOfficeID == postid).OrderByDescending(x => x.DiscountID).FirstOrDefault();
            string maxndg = string.Empty;
            if (maxid != null)
            {
                maxndg = string.Format(postid + "{0}", (Convert.ToUInt32(maxid.DiscountID.Substring(4)) + 1).ToString("D4"));
            }
            else
            {
                maxndg = postid + "0001";
            }
            ResultInfo result = new ResultInfo()
            {
                error = 0,
                msg = "",
                data = maxndg
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult create(string tuNgay, string denNgay, string ngayTao, IdentityDiscountPolicy csck, HTCoDinh htcd, List<HTDinhMuc> htdm, List<string> dv, List<string> cus, List<string> pt)
        {

            if (String.IsNullOrEmpty(tuNgay))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);
            // 20/12/2018
            string tngay = tuNgay.Substring(0, 2);
            string tthang = tuNgay.Substring(3, 2);
            string tnam = tuNgay.Substring(6, 4);

            string dngay = denNgay.Substring(0, 2);
            string dthang = denNgay.Substring(3, 2);
            string dnam = denNgay.Substring(6, 4);

            string cngay = ngayTao.Substring(0, 2);
            string cthang = ngayTao.Substring(3, 2);
            string cnam = ngayTao.Substring(6, 4);

            string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

            //format sever M/d/yyyy
            DateTime dateFrom;
            DateTime dateTo;
            DateTime dateTao;
            if (sysFormat == "M/d/yyyy")
            {
                dateFrom = DateTime.Parse(tthang + '/' + tngay + '/' + tnam);
                dateTo = DateTime.Parse(dthang + '/' + dngay + '/' + dnam);
                dateTao = DateTime.Parse(cthang + '/' + cngay + '/' + cnam);
            }
            else
            {
                dateFrom = DateTime.Parse(tuNgay);
                dateTo = DateTime.Parse(denNgay);
                dateTao = DateTime.Parse(ngayTao);
            }


            var check = db.MM_DiscountPolicys.Where(p => p.DiscountID == csck.DiscountID).FirstOrDefault();

            if (check != null)
                return Json(new ResultInfo() { error = 1, msg = "Đã tồn tại" }, JsonRequestBehavior.AllowGet);

            MM_DiscountPolicys cs = new MM_DiscountPolicys();
            cs.DiscountID = csck.DiscountID;
            cs.PostOfficeID = csck.PostOfficeID;
            cs.CreateDate = dateTao;
            cs.StartDate = dateFrom;
            cs.EndDate = dateTo;
            cs.EditDate = DateTime.Now;
            cs.StatusID = csck.StatusID;
            cs.PermanentCal = bool.Parse(csck.PermanentCal);
            cs.AllCustomer = bool.Parse(csck.AllCustomer);
            cs.AllService = bool.Parse(csck.AllService);
            cs.UserCreate = "LOIVV";
            cs.Description = csck.Description;
            cs.AllMethod = csck.AllMethod;
            // cs.DiscountPercent = csck.DiscountPercent;
            // cs.LimitValue = csck.LimitValue;


            //db.SaveChanges();
            //kiem tra các truong hop lưu bảng riêng
            //co dinh hoac dinh muc
            if (csck.PermanentCal == "true")
            {

                cs.LimitValue = csck.LimitValue;
                cs.DiscountPercent = csck.DiscountPercent;

            }
            else
            {
                foreach (var item in htdm)
                {
                    MM_DiscountPolicyDinhMuc dm = new MM_DiscountPolicyDinhMuc();
                    dm.DiscountID = csck.DiscountID;
                    dm.ValueBegin = item.BatDau;
                    dm.ValueEnd = item.KetThuc;
                    dm.DiscountPercent = item.TiLe;
                    db.MM_DiscountPolicyDinhMuc.Add(dm);
                }

            }
            //tat ca dich vu
            if (csck.AllService == "false")
            {
                foreach (var item in dv)
                {
                    MM_DiscountPolicyService sv = new MM_DiscountPolicyService();
                    sv.DiscountID = csck.DiscountID;
                    sv.ServiceID = item;
                    db.MM_DiscountPolicyService.Add(sv);
                    // db.SaveChanges();
                }
            }
            //tat ca khách hàng
            if (csck.AllCustomer == "false")
            {
                foreach (var item in cus)
                {
                    MM_DiscountPolicyCustomer kh = new MM_DiscountPolicyCustomer();
                    kh.DiscountID = csck.DiscountID;
                    kh.CustomerID = item;
                    db.MM_DiscountPolicyCustomer.Add(kh);
                }
            }
            if (csck.AllMethod == 0 || csck.AllMethod == 2)
            {
                foreach (var item in pt)
                {
                    MM_DiscountPolicyMethod mt = new MM_DiscountPolicyMethod();
                    mt.DiscountID = csck.DiscountID;
                    mt.ServiceID = item;
                    db.MM_DiscountPolicyMethod.Add(mt);
                }
            }
            db.MM_DiscountPolicys.Add(cs);
            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = sysFormat }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult edit(string tuNgay, string denNgay, string ngayTao, IdentityDiscountPolicy csck, HTCoDinh htcd, List<HTDinhMuc> htdm, List<string> dv, List<string> cus, List<string> pt)
        {
            if (String.IsNullOrEmpty(tuNgay))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);
            // 20/12/2018
            string tngay = tuNgay.Substring(0, 2);
            string tthang = tuNgay.Substring(3, 2);
            string tnam = tuNgay.Substring(6, 4);

            string dngay = denNgay.Substring(0, 2);
            string dthang = denNgay.Substring(3, 2);
            string dnam = denNgay.Substring(6, 4);

            string cngay = ngayTao.Substring(0, 2);
            string cthang = ngayTao.Substring(3, 2);
            string cnam = ngayTao.Substring(6, 4);

            string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

            //format sever M/d/yyyy
            DateTime dateFrom;
            DateTime dateTo;
            DateTime dateTao;
            if (sysFormat == "M/d/yyyy")
            {
                dateFrom = DateTime.Parse(tthang + '/' + tngay + '/' + tnam);
                dateTo = DateTime.Parse(dthang + '/' + dngay + '/' + dnam);
                dateTao = DateTime.Parse(cthang + '/' + cngay + '/' + cnam);
            }
            else
            {
                dateFrom = DateTime.Parse(tuNgay);
                dateTo = DateTime.Parse(denNgay);
                dateTao = DateTime.Parse(ngayTao);
            }


            var check = db.MM_DiscountPolicys.Where(p => p.DiscountID == csck.DiscountID).FirstOrDefault();

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            check.CreateDate = dateTao;
            check.StartDate = dateFrom;
            check.EndDate = dateTo;
            check.EditDate = DateTime.Now;
            check.StatusID = csck.StatusID;
            check.PermanentCal = bool.Parse(csck.PermanentCal);
            check.AllCustomer = bool.Parse(csck.AllCustomer);
            check.AllService = bool.Parse(csck.AllService);
            check.UserCreate = "LOIVV";
            check.Description = csck.Description;
            check.AllMethod = csck.AllMethod;
            db.Entry(check).State = System.Data.Entity.EntityState.Modified;
            // db.SaveChanges();
            //kiem tra các truong hop lưu bảng riêng
            //co dinh hoac dinh muc
            //xoa cac thong tin chi tiet luu truoc do
            db.MM_DiscountPolicyMethod.RemoveRange(db.MM_DiscountPolicyMethod.Where(p => p.DiscountID == csck.DiscountID));
            db.MM_DiscountPolicyDinhMuc.RemoveRange(db.MM_DiscountPolicyDinhMuc.Where(p => p.DiscountID == csck.DiscountID));
            db.MM_DiscountPolicyService.RemoveRange(db.MM_DiscountPolicyService.Where(p => p.DiscountID == csck.DiscountID));
            db.MM_DiscountPolicyCustomer.RemoveRange(db.MM_DiscountPolicyCustomer.Where(p => p.DiscountID == csck.DiscountID));
            if (csck.PermanentCal == "true")
            {
                check.LimitValue = csck.LimitValue;
                check.DiscountPercent = csck.DiscountPercent;

            }
            else
            {

                foreach (var item in htdm)
                {
                    MM_DiscountPolicyDinhMuc dm = new MM_DiscountPolicyDinhMuc();
                    dm.DiscountID = csck.DiscountID;
                    dm.ValueBegin = item.BatDau;
                    dm.ValueEnd = item.KetThuc;
                    dm.DiscountPercent = item.TiLe;
                    db.MM_DiscountPolicyDinhMuc.Add(dm);
                }

            }
            //tat ca dich vu
            if (csck.AllService == "false")
            {

                foreach (var item in dv)
                {
                    MM_DiscountPolicyService sv = new MM_DiscountPolicyService();
                    sv.DiscountID = csck.DiscountID;
                    sv.ServiceID = item;
                    db.MM_DiscountPolicyService.Add(sv);
                    //db.SaveChanges();
                }
            }
            //tat ca khách hàng
            if (csck.AllCustomer == "false")
            {

                foreach (var item in cus)
                {
                    MM_DiscountPolicyCustomer kh = new MM_DiscountPolicyCustomer();
                    kh.DiscountID = csck.DiscountID;
                    kh.CustomerID = item;
                    db.MM_DiscountPolicyCustomer.Add(kh);
                }
            }
            //
            if (csck.AllMethod == 0 || csck.AllMethod == 2)
            {
                foreach (var item in pt)
                {
                    MM_DiscountPolicyMethod mt = new MM_DiscountPolicyMethod();
                    mt.DiscountID = csck.DiscountID;
                    mt.ServiceID = item;
                    db.MM_DiscountPolicyMethod.Add(mt);
                }
            }
            db.SaveChanges();
            return Json(new ResultInfo() { error = 0, msg = "", data = csck }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult delete(string discountid)
        {
            if (String.IsNullOrEmpty(discountid))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.MM_DiscountPolicys.Where(p => p.DiscountID == discountid).FirstOrDefault();

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);
            db.MM_DiscountPolicyCustomer.RemoveRange(db.MM_DiscountPolicyCustomer.Where(p => p.DiscountID == discountid));
            db.MM_DiscountPolicyDinhMuc.RemoveRange(db.MM_DiscountPolicyDinhMuc.Where(p => p.DiscountID == discountid));
            db.MM_DiscountPolicyService.RemoveRange(db.MM_DiscountPolicyService.Where(p => p.DiscountID == discountid));
            db.MM_DiscountPolicyMethod.RemoveRange(db.MM_DiscountPolicyMethod.Where(p => p.DiscountID == discountid));
            db.Entry(check).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);
        }
    }
}