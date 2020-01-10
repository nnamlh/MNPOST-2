using System;
using System.Linq;
using System.Web.Mvc;
using MNPOSTCOMMON;

namespace MNPOST.Controllers.mailer
{
    public class MailerReturnController : BaseController
    {
        // GET: MailerReturn
        public ActionResult Index()
        {

            ViewBag.PostOffices = EmployeeInfo.postOffices;

            return View();
        }

        [HttpPost]
        public ActionResult GetData(string postId)
        {

            var data = db.MAILER_RETURN_GET_NOT_ACCEPT("%" + postId + "%").ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddMailers(string mailerId, string postId)
        {

            var findMailer = db.MM_Mailers.Find(mailerId);

            if (findMailer == null)
            {
                return Json(new { error = 1, msg = "Sai thông tin" }, JsonRequestBehavior.AllowGet);
            }


            if (findMailer.CurrentStatusID == 5 || findMailer.CurrentStatusID == 6)
            {

                if (findMailer.IsPostAccept == false)
                {
                    findMailer.IsPostAccept = true;
                    findMailer.LastUpdateDate = DateTime.Now;
                    db.Entry(findMailer).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    var history = new MailerReturnAcceptHistory()
                    {
                        CreateTime = DateTime.Now,
                        EmployeeActionId = EmployeeInfo.employeeId,
                        EmployeeDeliveryId = "",
                        Id = Guid.NewGuid().ToString(),
                        MailerId = mailerId,
                        PostId = postId,
                        StatusId = findMailer.CurrentStatusID,
                        UserAction = User.Identity.Name
                    };
                    db.MailerReturnAcceptHistories.Add(history);
                    db.SaveChanges();

                }
            }

            return Json(new { error = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}