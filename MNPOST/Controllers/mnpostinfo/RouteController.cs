using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MNPOSTCOMMON;
using MNPOST.Models;
namespace MNPOST.Controllers.mnpostinfo
{
    public class RouteController : BaseController
    {
        // GET: Route
        public ActionResult Show()
        {
            ViewBag.AllProvince = db.BS_Provinces.Select(p => new CommonData() { code = p.ProvinceID, name = p.ProvinceName }).ToList();
            ViewBag.AllPost = EmployeeInfo.postOffices;
            return View();
        }

        //
        [HttpGet]
        public ActionResult GetProvinces(string parentId, string type)
        {
            return Json(GetProvinceDatas(parentId, type), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetEmployeeRoutes(string postId)
        {
            var data = db.ROUTE_GET_ALLEMPLOYEE_ROUTE(postId).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDistrictRoutes(string provinceId, string employeeId, string type)
        {

            var checkEmployee = db.BS_Employees.Find(employeeId);

            if (checkEmployee == null)
                return Json(new ResultInfo()
                {
                    error = 1,
                    msg = "Sai nhân viên"
                }, JsonRequestBehavior.AllowGet);

            var allDistrict = db.BS_Districts.Where(p => p.ProvinceID == provinceId).ToList();

            List<ERouteInfo> districtRoutes = new List<ERouteInfo>();

            foreach (var item in allDistrict)
            {
                var findStaffRoutes = db.BS_Routes.Where(p => p.ProvinceID == provinceId && p.DistrictID == item.DistrictID && p.Type == type).ToList();

                var routeInfo = new ERouteInfo()
                {
                    DistrictID = item.DistrictID,
                    DistrictName = item.DistrictName,
                    ProvinceID = provinceId,
                    Staffs = new List<CommonData>(),
                    Type = type,
                    ISJoin = false
                };

                foreach (var staff in findStaffRoutes)
                {
                    var checkStaff = db.BS_Employees.Find(staff.EmployeeID);

                    if (checkStaff != null && checkStaff.PostOfficeID == checkEmployee.PostOfficeID)
                    {
                        routeInfo.Staffs.Add(new CommonData()
                        {
                            code = checkStaff.EmployeeID,
                            name = checkStaff.EmployeeName
                        });

                        if (checkStaff.EmployeeID == employeeId)
                        {
                            routeInfo.ISJoin = true;
                        }
                    }
                }

                districtRoutes.Add(routeInfo);
            }


            return Json(new ResultInfo()
            {
                error = 0,
                msg = "",
                data = districtRoutes
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetWardRoutes( string districtId, string employeeId, string type)
        {
            List<ERouteDetail> result = new List<ERouteDetail>();

            var check = db.ROUTE_GETWARD(type, districtId).ToList();

            foreach(var item in check)
            {
                var data = new ERouteDetail()
                {
                    DistrictID = item.DistrictID,
                    WardID = item.WardID,
                    WardName = item.WardName,
                    ISJoin = item.EmployeeID == employeeId ?true:false,
                    Staff = new CommonData { name = item.EmployeeName, code = item.EmployeeID }
                };

                if(String.IsNullOrEmpty(item.RouteID))
                {
                    data.IsChoose = true;
                } else
                {
                    if(data.ISJoin)
                    {
                        data.IsChoose = true;
                    } else
                    {
                        data.IsChoose = false;
                    }
                }

                result.Add(data);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddWardRoute (ERouteDetail detail, string employeeId, string provinceId, string type)
        {

            var findRoute = db.BS_Routes.Where(p => p.Type == type && p.EmployeeID == employeeId && p.ProvinceID == provinceId && p.DistrictID == detail.DistrictID).FirstOrDefault();

            if(findRoute != null)
            {
                var findWard = db.BS_RouteDetails.Where(p => p.WardID == detail.WardID && p.RouteID == findRoute.RouteID).FirstOrDefault();

                if(detail.ISJoin)
                {
                    // tham gia
                    if(findWard == null)
                    {
                        var data = new BS_RouteDetails()
                        {
                            RouteID = findRoute.RouteID,
                            WardID = detail.WardID
                        };
                        db.BS_RouteDetails.Add(data);
                        db.SaveChanges();

        
                    } else
                    {
                        // xoa
                        if(findWard != null)
                        {
                            db.BS_RouteDetails.Remove(findWard);
                            db.SaveChanges();
                        }
                    }
                }
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddDistrictRoutes(ERouteInfo info, string employeeId)
        {
            var getAll = db.BS_Routes.Where(p => p.ProvinceID == info.ProvinceID && p.Type == info.Type && p.DistrictID == info.DistrictID).ToList();

            var check = getAll.Where(p => p.EmployeeID == employeeId).FirstOrDefault();

            if(info.ISJoin)
            {
                // them
                if (check == null)
                {

                    var routes = new BS_Routes()
                    {
                        RouteID = Guid.NewGuid().ToString(),
                        DistrictID = info.DistrictID,
                        EmployeeID = employeeId,
                        IsDetail = getAll.Count() > 0?true:false,
                        ProvinceID = info.ProvinceID,
                        Type = info.Type
                    };

                    db.BS_Routes.Add(routes);
                    db.SaveChanges();

                    foreach(var item in getAll)
                    {
                        item.IsDetail = true;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            } else
            {
                // remove
                if (check != null)
                {
                    db.BS_Routes.Remove(check);
                    

                    var findWards = db.BS_RouteDetails.Where(p => p.RouteID == check.RouteID).ToList();
                    findWards.Clear();
                    db.SaveChanges();


                    getAll.Remove(check);
                   
                    if(getAll.Count() == 1)
                    {
                        var firstCheck = getAll[0];
                        firstCheck.IsDetail = false;
                        db.Entry(firstCheck).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            return Json(new ResultInfo() { error = 0, msg = "" }, JsonRequestBehavior.AllowGet);

        }
    }
}