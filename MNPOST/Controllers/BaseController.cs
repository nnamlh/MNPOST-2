using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MNPOST.Models;
using MNPOSTCOMMON;
using MNPOST.Utils;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using MNPOST.Report;

namespace MNPOST.Controllers
{

    [Authorize(Roles = "user")]
    public class BaseController : Controller
    {
        protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected SqlConnection conn;
        protected UserInfo EmployeeInfo;
        protected ReportUtils REPORTUTILS = new ReportUtils();
        protected MNPOSTEntities db = new MNPOSTEntities();

        protected RoleManager<IdentityRole> RoleManager { get; private set; }

        protected UserManager<ApplicationUser> UserManager { get; private set; }
        private ApplicationDbContext sdb = new ApplicationDbContext();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var checkUser = db.AspNetUsers.Where(p => p.UserName == requestContext.HttpContext.User.Identity.Name).FirstOrDefault();
                EmployeeInfo = new UserInfo()
                {
                    user = checkUser.UserName,
                    groupId = checkUser.GroupId,
                    level = checkUser.ULevel,
                    postOffices = new List<string>()
                };

                var checkStaff = db.BS_Employees.Where(p => p.UserLogin == checkUser.UserName).FirstOrDefault();

                if (checkStaff != null)
                {
                    EmployeeInfo.currentPost = checkStaff.PostOfficeID;
                    EmployeeInfo.employeeId = checkStaff.EmployeeID;
                    EmployeeInfo.fullName = checkStaff.EmployeeName;
                    EmployeeInfo.postOffices = GetPostPermit(EmployeeInfo.level, EmployeeInfo.currentPost);
                }
            }
        }
        public BaseController()
        {
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(sdb));
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(sdb));
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }



        protected bool checkAccess(string menu, int edit)
        {
            var user = User.Identity.Name;

            var roles = db.USER_GETROLE(user).ToList();


            if (roles == null || roles.Count() == 0)
                return false;

            var checkAdmin = roles.Where(p => p.Name == "admin").FirstOrDefault();

            if (checkAdmin != null)
                return true;


            var groupId = roles.First().GroupId;

            var getAccess = db.USER_CHECKACCESS(groupId, menu).FirstOrDefault();

            if (getAccess == null)
                return false;



            if (getAccess.CanEdit == edit)
                return true;


            return false;
        }

        protected bool checkAccess(string menu)
        {
            var user = User.Identity.Name;

            var roles = db.USER_GETROLE(user).ToList();


            if (roles == null || roles.Count() == 0)
                return false;

            var checkAdmin = roles.Where(p => p.Name == "admin").FirstOrDefault();

            if (checkAdmin != null)
                return true;


            var groupId = roles.First().GroupId;

            var getAccess = db.USER_CHECKACCESS(groupId, menu).FirstOrDefault();

            if (getAccess != null)
                return true;

            return false;
        }


        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpGet]
        public ActionResult Menus()
        {
            var user = User.Identity.Name;

            var getMenu = db.USER_GETMENU(user).ToList();

            List<GroupMenuInfo> groupMenus = new List<GroupMenuInfo>();

            var listGroup = db.UMS_GroupMenu.OrderBy(p => p.Position).ToList();

            foreach (var item in listGroup)
            {
                GroupMenuInfo groupInfo = new GroupMenuInfo()
                {
                    name = item.Name,
                    icon = item.Icon,
                    menus = new List<MenuInfo>()
                };

                var listMenu = getMenu.Where(p => p.GroupMenuId == item.Id).OrderBy(p => p.Position).ToList();

                if (listMenu.Count() > 0)
                {
                    foreach (var menuItem in listMenu)
                    {
                        groupInfo.menus.Add(new MenuInfo()
                        {
                            name = menuItem.Name,
                            link = menuItem.Link
                        });
                    }
                    groupMenus.Add(groupInfo);
                }
            }

            return PartialView("_MenuUser", groupMenus);
        }


        private List<string> GetPostPermit(string lv, string currentPost)
        {
            List<string> result = new List<string>();

            // toan
            if (lv == "HOST")
                return db.BS_PostOffices.Select(p => p.PostOfficeID).ToList();


            // local
            if (lv == "LOCAL")
            {
                result.Add(currentPost);
                return result;
            }

            // option
            return db.UserPostOptions.Where(p => p.TUser == User.Identity.Name).Select(p => p.TPostId).ToList();
        }

        //
        public List<CommonData> GetProvinceDatas(string parentId, string type)
        {
            if (type == "district")
            {
                return db.BS_Districts.Where(p => p.ProvinceID == parentId).Select(p => new CommonData()
                {
                    code = p.DistrictID,
                    name = p.DistrictName
                }).ToList();
            }
            else if (type == "ward")
            {
                return db.BS_Wards.Where(p => p.DistrictID == parentId).Select(p => new CommonData()
                {
                    code = p.WardID,
                    name = p.WardName
                }).ToList();
            }
            else if (type == "province")
            {
                return db.BS_Provinces.Select(p => new CommonData()
                {
                    code = p.ProvinceID,
                    name = p.ProvinceName
                }).ToList();
            }
            else
            {
                return new List<CommonData>();
            }

        }


        // return sql adapter
        protected SqlDataAdapter GetSqlDataAdapter(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
          
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            return da;
        }

        protected SqlDataAdapter GetSqlDataAdapter(string store, Dictionary<string, string> parameter)
        {
            SqlCommand cmd = new SqlCommand(store, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            if(parameter != null)
            {
                foreach(var item in parameter)
                {
                    cmd.Parameters.Add(new SqlParameter(item.Key, SqlDbType.Text)).Value = item.Value;
                }
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            return da;
        }


        // get data
        // nhan vien dang hoat dong
        protected List<CommonData> GetEmployees (string postID)
        {
            return db.BS_Employees.Where(p=> p.PostOfficeID == postID && p.IsActive == true).Select(p => new CommonData()
            {
                code = p.EmployeeID,
                name = p.EmployeeName
            }).ToList();

        }

        protected List<CommonData> GetCustomers(string postID)
        {

            return db.BS_Customers.Where(p => p.PostOfficeID == postID && p.IsActive == true).Select(p => new CommonData()
            {
                code = p.CustomerID,
                name = p.CustomerName
            }).ToList();

        }

        protected List<EmployeeInfoCommon> GetEmployeeByPost(string postId)
        {
            return db.BS_Employees.Where(p => p.PostOfficeID == postId && p.IsActive == true).Select(p => new EmployeeInfoCommon()
            {
                code = p.EmployeeID,
                name = p.EmployeeName,
                email = p.Email,
                phone = p.Phone
            }).ToList();
        }
    }
}