using MNPOST.Models;
using MNPOSTCOMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNPOST.Filters
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MyValidateAccess : ActionFilterAttribute
    {

        protected MNPOSTEntities db = new MNPOSTEntities();

        public string code { get; set; }

        public int edit { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string userName = HttpContext.Current.User.Identity.Name;

                var roles = db.USER_GETROLE(userName).ToList();

                if (roles == null || roles.Count() == 0)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = new ResultInfo()
                            {
                                error = 1,
                                msg = "Bạn không có quyền truy cập"
                            }
                        };
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("~/error/relogin");
                    }
                    return;
                }


                var checkAdmin = roles.Where(p => p.Name == "admin").FirstOrDefault();

                if (checkAdmin != null)
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }


                var groupId = roles.First().GroupId;

                var getAccess = db.USER_CHECKACCESS(groupId, code).FirstOrDefault();

                if (getAccess == null)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = new ResultInfo()
                            {
                                error = 1,
                                msg = "Bạn không có quyền truy cập"
                            }
                        };
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("~/error/relogin");
                    }
                    return;
                }

                if (getAccess.CanEdit == edit)
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }
            }

        }
    }
}