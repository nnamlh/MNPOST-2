using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MNPOST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNPOST.Controllers.manage
{

    [Authorize(Roles = "admin")]
    public class AdminController : BaseController
    {


     //   public RoleManager<IdentityRole> RoleManager { get; private set; }

        private UserManager<ApplicationUser> userManager;


        public AdminController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
          //  RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        }


        protected void AddRole (string userId, string roleName)
        {
            userManager.AddToRole(userId, roleName);
        }

        protected void RemoveRole(string userId, string roleName)
        {
            userManager.RemoveFromRole(userId, roleName);
        }
	}
}