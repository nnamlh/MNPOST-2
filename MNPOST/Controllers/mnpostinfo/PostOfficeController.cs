using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.mnpostinfo
{
    // postoffice 
    public class PostOfficeController : BaseController
    {

        public ActionResult Show()
        {
            ViewBag.AllProvince = db.BS_Provinces.ToList();
            ViewBag.AllZone = db.BS_Zones.ToList();
            return View();
        }


        [HttpGet]
        public ActionResult GetPostOffice(int? page, string search = "")
        {
            int pageSize = 50;

            int pageNumber = (page ?? 1);


            var data = db.BS_PostOffices.Where(p => p.PostOfficeID.Contains(search) || p.PostOfficeName.Contains(search)).ToList();

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

        [HttpPost]
        public ActionResult create(BS_PostOffices post)
        {

            if (String.IsNullOrEmpty(post.PostOfficeID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_PostOffices.Find(post.ProvinceID);

            if (check != null)
                return Json(new ResultInfo() { error = 1, msg = "Đã tồn tại" }, JsonRequestBehavior.AllowGet);


            post.CreationDate = DateTime.Now;
            db.BS_PostOffices.Add(post);

            db.SaveChanges();

            return Json(new ResultInfo() { error = 0, msg = "", data = post }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult edit(BS_PostOffices post)
        {
            if (String.IsNullOrEmpty(post.PostOfficeID))
                return Json(new ResultInfo() { error = 1, msg = "Missing info" }, JsonRequestBehavior.AllowGet);

            var check = db.BS_PostOffices.Find(post.PostOfficeID);

            if (check == null)
                return Json(new ResultInfo() { error = 1, msg = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);

            check.PostOfficeName = post.PostOfficeName;
            check.Address = post.Address;
            check.ZoneID = post.ZoneID;
            check.ProvinceID = post.ProvinceID;
            check.Phone = post.Phone;
            check.FaxNo = post.FaxNo;
            check.Email = post.Email;
            check.IsCollaborator = post.IsCollaborator;
            check.Notes = post.Notes;
            check.LastEditDate = DateTime.Now;
            check.TaxCode = post.TaxCode;
            check.BankAccount = post.BankAccount;
            check.BankName = post.BankName;
            check.Type = post.Type;


            db.Entry(check).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();


            return Json(new ResultInfo() { error = 0, msg = "", data = check }, JsonRequestBehavior.AllowGet);

        }

	}
}