using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNPOSTAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int IsActivced { get; set; }

        public string FullName { get; set; }

        public string AccountType { get; set; }

        public string GroupId { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}